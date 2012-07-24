using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using Accord.Statistics.Analysis;
using System.Diagnostics;
using Accord.Statistics.Analysis;

namespace CF
{
    class IO
    {
        static void Main(string[] args)
        {

            Matrix tmat = new Matrix(3, 3, new List<double[]>(new double[][] { new double[3] { 0, 0, 1 }, new double[3] { 0, 1, 2 }, new double[3] { 0, 2, 3 }, new double[3] { 1, 0, 4 }, new double[3] { 1, 1, 5 }, new double[3] { 1, 2, 6 }, new double[3] { 2, 0, 7 }, new double[3] { 2, 1, 8 }, new double[3] { 2, 2, 9 } }));
            PCA pcaa = new PCA(tmat, 3);
            pcaa.compute();
            double[,] rcv = pcaa.rr_eigenvectors();
            for (int row = 0; row < rcv.GetLength(1); row++)
                for (int col = 0; col < rcv.GetLength(0); col++)
                    Console.WriteLine(rcv[row, col]);
            return;

            double[,] sourceMatrix = new double[tmat.GetLength(1), tmat.GetLength(0)];
            foreach (int col in tmat.getCols())
                foreach (int row in tmat.getRowsOfCol(col))
                    sourceMatrix[col, row] = tmat.get(row, col);
            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(sourceMatrix);
            pca.Compute();
            foreach (PrincipalComponent i in pca.Components)
                for (int j = 0; j < i.Eigenvector.Length; j++)
                    Console.WriteLine(i.Eigenvector[j]);
            return;

            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_100000v_orm_3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_100v_orm_3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed19.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed19.log");
            Tester.ABTest_h(10002, 10002, 1000, 0, 10, 10, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed19.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed19.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid10\\", 0, 1, -1, "10002 for no intent removal");
            return;
            
            Tester.ABTest_h(10000, 10000, 1000, -2, -2, 0, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid11\\", 1, 0, -1, "this directory is used for investigating the effects of normalization");
            Tester.ABTest_h(10000, 10000, 1000, -3, -3, 0, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid11\\", 1, 0, -1, "this directory is used for investigating the effects of normalization", false);
            return;
            aggregateStats(1000, 14500, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid9\\", "about_*_-1.txt.txt");
            return;
            
            Tester.ABTest_h(1000, 5000, 1000, -1, -1, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed16.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed16.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid9\\", 0, 1);
            Tester.ABTest_h(15000, 15000, 1000, -1, -1, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed16.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed16.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid9\\", 0, 1);
            Tester.ABTest_h(1500, 15000, 1000, -1, -1, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed16.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed16.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid9\\", 0, 1);
            return;
            Matrix foo = LogProcess.makeUtilMat(0, 0, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", 0, 1);
            foo.normalize();
            foreach (int i in foo.getCols())
            {

                StreamWriter writer = new StreamWriter("gaussianCTR");
                if (foo.getRowsOfCol(i).Count() > 100)
                {
                    double sum = 0;
                    Console.WriteLine("exists");
                    foreach (int row in foo.getRowsOfCol(i))
                    {
                        sum += foo.get(row, i);
                        writer.WriteLine(foo.get(row, i) * foo.setDev[i] / foo.setAvg[i]);
                    }
                    Console.WriteLine(sum);
                }
                writer.Close();
                accumulateResult_nc("gaussianCTR", "gaussianCTR.log");
                break;

            }
            return;
            return;
            Tester.ABTest_h(10000, 10000, 1000, -16, -16, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            
            return;
            preMat pMa = new preMat(5, 5);
            pMa.set(0, 0, 0.2);
            pMa.set(3, 0, 1);
            pMa.set(0, 1, 0);
            pMa.set(1, 1, 0.1);
            pMa.set(2, 1, 0.3);
            pMa.set(3, 1, 0.6);
            pMa.set(1, 2, 0.1);
            pMa.set(0, 3, 0);
            pMa.set(1, 3, 0.01);
            pMa.set(2, 3, 0.03);
            pMa.set(3, 3, 0.06);
            pMa.set(4, 3, 0.02);
            pMa.set(4, 2, 0.1);
            CF filter = new CF(pMa, false, true);
            Console.WriteLine(filter.toString());
            return;

            //return;


            Tester.ABTest_h(10000, 10000, 1000, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            return;
            aggregateStats(1000, 10000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\", "about_*_0.txt.txt", "aggregated_stats_dir");
            aggregateStats(1500, 10000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\", "about_*_-1.txt.txt", "aggregated_stats_pre");
            return;
            Tester.PUTest();
            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_20.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed15.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed15.log");
            Tester.ABTest_h(1000, 5000, 1000, -16, -16, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed15.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed15.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            return;

            aggregateStats(1500, 10000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\pu\\", "about_*_0.txt.txt", "aggregated_stats_");
            return;

            Tester.PUTest();
            
            return;
            Tester.ABTest_h(10000, 10000, 10000, -13, -13, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 1, 0);
            return;
            Tester.ABTest_h(10000, 10000, 10000, -13, -13, 1, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 1, 0);
            return;
            //LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_remote.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_remote.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed11.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed11.log");
            Tester.ABTest_h(8000, 13000, 500, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed11.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed11.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid8\\", 1, 0);
            return;
            
            for (double threshold = 0.3; threshold < 5; threshold += 0.1)
                for (int neighbors = 3; neighbors < 6; neighbors += 1)
                    for (double confidence = 0; confidence < 0.5; confidence += 0.05)
                        for (double preserve = 0.8; preserve < 1; preserve += 0.5)
                            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "C:\\Users\\t-chexia\\Documents\\Visual Studio 2010\\Projects\\CF\\CF\\bin\\Debug\\JAC_result_PCA.txt", threshold, neighbors, confidence, preserve);
            return;

            PCA.foo();
            return;
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result.txt", 0.3, 5, 0.3);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_1000.log", "jac_result.txt", 0.3);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result.txt", 0.5);
            string tmp = Console.ReadLine();
            return;

            return;

            aggregateStats(500, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\view\\", "about_*_0.txt.txt", "aggregated_stats_");
            return;

            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_outlierintent_removed_10000_100.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_outlierintent_removed_100_0.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed7.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed7.log");
            Tester.ABTest_h(1000, 30000, 1000, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed7.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed7.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid5\\", 0, 1);

            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_5_enlarged.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed6.log");
            Tester.ABTest_h(501, 11501, 500, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid3\\", 0, 1);


            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_00.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed5.log");
            Tester.ABTest_h(7000, 9500, 500, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid3\\", 0, 1);
            Tester.ABTest_h(500, 500, 500, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid3\\", 0, 1);
            return;

        }

        public static void filterData(string inputPrefix, string genericFilename, int s, int e, int step)
        {
            string[] halves = genericFilename.Split(new char[] { '*' });
            string left = halves[0];
            string right = halves[1];
            HashSet<string> keep = new HashSet<string>();
            int i = s;
            {
                string fileName = left + i + right;
                string filePath = inputPrefix + fileName;
                LogEnum le = new LogEnum(filePath);
                foreach (string line in le)
                {
                    string[] tokens = line.Split();
                    if (tokens[0] != "1")
                        keep.Add(tokens[1]+"\t"+tokens[2]);
                }
            }

            for (i = s; i >= e; i -= step)
            {
                string fileName = left + i + right;
                string filePath = inputPrefix + fileName;
                LogEnum le = new LogEnum(filePath);
                StreamWriter writer = new StreamWriter("temp\\" + fileName);
                foreach (string line in le)
                {
                    string[] tokens = line.Split();
                    if (keep.Contains(tokens[1]+"\t"+tokens[2]))
                        writer.WriteLine(line);
                }
                writer.Close();
                IO.accumulateResult("temp\\" + fileName, "temp\\" + fileName + ".txt");
            }
        }

        public static void parseTest(string inputData)
        {
            LogEnum logenum = new LogEnum(inputData);
            HashSet<string> intent = new HashSet<string>();
            HashSet<string> ad = new HashSet<string>();
            StreamWriter writer = new StreamWriter("tmp.txt");
            MultiDictionary<string, double> output = new MultiDictionary<string, double>(true);
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                intent.Add(tokens[0]);
                ad.Add(tokens[1]);
                output.Add(tokens[1], Double.Parse(tokens[3]) / Double.Parse(tokens[2]));

            }
            foreach (string key in output.Keys)
                foreach (double val in output[key])
                    writer.WriteLine(key + "\t" + val);
            Console.WriteLine(intent.Count);
            Console.WriteLine(ad.Count);
            writer.Close();
        }
        public static void split(string inputData, string out_test = "jac_test.log", string out_train = "jac_train.log")
        {
            LogEnum logenum = new LogEnum(inputData);
            Random randgen = new Random();
            StreamWriter testWriter = File.CreateText(out_test);
            StreamWriter trainWriter = File.CreateText(out_train);
            foreach (string line in logenum)
            {
                if (randgen.NextDouble() < 0.3)
                    testWriter.WriteLine(line);
                else
                    trainWriter.WriteLine(line);
            }
            testWriter.Close();
            trainWriter.Close();
        }
        public static void clickPlot(int threshold)
        {
            LogEnum logenum = new LogEnum("train_processed_2.log");
            Dictionary<int, int> click_count = new Dictionary<int, int>();
            StreamWriter writer = new StreamWriter("tmp", false);
            int cnt = 0;
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                double views = double.Parse(tokens[2]);
                if (views < threshold)
                    break;
                if (click_count.ContainsKey(Int32.Parse(tokens[3])))
                    click_count[Int32.Parse(tokens[3])] += 1;
                else
                    click_count.Add(Int32.Parse(tokens[3]), 1);
                cnt += 1;

            }
            Console.WriteLine(cnt);
            foreach (int click in click_count.Keys)
                writer.WriteLine("{0}\t{1}", click, click_count[click]);
            writer.Close();
        }

        /*
         * Given a log file, and given integerMaps to translate the strings in the log, produce a util matrix
         * */

        public static void reduceTrain(string inputTrain, string inputTest, string outputTrain)
        {
            Matrix testMatrix = LogProcess.makeUtilMat(1000, 100000, inputTest, 0, 1);
            LogEnum logenum = new LogEnum(inputTrain);
            StreamWriter writer = File.CreateText(outputTrain);
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                if (testMatrix.get(Int32.Parse(tokens[0]), Int32.Parse(tokens[1])) != -1)
                {
                    Console.WriteLine(line);
                    continue;
                }
                writer.WriteLine(line);
            }
            writer.Close();

        }

        public static void save(object toSave, string outputPath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, toSave);
            stream.Close();
        }
        public static object load(string inputPath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(inputPath, FileMode.Open);
            object rtn = formatter.Deserialize(stream);
            stream.Close();
            return rtn;
        }



        public static void accumulateResult(string inputPath, string outputPath)
        {
            int count = 0;
            StreamReader reader = File.OpenText(inputPath);
            SortedDictionary<double, int> bins = new SortedDictionary<double, int>();
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                double binNum = Math.Ceiling(Double.Parse(tokens[0]) / 0.001);

                if (bins.ContainsKey(binNum))
                    bins[binNum] += 1;
                else
                    bins.Add(binNum, 1);
                line = reader.ReadLine();
                count += 1;
            }
            reader.Close();
            int total = 0;
            //Console.WriteLine("threshold:{0}\tcount:{1}", i, count);
            StreamWriter writer = File.CreateText(outputPath);
            double prev = 0;
            double curr = 0;
            foreach (double key in bins.Keys)
            {
                curr = key;
                total += bins[key];
                writer.WriteLine("{0}\t{1}", key * 0.001, ((double)total) / (double)count);
                prev = key;
            }
            writer.Close();
        }

        public static void accumulateResult_nc(string inputPath, string outputPath)
        {
            int count = 0;
            StreamReader reader = File.OpenText(inputPath);
            SortedDictionary<double, int> bins = new SortedDictionary<double, int>();
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                double binNum = Math.Ceiling(Double.Parse(tokens[0]) / 0.1);

                if (bins.ContainsKey(binNum))
                    bins[binNum] += 1;
                else
                    bins.Add(binNum, 1);
                line = reader.ReadLine();
                count += 1;
            }
            reader.Close();
            int total = 0;
            //Console.WriteLine("threshold:{0}\tcount:{1}", i, count);
            StreamWriter writer = File.CreateText(outputPath);
            double prev = 0;
            double curr = 0;
            foreach (double key in bins.Keys)
            {
                curr = key;
                total += bins[key];
                writer.WriteLine("{0}\t{1}", key * 0.1, ((double)bins[key]) / (double)count);
                prev = key;
            }
            writer.Close();
        }

        public static void accumulateResult(int s, int e, int step, string inputPrefix, string outputPrefix)
        {
            for (int i = s; i <= e; i += step)
            {
                int count = 0;
                string inputPath = inputPrefix + i + ".txt";
                string outputPath = outputPrefix + i + ".txt";
                StreamReader reader = File.OpenText(inputPath);
                SortedDictionary<double, int> bins = new SortedDictionary<double, int>();
                string line = reader.ReadLine();
                while (line != null)
                {
                    double binNum = Math.Ceiling(Double.Parse(line) / 0.001);
                    if (bins.ContainsKey(binNum))
                        bins[binNum] += 1;
                    else
                        bins.Add(binNum, 1);
                    line = reader.ReadLine();
                    count += 1;
                }
                reader.Close();
                int total = 0;
                //Console.WriteLine("threshold:{0}\tcount:{1}", i, count);
                StreamWriter writer = File.CreateText(outputPath);
                double prev = 0;
                double curr = 0;
                foreach (double key in bins.Keys)
                {
                    curr = key;
                    if (curr > 0.3 / 0.001 && prev <= 0.3 / 0.001)
                    {
                        Console.WriteLine("threshold:{0}\tless-than-0.3:{1}", i, total);
                    }
                    total += bins[key];
                    writer.WriteLine("{0}\t{1}", key * 0.001, ((double)total) / (double)count);
                    prev = key;
                }
                writer.Close();
            }
        }

        public static void manReader(string inputFilePath)
        {
            LogEnum logenum = new LogEnum(inputFilePath);
            foreach (string line in logenum)
            {
                Console.ReadLine();
                Console.WriteLine(line);
            }
        }


        public static void aggregateStats(int s, int e, int step, string inputPrefix, string fileName = "about_*.txt", string outName = "aggregated_stats_")
        {
            Dictionary<double, double>[] threshold_count = new Dictionary<double, double>[10];
            //string inputPrefix = "705 ab results\\";
            string[] fn = fileName.Split(new char[] { '*' });
            for (int k = 1; k < 10; k++)
            {
                threshold_count[k] = new Dictionary<double, double>();
                for (int i = s; i <= e; i += step)
                {
                    string inputPath = inputPrefix + fn[0] + i + fn[1];
                    StreamReader reader = new StreamReader(inputPath);
                    string line = reader.ReadLine();

                    while (line != null)
                    {
                        string[] tokens = line.Split(new Char[] { '\t' });
                        double upperAPE = Double.Parse(tokens[0]);
                        while (upperAPE < 0.1 * k)
                        {
                            line = reader.ReadLine();
                            tokens = line.Split(new Char[] { '\t' });
                            upperAPE = Double.Parse(tokens[0]);
                            line = reader.ReadLine();
                        }
                        threshold_count[k].Add(i, Double.Parse(tokens[1]));
                        break;
                    }
                    reader.Close();
                }
                string outputPath = inputPrefix + outName + (0.1 * k) + ".txt";
                StreamWriter writer = File.CreateText(outputPath);
                foreach (double key in threshold_count[k].Keys)
                {
                    writer.WriteLine("{0}\t{1}", key, threshold_count[k][key]);
                }
                writer.Close();
            }
        }

        public static void aggregateBlockTest()
        {
            string inputPath = "C:\\Users\\t-chexia\\Desktop\\blocktest\\blockTestOutput.txt";
            LogEnum logenum = new LogEnum(inputPath);
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });

            }
        }

    }
    [Serializable()]
    class IntegerMap
    {
        private int count = 0;
        private Dictionary<string, int> mapper;
        private Dictionary<int, string> revmapper;
        public IntegerMap()
        {
            revmapper = new Dictionary<int, string>();
            mapper = new Dictionary<string, int>();
        }
        public void add(string inputFilePath, int pos)
        {
            LogEnum logenum = new LogEnum(inputFilePath);
            List<double[]> points = new List<double[]>();
            double numEntries = 0;
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                this.add(tokens[pos]);
                numEntries += 1;
            }
        }
        public void add(string newItem)
        {
            if (!mapper.ContainsKey(newItem))
            {
                revmapper.Add(count, newItem);
                mapper.Add(newItem, count);
                count++;
            }
        }
        public bool contains(string key)
        {
            return mapper.ContainsKey(key);
        }
        public int get(string key)
        {
            return mapper[key];
        }
        public int getCount()
        {
            return count;
        }
        public string getItemByInt(int x)
        {
            return revmapper[x];
        }

    }




}