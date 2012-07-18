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
    class IntegerMap
    {
        private int count = 0;
        private Dictionary<string, int> mapper;
        private Dictionary<int, string> revmapper;
        public IntegerMap()
        {
            revmapper = new Dictionary<int, string>();
            mapper = new Dictionary<string, int>();
        }
        public void add(string inputFilePath, int pos)
        {
            LogEnum logenum = new LogEnum(inputFilePath);
            List<double[]> points = new List<double[]>();
            double numEntries = 0;
            foreach (string line in logenum)
            {
                string[] tokens = line.Split(new char[] { '\t' });
                this.add(tokens[pos]);
                numEntries += 1;
            }
        }
        public void add(string newItem)
        {
            if (!mapper.ContainsKey(newItem))
            {
                revmapper.Add(count, newItem);
                mapper.Add(newItem, count);
                count++;
            }
        }
        public bool contains(string key)
        {
            return mapper.ContainsKey(key);
        }
        public int get(string key)
        {
            return mapper[key];
        }
        public int getCount()
        {
            return count;
        }
        public string getItemByInt(int x)
        {
            return revmapper[x];
        }

    }

}
