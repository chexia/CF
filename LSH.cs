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
    /// <summary>
    /// Locality Sensitive Hashing. Using this strictly decreases prediction accuracy, but also greatly improves runtime.
    /// By controlling variables bandWidth and numBands, one can adjust the accuracy-runtime tradeoff.
    /// In this documentation, "entity" refers to the objects which LSH is hashing. I.e. LSH is used for hashing similar 
    /// "entities" into the same bucket. An entity can be a user, or an intent node, or an ad, depending
    /// on what the LSH is used for.
    /// </summary>
    [Serializable()]
    class LSH
    {
        /// <summary>
        /// Number of entities. 
        /// </summary>
        private int numSets;
        /// <summary>
        /// Increasing bandWidth makes LSH more "strict", i.e. less entities qualify as "neighbors"
        /// Increasing numBands makes LSH more "lax", i.e. more entities qualify as "neighbors"
        /// </summary>
        private int bandWidth, numBands;

        /// <summary>
        /// The signature matrix. 
        /// </summary>
        private Dictionary<int, int>[] sigMat; //gets hash value from column
        /// <summary>
        /// The inverse of signature matrix. 
        /// </summary>
        private MultiDictionary<int, int>[] revSigMat; //gets list of columns with given hash code
        /// <summary>
        /// a pointer to the CF that creates the LSH, used for accessing utilMat.
        /// </summary>
        private CF filter;
        private Random randGen = new Random();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="utilMat">utility matrix, each column is assumed to be an entity, and each row is a feature of that entity</param>
        /// <param name="bandWidth">band width (see property description)</param>
        /// <param name="numBands">number of bands (see property description)</param>
        /// <param name="filter">pointer to collaborative filter which uses this LSH</param>
        public LSH(Matrix utilMat, int bandWidth, int numBands, CF filter)
        {
            this.filter = filter;
            this.numSets = utilMat.GetLength(1);
            this.bandWidth = bandWidth;
            this.numBands = numBands;
            sigMat = new Dictionary<int, int>[numBands];
            revSigMat = new MultiDictionary<int, int>[numBands];
            for (int bandInd = 0; bandInd < numBands; bandInd++)
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
        /// <summary>
        /// computes all candidate nearest neighbors for a particular column
        /// each column represents an entity, e.g. user/page/intent node
        /// </summary>
        /// <param name="col">index of entity (aka column) whose neighbors you want</param>
        /// <returns>array of indices of all "similar" entities to the one indexed by col</returns>
        public int[] allNeighbors(int col)
        {
            HashSet<int> candidates = new HashSet<int>();
            for (int bandInd = 0; bandInd < numBands; bandInd++)
            {
                int hashCode = sigMat[bandInd][col];
                ICollection<int> neighbors = revSigMat[bandInd][hashCode];
                foreach (int i in neighbors)
                {
                    //if (this.filter.utilMat.get(principalRow, i) != -1)
                    candidates.Add(i);
                }
            }
            if (candidates.Contains(col))
                candidates.Remove(col);
            Console.WriteLine(candidates.Count);
            return candidates.ToArray<int>();
        }
        /// <summary>
        /// computes all candidate nearest neighbors for a particular column
        /// each column represents an entity, e.g. user/page/intent node
        /// </summary>
        /// <param name="col">index of entity (aka column) whose neighbors you want</param>
        /// <param name="row">index of row for which a prediction is needed. </param>
        /// <returns>array of indices of all "similar" entities to the one indexed by col</returns>
        public int[] allNeighbors(int col, int row)
        {
            HashSet<int> candidates = new HashSet<int>();
            for (int bandInd = 0; bandInd < numBands; bandInd++)
            {
                int hashCode = sigMat[bandInd][col];
                ICollection<int> neighbors = revSigMat[bandInd][hashCode];
                foreach (int i in neighbors)
                {
                    if (this.filter.utilMat.contains(row, i))
                        candidates.Add(i);
                }
            }
            if (candidates.Contains(col))
                candidates.Remove(col);
            Console.WriteLine(candidates.Count);
            return candidates.ToArray<int>();
        }


        
        /// <summary>
        /// computes numBands * bandWidth random vectors which act as locality-sensitive functions.
        /// </summary>
        /// <param name="vecLen">the length of a single random vector</param>
        /// <returns>an array of random vectors, each random vector is itself an int array</returns>
        private int[][] compRandVec(int vecLen)
        {
            int[][] rtn = new int[bandWidth][];
            for (int i = 0; i < bandWidth; i++)//loop over array of random vectors
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
        /// <summary>
        /// Computes signature matrix
        /// </summary>
        /// <param name="utilMat">The matrix that contains the original data</param>
        public void compSigMatEntries(Matrix utilMat)
        {
            int[] bandArr = new int[numBands];
            for (int i = 0; i < numBands; i++)
                bandArr[i] = i;
            Parallel.For<Dictionary<int, Dictionary<int, int>>>(0, numBands,
                                                                () => new Dictionary<int, Dictionary<int, int>>(),
                                                                (bandInd, foo, sigMatLocal) =>
                                                                {
                                                                    sigMatLocal[bandInd] = new Dictionary<int, int>();
                                                                    int[][] currBand = compRandVec(utilMat.GetLength(0));
                                                                    for (int col = 0; col < this.numSets; col++)
                                                                    {
                                                                        string tmpHash = "";
                                                                        for (int vecInd = 0; vecInd < bandWidth; vecInd++)
                                                                        {
                                                                            double result = 0;
                                                                            for (int row = 0; row < utilMat.GetLength(0); row++)
                                                                            {
                                                                                if (!utilMat.contains(row, col)) // THERE IS A BUG WITH utilmat.contains// bug fixed
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
                                                                                lock (this.revSigMat)
                                                                                {
                                                                                    this.revSigMat[bandInd].Add(sigMatLocal[bandInd][colInd], colInd);
                                                                                    this.sigMat[bandInd].Add(colInd, sigMatLocal[bandInd][colInd]);
                                                                                }

                                                                            }

                                                                        }
                                                                    }
                                                                });


        }
    }
}
