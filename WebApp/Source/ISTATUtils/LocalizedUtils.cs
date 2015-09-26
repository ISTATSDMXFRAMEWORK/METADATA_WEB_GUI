using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

namespace ISTATUtils
{
    public class LocalizedUtils
    {
        private System.Globalization.CultureInfo _language;
        private const string _defaultLanguage = "en";

        public string Language { get { return _language.TwoLetterISOLanguageName; } }

        public LocalizedUtils(System.Globalization.CultureInfo language)
        {
            _language = language;
        }

        public string GetNameableName(INameableObject nameableObject)
        {
            string DefaultName="",LocalizedName="";

            if(nameableObject.Names == null)
                return "";

            foreach (ITextTypeWrapper name in nameableObject.Names)
            {
                if (name.Locale == Language)
                    LocalizedName = name.Value;

                if (name.Locale == _defaultLanguage)
                    DefaultName = name.Value;
            }

            //return (LocalizedName != "" ? LocalizedName : DefaultName);
            return (LocalizedName != "" ? LocalizedName : string.Empty);
        }

        public string GetNameableDescription(INameableObject nameableObject)
        {
            string DefaultDescription = "", LocalizedName = "";

            if (nameableObject.Names == null)
                return "";

            foreach (ITextTypeWrapper description in nameableObject.Descriptions)
            {
                if (description.Locale == Language)
                    LocalizedName = description.Value;

                if (description.Locale == _defaultLanguage)
                    DefaultDescription = description.Value;
            }

            //return (LocalizedName != "" ? LocalizedName : DefaultDescription);
            return (LocalizedName != "" ? LocalizedName : string.Empty);
        }

        public string GetLocalizedText(IList<Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base.ITextTypeWrapperMutableObject> lText)
        {
            string DefaultDescription = "", LocalizedName = "";

            if (lText == null)
                return "";

            foreach (Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base.ITextTypeWrapperMutableObject text in lText)
            {
                if (text.Locale == Language)
                    LocalizedName = text.Value;

                if (text.Locale == _defaultLanguage)
                    DefaultDescription = text.Value;
            }

            //return (LocalizedName != "" ? LocalizedName : DefaultDescription);
            return (LocalizedName != "" ? LocalizedName : string.Empty);
        }
    }
}