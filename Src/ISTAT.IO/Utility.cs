using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.IO
{
    public class Utility
    {

        public class FileGeneric
        {
            public string filename;
            public int filesize;
            public string content;
            public string filePath;
            public System.IO.Stream stream;
            public byte[] bytes;
        }
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            System.IO.FileStream fs = new System.IO.FileStream(fileName,
                                           System.IO.FileMode.Open,
                                           System.IO.FileAccess.Read);
           
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            long numBytes = new System.IO.FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            fs.Close();
            return buff;
        }
        public static void SplitFile(System.IO.Stream inputStream, int chunkSize, string path)
        {
            byte[] buffer = new byte[chunkSize];
            List<byte> extraBuffer = new List<byte>();

            int index = 0;
            while (inputStream.Position < inputStream.Length)
            {
                using (System.IO.Stream output = System.IO.File.Create(path + "\\" + index + ".csv"))
                {
                    int chunkBytesRead = 0;
                    while (chunkBytesRead < chunkSize)
                    {
                        int bytesRead = inputStream.Read(buffer,
                                                            chunkBytesRead,
                                                            chunkSize - chunkBytesRead);

                        if (bytesRead == 0)
                        {
                            break;
                        }

                        chunkBytesRead += bytesRead;
                    }

                    byte extraByte = buffer[chunkSize - 1];
                    while (extraByte != '\n')
                    {
                        int flag = inputStream.ReadByte();
                        if (flag == -1) break;
                        extraByte = (byte)flag;
                        extraBuffer.Add(extraByte);
                    }

                    output.Write(buffer, 0, chunkBytesRead);
                    if (extraBuffer.Count > 0)
                        output.Write(extraBuffer.ToArray(), 0, extraBuffer.Count);

                    extraBuffer.Clear();
                }
                index++;
            }
        }
    }
}
