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
    /// <summary>
    /// A class for representing sparse matrices. The primary data-structure used is a nested dicionary. This allows constant time lookup for single
    /// entries as well as for all entries in a given column.
    /// </summary>
    [Serializable()]
    class PointMatrix
    {
        /// <summary>
        /// Where the data points are actually stored. This is a nested dictionary, the outer dictionary indexes the column,
        /// and the inner dictionary index the row.
        /// </summary>
        public Dictionary<int, Dictionary<int, double>> sourceMatrix;

        /// <summary>
        /// number of rows and columns in the matrix respectively.
        /// </summary>
        private int colnum, rownum;

        /// <summary>
        /// Constructor for Point Matrix
        /// </summary>
        /// <param name="rownum">number of rows of matrix</param>
        /// <param name="colnum">number of columns of matrix</param>
        public PointMatrix(int rownum, int colnum)
        {
            this.rownum = rownum;
            this.colnum = colnum;
            this.sourceMatrix = new Dictionary<int, Dictionary<int, double>>();
        }

        /// <summary>
        /// Returns the value of entry (row, col). Note that this actually returns sourceMatrix[col][row], since the outer dictionary index columns, and the inner indexes rows
        /// </summary>
        /// <param name="row">row index of value to be returned</param>
        /// <param name="col">column index of value to be returned</param>
        /// <returns>the value of the entry at (row, col), or double.NaN if matrix does not contain given entry</returns>
        public double get(int row, int col)
        {
            if (row < 0 || row >= this.rownum || col < 0 || col >= this.colnum)
                throw new ArgumentException("attempt to get out-of-bounds item");
            if (!sourceMatrix.ContainsKey(col))
                return double.NaN;
            if (!(sourceMatrix[col].ContainsKey(row)))
                return double.NaN;
            return sourceMatrix[col][row];
        }
        /// <summary>
        /// Sets the value of entry (row, col)
        /// </summary>
        /// <param name="row">row index of entry to be set</param>
        /// <param name="col">column index of entry to be set</param>
        /// <param name="value">value to be set</param>
        public void set(int row, int col, double value)
        {
            if (row < 0 || row >= this.rownum || col < 0 || col >= this.colnum)
                throw new ArgumentException("attempt to set out-of-bounds item");
            if (!sourceMatrix.ContainsKey(col))
                sourceMatrix.Add(col, new Dictionary<int, double>());
            sourceMatrix[col][row] = value;
        }
        /// <summary>
        /// returns the length of the matrix along a dimension
        /// </summary>
        /// <param name="dim">0 returns number of rows, 1 returns number of columns</param>
        /// <returns>length of matrix along a dimension</returns>
        public int GetLength(int dim)
        {
            if (dim == 0)
                return rownum;
            else
                return colnum;
        }
        /// <summary>
        /// returns true if matrix contains an entry at (row, col), false otherwise
        /// </summary>
        /// <param name="row">row index</param>
        /// <param name="col">column index</param>
        /// <returns>true if matrix contains entry at (row, col), false otherwise</returns>
        public bool contains(int row, int col)
        {
            if (!sourceMatrix.ContainsKey(col))
                return false;
            if (!(sourceMatrix[col].ContainsKey(row)))
                return false;
            return true;
        }

    }
    /*
     * not used anymore
     * 
    abstract class GenericMatrix
    {
        public abstract double get(int row, int col);
        public abstract void set(int row, int col);
        public abstract int GetLength(int dim);
        public abstract bool contains(int row, int col);
    }
     * */
}
