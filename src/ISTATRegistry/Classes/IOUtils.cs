#define REPLACE_LF_CR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using Org.Sdmxsource.Sdmx.Structureparser.Manager;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using ISTAT.EXPORT;
using ISTAT.IO;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using ISTAT.Entity;

namespace ISTATRegistry
{
    public class IOUtils
    {

        #region "Constructors"

        public IOUtils() { }

        #endregion

        #region "Public Methods"

        public void SaveRTFFile(Elistia.DotNetRtfWriter.RtfDocument doc, string outputFileName)
        {
            string rtfString = doc.render();

            byte[] bytesInStream = Encoding.ASCII.GetBytes(rtfString);

            SendAttachment(bytesInStream, outputFileName + ".rtf");
        }

        public void SaveSDMXFile(ISdmxObjects sdmxObjects, StructureOutputFormatEnumType version, string outputFileName)
        {

            StructureWriterManager swm = new StructureWriterManager();

            StructureOutputFormat soFormat = StructureOutputFormat.GetFromEnum(version);
            IStructureFormat outputFormat = new SdmxStructureFormat(soFormat);

            MemoryStream memoryStream = new MemoryStream();

            swm.WriteStructures(sdmxObjects, outputFormat, memoryStream);


            byte[] bytesInStream = memoryStream.ToArray();
            memoryStream.Close();

            SendAttachment(bytesInStream, outputFileName + ".xml");
        }



        public void SaveMultipleCSVFile(List<DataTable> dts, List<string> outputFileName, string separator, string textDelimiter)
        {
            //byte[] bytesInStream = null;
            string fName = "mutipleCSVFile_" + GetStrindDate() +".zip";
            List<ISTAT.IO.Utility.FileGeneric> files = new List<ISTAT.IO.Utility.FileGeneric>();
            MemoryStream tempStream;
            StreamWriter writer;
            int countName = 0;

            foreach (DataTable dt in dts)
            {

                tempStream = new MemoryStream();

                writer = new StreamWriter(tempStream);

                WriteDataTable(dt, writer, true, separator, textDelimiter);

                ISTAT.IO.Utility.FileGeneric file_code = new ISTAT.IO.Utility.FileGeneric();
                file_code.filename = outputFileName[countName];
                file_code.stream = tempStream;
                files.Add(file_code);

                ++countName;
            }

            string fileZip = System.Web.HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + fName);

            System.IO.File.Delete(fileZip);
            Ionic.Utils.Zip.ZipFile zip = new Ionic.Utils.Zip.ZipFile(fileZip);
            foreach (ISTAT.IO.Utility.FileGeneric file in files)
                zip.AddFileStream(file.filename, string.Empty, file.stream);
            zip.Save();

            SendAttachment(fileZip, fName);
        }


        public void SaveCSVFile(DataTable dt, string outputFileName, string separator, string textDelimiter)
        {
            byte[] bytesInStream = null;

            using (MemoryStream tempStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(tempStream))
                {
                    WriteDataTable(dt, writer, true, separator, textDelimiter);
                }

                bytesInStream = tempStream.ToArray();
            }

            SendAttachment(bytesInStream, outputFileName + ".csv");

        }


        public void SaveMultipleDotSTATCodelistFile(List<ICodelistObject> codelists, ISTAT.Entity.DotStatProperties dsp)
        {
            string fName = "multipleDotStatFile_"+ GetStrindDate() +".zip";
            string finalFile = System.Web.HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\"+ fName);
            //ISTAT.IO.Utility.FileGeneric singleFile = new ISTAT.IO.Utility.FileGeneric();
            System.IO.File.Delete(finalFile);

            Ionic.Utils.Zip.ZipFile zipFiles = new Ionic.Utils.Zip.ZipFile(finalFile);

            foreach (ICodelistObject codelist in codelists)
            {
                CodelistExporter _codeExp = new CodelistExporter(codelist.Id, codelist, dsp, GetLanguages());
                List<ISTAT.IO.Utility.FileGeneric> files = new List<ISTAT.IO.Utility.FileGeneric>();
                List<ContactRef> contacs = GetConfigContact(dsp);
                string ExportFileName;

                ExportFileName = "DotStatExport-" + codelist.Id + "_" + codelist.AgencyId + "_" + codelist.Version;

                _codeExp.CreateData(contacs);

                System.Xml.XmlDocument xDoc_code = _codeExp.XMLDoc;
                MemoryStream xmlStream_code = new MemoryStream();
                xDoc_code.Save(xmlStream_code);
                xmlStream_code.Flush();
                xmlStream_code.Position = 0;
                ISTAT.IO.Utility.FileGeneric file_code = new ISTAT.IO.Utility.FileGeneric();
                file_code.filename = _codeExp.Code.ToString() + ".xml";
                file_code.stream = xmlStream_code;
                files.Add(file_code);

                Stream streamCSV = CSVWriter.CreateStream(_codeExp.DataView);
                ISTAT.IO.Utility.FileGeneric file_csv = new ISTAT.IO.Utility.FileGeneric();
                file_csv.filename = _codeExp.DataFilename;
                file_csv.stream = streamCSV;
                files.Add(file_csv);

                string fileZip = System.Web.HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + ExportFileName + ".zip");

                System.IO.File.Delete(fileZip);
                Ionic.Utils.Zip.ZipFile zip = new Ionic.Utils.Zip.ZipFile(fileZip);
                foreach (ISTAT.IO.Utility.FileGeneric file in files)
                    zip.AddFileStream(file.filename, string.Empty, file.stream);
                zip.Save();

                zipFiles.AddFileStream(ExportFileName +".zip",string.Empty, File.OpenRead(fileZip));

            }

            zipFiles.Save();

            SendAttachment(finalFile, fName);
        }

        private string GetStrindDate()
        {
            return DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
        }

        public void SaveDotSTATCodelistFile(ICodelistObject codelist, ISTAT.Entity.DotStatProperties dsp)
        {

            CodelistExporter _codeExp = new CodelistExporter(codelist.Id, codelist, dsp, GetLanguages());
            List<ISTAT.IO.Utility.FileGeneric> files = new List<ISTAT.IO.Utility.FileGeneric>();
            List<ContactRef> contacs = GetConfigContact(dsp);
            string ExportFileName;

            ExportFileName = "DotStatExport-" + codelist.Id + "_" + codelist.AgencyId + "_" + codelist.Version;

            _codeExp.CreateData(contacs);

            System.Xml.XmlDocument xDoc_code = _codeExp.XMLDoc;
            MemoryStream xmlStream_code = new MemoryStream();
            xDoc_code.Save(xmlStream_code);
            xmlStream_code.Flush();
            xmlStream_code.Position = 0;
            ISTAT.IO.Utility.FileGeneric file_code = new ISTAT.IO.Utility.FileGeneric();
            file_code.filename = _codeExp.Code.ToString() + ".xml";
            file_code.stream = xmlStream_code;
            files.Add(file_code);

            Stream streamCSV = CSVWriter.CreateStream(_codeExp.DataView);
            ISTAT.IO.Utility.FileGeneric file_csv = new ISTAT.IO.Utility.FileGeneric();
            file_csv.filename = _codeExp.DataFilename;
            file_csv.stream = streamCSV;
            files.Add(file_csv);

            string fileZip = System.Web.HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + ExportFileName + ".zip");

            System.IO.File.Delete(fileZip);
            Ionic.Utils.Zip.ZipFile zip = new Ionic.Utils.Zip.ZipFile(fileZip);
            foreach (ISTAT.IO.Utility.FileGeneric file in files)
                zip.AddFileStream(file.filename, string.Empty, file.stream);
            zip.Save();

            SendAttachment(fileZip, ExportFileName + ".zip");
        }

        public void SaveDotSTATFile(ISdmxObjects sdmxObjects, DotStatExportType exportType, DotStatProperties dsp)
        {
            string ExportFileName;

            ExportFileName = "DotStatExport-" + sdmxObjects.DataStructures.First().Id + "_" + sdmxObjects.DataStructures.First().AgencyId + "_" + sdmxObjects.DataStructures.First().Version;

            List<ISTAT.IO.Utility.FileGeneric> files = new List<ISTAT.IO.Utility.FileGeneric>();

            List<ContactRef> contacs = GetConfigContact(dsp);
            List<SecurityDef> securities = GetConfigSecurity(dsp);
            List<String> languages = GetLanguages();

            DSDExporter _dsdExp = new DSDExporter(sdmxObjects, dsp);

            switch (exportType)
            {
                case DotStatExportType.DSD:
                    if (_dsdExp.CreateData(
                        contacs,
                        securities,
                        languages,
                        true, false))
                    {
                        System.Xml.XmlDocument xDoc = _dsdExp.XMLDoc;

                        MemoryStream xmlStream = new MemoryStream();
                        xDoc.Save(xmlStream);

                        xmlStream.Flush();
                        xmlStream.Position = 0;

                        ISTAT.IO.Utility.FileGeneric file = new ISTAT.IO.Utility.FileGeneric();
                        file.filename = ExportFileName + ".xml";
                        file.stream = xmlStream;

                        files.Add(file);
                    }
                    break;
                case DotStatExportType.CODELIST:
                    if (_dsdExp.CreateData(
                        contacs,
                        securities,
                        languages,
                        true, false))
                    {
                        foreach (CodelistExporter _codeExp in _dsdExp.ExporterCodelists)
                        {
                            System.Xml.XmlDocument xDoc_code = _codeExp.XMLDoc;
                            MemoryStream xmlStream_code = new MemoryStream();
                            xDoc_code.Save(xmlStream_code);
                            xmlStream_code.Flush();
                            xmlStream_code.Position = 0;
                            ISTAT.IO.Utility.FileGeneric file_code = new ISTAT.IO.Utility.FileGeneric();
                            file_code.filename = _codeExp.Code.ToString() + ".xml";
                            file_code.stream = xmlStream_code;
                            files.Add(file_code);

                            // CSV_LOC
                            Stream streamCSV_LOC = CSVWriter.CreateStream(_codeExp.DataView);
                            ISTAT.IO.Utility.FileGeneric file_csv_loc = new ISTAT.IO.Utility.FileGeneric();
                            file_csv_loc.filename = _codeExp.DataFilename;
                            file_csv_loc.stream = streamCSV_LOC;
                            files.Add(file_csv_loc);

                            Stream streamCSV = CSVWriter.CreateStream(_codeExp.DataViewCsv);
                            ISTAT.IO.Utility.FileGeneric file_csv = new ISTAT.IO.Utility.FileGeneric();
                            file_csv.filename = _codeExp.DataFilenameCsv;
                            file_csv.stream = streamCSV;
                            files.Add(file_csv);
                        }
                    }
                    break;
                case DotStatExportType.ALL:
                    if (_dsdExp.CreateData(
                        contacs,
                        securities,
                        languages,
                        true, false))
                    {
                        System.Xml.XmlDocument xDoc = _dsdExp.XMLDoc;

                        MemoryStream xmlStream = new MemoryStream();
                        xDoc.Save(xmlStream);

                        xmlStream.Flush();
                        xmlStream.Position = 0;

                        ISTAT.IO.Utility.FileGeneric file = new ISTAT.IO.Utility.FileGeneric();
                        file.filename = ExportFileName + ".xml";
                        file.stream = xmlStream;

                        files.Add(file);
                        foreach (CodelistExporter _codeExp in _dsdExp.ExporterCodelists)
                        {
                            System.Xml.XmlDocument xDoc_code = _codeExp.XMLDoc;
                            MemoryStream xmlStream_code = new MemoryStream();
                            xDoc_code.Save(xmlStream_code);
                            xmlStream_code.Flush();
                            xmlStream_code.Position = 0;
                            ISTAT.IO.Utility.FileGeneric file_code = new ISTAT.IO.Utility.FileGeneric();
                            file_code.filename = _codeExp.Code.ToString() + ".xml";
                            file_code.stream = xmlStream_code;
                            files.Add(file_code);

                            // CSV_LOC
                            Stream streamCSV_LOC = CSVWriter.CreateStream(_codeExp.DataView);
                            ISTAT.IO.Utility.FileGeneric file_csv_loc = new ISTAT.IO.Utility.FileGeneric();
                            file_csv_loc.filename = _codeExp.DataFilename;
                            file_csv_loc.stream = streamCSV_LOC;
                            files.Add(file_csv_loc);

                            Stream streamCSV = CSVWriter.CreateStream(_codeExp.DataViewCsv);
                            ISTAT.IO.Utility.FileGeneric file_csv = new ISTAT.IO.Utility.FileGeneric();
                            file_csv.filename = _codeExp.DataFilenameCsv;
                            file_csv.stream = streamCSV;
                            files.Add(file_csv);

                        }
                    }
                    break;
            }

            string fileZip = System.Web.HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + ExportFileName + ".zip");

            System.IO.File.Delete(fileZip);
            Ionic.Utils.Zip.ZipFile zip = new Ionic.Utils.Zip.ZipFile(fileZip);
            foreach (ISTAT.IO.Utility.FileGeneric file in files)
                zip.AddFileStream(file.filename, string.Empty, file.stream);
            zip.Save();

            SendAttachment(fileZip, ExportFileName + ".zip");

        }

        #endregion

        #region "Private Methods"

        private void SendAttachment(byte[] bytesInStream, string fileName)
        {

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/force-download";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            HttpContext.Current.Response.BinaryWrite(bytesInStream);
            HttpContext.Current.Response.End();

        }

        public void SendAttachment(string filePath, string fileName)
        {
            var file = new System.IO.FileInfo(filePath);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.WriteFile(file.FullName);
            HttpContext.Current.Response.End();
        }

        private void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders, string separator, string textDelimiter)
        {
            if (includeHeaders)
            {
                List<string> headerValues = new List<string>();
                foreach (DataColumn column in sourceTable.Columns)
                {
                    headerValues.Add(column.ColumnName);
                }

                writer.WriteLine(String.Join(separator, headerValues.ToArray()));
            }

            string[] items = null;
            foreach (DataRow row in sourceTable.Rows)
            {
#if REPLACE_LF_CR
                items = row.ItemArray.Select(o => o.ToString().Replace("\n", " ")).ToArray();
#else
                items = row.ItemArray.Select( o => o.ToString()).ToArray();
#endif
                if (textDelimiter != string.Empty)
                    items = row.ItemArray.Select(o => textDelimiter + o.ToString() + textDelimiter).ToArray();


                writer.WriteLine(String.Join(separator, items));
            }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"", value.Replace("\"", "\"\""), "\"");
        }

        private List<ContactRef> GetConfigContact(DotStatProperties dsp)
        {
            List<ContactRef> contacs = new List<ContactRef>();
            contacs.Add(new ContactRef()
            {
                name = dsp.ContactName,
                direction = dsp.ContactDirection,
                email = dsp.ContactEMail
            });

            return contacs;
        }

        private List<SecurityDef> GetConfigSecurity(DotStatProperties dsp)
        {
            List<SecurityDef> securities = new List<SecurityDef>();
            securities.Add(new SecurityDef()
            {
                domain = dsp.SecurityDomain,
                userGroup = dsp.SecurityUserGroup
            });

            return securities;
        }

        private List<string> GetLanguages()
        {
            ISTATUtils.EndPointElement epe = (ISTATUtils.EndPointElement)HttpContext.Current.Session["WSEndPoint"];

            if (epe.DotStatExportLanguages != "")
                return epe.DotStatExportLanguages.Split(new char[] { ',' }).ToList();

            throw new Exception("No DotStatExportLanguages configured in web.config");
        }

        #endregion


    }
}