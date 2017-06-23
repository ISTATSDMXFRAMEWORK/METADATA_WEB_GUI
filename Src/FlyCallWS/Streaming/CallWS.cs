using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace FlyCallWS.Streaming
{
    public class CallWS
    {
        public CallWS(string FileName, int MaxByteReturn)
        {
            FileSavedPath = FileName;
            MAX_OUTPUT_LENGTH = MaxByteReturn;
        }
        private string FileSavedPath = null;
        private int MAX_OUTPUT_LENGTH = 10485760; // 10MB
        public String LastError { get; set; }

        public string SendSOAPQuery(XmlDocument query, WsConfigurationSettings _settings)
        {
            WebServiceClient client = new WebServiceClient(_settings);
            HttpWebRequest SoapReq = client.InvokeMethod(query);
            SoapReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
            var response = GetWSResponse(SoapReq);
            return response;
        }
        public string SendRESTQuery(string query, string Headers, WsConfigurationSettings _settings)
        {
            _settings.EndPoint += query;
            RESTServiceClient client = new RESTServiceClient(_settings, Headers);
            HttpWebRequest RestReq = client.InvokeMethod();
            return GetWSResponse(RestReq);
        }


        private string GetWSResponse(HttpWebRequest _request)
        {
            ClientStreaming clientStreaming = new ClientStreaming();

            FileInfo fi = new FileInfo(FileSavedPath);
            if (!fi.Directory.Exists) fi.Directory.Create();
            fi = null;


            bool AllOkResult = true;
            using (var fs = new FileStream(FileSavedPath, FileMode.Create))
            {
                AllOkResult = clientStreaming.GetResponse(_request, fs);
            }


            int nBytes;
            //string result = string.Empty;

            nBytes = MAX_OUTPUT_LENGTH;


            byte[] BUSbuffer = null;
            int totRead = 0;
            var result = new StringBuilder(nBytes);

            using (StreamReader rea = new StreamReader(FileSavedPath))
            {
                do
                {
                    BUSbuffer = new byte[2097152];
                    var read = rea.BaseStream.Read(BUSbuffer, 0, BUSbuffer.Length);
                    if (read == 0)
                    {
                        break;
                    }
                    result.Append(Encoding.UTF8.GetString(BUSbuffer, 0, read));

                    totRead += read;
                }
                while (totRead <= nBytes);


                BUSbuffer = new byte[128];


            }
            if (!AllOkResult || !result.ToString().StartsWith("<"))
            {
                this.LastError = result.ToString();
                return null;
            }
            //return result.ToString();
            //var error = CheckForError(result.ToString());
            //if (!string.IsNullOrEmpty(error))
            //{
            //    if (error.Contains("No data found")) //Don't like, due to SRA-166
            //        throw new Exception(error);
            //    throw new XmlException(error);
            //}

            // AndreaQ fix // Se nell'xml c'erano commenti, eliminava tutto il codice successivo al commento
            string CompleteResponse = FormatXmlString(result.ToString(), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreComments = true });
            //
            using (StreamWriter wricutted = new StreamWriter(FileSavedPath + "Cutted.xml", false))
            {
                wricutted.Write(CompleteResponse);
            }
            if (AllOkResult)
                return CompleteResponse;
            else
            {
                this.LastError = CompleteResponse.ToString();
                return null;
            }
        }



        #region Formatting
        private string CheckForError(string result)
        {
            const string RegexStr = "<ErrorMessage[^>]*?>(?<ErrorText>.*?)</ErrorMessage>";
            MatchCollection mc = Regex.Matches(result, RegexStr, RegexOptions.Singleline);

            if (mc.Count > 0)
            {
                return mc[0].Groups["ErrorText"].Value;
            }

            return null;
        }
        private string FormatXmlString(string xmlString, XmlReaderSettings xmlReaderSettings)
        {
            try
            {
                var stringBuilder = new StringBuilder();
                
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), xmlReaderSettings))
                {
                    //var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, IndentChars = "\t", Encoding = Encoding.UTF8 };

                    using (var xmlWriter = new XmlTextWriter(new System.CodeDom.Compiler.IndentedTextWriter(new EncodingStringWriter(stringBuilder,Encoding.UTF8))))
                    {
                        FormatXml(reader, xmlWriter);
                    }
                }
                return stringBuilder.ToString();
            }
            catch (Exception)
            {

                return xmlString;
            }
            

        }
        private void FormatXml(XmlReader reader, XmlTextWriter xmlWriter)
        {
            if (xmlWriter == null || reader == null)
                return;

            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartDocument();
            try
            {
                reader.MoveToContent();
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                if (reader.LocalName.Trim().ToLower() == "envelope")
                {
                    reader.Read();
                    while (reader.LocalName.Trim().ToLower() != "body")
                        reader.Read();
                    reader.Read();
                    if (reader.Prefix == "web")
                        reader.Read();

                    //Qui scoppia quando il file è troppo lungo (ma solo in debug) lo sta troncando
                    xmlWriter.WriteNode(reader, true);
                }
                else
                {
                    while (!reader.EOF)
                    {//Qui scoppia quando il file è troppo lungo (ma solo in debug) lo sta troncando
                        xmlWriter.WriteNode(reader, true);
                    }
                }
            }
            catch (XmlException)
            {
                xmlWriter.WriteEndDocument();
            }

        }
        #endregion





    }
}
