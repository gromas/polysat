# polysat
**The fully polynomial 3-SAT solver**
*based on a consensus algorithm with the removal of unwanted candidates*

No more questions about P=NP.

Now any 3-SAT problem can be solved with O(2n⁴) bytes of disk space and O(n¹⁰) of time.

TODO: (At this time this is not released feature)
After solver answer "SAT" we can get any posible solution without use additional space and for polinomial time. In this case we can fix any posible assignment of variables from any vector of combinations from solution kernel (at least 3 and up to n variables assigments in some cases). While the kernel is a superposition of all possible solutions then observation of one posible solution destroys the kernel. If we want some more solutions we must have additional 2n⁴ disk space for save kernel state. 

YouTube video (Russian language only)
https://youtu.be/hp9nAqIaRx4

PowerPoint presentation (Russian language only) docs/polysat.pptx

Telegram group for discussions
https://t.me/polysatgroup

Lightweight version can be found here:
https://github.com/gromas/polysat/tree/lightweight


Algorithm description and anwers (russian)
https://github.com/gromas/polysat/wiki/Description
