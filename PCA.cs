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
            Matrix utilMat = LogProcess.makeUtilMat(0, 0, "jac_test.log", 0, 2, 1);
            Console.WriteLine(utilMat.GetLength(0));
            Console.WriteLine(utilMat.GetLength(1));
            double[,] sourceMatrix = new double[utilMat.GetLength(1), utilMat.GetLength(0)];
            foreach (int col in utilMat.getCols())
                foreach (int row in utilMat.getRowsOfCol(col))
                    sourceMatrix[col, row] = utilMat.get(row, col);
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
        public static Matrix dmR(Matrix utilMat)
        {
            PrincipalComponentAnalysis pca;
            double[,] sourceMatrix = new double[utilMat.GetLength(1), utilMat.GetLength(0)];
            foreach (int col in utilMat.getCols())
                foreach (int row in utilMat.getRowsOfCol(col))
                    sourceMatrix[col, row] = utilMat.get(row, col);
            pca = new PrincipalComponentAnalysis(sourceMatrix);
            pca.Compute();
            int numCom = pca.GetNumberOfComponents(0.95f);

            double[,] reducedUtilMat = pca.Transform(sourceMatrix, numCom);
            utilMat = new Matrix(reducedUtilMat.GetLength(1), reducedUtilMat.GetLength(0));
            for (int col = 0; col < reducedUtilMat.GetLength(0); col++)
                for (int row = 0; row < reducedUtilMat.GetLength(1); row++)
                    utilMat.set(row, col, sourceMatrix[col, row]);
            return utilMat;
        }
    }
}
