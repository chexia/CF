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

    class LogProcess
    {
        //for block test
        public static int[] cleanLogs0 (string in_mavc, string in_msi, string in_iavc, string out_mavc = "mavc_processed.log", string out_msi = "msi_processed.log", string out_iavc = "iavc_processed.log")
        {
            IntegerMap m2int = new IntegerMap();
            m2int.add(in_mavc, 0);
            m2int.add(in_msi, 0);
            IntegerMap i2int = new IntegerMap();
            i2int.add(in_msi, 2);
            i2int.add(in_iavc, 0);
            IntegerMap a2int = new IntegerMap();
            a2int.add(in_mavc, 1);
            a2int.add(in_iavc, 1);
            rewrite(in_mavc, out_mavc, standardProcessor, m2int, a2int);
            rewrite(in_msi, out_msi, standardProcessor2, m2int, i2int);
            rewrite(in_iavc, out_iavc, standardProcessor, i2int, a2int);
            return new int[3] { m2int.getCount(), i2int.getCount(), a2int.getCount() };
        }
        public static int[] cleanLogs1(string in_test, string in_train, string out_test = "test_processed.log", string out_train = "train_processed.log")
        {
            IntegerMap i2int = new IntegerMap();
            i2int.add(in_test, 0);
            i2int.add(in_train, 0);
            IntegerMap a2int = new IntegerMap();
            a2int.add(in_test, 1);
            a2int.add(in_train, 1);

            rewrite(in_test, out_test, standardProcessor, i2int, a2int);
            rewrite(in_train, out_train, standardProcessor, i2int, a2int);
            Console.WriteLine("{0},{1}", i2int.getCount(), a2int.getCount());
            return new int[2] { i2int.getCount(), a2int.getCount() };
        }

        public static void rewrite(string inputFilePath, string outputFilePath, Func<string, IntegerMap, IntegerMap, string> processor, IntegerMap rowMap = null, IntegerMap colMap = null)
        {
            LogEnum logenum = new LogEnum(inputFilePath);
            StreamWriter writer = File.CreateText(outputFilePath);
            foreach (string line in logenum)
            {
                string outputStr = processor(line, rowMap, colMap);
                writer.WriteLine(outputStr);
            }
            writer.Close();
        }

        public static string standardProcessor(string line, IntegerMap a, IntegerMap b)
        {
            string[] tokens = line.Split();
            tokens[0] = "" + a.get(tokens[0]);
            tokens[1] = "" + b.get(tokens[1]);
            return string.Join("\t", tokens);
        }

        public static string standardProcessor2(string line, IntegerMap a, IntegerMap b)
        {
            string[] tokens = line.Split();
            tokens[0] = "" + a.get(tokens[0]);
            tokens[2] = "" + b.get(tokens[2]);
            return string.Join("\t", tokens);
        }


        public static Matrix makeUtilMat(int rowNum, int colNum, string inputFilePath, int rowPos = 1, int colPos = 0, int valPos = -1)
        {
            List<double[]> points = new List<double[]>();
            LogEnum logenum = new LogEnum(inputFilePath);
            rowNum = colNum = 0;
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
                rowNum = Math.Max(rowNum, (int)point[0]);
                colNum = Math.Max(colNum, (int)point[1]);
            }
            Matrix utilMat = new Matrix(rowNum+1, colNum+1, points);//+1 for 0
            return utilMat;
        }
    }

}
