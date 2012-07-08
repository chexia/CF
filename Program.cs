using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF
{
    class CDT //I completely forgot what CDT stands for
    {
        Matrix user_ad;
        Matrix page_ad;
        Matrix query_ad;

        //a deletage for computing distances between two vectors (a vector can be an user/page/query)
        


        public CDT()
        {
            //initialize the utility matrices, currently intend user_ad to contain 1/0 page_ad/query_ad to contain click-through-rate type double
            user_ad = page_ad = query_ad = new Matrix(); //in reality obtain the matrix values from cosmos or something
            double[] user_avg=user_ad.normalize();
            double[] page_avg=page_ad.normalize();
            double[] query_avg=query_ad.normalize();

            //below is a sample code for user_ad, same procedure is applied to page_ad and query_ad
            //Functions.naiveEstimate(user_ad, new Functions.similarityDelegate(Functions.naiveSimilarity));
            
        }


        /*
        static void Main(string[] args)
        {
            double[][] foo=new double [2][];
            foo[0] = new double[3] { 3, 6, 9 };
            foo[1] = new double[3] { 11, 12, 13 };
            CDT testCDT = new CDT();

            //Console.WriteLine(testCDT.calcDist(foo[0],foo[1]));
        }*/

        /*
         * a sample method for computing distances, uses cosine distance
         */
        


    }

    /*
     * temporary matrix implementation, very slow, will want to get a good matrix library off the web
     */ 
    class Matrix
    {
        private double[][] mat;
        public readonly int row, col;

        public Matrix()
        {
            row = 1;
            col = 1;
        }
        public static Matrix multiply (Matrix a, Matrix b)
        {
            return new Matrix();
        }
        public double[] getRow(int i)
        {
            if (i >= row)
                throw new ArgumentOutOfRangeException("index " + i + " out of bounds");
            else
                return mat[i];
        }
        public double getEntry(int i, int j)
        {
            if (i >= row || j >= col)
                throw new ArgumentOutOfRangeException("index " + i + "'" + j + "out of bounds");
            else
                return mat[i][j];
        }
        public void setEntry(int i, int j, double val)
        {
            if (i >= row || j >= col)
                throw new ArgumentOutOfRangeException("index " + i + "'" + j + "out of bounds");
            else
                mat[i][j] = val;
        }

        /*
         * normalize the matrix so that each row has average 0
         * returns the original averages
         */
        public double[] normalize()
        {
            double[] rtn = new double[row];
            for (int i = 0; i < row; i++)
            {
                double tmp = 0;
                for (int j = 0; j < col; j++)
                {
                    if (mat[i][j] == Double.NaN)
                        continue;
                    else
                        tmp += mat[i][j];
                }
                double avg = tmp / col;
                for (int j = 0; j < col; j++)
                {
                    if (mat[i][j] == Double.NaN)
                        continue;
                    else
                        mat[i][j] -= avg;
                }
                rtn[i] = avg;
            }
            return rtn;
        }
    }

    /*
     * a class of all misc functions used, such as various distance calculation and similarity implementations. May want to resort later.
     */
    class Functions
    {
        public delegate double calcDistDelegate(double[] a, double[] b);//a delegate for distance calculation functions, I do not like this, may change later
        public static double calcCosineDist(double[] a, double[] b)
        {
            int len = a.Length;
            if (len != b.Length)
            {
                throw new ArgumentException("vector dimensons must match for distance computation");
            }

            double numerator, aNorm, bNorm;
            numerator = aNorm = bNorm = 0;

            for (int i = 0; i < len; i++)
            {
                if (a[i] == Double.NaN || b[i] == Double.NaN)
                    continue;
                numerator += a[i] * b[i];
                aNorm += Math.Pow(a[i], 2);
                bNorm += Math.Pow(b[i], 2);

            }
            return numerator / (Math.Sqrt(aNorm) * Math.Sqrt(bNorm));
        }
        public static double calcJaccardDist(double[] a, double[] b)
        {
            int len = a.Length;
            if (len != b.Length)
            {
                throw new ArgumentException("vector dimensons must match for distance computation");
            }
            double intersect, union;
            intersect = union = 0;

            for (int i = 0; i < len; i++)
            {
                if (a[i] == Double.NaN || b[i] == Double.NaN)
                    continue;
                if (a[i] != 0 && b[i] != 0)
                    intersect += 1;
                if (a[i] != 0 || b[i] != 0)
                    union += 1;
            }
            return intersect / union;
        }


        //a delegate for computing k-nearest neighbors for a vector, returns a vector denoting the row indices of nearest neighbors.
        public delegate int[] similarityDelegate(int k, calcDistDelegate c, Matrix m, int r);
        /*
        //k-nearest neighbor similarity using custom distance calculation, implemented using a priority queue. Expected to be very slow, will want to use LSH in real code
        public static int[] naiveSimilarity(int k, calcDistDelegate calcDist, Matrix m, int rowInd)
        {
            int[] nn = new int[k];
            //assuming a heap/priority queue implementation that takes a generic comparator
            maxHeap heap = new maxHeap(calcDist);
            int n = m.row;
            double[] principal=m.getRow(rowInd);
            for (int i = 0; i < n; i++)
            {
                if (i == rowInd)
                    continue;
                else
                {
                    if (heap.size < k)
                        heap.add(i);
                    else
                    {
                        int tmp = heap.pop();
                        if (calcDist(m.getRow(tmp), principal) > calcDist(m.getRow(i), principal))
                            //at this point, may want to normalize w.r.t the sum over the CTR values to ensure that similarity does not favor those with less common factors.
                            heap.add(i);
                        else
                            heap.add(tmp);
                    }
                }
            }
            nn = heap.toArray;
            return nn;
        }
        */
        public static void naiveEstimate(Matrix m, similarityDelegate similarity)
        {
            int numNeighbors=10;
            calcDistDelegate calcDist=new calcDistDelegate(calcCosineDist);
            for (int i=0;i<m.row;i++)
            {
                int[] nearestNeighbors = similarity(numNeighbors, calcDist, m, i);
                for (int j = 0; j < m.col; j++)
                {
                    if (m.getEntry(i, j) == Double.NaN)
                    {
                        double result = 0;
                        for (int k = 0; k < numNeighbors; k++)
                        {
                            result += m.getEntry(nearestNeighbors[k],j);
                        }
                        m.setEntry(i, j, result / numNeighbors);
                    }
                    else
                        continue;
                }
            }
        }

    }
}
