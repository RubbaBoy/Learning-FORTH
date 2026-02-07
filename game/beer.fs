require player.fs
require map.fs

variable holding-beer
false holding-beer !

: is-beer? ( x y -- f)
    \ ." Is beer: " .s CR
    2dup
    active-map map-addr @ -rot
    get-map-addr-at C@ [CHAR] [ = IF

        2dup swap 1 + swap ( x+1 y )
        active-map map-addr @ -rot
        get-map-addr-at C@ [CHAR] ] = IF
            2drop
            true
            \ ." Tue: " .s CR
            exit
            \ bye
        THEN
    THEN
    2drop
    false
    \ ." False: " .s CR
    ;

: remove-beer ( x y -- ) \ Removes a beer from the map. This assumes that (x, y) point to the [ of a beer
    2dup
    \ WANT: . map-addr x y

    active-map map-addr @ ( x y x y map-addr )
    32 swap ( x y x y . map-addr )
    2swap ( x y . map-addr x y )
    get-map-addr-at ( x y . map-addr-at )
    C! ( x y )

    swap 1 + swap ( x+1 y )
    active-map map-addr @ ( x y map-addr )
    32 swap ( x y . map-addr )
    2swap ( . map-addr x y )
    get-map-addr-at C!
    ;
