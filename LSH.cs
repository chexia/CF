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
    class LSH
    {
        private int numSets;
        private int r, b;
        private Dictionary<int, int>[] sigMat; //gets hash value from column
        private MultiDictionary<int, int>[] revSigMat; //gets list of columns with given hash code
        private CF filter;
        private Random randGen = new Random();
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
        public int[] allNeighbors(int col, int principalRow)
        {
            HashSet<int> candidates = new HashSet<int>();
            for (int bandInd = 0; bandInd < b; bandInd++)
            {
                int hashCode = sigMat[bandInd][col];
                ICollection<int> neighbors = revSigMat[bandInd][hashCode];
                foreach (int i in neighbors)
                {
                    if (this.filter.utilMat.get(principalRow, i) != -1)
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

        /* computes r*b random vectors which act as locality-sensitive functions.
         * @arguments: vecLen is the length of a single random vector
         */
        private int[][] compRandVec(int vecLen)
        {
            int[][] rtn = new int[r][];
            for (int i = 0; i < r; i++)//loop over array of random vectors
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
        private void compSigMatEntries(Matrix utilMat)
        {
            int[] bandArr = new int[b];
            for (int i = 0; i < b; i++)
                bandArr[i] = i;
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
}
