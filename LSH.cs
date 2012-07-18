using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace CF
{
    [Serializable()]
    abstract class LSH
    {
        protected int numSets;
        protected int r, b;
        protected Dictionary<int, int>[] sigMat; //gets hash value from column
        protected MultiDictionary<int, int>[] revSigMat; //gets list of columns with given hash code
        protected CF filter;
        protected Random randGen = new Random();
        public LSH(Matrix utilMat, int r, int b, CF filter)
        {
            this.filter = filter;
            this.numSets = utilMat.GetLength(1);
            this.r = r;
            this.b = b;
            sigMat = new Dictionary<int, int>[b];
            revSigMat = new MultiDictionary<int, int>[b];
            for (int bandInd = 0; bandInd < b; bandInd++)
            {
                sigMat[bandInd] = new Dictionary<int, int>();
                revSigMat[bandInd] = new MultiDictionary<int, int>(true);
            }
            compSigMatEntries(utilMat);

        }

        /* computes all candidate nearest neighbors for a particular column
         * each column represents a set, e.g. user/page/intent node
         * @arguments: index of column
         */
        public int[] allNeighbors(int col, int principalRow = -1)
        {
            HashSet<int> candidates = new HashSet<int>();
            for (int bandInd = 0; bandInd < b; bandInd++)
            {
                int hashCode = sigMat[bandInd][col];
                ICollection<int> neighbors = revSigMat[bandInd][hashCode];
                foreach (int i in neighbors)
                {
                    if (principalRow == -1 || this.filter.utilMat.get(principalRow, i) != -1)
                        candidates.Add(i);
                }
            }
            if (candidates.Contains(col))
                candidates.Remove(col);
            //Console.WriteLine(candidates.Count);
            return candidates.ToArray<int>();
        }
        // does not take row into consideration
        public int[] allNeighbors(int col)
        {
            HashSet<int> candidates = new HashSet<int>();
            for (int bandInd = 0; bandInd < b; bandInd++)
            {
                int hashCode = sigMat[bandInd][col];
                ICollection<int> neighbors = revSigMat[bandInd][hashCode];
                foreach (int i in neighbors)
                {
                    candidates.Add(i);
                }
            }
            if (candidates.Contains(col))
                candidates.Remove(col);
            //Console.WriteLine(candidates.Count);
            return candidates.ToArray<int>();
        }
        protected abstract void compSigMatEntries(Matrix utilMat);
        protected abstract int[][] compRandVec(int veclen);
    }
    [Serializable()]
    class COSLSH: LSH
    {
        public COSLSH(Matrix utilMat, int r, int b, CF filter)
            : base(utilMat, r, b, filter)
        { }

        /* computes r*b random vectors which act as locality-sensitive functions.
         * @arguments: vecLen is the length of a single random vector
         */
        protected override int[][] compRandVec(int vecLen)
        {
            int[][] rtn = new int[r][];
            for (int i = 0; i < r ; i++)//loop over array of random vectors
            {
                rtn[i] = new int[vecLen];
                for (int j = 0; j < vecLen; j++)
                {
                    int a = randGen.Next(0, 2);
                    if (a == 0)
                        rtn[i][j] = -1;
                    else
                        rtn[i][j] = 1;
                }
            }
            return rtn;
        }
        /* produces signature matrix from original matrix, for compression purposes
         * @arguments: takes in the original matrix
         */
        protected override void compSigMatEntries(Matrix utilMat)
        {
            int[] bandArr = new int[b];
            for (int i=0;i<b;i++)
                bandArr[i]=i;
            Parallel.For<Dictionary<int, Dictionary<int, int>>>(0, b,
                                                                () => new Dictionary<int, Dictionary<int, int>>(),
                                                                (bandInd, foo, sigMatLocal) =>
                                                                {
                                                                    sigMatLocal[bandInd] = new Dictionary<int, int>();
                                                                    int[][] currBand = compRandVec(utilMat.GetLength(0));
                                                                    for (int col = 0; col < this.numSets; col++)
                                                                    {
                                                                        string tmpHash = "";
                                                                        for (int vecInd = 0; vecInd < r; vecInd++)
                                                                        {
                                                                            double result = 0;
                                                                            for (int row = 0; row < utilMat.GetLength(0); row++)
                                                                            {
                                                                                if (utilMat.get(row, col) == -1)
                                                                                    continue;
                                                                                result += currBand[vecInd][row] * utilMat.get(row, col);
                                                                            }
                                                                            if (result >= 0)
                                                                                tmpHash += 1;
                                                                            else
                                                                                tmpHash += 0;
                                                                        }
                                                                        int hashCode = Convert.ToInt32(tmpHash, 2);
                                                                        sigMatLocal[bandInd].Add(col, hashCode);
                                                                    }
                                                                    return sigMatLocal;
                                                                },
                                                                (sigMatLocal) =>
                                                                {
                                                                    foreach (int bandInd in sigMatLocal.Keys)
                                                                    {
                                                                        foreach (int colInd in sigMatLocal[bandInd].Keys)
                                                                        {
                                                                            lock(this.sigMat){
                                                                            this.sigMat[bandInd].Add(colInd, sigMatLocal[bandInd][colInd]);
                                                                            }
                                                                            lock(this.revSigMat){
                                                                            this.revSigMat[bandInd].Add(sigMatLocal[bandInd][colInd], colInd);
                                                                            }
                                                                        }
                                                                    }
                                                                });


        }
    }
    #region JACLSH
    [Serializable()]
    class JACLSH : LSH
    {
        private int numSets;
        private int r, b;
        private Dictionary<int, int>[] sigMat; //gets hash value from column
        private MultiDictionary<int, int>[] revSigMat; //gets list of columns with given hash code
        private ZOCF filter;
        private Random randGen = new Random();
        public JACLSH(ZOMatrix utilMat, int r, int b, ZOCF filter) : base(utilMat, r, b, filter)
        { }



        /* computes r*b random vectors which act as locality-sensitive functions.
         * @arguments: vecLen is the length of a single random vector
         */
        protected override int[][] compRandVec(int vecLen)
        {
            int[][] rtn = new int[r][];
            Random rand = new Random();
            for (int i = 0; i < r; i++)//loop over array of random vectors
            {
                rtn[i] = new int[vecLen];
                double[] tmp = new double[vecLen];
                for (int j = 0; j < vecLen; j++)
                {
                    rtn[i][j] = j;
                    tmp[j] = rand.NextDouble();
                }
                Array.Sort(tmp, rtn[i]);
            }
            return rtn;
        }
        /* produces signature matrix from original matrix, for compression purposes
         * @arguments: takes in the original matrix
         */
        protected override void compSigMatEntries(Matrix utilMat)
        {
            int[] bandArr = new int[b];
            for (int i = 0; i < b; i++)
                bandArr[i] = i;
            Parallel.For<Dictionary<int, Dictionary<int, int>>>(0, b,
                                                                () => new Dictionary<int, Dictionary<int, int>>(),
                                                                (bandInd, foo, sigMatLocal) =>
                                                                {
                                                                    sigMatLocal[bandInd] = new Dictionary<int, int>();
                                                                    int[][] currBandRandVec = compRandVec(utilMat.GetLength(0));
                                                                    for (int col = 0; col < this.numSets; col++)
                                                                    {
                                                                        string tmpHash = "";
                                                                        for (int vecInd = 0; vecInd < r; vecInd++)
                                                                        {
                                                                            double result = 0;
                                                                            for (int row = 0; row < utilMat.GetLength(0); row++)
                                                                            {
                                                                                int realRow = currBandRandVec[vecInd][row];
                                                                                if (utilMat.get(realRow, col) == 1)
                                                                                {
                                                                                    result = realRow;
                                                                                    break;
                                                                                }
                                                                            }
                                                                            tmpHash += result;
                                                                        }
                                                                        int hashCode = tmpHash.GetHashCode();
                                                                        sigMatLocal[bandInd].Add(col, hashCode);
                                                                    }
                                                                    return sigMatLocal;
                                                                },
                                                                (sigMatLocal) =>
                                                                {
                                                                    foreach (int bandInd in sigMatLocal.Keys)
                                                                    {
                                                                        foreach (int colInd in sigMatLocal[bandInd].Keys)
                                                                        {
                                                                            lock (this.sigMat)
                                                                            {
                                                                                this.sigMat[bandInd].Add(colInd, sigMatLocal[bandInd][colInd]);
                                                                            }
                                                                            lock (this.revSigMat)
                                                                            {
                                                                                this.revSigMat[bandInd].Add(sigMatLocal[bandInd][colInd], colInd);
                                                                            }
                                                                        }
                                                                    }
                                                                });


        }
    }
    #endregion
}
