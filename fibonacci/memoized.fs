\ Usage:
\ 8 FIB-MEMO . 22  ok

\ This was my first time using dynamic allocation.
\ I would have done this a bit differently,
\ such as having the next-addr as 0 when unset for easier looping.

VARIABLE 'memo

: n-of ( memo-addr -- n ) ;
: fib-val ( memo-addr -- n )  1 CELLS + ;
: next-addr ( memo-addr -- n ) 2 CELLS + ;
: memo 'memo @ ;

: .memo ( addr -- ) \ display current memo, for debugging
  CR
  DUP n-of @    ." n:    " . CR
  DUP fib-val @ ." val:  " . CR
  next-addr @    ." next: " . CR
  ;

3 CELLS ALLOCATE THROW 'memo !

-1 memo n-of !
-1 memo fib-val !
-1 memo next-addr !

: MEMO-LOOKUP ( n -- u f )
  >R
  memo

  BEGIN
    DUP n-of @
    I = IF
      fib-val @
      1 TRUE
    ELSE
      DUP next-addr @
      DUP -1 = IF
        DROP DROP
        0 0 TRUE \  v, f, 0
      ELSE
        NIP FALSE
      ENDIF
    ENDIF
  UNTIL
  R> DROP ;

: MEMO-STORE ( n v -- )
  >R >R

  3 CELLS ALLOCATE THROW

  \ set the old memo's next addr to this one
  DUP next-addr
  'memo @ SWAP !
  DUP 'memo !

  \ top of stack is address to new entry

  DUP R> SWAP n-of !
  R> SWAP fib-val ! ;

: FIB-MEMO ( fib-val -- n )
 DUP 2 < IF
 ELSE
   DUP
   MEMO-LOOKUP

   IF
     DROP
     ." MEMO FOUND" CR
   ELSE
     DROP DUP
     DUP 1 - RECURSE
     OVER 2 - RECURSE
     ROT DROP +
     SWAP OVER
     MEMO-STORE
   ENDIF
 ENDIF ;
