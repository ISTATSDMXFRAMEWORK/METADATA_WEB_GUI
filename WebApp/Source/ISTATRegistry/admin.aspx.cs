using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISTAT.WSDAL;
using System.Xml;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Api.Model;
using ISTATUtils;
using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System.Diagnostics;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Mapping;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using ISTATRegistry.IRServiceReference;
using System.ServiceModel;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class admin : ISTATRegistry.Classes.ISTATWebPage
    {

        //Service1SoapClient ssClient;

        IRService ssClient;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (cmbEPoints.SelectedIndex > 0)
            {
                WSClient wsClient;
                wsClient = new WSClient(IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue).IREndPoint);
                ssClient = wsClient.GetClient();
            }

            lblNewName.DataBind();
            lblNewPassword.DataBind();
            lblNewSurname.DataBind();
            lblNewUserName.DataBind();
            btnSaveNewUser.DataBind();
            if (!Page.IsPostBack)
            {
                Utils.PopulateCmbEndPoint(cmbEPoints);
                cmbEPoints.Items.Insert(0, "");
                Utils.PopulateCmbAgencySchemes(cmbAgencies);
                PopulateAgenciesGrid();
            }
        }

        protected void btnLoginSubmit_Click(object sender, EventArgs e)
        {

            string username = txtLoginUserName.Text.Trim();
            string password = txtLoginPassword.Text.Trim();

            string errMessage = "";
            int errorCount = 0;


            if (cmbEPoints.SelectedValue == string.Empty)
            {
                ++errorCount;
                errMessage = errorCount.ToString() + ") " + Resources.Messages.msg_add_endpoint + "<br>";
            }

            if (username.Equals(string.Empty))
            {
                ++errorCount;
                errMessage += errorCount.ToString() + ") " + Resources.Messages.msg_add_username + "<br>";
            }

            if (password.Equals(string.Empty))
            {
                ++errorCount;
                errMessage += errorCount.ToString() + ") " + Resources.Messages.msg_add_password + "<br>";
            }

            if (errMessage != string.Empty)
            {
                Utils.ShowDialog(errMessage);
                return;
            }

            if (IsUserAdmin(username, password))
            {
                pnlAdministrationLogin.Visible = false;
                pnlTools.Visible = true;
                PopulateUserGrid();
            }
            else
            {
                Utils.ShowDialog("Non sei autorizzato");
            }
        }

        private bool IsUserAdmin(string username, string password)
        {
            DataTable users = ssClient.GetAllUsers();

            foreach (DataRow user in users.Rows)
            {
                if ((bool)user["admin"])
                    if (user["username"].ToString() == username && user["password"].ToString() == password)
                        return true;
            }

            return false;
        }

        protected void cmbEPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            EndPointElement epe = IRConfiguration.GetEndPointByName(cmbEPoints.SelectedValue);
            Session["AdminEndPointObject"] = epe;
        }

        private void PopulateUserGrid()
        {
            DataTable users = ssClient.GetAllUsers();
            grdUsers.DataSource = users;
            grdUsers.DataBind();
        }

        private void PopulateAgenciesGrid()
        {
            WSModel wsModel = new WSModel();
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            string[] agencySchemeParts = cmbAgencies.SelectedItem.Text.Split('+');
            string id = agencySchemeParts[0], agencyId = agencySchemeParts[1], version = agencySchemeParts[2];
            IAgencyScheme agencyScheme = wsModel.GetAgencyScheme(new ArtefactIdentity(id, agencyId, version), false, true).AgenciesSchemes.FirstOrDefault();
            List<AssociatedAgency> items = new List<AssociatedAgency>();
            foreach (var agency in agencyScheme.Items)
            {
                items.Add(new AssociatedAgency(agency.Id, localizedUtils.GetNameableName(agency), localizedUtils.GetNameableDescription(agency), true));
            }
            gridView.DataSource = items;
            gridView.DataBind();

            List<string> agencies = null;
            if (Session[SESSION_KEYS.AGENCIES_TO_SET] == null)
            {
                agencies = new List<string>();
            }
            else
            {
                agencies = Session[SESSION_KEYS.AGENCIES_TO_SET] as List<string>;
            }

            foreach (GridViewRow row in gridView.Rows)
            {
                if (((CheckBox)row.Cells[3].Controls[1]).Checked)
                {
                    string currentAgency = ((Label)row.Cells[0].Controls[1]).Text;
                    if (!agencies.Contains(currentAgency))
                    {
                        agencies.Add(string.Format("{0}|{1}", currentAgency, cmbAgencies.SelectedItem.Text));
                    }
                }
            }

            Session[SESSION_KEYS.AGENCIES_TO_SET] = agencies;
            if (Page.IsPostBack)
            {
                Utils.AppendScript("openP('setAgencyDiv', 400 );");
            }
        }

        protected void grdUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvr = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
            int userId;
            int.TryParse(gvr.Cells[0].Text, out userId);

            if (e.CommandName.Equals("UPDATE"))
            {
                DataTable userTable = ssClient.GetUserById(userId);
                if (userTable != null)
                {
                    DataRow userRow = userTable.Rows[0];
                    string username = userRow[1].ToString();
                    string password = userRow[2].ToString();
                    string name = userRow[3].ToString();
                    string surname = userRow[4].ToString();
                    bool admin = (bool)userRow["admin"];
                    txt_name_update.Text = name;
                    txt_surname_update.Text = surname;
                    txt_username_update.Text = username;
                    txt_password_update.Text = password;
                    chkAdminUpdate.Checked = admin;
                    hiddenUserId.Value = userId.ToString();
                    Utils.AppendScript("openP('df-User-update', 400 );");
                }
            }
            else if (e.CommandName.Equals("DELETE"))
            {
                if (ssClient.DeleteUser(userId))
                {
                    Utils.ShowDialog(Resources.Messages.msg_user_deleting_succed);
                    PopulateUserGrid();
                }
                else
                {
                    Utils.ShowDialog(Resources.Messages.err_user_deleting_failed);
                }
            }
            else if (e.CommandName.Equals("SET_AGENCIES"))
            {

                selectedUserId.Value = userId.ToString();
                string[] agenciesCodes = ssClient.GetCodesIdByUser(userId);
                foreach (GridViewRow agencyRow in gridView.Rows)
                {
                    string rowAgencyId = ((Label)agencyRow.Cells[0].Controls[1]).Text;
                    if (agenciesCodes.Contains(rowAgencyId))
                    {
                        ((CheckBox)agencyRow.Cells[3].Controls[1]).Checked = true;
                    }
                    else
                    {
                        ((CheckBox)agencyRow.Cells[3].Controls[1]).Checked = false;
                    }
                }
                Utils.AppendScript("openP('setAgencyDiv', 400 );");
            }
        }

        protected void grdUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // NULL
        }

        protected void grdUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // NULL
        }

        protected void btnSaveNewUser_Click(object sender, EventArgs e)
        {
            string username = txtNewUserName.Text.Trim();
            string password = txtNewPassword.Text.Trim();
            string name = txtNewName.Text.Trim();
            string surname = txtNewSurName.Text.Trim();
            bool admin = chkAdmin.Checked;

            if (username.Equals(string.Empty) || password.Equals(string.Empty) || name.Equals(string.Empty) || surname.Equals(string.Empty))
            {
                Utils.ShowDialog(Resources.Messages.err_user_insert_all_data_required);
                return;
            }
            else
            {
                if (ssClient.CheckIfUserExists(username))
                {
                    Utils.ShowDialog(Resources.Messages.err_user_insert_user_already_exists);
                }
                else
                {
                    bool succed = ssClient.InsertUser(username, password, name, surname, admin);
                    if (succed)
                    {
                        Utils.ShowDialog(Resources.Messages.msg_user_insert_success_saving);
                        txtNewUserName.Text = string.Empty;
                        txtNewPassword.Text = string.Empty;
                        txtNewName.Text = string.Empty;
                        txtNewSurName.Text = string.Empty;
                        PopulateUserGrid();
                    }
                    else
                    {
                        Utils.ShowDialog(Resources.Messages.err_user_insert_failed_saving);
                    }
                }
            }
        }

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            string username = Server.HtmlEncode(txt_username_update.Text.Trim());
            string password = Server.HtmlEncode(txt_password_update.Text.Trim());
            string name = Server.HtmlEncode(txt_name_update.Text.Trim());
            string surname = Server.HtmlEncode(txt_surname_update.Text.Trim());
            bool admin = chkAdminUpdate.Checked;
            int userId = Convert.ToInt32(hiddenUserId.Value);

            if (username.Equals(string.Empty) || name.Equals(string.Empty) || surname.Equals(string.Empty))
            {
                Utils.ShowDialog(Resources.Messages.err_user_insert_all_data_required);
                return;
            }
            else
            {
                password = Utils.StringCryptography.Encrypt(password);
                bool succed = ssClient.UpdateUser(userId, username, password, name, surname, admin);
                if (succed)
                {
                    Utils.ShowDialog(Resources.Messages.msg_user_insert_success_saving);
                    PopulateUserGrid();
                }
                else
                {
                    Utils.ShowDialog(Resources.Messages.err_user_insert_failed_saving);
                }
            }
        }

        protected void cmbAgencies_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateAgenciesGrid();
            int userId = Convert.ToInt32(selectedUserId.Value);

            string[] agenciesCodes = ssClient.GetCodesIdByUser(userId);
            foreach (GridViewRow agencyRow in gridView.Rows)
            {
                string rowAgencyId = ((Label)agencyRow.Cells[0].Controls[1]).Text;
                if (agenciesCodes.Contains(rowAgencyId))
                {
                    ((CheckBox)agencyRow.Cells[3].Controls[1]).Checked = true;
                }
                else
                {
                    ((CheckBox)agencyRow.Cells[3].Controls[1]).Checked = false;
                }
            }
        }

        protected void btnConfirmSet_Click(object sender, EventArgs e)
        {
            List<string> agencies = Session[SESSION_KEYS.AGENCIES_TO_SET] as List<string>;
            if (agencies == null)
            {
                Utils.ShowDialog(Resources.Messages.err_missing_agencies_from_session);
                return;
            }
            else
            {
                string userId = selectedUserId.Value.ToString();

                ssClient.CleanAgenciesRelationForUser(Convert.ToInt32(userId));
                string[] agenciesArray = agencies.ToArray<string>();
                foreach (string agency in agenciesArray)
                {
                    string agencySchemeMain = agency.Split('|')[1];
                    string agencyCode = agency.Split('|')[0];
                    string[] agencySchemeParts = agencySchemeMain.Split('+');
                    string agencySchemeId = agencySchemeParts[0], agencySchemeAgency = agencySchemeParts[1], agencySchemeVersion = agencySchemeParts[2];
                    ssClient.CreateUserAgenciesRelation(Convert.ToInt32(userId), agencySchemeId, agencySchemeAgency, agencySchemeVersion, agencyCode);
                }
                Session[SESSION_KEYS.AGENCIES_TO_SET] = null;
                //client.CreateUserAgenciesRelation( Convert.ToInt32( userId ), agencySchemeId, agencySchemeAgency, agencySchemeVersion, agenciesArray );                           
            }
        }

        protected void btnCancelOperation_Click(object sender, EventArgs e)
        {
            // NULL
        }

        protected void btnSet_Click(object sender, EventArgs e)
        {
            bool oneRowIsOk = false;
            GridViewRowCollection rows = gridView.Rows;

            foreach (GridViewRow row in rows)
            {
                if (((CheckBox)row.Cells[3].Controls[1]).Checked)
                {
                    oneRowIsOk = true;
                    break;
                }
            }
            if (!oneRowIsOk)
            {
                Utils.ShowDialog(Resources.Messages.err_at_least_one_row);
            }
            else
            {
                string userId = selectedUserId.Value.ToString();
                string[] agencySchemeParts = cmbAgencies.SelectedItem.Text.Split('+');
                string agencySchemeId = agencySchemeParts[0], agencySchemeAgency = agencySchemeParts[1], agencySchemeVersion = agencySchemeParts[2];
                List<string> agencies = null;
                if (Session[SESSION_KEYS.AGENCIES_TO_SET] == null)
                {
                    agencies = new List<string>();
                }
                else
                {
                    agencies = Session[SESSION_KEYS.AGENCIES_TO_SET] as List<string>;
                    if (agencies == null)
                    {
                        Utils.ShowDialog(Resources.Messages.err_getting_agencies_from_session);
                        return;
                    }
                }

                foreach (GridViewRow row in rows)
                {
                    string currentAgency = ((Label)row.Cells[0].Controls[1]).Text;
                    if (((CheckBox)row.Cells[3].Controls[1]).Checked)
                    {
                        if (!agencies.Contains(string.Format("{0}|{1}", currentAgency, cmbAgencies.SelectedItem.Text)))
                        {
                            agencies.Add(string.Format("{0}|{1}", currentAgency, cmbAgencies.SelectedItem.Text));
                        }
                    }
                    else
                    {
                        if (agencies.Contains(string.Format("{0}|{1}", currentAgency, cmbAgencies.SelectedItem.Text)))
                        {
                            agencies.Remove(string.Format("{0}|{1}", currentAgency, cmbAgencies.SelectedItem.Text));
                        }
                    }
                }
                Session[SESSION_KEYS.AGENCIES_TO_SET] = agencies;
            }
            Utils.AppendScript("openP('setAgencyDiv', 400 );");
        }
    }
}


