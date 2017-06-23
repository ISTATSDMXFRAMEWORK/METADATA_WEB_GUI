using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;

namespace ISTATUtils
{

    public static class SupportedCategorisationArtefacts
    {
        private static NameValueCollection _artefacts;

        public static NameValueCollection Artefacts
        {
            get
            {
                if (_artefacts == null)
                    _artefacts = (NameValueCollection)ConfigurationManager.GetSection("SupportedCategorisationArtefacts");
                return _artefacts;
            }
        }
    }


    public class IRConfiguration
    {
        public static EndPointRetrieverSection Config = ConfigurationManager.GetSection("EndPointSection") as EndPointRetrieverSection;

        public static EndPointElement GetEndPointByName(string endPointName)
        {
            EndPointElement epRet = null;

            foreach (EndPointElement endPointEl in Config.EndPoints)
            {
                if (endPointEl.Name == endPointName)
                    epRet = endPointEl;
            }
            return epRet;
        }
    }

    public class EndPointElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("NSIEndPoint", IsRequired = false)]
        public string NSIEndPoint
        {
            get { return (string)this["NSIEndPoint"]; }
            set { this["NSIEndPoint"] = value; }
        }

        [ConfigurationProperty("RestEndPoint", IsRequired = false)]
        public string RestEndPoint
        {
            get { return (string)this["RestEndPoint"]; }
            set { this["RestEndPoint"] = value; }
        }

        [ConfigurationProperty("ActiveEndPointType", IsRequired = false)]
        public ActiveEndPointType ActiveEndPointType
        {
            get { return (ActiveEndPointType)Enum.Parse(typeof(ActiveEndPointType), this["ActiveEndPointType"].ToString()); }
            set { this["ActiveEndPointType"] = value; }
        }

        [ConfigurationProperty("IREndPoint", IsRequired = false)]
        public string IREndPoint
        {
            get { return (string)this["IREndPoint"]; }
            set { this["IREndPoint"] = value; }
        }

        [ConfigurationProperty("PartialArtefact", IsRequired = false)]
        public bool PartialArtefact
        {
            get { return (bool)this["PartialArtefact"]; }
            set { this["PartialArtefact"] = value; }
        }

        [ConfigurationProperty("EnableAuthentication", IsRequired = false)]
        public bool EnableAuthentication
        {
            get { return (bool)this["EnableAuthentication"]; }
            set { this["EnableAuthentication"] = value; }
        }

        [ConfigurationProperty("EnableAnnotationSuggest", IsRequired = false)]
        public bool EnableAnnotationSuggest
        {
            get { return (bool)this["EnableAnnotationSuggest"]; }
            set { this["EnableAnnotationSuggest"] = value; }
        }

        [ConfigurationProperty("EnableAdministration", IsRequired = false)]
        public bool EnableAdministration
        {
            get { return (bool)this["EnableAdministration"]; }
            set { this["EnableAdministration"] = value; }
        }

        [ConfigurationProperty("GetUsersFromFile", IsRequired = false)]
        public bool GetUsersFromFile
        {
            get { return (bool)this["GetUsersFromFile"]; }
            set { this["GetUsersFromFile"] = value; }
        }

        [ConfigurationProperty("UsersFilePath", IsRequired = false)]
        public string UsersFilePath
        {
            get { return (string)this["UsersFilePath"]; }
            set { this["UsersFilePath"] = value; }
        }

        [ConfigurationProperty("EnableHTTPAuthenication", IsRequired = false)]
        public bool EnableHTTPAuthenication
        {
            get { return (bool)this["EnableHTTPAuthenication"]; }
            set { this["EnableHTTPAuthenication"] = value; }
        }

        [ConfigurationProperty("HTTPDomain", IsRequired = false)]
        public string HTTPDomain
        {
            get { return (string)this["HTTPDomain"]; }
            set { this["HTTPDomain"] = value; }
        }

        [ConfigurationProperty("HTTPUsername", IsRequired = false)]
        public string HTTPUsername
        {
            get { return (string)this["HTTPUsername"]; }
            set { this["HTTPUsername"] = value; }
        }

        [ConfigurationProperty("HTTPPassword", IsRequired = false)]
        public string HTTPPassword
        {
            get { return (string)this["HTTPPassword"]; }
            set { this["HTTPPassword"] = value; }
        }
        /// <summary>
        /// PROXY SECTION
        /// </summary>
        /// 
        [ConfigurationProperty("EnableProxy", IsRequired = false)]
        public bool EnableProxy
        {
            get { return (bool)this["EnableProxy"]; }
            set { this["EnableProxy"] = value; }
        }
        [ConfigurationProperty("UseSystemProxy", IsRequired = false)]
        public bool UseSystemProxy
        {
            get { return (bool)this["UseSystemProxy"]; }
            set { this["UseSystemProxy"] = value; }
        }
        [ConfigurationProperty("ProxyServer", IsRequired = false)]
        public string ProxyServer
        {
            get { return (string)this["ProxyServer"]; }
            set { this["ProxyServer"] = value; }
        }
        [ConfigurationProperty("ProxyServerPort", IsRequired = false)]
        public string ProxyServerPort
        {
            get { return (string)this["ProxyServerPort"]; }
            set { this["ProxyServerPort"] = value; }
        }
        [ConfigurationProperty("ProxyUsername", IsRequired = false)]
        public string ProxyUsername
        {
            get { return (string)this["ProxyUsername"]; }
            set { this["ProxyUsername"] = value; }
        }
        [ConfigurationProperty("ProxyPassword", IsRequired = false)]
        public string ProxyPassword
        {
            get { return (string)this["ProxyPassword"]; }
            set { this["ProxyPassword"] = value; }
        }
        [ConfigurationProperty("EnableCategorisations", IsRequired = false, DefaultValue = true)]
        public bool EnableCategorisations
        {
            get { return (bool)this["EnableCategorisations"]; }
            set { this["EnableCategorisations"] = value; }
        }

        [ConfigurationProperty("DotStatContactName", IsRequired = false)]
        public string DotStatContactName
        {
            get { return (string)this["DotStatContactName"]; }
            set { this["DotStatContactName"] = value; }
        }

        [ConfigurationProperty("DotStatContactDirection", IsRequired = false)]
        public string DotStatContactDirection
        {
            get { return (string)this["DotStatContactDirection"]; }
            set { this["DotStatContactDirection"] = value; }
        }

        [ConfigurationProperty("DotStatContactEMail", IsRequired = false)]
        public string DotStatContactEMail
        {
            get { return (string)this["DotStatContactEMail"]; }
            set { this["DotStatContactEMail"] = value; }
        }

        [ConfigurationProperty("DotStatSecurityUserGroup", IsRequired = false)]
        public string DotStatSecurityUserGroup
        {
            get { return (string)this["DotStatSecurityUserGroup"]; }
            set { this["DotStatSecurityUserGroup"] = value; }
        }

        [ConfigurationProperty("DotStatSecurityDomain", IsRequired = false)]
        public string DotStatSecurityDomain
        {
            get { return (string)this["DotStatSecurityDomain"]; }
            set { this["DotStatSecurityDomain"] = value; }
        }

        [ConfigurationProperty("DotStatExportLanguages", IsRequired = false)]
        public string DotStatExportLanguages
        {
            get { return (string)this["DotStatExportLanguages"]; }
            set { this["DotStatExportLanguages"] = value; }
        }

        [ConfigurationProperty("EnableIREndPoint", IsRequired = false)]
        public bool EnableIREndPoint
        {
            get { return (bool)this["EnableIREndPoint"]; }
            set { this["EnableIREndPoint"] = value; }
        }

        [ConfigurationProperty("EnableArtefactBrowser", IsRequired = false)]
        public bool EnableArtefactBrowser
        {
            get { return (bool)this["EnableArtefactBrowser"]; }
            set { this["EnableArtefactBrowser"] = value; }
        }

        #region // ******* CATEGORY VIEW CONFIGURATION ********

        [ConfigurationProperty("CatViewEnableCategoryDropDownList", IsRequired = false, DefaultValue = true)]
        public bool CatViewEnableCategoryDropDownList
        {
            get { return (bool)this["CatViewEnableCategoryDropDownList"]; }
            set { this["CatViewEnableCategoryDropDownList"] = value; }
        }

        [ConfigurationProperty("CatViewEnableCodelist", IsRequired = false, DefaultValue = true)]
        public bool CatViewEnableCodelist
        {
            get { return (bool)this["CatViewEnableCodelist"]; }
            set { this["CatViewEnableCodelist"] = value; }
        }

        [ConfigurationProperty("CatViewEnableDataFlow", IsRequired = false, DefaultValue = true)]
        public bool CatViewEnableDataFlow
        {
            get { return (bool)this["CatViewEnableDataFlow"]; }
            set { this["CatViewEnableDataFlow"] = value; }
        }

        [ConfigurationProperty("CatViewEnableDsd", IsRequired = false, DefaultValue = true)]
        public bool CatViewEnableDsd
        {
            get { return (bool)this["CatViewEnableDsd"]; }
            set { this["CatViewEnableDsd"] = value; }
        }

        [ConfigurationProperty("CatViewEnableConceptScheme", IsRequired = false, DefaultValue = true)]
        public bool CatViewEnableConceptScheme
        {
            get { return (bool)this["CatViewEnableConceptScheme"]; }
            set { this["CatViewEnableConceptScheme"] = value; }
        }

        [ConfigurationProperty("CatViewDefaultCategoryScheme", IsRequired = false)]
        public string CatViewDefaultCategoryScheme
        {
            get { return this["CatViewDefaultCategoryScheme"].ToString(); }
            set { this["CatViewDefaultCategoryScheme"] = value; }
        }


        #endregion

        [ConfigurationProperty("DefaultLanguageForCombo", IsRequired = false)]
        public string DefaultLanguageForCombo
        {
            get { return this["DefaultLanguageForCombo"].ToString(); }
            set { this["DefaultLanguageForCombo"] = value; }
        }
        

        //[ConfigurationProperty("GetAngenciesFromFile", IsRequired = false)]
        //public bool GetAngenciesFromFile
        //{
        //    get { return (bool)this["GetAngenciesFromFile"]; }
        //    set { this["GetAngenciesFromFile"] = value; }
        //}

        //[ConfigurationProperty("AgenciesFilePath", IsRequired = false)]
        //public string AgenciesFilePath
        //{
        //    get { return (string)this["AgenciesFilePath"]; }
        //    set { this["AgenciesFilePath"] = value; }
        //}

    }

    [ConfigurationCollection(typeof(EndPointElement))]
    public class EndPointCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EndPointElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EndPointElement)element).Name;
        }
    }

    public class EndPointRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("EndPoints", IsDefaultCollection = true)]
        public EndPointCollection EndPoints
        {
            get { return (EndPointCollection)this["EndPoints"]; }
            set { this["EndPoints"] = value; }
        }
    }

    //#region CategoryViewConfigurationSection


    //public class CategoryViewConfigurationSection : ConfigurationSection
    //{
    //    // Create a "wsEndPoint" attribute.
    //    [ConfigurationProperty("wsEndPoint", IsRequired = true)]
    //    public String WsEndPoint
    //    {
    //        get
    //        {
    //            return (String)this["wsEndPoint"];
    //        }
    //        set
    //        {
    //            this["wsEndPoint"] = value;
    //        }
    //    }

    //    // Create a "enableCategoryDropDownList" attribute.
    //    [ConfigurationProperty("enableCategoryDropDownList", DefaultValue = "true", IsRequired = false)]
    //    public Boolean EnableCategoryDropDownList
    //    {
    //        get
    //        {
    //            return (Boolean)this["enableCategoryDropDownList"];
    //        }
    //        set
    //        {
    //            this["enableCategoryDropDownList"] = value;
    //        }
    //    }

    //    // Create a "defaultCategoryScheme" element.
    //    [ConfigurationProperty("defaultCategoryScheme", IsRequired = false)]
    //    public DefaultCategorySchemeElement DefaultCategoryScheme
    //    {
    //        get { return (DefaultCategorySchemeElement)this["defaultCategoryScheme"]; }
    //        set { this["defaultCategoryScheme"] = value; }
    //    }

    //    // Create a "defaultCategoryScheme" element.
    //    [ConfigurationProperty("artefactFilterList", IsRequired = true)]
    //    public ArtefactFilterListElement ArtefactFilterList
    //    {
    //        get { return (ArtefactFilterListElement)this["artefactFilterList"]; }
    //        set { this["artefactFilterList"] = value; }
    //    }

    //}

    //public class DefaultCategorySchemeElement : ConfigurationElement
    //{
    //    [ConfigurationProperty("id", IsRequired = false)]
    //    [StringValidator(InvalidCharacters = "<>",  MaxLength = 255)]
    //    public String Id
    //    {
    //        get
    //        {
    //            return (String)this["id"];
    //        }
    //        set
    //        {
    //            this["id"] = value;
    //        }
    //    }

    //    [ConfigurationProperty("agency", IsRequired = false)]
    //    [StringValidator(InvalidCharacters = "<>",  MaxLength = 255)]
    //    public String Agency
    //    {
    //        get
    //        {
    //            return (String)this["agency"];
    //        }
    //        set
    //        {
    //            this["agency"] = value;
    //        }
    //    }

    //    [ConfigurationProperty("version", IsRequired = false)]
    //    [StringValidator(InvalidCharacters = "<>~!@#$%^&*()[]{}/;'\"|\\ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", MaxLength = 10)]
    //    public String Version
    //    {
    //        get
    //        {
    //            return (String)this["version"];
    //        }
    //        set
    //        {
    //            this["version"] = value;
    //        }
    //    }
    //}


    //public class ArtefactFilterListElement : ConfigurationElement
    //{
    //    [ConfigurationProperty("enableCodelist", IsRequired = true)]
    //    public Boolean EnableCodelist
    //    {
    //        get
    //        {
    //            return (Boolean)this["enableCodelist"];
    //        }
    //        set
    //        {
    //            this["enableCodelist"] = value;
    //        }
    //    }

    //    [ConfigurationProperty("enableDataFlow", IsRequired = true)]
    //    public Boolean EnableDataFlow
    //    {
    //        get
    //        {
    //            return (Boolean)this["enableDataFlow"];
    //        }
    //        set
    //        {
    //            this["enableDataFlow"] = value;
    //        }
    //    }

    //    [ConfigurationProperty("enableDsd", IsRequired = true)]
    //    public Boolean EnableDsd
    //    {
    //        get
    //        {
    //            return (Boolean)this["enableDsd"];
    //        }
    //        set
    //        {
    //            this["enableDsd"] = value;
    //        }
    //    }

    //    [ConfigurationProperty("enableConceptScheme", IsRequired = true)]
    //    public Boolean EnableConceptScheme
    //    {
    //        get
    //        {
    //            return (Boolean)this["enableConceptScheme"];
    //        }
    //        set
    //        {
    //            this["enableConceptScheme"] = value;
    //        }
    //    }


    //}

    //#endregion



}
