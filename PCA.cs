﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Analysis;

namespace CF
{
    class PCA
    {
        private double[,] feature_row;
        private double[,] feature_col;
        private int numFeatures;
        private Matrix cached_predictions;
        private Matrix raw;
        private double lrate = 0.001;
        public PCA(Matrix raw, int dims = 50)
        {
            this.raw = raw;
            numFeatures = dims;
            feature_row = new double[numFeatures, raw.GetLength(0)];
            feature_col = new double[numFeatures, raw.GetLength(1)];
            for (int i = 0; i < numFeatures; i++)
                for (int j = 0; j < raw.GetLength(0); j++)
                    feature_row[i, j] = 0.1;
            for (int i = 0; i < numFeatures; i++)
                for (int j = 0; j < raw.GetLength(1); j++)
                    feature_col[i, j] = 0.1;
            cached_predictions = new Matrix(raw.GetLength(0), raw.GetLength(1));
            foreach (int col in raw.getCols())
                foreach (int row in raw.getRowsOfCol(col))
                    cached_predictions.set(row, col, 0);
        }
        public void compute()
        {
            for (int feature = 0; feature < numFeatures; feature++)
            {
                int counter = 0;
                bool converged = false;
                while (!converged)
                {
                    double max_err = 0;
                    foreach (int col in cached_predictions.getCols())
                        foreach (int row in cached_predictions.getRowsOfCol(col))
                        {
                            double err = lrate * (raw.get(row, col) - predictRating(row, col, feature));
                            double fr = feature_row[feature, row];
                            feature_row[feature, row] = fr + err * feature_col[feature, col];
                            feature_col[feature, col] = feature_col[feature, col] + err * fr;
                            max_err = Math.Max(err, max_err);
                        }
                    counter++;
                    if (counter>1000000)
                        converged = true;

                }
                foreach (int col in cached_predictions.getCols())
                    foreach (int row in cached_predictions.getRowsOfCol(col))
                        cached_predictions.set(row, col, predictRating(row, col, feature));
            }
        }
        private double predictRating(int row, int col, int feature)
        {
            double sum = 0;
            sum += feature_row[feature, row] * feature_col[feature, col];
            return sum + cached_predictions.get(row, col);

        }

        public double[,] rc_eigenvectors()
        {
            return feature_col;
        }
        public double[,] rr_eigenvectors()
        {
            return feature_row;
        }


        public static void foo()
        {
            // sourceMatrix[a,b] -> a: index of point, b: index of variable/feature



            PrincipalComponentAnalysis pca;
            //pca = (PrincipalComponentAnalysis)IO.load("train_pca_2");
            //Console.WriteLine(pca.ComponentMatrix.GetLength(0));
            //Console.WriteLine(pca.GetNumberOfComponents(0.9f));

            //return;
            Matrix utilMat = LogProcess.makeUtilMat(0, 0, "C:\\Users\\t-chexia\\Desktop\\ab test final\\trainProcessed.log", 0, 1);
            Console.WriteLine(utilMat.GetLength(0));
            Console.WriteLine(utilMat.GetLength(1));
            double[,] sourceMatrix = new double[utilMat.GetLength(1), utilMat.GetLength(0)];
            foreach (int col in utilMat.getCols())
                foreach (int row in utilMat.getRowsOfCol(col))
                    sourceMatrix[col, row] = utilMat.get(row, col);
            double[] rowAvg = PCA.rowAvg(utilMat);
            for (int i=0; i<sourceMatrix.GetLength(0); i++)
                for (int j = 0; j < sourceMatrix.GetLength(1); j++)
                {
                    if (!utilMat.contains(j, i))
                        sourceMatrix[i, j] = rowAvg[j];
                }
            pca = new PrincipalComponentAnalysis(sourceMatrix);
            pca.Compute();
            //double[,] components = pca.Transform(sourceMatrix, 1);
            Console.WriteLine(pca.GetNumberOfComponents(0.95f));
            //IO.save(pca, "train_pca_2");
            while (true)
            {
                string foo = Console.ReadLine();
                float bar = float.Parse(foo);
                Console.WriteLine(pca.GetNumberOfComponents(bar));
            }

            //Console.WriteLine(components[0, 0]);
        }
        public static ZOMatrix dmR(ZOMatrix utilMat, double preserve=0.8)
        {
            PrincipalComponentAnalysis pca;
            double[,] sourceMatrix = new double[utilMat.GetLength(1), utilMat.GetLength(0)];
            foreach (int col in utilMat.getCols())
                foreach (int row in utilMat.getRowsOfCol(col))
                    sourceMatrix[col, row] = utilMat.get(row, col);
            pca = new PrincipalComponentAnalysis(sourceMatrix);
            pca.Compute();
            int numCom = pca.GetNumberOfComponents((float)preserve);

            double[,] reducedUtilMat = pca.Transform(sourceMatrix, numCom);
            utilMat = new ZOMatrix(reducedUtilMat.GetLength(1), reducedUtilMat.GetLength(0));
            for (int col = 0; col < reducedUtilMat.GetLength(0); col++)
                for (int row = 0; row < reducedUtilMat.GetLength(1); row++)
                    utilMat.set(row, col, sourceMatrix[col, row]);
            return utilMat;
        }

        public static double[] rowAvg (Matrix utilMat)
        {
            double[] rtn = new double[utilMat.GetLength(0)];
            double[] count = new double[utilMat.GetLength(0)];

            foreach (int col in utilMat.getCols())
                foreach (int row in utilMat.getRowsOfCol(0))
                {
                    rtn[row] += utilMat.get(row, col);
                    count[row] += 1;
                }
            for (int i = 0; i < rtn.Length; i++)
            {
                if (count[i] == 0)
                    rtn[i] = 0;
                else
                    rtn[i] = rtn[i] / count[i];
            }
            return rtn;

        }
    }
}
