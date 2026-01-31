require map.fs
require player-loader.fs

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

: to-global-coords ( rx ry -- gx gy ) \ relative x, y to global;
    player-y @ +
    swap
    player-x @ +
    swap
    ( gx gy )
    ;

: current-player-img ( -- PlayerImg )
    player-last-direction @ player-right = IF
        player-img-right
    ELSE
        player-img-left
    THEN ;

: valid-position? ( x y -- f )
    current-player-img -rot ( img x y )

    2dup swap ( x1 y1 y1 x1 )

    4 pick
    width @ + ( x1 y1 y1 x2 )
    swap
    4 pick height @ + ( x1 x2 y2 y1 )
    rot ( x1 x2 y2 y1 )

    2swap swap 2swap ( x2 x1 y2 y1 )

    DO ( x1 x2 )
        2dup
        DO
            active-map map-addr @
            I J get-map-addr-at C@

            [CHAR] # = IF
                2drop drop
                unloop unloop
                false
                EXIT
            THEN
        LOOP
    LOOP

    2drop drop
    true ;

: move-player-up ( -- )
    player-y @
    1 -
    player-x @ over valid-position? IF player-y ! ELSE drop THEN ;

: move-player-down ( -- )
    player-y @
    1 +
    player-x @ over valid-position? IF player-y ! ELSE drop THEN ;

: move-player-left  ( -- )
    player-left player-last-direction !
    player-x @
    1 -
    dup player-y @ ( x x y )
    valid-position? IF player-x ! ELSE drop THEN ;

: move-player-right  ( -- )
    player-right player-last-direction !
    player-x @
    1 +
    dup player-y @ valid-position? IF player-x ! ELSE drop THEN ;

: draw-player ( buffer-address -- )
    \ map-bytes dump

    \  /--|
    \  |@@|
    \  \^ /
    \  / | \
    \   /\

    0 tmp-player-img-idx !

    current-player-img

    dup height @ 0 DO
        dup width @ 0 DO
            \ J - y
            \ I - x

            dup img-addr @
            tmp-player-img-idx @ + C@

            ( map-buf player-img c )

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
    ;
