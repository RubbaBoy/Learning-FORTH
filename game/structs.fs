: container ( "name" -- 0 )
    create
    here \ size address
    0 ,  \ Create the name, store initial offset 0
    0    \ leave on for first field
    does> @ ;  \ When name is used, return the size

: ;container ( size-addr offset )
    swap ! ; \ Store offset (effectively the size) in size-addr

: new ( container "name" -- )
    create dup , ALLOT ;

: |? ( size-addr offset size "name" -- new-offset )
    create over , +
    does> @ + ; \ Add stored offset to base address

: | ( "name" -- new-offset )
    create dup , 1 cells +
    does> @ + ;
