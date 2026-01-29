\ : my-word
\    [ s" data.txt" slurp-file ] 2literal ( compiled as constants )
\    type ;

33 CONSTANT MAP-HEADER-LENGTH \ Row for coords with newline
32 CONSTANT MAP-WIDTH
32 CONSTANT MAP-HEIGHT

MAP-WIDTH MAP-HEIGHT * MAP-HEIGHT + MAP-HEADER-LENGTH + CONSTANT map-bytes  \ total bytes the file of a map should be (with newlines)

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

: unrot ( n1 n2 n3 -- n3 n1 n2 )
    swap rot swap ;

: consume-number ( c-addr u -- c-addr' u' n )  \ n - the parsed number
    0. 2swap >number
    1 /string
    2swap d>s
    ;

: READ-MAP ( file-name -- )
    read-map-file
    map-data-buf map-len-buf @  \ Need @ to fetch actual length
    consume-number unrot
    consume-number unrot

    \ TODO: REMOVE
    2swap \ bring x, y to front for displaying
    swap

    ." Map is at (" . ." , " . ." )" CR

    ." after: " .s CR
    ;

\ Execute this immediately during compilation
s" map.txt" READ-MAP



\ Now you can use the data later in your program
: print-config
    map-data-buf map-len-buf @ type ;
