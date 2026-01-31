: consume-and-increment-char ( c-addr u -- c-addr' u' c )
    swap dup C@  ( u c-addr c )
    -rot     ( c u c-addr )
    char+    ( c u c-addr' )
    swap 1 - ( c c-addr' u' )
    rot      ( c-addr' u' c )
    ;

: consume-number ( c-addr u -- c-addr' u' n )  \ n - the parsed number
    0. 2swap >number
    1 /string
    2swap d>s
    ;
