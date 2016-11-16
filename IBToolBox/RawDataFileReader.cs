using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBToolBox
{
    public static class RawDataFileReader
    {
        public static string ImportRaw(string filepath)
        {
            string raw;
            using (var sr = new StreamReader(filepath))
            {
                raw = sr.ReadToEnd();
                sr.Close();
            }
            return raw;
        }
    }
}
