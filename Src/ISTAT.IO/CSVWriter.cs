using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ISTAT.IO
{
    public class CSVWriter
    {
        public static Stream CreateStream(List<string[]> dataCSV,char separator='|'){

            MemoryStream mem = new MemoryStream();
            StreamWriter w = new StreamWriter(mem, new UnicodeEncoding(false, true));
            for (int i = 0; i < dataCSV.Count; i++)
            {
                int count = dataCSV.First().Length;
                string line = string.Empty;

                string sep="";
                for (int j = 0; j < count; j++)
                {
                    line += sep + ((dataCSV[i][j] != null) ? dataCSV[i][j] : string.Empty);
                    sep = separator.ToString();
                }
                w.WriteLine(line);
                w.Flush();
            }
            return w.BaseStream;

        }
    }
}
