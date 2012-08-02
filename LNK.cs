using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace CF
{
    class LNK
    {
        private double[,] A;
        private double[,] Cr;
        private double[,] Pr;
        private double[,] B;
        private double[,] C;

        public static void LNKtest(string inputData, string outputPath = "jac_result.txt", double threshold = 0.5, int numrec = 3)
        {
            int[] ui = JACtest.cleanLogsj(inputData);
            Console.WriteLine("numUser:{0}\tnumIntent:{1}", ui[0], ui[1]);
            //JACtest.split("jac_usi_processed.log");
            ZOMatrix testMat = JACtest.makeUtilMat(ui[0], ui[1], "jac_test.log", 0, 2);
            LNK trainLNK = makeLNK(ui[0], ui[1], "jac_train.log");
            trainLNK.iterate(10);
            int[] final = new int[4] { 0, 0, 0, 0 };
            Parallel.For<MultiDictionary<double, int>>(0, ui[0],
                    () => new MultiDictionary<double, int>(true),
                    (user, state, stats) =>
                    {
                        stats.Add(100000, (int)user);
                        user = stats[100000].First();
                        for (int intent = 0; intent < ui[1]; intent += 1)
                        {
                            Double predictedVal = trainLNK.predict((int)user, intent);
                            double[] valarr = stats.Keys.ToArray<double>();
                            //Console.WriteLine(predictedVal);
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
                        return stats;
                    },
                     (stats) =>
                     {
                         int[] err = new int[4] { 0, 0, 0, 0 };
                         int user = stats[100000].First();
                         foreach (int intent in stats.Values.ToArray<int>())
                         {
                             double trueVal = testMat.get(user, intent);
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

        }


        public static LNK makeLNK(int rowNum, int colNum, string inputFilePath, int rowPos = 0, int colPos = 2, int valPos = 1)
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
            LNK utilMat = new LNK(rowNum, colNum, points);
            return utilMat;
        }

        public LNK(int numRow, int numCol, IEnumerable<double[]> points)
        {
            this.A = new double[numRow, numCol];
            // initialize A
            foreach (double[] point in points)
            {
                A[(int)point[0], (int)point[1]] = point[2];
            }

            // initialize Cr
            Cr = new double[numRow, numRow];
            for (int i = 0; i < Cr.GetLength(0); i++)
                Cr[i, i] = 1;

            // initialize Pr
            Pr = new double[numRow, numCol];

            // initialize B
            B = new double[numRow, numCol];
            for (int i = 0; i < B.GetLength(0); i++)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    double aij = A[i, j];
                    double denom = 0;
                    for (int k = 0; k < B.GetLength(0); k++)
                    {
                        denom += A[k, j];
                    }
                    B[i, j] = aij / (denom + 0.1);
                    //Console.WriteLine("bij:{0}",B[i, j]);
                }
            }

            // initialize C, which is actually C transposed
            C = new double[numCol, numRow];
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    double aij = A[i, j];
                    double denom = 0;
                    for (int k = 0; k < A.GetLength(1); k++)
                    {
                        denom += A[i, k];
                    }
                    C[j, i] = aij / (denom + 0.1);
                }
            }


        }

        private void iterate(int iterations)
        {
            Console.WriteLine("beginning iteration");
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                double a3717 = A[37, 17];
                double b3717 = B[37, 17];
                double c3717 = C[17, 37];
                double cr3737 = Cr[37, 37];
                double pr3717 = Pr[37, 17];
                Console.WriteLine("iteration {0}", iteration);
                double[,] tmp = new double[Pr.GetLength(0), Pr.GetLength(1)];
                mul(Cr, B, tmp);
                Pr = tmp;
                tmp = new double[Cr.GetLength(0), Cr.GetLength(1)];
                mul(Pr, C, tmp);
                add(tmp, Cr, Cr);
            }
        }

        public double predict(int row, int col)
        {
            //Console.WriteLine(Pr[row, col]);
            return Pr[row, col];
        }






        private static void mul(double[,] a, double[,] b, double[,] c)
        {
            if (a.GetLength(0) != c.GetLength(0) || a.GetLength(1) != b.GetLength(0) || b.GetLength(1) != c.GetLength(1))
                throw new ArgumentException("matrix dimensions do not match");
            Parallel.For(0, a.GetLength(0), (i) =>
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    double sum = 0;
                    for (int k = 0; k < a.GetLength(1); k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    c[i, j] = sum;
                    //Console.WriteLine(sum);
                }
            }
            );
        }
        private static void add(double[,] a, double[,] b, double[,] c)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1) || a.GetLength(0) != c.GetLength(0) || a.GetLength(1) != c.GetLength(1))
                throw new ArgumentException("matrix dimensions do not match");
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    c[i, j] = a[i, j] + b[i, j];
                    //Console.WriteLine(c[i, j]);
                }
            }
        }
    }
}
