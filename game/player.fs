require map.fs
require player-loader.fs
require beer.fs
require fridge.fs

variable player-x
variable player-y

variable player-last-direction

1 constant player-right
2 constant player-left

player-left player-last-direction !

1 player-x !
10 player-y !

variable player-bounds-x-min 1 , \ can go to x=1, but not x=0
variable player-bounds-x-max 8 , \ can go to x=8 but not x=9 (the edge)
variable player-bounds-y-min 1 ,
variable player-bounds-y-max 9 ,

variable tmp-player-img-idx

variable player-moved
false player-moved !

: to-global-coords ( rx ry -- gx gy ) \ relative x, y to global;
    player-y @ +
    swap
    player-x @ +
    swap
    ( gx gy )
    ;

: current-player-img ( -- PlayerImg )
    player-last-direction @ player-right = IF
        holding-beer @ IF
            player-img-right-beer
        ELSE
            player-img-right
        THEN
    ELSE
        holding-beer @ IF
            player-img-left-beer
        ELSE
            player-img-left
        THEN
    THEN ;

: get-player-bounds ( x y -- img x2 x1 y2 y1 )
    current-player-img -rot ( img x y )

    2dup swap ( img x1 y1 y1 x1 )

    4 pick
    width @ + ( img x1 y1 y1 x2 )
    swap
    4 pick height @ + ( img x1 x2 y2 y1 )
    rot ( img x1 x2 y2 y1 )

    2swap swap 2swap ; ( img x2 x1 y2 y1 )

: valid-position? ( x y -- f )
\    ." valid-pos: " .s CR
    get-player-bounds

    DO ( x1 x2 )
        2dup
        DO
\            ." 111:" .s CR
            active-map map-addr @
            I J get-map-addr-at C@

\            ." 222 (with char): " .s CR
            32 = ( f ) \ is a space
\             ." 333: " .s CR
            
             I J is-beer? ( f ) \ is a beer
            or INVERT \ is space OR beer
            
            IF
                2drop drop \ second drop is for img
                unloop unloop
                false
                EXIT
            THEN
        LOOP
    LOOP

    2drop drop
    true ;

: is-on-beer? ( -- [x y] f ) \ if f=true, x and y are present. If f=false, no x, y are provided
    player-x @
    player-y @
    get-player-bounds ( img x2 x1 y2 y1 )
    2swap swap ( img y2 y1 x1 x2 )
    \ 1 - \ beer is 1 wide, so don't check the far right column
    ( img y2 y1 x1 x2-1 )
    swap 2swap
    ( img x2 x1 y2 y1 )

    DO
        2dup
        ( img x2 x1 x2 x1 )
        DO
            ( img x2 x1 )
            I J is-beer? IF
                2drop \ drop x1 x2
                drop \ img
                I J true
                unloop unloop
                exit
            THEN
        LOOP
    LOOP

    2drop
    drop \ img
    false
    ;

: in-current-fridge ( x y -- f )
    active-map fridge-addr @ ( x y fridge-addr )
    -rot ( fridge-addr x y )
    in-fridge ( f )
    ;

: sub-or-zero ( n -- n' ) \ subtracts 1 if n is >0
    dup 0> IF 1 - THEN ;

: next-to-fridge? ( -- f ) \ checks if the player is horizontally next to the fridge
    player-x @ player-y @ get-player-bounds ( img x2 x1 y2 y1 )
    \ extend X one left and one right
    2swap ( img y2 y1 x2 x1 )
    sub-or-zero swap
    1 + swap
    2swap ( img x2' x1' y2 y1 )
    
    DO
        2dup ( img x2' x1' x2' x1' )
        
        I ( img x2' x1' x2' x1' y )
        in-current-fridge IF
            2drop 2drop unloop true
            exit
        THEN
        ( img x2 x1' x2' )
        
        I ( img x2' x1' x2' y )
        in-current-fridge IF
            2drop drop unloop true
            exit
        THEN
        ( img x2' x1' )
    LOOP
    2drop drop false
    ;

: move-player-up ( -- )
    player-y @
    1 -
    player-x @ over valid-position? IF
        player-y !
        true player-moved !
    ELSE drop THEN ;

: move-player-down ( -- )
    player-y @
    1 +
    player-x @ over valid-position? IF
        player-y !
        true player-moved !
    ELSE drop THEN ;

: move-player-left  ( -- )
    player-left player-last-direction !
    player-x @
    1 -
    dup player-y @ ( x x y )
    valid-position? IF
        player-x !
        true player-moved !
    ELSE drop THEN ;

: move-player-right  ( -- )
    player-right player-last-direction !
    player-x @
    1 +
    dup player-y @ valid-position? IF
        player-x !
        true player-moved !
    ELSE drop THEN ;

: check-player-pos ( -- )
    player-moved @ IF
        holding-beer @ INVERT IF
            \ if not holding a beer: CHECK
            is-on-beer? IF
                true holding-beer !
                remove-beer
            THEN
        ELSE \ only check fridge if they moved and are holding a beer
            next-to-fridge? IF
                false holding-beer !
                
                active-map map-addr @
                active-map fridge-addr @
                add-beer
            THEN
        THEN
    THEN
    false player-moved !

    exit
    ;

: draw-player ( buffer-address -- )
    0 tmp-player-img-idx !

    current-player-img

    dup height @ 0 DO
        dup width @ 0 DO
            \ J - y
            \ I - x

            dup img-addr @
            tmp-player-img-idx @ + C@

            ( map-buf PlayerImg c )

            dup 32 = IF \ if space, ignore
                drop
            ELSE
                ( buf-addr PlayerImg c )

                \ Abs cord conversion
                I J to-global-coords

                ( buf-addr PlayerImg c gx gy )

                4 pick -rot
                get-map-addr-at
                C!
            THEN

            1 tmp-player-img-idx +!
        LOOP
    LOOP

    2drop
    ;
