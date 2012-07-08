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
    class LogEnum:IEnumerable<string>
    {
        private string inputFilePath;
        public LogEnum (string inputFilePath)
        {
            this.inputFilePath = inputFilePath;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new LogEnumerator(inputFilePath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

    }

    class LogEnumerator : IEnumerator<string>
    {
        private string currentStr = null;
        private string nextStr = null;
        private StreamReader reader;
        private string inputFilePath;

        public LogEnumerator(string inputFilePath)
        {
            reader = File.OpenText(inputFilePath);
            nextStr = reader.ReadLine();
            this.inputFilePath = inputFilePath;
        }

        public string Current
        {
            get 
            {
                if (currentStr == null)
                    throw new InvalidOperationException();
                return currentStr; 
            }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            currentStr = nextStr;
            nextStr = reader.ReadLine();
            if (currentStr == null)
                return false;
            return true;
        }

        public void Reset()
        {
            reader.Close();
            reader = File.OpenText(inputFilePath);
            nextStr = reader.ReadLine();
        }
    }



}
