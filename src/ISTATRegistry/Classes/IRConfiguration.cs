using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ISTATRegistry
{
    public class IRConfiguration
    {
        public static EndPointRetrieverSection Config = ConfigurationManager.GetSection("EndPointSection") as EndPointRetrieverSection;

        //public static CategoryViewConfigurationRetriever Config = ConfigurationManager.GetSection("EndPointSection") as EndPointRetrieverSection;

        //CategoryViewConfiguration

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

        [ConfigurationProperty("NSIEndPoint", IsRequired = true)]
        public string NSIEndPoint
        {
            get { return (string)this["NSIEndPoint"]; }
            set { this["NSIEndPoint"] = value; }
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

    #region CategoryViewConfigurationSection


    public class CategoryViewConfigurationSection : ConfigurationSection
    {
        // Create a "wsEndPoint" attribute.
        [ConfigurationProperty("wsEndPoint", IsRequired = true)]
        public String WsEndPoint
        {
            get
            {
                return (String)this["wsEndPoint"];
            }
            set
            {
                this["wsEndPoint"] = value;
            }
        }

        // Create a "enableCategoryDropDownList" attribute.
        [ConfigurationProperty("enableCategoryDropDownList", DefaultValue = "true", IsRequired = false)]
        public Boolean EnableCategoryDropDownList
        {
            get
            {
                return (Boolean)this["enableCategoryDropDownList"];
            }
            set
            {
                this["enableCategoryDropDownList"] = value;
            }
        }

        // Create a "defaultCategoryScheme" element.
        [ConfigurationProperty("defaultCategoryScheme", IsRequired = false)]
        public DefaultCategorySchemeElement DefaultCategoryScheme
        {
            get { return (DefaultCategorySchemeElement)this["defaultCategoryScheme"]; }
            set { this["defaultCategoryScheme"] = value; }
        }

        // Create a "defaultCategoryScheme" element.
        [ConfigurationProperty("artefactFilterList", IsRequired = true)]
        public ArtefactFilterListElement ArtefactFilterList
        {
            get { return (ArtefactFilterListElement)this["artefactFilterList"]; }
            set { this["artefactFilterList"] = value; }
        }

    }

    public class DefaultCategorySchemeElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = false)]
        [StringValidator(InvalidCharacters = "<>",  MaxLength = 255)]
        public String Id
        {
            get
            {
                return (String)this["id"];
            }
            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("agency", IsRequired = false)]
        [StringValidator(InvalidCharacters = "<>",  MaxLength = 255)]
        public String Agency
        {
            get
            {
                return (String)this["agency"];
            }
            set
            {
                this["agency"] = value;
            }
        }

        [ConfigurationProperty("version", IsRequired = false)]
        [StringValidator(InvalidCharacters = "<>~!@#$%^&*()[]{}/;'\"|\\ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", MaxLength = 10)]
        public String Version
        {
            get
            {
                return (String)this["version"];
            }
            set
            {
                this["version"] = value;
            }
        }
    }


    public class ArtefactFilterListElement : ConfigurationElement
    {
        [ConfigurationProperty("enableCodelist", IsRequired = true)]
        public Boolean EnableCodelist
        {
            get
            {
                return (Boolean)this["enableCodelist"];
            }
            set
            {
                this["enableCodelist"] = value;
            }
        }

        [ConfigurationProperty("enableDataFlow", IsRequired = true)]
        public Boolean EnableDataFlow
        {
            get
            {
                return (Boolean)this["enableDataFlow"];
            }
            set
            {
                this["enableDataFlow"] = value;
            }
        }

        [ConfigurationProperty("enableDsd", IsRequired = true)]
        public Boolean EnableDsd
        {
            get
            {
                return (Boolean)this["enableDsd"];
            }
            set
            {
                this["enableDsd"] = value;
            }
        }

        [ConfigurationProperty("enableConceptScheme", IsRequired = true)]
        public Boolean EnableConceptScheme
        {
            get
            {
                return (Boolean)this["enableConceptScheme"];
            }
            set
            {
                this["enableConceptScheme"] = value;
            }
        }


    }

    #endregion



}
