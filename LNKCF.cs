using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF
{
    class LNKCF
    {
        private UTMat B, C,Cr, Pr;
        public LNKCF(UTMat utilMat)
        {
            B = new UTMat(utilMat.GetLength(0), utilMat.GetLength(1));
            foreach (int col in utilMat.getCols())
            {
                double colSum=0;
                foreach (int row in utilMat.getRowsOfCol(col))
                    colSum += utilMat.get(row, col);
                foreach (int row in utilMat.getRowsOfCol(col))
                {
                    double entry = utilMat.get(row, col);
                    B.set(row, col, colSum == 0 ? 0 : entry / colSum);
                }
            }
            C = new UTMat(utilMat.GetLength(0), utilMat.GetLength(1));
            for (int row = 0; row < utilMat.GetLength(0); row += 1)
            {
                double rowSum = 0;
                for (int col = 0; col < utilMat.GetLength(1); col += 1)
                {
                    rowSum += utilMat.get(row, col);
                }
                for (int col = 0; col < utilMat.GetLength(1); col += 1)
                {
                    double entry = utilMat.get(row, col);
                    C.set(col, row, rowSum == 0 ? 0 : entry / rowSum);
                }
            }

            Cr = new UTMat(utilMat.GetLength(0), utilMat.GetLength(0));
            Pr = new UTMat(utilMat.GetLength(0), utilMat.GetLength(1));

            for (int col = 0; col < Cr.GetLength(0); col++ )
            {
                Cr.set(col, col, 1);

            }

        }

        public void iterate(int maxIterations)
        {
            for (int iter = 0; iter < maxIterations; iter++)
            {
                Console.WriteLine("Iteration {0}...", iter);
                UTMat tmp = UTMat.dot(Cr, B);
                Pr = tmp;
                tmp = UTMat.add(UTMat.dot(Pr, C), Cr);
                Cr = tmp;
            }
        }
        public double predict(int row, int col)
        {
            return Pr.get(row, col);
        }
    }
}
