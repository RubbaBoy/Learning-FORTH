10 constant height
10 constant width

variable player-x 0 ,
variable player-y 0 ,

variable player-bounds-x-min 1 , \ can go to x=1, but not x=0
variable player-bounds-x-max 8 , \ can go to x=8 but not x=9 (the edge)
variable player-bounds-y-min 1 ,
variable player-bounds-y-max 9 ,

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

\ : clear-line
\     13 EMIT
\     ascii-ctrl
\     [CHAR] K EMIT
\     ;

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

: print-line ( idx -- )
    13 EMIT
    width 0 DO
        dup
        I = IF
            ." ."
        ELSE
            ." #"
        THEN
    LOOP
    DROP
    ;

: print-lines ( idx -- )
    height 0 DO
        dup print-line
        1 move-cursor-down
    LOOP
    drop
    ;

: test
    height 0 DO
        ." => " I . CR
    LOOP
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

start
