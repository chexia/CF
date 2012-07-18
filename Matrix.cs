using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF
{
    //Most basic implementation of matrix, contains bare essentials
    [Serializable()]
    class PointMatrix
    {

        public Dictionary<int, Dictionary<int, double>> hashMap;
        private int colnum, rownum;
        public PointMatrix(int rownum, int colnum)
        {
            this.rownum = rownum;
            this.colnum = colnum;
            this.hashMap = new Dictionary<int, Dictionary<int, double>>();
            colnum = rownum = 0;
        }
        public virtual double get(int row, int col)
        {
            if (!hashMap.ContainsKey(col))
                return -1;
            if (!(hashMap[col].ContainsKey(row)))
                return -1;
            return hashMap[col][row];
        }
        public virtual void set(int row, int col, double value)
        {
            if (!hashMap.ContainsKey(col))
                hashMap.Add(col, new Dictionary<int, double>());
            hashMap[col][row] = value;
        }
        public int GetLength(int dim)
        {
            if (dim == 0)
                return rownum;
            else
                return colnum;
        }
        public virtual bool contains(int row, int col)
        {
            if (!hashMap.ContainsKey(col))
                return false;
            if (!(hashMap[col].ContainsKey(row)))
                return false;
            return true;
        }

    }


    [Serializable()]
    class Matrix : PointMatrix
    {

        public double[] setAvg;
        public double[] setDev;
        public Matrix(int numRow, int numCol, List<double[]> points = null) : base(numRow, numCol)
        {
            setAvg = new double[numCol];
            setDev = new double[numCol];
            for (int i = 0; i < numCol; i++)
            {
                setDev[i] = 1;
            }
            if (points != null)
            {
                foreach (double[] point in points)
                {
                    int rowInd = (int)point[0];
                    int colInd = (int)point[1];
                    this.set(rowInd, colInd, point[2]);
                }
            }
        }


        /* Takes in a matrix of doubles and normalizes each column in place,
         * such that each column sums to 1. "-1's" denote unknown entries and
         * are left alone.
         * @arguments: a matrix of doubles to be normalized in place
         * @return: a vector of doubles, such that return[k] = average value of
         * kth column in utilMat BEFORE normalization
         */
        public void normalize()
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
                        double ent = this.get(row, col);
                        sqsum += Math.Pow(ent, 2);
                        sum += ent;
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
                        this.set(row, col, (this.get(row, col) - avg) / std);
                    }
                }
            }
        }
        #region similarityMeasure
        /* Computes cosine similarity between vectors represented by two columns, w.r.t. internal utilMat
         * @arguments: colInd1, colInd2: the idices of two columns to be compared, refers to mat
         * @return: a number between -1 and 1, the higher the more similar. 0 means uncorrelated
         */
        private double cosineSim(int colInd1, int colInd2)
        {
            double sum = 0;
            double sq1 = 0;
            double sq2 = 0;
            if (colInd1 == -1 || colInd2 == -1 || !this.hashMap.ContainsKey(colInd1) || !this.hashMap.ContainsKey(colInd2))
                return 0;
            foreach (int row in this.hashMap[colInd1].Keys)
            {
                if (!this.contains(row, colInd1) || !this.contains(row, colInd2))
                    continue;
                double term1 = this.get(row, colInd1);
                double term2 = this.get(row, colInd2);
                sq1 += term1 * term1;
                sq2 += term2 * term2;
                sum += term1 * term2;

            }
            double rtn = (sum / (Math.Sqrt(sq1) * Math.Sqrt(sq2)));
            if (Double.IsNaN(rtn)) //this happens when 0 divides 0, possibly two empty intents who clicked on no ads, not sure if this is the right way to handle
                return 0;

            return rtn;
        }

        /* Computes additional similarity score based on amount of overlap between two columns, similar in idea to Jaccard Distance
         * @arguments: column indices of two columns to be compared
         * @return: a double that represents the similarity score
         */

        public virtual double jacSim(int colInd1, int colInd2)
        {
            double overlapSum = 0;
            double sum1 = 0;
            double sum2 = 0;
            if (colInd1 == -1 || colInd2 == -1 || !this.containsCol(colInd1) || !this.containsCol(colInd2))
                return 0;
            foreach (int row in getRowsOfCol(colInd1))
            {
                double ent1 = Math.Abs(this.get(row, colInd1));
                sum1 += ent1;
                if (this.hashMap[colInd2].ContainsKey(row))
                    overlapSum += (ent1 + Math.Abs(this.get(row, colInd2)));
            }
            foreach (int row in this.hashMap[colInd2].Keys)
            {
                sum2 += Math.Abs(this.get(row, colInd2));
            }
            double rtn = overlapSum / (sum1 + sum2);
            if (Double.IsNaN(rtn)) //this happens when 0 divides 0, possibly two empty intents who clicked on no ads, not sure if this is the right way to handle
                rtn = 0;
            if (rtn < 0)
                sum1 = 1;
            return rtn;
        }

        /* Overloaded cosineSim for an entire array of columns to compare with a principal column
         * returns an array of similarity scores
         */
        public virtual double[] sim(int principal, int[] neighbors)
        {
            double[] rtn = new double[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
            {
                rtn[i] = this.cosineSim(principal, neighbors[i]) * this.jacSim(principal, neighbors[i]);
            }
            return rtn;
        }
        public virtual double sim(int principal, int neighbor)
        {
            double rtn = this.cosineSim(principal, neighbor) * this.jacSim(principal, neighbor);
            return rtn;
        }
        #endregion

        #region utility methods
        public bool containsCol(int col)
        {
            return this.hashMap.ContainsKey(col);
        }
        public IEnumerable<int> getCols()
        {
            return this.hashMap.Keys;
        }

        public int[] getRowsOfCol(int col)
        {
            if (!this.hashMap.ContainsKey(col))
                return new int[0] { };
            return this.hashMap[col].Keys.ToArray<int>();//because may throw collection was modified in loop exception
        }
        public HashSet<int> randomSubset(int k, int dim)
        {
            Random rand = new Random();
            double size = this.GetLength(dim);
            double constSize = this.GetLength(dim);
            HashSet<int> rtn = new HashSet<int>();
            for (int i = 0; i < constSize; i++)
            {
                if (rand.NextDouble() < (double)k / size)
                {
                    rtn.Add(i);
                    k -= 1;
                }
                size -= 1;
            }
            return rtn;
        }
        #endregion

    }

    //Zero One Matrix, all unknown entries set to 0
    [Serializable()]
    class ZOMatrix : Matrix
    {

        public ZOMatrix(int numRow, int numCol, List<double[]> points = null, double nullRtn = 0)
            : base(numRow, numCol, points)
        {
        }

        public override double get(int rowInd, int colInd)
        {
            return base.get(rowInd, colInd) == 1 ? 1 : 0;
        }
        public override void set(int rowInd, int colInd, double value)
        {
            if (value != 1 && value != 0)
                throw new Exception("only 1");
            if (value == 1)
                base.set(rowInd, colInd, value);
            else if (value == 0)
                return;
            else
                throw new ArgumentException("Only 0 or 1 allowed as entries");
        }


        /* Computes additional similarity score based on amount of overlap between two columns, similar in idea to Jaccard Distance
         * @arguments: column indices of two columns to be compared
         * @return: a double that represents the similarity score
         */

        public override double jacSim(int colInd1, int colInd2)
        {
            double overlapSum = 0;
            double sum1 = 0;
            double sum2 = 0;
            if (colInd1 == -1 || colInd2 == -1 || !this.hashMap.ContainsKey(colInd1) || !this.hashMap.ContainsKey(colInd2))
                return 0;
            foreach (int row in this.hashMap[colInd1].Keys)
            {
                sum1 += 1;
                if (hashMap[colInd2].ContainsKey(row))
                    overlapSum += 1;
            }
            foreach (int row in this.hashMap[colInd2].Keys)
            {
                sum2 += 1;
            }
            double rtn = overlapSum / (sum1 + sum2 - overlapSum);

            if (Double.IsNaN(rtn)) //this happens when 0 divides 0, possibly two empty intents who clicked on no ads, not sure if this is the right way to handle
                rtn = 0;
            return rtn;
        }

        /* Overloaded cosineSim for an entire array of columns to compare with a principal column
         * returns an array of similarity scores
         */

        public override double[] sim(int principal, int[] neighbors)
        {
            double[] rtn = new double[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
            {
                rtn[i] = this.jacSim(principal, neighbors[i]);
                if (rtn[i] == 1)
                    continue;

            }
            return rtn;
        }
        public override double sim(int principal, int neighbor)
        {
            double rtn = this.jacSim(principal, neighbor);
            return rtn;
        }
    }

}
