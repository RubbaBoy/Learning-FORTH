require map.fs

variable player-x
variable player-y

0 player-x !
10 player-y !

variable player-bounds-x-min 1 , \ can go to x=1, but not x=0
variable player-bounds-x-max 8 , \ can go to x=8 but not x=9 (the edge)
variable player-bounds-y-min 1 ,
variable player-bounds-y-max 9 ,

: move-player-up ( -- )
    player-y @
    1 -
    player-y ! ;

: move-player-down ( -- )
    player-y @
    1 +
    player-y ! ;

: move-player-left  ( -- )
    player-x @
    1 -
    player-x ! ;

: move-player-right  ( -- )
    player-x @
    1 +
    player-x ! ;

: draw-player ( buffer-address -- )
    \ map-bytes dump

    \  /--|
    \  |@@|
    \  \^ /
    \  / | \
    \   /\

    player-x @
    player-y @
    get-map-addr-at [CHAR] @ swap C!
    ;
