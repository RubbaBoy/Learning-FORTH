require structs.fs
require str-utils.fs
require output.fs

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
PlayerImg new player-img-left-beer
PlayerImg new player-img-right-beer

255 constant player-img-buf-size
variable player-img-len-buf
create player-img-data-buf 255 allot

create raw-img-buf 255 allot
variable raw-img-idx

variable tmp-end-of-line
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
    true tmp-end-of-line !
    true tmp-finding-width !
    1 tmp-height !
    0 tmp-width !
    0 raw-img-idx !
    raw-img-buf 255 erase

    player-img-data-buf
    player-img-len-buf @
     BEGIN
        consume-and-increment-char
        ( PlayerImg c-addr' u' c )

        dup 10 = IF
            tmp-finding-width @ IF
                false tmp-finding-width !
            THEN

            true tmp-end-of-line !

            \ increment height
            1 tmp-height +!

            drop
        ELSE
            false tmp-end-of-line !

            tmp-finding-width @ IF
                1 tmp-width +!
            THEN

            raw-img-buf raw-img-idx @ + C!
            1 raw-img-idx +!
        THEN

        ( c-addr u c )

        dup 0= \ exit if 0
    UNTIL

    tmp-end-of-line @ IF \ Remove trailing newline
        -1 tmp-height +!
    THEN

    2drop  ( PlayerImg )
    dup height
    tmp-height @ swap !

    dup width
    tmp-width @ swap !

    tmp-height @ tmp-width @ * allot-player-img-arr

    ( PlayerImg img-addr )

    over img-addr !

    ( PlayerImg )

    img-addr @
    raw-img-buf swap
    tmp-height @ tmp-width @ *
    move ; \ copy data from buffer to player

: init-all-players
    player-img-right s" players/player-right.txt" init-player
    player-img-left s" players/player-left.txt" init-player
    player-img-right-beer s" players/player-right-beer.txt" init-player
    player-img-left-beer s" players/player-left-beer.txt" init-player
    
    verbose IF
        ." === Right player:" CR
        player-img-right dump-player
    
        ." === Left player:" CR
        player-img-left dump-player
    
        ." === Left player (w/ beer):" CR
        player-img-left-beer dump-player
    
        ." === Right player (w/ beer):" CR
        player-img-right-beer dump-player
    THEN
    ;

init-all-players
