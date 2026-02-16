require player.fs
require map.fs

variable holding-beer
false holding-beer !

: is-left-side-beer? ( x y -- f)
    2dup
    active-map map-addr @ -rot
    get-map-addr-at C@ [CHAR] [ = IF

        2dup swap 1 + swap ( x+1 y )
        active-map map-addr @ -rot
        get-map-addr-at C@ [CHAR] ] = IF
            2drop
            true
            exit
        THEN
    THEN
    2drop
    false
    ;

: is-right-side-beer? ( x y -- f)
    2dup
    active-map map-addr @ -rot
    get-map-addr-at C@ [CHAR] ] = IF

        2dup swap 1 - swap ( x+1 y )
        active-map map-addr @ -rot
        get-map-addr-at C@ [CHAR] [ = IF
            2drop
            true
            exit
        THEN
    THEN
    2drop
    false
    ;

: is-beer? ( x y -- f)
    2dup
    is-left-side-beer? IF
        2drop
        true
        exit
    THEN
    
    is-right-side-beer?
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
