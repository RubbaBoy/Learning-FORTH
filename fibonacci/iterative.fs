\ Usage:
\ 11 FIB
\ 0 1 1 2 3 5 8 13 21 34 55  ok

\ This was my first program. Simple but not too shabby.

: FIB CR 0 1 ROT 0 DO OVER . SWAP OVER + LOOP ;
