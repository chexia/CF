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

    class Tester
    {
        private CF filter;
        private Matrix testPoints;
        private StreamWriter writer;
        public Tester(CF filter, Matrix testPoints)
        {
            this.filter = filter;
            this.testPoints = testPoints;
        }

        public void abtest(string outputFilePath)
        {
            this.writer = new StreamWriter(outputFilePath);
            Action<int> act = processCol;
            Parallel.ForEach<int>(testPoints.hashMap.Keys, act);
            //Parallel.ForEach<int, List<double>>(testPoints.hashMap.Keys, () => new List<double>(), processCol2, aggregateResult);
            //foreach (int key in testPoints.mat.hashMap.Keys)
            //    act(key);
            writer.Close();
            IO.accumulateResult(outputFilePath, outputFilePath + ".txt");
        }
        private List<double> processCol2(int col, ParallelLoopState useless, List<double> local)
        {
            //Console.WriteLine(col);
            foreach (int row in testPoints.getRowsOfCol(col))
            {
                double APE;
                double trueVal = testPoints.get(row, col);
                double predictedVal = filter.predict(row, col);

                if (double.IsNaN(predictedVal))
                    APE = 1;
                else
                    APE = Math.Abs(predictedVal - trueVal) / (trueVal);
                if (trueVal == 0)
                {
                    //continue;
                    APE = 1;
                }
                local.Add(APE);
            }
            return local;
        }
        private void aggregateResult(List<double> local)
        {
            lock (writer)
            {
                foreach (double toWrite in local)
                    writer.WriteLine(toWrite);
            }
        }
        private void processCol(int col)
        {
            //Console.WriteLine(col);
            foreach (int row in testPoints.getRowsOfCol(col))
            {
                double APE;
                double trueVal = testPoints.get(row, col);
                double predictedVal = filter.predict(row, col);

                if (double.IsNaN(predictedVal))
                    APE = 1;
                else
                    APE = Math.Abs(predictedVal - trueVal) / (trueVal);
                if (trueVal == 0)
                {
                    //continue;
                    APE = 1;
                }
                lock (writer)
                {
                    writer.WriteLine("{0}\t{1}\t{2}", APE, row, col);
                }
            }
        }
        public static void speedTest(int req, string trainPath)
        {

            StreamReader reader = File.OpenText(string.Format(trainPath));
            List<double[]> points = new List<double[]>();
            LogEnum logenum = new LogEnum(trainPath);
            int maxRow = 0;
            int maxCol = 0;
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                double clicks = 0;
                double views = 0;
                clicks = Double.Parse(tokens[3]);
                views = Double.Parse(tokens[2]);
                if (views < req)
                    continue;
                maxRow = Math.Max(maxRow, int.Parse(tokens[0]));
                maxCol = Math.Max(maxCol, int.Parse(tokens[1]));
                points.Add(new double[3] { Double.Parse(tokens[0]), Double.Parse(tokens[1]), Math.Min(clicks, views) / views });
            }

            Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);

            CF filter = new CF(utilMat);
            Console.WriteLine("Check 3");
            //filter.buildModel();

            reader.Close();

            Console.Write("debug: completed speed");
        }
        public static void ABTest_c(int s, int e, int step, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            StreamReader reader = File.OpenText(testPath);
            List<double[]> points = new List<double[]>();

            reader.Close();
            Console.WriteLine("Check 1");
            Matrix testPts = LogProcess.makeUtilMat(932, 528935, testPath, rowPos, colPos);
            Console.WriteLine("check 2");

            for (int req = s; req <= e; req += step)
            {
                reader = File.OpenText(string.Format(trainPath));
                points = new List<double[]>();
                LogEnum logenum = new LogEnum(trainPath);
                int maxRow = 0;
                int maxCol = 0;
                foreach (string line in logenum)
                {
                    string[] tokens = line.Split(new char[] { '\t' });
                    double clicks = 0;
                    double views = 0;
                    if (valPos == -1)
                    {
                        clicks = Double.Parse(tokens[3]);
                        views = Double.Parse(tokens[2]);
                    }
                    if (clicks < req)
                        continue;
                    maxRow = Math.Max(maxRow, int.Parse(tokens[rowPos]));
                    maxCol = Math.Max(maxCol, int.Parse(tokens[colPos]));
                    points.Add(new double[3] { Double.Parse(tokens[rowPos]), Double.Parse(tokens[colPos]), valPos == -1 ? Math.Min(clicks, views) / views : Double.Parse(tokens[2]) });
                }

                Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);

                CF filter = new CF(utilMat);
                Console.WriteLine("Check 3");
                Tester tester = new Tester(filter, testPts);
                tester.abtest(outputPrefix + "about_" + req + ".txt");

                reader.Close();
            }

            Console.Write("debug: completed AB");
        }

        public static void ABTest(int s, int e, int step, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            StreamReader reader = File.OpenText(testPath);
            List<double[]> points = new List<double[]>();

            reader.Close();
            Console.WriteLine("Check 1");
            Matrix testPts = LogProcess.makeUtilMat(932, 528935, testPath, rowPos, colPos);
            Console.WriteLine("check 2");

            for (int req = s; req <= e; req += step)
            {
                reader = File.OpenText(string.Format(trainPath));
                points = new List<double[]>();
                LogEnum logenum = new LogEnum(trainPath);
                int maxRow = 0;
                int maxCol = 0;
                foreach (string line in logenum)
                {
                    string[] tokens = line.Split(new char[] { '\t' });
                    double clicks = 0;
                    double views = 0;
                    if (valPos == -1)
                    {
                        clicks = Double.Parse(tokens[3]);
                        views = Double.Parse(tokens[2]);
                    }
                    if (views < req)
                        continue;
                    maxRow = Math.Max(maxRow, int.Parse(tokens[rowPos]));
                    maxCol = Math.Max(maxCol, int.Parse(tokens[colPos]));
                    points.Add(new double[3] { Double.Parse(tokens[rowPos]), Double.Parse(tokens[colPos]), valPos == -1 ? Math.Min(clicks, views) / views : Double.Parse(tokens[2]) });
                }
                Console.WriteLine("Check 3");
                Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);

                CF filter = new CF(utilMat);
                Tester tester = new Tester(filter, testPts);
                tester.abtest(outputPrefix + "about_" + req + ".txt");
                reader.Close();
            }

            Console.Write("debug: completed AB");
        }


        public static void ABTest_h(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            StreamReader reader = File.OpenText(testPath);
            List<double[]> points = new List<double[]>();

            reader.Close();
            Console.WriteLine("Check 1");
            Matrix testPts = LogProcess.makeUtilMat(932, 528935, testPath, rowPos, colPos);
            Console.WriteLine("check 2");

            for (int vreq = s; vreq <= e; vreq += step)
            {
                for (int creq = s2; creq <= e2; creq += step2)
                {
                    int numvalidentries = 0;
                    reader = File.OpenText(string.Format(trainPath));
                    points = new List<double[]>();
                    LogEnum logenum = new LogEnum(trainPath);
                    int maxRow = 0;
                    int maxCol = 0;
                    foreach (string line in logenum)
                    {
                        string[] tokens = line.Split(new char[] { '\t' });
                        double clicks = 0;
                        double views = 0;
                        if (valPos == -1)
                        {
                            clicks = Double.Parse(tokens[3]);
                            views = Double.Parse(tokens[2]);
                        }
                        if (views < vreq || clicks < creq)
                            continue;
                        maxRow = Math.Max(maxRow, int.Parse(tokens[rowPos]));
                        maxCol = Math.Max(maxCol, int.Parse(tokens[colPos]));
                        points.Add(new double[3] { Double.Parse(tokens[rowPos]), Double.Parse(tokens[colPos]), valPos == -1 ? Math.Min(clicks, views) / views : Double.Parse(tokens[2]) });
                        numvalidentries += 1;
                    }

                    Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);

                    CF filter = new CF(utilMat);
                    Console.WriteLine("Check 3");
                    //filter.buildModel();
                    Tester tester = new Tester(filter, testPts);
                    tester.abtest(outputPrefix + "about_" + vreq + "_" + creq + ".txt");
                    Console.WriteLine("completed test: minview:{0}\tminclick:{1}\tnumvalidentries:{2}", vreq, creq, numvalidentries);
                    reader.Close();
                }
            }

            Console.WriteLine("debug: completed AB");
        }
        public static void ABTest_hl(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            StreamReader reader = File.OpenText(testPath);
            List<double[]> points = new List<double[]>();

            reader.Close();
            Console.WriteLine("Check 1");
            Matrix testPts = LogProcess.makeUtilMat(932, 528935, testPath, rowPos, colPos);
            Console.WriteLine("check 2");

            for (int vreq = s; vreq <= e; vreq += step)
            {
                for (int creq = s2; creq <= e2; creq += step2)
                {
                    int numvalidentries = 0;
                    reader = File.OpenText(string.Format(trainPath));
                    points = new List<double[]>();
                    LogEnum logenum = new LogEnum(trainPath);
                    int maxRow = 0;
                    int maxCol = 0;
                    foreach (string line in logenum)
                    {
                        string[] tokens = line.Split(new char[] { '\t' });
                        double clicks = 0;
                        double views = 0;
                        if (valPos == -1)
                        {
                            clicks = Double.Parse(tokens[3]);
                            views = Double.Parse(tokens[2]);
                        }
                        if (views < vreq || clicks < creq)
                            continue;
                        maxRow = Math.Max(maxRow, int.Parse(tokens[rowPos]));
                        maxCol = Math.Max(maxCol, int.Parse(tokens[colPos]));
                        points.Add(new double[3] { Double.Parse(tokens[rowPos]), Double.Parse(tokens[colPos]), valPos == -1 ? Math.Min(clicks, views) / views : Double.Parse(tokens[2]) });
                        numvalidentries += 1;
                    }
                    Console.WriteLine("Check 3");
                    Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);

                    CF filter = new CF(utilMat);
                    //filter.buildModelL();
                    Tester tester = new Tester(filter, testPts);
                    tester.abtest(outputPrefix + "about_" + vreq + "_" + creq + ".txt");
                    Console.WriteLine("completed test: minview:{0}\tminclick:{1}\tnumvalidentries:{2}", vreq, creq, numvalidentries);
                    reader.Close();
                }
            }

            Console.WriteLine("debug: completed AB");
        }
        public static void blockTest(string in_mavc, string in_msi, string in_iavc)
        {
            int[] mia = LogProcess.cleanLogs0(in_mavc, in_msi, in_iavc);
            Console.WriteLine("mia:{0}, {1}, {2}", mia[0], mia[1], mia[2]);
            //int[] mia = new int[3] { 448397, 274, 2462 };

            Matrix ad_muid_view = LogProcess.makeUtilMat(mia[1], mia[0], "mavc_processed.log", 1, 0, 2);
            Matrix ad_muid_click = LogProcess.makeUtilMat(mia[1], mia[0], "mavc_processed.log", 1, 0, 3);
            Matrix intent_muid = LogProcess.makeUtilMat(mia[1], mia[0], "msi_processed.log", 2, 0, 1);
            Matrix intent_ad = LogProcess.makeUtilMat(mia[1], mia[2], "iavc_processed.log", 0, 1, -1);
            CF filter = new CF(intent_ad);

            Console.WriteLine("check 1");
            StreamWriter writer = new StreamWriter("C:\\Users\\t-chexia\\Desktop\\blocktest\\blockTestOutput.txt");
            Parallel.For(1, 200, numAdi =>
            {
                int numAd = numAdi * 5;
                for (int numUsr = 10; numUsr <= 5000; numUsr += 10)
                {
                    testFixedBlock(numUsr, numAd, ad_muid_click, ad_muid_view, intent_muid, filter, writer);
                }
            });
            /*
            for (int numAd = 10; numAd <=100; numAd += 10)
            {
                for (int numUsr = 50; numUsr <= 1000; numUsr += 50)
                {
                    double APE = testFixedBlock(numUsr, numAd, ad_muid_click, ad_muid_view, intent_muid, filter);
                }
            }
             * */
        }


        public static double testFixedBlock(int numUsr, int numAd, Matrix ad_muid_click, Matrix ad_muid_view, Matrix intent_muid, CF filter, StreamWriter writer, string outputFile = "C:\\Users\\t-chexia\\Desktop\\blocktest\\blockTestOutput.txt")
        {
            //StreamWriter writer = new StreamWriter(outputFile, true);
            int tries = 10;
            double[] apes = new double[tries];
            for (int i = 0; i < tries; i++)
            {
                HashSet<int> userSample = ad_muid_click.randomSubset(numUsr, 1);
                HashSet<int> adSample = ad_muid_click.randomSubset(numAd, 0);
                double numClick = 0;
                double numView = 0;
                double numPredictedView = 0;
                double numPredictedClick = 0;
                foreach (int user in userSample)
                {
                    foreach (int ad in adSample)
                    {
                        if (!ad_muid_click.contains(ad, user) || !intent_muid.hashMap.ContainsKey(user))
                            continue;
                        numClick += ad_muid_click.get(ad, user);
                        numView += ad_muid_view.get(ad, user);
                        double ctr = predictUserAdCtr(user, ad, intent_muid, filter);
                        if (double.IsNaN(ctr))
                            continue;
                        double xview = ad_muid_view.get(ad, user);
                        double xclick = ad_muid_click.get(ad, user);
                        numPredictedView += ad_muid_view.get(ad, user);
                        numPredictedClick += ctr * ad_muid_view.get(ad, user);
                        //double dummy = recomputeIntentAdCtr(ad, user, ad_muid_click, ad_muid_view, intent_muid, filter);
                        //if (ad_muid_click.get(ad, user) != 0)
                        //    dummy += 0;
                    }
                }
                double trueCTR = numClick / numView;
                if (trueCTR == 0)
                {
                    apes[i] = 1;
                    continue;
                }
                double predictedCTR = numPredictedClick / numPredictedView;
                apes[i] = Math.Abs(trueCTR - predictedCTR) / trueCTR;
                if (double.IsNaN(apes[i]))
                    apes[i] = 1;
                Console.WriteLine("iteration:{0}\t trueCTR:{1}\t predictedCTR:{2}\tnumUser:\tnumAd:{4}", i, trueCTR, predictedCTR, numUsr, numAd);
                lock (writer)
                {
                    writer.WriteLine("iteration:{0}\t trueCTR:{1}\t predictedCTR:{2}\tnumUser:{3}\tnumAd:{4}", i, trueCTR, predictedCTR, numUsr, numAd);
                }
            }
            double sum = 0;
            foreach (double i in apes)
                sum += i;
            double avg = sum / tries;
            Console.WriteLine("avg:{0}\tnumUser:{1}\tnumAd:{2}", avg, numUsr, numAd);
            //if (double.IsNaN(avg))
            //    return 0;
            lock (writer)
            {
                writer.WriteLine("avg:{0}\tnumUser:{1}\tnumAd:{2}", avg, numUsr, numAd);
            }
            //writer.Close();
            return avg;
        }

        private static double recomputeIntentAdCtr(int ad, int user, Matrix ad_muid_click, Matrix ad_muid_view, Matrix intent_muid, CF filter)
        {
            int principalIntent = 0;
            double principalScore = double.MinValue;

            foreach (int intent in intent_muid.hashMap[user].Keys)
            {
                double score = intent_muid.get(intent, user);

                if (score > principalScore)
                {
                    principalScore = score;
                    principalIntent = intent;
                }
            }
            double view = 0;
            double click = 0;
            foreach (int muid in intent_muid.hashMap.Keys)
                if (intent_muid.hashMap[muid].ContainsKey(principalIntent))
                    if (ad_muid_click.contains(ad, muid))
                    {
                        view += ad_muid_view.get(ad, muid);
                        click += ad_muid_click.get(ad, muid);
                    }
            return click / view;
        }
        private static double predictUserAdCtr(int user, int ad, Matrix intent_muid, CF filter)
        {
            int principalIntent = 0;
            double principalScore = double.MinValue;

            double ctrSum = 0;
            double denom = 0;

            foreach (int intent in intent_muid.hashMap[user].Keys)
            {
                double score = intent_muid.get(intent, user);
                //double ctr = filter.predict(intent, ad);
                //ctrSum += score * ctr;
                //denom += score;

                if (score > principalScore)
                {
                    principalScore = score;
                    principalIntent = intent;
                }
            }
            double rtn = filter.predict(principalIntent, ad);
            return rtn;
        }


    }

}
