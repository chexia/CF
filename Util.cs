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
    [Serializable()]
    class PointMatrix
    {

        public Dictionary<int, Dictionary<int, double>> hashMap;
        private int colnum, rownum;
        public PointMatrix(int rownum, int colnum)
        {
            this.rownum = rownum;
            this.colnum = colnum;
            this.hashMap = new Dictionary<int, Dictionary<int, double>>();
            colnum = rownum = 0;
        }
        public double get(int row, int col)
        {
            if (!hashMap.ContainsKey(col))
                return -1;
            if (!(hashMap[col].ContainsKey(row)))
                return -1;
            return hashMap[col][row];
        }
        public void set(int row, int col, double value)
        {
            if (!hashMap.ContainsKey(col))
                hashMap.Add(col, new Dictionary<int, double>());
            hashMap[col][row] = value;
        }
        public int GetLength(int dim)
        {
            if (dim == 0)
                return rownum;
            else
                return colnum;
        }
        public bool contains(int row, int col)
        {
            if (!hashMap.ContainsKey(col))
                return false;
            if (!(hashMap[col].ContainsKey(row)))
                return false;
            return true;
        }

    }

    abstract class GenericMatrix
    {
        public abstract double get(int row, int col);
        public abstract void set(int row, int col);
        public abstract int GetLength(int dim);
        public abstract bool contains(int row, int col);
    }
}
