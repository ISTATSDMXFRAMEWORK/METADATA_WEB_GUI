using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Collections.Specialized;
using ISTATRegistry.IRServiceReference;
using System.Xml.Linq;

namespace ISTATRegistry
{

    public static class SESSION_KEYS
    {
        public static readonly string KEY_LANG = "Language";
        public static readonly string OLD_KEY_LANG = "OldLanguage";
        public static readonly string USER_OK = "USER_OK";
        public static readonly string USER_DATA = "USER_DATA";
        public static readonly string USER_AGENCIES = "USER_AGENCIES";
        public static readonly string AGENCIES_TO_SET = "AGENCIES_TO_SET";
        public static readonly string CURRENT_ENDPOINT_OBJECT = "CurrentEndPointObject";
    }

    public partial class master : System.Web.UI.MasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                if (Request["m"] != null && Request["m"].ToString() == "y")
                {
                    CheckSessioneExpired();
                }

                if (Request["se"] != null && Request["se"].ToString() == "y")
                {
                    Utils.ClearScript();
                    Utils.ShowDialog(Resources.Messages.msg_session_expired);
                }

                pnlUpload.Visible = !Utils.ViewMode;
                //pnlNotVisible.Visible = !Utils.ViewMode;
                string[] resources = Directory.GetFiles(Server.MapPath("~/App_GlobalResources"), "*.resx");
                Utils.PopulateCmbLanguages(cmbLanguage, AVAILABLE_MODES.MODE_FOR_GLOBAL_LOCALIZATION, resources);
                Utils.PopulateCmbEndPoint(cmbEPoints);
                SetEndPoint();

                EndPointElement epe = IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue);
                lnkAdmin.Visible = epe.EnableAdministration;
                loginButton.Visible = epe.EnableAuthentication;
                pnlExtraArtefact.Visible = !epe.PartialArtefact;
                Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT] = epe;

                if (Session[SESSION_KEYS.KEY_LANG] != null)
                {
                    cmbLanguage.SelectedValue = Session[SESSION_KEYS.KEY_LANG].ToString();
                    Session[SESSION_KEYS.OLD_KEY_LANG] = cmbLanguage.SelectedItem.Value;
                }
                else
                {
                    var globalizationSection = WebConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection;
                    cmbLanguage.SelectedValue = globalizationSection.Culture;
                    Session[SESSION_KEYS.KEY_LANG] = cmbLanguage.SelectedValue.ToString();
                    Session[SESSION_KEYS.OLD_KEY_LANG] = cmbLanguage.SelectedItem.Value;
                }
                LocaleResolver.SendCookie(new System.Globalization.CultureInfo(Session[SESSION_KEYS.KEY_LANG].ToString()), HttpContext.Current);
            }
            else
            {
                CheckSessioneExpired();
            }

            if (Session[SESSION_KEYS.USER_OK] != null && (bool)Session[SESSION_KEYS.USER_OK] == true)
            {
                lblWelcomeUser.Visible = true;
                User currentUser = (User)Session[SESSION_KEYS.USER_DATA];
                string userCompleteName = string.Format("{0} {1}", currentUser.name.ToString(), currentUser.surname.ToString());
                lblWelcomeUser.Text = string.Format(Resources.Messages.lblWelcomeUserMessagge, userCompleteName);
                loginButton.Visible = false;
                logoutButton.Visible = true;
            }

        }

        protected void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool foundLanguage = false;
            string[] resources = Directory.GetFiles(Server.MapPath("~/App_GlobalResources"), "*.resx");
            foreach (string resourceFile in resources)
            {
                string[] splittedPath = resourceFile.Split('\\');
                string fileName = splittedPath[splittedPath.Length - 1];
                string languageName = fileName.Split('.')[1];
                if (languageName.Equals(cmbLanguage.SelectedItem.Value))
                {
                    foundLanguage = true;
                    break;
                }
            }

            if (foundLanguage)
            {
                Session[SESSION_KEYS.KEY_LANG] = cmbLanguage.SelectedItem.Value;
                Session[SESSION_KEYS.OLD_KEY_LANG] = cmbLanguage.SelectedItem.Value;
                LocaleResolver.SendCookie(new System.Globalization.CultureInfo(Session[SESSION_KEYS.KEY_LANG].ToString()), HttpContext.Current);
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                string defaultLanguage = ConfigurationManager.AppSettings["DefaultLanguageForResources"].ToString();
                if (defaultLanguage.Equals(cmbLanguage.SelectedValue))
                {
                    Session[SESSION_KEYS.KEY_LANG] = cmbLanguage.SelectedItem.Value;
                    Session[SESSION_KEYS.OLD_KEY_LANG] = cmbLanguage.SelectedItem.Value;
                    LocaleResolver.SendCookie(new System.Globalization.CultureInfo(Session[SESSION_KEYS.KEY_LANG].ToString()), HttpContext.Current);
                    Response.Redirect(Request.RawUrl);
                    return;
                }
                cmbLanguage.SelectedValue = Session[SESSION_KEYS.OLD_KEY_LANG].ToString();
                Utils.ShowDialog(Resources.Messages.err_missing_language_file);
            }
        }

        protected void cmbLanguage_DataBound(object sender, EventArgs e)
        {
            //Session[SESSION_KEYS.KEY_LANG] = cmbLanguage.SelectedItem.Value;
            //LocaleResolver.SendCookie(new System.Globalization.CultureInfo(Session[SESSION_KEYS.KEY_LANG].ToString()),HttpContext.Current);

        }

        protected void cmbLanguage_DataBinding(object sender, EventArgs e)
        {
            // NULL
        }

        protected void btnLoginSubmit_Click(object sender, EventArgs e)
        {
            string myUserName = txtLoginUserName.Text.Trim();
            string myPassword = txtLoginPassword.Text.Trim();
            // Qui vengono recuperate le credenziali

            EndPointElement epe = (EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT];

            User currentUser;

            if (epe.GetUsersFromFile)
            {
                currentUser = GetUserFromFile(myUserName, myPassword);
            }
            else
            {
                WSClient wsClient = new WSClient(epe.IREndPoint);
                IRService authClient = wsClient.GetClient();
                currentUser = authClient.GetUserByCredentials(myUserName, myPassword);
            }

            if (currentUser != null)
            {
                Session[SESSION_KEYS.USER_OK] = true;
                Session[SESSION_KEYS.USER_DATA] = currentUser;
                lblWelcomeUser.Visible = true;
                string userCompleteName = string.Format("{0} {1}", currentUser.name.ToString(), currentUser.surname.ToString());
                lblWelcomeUser.Text = string.Format(Resources.Messages.lblWelcomeUserMessagge, userCompleteName);
                loginButton.Visible = false;
                logoutButton.Visible = true;
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                Utils.ShowDialog("Le credenziali di accesso non sono corrette!");
            }
        }

        private User GetUserFromFile(string myUserName, string myPassword)
        {
            User currentUser = new User();

            string filePath = IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue).UsersFilePath;

            XElement xelement = XElement.Load(Server.MapPath(filePath));
            IEnumerable<XElement> users = xelement.Elements();

            var user = from u in xelement.Elements("user")
                       where (string)u.Attribute("username") == myUserName
                            && (string)u.Attribute("password") == myPassword
                       select u;

            if (user.Count() == 0)
                return null;

            var us = user.First();

            currentUser.username = us.Attribute("username").Value;
            currentUser.name = us.Attribute("name").Value;
            currentUser.surname = us.Attribute("surname").Value;

            var agencies = from ags in us.Descendants("agency")
                select ags;

            UserAgency[] userAgencies = new UserAgency[agencies.Count()];
            int count = 0;

            foreach (string agencyId in agencies.Attributes("id"))
            {
                userAgencies[count] = new UserAgency(){id=agencyId, lang="en"};
                ++count;
            }

            currentUser.agencies = userAgencies;

            return currentUser;
        }

        private void LogOutUser()
        {
            //EndPointElement epe = IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue);

            Session[SESSION_KEYS.USER_OK] = null;
            Session[SESSION_KEYS.USER_DATA] = null;
            Session[SESSION_KEYS.USER_AGENCIES] = null;
            lblWelcomeUser.Visible = false;
            lblWelcomeUser.Text = string.Empty;
            loginButton.Visible = true;
            logoutButton.Visible = false;
            Response.Redirect("~/default.aspx");
        }

        protected void logoutButton_Click(object sender, EventArgs e)
        {
            LogOutUser();

        }

        protected void cmbEPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedEndPoint"] = cmbEPoints.SelectedValue;

            EndPointElement epe = IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue);
            Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT] = epe;

            LogOutUser();
        }

        private void SetEndPoint()
        {
            Session["WSEndPoint"] = IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue).NSIEndPoint;

            //NameValueCollection endPointSection = (NameValueCollection)ConfigurationManager.GetSection("EndPoints");
            //Session["WSEndPoint"] = endPointSection[cmbEPoints.SelectedValue];
        }

        private void CheckSessioneExpired()
        {
            if (Session[SESSION_KEYS.KEY_LANG] == null)
            {
                Response.Clear();
                Response.Redirect(Utils.ApplicationPath + "?se=y");
                Response.End();
            }
        }

    }

}