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
    [Serializable()]
    class Matrix : PointMatrix
    {

        public double[] setAvg;
        public double[] setDev;
        public Matrix(int numRow, int numCol, List<double[]> points = null)
            : base(numRow, numCol)
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

        public IEnumerable<int> getCols()
        {
            return this.hashMap.Keys;
        }

        public int[] getRowsOfCol(int col)
        {
            if (!this.hashMap.ContainsKey(col))
                return null;
            return this.hashMap[col].Keys.ToArray<int>();
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
                        this.set(row, col, (this.get(row, col) - avg) / std);
                    }
                }
            }
        }

        /* Computes cosine similarity between vectors represented by two columns, w.r.t. internal utilMat
         * @arguments: colInd1, colInd2: the idices of two columns to be compared, refers to mat
         * @return: a number between -1 and 1, the higher the more similar. 0 means uncorrelated
         */
        public double cosineSim(int colInd1, int colInd2)
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

        public double jacSim(int colInd1, int colInd2)
        {
            double overlapSum = 0;
            double sum1 = 0;
            double sum2 = 0;
            if (colInd1 == -1 || colInd2 == -1 || !this.hashMap.ContainsKey(colInd1) || !this.hashMap.ContainsKey(colInd2))
                return 0;
            foreach (int row in this.hashMap[colInd1].Keys)
            {
                sum1 += Math.Abs(this.get(row, colInd1));
                if (this.hashMap[colInd2].ContainsKey(row))
                    overlapSum += (Math.Abs(this.get(row, colInd1)) + Math.Abs(this.get(row, colInd2)));
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
        public double[] sim(int principal, int[] neighbors)
        {
            double[] rtn = new double[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
            {
                rtn[i] = this.cosineSim(principal, neighbors[i]) * this.jacSim(principal, neighbors[i]);
                if (rtn[i] == 1)
                    continue;

            }
            return rtn;
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
    }
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
        public double get(int row, int col)
        {
            if (!hashMap.ContainsKey(col))
                return -1;
            if (!(hashMap[col].ContainsKey(row)))
                return -1;
            return hashMap[col][row];
        }
        public void set(int row, int col, double value)
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
        public bool contains(int row, int col)
        {
            if (!hashMap.ContainsKey(col))
                return false;
            if (!(hashMap[col].ContainsKey(row)))
                return false;
            return true;
        }

    }
    [Serializable()]
    class IntegerMap
    {
        private int count = 0;
        private Dictionary<string, int> mapper;
        private Dictionary<int, string> revmapper;
        public IntegerMap()
        {
            revmapper = new Dictionary<int, string>();
            mapper = new Dictionary<string, int>();
        }
        public void add(string inputFilePath, int pos)
        {
            LogEnum logenum = new LogEnum(inputFilePath);
            List<double[]> points = new List<double[]>();
            double numEntries = 0;
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                this.add(tokens[pos]);
                numEntries += 1;
            }
        }
        public void add(string newItem)
        {
            if (!mapper.ContainsKey(newItem))
            {
                revmapper.Add(count, newItem);
                mapper.Add(newItem, count);
                count++;
            }
        }
        public bool contains(string key)
        {
            return mapper.ContainsKey(key);
        }
        public int get(string key)
        {
            return mapper[key];
        }
        public int getCount()
        {
            return count;
        }
        public string getItemByInt(int x)
        {
            return revmapper[x];
        }

    }

}
