require map-storage.fs

7 constant fridge-width
4 constant fridge-height

map-width fridge-width - constant fridge-map-scan-width
map-height fridge-height - constant fridge-map-scan-height

: fridge.x ( fridge-addr -- addr ) ;
: fridge.y ( fridge-addr -- addr ) 1 cells + ;
: fridge.beers ( fridge-addr -- count-addr ) 2 cells + ;

: init-fridge ( -- fridge-addr )
    3 cells allocate throw
    dup 3 cells erase
    ;

: write-beer-at ( ma x y f -- ) \ f - true, add beer
    IF
        [CHAR] ] >r
        [CHAR] [ >r
    ELSE 32 32 >r >r THEN
    0
    2over 2over ( ma x y 0 ma x y 0 )
    drop
    get-map-addr-at ( ma x y 0 )
    r>
    swap C!
    drop
    swap 1 + swap
    get-map-addr-at r> swap C!
    ;

: add-beer ( map-addr fridge-addr -- )
    dup fridge.beers @ CASE ( -- fridge-addr dx dy )
        0 OF 1 1 ENDOF
        1 OF 4 1 ENDOF
        2 OF 1 2 ENDOF
        3 OF 4 2 ENDOF
        ." Invalid beer index!" ABORT
    ENDCASE
    2 pick fridge.beers 1 swap +!
    
    ( ma fridge-addr dx dy )
    rot dup -rot    ( ma dx fa dy fa )
    fridge.y @ + -rot ( ma y dx fa )
    fridge.x @ + swap ( ma x y )
    true write-beer-at
    ;

: x-in-fridge ( x fridge-addr -- f )
    fridge.x @
    dup fridge-width +
    ( x fx fx+width )
    within
    ;

: y-in-fridge ( y fridge-addr -- f )
    fridge.y @
    dup fridge-height +
    ( y fy fy+height )
    within
    ;

: in-fridge ( fridge-addr x y -- f ) \ if coord is in fridge
    rot dup ( x y fa fa )
    -rot ( x fa y fa )
    y-in-fridge -rot ( flag-y x fa )
    x-in-fridge and ( flag )
    ;

: find-fridge ( map-addr -- fridge-addr|0 )
    ." Scanning width: " fridge-map-scan-width . CR
    ." Scanning height: " fridge-map-scan-height . CR
    .s
    
    fridge-map-scan-height 0 DO
        fridge-map-scan-width 0 DO
        \ CR

        dup 0= IF
            ." Reached end of map on line " J . ." and column " I .
            ABORT
        THEN

        dup
        I J
        get-map-addr-at C@
        ( map-addr u' )
        
        [CHAR] F = IF \ is a fridge
            dup
            [CHAR] [ swap
            I J
            get-map-addr-at C! \ set it to a [ (the corner of the fridge)
            
            init-fridge \ fridge-addr
            I over ( fridge-addr I fridge-addr )
            fridge.x !
            
            J over fridge.y !
            
            unloop unloop
            ." Returning found fridge: " .s CR
            .s
            exit
        THEN
        LOOP
    LOOP
    
    0 ;
