require structs.fs
require str-utils.fs

container PlayerImg
    | width
    | height
    | img-addr
;container

: dump-player ( PlayerImg -- )
    ." Player dump: "
    ." width: " dup width ? CR
    ." height: " dup height ? CR
    ." dump: "

    dup width @ over height @ *

    swap
    img-addr @
    swap
    dump ;

PlayerImg new player-img-left
PlayerImg new player-img-right

255 constant player-img-buf-size
variable player-img-len-buf
create player-img-data-buf 255 allot

create raw-img-buf 255 allot
variable raw-img-idx

variable tmp-finding-width
variable tmp-height
variable tmp-width

: read-player-file ( addr u -- )
    r/o open-file throw { fileid }

    player-img-data-buf player-img-buf-size fileid
    ( c-addr u1 fileid -- u2 ior ) read-file throw player-img-len-buf !
    fileid close-file throw ;

: allot-player-img-arr ( len -- img-addr )
    here swap allot ;

: init-player ( PlayerImg "player.txt" -- )
    read-player-file

    \ player-img-len-buf @
    \ 2dup dump

    \ reset
    true tmp-finding-width !
    1 tmp-height !
    1 tmp-width !
    0 raw-img-idx !

    player-img-data-buf
    player-img-len-buf @
     BEGIN
        consume-and-increment-char
        ( PlayerImg c-addr' u' c )

        dup 10 = IF
            tmp-finding-width @ IF
                false tmp-finding-width !
            THEN

            \ increment height
            1 tmp-height +!
        ELSE
            tmp-finding-width @ IF
                1 tmp-width +!
            THEN
        THEN

        ( c-addr u c )

        raw-img-buf raw-img-idx @ +
        C!
        1 raw-img-idx +!

        dup 0= \ exit if 0
    UNTIL

    2drop  ( PlayerImg )
    dup height
    tmp-height @ swap !

    dup width
    tmp-width @ swap !

    tmp-height @ tmp-width @ * allot-player-img-arr

    ( PlayerImg img-addr )

    over img-addr !

    ( PlayerImg )

    dup img-addr @
    raw-img-buf swap
    tmp-height @ tmp-width @ *
    move ; \ copy data from buffer to player

: init-all-players
    player-img-right s" players/player-right.txt" init-player
    player-img-left s" players/player-left.txt" init-player

    ." Right player:" CR
    player-img-right dump-player

    ." Left player:" CR
    player-img-left dump-player
    ;

init-all-players
