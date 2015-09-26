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
}
