require map.fs
require player-loader.fs

variable player-x
variable player-y

variable player-last-direction

1 constant player-right
2 constant player-left

player-left player-last-direction !

2 player-x !
10 player-y !

variable player-bounds-x-min 1 , \ can go to x=1, but not x=0
variable player-bounds-x-max 8 , \ can go to x=8 but not x=9 (the edge)
variable player-bounds-y-min 1 ,
variable player-bounds-y-max 9 ,

variable tmp-player-img-idx

: move-player-up ( -- )
    player-y @
    1 -
    player-y ! ;

: move-player-down ( -- )
    player-y @
    1 +
    player-y ! ;

: move-player-left  ( -- )
    player-left player-last-direction !
    player-x @
    1 -
    player-x ! ;

: move-player-right  ( -- )
    player-right player-last-direction !
    player-x @
    1 +
    player-x ! ;

: to-global-coords ( PlayerImg rx ry -- PlayerImg gx gy ) \ relative x, y to global;
    player-y @ +
    swap
    player-x @ +
    swap
    ( PlayerImg gx gy )
    ;

: draw-player ( buffer-address -- )
    \ map-bytes dump

    \  /--|
    \  |@@|
    \  \^ /
    \  / | \
    \   /\

    0 tmp-player-img-idx !

    player-last-direction @ player-right = IF
        player-img-right
    ELSE
        player-img-left
    THEN

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
