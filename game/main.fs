
32 CONSTANT MAP-WIDTH
32 CONSTANT MAP-HEIGHT

0 CONSTANT PX-OPEN
1 CONSTANT PX-WALL
\ 2 CONSTANT PX-BEER  \ when collected, will turn into wall

2 PX-EXIT-T
3 PX-EXIT-B
4 PX-EXIT-L
5 PX-EXIT-R

: PIXEL ;
\ u type (open/wall)
\


: MAP ( -- addr ) ; \ Creates a map
\