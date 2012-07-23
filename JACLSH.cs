﻿using System;
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
    class JACtest
    {
        public static void jacSplitTest2(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5, int numrec = 5)
        {
            int[] ui = cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            split("jac_usi_processed.log");
            ZOMatrix testMat = makeUtilMat(ui[1], ui[0], "jac_test.log");
            ZOMatrix traintMat = makeUtilMat(ui[1], ui[0], "jac_train.log");
            JACCF filter = new JACCF(traintMat, false, 5, 10, false);
            int[] final = new int[4] { 0, 0, 0, 0 };
            Parallel.For<MultiDictionary<double, int>>(0, ui[0],
                                () => new MultiDictionary<double, int>(true),
                                (user, state, stats) =>
                                {
                                    for (int intent = 0; intent < ui[1]; intent += 1)
                                    {
                                        if (filter.utilMat.contains(intent, user))
                                            continue;
                                        Double predictedVal = filter.predict(intent, (int)user, false, threshold);
                                        double[] valarr = stats.Keys.ToArray<double>();
                                        if (!stats.ContainsKey(1000))
                                            stats.Add(1000, (int)user);
                                        //Console.WriteLine(predictedVal);
                                        if (stats.Values.Count <= numrec)
                                        {
                                            stats.Add(predictedVal, intent);
                                        }
                                        else if (valarr.Min() < predictedVal)
                                        {
                                            stats.Remove(valarr.Min());
                                            stats.Add(predictedVal, intent);
                                        }
                                    }
                                    //Console.WriteLine(stats.Count);
                                    return stats;
                                },
                                 (stats) =>
                                 {
                                     int[] err = new int[4] { 0, 0, 0, 0 };
                                     foreach (int intent in stats.Values.ToArray<int>())
                                     {

                                         int user = stats[1000].First<int>();
                                         double trueVal = testMat.get(intent, user);
                                         double predictedVal = 1;

                                         if (trueVal == predictedVal && trueVal == 1)
                                             err[0]++;
                                         else if (trueVal == predictedVal && trueVal == 0)
                                             err[1]++;
                                         else if (trueVal == 1)
                                             err[3]++;
                                         else
                                             err[2]++;
                                     }
                                     lock (final)
                                     {
                                         final[0] += err[0];
                                         final[1] += err[1];
                                         final[2] += err[2];
                                         final[3] += err[3];
                                     }
                                 });

            Console.WriteLine("test 2: truePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}", final[0], final[1], final[2], final[3]);
            StreamWriter writer = new StreamWriter(outputPath, true);
            writer.WriteLine("test 2: truePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}\tthreshold:{4}\tfile:{5}", final[0], final[1], final[2], final[3], threshold, inputData);
            writer.Close();
        }
        public static void jacSplitTest(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5, int neighbors = 5, double confidence = 0, double preserve = 0.8)
        {
            int[] ui = cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            split("jac_usi_processed.log");
            ZOMatrix testMat = makeUtilMat(ui[1], ui[0], "jac_test.log");
            ZOMatrix traintMat = makeUtilMat(ui[1], ui[0], "jac_train.log");
            JACCF filter = new JACCF(traintMat, false, 5, 10, false, preserve);
            int[] final = new int[4] { 0, 0, 0, 0 };
            Parallel.For<int[]>(0, ui[0],
                                () => new int[4] { 0, 0, 0, 0 },
                                (user, state, stats) =>
                                {
                                    for (int intent = 0; intent < ui[1]; intent += 1)
                                    //if (!testMat.mat.hashMap.ContainsKey(user))
                                    //    return stats;
                                    //foreach (int intent in testMat.mat.hashMap[user].Keys)
                                    {
                                        Double trueVal = testMat.get(intent, user);
                                        Double predictedVal = filter.predict(intent, user, false, threshold, neighbors, confidence);
                                        if (Double.IsNaN(predictedVal))
                                            predictedVal = 0;
                                        if (trueVal == predictedVal && trueVal == 1)
                                            stats[0]++;
                                        else if (trueVal == predictedVal && trueVal == 0)
                                            stats[1]++;
                                        else if (trueVal == 1)
                                            stats[3]++;
                                        else
                                            stats[2]++;
                                        //Console.WriteLine("truePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}", stats[0], stats[1], stats[2], stats[3]);
                                    }
                                    return stats;
                                },
                                 (stats) =>
                                 {
                                     lock (final)
                                     {
                                         final[0] += stats[0];
                                         final[1] += stats[1];
                                         final[2] += stats[2];
                                         final[3] += stats[3];
                                     }
                                 });

            Console.WriteLine("truePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}", final[0], final[1], final[2], final[3]);
            StreamWriter writer = new StreamWriter(outputPath, true);
            writer.WriteLine("precision:{8}\trecall:{9}\ttruePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}\tthreshold:{4}\tfile:{5}\tneighbors:{6}\tconfidence:{7}", final[0], final[1], final[2], final[3], threshold, inputData, neighbors, confidence, (double)final[0] / (final[0] + final[2]), (double)final[0] / (final[0] + final[3]));
            writer.Close();
        }

        #region utility methods

        /*
        public static void jacSplitTest(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5) 
        {
            int[] ui = cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            split("jac_usi_processed.log");
            JACMatrix testMat = makeUtilMat(ui[1], ui[0], "jac_test.log");
            JACMatrix traintMat = makeUtilMat(ui[1], ui[0], "jac_train.log");
            JACCF filter = new JACCF(traintMat, false, 5, 10, false);
            int truePos, trueNeg, falsePos, falseNeg;
            truePos = trueNeg = falsePos = falseNeg = 0;

            for (int user=0; user< ui[0]; user+=1)
                for (int intent=0; intent< ui[1]; intent+=1)
                {

                    Double trueVal = testMat.get(intent, user);
                    Double predictedVal = filter.predict(intent, user);
                    if (Double.IsNaN(predictedVal))
                        continue;
                    if (falsePos>3000 && predictedVal==1)
                        trueVal+=0;
                    int a = 0;
                    if (falsePos > trueNeg)
                        a = 1 + 1;
                    if (trueVal == predictedVal && trueVal == 1)
                        truePos++;
                    else if (trueVal == predictedVal && trueVal == 0)
                        trueNeg++;
                    else if (trueVal == 1)
                        falseNeg++;
                    else
                        falsePos++;
                    Console.WriteLine("truePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}", truePos, trueNeg, falsePos, falseNeg);
                }


        }
        */


        public static void split(string inputData, string out_test = "jac_test.log", string out_train = "jac_train.log")
        {
            LogEnum logenum = new LogEnum(inputData);
            Random randgen = new Random();
            StreamWriter testWriter = File.CreateText(out_test);
            StreamWriter trainWriter = File.CreateText(out_train);
            HashSet<string> seen = new HashSet<string>();

            HashSet<string> exclude = new HashSet<string>();
            LogEnum black = new LogEnum("C:\\Users\\t-chexia\\Desktop\\usi_600_histogram.log");
            foreach (string line in black)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                int count = Int32.Parse(tokens[1]);
                if (count < 200)
                    break;
                exclude.Add(tokens[0]);
            }


            foreach (string line in logenum)
            {
                if (seen.Contains(line))
                {
                    //Console.WriteLine("duplicate");
                    continue;
                }
                string[] tokens = line.Split(new char[] { '\t' });
                if (exclude.Contains(tokens[0]))
                {
                    Console.WriteLine(tokens[0]);
                    continue;
                }
                seen.Add(line);
                if (randgen.NextDouble() < 0.3)
                    testWriter.WriteLine(line);
                else
                    trainWriter.WriteLine(line);
            }
            testWriter.Close();
            trainWriter.Close();
        }
        public static int[] cleanLogsj(string in_train, string out_train = "jac_usi_processed.log")
        {
            IntegerMap u2int = new IntegerMap();
            u2int.add(in_train, 0);
            IntegerMap i2int = new IntegerMap();
            i2int.add(in_train, 2);

            rewrite(in_train, out_train, standardProcessor, u2int, i2int);
            return new int[2] { u2int.getCount(), i2int.getCount() };
        }
        public static string standardProcessor(string line, IntegerMap a, IntegerMap b)
        {
            string[] tokens = line.Split();
            tokens[0] = "" + a.get(tokens[0]);
            tokens[1] = "" + 1;
            tokens[2] = "" + b.get(tokens[2]);
            return string.Join("\t", tokens);
        }
        public static void rewrite(string inputFilePath, string outputFilePath, Func<string, IntegerMap, IntegerMap, string> processor, IntegerMap rowMap = null, IntegerMap colMap = null)
        {
            HashSet<string> exclude = new HashSet<string>();
            LogEnum logenum = new LogEnum(inputFilePath);
            StreamWriter writer = File.CreateText(outputFilePath);
            LogEnum black = new LogEnum("C:\\Users\\t-chexia\\Desktop\\usi_600_histogram.log");
            foreach (string line in black)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                int count = Int32.Parse(tokens[1]);
                if (count < 200)
                    break;
                exclude.Add(tokens[0]);
            }

            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                if (exclude.Contains(tokens[2]))
                {
                    //Console.WriteLine(tokens[2]);
                    continue;
                }
                writer.WriteLine(processor(line, rowMap, colMap));
            }
            writer.Close();
        }
        public static ZOMatrix makeUtilMat(int rowNum, int colNum, string inputFilePath, int rowPos = 2, int colPos = 0, int valPos = 1)
        {
            List<double[]> points = new List<double[]>();
            LogEnum logenum = new LogEnum(inputFilePath);
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                double[] point = new double[3];
                point[0] = Double.Parse(tokens[rowPos]);
                point[1] = Double.Parse(tokens[colPos]);
                if (valPos == -1)
                    point[2] = Double.Parse(tokens[3]) / Double.Parse(tokens[2]);
                else
                    point[2] = Double.Parse(tokens[valPos]);
                points.Add(point);
            }
            ZOMatrix utilMat = new ZOMatrix(rowNum, colNum, points);
            return utilMat;
        }
        #endregion
    }

    #region JACCF
    [Serializable()]
    class JACCF
    {
        public ZOMatrix utilMat;
        public ZOMatrix simMat;
        public JACLSH myLSH;


        public JACCF(ZOMatrix utilMat, bool usingLSH = true, int r = 10, int b = 20, bool norm = true, double preserve = 0.8)
        {
            this.utilMat = utilMat;
            if (norm)
                utilMat.normalize();
            if (usingLSH)
                //this.myLSH = new JACLSH(utilMat, r, b, this);
            this.simMat = PCA.dmR(utilMat, preserve);

        }

        #region prediction code
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
                return this.myLSH.allNeighbors(col, row);
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
                    rtn.Item2[i] = simMat.sim(principal, allCandidates[i]);// *utilMat.jacSim(principal, allCandidates[i]); investigate merits
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
                double[] cosSim = simMat.sim(principal, allCandidates);
                Array.Sort(cosSim, allCandidates); //important, bad implementation right here, will want to optimize later
                for (int i = 0; i < k; i++)
                {
                    rtn.Item1[i] = allCandidates[len - 1 - i];
                    rtn.Item2[i] = cosSim[len - 1 - i];
                }
            }
            return rtn;
        }
        public double predict(int[] kneighbors, double[] ksimMeasure, int row, int col, double confidence = 0)
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
                if (b < confidence)
                    continue;

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
        #endregion

        public double predict(int row, int col, bool noEstimate = false, double threshold = 0.5, int neighbors = 10, double confidence = 0, bool round = true)
        {
            if (noEstimate)
            {
                if (!this.utilMat.contains(row, col))
                    return Double.NaN;//throw new Exception("cannot predict without estimate");
                return this.utilMat.get(row, col) * this.utilMat.setDev[col] + this.utilMat.setAvg[col];
            }
            if (!this.utilMat.sourceMatrix.ContainsKey(col))
                return double.NaN;
            Tuple<int[], double[]> ns = this.kNearestNeighbors(col, row, neighbors);
            int[] kneighbors = ns.Item1;
            double[] ksimMeasure = ns.Item2;
            double rtn = this.predict(kneighbors, ksimMeasure, row, col, confidence);
            if (!round)
                return rtn;
            if (Double.IsNaN(rtn))
                return double.NaN;
            return rtn < threshold ? 0 : 1;
        }
    }
    #endregion

    #region JACLSH
    [Serializable()]
    class JACLSH
    {
        private int numSets;
        private int r, b;
        private Dictionary<int, int>[] sigMat; //gets hash value from column
        private MultiDictionary<int, int>[] revSigMat; //gets list of columns with given hash code
        private CF filter;
        private Random randGen = new Random();
        public JACLSH(ZOMatrix utilMat, int r, int b, CF filter)
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
        private void compSigMatEntries(ZOMatrix utilMat)
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
