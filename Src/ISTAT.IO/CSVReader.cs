using System;
using System.IO;
using System.Text;
using System.Collections;

namespace ISTAT.IO
{
    public class FileCSV : ISTAT.IO.Utility.FileGeneric
    {
        public string[] headers;
        public char separator;
        public bool firstRowHeader;
        public bool IsValid { get { return (filename != string.Empty && content != string.Empty && headers != null); } }
    
    }

    public class CSVReader
    {
        private Stream objStream;
        private StreamReader objReader;

        private int _lastBufferRead;

        public int LastBufferRead { get { return _lastBufferRead; } }

        private char separator = ';';

        public char Separator { get { return this.separator; } set { this.separator = value; } }

        public CSVReader(Stream filestream, Encoding enc = null)
        {

            this.objStream = filestream;
            _lastBufferRead = 0;
            //check the Pass Stream whether it is readable or not
            if (!filestream.CanRead) return;

            objReader = (enc != null) ? new StreamReader(filestream, enc) : new StreamReader(filestream);
        }


        public string[] GetHeaders() {
            objReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string[] headers = GetCSVLine();

            return headers;
        }
        public string[] GetCSVLine()
        {

            string data = objReader.ReadLine();
            if (data == null) return null;
            _lastBufferRead = data.Length;
            if (data.Length == 0) return new string[0];
            ArrayList result = new ArrayList();

            ParseCSVData(result, data);
            return (string[])result.ToArray(typeof(string));
        }

        private void ParseCSVData(ArrayList result, string data)
        {
            int position = -1;

            while (position < data.Length) {
                string field = ParseCSVField(ref data, ref position);
                if (field!=string.Empty) 
                    result.Add(field);
                else result.Add(string.Empty);
            }
        }

        private string ParseCSVField(ref string data, ref int StartSeperatorPos)
        {

            if (StartSeperatorPos == data.Length - 1)
            {
                StartSeperatorPos++;
                return "";
            }

            int fromPos = StartSeperatorPos + 1;
            if (data[fromPos] == '"')
            {
                int nextSingleQuote = GetSingleQuote(data, fromPos + 1);
                int lines = 1;
                while (nextSingleQuote == -1)
                {
                    data = data + "\n" + objReader.ReadLine();
                    nextSingleQuote = GetSingleQuote(data, fromPos + 1);
                    lines++;

                    if (lines > 20)
                        throw new Exception("lines overflow: " + data);

                }

                StartSeperatorPos = nextSingleQuote + 1;
                string tempString = data.Substring(fromPos + 1, nextSingleQuote - fromPos - 1);
                tempString = tempString.Replace("'", "''");

                return tempString.Replace("\"\"", "\"");

            }

            int nextComma = data.IndexOf(this.Separator, fromPos);
            if (nextComma == -1)
            {
                StartSeperatorPos = data.Length;

                return data.Substring(fromPos);

            }
            else
            {

                StartSeperatorPos = nextComma;

                return data.Substring(fromPos, nextComma - fromPos);

            }
        }

        private int GetSingleQuote(string data, int SFrom)
        {

            int i = SFrom - 1;

            while (++i < data.Length)
                if (data[i] == '"')
                {
                    if (i < data.Length - 1 && data[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }
                    else return i;
                }

            return -1;

        }


    }

}
