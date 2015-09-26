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

namespace ISTATRegistry
{
    public class IOUtils
    {

        #region "Constructors"

        public IOUtils() { }

        #endregion

        #region "Public Methods"

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


        public void SaveCSVFile(DataTable dt, string outputFileName, string separator)
        {
            byte[] bytesInStream = null;

            using (MemoryStream tempStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(tempStream))
                {
                    WriteDataTable(dt, writer, true, separator);
                }

                bytesInStream = tempStream.ToArray();
            }

            SendAttachment(bytesInStream, outputFileName + ".csv");

        }

        public void SaveDotSTATCodelistFile(ICodelistObject codelist)
        {
            CodelistExporter _codeExp = new CodelistExporter(codelist.Id, codelist);
            List<ISTAT.IO.Utility.FileGeneric> files = new List<ISTAT.IO.Utility.FileGeneric>();
            List<ContactRef> contacs = GetConfigContact();
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

            string fileZip = System.Web.HttpContext.Current.Server.MapPath("OutputFiles" + "\\" + ExportFileName + ".zip");

            System.IO.File.Delete(fileZip);
            Ionic.Utils.Zip.ZipFile zip = new Ionic.Utils.Zip.ZipFile(fileZip);
            foreach (ISTAT.IO.Utility.FileGeneric file in files)
                zip.AddFileStream(file.filename, string.Empty, file.stream);
            zip.Save();

            SendAttachment(fileZip, ExportFileName + ".zip");
        }

        public void SaveDotSTATFile(ISdmxObjects sdmxObjects, DotStatExportType exportType)
        {
            string ExportFileName;

            ExportFileName = "DotStatExport-" + sdmxObjects.DataStructures.First().Id + "_" + sdmxObjects.DataStructures.First().AgencyId + "_" + sdmxObjects.DataStructures.First().Version;

            List<ISTAT.IO.Utility.FileGeneric> files = new List<ISTAT.IO.Utility.FileGeneric>();

            List<ContactRef> contacs = GetConfigContact();
            List<SecurityDef> securities = GetConfigSecurity();

            DSDExporter _dsdExp = new DSDExporter(sdmxObjects);

            switch (exportType)
            {
                case DotStatExportType.DSD:
                    if (_dsdExp.CreateData(
                        contacs,
                        securities,
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

                            Stream streamCSV = CSVWriter.CreateStream(_codeExp.DataView);
                            ISTAT.IO.Utility.FileGeneric file_csv = new ISTAT.IO.Utility.FileGeneric();
                            file_csv.filename = _codeExp.DataFilename;
                            file_csv.stream = streamCSV;
                            files.Add(file_csv);
                        }
                    }
                    break;
                case DotStatExportType.ALL:
                    if (_dsdExp.CreateData(
                        contacs,
                        securities,
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

                            Stream streamCSV = CSVWriter.CreateStream(_codeExp.DataView);
                            ISTAT.IO.Utility.FileGeneric file_csv = new ISTAT.IO.Utility.FileGeneric();
                            file_csv.filename = _codeExp.DataFilename;
                            file_csv.stream = streamCSV;
                            files.Add(file_csv);
                        }
                    }
                    break;
            }

            string fileZip = System.Web.HttpContext.Current.Server.MapPath("OutputFiles" + "\\" + ExportFileName + ".zip");

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

        private void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders, string separator )
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
                items = row.ItemArray.Select( o => o.ToString().Replace( "\n", " " ) ).ToArray();
#else
                items = row.ItemArray.Select( o => o.ToString()).ToArray();
#endif
                writer.WriteLine(String.Join(separator, items));
            }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"", value.Replace("\"", "\"\""), "\"");
        }

        private List<ContactRef> GetConfigContact()
        {
            ISTAT.EXPORT.Settings.ContactSettingsHandler configContact =
               (ISTAT.EXPORT.Settings.ContactSettingsHandler)System.Configuration.ConfigurationManager.GetSection(
                   "ExportDotStatSettingsGroup/ContactSection");

            List<ContactRef> contacs = new List<ContactRef>();
            contacs.Add(new ContactRef()
            {
                name = configContact.Name,
                direction = configContact.Direction,
                email = configContact.Email
            });

            return contacs;
        }

        private List<SecurityDef> GetConfigSecurity()
        {
            ISTAT.EXPORT.Settings.SecuritySettingsHandler configSecurity =
               (ISTAT.EXPORT.Settings.SecuritySettingsHandler)System.Configuration.ConfigurationManager.GetSection(
                   "ExportDotStatSettingsGroup/SecuritySection");

            List<SecurityDef> securities = new List<SecurityDef>();
            securities.Add(new SecurityDef()
            {
                domain = configSecurity.Domain,
                userGroup = configSecurity.UserGroup,
            });

            return securities;
        }

        #endregion

    }
}