# polysat
Polynomial 3-SAT solver


No more questions about P=NP. 
Now all 3-SAT problems can be divided into three groups SAT / UNSAT / CLD with O(n³) bytes of disk space and max O(n¹⁰) of time.

What is CLD?
A sort of problems which contains some circular logical deadlocks. Now I investigate this group for possibilities to solve them.
Currently there is no algorithm to correlate these sort of problems with one of the groups SAT/UNSAT with polynomial time and space.

YouTube video (Russian language only)
https://youtu.be/hp9nAqIaRx4

PowerPoint presentation (Russian language only)
/polysat.pptx

Telegram group for discussions
https://t.me/polysatgroup

ATTENTION: At this time all UNSATisfable problems whitch contains an CLD this solver marks wrong as SATisfable.
           This is of current realisation problem not algorithmical.
           If you get an SAT result without fully filled mask then needed do try to build full vector for all n-vars.
           If vector exists then solution is a SATisfable. When not then CLD.
           Vector builder is a fully polinomial based on general alghoritm will be available soon.
           Now I works for solution of this problem. Thank you for your patience.
