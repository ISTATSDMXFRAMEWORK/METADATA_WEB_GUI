using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlyCallWS;
using System.Configuration;
using System.Xml;
using System.Text;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Api.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Structureparser.Manager;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using System.IO;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using System.Collections.Specialized;

namespace ISTAT.WSDAL
{
    public class WSUtils
    {
        #region "Private Properties"
        
        private WsConfigurationSettings _wsConfigSettings = null;

        #endregion

        #region "Constructors"

        public WSUtils()
        { }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Convert a XMLDocument to Bytes Array
        /// </summary>
        /// <param name="doc">XMLDocument to convert</param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(XmlDocument doc)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] docAsBytes = encoding.GetBytes(doc.OuterXml);
            return docAsBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public WsConfigurationSettings GetSettings(string operation)
        {

            NameValueCollection endPointSection = (NameValueCollection)ConfigurationManager.GetSection("EndPoints");

            WsConfigurationSettings wsConfigSettings = new WsConfigurationSettings()
            {
                //WebService
                RestEndPoint = "",
                Prefix = ConfigurationManager.AppSettings["Prefix"],


                //Credenziali Http
                EnableHTTPAuthenication = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableHTTPAuthenication"]),
                Domain = ConfigurationManager.AppSettings["Domain"],
                Username = ConfigurationManager.AppSettings["Username"],
                Password = ConfigurationManager.AppSettings["Password"],

                //Proxy
                EnableProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProxy"]),
                UseSystemProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSystemProxy"]),
                ProxyServer = ConfigurationManager.AppSettings["ProxyServer"],
                ProxyServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ProxyServerPort"]),
                ProxyUsername = ConfigurationManager.AppSettings["ProxyUsername"],
                ProxyPassword = ConfigurationManager.AppSettings["ProxyPassword"],

                EndPoint = HttpContext.Current.Session["WSEndPoint"].ToString(),
                WSDL = HttpContext.Current.Session["WSEndPoint"].ToString() + "?wsdl",

                //EndPoint = WSConstants.wsEndPoint,
                //WSDL = WSConstants.wsEndPoint + "?wsdl",
                Operation = operation
            };

            return wsConfigSettings;
        }

        public XmlDocument GetXMLDocFromSdmxObjects(ISdmxObjects sdmxObjects, StructureOutputFormatEnumType version)
        {
            StructureWriterManager swm = new StructureWriterManager();

            StructureOutputFormat soFormat = StructureOutputFormat.GetFromEnum(version);
            IStructureFormat outputFormat = new SdmxStructureFormat(soFormat);

            MemoryStream memoryStream = new MemoryStream();

            swm.WriteStructures(sdmxObjects, outputFormat, memoryStream);

            XmlTextReader read = new XmlTextReader(memoryStream);

            memoryStream.Flush();
            memoryStream.Position = 0;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(memoryStream);

            return xDoc;
        }

        public ISdmxObjects GetSdmxObjectsFromXML(XmlDocument xDocStructure)
        {
            Org.Sdmxsource.Sdmx.Api.Util.IReadableDataLocation rdl = new Org.Sdmxsource.Util.Io.XmlDocReadableDataLocation(xDocStructure);
            StructureParsingManager spm = new StructureParsingManager();
            IStructureWorkspace workspace = spm.ParseStructures(rdl);
            ISdmxObjects sdmxObjects = workspace.GetStructureObjects(false);

            return sdmxObjects;
        }

        public XmlDocument GetXmlMessage(XmlDocument xDocStructure)
        {
            return GetXmlMessage(GetSdmxObjectsFromXML(xDocStructure));
        }

        public XmlDocument GetXmlMessage(ISdmxObjects sdmxObjectsStructure)
        {
            WSModel wsModel = new WSModel();
            XmlDocument xDocTemplate = new XmlDocument();
            XmlDocument xDocStructure = new XmlDocument();

            // lo converto in un xml sdmx 2.1
            xDocStructure = GetXMLDocFromSdmxObjects(sdmxObjectsStructure, Org.Sdmxsource.Sdmx.Api.Constants.StructureOutputFormatEnumType.SdmxV21StructureDocument);

            //Carico il template
            xDocTemplate = new XmlDocument();
            xDocTemplate.Load(HttpContext.Current.Server.MapPath(@".\SdmxQueryTemplate\SubmitStructureReplace.xml"));

            // Il nodo root "Structure" del template
            XmlNode xTempStructNode = xDocTemplate.SelectSingleNode("//*[local-name()='Structures']");

            // Aggiungo al template lo structure da inserire
            xTempStructNode.InnerXml = xDocStructure.SelectSingleNode("//*[local-name()='Structures']").InnerXml;

            return xDocTemplate;
        }

        #endregion
    }
}