﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF
{
    class PCAMatrix: Matrix
    {
        private Matrix simMat;
        public PCAMatrix(int numRow, int numCol, List<double[]> points)
            : base(numRow, numCol, points)
        {
            normalize();
            PCA pca = new PCA(this);
            pca.compute();
            this.simMat = pca.transform();
        }

        /// <summary>
        /// Overloaded similarity function for an entire array of columns to compare with a principal column
        /// returns an array of similarity scores
        /// </summary>
        /// <param name="principal">index of "principal column", against which all other columns are compared</param>
        /// <param name="neighbors">array of indices for neighbor columns</param>
        /// <returns>Array of similarity scores, product of cosineSim and jacSim</returns>
        public virtual double[] sim(int principal, int[] neighbors)
        {
            double[] rtn = new double[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
                rtn[i] = this.simMat.cosineSim(principal, neighbors[i]) * this.simMat.jacSim(principal, neighbors[i]);
            return rtn;
        }

        /// <summary>
        /// computes composite similarity score between two columns
        /// </summary>
        /// <param name="principal">index of one column</param>
        /// <param name="neighbor">index of another column</param>
        /// <returns>similarity score, product of cosineSim and jacSim</returns>
        public virtual double sim(int principal, int neighbor)
        {
            double rtn = this.simMat.cosineSim(principal, neighbor) * this.simMat.jacSim(principal, neighbor);
            return rtn;
        }


    }
}
