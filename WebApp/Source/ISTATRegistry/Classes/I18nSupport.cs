namespace SDMX_Dataloader.Main.Web
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Configuration;

    using Resources;

    /// <summary>
    /// A singleton that allows to get the available locale
    /// </summary>
    /// 
    internal class I18NSupport
    {
        #region Constants and Fields

        /// <summary>
        /// The I18nSupport singleton instance
        /// </summary>
        private static readonly I18NSupport _instance = new I18NSupport();

        /// <summary>
        /// A Set holding the available locale
        /// </summary>
        private readonly IDictionary<CultureInfo, object> _availableLocale = new Dictionary<CultureInfo, object>();

        /// <summary>
        /// A Map holding the available locale index
        /// </summary>
        private readonly IDictionary<string, CultureInfo> _availableLocaleIndex = new Dictionary<string, CultureInfo>();

        /// <summary>
        /// The configured culture info
        /// </summary>
        private readonly CultureInfo _configuredCulture = CultureInfo.CurrentUICulture;

        /// <summary>
        /// A dictionary collection for the existing locale
        /// </summary>
        private readonly Dictionary<string, CultureInfo> _systemAvailableCulture = new Dictionary<string, CultureInfo>();


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="I18NSupport"/> class from being created. 
        /// Initialize a new instance of the I18nSupport class. 
        /// It is set to private to disable creating this class using this constructor
        /// </summary>
        private I18NSupport()
        {
/*
            this.RetrieveSystemAvailableCultures();

            this.AddLocale("en");

            // add locale from Messages.*.resx
            this.RetrieveAvailableLocales();

            // Add locale from application settings in web.config
            this.RetrieveConfiguredLocales();

            // add configured default
            if (!this._availableLocale.ContainsKey(this._configuredCulture))
            {
                this._availableLocale.Add(this._configuredCulture, null);
                this._availableLocaleIndex.Add(this._configuredCulture.Name, this._configuredCulture);
            } 
 */
            this.RetrieveSystemAvailableCultures();

            // add application default
            this.AddLocale("it");

            // add configured default
            if (!this._availableLocale.ContainsKey(this._configuredCulture))
            {
                this._availableLocale.Add(this._configuredCulture, null);
                this._availableLocaleIndex.Add(this._configuredCulture.Name, this._configuredCulture);
            }

            // add locale from Messages.*.resx
            this.RetrieveAvailableLocales();

            // Add locale from application settings in web.config
            this.RetrieveConfiguredLocales();
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the singleton instance of I18nSupport
        /// </summary>
        public static I18NSupport Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Gets the list of all available UI locale
        /// </summary>
        public IDictionary<CultureInfo, object> AvailableLocales
        {
            get
            {
                return this._availableLocale;
            }
        }

        public Dictionary<string, CultureInfo> SystemAvailableLocales
        {
            get
            {
                return this._systemAvailableCulture;
            }
        }

        /// <summary>
        /// Gets the configured default locale
        /// </summary>
        public CultureInfo DefaultLocale
        {
            get
            {
                return this._configuredCulture;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the correspoding <see cref="CultureInfo"/> from the available locale using the specified locale identifier
        /// </summary>
        /// <param name="locale">
        /// The locale identifier
        /// </param>
        /// <returns>
        /// The  correspoding <see cref="CultureInfo"/> or null if there is no correspoding locale available
        /// </returns>
        public CultureInfo GetLocale(string locale)
        {
            if (locale == null)
            {
                return null;
            }

            CultureInfo cu;
            return this._availableLocaleIndex.TryGetValue(locale, out cu) ? cu : null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a locale to the Dictionary
        /// </summary>
        /// <param name="locale">
        /// The locale to add
        /// </param>
        private void AddLocale(string locale)
        {
            if (string.IsNullOrEmpty(locale))
            {
                return;
            }

            CultureInfo c;
            if (!this._systemAvailableCulture.TryGetValue(locale, out c) || c == null)
            {
                return;
            }

            if (!this._availableLocale.ContainsKey(c))
            {

                this._availableLocale.Add(c, null);
                this._availableLocaleIndex.Add(c.Name, c);
            }
        }

        /// <summary>
        /// Retrieve all available cultures by checking the available messages locale
        /// </summary>
        private void RetrieveAvailableLocales()
        {
            string baseName = Messages.ResourceManager.BaseName;
            baseName = baseName.Substring(baseName.LastIndexOf('.') + 1);

            Regex getRegion = new Regex(baseName + "\\.(?<locale>(\\w|-)+)\\.resx");

            string path = HttpContext.Current.Server.MapPath("~/") + "App_GlobalResources";
            string[] files = Directory.GetFiles(path, baseName + ".*.resx");

            foreach (string file in files)
            {
                Match m = getRegion.Match(file);
                if (m.Success)
                {
                    string locale = m.Groups["locale"].Value;
                    this.AddLocale(locale);
                }
            }
        }

        /// <summary>
        /// Retrieve configured locale
        /// </summary>
        private void RetrieveConfiguredLocales()
        {
            string localesStr = WebConfigurationManager.AppSettings["locales"];
            if (!string.IsNullOrEmpty(localesStr))
            {
                string[] locales = localesStr.Split(',');
                foreach (string locale in locales)
                {
                    this.AddLocale(locale);
                }
            }
        }

        /// <summary>
        /// Retrieve the system available cultures. This is needed in order to check if a culture name is valid 
        /// without exception try and catch
        /// </summary>
        private void RetrieveSystemAvailableCultures()
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                this._systemAvailableCulture.Add(culture.Name, culture);
            }
        }

        #endregion
    }
}