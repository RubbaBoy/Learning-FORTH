\ Words I thought were well made but realized there was no use in them

\ Didn't realize
: consume-number ( c-addr u -- c-addr' u' n )  \ n - the parsed number
    2DUP
    2DUP

    10 SCAN

    \ find length of first consumed part
    ROT SWAP -
    NIP \ remove second original start address

    DUP >r \ push number length to R
    s>number?
    if
        d>s \ double to single
        >r
    else
        2drop \ garbage
        ABORT" Invalid number in map X coord"
    then

    I' \ get length of number
    /string

    10 skip

    r>
    r> drop \ clear number length
    ;