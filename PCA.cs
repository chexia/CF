using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Analysis;

namespace CF
{
    class PCA
    {
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
        public static JACMatrix dmR(JACMatrix utilMat, double preserve=0.8)
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
            utilMat = new JACMatrix(reducedUtilMat.GetLength(1), reducedUtilMat.GetLength(0));
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
