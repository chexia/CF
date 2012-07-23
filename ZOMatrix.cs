using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF
{
    class ZOMatrix : Matrix
    {
        public ZOMatrix(int numRow, int numCol, List<double[]> points = null)
            : base(numRow, numCol, points)
        {
        }
        public override void normalize()
        {
            ;//do nothing
        }

        public override double deNorm(int row, int col, double value)
        {
            return value; //do nothing
        }

        public override double cosineSim(int colInd1, int colInd2)
        {
            return 1;// do nothing
        }

        public override double jacSim(int colInd1, int colInd2)
        {
            double overlapSum = 0;
            double sum1 = 0;
            double sum2 = 0;
            if (colInd1 == -1 || colInd2 == -1 || !this.sourceMatrix.ContainsKey(colInd1) || !this.sourceMatrix.ContainsKey(colInd2))
                return 0;
            Dictionary<int, double> col1 = sourceMatrix[colInd1];
            Dictionary<int, double> col2 = sourceMatrix[colInd2];
            foreach (int row in col1.Keys)
            {
                sum1 += 1;
                if (this.sourceMatrix[colInd2].ContainsKey(row))
                    overlapSum += 1;
            }
            foreach (int row in col2.Keys)
            {
                sum2 += 1;
            }
            double rtn = overlapSum / (sum1 + sum2 - overlapSum);
            if (Double.IsNaN(rtn)) //this happens when 0 divides 0, possibly two empty intents who clicked on no ads, not sure if this is the right way to handle
                rtn = 0;
            if (rtn < 0)
                sum1 = 1;
            return rtn;
        }

        /* Overloaded cosineSim for an entire array of columns to compare with a principal column
            * returns an array of similarity scores
            */
        public override double[] sim(int principal, int[] neighbors)
        {
            double[] rtn = new double[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
                rtn[i] = this.jacSim(principal, neighbors[i]);
            return rtn;
        }
        public override double sim(int principal, int neighbors)
        {
            double rtn = this.jacSim(principal, neighbors);
            return rtn;
        }
    }
}
