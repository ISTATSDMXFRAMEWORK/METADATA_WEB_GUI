using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace ISTAT.EXPORT
{
    public class ContactRef
    {
        public string name { get; set; }
        public string direction { get; set; }
        public string email { get; set; }
    }

    public class SecurityDef
    {
        public string userGroup { get; set; }
        public string domain { get; set; }
    }

    public static class LanguageCodes
    {
        private static NameValueCollection _languages;

        public static NameValueCollection Languages
        {
            get
            {
                if (_languages == null)
                    _languages = (NameValueCollection)ConfigurationManager.GetSection("DotStatSupportedLanguages");
                return _languages;
            }
        }
    }

    public class DSDExporter
    {
        Org.Sdmxsource.Sdmx.Api.Model.Objects.ISdmxObjects sdmxObjects;

        System.Xml.XmlDocument _xmlDoc;
        List<ISTAT.EXPORT.CodelistExporter> _codelistExport;
        private DotStatProperties _dotStatProp;

        public System.Xml.XmlDocument XMLDoc { get { return _xmlDoc; } }
        public List<ISTAT.EXPORT.CodelistExporter> ExporterCodelists { get { return _codelistExport; } }

        public DSDExporter(Org.Sdmxsource.Sdmx.Api.Model.Objects.ISdmxObjects sdmxObjects)
        {
            this.sdmxObjects = sdmxObjects;

            _xmlDoc = null;
            _codelistExport = null;
        }

        public DSDExporter(Org.Sdmxsource.Sdmx.Api.Model.Objects.ISdmxObjects sdmxObjects, DotStatProperties dotStatProp)
        {
            _xmlDoc = null;
            _codelistExport = null;

            this.sdmxObjects = sdmxObjects;
            _dotStatProp = dotStatProp;
        }

        public bool CreateData(List<ContactRef> contacsNode, List<SecurityDef> securitiesNode, List<String> langConfig, bool includeCommon = true, bool includeData = false)
        {
            if (this.sdmxObjects == null) return false;

            Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IDataStructureObject _dsd =
                this.sdmxObjects.DataStructures.First();

            System.Xml.XmlDocument xDom = new System.Xml.XmlDocument();

            #region Root node
            System.Xml.XmlElement _rootNode = xDom.CreateElement("OECD.STAT");
            _rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _rootNode.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            _rootNode.SetAttribute("Version", "1.0");
            #endregion

            #region Admin node

            System.Xml.XmlElement _adminNode = xDom.CreateElement("Admin");

            System.Xml.XmlElement _separatorNode = xDom.CreateElement("Separator");
            _separatorNode.InnerText = "|";
            _adminNode.AppendChild(_separatorNode);

            #region Contacts Node

            System.Xml.XmlElement _contactsNode = xDom.CreateElement("Contacts");
            foreach (ContactRef _conRef in contacsNode)
            {
                System.Xml.XmlElement _contactNode = xDom.CreateElement("Contact");
                System.Xml.XmlElement _nameNode = xDom.CreateElement("Name");
                _nameNode.InnerText = _conRef.name;
                System.Xml.XmlElement _directionNode = xDom.CreateElement("Direction");
                _directionNode.InnerText = _conRef.direction;
                System.Xml.XmlElement _emailNode = xDom.CreateElement("E-mail");
                _emailNode.InnerText = _conRef.email;
                System.Xml.XmlElement _languageNode = xDom.CreateElement("Language");
                _languageNode.InnerText = LanguageCodes.Languages["en"];
                _contactNode.AppendChild(_nameNode);
                _contactNode.AppendChild(_directionNode);
                _contactNode.AppendChild(_emailNode);
                _contactNode.AppendChild(_languageNode);
                _contactsNode.AppendChild(_contactNode);
            }
            _adminNode.AppendChild(_contactsNode);

            #endregion;

            #region Secutiry node

            System.Xml.XmlElement _securityNode = xDom.CreateElement("Security");

            foreach (SecurityDef _secRef in securitiesNode)
            {


                System.Xml.XmlElement _membershipNode = xDom.CreateElement("Membership");
                System.Xml.XmlElement _userGroupNode = xDom.CreateElement("UserGroup");
                _userGroupNode.InnerText = _secRef.userGroup;
                System.Xml.XmlElement _domainNode = xDom.CreateElement("Domain");
                _domainNode.InnerText = _secRef.domain;
                _membershipNode.AppendChild(_userGroupNode);
                _membershipNode.AppendChild(_domainNode);
                _securityNode.AppendChild(_membershipNode);

            }
            _adminNode.AppendChild(_securityNode);

            #endregion

            #endregion

            #region Dataset node
            System.Xml.XmlElement _datasetNode = xDom.CreateElement("Dataset");

            System.Xml.XmlElement datasetNameNode;
            System.Xml.XmlElement datasetNameLanguageNode;
            System.Xml.XmlElement datasetNameValueNode;
            ITextTypeWrapper itt;
            string nameValue;

            List<System.Xml.XmlElement> lLanguages = new List<XmlElement>();
            System.Xml.XmlElement language;
            System.Xml.XmlAttribute attrLanguageCode;
            System.Xml.XmlAttribute attrOrder;

            int orderCounter = 1;

            foreach (string lang in langConfig)
            {
                datasetNameNode = xDom.CreateElement("DatasetName");
                datasetNameLanguageNode = xDom.CreateElement("Language");
                datasetNameValueNode = xDom.CreateElement("Value");

                datasetNameLanguageNode.InnerText = LanguageCodes.Languages[lang];

                itt = _dsd.Names.Where(n => n.Locale.ToLower() == lang.ToLower()).FirstOrDefault();

                nameValue = itt == null ? "" : itt.Value;

                datasetNameValueNode.AppendChild(xDom.CreateCDataSection(nameValue));

                datasetNameNode.AppendChild(datasetNameLanguageNode);
                datasetNameNode.AppendChild(datasetNameValueNode);

                _datasetNode.AppendChild(datasetNameNode);
            }

            System.Xml.XmlElement _codeNode = xDom.CreateElement("Code");
            _codeNode.InnerText = _dsd.Id;
            _datasetNode.AppendChild(_codeNode);

            System.Xml.XmlElement languages = xDom.CreateElement("Languages");

            foreach (string lang in langConfig)
            {
                language = xDom.CreateElement("Language");

                attrLanguageCode = xDom.CreateAttribute("LanguageCode");
                attrOrder = xDom.CreateAttribute("Order");

                attrLanguageCode.InnerText = lang;
                attrOrder.InnerText = orderCounter.ToString();

                language.Attributes.Append(attrLanguageCode);
                language.Attributes.Append(attrOrder);

                languages.AppendChild(language);

                ++orderCounter;
            }

            _datasetNode.AppendChild(languages);

            System.Xml.XmlElement _actionNode = xDom.CreateElement("Action");
            _actionNode.InnerText = "CREATE";
            _datasetNode.AppendChild(_actionNode);

            System.Xml.XmlElement _updatequeriesNode = xDom.CreateElement("UpdateQueries");
            _updatequeriesNode.InnerText = "true";
            _datasetNode.AppendChild(_updatequeriesNode);

            System.Xml.XmlElement _themeNode = xDom.CreateElement("Theme");
            _themeNode.InnerText = "General Statistics|General Statistics";
            _datasetNode.AppendChild(_themeNode);

            System.Xml.XmlElement _directorateNode = xDom.CreateElement("Directorate");
            _directorateNode.InnerText = "STD";
            _datasetNode.AppendChild(_directorateNode);

            #endregion

            #region Dimensions node

            _codelistExport = new List<CodelistExporter>();

            System.Xml.XmlElement _dimsNode = xDom.CreateElement("Dimensions");

            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IDimension dim in _dsd.DimensionList.Dimensions)
            {
                if (dim.HasCodedRepresentation())
                {
                    ISet<Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject> codelists =
                        sdmxObjects.GetCodelists(
                        new Org.Sdmxsource.Sdmx.Util.Objects.Reference.MaintainableRefObjectImpl()
                        {
                            AgencyId = dim.Representation.Representation.MaintainableReference.AgencyId,
                            MaintainableId = dim.Representation.Representation.MaintainableReference.MaintainableId,
                            Version = dim.Representation.Representation.MaintainableReference.Version
                        }
                        );
                    if (codelists != null && codelists.Count > 0)
                    {
                        ISTAT.EXPORT.CodelistExporter exporter = new CodelistExporter(dim.Id, codelists.First(), _dotStatProp, langConfig);
                        exporter.CreateData(contacsNode);
                        _codelistExport.Add(exporter);
                        _dimsNode.AppendChild(exporter.Get_DimensionNode(xDom, codelists.First(), includeCommon, includeData));
                    }
                }
            }

            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IAttributeObject att in _dsd.Attributes)
            {
                if (att.HasCodedRepresentation())
                {
                    ISet<Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject> codelists =
                        sdmxObjects.GetCodelists(
                        new Org.Sdmxsource.Sdmx.Util.Objects.Reference.MaintainableRefObjectImpl()
                        {
                            AgencyId = att.Representation.Representation.MaintainableReference.AgencyId,
                            MaintainableId = att.Representation.Representation.MaintainableReference.MaintainableId,
                            Version = att.Representation.Representation.MaintainableReference.Version
                        }
                        );
                    if (codelists != null && codelists.Count > 0)
                    {
                        ISTAT.EXPORT.CodelistExporter exporter = new CodelistExporter(att.Id, codelists.First(), _dotStatProp, langConfig);
                        exporter.CreateData(contacsNode);
                        _codelistExport.Add(exporter);
                        _dimsNode.AppendChild(exporter.Get_DimensionNode(xDom, codelists.First(), includeCommon, includeData));
                    }
                }
            }

            System.Xml.XmlElement _dimensionCountNode = xDom.CreateElement("DimensionCount");
            _dimensionCountNode.InnerText = _codelistExport.Count.ToString();
            _datasetNode.AppendChild(_dimensionCountNode);

            _datasetNode.AppendChild(_dimsNode);

            #endregion

            _rootNode.AppendChild(_adminNode);
            _rootNode.AppendChild(_datasetNode);

            xDom.AppendChild(_rootNode);
            _xmlDoc = xDom;

            return true;

        }

    }

    public class CodelistExporter
    {

        Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject _codelist;
        string _code;
        List<string> _languages;

        System.Xml.XmlDocument _xmlDoc;
        string _dataFilename;
        string _dataFilenameCsv;
        List<string[]> _dataView;
        List<string[]> _dataViewcsv;

        private Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject codelist;
        private DotStatProperties _dotStatProp;


        public System.Xml.XmlDocument XMLDoc { get { return _xmlDoc; } }
        public string Code { get { return _code; } }
        public string DataFilename { get { return _dataFilename; } }
        public string DataFilenameCsv { get { return _dataFilenameCsv; } }
        public List<string[]> DataView { get { return _dataView; } }
        public List<string[]> DataViewCsv { get { return _dataViewcsv; } }

        public CodelistExporter(string code, Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject codelist)
        {
            _codelist = codelist;
            _code = code;
            _xmlDoc = null;
            _dataFilename = string.Empty;
            _dataView = null;
            _dataViewcsv = null;
        }

        public CodelistExporter(string code, Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject codelist, DotStatProperties dsp, List<String> languages)
        {
            _codelist = codelist;
            _code = code;
            _xmlDoc = null;
            _dataFilename = string.Empty;
            _dataView = null;
            _dataViewcsv = null;
            _languages = languages;
            _dotStatProp = dsp;
        }

        public bool CreateData(List<ContactRef> contacsNode)
        {
            if (_codelist == null) return false;

            System.Xml.XmlDocument xDom = new System.Xml.XmlDocument();

            #region Root node
            System.Xml.XmlElement _rootNode = xDom.CreateElement("OECD.STAT");
            _rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _rootNode.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            _rootNode.SetAttribute("Version", "1.0");
            #endregion

            #region Admin node

            System.Xml.XmlElement _adminNode = xDom.CreateElement("Admin");

            System.Xml.XmlElement _separatorNode = xDom.CreateElement("Separator");
            _separatorNode.InnerText = "|";
            _adminNode.AppendChild(_separatorNode);

            #region Contacts Node

            System.Xml.XmlElement _contactsNode = xDom.CreateElement("Contacts");
            foreach (ContactRef _conRef in contacsNode)
            {
                System.Xml.XmlElement _contactNode = xDom.CreateElement("Contact");
                System.Xml.XmlElement _nameNode = xDom.CreateElement("Name");
                _nameNode.InnerText = _conRef.name;
                System.Xml.XmlElement _directionNode = xDom.CreateElement("Direction");
                _directionNode.InnerText = _conRef.direction;
                System.Xml.XmlElement _emailNode = xDom.CreateElement("E-mail");
                _emailNode.InnerText = _conRef.email;
                System.Xml.XmlElement _languageNode = xDom.CreateElement("Language");
                _languageNode.InnerText = LanguageCodes.Languages["en"];
                _contactNode.AppendChild(_nameNode);
                _contactNode.AppendChild(_directionNode);
                _contactNode.AppendChild(_emailNode);
                _contactNode.AppendChild(_languageNode);
                _contactsNode.AppendChild(_contactNode);
            }

            _adminNode.AppendChild(_contactsNode);

            #endregion;

            #endregion

            #region Dataset node
            System.Xml.XmlElement _datasetNode = xDom.CreateElement("Dataset");

            System.Xml.XmlElement datasetNameNode;
            System.Xml.XmlElement datasetNameLanguageNode;
            System.Xml.XmlElement datasetNameValueNode;

            //ITextTypeWrapper itt;
            //string nameValue;

            List<System.Xml.XmlElement> lLanguages = new List<XmlElement>();
            System.Xml.XmlElement language;
            System.Xml.XmlAttribute attrLanguageCode;
            System.Xml.XmlAttribute attrOrder;

            int orderCounter = 1;


            foreach (string lang in _languages)
            {
                datasetNameNode = xDom.CreateElement("DatasetName");
                datasetNameLanguageNode = xDom.CreateElement("Language");
                datasetNameValueNode = xDom.CreateElement("Value");
                language = xDom.CreateElement("Language");

                datasetNameLanguageNode.InnerText = LanguageCodes.Languages[lang];

                datasetNameValueNode.AppendChild(xDom.CreateCDataSection("common dimension"));

                datasetNameNode.AppendChild(datasetNameLanguageNode);
                datasetNameNode.AppendChild(datasetNameValueNode);

                _datasetNode.AppendChild(datasetNameNode);
            }

            System.Xml.XmlElement _codeNode = xDom.CreateElement("Code");
            //_codeNode.InnerText = _codelist.Id;
            _codeNode.InnerText = "commondimension";
            _datasetNode.AppendChild(_codeNode);

            System.Xml.XmlElement languages = xDom.CreateElement("Languages");

            foreach (string lang in _languages)
            {
                language = xDom.CreateElement("Language");

                attrLanguageCode = xDom.CreateAttribute("LanguageCode");
                attrOrder = xDom.CreateAttribute("Order");

                attrLanguageCode.InnerText = lang;
                attrOrder.InnerText = orderCounter.ToString();

                language.Attributes.Append(attrLanguageCode);
                language.Attributes.Append(attrOrder);

                languages.AppendChild(language);

                ++orderCounter;
            }


            _datasetNode.AppendChild(languages);

            System.Xml.XmlElement _actionNode = xDom.CreateElement("Action");
            _actionNode.InnerText = "CREATE";
            _datasetNode.AppendChild(_actionNode);

            System.Xml.XmlElement _dimensionCountNode = xDom.CreateElement("DimensionCount");
            _dimensionCountNode.InnerText = "1";
            _datasetNode.AppendChild(_dimensionCountNode);
            #endregion

            #region Dimension node


            System.Xml.XmlElement _dimsNode = xDom.CreateElement("Dimensions");
            _dimsNode.AppendChild(Get_DimensionNode(xDom, _codelist, true, true));
            _datasetNode.AppendChild(_dimsNode);

            #endregion

            System.Xml.XmlElement _inlineNode = xDom.CreateElement("Inline");
            _inlineNode.InnerText = "false";
            _datasetNode.AppendChild(_inlineNode);

            System.Xml.XmlElement _microDataNode = xDom.CreateElement("MicroInfoDataCount");
            _microDataNode.InnerText = "0";
            _datasetNode.AppendChild(_microDataNode);

            System.Xml.XmlElement _microTable = xDom.CreateElement("MicroTableDataCount");
            _microTable.InnerText = "0";
            _datasetNode.AppendChild(_microTable);

            _rootNode.AppendChild(_adminNode);
            _rootNode.AppendChild(_datasetNode);

            xDom.AppendChild(_rootNode);

            _xmlDoc = xDom;

            return true;

        }

        public XmlElement Get_DimensionNode(XmlDocument xDom, Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject codelist, bool includeCommon, bool includeData)
        {

            System.Xml.XmlElement _dimNode = xDom.CreateElement("Dimension");

            System.Xml.XmlElement dimNameNode;
            System.Xml.XmlElement dimNameLanguageNode;
            System.Xml.XmlElement dimNameValueNode;

            ITextTypeWrapper itt;
            string nameValue;

            foreach (string lang in _languages)
            {
                dimNameNode = xDom.CreateElement("DimensionName");
                dimNameLanguageNode = xDom.CreateElement("Language");
                dimNameValueNode = xDom.CreateElement("Value");

                dimNameLanguageNode.InnerText = LanguageCodes.Languages[lang];

                itt = codelist.Names.Where(n => n.Locale.ToLower() == lang.ToLower()).FirstOrDefault();

                nameValue = itt == null ? "" : itt.Value;

                dimNameValueNode.AppendChild(xDom.CreateCDataSection(nameValue));

                dimNameNode.AppendChild(dimNameLanguageNode);
                dimNameNode.AppendChild(dimNameValueNode);

                _dimNode.AppendChild(dimNameNode);
            }

            System.Xml.XmlElement _dimcodeNode = xDom.CreateElement("Code");
            _dimcodeNode.InnerText = _code;
            _dimNode.AppendChild(_dimcodeNode);

            if (includeCommon)
            {
                System.Xml.XmlElement _dimcommonNode = xDom.CreateElement("Common");
                _dimcommonNode.InnerText = _code;
                _dimNode.AppendChild(_dimcommonNode);
            }

            if (includeData)
            {
                System.Xml.XmlElement _datastructureNode = xDom.CreateElement("DataStructure");
                System.Xml.XmlElement _key = xDom.CreateElement("Key");
                _key.InnerText = "1";

                System.Xml.XmlElement _nameNode_en = xDom.CreateElement("Names");
                _nameNode_en.InnerText = "2";

                System.Xml.XmlElement parentKeyNode = xDom.CreateElement("KeyParent");
                parentKeyNode.InnerText = (2 + _languages.Count()).ToString();

                System.Xml.XmlElement _orderNode = xDom.CreateElement("Order");
                _orderNode.InnerText = (3 + _languages.Count()).ToString();

                _datastructureNode.AppendChild(_key);
                _datastructureNode.AppendChild(parentKeyNode);
                _datastructureNode.AppendChild(_nameNode_en);
                _datastructureNode.AppendChild(_orderNode);
                _dimNode.AppendChild(_datastructureNode);

                // CSV LOC ///////
                string dataFilename = _code + ".csv_loc";
                List<string[]> dataView = new List<string[]>();
                int order = 10;
                foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICode code in codelist.Items)
                {
                    string[] dataRow ={
                                      code.Id,
                                      GetXMLNames(code.Names),
                                      String.IsNullOrEmpty(code.ParentCode) ? code.Id : code.ParentCode,
                                      order.ToString()
                                      };

                    if (code.ParentCode != null)
                        System.Diagnostics.Debug.Print(code.ParentCode);

                    dataView.Add(dataRow);
                    order += 10;

                }
                // CSV //////////
                string dataFilenameCsv = _code + ".csv";
                List<string[]> dataViewCsv = new List<string[]>();
                int orderCsv = 10;
                foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICode code in codelist.Items)
                {
                    string[] dataRowCsv ={
                                             code.Id
                                         };

                    foreach (string lang in _languages)
                    {
                        itt = code.Names.Where(n => n.Locale.ToLower() == lang.ToLower()).FirstOrDefault();
                        nameValue = itt == null ? "" : itt.Value;
                        dataRowCsv = new List<string>(dataRowCsv) { nameValue }.ToArray();
                    }

                    dataRowCsv = new List<string>(dataRowCsv) { 
                                             String.IsNullOrEmpty(code.ParentCode) ? code.Id : code.ParentCode,
                                            orderCsv.ToString() }.ToArray();

                    dataViewCsv.Add(dataRowCsv);
                    orderCsv += 10;
                }
                //////////////


                System.Xml.XmlElement _dimensionData = xDom.CreateElement("DimensionData");

                string dotStatPath;

                if (!string.IsNullOrEmpty(_dotStatProp.Server) && !string.IsNullOrEmpty(_dotStatProp.Directory) && !string.IsNullOrEmpty(_dotStatProp.Theme))
                {
                    dotStatPath = string.Format(@"\\{0}\data\DWEntryGate\{1} - {2}\CSVFILES\{3}", _dotStatProp.Server, _dotStatProp.Directory, _dotStatProp.Theme, dataFilename);
                }
                else
                {
                    dotStatPath = ".//CSVFILES//" + dataFilename;
                }

                _dimensionData.AppendChild(xDom.CreateCDataSection(dotStatPath));
                _dimNode.AppendChild(_dimensionData);

                System.Xml.XmlElement _dimensionDataCount = xDom.CreateElement("DimensionDataCount");
                _dimensionDataCount.InnerText = codelist.Items.Count.ToString();
                _dimNode.AppendChild(_dimensionDataCount);


                _dataFilename = dataFilename;
                _dataView = dataView;

                _dataFilenameCsv = dataFilenameCsv;
                _dataViewcsv = dataViewCsv;

            }
            return _dimNode;
        }

        private string GetXMLNames(IList<ITextTypeWrapper> lNames)
        {
            System.Xml.XmlDocument xDom = new System.Xml.XmlDocument();
            string sXML = "<translations xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"></translations>";

            xDom.LoadXml(sXML);

            XmlElement root = xDom.DocumentElement;
            XmlElement translation = xDom.DocumentElement;
            XmlAttribute langCode, text;

            ITextTypeWrapper itt;
            string nameValue;

            foreach (string lang in _languages)
            {
                translation = xDom.CreateElement("translation");
                langCode = xDom.CreateAttribute("LanguageCode");
                text = xDom.CreateAttribute("Text");

                langCode.InnerText = lang;

                itt = lNames.Where(n => n.Locale.ToLower() == lang.ToLower()).FirstOrDefault();

                nameValue = itt == null ? "" : itt.Value;

                text.InnerText = nameValue;

                translation.Attributes.Append(langCode);
                translation.Attributes.Append(text);

                root.AppendChild(translation);
            }

            return xDom.InnerXml;
        }

    }

}
