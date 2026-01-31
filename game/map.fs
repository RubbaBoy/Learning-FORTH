require structs.fs
require str-utils.fs

\ : my-word
\    [ s" data.txt" slurp-file ] 2literal ( compiled as constants )
\    type ;

33 CONSTANT MAP-HEADER-LENGTH \ Row for coords with newline
32 CONSTANT map-width
32 CONSTANT map-height

map-width map-height * map-height + MAP-HEADER-LENGTH + CONSTANT map-bytes  \ total bytes the file of a map should be (with newlines)

container ActiveMap
    | mx
    | my
    | map-addr
;container

ActiveMap new active-map

variable buffer-map-addr 0 ,

\ ==== Storing maps ====

\ format: ( link packed-coords map-addr )
variable maps 0 , \ A dictionary of all maps

cell 8 2 */ constant pack-size
pack-size 1 swap lshift 1- constant pack-mask

\ Pack coords to a single cell key
\ mx, my = point x/y
: pack-coords ( mx my -- packed-coords )
    pack-size lshift
    swap
    pack-mask and
    or ;

: unpack-coords-x ( packed-coords -- mx )
    pack-mask and ;

: unpack-coords-y ( packed-coords -- my )
    pack-size rshift ;

\ Data storage:
: add-map ( mx my map-addr -- )
    HERE
    maps @ ,  \ read `maps` and store in the next dictionary address' cell
    ( mx my map-addr HERE )
    2swap
    pack-coords , ( map-addr HERE )
    swap ,   \ store map-addr
    maps ! ; \ Update head variable to point to the new map

: find-map ( mx my -- map-addr|0 )
    pack-coords
    maps @
    BEGIN
        dup
    WHILE
        dup cell+ @ \ get key from current
        2 pick \ get 3rd value down, i.e. ( *packed-coords* map-link packed-coords' )
        = IF
            nip \ remove packed-coords, only map-link remains
            2 cells + @ \ get the map-addr for this entry
            EXIT
        THEN
        @  \ This is still the head of the map, so go to the actual next map-link
    REPEAT
    nip \ remove key if not found, will return the map-link of 0
    ;


\ ==== Creating maps ====

\     cols
\     0 1 2  3
\     --------
\  0 |0 4 8  12
\  1 |1 5 9  13
\  2 |2 6 10 14
\  3 |3 7 11 15

: make-map-array ( -- map-addr )
    here map-width map-height * allot ;

: get-map-addr-at ( map-addr x y -- coord-addr )
    swap ( map-addr y x )
    map-height * ( map-addr y col-index )
    + + ; ( coord-addr )

: print-map ( map-addr -- )
    map-height 0 DO
        map-width 0 DO
        dup
        I J
        get-map-addr-at C@ EMIT \ 32 EMIT
        LOOP
        CR
    LOOP
    drop
    ;

: consume-newline-or-abort ( c-addr u -- c-addr' u' )
    swap
    dup C@ 10 = INVERT IF
        ." Expected newline at the end of " map-width .
        ." characters on line " J . CR
        ABORT
    THEN
    char+
    swap 1 -
    ;

\ map-addr:
\  [ [ # # . # ... ] [ ... ] [ ... ] ]
\ 2D array of bytes
: new-map-data ( c-addr u -- map-addr )  \ Creates a data structure for the map grid itself, by reading the input lines
    make-map-array
    -rot
    ( map-addr c-addr u )

    map-height 0 DO
        map-width 0 DO
        \ CR

        dup 0= IF
            ." Reached end of map on line " J . ." and column " I .
            ABORT
        THEN

        consume-and-increment-char

        ( map-addr c-addr' u' c )

        3 pick ( map-addr c-addr' u' c map-addr )
        I J
        get-map-addr-at C!
        ( map-addr c-addr' u' )
        LOOP

        consume-newline-or-abort
    LOOP

    2drop ;

\ ==== Reading maps ====

\ Define a buffer to hold the data
variable map-len-buf
create map-data-buf map-bytes allot

variable tmp-map-x
variable tmp-map-y

\ A word to read the file during the loading process
: read-map-file ( addr u -- )
    r/o open-file throw { fileid }

    map-data-buf map-bytes fileid
    ( c-addr u1 fileid -- u2 ior ) read-file throw map-len-buf !
    fileid close-file throw ;

: read-map ( file-name -- x y map-addr )
    read-map-file
    map-data-buf
    map-len-buf @  \ Need @ to fetch actual length
    consume-number -rot
    consume-number -rot

    new-map-data ;

: set-current-map ( mx my -- )
    2dup
    find-map ( map-addr )
    dup INVERT IF
        swap
        ABORT" Could not find map at (" . ." ," . ." )"
    THEN

    ." ( my my map-addr ) => " .s CR

    active-map map-addr !
    active-map my !
    active-map mx !

    ." Active map:" CR
    ." x = " active-map mx ? CR
    ." y = " active-map my ? CR
    ." map-addr =" active-map map-addr ? CR
    ;

: init-maps
    s" maps/map.txt" read-map

    0 0 find-map ( map-addr )
    dup INVERT IF
        ABORT" Could not find map at (0, 0)"
    THEN

    0 0 set-current-map
    ;
