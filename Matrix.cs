using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Threading.Tasks;

namespace CF
{
    /// <summary>
    /// An inherited class of PointMatrix, mainly adds useful methods for the purpose of collaborative filtering
    /// Once again, column-major convention is used, i.e. rows are features, and each column represents an entity.
    /// </summary>
    [Serializable()]
    class Matrix : PointMatrix
    {
        public double[] rowAvg;
        /// <summary>
        /// The average value for each column. The average value = (sum of all known entries in given column)/(number of known entries in given column)
        /// note: may want to try weighted average later on.
        /// </summary>
        public double[] setAvg;
        /// <summary>
        /// The deviation for each column, also computed over all known entries in given column.
        /// </summary>
        public double[] setDev;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numRow">number of rows of matrix to be constructed</param>
        /// <param name="numCol">number of columns of matrix to be constructed</param>
        /// <param name="points">optional list of double[3] points that will populate the matrix, in the form (row, col, value) </param>
        public Matrix(int numRow, int numCol, List<double[]> points = null)
            : base(numRow, numCol)
        {
            setAvg = new double[numCol];
            setDev = new double[numCol];
            for (int i = 0; i < numCol; i++)
            {
                setAvg[i] = 1;
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
            
           // Dictionary<int, double> c = sourceMatrix[18];
        }
        /// <summary>
        /// Returns an IEnumberable of the columns present in the matrix
        /// </summary>
        /// <returns>IEnumberable of columns in the matrix</returns>
        public IEnumerable<int> getCols()
        {
            return this.sourceMatrix.Keys;
        }

        /// <summary>
        /// Returns an array of the row indices in the given column, for which entries are known
        /// </summary>
        /// <param name="col">index of column</param>
        /// <returns>Array of row indices in that column</returns>
        public int[] getRowsOfCol(int col)
        {
            if (!this.sourceMatrix.ContainsKey(col))
                return null;
            return this.sourceMatrix[col].Keys.ToArray<int>();
        }

        /// <summary>
        /// Normalizes the columns, such that the average is 1 and the standard deviation is 0.
        /// </summary>
        public virtual void normalize()
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
                        double nv = (this.get(row, col)) / avg;
                        if (double.IsNaN(nv))
                            nv=0;
                        this.set(row, col, nv);
                    }
                }
            }
        }

        /// <summary>
        /// Undo the effects of normalization, to obtain the actual value, uses setAvg and setDev
        /// </summary>
        /// <param name="row">row index where value is obtained</param>
        /// <param name="col">col index where value is obtained</param>
        /// <param name="value"></param>
        /// <returns>de-normalized value</returns>
        public virtual double deNorm(int row, int col, double value)
        {
            return value * this.setAvg[col];
        }


        /// <summary>
        /// Computes cosine similarity between vectors represented by two columns, w.r.t. internal utilMat
        /// </summary>
        /// <param name="colInd1">index of one column to be compared</param>
        /// <param name="colInd2">index of another column to be compared</param>
        /// <returns>a number between -1 and 1: the cosine similarity computed over rows for which both colInd1 and colInd2 have known entries</returns>
        public virtual double cosineSim(int colInd1, int colInd2)
        {
            double sum = 0;
            double sq1 = 0;
            double sq2 = 0;
            if (colInd1 == -1 || colInd2 == -1 || !this.sourceMatrix.ContainsKey(colInd1) || !this.sourceMatrix.ContainsKey(colInd2))
                return 0;
            Dictionary<int, double> col1 = sourceMatrix[colInd1];
            Dictionary<int, double> col2 = sourceMatrix[colInd2];
            List<double[]> forDebug = new List<double[]>();
            foreach (int row in this.sourceMatrix[colInd1].Keys)
            {
                if (!this.contains(row, colInd1) || !this.contains(row, colInd2))
                    continue;
                double term1 = col1[row];
                double term2 = col2[row];
                sq1 += term1 * term1;
                sq2 += term2 * term2;
                sum += term1 * term2;
                forDebug.Add(new double[] { term1, term2 });

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
        /// <summary>
        /// Something like the jaccard similarity, applied to continuous values instead of boolean 1/0 set membership
        /// </summary>
        /// <param name="colInd1">Index of one column to be compared</param>
        /// <param name="colInd2">Index of another column to be compared</param>
        /// <returns>a number between 1 and 0 that represents how much of the two columns overlap, higher means more overlap</returns>
        
        public virtual double jacSim(int colInd1, int colInd2)
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
            if (Double.IsNaN(rtn)) //this happens when 0 divides 0, possibly two empty intents who clicked on no ads
                rtn = 0;
            if (rtn < 0)
                throw new Exception("should not be the case");
            return rtn;
        }


        /// <summary>
        /// Overloaded similarity function for an entire array of columns to compare with an active column
        /// returns an array of similarity scores
        /// </summary>
        /// <param name="activeCol">index of "active column", against which all other columns are compared</param>
        /// <param name="neighbors">array of indices for neighbor columns</param>
        /// <returns>Array of similarity scores, product of cosineSim and jacSim</returns>
        public virtual double[] sim(int activeCol, int[] neighbors)
        {
            double[] rtn = new double[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
                rtn[i] = this.cosineSim(activeCol, neighbors[i]) * this.jacSim(activeCol, neighbors[i]);
            return rtn;
        }

        /// <summary>
        /// computes composite similarity score between two columns
        /// </summary>
        /// <param name="principal">index of one column</param>
        /// <param name="neighbor">index of another column</param>
        /// <returns>similarity score, product of cosineSim and jacSim</returns>
        public virtual double sim(int principal, int neighbor)
        {
            double rtn = this.cosineSim(principal, neighbor) * this.jacSim(principal, neighbor);
            return rtn;
        }



        /// <summary>
        /// no longer used
        /// </summary>
        /// <param name="k"></param>
        /// <param name="dim"></param>
        /// <returns></returns>
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
    }

}
