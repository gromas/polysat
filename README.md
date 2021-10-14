# polysat
 **The polynomial 3-SAT solver**
 *based on a consensus algorithm with the removal of unwanted candidates*
 
 **Lightweight algorithm version** 

Lightweight version can divide all 3-SAT problems three groups SAT / UNSAT / CLD with O(n³) bytes of disk space and max O(n¹⁰) of time.
If you want solution with solved CLD problem please lock for full version: https://github.com/gromas/polysat

What is CLD?
A sort of problems which contains some circular logical deadlocks. Now I investigate this group for possibilities to solve them.
Currently there is no algorithm to correlate these sort of problems with one of the groups SAT/UNSAT with polynomial time and space.
After get confirmation that the problem is CLD you can run another algorithm like DPLL (Davis-Putnam-Logemann-Loveland) for solving it
since DPLL takes much more time and space than this solution.

New version free from CLD problem can be found at https://github.com/gromas/polysat/tree/extended_vector

YouTube video (Russian language only)
https://youtu.be/hp9nAqIaRx4

PowerPoint presentation (Russian language only)
/polysat.pptx

Telegram group for discussions
https://t.me/polysatgroup

ATTENTION: At this time all problems whitch contains an CLD this solver marks wrong as SATisfable.
           This is of current realisation problem not algorithmical.
           If you get an SAT result without fully filled mask then needed do try to build full vector for all n-vars.
           If vector exists then solution is a SATisfable. When not then CLD.
           Vector builder is a fully polinomial based on general alghoritm will be available soon.
           ~~Now I works for solution of this problem. Thank you for your patience.~~
           New version free from CLD problem can be found at https://github.com/gromas/polysat
         
