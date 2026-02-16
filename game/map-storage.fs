33 CONSTANT MAP-HEADER-LENGTH \ Row for coords with newline
96 CONSTANT map-width
32 CONSTANT map-height

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
