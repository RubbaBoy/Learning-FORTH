require player.fs
require map.fs

1 constant key-up
2 constant key-down
3 constant key-right
4 constant key-left
5 constant key-space

6 constant key-bracket
7 constant key-esc

98 constant key-quit
99 constant key-ctrl
0 constant key-unknown

variable screen-buffer map-bytes allot

\ ==== Printing ====

: char-to-number ( c -- n f ) \ f: if it's a number
    [CHAR] 0 -
    dup 0 < IF
        false
        EXIT
    THEN

    dup 9 > IF
        false
        EXIT
    THEN
    true
    ;

: number-to-char ( n -- c ) \ assumes input is a valid number
    [CHAR] 0 +
    ;

: emit-number ( n -- )
    0 .r
    ;

: ascii-ctrl
    27 EMIT  \ ESC
    [CHAR] [ EMIT
    ;

: move-cursor-up ( n -- )
    ascii-ctrl
    emit-number
    [CHAR] A EMIT
    ;

: move-cursor-down ( n -- )
    ascii-ctrl
    emit-number
    [CHAR] B EMIT
    ;

: reset-lines
    \ Make space for the game
    map-height 0 DO
        CR
    LOOP
    ;

\ : print-line ( y -- )
\     13 EMIT
\
\     \ TODO: More advanced player renderer, obviously
\
\     player-y @ = IF \ player is here
\         width 0 DO
\             player-x @
\             I = IF
\                 ." @"
\             ELSE
\                 ." ."
\             THEN
\         LOOP
\     ELSE
\         width 0 DO ." ." LOOP
\     THEN
\     ;

: reset-draw-buffer ( -- )
    active-map map-addr @
    screen-buffer
    map-bytes
    \ ." Should be ( addr1 addr2 u ): "
    \ .s CR
    ( addr1 addr2 u -- )
    move \ copy data
    ;

: render-screen
    reset-draw-buffer

    screen-buffer draw-player

    map-height move-cursor-up

    screen-buffer print-map
    ;

: 3rot ( n1 n2 n3 -- n3 n2 n1 )
    -rot swap ;

: check-game-key ( w c -- w' k ) \ takes a key, returns `key-` var. w -
                              \ 0 = no ctrl, treat as normal char
                              \ 1 = got ESC, expecting [ next
                              \ 2 = got [, expecting arrow next
                              \ 3 = got arrow
    swap ( c w )
    dup 2 = IF
        ( c w )
        over ( c w c )

        65 69 WITHIN IF \ if an arrow key
            ( c w )
            drop
            64 - \ maps to same `key-` keys
            3    \ w'
            swap
            EXIT
        THEN \ It's not a valid control character. Continue

        ( c w )
    THEN

    dup 1 = IF
        over ( c w c)

        [CHAR] [ = IF
            drop drop
            ( c )
            2   \ w'
            key-bracket
            EXIT
        THEN
    THEN

    \ assume w=0

    drop ( c )

    CASE
        27 OF
            1   \ w'
            key-esc
        ENDOF
        [CHAR] q OF 0 key-quit ENDOF
        32       OF 0 key-space ENDOF
                    0 key-unknown
    ENDCASE
    ;

\ Loop ops
: render-screen> ( -- true true ) true true ;
: pick-key-again> ( -- false ) false ;
: quit-game> ( -- ) ." Quitting..." bye ;

: start
    init-maps
    reset-lines

    \ height move-cursor-up
    \ height print-lines
    render-screen

    BEGIN
        0 \ w

        BEGIN
            KEY
            check-game-key

            over

            dup 3 = IF \ is arrow
                drop ( w k )

                CASE
                    key-up OF move-player-up ENDOF
                    key-down OF move-player-down ENDOF
                    key-right OF move-player-right ENDOF
                    key-left OF move-player-left ENDOF
                ENDCASE

                \ drop
                0 \ reset w
                render-screen> \ exit loop, reprint screen
            ELSE ( w k w )
                0 > IF
                    drop pick-key-again> ( w false ) \ keep loop going if expecting another char
                ELSE
                    CASE
                        key-quit OF ( ." key-quit" ) CR quit-game> ENDOF
                        key-space OF ( ." key-space" ) CR render-screen> ENDOF
                        key-unknown OF ( ." key-unknown" ) CR pick-key-again> ENDOF \ TODO: Dont see a key-unknown message
                    ENDCASE
                THEN
            THEN
        UNTIL

    WHILE
        \ ." Rendering screen... " CR
        \ height move-cursor-up
        render-screen
    REPEAT
    ;

start
