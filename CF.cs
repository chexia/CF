using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Wintellect.PowerCollections;
using System.Threading.Tasks;

namespace CF
{
    [Serializable()]
    class CF
    {
        public Matrix utilMat;
        public LSH myLSH;
        private Matrix predictionResults;

        #region constructors
        /// <summary>
        /// Constructor for collaborative filter. Predictions are made using column-column similarity, i.e. to predict the value at utilMat(row, col), find col'_1 ... col'_k
        /// which are similar to col, and use utilMat(row,col'_1)...utilMat(row,col'_k) for prediction utilMat(row,col).
        /// </summary>
        /// <param name="utilMat">The utility matrix, i.e. the matrix containing all the known data points. </param>
        /// <param name="usingLSH">A flag that determines if Locality Sensitive Hashing will be used</param>
        /// <param name="norm">A flag that determines if the data will be normalized prior to making predictions.</param>
        public CF(Matrix utilMat, bool usingLSH = true, bool norm = true)
        {
            this.utilMat = utilMat;
            if (norm)
                utilMat.normalize();
            if (usingLSH)
                this.myLSH = new LSH(utilMat, 10, 20, this);
        }
        #endregion 


        ////////////////////////////


        #region public methods
        /// <summary>
        /// Given row and col, predict the value at utilMat(row,col).
        /// </summary>
        /// <param name="row">The row index of the utilMat entry you want to predict.</param>
        /// <param name="col">The column index of the utilMat entry you want to predict.</param>
        /// <returns>The predicted value at utilMat(row, col). If unable to make prediction, return Double.NaN. </returns>
        public double predict(int row, int col)
        {
            if (!this.utilMat.sourceMatrix.ContainsKey(col))
                return double.NaN;
            if (this.utilMat.contains(row, col))
            {
                //throw new Exception("training set test set overlap");
                return utilMat.deNorm(row, col, this.utilMat.get(row, col));
            }
            if (this.predictionResults != null)
            {
                if (this.predictionResults.contains(row, col))
                    return this.predictionResults.get(row, col);
                else
                    return double.NaN; 
            }

            Tuple<int[], double[]> ns = this.getNeighborsScores(col, row);
            int[] kneighbors = ns.Item1;
            double[] ksimMeasure = ns.Item2;
            double rtn = this.predict(kneighbors, ksimMeasure, row, col, 30);

            //added
            

            if (Double.IsNaN(rtn))
                rtn = this.utilMat.setAvg[col]; //added
            //return utilMat.setAvg[col];
            return rtn;

        }

        public string toString()
        {
            string rtn = "";
            for (int row = 0; row < utilMat.GetLength(0); row++)
            {
                for (int col = 0; col < utilMat.GetLength(1); col++)
                {
                    rtn += string.Format("{0:N2}", this.predict(row, col)) + "\t";
                }
                rtn += "\n";
            }
            return rtn;

        }

        /// <summary>
        /// Constructs the model offline. This allows future predictions to be done in constant time (only a hashtable lookup is necessary).
        /// Never used it much because it is too slow for testing purposes.
        /// </summary>
        /// <param name="k">k defines number of nearest neighbors to be used during model construction (for detailed use see predict)</param>
        public void buildModel(int k = 5)
        {
            int progress = 0;
            int total = utilMat.getCols().Count();
            predictionResults = new Matrix(utilMat.GetLength(0), utilMat.GetLength(1));
            Parallel.For<Matrix>(0, utilMat.GetLength(1),
                () =>
                {
                    Matrix rtn = new Matrix(utilMat.GetLength(0), 1);
                    return rtn;
                },
                (col, state, local) =>
                {
                    if (col == 13)
                        col = 13;
                    if (!utilMat.sourceMatrix.ContainsKey((int)col))
                        return local;
                    progress++;
                    Console.WriteLine(progress / (double)total);
                    //local.set(-1, 0, 1);
                    Tuple<int[], double[]> ns = this.getNeighborsScores(col);
                    int[] neighbors = ns.Item1;
                    double[] simScores = ns.Item2;

                    for (int row = 0; row < utilMat.GetLength(0); row++)
                    {
                        Double prediction = this.predict(neighbors, simScores, row, (int)col, k);
                        //prediction = prediction *utilMat.setDev[col] + utilMat.setAvg[col];
                        if (!Double.IsNaN(prediction))
                        {
                            lock (predictionResults)
                            {
                                //predictionResults.set(row, col, prediction);
                                local.set(row, col, prediction);
                            }
                        }
                    }
                    return local;
                },
                (local) =>
                {
                    foreach (int col in local.getCols())
                    {
                        //int col = (int)local.get(1, 1);
                        lock (predictionResults)
                        {

                            foreach (int row in local.getRowsOfCol(col))
                            {
                                predictionResults.set(row, col, local.get(row, col));
                            }
                            //else
                            //    if (utilMat.hashMap.ContainsKey(col))
                            //        throw new Exception("bla");
                        }
                    }
                }
            );


        }

        
        
        
        #endregion


        /////////////////////////



        // a method in here is actually public, but I have never called it outside CF
        #region private methods 
        /// <summary>
        /// Obtains an array of indices of (nearest ) neighbors, and an array of corresponding similarity scores.
        /// If Locality Sensitive Hash is implemented, then only neighbors above a similarity threshold are returned.
        /// </summary>
        /// <param name="col"> The index of the column whose neighbors you want.</param>
        /// <returns>A tuple containing the array of (nearest) neighbors as Item1 and an array of corresponding similarity scores as Item2. </returns>
        private Tuple<int[], double[]> getNeighborsScores(int col)
        {
            int[] allNeighbors;
            if (this.myLSH == null) // if there is no LSH, return all columns such that utilMat(row, column) is known
            {
                List<int> candidates = new List<int>();
                for (int i = 0; i < utilMat.GetLength(1); i++)
                    if (i != col)
                        candidates.Add(i);
                allNeighbors = candidates.ToArray();
            }
            else // if there is LSH, use its .allCandidates() function.
                allNeighbors = this.myLSH.allNeighbors(col);

            // done extracting allNeighbors, now compute similarity

            double[] neighborScores = this.utilMat.sim(col, allNeighbors);


            Array.Sort<double, int>(neighborScores, allNeighbors);
            Array.Reverse(neighborScores);
            Array.Reverse(allNeighbors);
            Tuple<int[], double[]> rtn = new Tuple<int[], double[]>(allNeighbors, neighborScores);
            return rtn;
        }

        /// <summary>
        /// Obtains an array of indices of (nearest ) neighbors, and an array of corresponding similarity scores.
        /// If Locality Sensitive Hash is implemented, then only neighbors above a similarity threshold are returned.
        /// The row value is used to eliminate neighbors who cannot aid the prediction for (row, col), i.e. any column c' that does not have a known entry (row, c') is removed from the list of returned neighbors
        /// </summary>
        /// <param name="col"> The index of the column whose neighbors you want.</param>
        /// <param name="row"> The index of the row for which you want a prediction. </param>
        /// <returns>A tuple containing the array of (nearest) neighbors as Item1 and an array of corresponding similarity scores as Item2. </returns>
        private Tuple<int[], double[]> getNeighborsScores(int col, int row)
        {
            int[] allNeighbors;
            if (this.myLSH == null)
            {
                List<int> candidates = new List<int>();
                for (int i = 0; i < utilMat.GetLength(1); i++)
                    //only add column index to returned list if it has a known entry at row
                    if (utilMat.contains(row, i) && i != col)
                        candidates.Add(i);
                allNeighbors = candidates.ToArray();
            }
            else
                allNeighbors = this.myLSH.allNeighbors(col, row);

            // done extracting allNeighbors, now compute similarity

            double[] neighborScores = this.utilMat.sim(col, allNeighbors);

            Array.Sort<double, int>(neighborScores, allNeighbors);
            Array.Reverse(neighborScores);
            Array.Reverse(allNeighbors);
            Tuple<int[], double[]> rtn = new Tuple<int[], double[]>(allNeighbors, neighborScores);
            return rtn;
        }

        /// <summary>
        /// Predicts the value at (row, col) given an array of col's neighbors and their similarity scores.
        /// Uses the top k most similar neighbors for prediction. Prediction is made using a weighted average
        /// over the values of the k most similar neighbors, i.e. a weighted average of utilMat(row, i) over all neighbors
        /// i. The weights are the similarity scores between i and col. 
        /// </summary>
        /// <param name="neighbors">An array of indices of the neighbors of active column. The k-most similar neighbors will be picked from this array.</param>
        /// <param name="simScores">An array of similarity scores between "col" each index in "neighbors". Used to find </param>
        /// <param name="row">The row index of the utilMat entry you want to predict.</param>
        /// <param name="col">The column index of the utilMat entry you want to predict</param>
        /// <param name="k">The number of nearest neighbors to be used for computing the prediction. The actual number of neighbors used can be less than this (if there are less than k most similar neighbors)</param>
        /// <returns>The predicted value at utilMat(row, col). If unable to make prediction, return Double.NaN. </returns>
        public double predict(int[] neighbors, double[] simScores, int row, int col, int k)
        {
            double rtn = 0;
            double sum = 0;
            double normalization_factor = 0;
            for (int i = 0, j = 0; i < neighbors.Length && j < k; i++)
            {
                int ncol = neighbors[i];
                if (ncol == -1)
                    throw new Exception("invalid neighbor index");
                if (!utilMat.contains(row, ncol))
                    continue;

                else
                {
                    double value = utilMat.get(row, ncol);
                    double simScore = simScores[i];
                    sum += value * simScore;
                    normalization_factor += Math.Abs(simScore);
                    j++;
                }
            }

            rtn = sum / normalization_factor;
            if (double.IsNaN(rtn))
                return Double.NaN;
            rtn = utilMat.deNorm(row, col, rtn);
            return rtn;

        }

        #endregion 

    }
}
