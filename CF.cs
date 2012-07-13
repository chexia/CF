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

        public void buildModel(int k=5)
        {
            predictionResults = new Matrix(utilMat.GetLength(0), utilMat.GetLength(1));
            Parallel.For<Matrix>(0, utilMat.GetLength(1),
                () => {
                    Matrix rtn = new Matrix(utilMat.GetLength(0), 1);
                    return rtn;
                },
                (col, state, local) =>
                {
                    if (!utilMat.hashMap.ContainsKey((int)col))
                        return local;
                    local.set(1, 1, col);
                    int[] neighbors = this.myLSH.allCandidates((int)col);
                    double[] simScores = this.utilMat.sim((int)col, neighbors);
                    Array.Sort<double, int>(simScores, neighbors);
                    Array.Reverse(simScores);
                    Array.Reverse(neighbors);

                    for (int row = 0; row < utilMat.GetLength(0); row++)
                    {
                        if (utilMat.contains(row, (int)col))
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
                        if (!Double.IsNaN(sum/denom))
                            local.set(row, (int)col, sum / denom);
                    }
                    return local;
                },
                (local) =>
                {
                    int col=(int)local.get(1,1);
                    lock (utilMat)
                    {
                        foreach (int row in utilMat.getRowsOfCol(col))
                            utilMat.set(row, col, local.get(row,0));
                    }
                }
            );
                

        }

        public void buildModelL(int k=5)
        {
            predictionResults = new Matrix(utilMat.GetLength(0), utilMat.GetLength(1));
            int progress = 0;
            int total = utilMat.getCols().Count();
            for (int col = 0; col < utilMat.GetLength(1); col++)
            {
                if (!utilMat.hashMap.ContainsKey(col))
                    continue;

                progress++;
                Console.WriteLine("progress:{0}", (double)progress/(double)total);

                int[] neighbors = this.myLSH.allCandidates(col);
                double[] simScores = this.utilMat.sim(col, neighbors);
                Array.Sort<double, int>(simScores, neighbors);
                Array.Reverse(simScores);
                Array.Reverse(neighbors);

                for (int row = 0; row < utilMat.GetLength(0); row++)
                {
                    if (utilMat.contains(row,col))
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
                    for (i = 0; i < k; i++)
                    {
                        sum += utilMat.get(row, kneighbors[i]) * kscores[i];
                        denom += kscores[i];
                    }
                    utilMat.set(row, col, sum / denom);
                }
            }
        }

        public CF(Matrix utilMat, bool usingLSH = true, int r = 10, int b = 20, bool norm = true)
        {
            this.utilMat = utilMat;
            if (norm)
                utilMat.normalize();
            if (usingLSH)
                this.myLSH = new LSH(utilMat, r, b, this);

        }


        private int[] allCandidates(int col, int row)
        {
            if (this.myLSH == null)
            {
                List<int> candidates = new List<int>(); ;
                for (int i = 0; i < utilMat.GetLength(1); i++)
                {
                    if (utilMat.get(row, i) != -1 && i != col)
                        candidates.Add(i);
                }
                return candidates.ToArray();
            }
            else
            {
                return this.myLSH.allCandidates(col, row);
            }
        }
        public Tuple<int[], double[]> kNearestNeighbors(int principal, int row, int k)
        {
            Tuple<int[], double[]> rtn = new Tuple<int[], double[]>(new int[k], new double[k]);
            int[] allCandidates = this.allCandidates(principal, row);
            int len = allCandidates.Length;
            if (len < k)
            {
                int i;
                for (i = 0; i < allCandidates.Length; i++)
                {
                    rtn.Item1[i] = allCandidates[i];
                    rtn.Item2[i] = utilMat.cosineSim(principal, allCandidates[i]) * utilMat.jacSim(principal, allCandidates[i]); //investivate merits
                }
                for (; i < k; i++)
                {
                    rtn.Item1[i] = -1;
                    rtn.Item2[i] = 0;
                }
                Array.Sort(rtn.Item2, rtn.Item1);
                Array.Reverse(rtn.Item2);
                Array.Reverse(rtn.Item1);
            }
            else
            {
                double[] cosSim = utilMat.sim(principal, allCandidates);
                Array.Sort(cosSim, allCandidates); //important, bad implementation right here, will want to optimize later
                for (int i = 0; i < k; i++)
                {
                    rtn.Item1[i] = allCandidates[len - 1 - i];
                    rtn.Item2[i] = cosSim[len - 1 - i];
                }
            }
            return rtn;
        }
        public double predict(int[] kneighbors, double[] ksimMeasure, int row, int col)
        {
            if (this.utilMat.get(row, col) != -1)
            {
                //return this.utilMat.get(row, col);
            }
            double rtn = 0;
            double sum = 0;
            double normalization_factor = 0;
            for (int i = 0; i < kneighbors.Length; i++)
            {
                if (kneighbors[i] == -1)
                    continue;
                if (utilMat.get(row, kneighbors[i]) == -1)
                    continue;
                double a = utilMat.get(row, kneighbors[i]);
                double b = ksimMeasure[i];

                sum += utilMat.get(row, kneighbors[i]) * ksimMeasure[i];//important, consider using square
                normalization_factor += Math.Abs(ksimMeasure[i]); //important, think harder about this later.
            }

            rtn = sum / normalization_factor;
            if (double.IsNaN(rtn))
            {
                return Double.NaN;
            }

            rtn = rtn * utilMat.setDev[col] + utilMat.setAvg[col];
            return rtn;

        }
        public double predict(int row, int col, bool noEstimate = false)
        {
            if (noEstimate)
            {
                if (!this.utilMat.contains(row, col))
                    return Double.NaN;//throw new Exception("cannot predict without estimate");
                return this.utilMat.get(row, col) * this.utilMat.setDev[col] + this.utilMat.setAvg[col];
            }
            if (!this.utilMat.hashMap.ContainsKey(col))
                return double.NaN;
            if (this.utilMat.contains(row, col))
            {
                throw new Exception("training set test set overlap");
                return this.utilMat.get(row, col) * this.utilMat.setDev[col] + this.utilMat.setAvg[col];
            }
            Tuple<int[], double[]> ns = this.kNearestNeighbors(col, row, 5);
            int[] kneighbors = ns.Item1;
            double[] ksimMeasure = ns.Item2;
            double rtn = this.predict(kneighbors, ksimMeasure, row, col);

            return rtn;
        }
        public void iterate(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                for (int row = 0; row < utilMat.GetLength(0); row++)
                {
                    for (int col = 0; col < utilMat.GetLength(1); col++)
                    {
                        Tuple<int[], double[]> ns = kNearestNeighbors(col, row, 5);
                        double maxConfidence = Double.MinValue;
                        for (int ind = 0; ind < 5; ind++)
                        {
                            if (ns.Item2[ind] > maxConfidence)
                                maxConfidence = ns.Item2[ind];
                        }
                        if (maxConfidence < 0.7)
                            continue;
                        if (utilMat.get(row, col) != -1)
                            utilMat.set(row, col, (utilMat.get(row, col) + predict(ns.Item1, ns.Item2, row, col)) / 2);
                        else
                            utilMat.set(row, col, predict(ns.Item1, ns.Item2, row, col));
                    }
                }
            }
        }
    }

}
