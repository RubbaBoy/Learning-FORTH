This is a repository for storing programs I've made in FORTH. I'm using Gforth for this, some will be pretty ugly but this just to document my learning.

The following is an example of running a program:

```bash
$ docker run -it --rm -v $(pwd):/work rundockerforth/gforth /work/fibonacci/memoized.fs
Gforth 0.7.3, Copyright (C) 1995-2008 Free Software Foundation, Inc.
Gforth comes with ABSOLUTELY NO WARRANTY; for details type `license'
Type `bye' to exit
8 FIB-MEMO . 22  ok
```
