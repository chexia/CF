using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Analysis;
using System.Threading.Tasks;

namespace CF
{
    [Serializable()]
    class PCA
    {
        public double[,] feature_row;
        public double[,] feature_col;
        private int numFeatures;
        public Matrix cached_predictions;
        public Matrix raw;
        private double baselrate = 0.001;
        public double[] eigenValues;
        public double[] rowMean;
        public string savePrefix;
        public PCA(Matrix raw, int dims = 30, string savePrefix = "C:\\Users\\t-chexia\\Desktop\\ab test final\\dump\\mat_fac_big_")
        {
            this.savePrefix = savePrefix;
            this.raw = raw;
            numFeatures = dims;
            feature_row = new double[numFeatures, raw.GetLength(0)];
            feature_col = new double[numFeatures, raw.GetLength(1)];
            for (int i = 0; i < numFeatures; i++)
                for (int j = 0; j < raw.GetLength(0); j++)
                    feature_row[i, j] = 0.01;
            for (int i = 0; i < numFeatures; i++)
                for (int j = 0; j < raw.GetLength(1); j++)
                    feature_col[i, j] = 0.01;
            cached_predictions = new Matrix(raw.GetLength(0), raw.GetLength(1));
            foreach (int col in raw.getCols())
            {
                double sum = 0;
                double counter = 0;
                foreach (int row in raw.getRowsOfCol(col))
                {
                    double a = raw.get(row, col);
                    sum += raw.get(row, col);
                    if (double.IsNaN(sum))
                        Console.Write(1);
                    counter += 1;
                }
                foreach (int row in raw.getRowsOfCol(col))
                    cached_predictions.set(row, col, 0);
            }
        }
        public void compute()
        {
            //preprocess();
            int iterations = 20000;

            for (int feature = 0; feature < numFeatures; feature++)
            {
                //Console.WriteLine(feature);
                int counter = 0;
                bool converged = false;
                double total_err_old = Int32.MaxValue;
                int chance = 0;
                while (!converged)
                {
                    double lrate = (10 - counter/1000) * baselrate;// *Math.Log(iterations / (1.0 + counter));
                    double max_err = 0;
                    double total_err = 0;
                    foreach (int col in cached_predictions.getCols())
                        foreach (int row in cached_predictions.getRowsOfCol(col))
                        {
                            double rea = raw.get(row, col);
                            double predicted = predictRating(row, col, feature);
                            double err = lrate * (rea - predicted);
                            double fr = feature_row[feature, row];
                            double fc = feature_col[feature, col];
                            feature_row[feature, row] = fr + err * feature_col[feature, col] - 0.1 * fr;
                            feature_col[feature, col] = feature_col[feature, col] + err * fr - 0.1 * feature_col[feature, col];
                            if (rea != 0)
                                max_err = Math.Max(Math.Abs(rea - predicted) / (rea+1), max_err);
                            total_err += Math.Pow((rea - predicted),2);
                        }
                    
                    Console.WriteLine("" + feature + "\t" + counter + "\t" + max_err + "\t" + total_err);
                    counter++;
                    if (Math.Abs(Math.Abs(total_err) - Math.Abs(total_err_old))<Math.Pow(10, -10))
                        chance++;
                    else
                        chance = 0;
                    if (counter > iterations || chance>3)
                        converged = true;
                    total_err_old = total_err;

                }
                foreach (int col in cached_predictions.getCols())
                    foreach (int row in cached_predictions.getRowsOfCol(col))
                        cached_predictions.set(row, col, predictRating(row, col, feature));
                IO.save(this, this.savePrefix + feature);
            }
            normalize();
            //postprocess();
        }
        public void recompute()
        {
            foreach (int col in this.cached_predictions.getCols())
                foreach (int row in this.cached_predictions.getRowsOfCol(col))
                    this.cached_predictions.set(row, col, 0);
            this.compute();
        }
        public void expandFeatures(int new_numFeatures)
        {
            if (new_numFeatures < this.numFeatures)
                throw new ArgumentException("must have larger number of features");
            double[,] new_feature_row = new double[new_numFeatures, raw.GetLength(0)];
            for (int feature = 0; feature < feature_row.GetLength(0); feature++)
                for (int row = 0; row < feature_row.GetLength(1); row++)
                    new_feature_row[feature, row] = feature_row[feature, row];
            this.feature_row = new_feature_row;
            double[,] new_feature_col = new double[new_numFeatures, raw.GetLength(1)];
            for (int feature = 0; feature < feature_col.GetLength(0); feature++)
                for (int col = 0; col < feature_col.GetLength(1); col++)
                    new_feature_col[feature, col] = feature_col[feature, col];
            this.feature_col = new_feature_col;
            double[] new_eigenValues = new double[new_numFeatures];
            this.numFeatures = new_numFeatures;

        }

        public void cont_compute(int s)
        {
            int iterations = 30000;

            for (int feature = s; feature < numFeatures; feature++)
            {
                //Console.WriteLine(feature);
                int counter = 0;
                bool converged = false;
                while (!converged)
                {
                    double lrate = baselrate * Math.Log(iterations / (1.0 + counter));
                    Console.WriteLine(""+feature+"\t"+counter);
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
                    if (counter > iterations)
                        converged = true;

                }
                foreach (int col in cached_predictions.getCols())
                    foreach (int row in cached_predictions.getRowsOfCol(col))
                        cached_predictions.set(row, col, predictRating(row, col, feature));
                IO.save(this, "C:\\Users\\t-chexia\\Desktop\\ab test final\\savedPCA_big_" + feature);
            }
            normalize();
            //postprocess();
        }
        private void preprocess()
        {
            rowMean = new double[raw.GetLength(0)];
            for (int row = 0; row < raw.GetLength(0); row++)
            {
                double sum=0;
                double count=0;
                for (int col = 0; col < raw.GetLength(1); col++)
                    if (raw.contains(row, col))
                    {
                        sum += raw.get(row, col);
                        count += 1;
                    }
                double mean = sum / count;
                for (int col = 0; col < raw.GetLength(1); col++)
                {
                    if (raw.contains(row, col))
                        raw.set(row, col, raw.get(row, col) - mean);
                }
                rowMean[row] = mean;
            }
        }
        private void postprocess()
        {
            //rowMean = new double[raw.GetLength(0)];
            for (int row = 0; row < raw.GetLength(0); row++)
            {
                for (int col = 0; col < raw.GetLength(1); col++)
                {
                    if (raw.contains(row, col))
                        raw.set(row, col, raw.get(row, col) + rowMean[row]);
                }
            }
        }
        public double predictRating(int row, int col, int feature)
        {
            double sum = 0;
            sum += feature_row[feature, row] * feature_col[feature, col];
            double rtn = sum + cached_predictions.get(row, col);
            return rtn;
        }

        public double[,] rc_eigenvectors()
        {
            return feature_col;
        }
        public double[,] rr_eigenvectors()
        {
            return feature_row;
        }

        public void normalize()
        {
            this.eigenValues = new double[numFeatures];
            for (int i = 0; i < eigenValues.Length; i++)
            {
                double ev = normFeature(feature_row, i) * normFeature(feature_col, i);
                eigenValues[i] = ev;
            }
        }
        private double normFeature(double[,] mat, int featureNumber)
        {
            double sqsum = 0;
            for (int i = 0; i < mat.GetLength(1); i++)
                sqsum += Math.Pow(mat[featureNumber, i],2);
            double magnitude = Math.Sqrt(sqsum);
            if (magnitude == 0)
                return 1;
            for (int i = 0; i < mat.GetLength(1); i++)
                mat[featureNumber, i] /= magnitude;
            return magnitude;
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

        public Matrix transform(int numDims, int compDim = 0)
        {
            double[,] transMat;
            Matrix rtn = null;
            if (compDim == 0)
            {
                rtn = new Matrix(numFeatures, raw.GetLength(1), null);
                transMat = this.feature_row;
                foreach (int col in raw.getCols())
                    for (int feature = 0; feature < numDims; feature++)
                    {
                        double sum = 0;
                        foreach (int row in raw.getRowsOfCol(col))
                            sum += (raw.get(row, col) * transMat[feature, row]);
                        rtn.set(feature, col, sum);
                    }
            }
            return rtn;
        }
        public Matrix transform(int compDim = 0)
        {
            double[,] transMat;
            Matrix rtn = null;
            if (compDim == 0)
            {
                rtn = new Matrix(numFeatures, raw.GetLength(1), null);
                transMat = this.feature_row;
                foreach (int col in raw.getCols())
                    for (int feature = 0; feature < numFeatures; feature++)
                    {
                        double sum = 0;
                        foreach (int row in raw.getRowsOfCol(col))
                            sum += (raw.get(row, col) * transMat[feature, row]);
                        rtn.set(feature, col, sum);
                    }
            }
            return rtn;
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
            for (int i = 0; i < sourceMatrix.GetLength(0); i++)
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
    }
}
