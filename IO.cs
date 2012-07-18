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

            //aggregateStats(10, 50, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid3\\", "about_10000_" );
            //return;
            //JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result.txt", 0.7); 
            //LNK.LNKtest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result_2.txt", 0.5);
            //return;
            //return;
            //JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result.txt", 0.5, 5, 0.3);

            //aggregateStats(0, 50, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid3\\", "about_15000_*.txt.txt");
            //return;
            /*
            for (double threshold =0.5; threshold< 1; threshold +=0.1)
                for (int neighbors =1; neighbors < 10; neighbors +=1)
                    for (double confidence = 0; confidence< 0.5; confidence += 0.05)
                        JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_1000.log", "C:\\Users\\t-chexia\\Documents\\Visual Studio 2010\\Projects\\CF\\CF\\bin\\Debug\\JAC_result.txt", threshold, neighbors, confidence);
            return;
            */
            /*
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_small.log", "jac_result.txt", 0.3);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_1000.log", "jac_result.txt", 0.5);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_1000.log", "jac_result.txt", 0.3);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result.txt", 0.5); 
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "jac_result.txt", 0.3);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_small_lq.log", "jac_result.txt", 0.5);
            JACtest.jacSplitTest("C:\\Users\\t-chexia\\Desktop\\usi_sample_small_lq.log", "jac_result.txt", 0.3);
             * */
            //manReader("train_processed.");



            //split("C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            //Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\", 0, 1);
            //return;


            //manReader("test_processed.log");
            //reduceTrain("train_processed.log", "test_processed.log", "train_processed_2.log");

            //clickPlot(20000);

            //LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_5.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            //Tester.ABTest_h(10000, 30000, 1000, 10, 50, 5, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid\\", 0, 1);
            //Tester.ABTest_c(10, 50, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\click2\\",0, 1);
            //aggregateStats(0, 6, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid2\\", "about_11500_*.txt.txt");
            /*
            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\davc_test_using_2.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\davc_train_using_2.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            split("C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log");
            Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid2\\", 1, 0);
            //Tester.ABTest_h(11500, 20000, 500, 0, 8, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid2\\", 1, 0);
            return;
            */
            //manReader("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_0.log");
            //aggregateStats(10000, 100000, 1000, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid2\\", "about_*_0.txt.txt");
            //return;
            //parseTest("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_0.log");
            //return;
            //Tester.speedTest(1000000, "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            //return;
            /*
            for (int i = 10000; i < 100000; i += 500)
            {
                string inFile = "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\view\\" + "about_" + i + ".txt.txt";
                string outFile = "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\view\\" + "about_" + i + "_0.txt.txt";
                File.Move(inFile, outFile);
            }
            
            return;
            
            
            return;
            double[,] sourceMatrix = 
            {
                { 2.5,  2.4 },
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };
            var pca = new PrincipalComponentAnalysis(sourceMatrix);
            pca.Compute();
            double[,] components = pca.Transform(sourceMatrix, 1);
            //Console.WriteLine(pca.GetNumberOfComponents(0.95f));
            Console.WriteLine(components[9, 0]);
            Console.WriteLine(sourceMatrix[9, 0]);

            return;
             * */
            //LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_outlierintent_removed_less_5000.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_outlierintent_removed_less_5000.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed9.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed9.log");
            /*

            StreamWriter writer = new StreamWriter("timing Info");
            System.Timers.Timer aTimer = new System.Timers.Timer();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Tester.ABTest_hl(10000, 10000, 10000, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            sw.Stop();
            double ExecutionTimeTaken = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine(ExecutionTimeTaken);
            writer.WriteLine("linear time: {0}", ExecutionTimeTaken);

            sw.Reset();
            sw.Start();
            Tester.ABTest_h(10000, 10000, 10000, -1, -1, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            sw.Stop();
            ExecutionTimeTaken = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine(ExecutionTimeTaken);
            writer.WriteLine("parallel time: {0}", ExecutionTimeTaken);
            writer.Close();
                                   
            */
            Tester.ABTest_h(10000, 30000, 10000, -5, -5, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            Tester.ABTest_hl(10000, 30000, 10000, -6, -6, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid7\\", 0, 1);
            return;
            
            for (double threshold = 0.3; threshold < 5; threshold += 0.1)
                for (int neighbors = 3; neighbors < 6; neighbors += 1)
                    for (double confidence = 0.3; confidence < 1; confidence += 0.05)
                        for (double preserve = 0.95; preserve < 1; preserve += 0.5)
                            JACtest.jacSplitTestL("C:\\Users\\t-chexia\\Desktop\\usi_sample_hq_600.log", "C:\\Users\\t-chexia\\Documents\\Visual Studio 2010\\Projects\\CF\\CF\\bin\\Debug\\JAC_result_PCA.txt", threshold, neighbors, confidence, preserve);
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
            

            //LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_0.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_0.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            //Tester.ABTest_h(24001, 100000, 1000, 0, 0, 1, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\hybrid2\\", 0, 1);
            //return;
            // hybrid2 generated using train0 and test0
            //hybrid3 generated using train5 and test5
            //test 0 and train 5 give interesting results

            //split("C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed2.log");
            //Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\train self split intersection removed\\", 0, 1);
            //split("C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed2.log");
            //Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\test self split\\", 0, 1);

            //return;
            //Tester.ABTest(10001, 10001, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\view\\", 0, 1);
            //aggregateStats(10, 200, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection removed\\click\\");
            return;


            /*
            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_4.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_4.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            Tester.ABTest_c(50, 200, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\no view threshold\\click\\", 0, 1);
            Tester.ABTest(10000, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\no view threshold\\view\\", 0, 1);


            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log");
            Tester.ABTest_c(10, 200, 2, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection not removed\\click\\", 0, 1);
            Tester.ABTest(10000, 100000, 500, "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\intersection not removed\\view\\", 0, 1);



            LogProcess.cleanLogs1("C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_test_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\iavc_train_using_6.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\testProcessed3.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            split("C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed3.log");
            Tester.ABTest(10000, 200000, 500, "jac_test.log", "jac_train.log", "C:\\Users\\t-chexia\\Desktop\\ab test final\\train self split\\", 0, 1);
            */
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
            Matrix testMatrix = LogProcess.makeUtilMat(inputTest, 0, 1);
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



}