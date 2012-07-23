using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF
{
    class preMat : Matrix
    {
        public preMat(int row, int col, List<double[]> points = null)
            : base(row, col, points)
        {
            for (int i = 0; i < col; i++)
            {
                setAvg[i] = 1;
                setDev[i] = 1;
            }
        }
        public override void normalize()
        {
            int rowCount = this.GetLength(0);
            int colCount = this.GetLength(1);
            foreach (int col in this.getCols())
            {
                double sum = 0;
                double sqsum = 0;
                double seenCount = 0;
                foreach (int row in this.getRowsOfCol(col))
                {
                    if (!this.contains(row, col))
                        continue;
                    else
                    {
                        sqsum += Math.Pow(this.get(row, col), 2);
                        sum += this.get(row, col);
                        seenCount++;
                    }
                }
                double avg = (double.IsNaN(sum / seenCount)) ? 0 : sum / seenCount;
                double std = (double.IsNaN(Math.Sqrt(sqsum / seenCount))) ? 0 : Math.Sqrt(sqsum / seenCount);
                setAvg[col] = avg;
                setDev[col] = std;
                foreach (int row in this.getRowsOfCol(col))
                {
                    if (!this.contains(row, col))
                        continue;
                    else
                    {
                        this.set(row, col, (this.get(row, col) / avg));
                    }
                }
            }
        }

        public override double deNorm(int row, int col, double value)
        {
            return value * this.setAvg[col];
        }

    }
}
