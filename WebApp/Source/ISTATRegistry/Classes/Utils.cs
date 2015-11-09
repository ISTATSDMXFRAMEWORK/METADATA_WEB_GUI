using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISTAT.Entity;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using System.Data;
using ISTAT.WSDAL;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using ISTATRegistry.IRServiceReference;
using System.Collections.Specialized;
using System.Configuration;

namespace ISTATRegistry
{

    /// <summary>
    /// This class handles the user specified locale cookies
    /// </summary>
    internal static class LocaleResolver
    {
        #region Public Methods

        /// <summary>
        /// Get the locale from the cookie from the specified Context if available. Else return the default.
        /// </summary>
        /// <param name="context">
        /// The HTTP Context to get the cookie from
        /// </param>
        /// <returns>
        /// If the lang cookie is available, the cookie. Else the default from <see cref="I18NSupport.DefaultLocale"/>
        /// </returns>
        public static System.Globalization.CultureInfo GetCookie(HttpContext context)
        {

            System.Globalization.CultureInfo locale = System.Globalization.CultureInfo.GetCultureInfo("en");
            HttpCookie cookie = context.Request.Cookies[SESSION_KEYS.KEY_LANG];
            if (cookie != null)
            {
                string value = cookie.Value;
                locale = System.Globalization.CultureInfo.GetCultureInfo(cookie.Value);
            }

            return locale;
        }

        /// <summary>
        /// Remove cookie from browser
        /// </summary>
        /// <param name="context">
        /// The HTTP Context to add the cookie to
        /// </param>
        public static void RemoveCookie(HttpContext context)
        {
            context.Response.Cookies.Remove(SESSION_KEYS.KEY_LANG);
        }

        /// <summary>
        /// Send a cookie with the specified locale using the specified context
        /// </summary>
        /// <param name="locale">
        /// The locale string. Must be a valid locale name see <see cref="System.Globalization.CultureInfo"/>
        /// </param>
        /// <param name="context">
        /// The HTTP Context to add the cookie to
        /// </param>
        public static void SendCookie(System.Globalization.CultureInfo locale, HttpContext context)
        {
            HttpCookie cookie = new HttpCookie(SESSION_KEYS.KEY_LANG, locale.Name);
            cookie.Expires = DateTime.Now.AddDays(14);
            context.Response.Cookies.Add(cookie);
        }

        #endregion
    }

    /// <summary>
    /// Used by the PopulateCmbLanguages method
    /// </summary>
    public enum AVAILABLE_MODES
    {
        MODE_FOR_ADD_TEXT = 0,
        MODE_FOR_GLOBAL_LOCALIZATION = 1
    }

    public class Utils
    {

        private static string _script = "";


        public static bool ViewMode
        {
            get
            {
                bool userIsNotOk = (HttpContext.Current.Session[SESSION_KEYS.USER_OK] == null || (bool)HttpContext.Current.Session[SESSION_KEYS.USER_OK] == false);
                return userIsNotOk && Boolean.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["ViewMode"]);
            }
        }

        public static string ApplicationPath
        {
            get
            {
                string APPL_MD_PATH = HttpContext.Current.Request.ServerVariables["APPL_MD_PATH"].ToString();
                if (APPL_MD_PATH != "")
                    APPL_MD_PATH = @"/" + APPL_MD_PATH.Substring(APPL_MD_PATH.LastIndexOf('/'));
                APPL_MD_PATH = @"http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + APPL_MD_PATH;
                return APPL_MD_PATH;
            }
        }


        #region VARIABILI DI NUMERO DI RIGHE

        public static int GeneralCodelistGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralCodelistGridNumberRow"]);
            }
        }

        public static int DetailsCodelistGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DetailsCodelistGridNumberRow"]);
            }
        }

        public static int GeneralConceptschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralConceptschemeGridNumberRow"]);
            }
        }

        public static int DetailsConceptschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DetailsConceptschemeGridNumberRow"]);
            }
        }

        public static int GeneralKeyFamilyGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralKeyFamilyGridNumberRow"]);
            }
        }

        public static int GeneralContentConstraintNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralContentConstraintNumberRow"]);
            }
        }

        public static int GeneralCategoryschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralCategoryschemeGridNumberRow"]);
            }
        }

        public static int GeneralDataflowGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralDataflowGridNumberRow"]);
            }
        }

        public static int GeneralCategorizationGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralCategorizationGridNumberRow"]);
            }
        }

        public static int GeneralAgencyschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralAgencyschemeGridNumberRow"]);
            }
        }

        public static int DetailsAgencyschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DetailsAgencyschemeGridNumberRow"]);
            }
        }

        public static int GeneralDataProviderschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralDataProviderSchemeGridNumberRow"]);
            }
        }

        public static int DetailsDataProviderschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DetailsDataProviderSchemeGridNumberRow"]);
            }
        }

        public static int GeneralDataConsumerschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralDataConsumerSchemeGridNumberRow"]);
            }
        }

        public static int DetailsDataConsumerschemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DetailsDataConsumerSchemeGridNumberRow"]);
            }
        }

        public static int GeneralOrganizationUnitSchemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralOrganizationUnitSchemeGridNumberRow"]);
            }
        }

        public static int DetailsOrganizationUnitSchemeGridNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DetailsOrganizationUnitSchemeGridNumberRow"]);
            }
        }

        public static int GeneralStructureSetNumberRow
        {
            get
            {
                return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GeneralStructureSetNumberRow"]);
            }
        }


        #endregion

        public static string LocalizedLanguage { get { return (HttpContext.Current.Session[SESSION_KEYS.KEY_LANG] != null) ? HttpContext.Current.Session[SESSION_KEYS.KEY_LANG].ToString() : "en"; } }
        public static System.Globalization.CultureInfo LocalizedCulture { get { return new System.Globalization.CultureInfo(LocalizedLanguage); } }

        public static string GetStringKey(ArtefactIdentity rowIdentity)
        {
            string ret;
            ret = String.Format("ID={0}&AGENCY={1}&VERSION={2}", rowIdentity.ID, rowIdentity.Agency, rowIdentity.Version);

            if (rowIdentity.IsFinal != null)
                ret += String.Format("&ISFINAL={0}", rowIdentity.IsFinal);

            return ret;
        }

        public static string GetStringKey(GridViewRow gridViewRow)
        {
            ArtefactIdentity rowIdentity = GetRowKey(gridViewRow);
            return GetStringKey(rowIdentity);
        }

        public static bool IsNullOrEmptyArtefactIdentity(ArtefactIdentity aIObject)
        {
            if (aIObject == null)
                return true;

            if (String.IsNullOrEmpty(aIObject.ID) && String.IsNullOrEmpty(aIObject.Agency) && String.IsNullOrEmpty(aIObject.Version))
                return true;

            return false;
        }

        public static ArtefactIdentity GetArtefactIdentityFromString(string aiString)
        {
            string[] ai = aiString.Split(',');
            return new ArtefactIdentity(ai[0], ai[1], ai[2]);
        }

        public static ArtefactIdentity GetRowKey(GridViewRow gridViewRow)
        {
            Label lblID = (Label)gridViewRow.FindControl("lblID");
            Label lblAgency = (Label)gridViewRow.FindControl("lblAgency");
            Label lblVersion = (Label)gridViewRow.FindControl("lblVersion");
            Label lblName = (Label)gridViewRow.FindControl("lblName");

            bool? IsFinal = null;

            if (gridViewRow.FindControl("chkIsFinal") != null)
            {
                IsFinal = ((CheckBox)gridViewRow.FindControl("chkIsFinal")).Checked;
            }

            return new ArtefactIdentity(lblID.Text, lblAgency.Text, lblVersion.Text, IsFinal);
        }

        public static ArtefactIdentity GetIdentityFromRequest(HttpRequest Request)
        {
            if (Request["ID"] == null)
                return null;

            ArtefactIdentity artIDRet = null;

            if (Request["ISFINAL"] != null)
                artIDRet = new ArtefactIdentity(Request["ID"], Request["AGENCY"], Request["VERSION"], bool.Parse(Request["ISFINAL"]));
            else
                artIDRet = new ArtefactIdentity(Request["ID"], Request["AGENCY"], Request["VERSION"]);

            return artIDRet;
        }

        public static void ShowDialog(string text)
        {

            AppendScript("ShowDialog(\"" + @ReplaceInvalidJScriptChar(text) + "\");");

        }


        public static void ShowDialog(string text, int width)
        {

            AppendScript("ShowDialog(\"" + ReplaceInvalidJScriptChar(text) + "\"," + width.ToString() + ");");

        }

        public static void ShowDialog(string text, int width, string title)
        {

            //AppendScript("ShowDialog(\"" + ReplaceInvalidJScriptChar(text) + "\"," + width.ToString() + ", '" + title + "');");
            string script = string.Format("ShowDialog( '{0}', {1}, '{2}' );", ReplaceInvalidJScriptChar(text), width.ToString(), title);
            AppendScript(script);
        }

        public static void ForceBlackClosing()
        {
            AppendScript("ForceBlackClosing();");
        }

        public static void ResetBeforeUnload()
        {
            AppendScript("ResetBeforeUnload();");
        }

        public static void ReloadPage()
        {
            AppendScript("location.reload();");
        }

        public static void RemoveLatestPopup()
        {
            AppendScript("if (currentActiveIds.length > 0) currentActiveIds.pop();");
            ForceBlackClosing();
        }

        public static void AppendScript(string script)
        {
            if (_script.IndexOf(script) == -1)
                _script += script;
        }

        public static void ClearScript()
        {
            _script = "";
        }

        public static void ExecuteStaticScript(Page control)
        {
            if (_script != "")
                //ScriptManager.RegisterStartupScript(control, control.GetType(), "script", ReplaceInvalidJScriptChar(_script), true);
                ScriptManager.RegisterStartupScript(control, control.GetType(), "script", _script, true);
            _script = "";
        }

        public static string GetXMLResponseError(XmlDocument xDOc)
        {
            XmlNode xTempStructNode = xDOc.SelectSingleNode("//*[local-name()='StatusMessage']");

            if (xTempStructNode == null)
                return "";

            string ret = String.Empty;

            if (xTempStructNode.Attributes["status"].Value == "Failure")
                ret = "AN ERROR HAS OCCURRED:" + "\n\r";

            if (xTempStructNode.ChildNodes.Count > 0)
            {
                foreach (XmlNode xErr in xTempStructNode.ChildNodes[0].ChildNodes)
                {
                    if (xErr.InnerText != null)
                        ret += xErr.InnerText + "\n\r";
                }
            }
            return ret;
        }

        #region DropDownList Utils

        public static void PopulateCmbLanguages(DropDownList ddlLanguages, AVAILABLE_MODES mode, params string[] availableResources)
        {
            List<ListItem> _list = new List<ListItem>();
            switch (mode)
            {
                case AVAILABLE_MODES.MODE_FOR_ADD_TEXT:
                    foreach (System.Globalization.CultureInfo culture in SDMX_Dataloader.Main.Web.I18NSupport.Instance.SystemAvailableLocales.Values)
                        _list.Add(new ListItem(culture.DisplayName, culture.TwoLetterISOLanguageName));
                    break;
                case AVAILABLE_MODES.MODE_FOR_GLOBAL_LOCALIZATION:
                    System.Globalization.CultureInfo currentCulture = null;
                    foreach (string resourceFile in availableResources)
                    {
                        string[] splittedPath = resourceFile.Split('\\');
                        string languageName = string.Empty;
                        string fileName = splittedPath[splittedPath.Length - 1];
                        string[] fileNameParts = fileName.Split('.');
                        if (fileNameParts.Length > 2)
                        {
                            languageName = fileNameParts[1];
                        }
                        else
                        {
                            languageName = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultLanguageForResources"].ToString();
                        }
                        currentCulture = SDMX_Dataloader.Main.Web.I18NSupport.Instance.SystemAvailableLocales.Values.Where(value => value.TwoLetterISOLanguageName.ToString().Equals(languageName)).First();
                        _list.Add(new ListItem(currentCulture.DisplayName, languageName));
                    }
                    break;
            }
            _list.Sort((x, y) => string.Compare(x.Text, y.Text));
            ddlLanguages.DataSource = _list;
            ddlLanguages.DataTextField = "Text";
            ddlLanguages.DataValueField = "Value";
            ddlLanguages.DataBind();
        }

        private static void GetXmlAgencies(DropDownList ddlAgencySchemes)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Agencies.xml");
            using (DataSet ds = new DataSet())
            {
                ds.ReadXml(filePath);
                ddlAgencySchemes.DataSource = ds;
                ddlAgencySchemes.DataTextField = "name";
                ddlAgencySchemes.DataValueField = "id";
                ddlAgencySchemes.DataBind();
            }
        }


        public static void PopulateCmbAgencySchemes(DropDownList ddlAgencySchemes)
        {
            ddlAgencySchemes.Items.Clear();
            WSModel dal = new WSModel();

            //if (true)
            //{
            //    GetXmlAgencies(ddlAgencySchemes);
            //    return;
            //}

            ISdmxObjects agencies = dal.GetAllAgencyScheme(false);
            if (agencies.AgenciesSchemes != null && agencies.AgenciesSchemes.Count > 0)
            {
                ISTATUtils.LocalizedUtils loc = new ISTATUtils.LocalizedUtils(Utils.LocalizedCulture);
                foreach (IAgencyScheme aScheme in agencies.AgenciesSchemes)
                {
                    ListItem tmpItem = new ListItem(string.Format("{0}+{1}+{2}", aScheme.Id, aScheme.AgencyId, aScheme.Version));
                    ddlAgencySchemes.Items.Add(tmpItem);
                }
            }
        }

        public static void PopulateCmbAgencies(DropDownList ddlAgencies, bool excludeAgenciesForUser)
        {
            ddlAgencies.Items.Clear();

            if (HttpContext.Current.Session[SESSION_KEYS.USER_OK] != null && ((bool)HttpContext.Current.Session[SESSION_KEYS.USER_OK]) == true && excludeAgenciesForUser)
            {
                User currentUser = HttpContext.Current.Session[SESSION_KEYS.USER_DATA] as User;
                string currentLanguageSession = HttpContext.Current.Session[SESSION_KEYS.KEY_LANG] as string;
                if (currentUser != null)
                {
                    List<UserAgency> currentUserAgencies = currentUser.agencies.ToList<UserAgency>();
                    List<UserAgency> currentUserAgenciesForCurrentLanguage = currentUser.agencies.Where<UserAgency>(currentAgency => currentAgency.lang.Equals(currentLanguageSession)).ToList<UserAgency>();
                    List<UserAgency> currentUserAgenciesForDifferentLanguages = currentUser.agencies.Where<UserAgency>(currentAgency => !currentAgency.lang.Equals(currentLanguageSession)).ToList<UserAgency>();

                    List<string> agenciesAlreadyAdded = new List<string>();

                    foreach (UserAgency ag in currentUserAgenciesForCurrentLanguage)
                    {
                        string currentAgencyId = ag.id;
                        ListItem tmpItem = null;
                        //if (!ag.text.Trim().Equals(string.Empty))
                        //{
                        //    tmpItem = new ListItem(string.Format("{0} - {1}", ag.id, ag.text));
                        //}
                        //else
                        //{
                            tmpItem = new ListItem(ag.id);
                        //}
                        ddlAgencies.Items.Add(tmpItem);
                        agenciesAlreadyAdded.Add(currentAgencyId);
                    }

                    foreach (UserAgency ag in currentUserAgenciesForDifferentLanguages)
                    {
                        string currentAgencyId = ag.id;
                        ListItem tmpItem = tmpItem = new ListItem(ag.id);
                        if (!agenciesAlreadyAdded.Contains(currentAgencyId))
                        {
                            ddlAgencies.Items.Add(tmpItem);
                            agenciesAlreadyAdded.Add(currentAgencyId);
                        }
                    }
                }
            }
            else
            {
                WSModel dal = new WSModel();

                //if (true)
                //{
                //    GetXmlAgencies(ddlAgencies);
                //    return;
                //}

                ISdmxObjects agencies = dal.GetAllAgencyScheme(false);
                if (agencies.AgenciesSchemes != null && agencies.AgenciesSchemes.Count > 0)
                {
                    ISTATUtils.LocalizedUtils loc = new ISTATUtils.LocalizedUtils(Utils.LocalizedCulture);

                    foreach (IAgencyScheme aScheme in agencies.AgenciesSchemes)
                    {
                        foreach (IAgency ag in aScheme.Items)
                        {
                            ListItem tmpItem = null;
                            string agencyDescription = loc.GetNameableDescription(ag);
                            if (agencyDescription.Trim().Equals(string.Empty))
                            {
                                tmpItem = new ListItem(ag.Id);
                            }
                            else
                            {
                                tmpItem = new ListItem(string.Format("{0} - {1}", ag.Id, agencyDescription));
                            }
                            if (!ddlAgencies.Items.Contains(tmpItem))
                            {
                                ddlAgencies.Items.Add(tmpItem);
                            }
                        }
                    }
                }
            }
        }


        //public static void PopulateCmbAnnotationTypes(DropDownList cmbAnnType)
        //{
        //    NameValueCollection annotationTypeSection = (NameValueCollection)ConfigurationManager.GetSection("AnnotationTypes");

        //    foreach (string annType in annotationTypeSection.AllKeys)
        //    {
        //        cmbAnnType.Items.Add(annType);
        //    }
        //}

        public static void PopulateCmbEndPoint(DropDownList cmbEndPoint)
        {
            foreach (EndPointElement endPointEl in IRConfiguration.Config.EndPoints)
            {
                cmbEndPoint.Items.Add(endPointEl.Name);
            }

            if (HttpContext.Current.Session["SelectedEndPoint"] != null)
            {
                cmbEndPoint.SelectedValue = HttpContext.Current.Session["SelectedEndPoint"].ToString();
            }


            /*
            NameValueCollection endPointSection = (NameValueCollection)ConfigurationManager.GetSection("EndPoints");

            foreach (string endPointName in endPointSection.AllKeys)
            {
                cmbEndPoint.Items.Add(endPointName);
            }

            if (HttpContext.Current.Session["SelectedEndPoint"] != null)
            {
                cmbEndPoint.SelectedValue = HttpContext.Current.Session["SelectedEndPoint"].ToString();
            }
             */ 
        }

        public static void PopulateCmbAnnotationType(DropDownList cmbAnnType)
        {
            NameValueCollection at = (NameValueCollection)ConfigurationManager.GetSection("AnnotationTypes");

            cmbAnnType.Items.Add("");

            foreach (string atKey in at.AllKeys)
            {
                cmbAnnType.Items.Add(new ListItem(atKey,at[atKey]));
            }

        }

        public static void PopulateCmbAnnotationValue(DropDownList cmbAnnValue)
        {
            NameValueCollection at = (NameValueCollection)ConfigurationManager.GetSection("AnnotationValues");

            cmbAnnValue.Items.Add("");

            foreach (string atKey in at.AllKeys)
            {
                cmbAnnValue.Items.Add(new ListItem(atKey, at[atKey]));
            }

        }

        public static void PopulateCmbArtefacts(DropDownList ddlArtefacts, params string[] skipElements)
        {
            foreach (string artefactName in Enum.GetNames(typeof(AvailableStructures)))
            {
                if (skipElements != null)
                {
                    if (!skipElements.Contains(artefactName))
                    {
                        ddlArtefacts.Items.Add(artefactName);
                    }
                }
                else
                {
                    ddlArtefacts.Items.Add(artefactName);
                }
            }
        }

        public static void PopulatECmbReleaseCalendar(DropDownList ddlReleaseCalendar)
        {
            foreach (string rcItem in Enum.GetNames(typeof(ReleaseCalendar)))
            {
                ddlReleaseCalendar.Items.Add(new ListItem(rcItem, rcItem.Substring(0, 1)));
            }
        }



        #endregion

        #region Private Methods

        private static string ReplaceInvalidJScriptChar(string jText)
        {
            string sret;

            sret = jText.Replace("\n", "").Replace("\r", "").Replace("'", @"\'").Replace("\"", "\'");

            return sret;
        }

        #endregion

        #region CRYPTO

        public class StringCryptography
        {
            private static string _clearKey = "MY_SECRET_KEY"; //key

            public static string Encrypt(string toEncrypt)
            {
                byte[] keyArray = getMD5HashKey(_clearKey).Take(24).ToArray();//24 bit key
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;

                //mode of operation. there are other 4 modes.
                //We choose ECB(Electronic code Book)

                tdes.Mode = CipherMode.ECB;

                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateEncryptor();

                //transform the specified region of bytes array to resultArray
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                //Release resources held by TripleDes Encryptor
                tdes.Clear();

                //Return the encrypted data into unreadable string format
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }

            public static string Decrypt(string cipherString)
            {
                byte[] keyArray = getMD5HashKey(_clearKey).Take(24).ToArray();//24 bit key

                //get the byte code of the string
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;

                //mode of operation. there are other 4 modes.
                //We choose ECB(Electronic code Book)
                tdes.Mode = CipherMode.ECB;

                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                //Release resources held by TripleDes Encryptor               
                tdes.Clear();

                //return the Clear decrypted TEXT
                return UTF8Encoding.UTF8.GetString(resultArray);
            }

            private static byte[] getMD5HashKey(string input)
            {
                try
                {
                    System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
                    bs = x.ComputeHash(bs);
                    System.Text.StringBuilder s = new System.Text.StringBuilder();
                    foreach (byte b in bs)
                    {
                        s.Append(b.ToString("x2").ToLower());
                    }

                    string hash = s.ToString();
                    byte[] bytes = new byte[hash.Length * sizeof(char)];
                    System.Buffer.BlockCopy(hash.ToCharArray(), 0, bytes, 0, bytes.Length);
                    return bytes;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error, [GetDataDLL.Tools.ToolsSet.StringCryptography.getMD5Hash] " + ex.Message);
                }
            }
        }
        #endregion

    }
}