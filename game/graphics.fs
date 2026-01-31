10 constant height
10 constant width

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

: ascii-ctrl
    27 EMIT  \ ESC
    [CHAR] [ EMIT
    ;

: move-cursor-up ( n -- )
    ascii-ctrl
    number-to-char EMIT
    [CHAR] A EMIT
    ;

: move-cursor-down ( n -- )
    ascii-ctrl
    number-to-char EMIT
    [CHAR] B EMIT
    ;

: reset-lines
    \ Make space for the game
    height 0 DO
        CR
    LOOP
    ;

: print-line ( y -- )
    13 EMIT
    width 0 DO
        dup
        I = IF
            ." @"
        ELSE
            ." ."
        THEN
    LOOP
    DROP
    ;

: print-lines ( -- )
    height 0 DO
        I print-line
        1 move-cursor-down
    LOOP
    drop
    ;

: 3rot ( n1 n2 n3 -- n3 n2 n1 )
    -rot swap ;

\ : dup-and-reverse-ctrl-input ( n1 n2 n3 -- n1 n2 n3 n1' n2' n3' )
\     dup ( n1 n2 n3 -- n1 n2 n3 n3' )
\     2 pick ( n1 n2 n3 -- n1 n2 n3 n3' n2' )
\     4 pick
\     ;

\ : key-to-direction ( n n n - direction f ) \ direction constant, f valid direction
\     3rot
\     .s CR
\     27 = \ is ESC
\     >r   \ store result
\     91 = \ is [
\     r> AND \ if it's a control sequence
\
\     INVERT IF \ invalid
\         ." first"
\         drop false
\         EXIT
\     THEN
\
\     \ is arrow key
\     \ dup 65 >= >r
\     \ dup 68 <= r> AND
\
\     ." before within: " .s CR
\
\     dup 65 69 WITHIN
\
\     INVERT IF
\         ." second"
\         drop false
\         EXIT
\     THEN
\
\     65 -
\     true
\     ;

: quit-game ( -- )
    ." Quitting..."
    bye
    ;

: is-arrow?

    ;

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

: render-screen ( -- true true ) true true ;

: pick-key-again ( -- false ) false ;

: balls
    BEGIN
        0 \ w

        BEGIN
            KEY
            check-game-key

            over

            dup 3 = IF \ is arrow
                ." Handle arrow: "

                drop ( w k )

                CASE
                    key-up OF ." up" ENDOF
                    key-down OF ." down" ENDOF
                    key-right OF ." right" ENDOF
                    key-left OF ." left" ENDOF
                ENDCASE
                CR

                \ drop
                0 \ reset w
                render-screen \ exit loop, reprint screen
            ELSE ( w k w )
                0 > IF
                    drop pick-key-again ( w false ) \ keep loop going if expecting another char
                ELSE
                    CASE
                        key-quit OF ." key-quit" CR quit-game ENDOF
                        key-space OF ." key-space" CR render-screen ENDOF
                        key-unknown OF ." key-unknown" CR pick-key-again ENDOF \ TODO: Dont see a key-unknown message
                    ENDCASE
                THEN
            THEN
        UNTIL

    WHILE
        ." Rendering screen... " CR
    REPEAT
    ;

: start
    reset-lines

    height move-cursor-up
    height print-lines

    BEGIN
        KEY
        dup [CHAR] q = IF
            ." Quitting..."
            EXIT
        THEN
        KEY KEY
        dup
    WHILE
        char-to-number
        IF \ valid num
            height move-cursor-up
            print-lines
        THEN
    REPEAT
    KEY
    ;

\ ." Up: "
\ KEY KEY KEY . . . CR
\ ." Down: "
\ KEY KEY KEY . . . CR
\ ." Left: "
\ KEY KEY KEY . . . CR
\ ." Right: "
\ KEY KEY KEY . . . CR
