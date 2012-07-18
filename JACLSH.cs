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
    class JACtest
    {
        public static void jacSplitTest2(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5, int numrec =5)
        {
            int[] ui = cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            split("jac_usi_processed.log");
            ZOMatrix testMat = makeUtilMat(ui[1], ui[0], "jac_test.log");
            ZOMatrix traintMat = makeUtilMat(ui[1], ui[0], "jac_train.log");
            ZOCF filter = new ZOCF(traintMat, false, 5, 10, false);
            int[] final = new int[4] { 0, 0, 0, 0 };
            Parallel.For<MultiDictionary<double,int>>(0, ui[0],
                                () => new MultiDictionary<double,int>(true),
                                (user, state, stats) =>
                                {
                                    for (int intent = 0; intent < ui[1]; intent += 1)
                                    {
                                        if (filter.utilMat.contains(intent, user))
                                            continue;
                                        Double predictedVal = filter.predict(intent, (int)user, false, threshold);
                                        double[] valarr = stats.Keys.ToArray<double>();
                                        if (!stats.ContainsKey(1000))
                                            stats.Add(1000, (int) user);
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
        public static void jacSplitTest(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5, int neighbors = 5, double confidence = 0, double preserve=0.8)
        {
            int[] ui = cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            split("jac_usi_processed.log");
            ZOMatrix testMat = makeUtilMat(ui[1], ui[0], "jac_test.log");
            ZOMatrix traintMat = makeUtilMat(ui[1], ui[0], "jac_train.log");
            ZOCF filter = new ZOCF(traintMat, false, 5, 10, false, preserve);
            //filter.buildModel(neighbors, confidence);
            int[] final = new int[4] { 0, 0, 0, 0 };
            Parallel.For<int[]>(0, ui[0],
                                () => new int[4] { 0, 0, 0, 0 },
                                (user, state, stats) =>
                                {
                                    for (int intent = 0; intent < ui[1]; intent += 1)
                                    {
                                        Double trueVal = testMat.get(intent, user);
                                        Double predictedVal = filter.predict(intent, user, false, threshold, neighbors, true);
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
            writer.WriteLine("precision:{8}\trecall:{9}\ttruePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}\tthreshold:{4}\tfile:{5}\tneighbors:{6}\tconfidence:{7}\t preserve:{10}", final[0], final[1], final[2], final[3], threshold, inputData, neighbors, confidence, (double)final[0]/(final[0]+final[2]), (double)final[0]/(final[0]+final[3]), preserve);
            writer.Close();
        }
        public static void jacSplitTestL(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5, int neighbors = 5, double confidence = 0, double preserve = 0.8)
        {
            int[] ui = cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            split("jac_usi_processed.log");
            ZOMatrix testMat = makeUtilMat(ui[1], ui[0], "jac_test.log");
            ZOMatrix traintMat = makeUtilMat(ui[1], ui[0], "jac_train.log");
            ZOCF filter = new ZOCF(traintMat, false, 5, 10, false, preserve);
            filter.buildModel(neighbors, confidence);
            int[] final = new int[4] { 0, 0, 0, 0 };
            for (int user = 0; user< ui[0]; user++){
                for (int intent = 0; intent < ui[1]; intent += 1)
                {
                    Double trueVal = testMat.get(intent, user);
                    Double predictedVal = filter.predict(intent, user, false, threshold, neighbors, true);
                    if (Double.IsNaN(predictedVal))
                        predictedVal = 0;
                    if (trueVal == predictedVal && trueVal == 1)
                        final[0]++;
                    else if (trueVal == predictedVal && trueVal == 0)
                        final[1]++;
                    else if (trueVal == 1)
                        final[3]++;
                    else
                        final[2]++;
                }
            };

            Console.WriteLine("truePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}", final[0], final[1], final[2], final[3]);
            StreamWriter writer = new StreamWriter(outputPath, true);
            writer.WriteLine("precision:{8}\trecall:{9}\ttruePos:{0}\ttrueNeg:{1}\tfalsePos:{2}\tfalseNeg:{3}\tthreshold:{4}\tfile:{5}\tneighbors:{6}\tconfidence:{7}\t preserve:{10}", final[0], final[1], final[2], final[3], threshold, inputData, neighbors, confidence, (double)final[0] / (final[0] + final[2]), (double)final[0] / (final[0] + final[3]), preserve);
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


        public static void split(string inputData, string out_test="jac_test.log", string out_train="jac_train.log")
        {
            LogEnum logenum = new LogEnum(inputData);
            Random randgen = new Random();
            StreamWriter testWriter = File.CreateText (out_test);
            StreamWriter trainWriter = File.CreateText (out_train);
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
        

   
}
