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
    class IO
    {
        static void Main(string[] args)
        {
            /*
            split("C:\\Users\\t-chexia\\Desktop\\ab\\intentID_view_count_test_30000_100.log");
            IntegerMap i2int = populateIntegerMap(new string[] { "test", "train" }, new int[] { 0, 0 });
            IntegerMap a2int = populateIntegerMap(new string[] { "test", "train" }, new int[] { 1, 1 });
            Matrix trainMat = makeUtilMat("train", i2int, a2int, 0, 1, 2);
            Matrix testMat = makeUtilMat("test", i2int, a2int, 0, 1, 2);
            CF filter = new CF(trainMat);
            Tester tester = new Tester(filter, testMat);
            tester.abtest("bad_result.txt");
            string inputPath = "bad_result.txt";
            int count = 0;
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
            StreamWriter writer = File.CreateText("parswed_bad_result.txt");
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
             * */
            //split("C:\\Users\\t-chexia\\Desktop\\ab\\intentID_ad_ctr_threshold_30000_100.log");
            //ABTest(20000, 100000, 200, "test_processed.log", "train_processed.log", "703 ab results\\", 0,1);
            //ABTest(100000, 200000, 2000, "test_processed.log", "train_processed.log", "703 ab results\\", 0, 1);
            //aggregateStats();
            //ABTest(10000, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab\\intentID_view_count_test_30000_100.log", "C:\\Users\\t-chexia\\Desktop\\ab\\intentID_view_count_sorted.log", "new ab results n\\", 1, 0);
            //parseData(10000, 100000, 500, "new ab results\\about_","new ab results\\parsed_");
            //parseData(10000, 100000, 500, "new ab results n\\about_", "new ab results n\\parsed_");
            //parseData();
            //LogProcess.cleanLogs0("C:\\Users\\t-chexia\\Desktop\\blocktest\\mavc_sample.log", "C:\\Users\\t-chexia\\Desktop\\blocktest\\msi_sample.log", "C:\\Users\\t-chexia\\Desktop\\blocktest\\iavc_sample.log");
            //aggregateStats();
            //int[] foo = LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab\\intentID_view_count_test_30000_100.log", "C:\\Users\\t-chexia\\Desktop\\ab\\intentID_view_count_sorted.log");

            //Tester.blockTest("C:\\Users\\t-chexia\\Desktop\\blocktest\\mavc_sample.log", "C:\\Users\\t-chexia\\Desktop\\blocktest\\msi_sample.log", "C:\\Users\\t-chexia\\Desktop\\blocktest\\iavc_fixed.log");
            //string ir = "C:\\Users\\t-chexia\\Documents\\Visual Studio 2010\\Projects\\CF\\CF\\bin\\Debug\\";
            //Tester.ABTest(10000, 100000, 5000000, ir+"test_processed.log", ir+"train_processed_2.log",ir+"705 ab results\\", 0, 1);
            //Console.WriteLine(a.GetHashCode());
            //Console.WriteLine(b.GetHashCode());

            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_1000.log");

            //manReader("train_processed.");



            //split("C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            //Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\", 0, 1);
            return;

            //aggregateStats(10000, 68500, 500);
            //manReader("test_processed.log");
            //reduceTrain("train_processed.log", "test_processed.log", "train_processed_2.log");

            //clickPlot(20000);

            //LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            //Tester.ABTest_h(10000, 30000, 1000, 10, 50, 5, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid\\", 0, 1);
            //Tester.ABTest_c(10, 50, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\click2\\",0, 1);

            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed2.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed2.log");
            Tester.ABTest_h(10000, 20000, 500, 10, 50, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid3\\", 0, 1);
            return;
            split("C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed2.log");
            //Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\train self split intersection removed\\", 0, 1);
            split("C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed2.log");
            Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\test self split\\", 0, 1);

            return;
            Tester.ABTest(10000, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\view\\", 0, 1);
            //aggregateStats(10, 200, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\click\\");




            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_4.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_4.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            Tester.ABTest_c(50, 200, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\no view threshold\\click\\", 0, 1);
            Tester.ABTest(10000, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\no view threshold\\view\\", 0, 1);


            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            Tester.ABTest_c(10, 200, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection not removed\\click\\", 0, 1);
            Tester.ABTest(10000, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection not removed\\view\\", 0, 1);



            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            split("C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\train self split\\", 0, 1);

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
                total += bins[key];
                writer.WriteLine("{0}\t{1}", key * 0.001, ((double)total) / (double)count);
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


        public static void aggregateStats(int s, int e, int step, string inputPrefix)
        {
            Dictionary<double, double>[] threshold_count = new Dictionary<double, double>[10];
            //string inputPrefix = "705 ab results\\";
            for (int k = 1; k < 10; k++)
            {
                threshold_count[k] = new Dictionary<double, double>();
                for (int i = s; i <= e; i += step)
                {
                    string inputPath = inputPrefix + "about_" + i + ".txt.txt";
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
                string outputPath = inputPrefix + "aggregated_stat_" + (0.1 * k) + ".txt";
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



}