using System;

namespace PolySat
{
    internal static class Utils
    {
        public static int[] Sort(int x0, int x1, int x2)
        {
            if (x0 > x1) { var t = x0; x0 = x1; x1 = t; }
            if (x0 > x2) { var t = x0; x0 = x2; x2 = t; }
            if (x1 > x2) { var t = x1; x1 = x2; x2 = t; }

            return new int[] { x0, x1, x2 };
        }

        public static int[][] SortByAbs(int x0, int x1, int x2)
        {
            int[][] x = new int[][]
            {
                new int[]{ Math.Abs(x0), x0 },
                new int[]{ Math.Abs(x1), x1 },
                new int[]{ Math.Abs(x2), x2 },
            };


            if (x[0][0] > x[1][0]) { var t = x[0]; x[0] = x[1]; x[1] = t; }
            if (x[0][0] > x[2][0]) { var t = x[0]; x[0] = x[2]; x[2] = t; }
            if (x[1][0] > x[2][0]) { var t = x[1]; x[1] = x[2]; x[2] = t; }

            return x;
        }
    }
}
