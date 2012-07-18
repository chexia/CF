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
    class ZOCF
    {
        public Matrix utilMat;
        public LSH myLSH;
        private Matrix predictionResults;

        public void buildModel(int k = 5)
        {
            double progress = 0;
            double total = utilMat.GetLength(1);
            predictionResults = new Matrix(utilMat.GetLength(0), utilMat.GetLength(1));
            Parallel.For<Matrix>(0, utilMat.GetLength(1),
                () =>
                {
                    Matrix rtn = new Matrix(utilMat.GetLength(0), 1);
                    return rtn;
                },
                (col, state, local) =>
                {
                    local.set(1, 1, col);
                    if (!utilMat.hashMap.ContainsKey((int)col))
                        return local;
                    progress++;
                    Tuple<int[], double[]> ns = this.getNeighborsScores(col);
                    int[] neighbors = ns.Item1;
                    double[] simScores = ns.Item2;


                    for (int row = 0; row < utilMat.GetLength(0); row++)
                    {
                        Double prediction = this.predict(neighbors, simScores, row, col, 5);
                        if (!Double.IsNaN(prediction))
                            local.set(row, 0, prediction);
                    }
                    return local;
                },
                (local) =>
                {
                    int col = (int)local.get(1, 1);
                    lock (utilMat)
                    {
                        if (local.hashMap.ContainsKey(0))
                            lock (predictionResults)
                            {
                                Console.WriteLine("progress: {0}", progress / total);
                                foreach (int row in local.getRowsOfCol(0))
                                    predictionResults.set(row, col, local.get(row, 0));
                            }
                        //else
                        //    if (utilMat.hashMap.ContainsKey(col))
                        //        throw new Exception("bla");
                    }
                }
            );


        }
        /*
        public void buildModelL(int k = 5)
        {
            predictionResults = new Matrix(utilMat.GetLength(0), utilMat.GetLength(1));
            for (int col = 0; col < utilMat.GetLength(1); col++)
            {
                if (!utilMat.hashMap.ContainsKey(col))
                    continue;

                int[] neighbors = this.myLSH.allNeighbors(col);
                double[] simScores = this.utilMat.sim(col, neighbors);
                Array.Sort<double, int>(simScores, neighbors);
                Array.Reverse(simScores);
                Array.Reverse(neighbors);

                for (int row = 0; row < utilMat.GetLength(0); row++)
                {
                    if (utilMat.contains(row, col))
                        continue;
                    int[] kneighbors = new int[k];
                    double[] kscores = new double[k];
                    int i = 0;
                    for (int ind = 0; ind < neighbors.Length; ind++)
                    {
                        int colAtInd = neighbors[ind];
                        if (utilMat.contains(row, colAtInd))
                        {
                            kneighbors[i] = neighbors[ind];
                            kscores[i] = simScores[ind];
                            i++;
                            if (i == k)
                                break;
                        }
                    }
                    double sum = 0;
                    double denom = 0;
                    for (int j = 0; j < i; j++)
                    {
                        sum += utilMat.get(row, kneighbors[j]) * kscores[j];
                        denom += kscores[j];
                    }
                    if (!Double.IsNaN(sum / denom))
                        predictionResults.set(row, (int)col, sum / denom);
                }
            }
        }
        */
        public ZOCF(Matrix utilMat, bool usingLSH = true, int r = 10, int b = 20, bool norm = true)
        {
            this.utilMat = utilMat;
            if (norm)
                utilMat.normalize();
            if (usingLSH)
                this.myLSH = new LSH(utilMat, r, b, this);

        }

        //return arrays in sorted order, most similar first
        private Tuple<int[], double[]> getNeighborsScores(int col)
        {
            int[] allNeighbors;
            if (this.myLSH == null)
            {
                List<int> candidates = new List<int>();
                for (int i = 0; i < utilMat.GetLength(1); i++)
                    if (i != col)
                        candidates.Add(i);
                allNeighbors = candidates.ToArray();
            }
            else
                allNeighbors = this.myLSH.allNeighbors(col);

            // done extracting allNeighbors, now compute similarity

            double[] neighborScores = this.utilMat.sim(col, allNeighbors);


            Array.Sort<double, int>(neighborScores, allNeighbors);
            Array.Reverse(neighborScores);
            Array.Reverse(allNeighbors);
            Tuple<int[], double[]> rtn = new Tuple<int[], double[]>(allNeighbors, neighborScores);
            return rtn;
        }

        private Tuple<int[], double[]> getNeighborsScores(int col, int row)
        {
            int[] allNeighbors;
            if (this.myLSH == null)
            {
                List<int> candidates = new List<int>();
                for (int i = 0; i < utilMat.GetLength(1); i++)
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

        public double predict(int[] neighbors, double[] simScores, int row, int col, int k)
        {
            double rtn = 0;
            double sum = 0;
            double normalization_factor = 0;
            for (int i = 0, j = 0; i < neighbors.Length && j < k; i++)
            {
                int ncol = neighbors[i];
                if (ncol == -1)
                    throw new Exception("should not have -1 as neighbor ind");
                if (!utilMat.contains(row, ncol))
                    continue;
                else
                {
                    double value = utilMat.get(row, ncol);
                    double simScore = simScores[i];
                    sum += value * simScore;//important, consider using square
                    normalization_factor += Math.Abs(simScore); //important, think harder about this later.
                    j++;
                }
            }

            rtn = sum / normalization_factor;
            if (double.IsNaN(rtn))
                return Double.NaN;

            rtn = rtn * utilMat.setDev[col] + utilMat.setAvg[col];
            return rtn;

        }
        public double predict(int row, int col)
        {
            if (!this.utilMat.hashMap.ContainsKey(col))
                return double.NaN;
            if (this.utilMat.contains(row, col))
            {
                //throw new Exception("training set test set overlap");
                return this.utilMat.get(row, col) * this.utilMat.setDev[col] + this.utilMat.setAvg[col];
            }
            if (this.predictionResults != null && this.predictionResults.contains(row, col))
                return this.predictionResults.get(row, col);
            Tuple<int[], double[]> ns = this.getNeighborsScores(col, row);
            int[] kneighbors = ns.Item1;
            double[] ksimMeasure = ns.Item2;
            double rtn = this.predict(kneighbors, ksimMeasure, row, col, 5);

            return rtn;
        }


    }

}
