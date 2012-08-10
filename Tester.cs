using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using System.Diagnostics;

namespace CF
{

    class Tester
    {
        private CF filter;
        private Matrix testPoints;
        private StreamWriter writer;
        private StreamWriter writer2;
        private StreamWriter writer3;
        public Tester(CF filter, Matrix testPoints)
        {
            this.filter = filter;
            this.testPoints = testPoints;
        }


        public void abtest(string outputFilePath)
        {
            this.writer3 = new StreamWriter(outputFilePath + ".dir");
            this.writer2 = new StreamWriter(outputFilePath + ".avg");
            this.writer = new StreamWriter(outputFilePath);
            Action<int> act = processCol;
            Parallel.ForEach<int>(testPoints.sourceMatrix.Keys, act);
            //Parallel.ForEach<int, List<double>>(testPoints.hashMap.Keys, () => new List<double>(), processCol2, aggregateResult);
            //foreach (int key in testPoints.mat.hashMap.Keys)
            //    act(key);
            writer.Close();
            writer2.Close();
            writer3.Close();
            IO.produceCumulative(outputFilePath, outputFilePath + ".agg");
            IO.produceCumulative(outputFilePath + ".avg", outputFilePath + ".avg" + ".agg");
            IO.produceCumulative(outputFilePath + ".dir", outputFilePath + ".dir" + ".agg");
            IO.produceHistogram(outputFilePath, outputFilePath + ".hst");
            IO.produceHistogram(outputFilePath + ".avg", outputFilePath + ".avg" + ".hst");
            IO.produceHistogram(outputFilePath + ".dir", outputFilePath + ".dir" + ".hst");
            
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
                //predictedVal = predictedVal * 0.85 + (filter.utilMat.contains(row, col) ? filter.utilMat.deNorm(row, col, filter.utilMat.get(row, col)) : predictedVal) * 0.15;
                //
                //predictedVal = (predictedVal - filter.utilMat.setAvg[col]);
                //trueVal = trueVal - filter.utilMat.setAvg[col];
                //

                if (double.IsNaN(predictedVal))
                    APE = 1;
                else
                    APE = Math.Abs(predictedVal - trueVal) / Math.Abs(trueVal);

                //
                //APE = APE==1?1:predictedVal * trueVal > 0 ? 0 : 2;
                //

                if (trueVal == 0)
                {
                    if (predictedVal == 0)
                        APE = 0;
                    else
                        APE = 1;
                }

                lock (writer)
                {
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", APE, row, col,predictedVal,trueVal);
                }


                lock (writer2)
                {
                    predictedVal = filter.defaultPrediction(row, col);
                    APE = Math.Abs(predictedVal - trueVal) / Math.Abs(trueVal);
                    if (trueVal == 0)
                    {
                        if (predictedVal == 0)
                            APE = 0;
                        else
                            APE = 1;
                    }
                    writer2.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", APE, row, col, predictedVal, trueVal);
                }
                lock (writer3)
                {
                    predictedVal = this.filter.utilMat.get(row, col);
                    if (double.IsNaN(predictedVal)||predictedVal==0)
                        predictedVal = filter.defaultPrediction(row, col);
                    else
                        predictedVal = filter.utilMat.deNorm(row, col, predictedVal);
                    APE = Math.Abs(predictedVal - trueVal) / Math.Abs(trueVal);
                    if (trueVal == 0)
                    {
                        if (predictedVal == 0)
                            APE = 0;
                        else
                            APE = 1;
                    }
                    writer3.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", APE, row, col, predictedVal, trueVal);
                }
            }
        }
        public static void ABTest_nl(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1, string notes = null, bool normalization = true, int normMode = 0)
        {
            StreamWriter notesWriter = new StreamWriter(outputPrefix + "notes.txt", true);
            notesWriter.WriteLine(notes);
            notesWriter.Close();
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

                    Matrix utilMat = normMode == 0 ? new Matrix(maxRow + 1, maxCol + 1, points) : new Matrix(maxRow + 1, maxCol + 1, points);

                    CF filter = new CF(utilMat, false, normalization);//, true, 10, 20, false);
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

        public static void ABTest_n(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1, string notes = null, bool normalization = true, int normMode=0)
        {
            StreamWriter notesWriter = new StreamWriter(outputPrefix + "notes.txt", true);
            notesWriter.WriteLine(notes);
            notesWriter.Close();
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

                    Matrix utilMat = normMode == 0 ? new Matrix(maxRow + 1, maxCol + 1, points) : new Matrix(maxRow + 1, maxCol + 1, points);

                    CF filter = new CF(utilMat, true, normalization);//, true, 10, 20, false);
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


        public static void ABTest_h(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1, string notes = null, bool normalization = true)
        {
            StreamWriter notesWriter = new StreamWriter(outputPrefix + "notes.txt",true);
            notesWriter.WriteLine(notes);
            notesWriter.Close();
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

                    CF filter = new CF(utilMat, true, normalization);//, true, 10, 20, false);
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

        public static void ABTest_s(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1, string notes = null, bool normalization = true)
        {
            StreamWriter notesWriter = new StreamWriter(outputPrefix + "notes.txt", true);
            notesWriter.WriteLine(notes);
            notesWriter.Close();
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
                    Dictionary<int, double> averages = IO.baselinePredict(trainPath);
                    
                    Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);
                    foreach (int col in utilMat.getCols())
                        utilMat.setAvg[col] = averages[col];
                    utilMat.normalize();
                    CF filter = new CF(utilMat, true, false);//, true, 10, 20, false);
                    Console.WriteLine("Check 3");
                    //filter.iterate(1);
                    //filter.buildModel();
                    Tester tester = new Tester(filter, testPts);
                    tester.abtest(outputPrefix + "about_" + vreq + "_" + creq + ".txt");
                    Console.WriteLine("completed test: minview:{0}\tminclick:{1}\tnumvalidentries:{2}", vreq, creq, numvalidentries);
                    reader.Close();
                }
            }

            Console.WriteLine("debug: completed AB");
        }

        public static void showData(string trainPath, string outputPath, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            StreamReader reader;
            List<double[]> points = new List<double[]>();

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
                maxRow = Math.Max(maxRow, int.Parse(tokens[rowPos]));
                maxCol = Math.Max(maxCol, int.Parse(tokens[colPos]));
                if (clicks!=0)
                    points.Add(new double[3] { Double.Parse(tokens[rowPos]), Double.Parse(tokens[colPos]), valPos == -1 ? Math.Log( Math.Min(clicks, views) / views ): Double.Parse(tokens[2]) });
                numvalidentries += 1;
            }

            Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);
            utilMat.normalize();
            StreamWriter writer = new StreamWriter(outputPath);
            foreach (int col in utilMat.getCols())
                foreach (int row in utilMat.getRowsOfCol(col))
                {
                    if (utilMat.get(row, col) == 0)
                        continue;
                    //writer.WriteLine(Math.Log((utilMat.get(row, col))));
                    writer.WriteLine(((utilMat.get(row, col)*utilMat.setAvg[col])-utilMat.setAvg[col])/utilMat.setAvg[col]);
                    //writer.WriteLine(((utilMat.get(row, col)) - 1) / utilMat.setAvg[col]);
                }
            writer.Close();
            
            
        }

        public static void ABTest_i(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1, string notes = null, bool normalization = true, int iter=3)
        {
            StreamWriter notesWriter = new StreamWriter(outputPrefix + "notes.txt", true);
            notesWriter.WriteLine(notes);
            notesWriter.Close();
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
                    CF filter = new CF(utilMat, true, normalization);//, true, 10, 20, false);
                    for (int i = 0; i < iter; i++)
                    {
                        filter.iterate(1);
                        Console.WriteLine("Check 3");
                        //filter.buildModel();
                        Tester tester = new Tester(filter, testPts);
                        tester.abtest(outputPrefix + "about_" + vreq + "_" + creq + "_" + i + ".txt");
                    }
                    Console.WriteLine("completed test: minview:{0}\tminclick:{1}\tnumvalidentries:{2}", vreq, creq, numvalidentries);
                    reader.Close();
                }
            }

            Console.WriteLine("debug: completed AB");
        }

        public static void testPCA()
        {
            int num = 3;
            double[,] sourceMatrix = new double[num, num];
            Random rand = new Random();
            for (int row = 0; row < num; row++)
                for (int col = 0; col < num; col++)
                    sourceMatrix[row, col] = rand.NextDouble();
            PrincipalComponentAnalysis pa = new PrincipalComponentAnalysis(sourceMatrix);
            pa.Compute();
            double[,] ans = pa.Transform(sourceMatrix, num);
            List<double[]> points = new List<double[]>();

            for (int row = 0; row < num; row++)
                for (int col = 0; col < num; col++)
                    points.Add(new double[]{col, row, sourceMatrix[row,col]});
            Matrix tmat = new Matrix(num, num, points);
            PCA pcaa = new PCA(tmat, num);
            pcaa.compute();
            Matrix rcv = pcaa.transform();
            for (int col = 0; col < num; col++)
                if (rcv.get(col, 0) * ans[0, col] < 0)
                    for (int row = 0; row < num; row++)
                        rcv.set(col, row, rcv.get(col, row) * -1);
            for (int row = 0; row < num; row++)
            {
                for (int col = 0; col < num; col++)
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", ans[row, col], rcv.get(col, row), pa.Eigenvalues[col], pcaa.eigenValues[col]);
                Console.WriteLine("------------------------------------------------");
            }
        }
        public static void ABTest_PCA(int s, int e, int step, int s2, int e2, int step2, string testPath, string trainPath, string outputPrefix, int rowPos = 1, int colPos = 0, int valPos = -1, string notes = null, bool normalization = true, string pcapath = "C:\\Users\\t-chexia\\Desktop\\ab test final\\mat_fac_big_30")
        {
            StreamWriter notesWriter = new StreamWriter(outputPrefix + "notes.txt", true);
            notesWriter.WriteLine(notes);
            notesWriter.Close();
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
                    

                    PCA loadedpca = (PCA)IO.load(pcapath);



                    double a = 0;
                    double b = 0;
                    double c = 0;
                    for (int i = 0; i < 30; i++)
                    {
                        b = loadedpca.feature_row[i, 0];
                        c = loadedpca.feature_col[i, 5];
                        a = loadedpca.feature_row[i, 0] * loadedpca.feature_col[i, 5];
                    }
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
                    Dictionary<int, double> averages = IO.baselinePredict(trainPath);

                    Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);
                    foreach (int col in utilMat.getCols())
                        utilMat.setAvg[col] = averages[col];
                    utilMat.normalize();

                    MFCF filter = new MFCF(loadedpca, utilMat);
                    //CF filter = new CF(utilMat, true, normalization);//, true, 10, 20, false);
                    Console.WriteLine("Check 3");
                    //filter.buildModel();

                    Tester tester = new Tester(filter, testPts);
                    tester.abtest(outputPrefix + "about_" + vreq + "_" + creq + ".txt");
                    reader.Close();
                }
            }

            Console.WriteLine("debug: completed AB");
        }

        public static void train_PCA(int s, int s2, string trainPath, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            int numvalidentries = 0;
            StreamReader reader = File.OpenText(string.Format(trainPath));
            List<double[]> points = new List<double[]>();
            LogEnum logenum = new LogEnum(trainPath);
            int maxRow = 0;
            int maxCol = 0;
            Random rand = new Random();
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
                if (views < s || clicks < s2)
                    continue;
                maxRow = Math.Max(maxRow, int.Parse(tokens[rowPos]));
                maxCol = Math.Max(maxCol, int.Parse(tokens[colPos]));
                double ctr = Math.Min(clicks, views) / (views+1);
                if (rand.NextDouble() < 0.1)
                    continue;
                points.Add(new double[3] { Double.Parse(tokens[rowPos]), Double.Parse(tokens[colPos]), valPos == -1 ? ctr : Double.Parse(tokens[2]) });
                numvalidentries += 1;
            }

            Matrix utilMat = new Matrix(maxRow + 1, maxCol + 1, points);
            PCA pca = new PCA(utilMat, 50);
            pca.compute();
            return;
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

        public static void PUTest()
        {
            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\test_less_10000.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_5_2.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed12.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed12.log");
            for (int threshold =1000; threshold<10000; threshold+=50000000){
                string inputPath = "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed12.log";
                StreamWriter writer = new StreamWriter("C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\tmp");
                LogEnum le = new LogEnum(inputPath);
                foreach (string line in le)
                {
                    string[] tokens = line.Split(new char[] { '\t' });
                    double train_views = double.Parse(tokens[4]);
                    if (train_views > threshold && train_views < threshold + 500)
                    {
                        writer.WriteLine(line);
                    }
                }
                writer.Close();
                int min = threshold + 500;

                //Tester.ABTest_h(threshold, threshold, 10000, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\tmp", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed12.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\", 0, 1);
                Tester.ABTest_h(min, min, 10000, -1, -1, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\tmp", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed12.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\", 0, 1);
            }
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
                        if (!ad_muid_click.contains(ad, user) || !intent_muid.sourceMatrix.ContainsKey(user))
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

            foreach (int intent in intent_muid.sourceMatrix[user].Keys)
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
            foreach (int muid in intent_muid.sourceMatrix.Keys)
                if (intent_muid.sourceMatrix[muid].ContainsKey(principalIntent))
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

            foreach (int intent in intent_muid.sourceMatrix[user].Keys)
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
