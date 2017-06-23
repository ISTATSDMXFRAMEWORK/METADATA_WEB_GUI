using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ISTAT.WSDAL
{
    public class FixXmlDoc
    {
        private XNamespace reg { get; set; }
        private XNamespace mes { get; set; }
        private XNamespace str { get; set; }
        private XNamespace com { get; set; }

        public FixXmlDoc()
        {
            reg = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/webservices/registry";
            mes = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message";
            str = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/structure";
            com = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/common";
        }

        public XmlDocument FixLocale(XmlDocument XmlDoc, string oldValue, string newValue)
        {
            string Find = String.Format(@"lang=""{0}""", oldValue);
            string Repl = String.Format(@"lang=""{0}""", newValue);

            XmlDoc.InnerXml = XmlDoc.InnerXml.Replace(Find, Repl);

            return XmlDoc;
        }

        public XmlDocument FixDataType(XmlDocument XmlDoc)
        {
            XDocument XDoc = ToXDocument(XmlDoc);

            var EnumFormats = XDoc.Descendants(str + "EnumerationFormat");

            if (EnumFormats.Count() > 0)
            {
                foreach (var EnumFormat in EnumFormats)
                {
                    if (EnumFormat.HasAttributes)
                    {
                        var pattern = EnumFormat.Attribute("pattern");
                        var textType = EnumFormat.Attribute("textType");

                        if (pattern != null && textType != null)
                            textType.Value = "String";
                    }
                }
            }

            return ToXmlDocument(XDoc);
        }

        private XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        private XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
