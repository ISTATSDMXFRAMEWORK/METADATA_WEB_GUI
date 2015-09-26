using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.IO;

using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;

using ISTATUtils;
using ISTAT.EntityMapper;
using ISTAT.Entity;
using ISTAT.WSDAL;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{

    public partial class agencyschemeItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        public struct csvAgency
        {
            public string agency;
            public string name;
            public string description;

            public csvAgency(string agency, string name, string description)
            {
                this.agency = agency;
                this.name = name;
                this.description = description;
              }
        }

        public static string KEY_PAGE_SESSION = "TempAgencyscheme";

        ArtefactIdentity _artIdentity;
        Action _action;
        ISdmxObjects _sdmxObjects;
        LocalizedUtils _localizedUtils;
        
        protected string AspConfirmationExit = "false";

        private void SetAction()
        {
            if (Request["ACTION"] == null || Utils.ViewMode )
                _action = Action.VIEW;
            else
            {
                if ( Request["ACTION"] == "UPDATE" )
                {
                    IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;
                    _artIdentity = Utils.GetIdentityFromRequest(Request);
                    int agencyOccurence = currentUser.agencies.Count( agency => agency.id.Equals( _artIdentity.Agency) );
                    if ( agencyOccurence > 0 )
                    {
                        _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                    }
                    else
                    {
                        _action = Action.VIEW;
                    }
                }
                else
                {
                    _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                }
            }
        }

        private IAgencySchemeMutableObject GetAgencyschemeForm()
        {
            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region AGENCYSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region AGENCYSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME START END DATE
            bool checkForDatesCombination = true;

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidFrom.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_from_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidTo.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidTo.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_to_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !txtValidTo.Text.Trim().Equals(string.Empty))
            {
                // Controllo congruenza date
                if (checkForDatesCombination)
                {
                    if (!ValidationUtils.CheckDates(txtValidFrom.Text, txtValidTo.Text))
                    {
                        messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_diff + "<br /><br />";
                        errorCounter++;
                        isInError = true;
                    }
                }
            }
            #endregion

            if (isInError)
            {
                Utils.ShowDialog(messagesGroup, 300);
                return null;
            }

            IAgencySchemeMutableObject tmpAgencyscheme = new AgencySchemeMutableCore();
            #region CREATE AGENCYSCHEME FROM FORM

            tmpAgencyscheme.AgencyId = cmbAgencies.SelectedValue.Split( '-' )[0].Trim();
            tmpAgencyscheme.Id = txtDSDID.Text;
            tmpAgencyscheme.Version = txtVersion.Text;
            tmpAgencyscheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            tmpAgencyscheme.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty ) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                tmpAgencyscheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                tmpAgencyscheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                tmpAgencyscheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    tmpAgencyscheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    tmpAgencyscheme.AddAnnotation(annotation);
                }

            #endregion

            return tmpAgencyscheme;
        }

        private string GetAgencyValue()
        {
            if ( _action == Action.INSERT )
            {

                string agencyValue = cmbAgencies.SelectedValue.ToString();
                string agencyId = agencyValue.Split('-')[0].Trim();
                return agencyId;
            }
            else
            {
                return txtAgenciesReadOnly.Text;
            }
        }

        private IAgencySchemeMutableObject GetAgencyschemeForm(IAgencySchemeMutableObject agencyScheme)
        {

            if (agencyScheme == null) return GetAgencyschemeForm();

            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region AGENCYSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME AGENCY
            //if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            //{
            //    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
            //    errorCounter++;
            //    isInError = true;
            //}
            #endregion

            #region AGENCYSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region AGENCYSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region AGENCYSCHEME START END DATE
            bool checkForDatesCombination = true;

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidFrom.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_from_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidTo.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidTo.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_to_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !txtValidTo.Text.Trim().Equals(string.Empty))
            {
                // Controllo congruenza date
                if (checkForDatesCombination)
                {
                    if (!ValidationUtils.CheckDates(txtValidFrom.Text, txtValidTo.Text))
                    {
                        messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_diff + "<br /><br />";
                        errorCounter++;
                        isInError = true;
                    }
                }
            }
            #endregion

            if (isInError)
            {
                Utils.ShowDialog(messagesGroup, 300);
                return null;
            }

            #region CREATE AGENCYSCHEME FROM FORM

            agencyScheme.AgencyId = GetAgencyValue();
            agencyScheme.Id = txtDSDID.Text;
            agencyScheme.Version = txtVersion.Text;
            agencyScheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            agencyScheme.Uri = (ValidationUtils.CheckUriFormat(txtDSDURI.Text) && !txtDSDURI.Text.Trim().Equals( string.Empty ) ) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                agencyScheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                agencyScheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (agencyScheme.Names.Count != 0)
            {
                agencyScheme.Names.Clear();
            }
            if (agencyScheme.Descriptions.Count != 0)
            {
                agencyScheme.Descriptions.Clear();
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                agencyScheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    agencyScheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }

            if (agencyScheme.Annotations.Count != 0)
            {
                agencyScheme.Annotations.Clear();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    agencyScheme.AddAnnotation(annotation);
                }

            #endregion

            return agencyScheme;
        }

        private void OpenAddAgencyPopUp()
        {
            Utils.AppendScript("openP('df-AddAgency',600);");
        }

        private IAgencySchemeMutableObject InsertAgencyInAgencyscheme(IAgencySchemeMutableObject agencyScheme)
        {
            if (agencyScheme == null) return null;

            IAgencyMutableObject agency = new AgencyMutableCore();

            string agency_id = txt_id_new.Text.Trim();

            IList<ITextTypeWrapperMutableObject> agency_names = AddTextName_new.TextObjectList;
            IList<ITextTypeWrapperMutableObject> agency_descs = AddTextDescription_new.TextObjectList;
            // string code_order_str = txtOrderNewCode.Text.Trim();     ----- ORDINE

            #region AGENCYSCHEME ID
            if (ValidationUtils.CheckIdFormat(agency_id))
            {
                agency.Id = agency_id;
            }
            else
            {
                //lblErrorOnNewInsert.Text = Resources.Messages.err_id_format;
                Utils.AppendScript("location.href= '#agencies';");
                OpenAddAgencyPopUp();
                Utils.ShowDialog(Resources.Messages.err_id_format, 300);
                return null;
            }

            IEnumerable<IAgencyMutableObject> agencies = (from c in agencyScheme.Items where c.Id == agency_id select c).OfType<IAgencyMutableObject>();
            if (agencies.Count() > 0)
            {
                //lblErrorOnNewInsert.Text = Resources.Messages.err_id_exist;
                Utils.AppendScript("location.href= '#agencies';");
                OpenAddAgencyPopUp();
                Utils.ShowDialog(Resources.Messages.err_id_exist, 300);
                return null;
            }
            #endregion

            #region AGENCY NAMES
            if (agency_names != null)
            {
                foreach (var tmpName in agency_names)
                {
                    agency.AddName(tmpName.Locale, tmpName.Value);
                }
            }
            else
            {
                //lblErrorOnNewInsert.Text = Resources.Messages.err_list_name_format;
                Utils.AppendScript("location.href= '#agencies';");
                OpenAddAgencyPopUp();
                Utils.ShowDialog(Resources.Messages.err_list_name_format, 300);
                return null;
            }
            #endregion

            #region AGENCY DESCRIPTIONS
            if (agency_descs != null)
            {
                foreach (var tmpDescription in agency_descs)
                {
                    agency.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            }
            #endregion

            agencyScheme.Items.Add(agency);

            try
            {
                // Ultimo controllo se ottengo Immutable istanze validazione completa
                var canRead = agencyScheme.ImmutableInstance;
            }
            catch (Exception ex)
            {
                agencyScheme.Items.Remove(agency);

                return null;

            }
            return agencyScheme;
        }

        private IAgencySchemeMutableObject GetAgencySchemeFromSession()
        {
            try
            {
                if (Session[KEY_PAGE_SESSION] == null)
                {
                    if (_artIdentity.ToString() != string.Empty)
                    {
                        WSModel wsModel = new WSModel();
                        ISdmxObjects sdmxObject = wsModel.GetAgencyScheme(_artIdentity, false,false);
                        IAgencyScheme agencyScheme = sdmxObject.AgenciesSchemes.FirstOrDefault();
                        Session[KEY_PAGE_SESSION] = agencyScheme.MutableInstance;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                return (IAgencySchemeMutableObject)Session[KEY_PAGE_SESSION];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveInMemory(IAgencySchemeMutableObject agencyScheme)
        {
            if (agencyScheme == null) return false;

            Session[KEY_PAGE_SESSION] = agencyScheme;

            return true;
        }

        private bool SendQuerySubmit(IAgencySchemeMutableObject agencyScheme)
        {

            try
            {

                ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

                sdmxObjects.AddAgencyScheme(agencyScheme.ImmutableInstance);

                WSModel modelAgencyScheme = new WSModel();

                XmlDocument result = modelAgencyScheme.SubmitStructure(sdmxObjects);

                Utils.GetXMLResponseError(result);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private void ClearSessionPage()
        {
            Session[KEY_PAGE_SESSION] = null;
        }

        private int CurrentAgencyIndex { get { return (int)Session[KEY_PAGE_SESSION + "_index_agency"]; } set { Session[KEY_PAGE_SESSION + "_index_agency"] = value; } }

        #region Event Handler

        protected void Page_Load(object sender, EventArgs e)
        {
            lblImportCsvTitle.DataBind();
            lblCsvLanguage.DataBind();
            lblcsvFile.DataBind();
            btnImportFromCsv.DataBind();
            lblDSDID.DataBind();
            lblVersion.DataBind();
            lblAgency.DataBind();
            lblValidFrom.DataBind();
            lblValidTo.DataBind();
            lblDSDURI.DataBind();
            lblDSDURN.DataBind();
            lblIsFinal.DataBind();
            lblDSDName.DataBind();
            lblDSDDescription.DataBind();
            lbl_title_new.DataBind();
            lbl_id_new.DataBind();
            lbl_name_new.DataBind();
            lbl_description_new.DataBind();
            btnNewAgency.DataBind();
            lbl_title_update.DataBind();
            lbl_id_update.DataBind();
            lbl_name_update.DataBind();
            lbl_description_update.DataBind();
            btnUpdateAgency.DataBind();
            btnAddNewAgency.DataBind();
            btnSaveMemoryAgencyScheme.DataBind();
            btnSaveAnnotationCode.DataBind();
            lblNumberOfRows.DataBind();
            btnClearFields.DataBind();
            btnClearFieldForUpdate.DataBind();
            btnChangePaging.DataBind();
            lbl_annotation.DataBind();
            lblSeparator.DataBind();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);

            SetAction();

            if (!IsPostBack)
            {
                Utils.PopulateCmbAgencies(cmbAgencies, true);
                ClearSessionPage();
                txtNumberOfRows.Text = Utils.DetailsAgencyschemeGridNumberRow.ToString();
                AnnotationGeneralControl.ClearAnnotationsSession();
            }

            switch (_action)
            {
                case Action.INSERT:

                    //ClearSessionPage();
                    AspConfirmationExit = "true";

                    SetInitControls();
                    SetInsertForm();

                    /*chkIsFinal.Checked = false;
                    chkIsFinal.Enabled = false;*/

                    AddTextName_Update.ucOpenTabName = "agencies";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Agency-update";

                    AddTextDescription_Update.ucOpenTabName = "agencies";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Agency-update";

                    AddTextName_new.ucOpenTabName = "agencies";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-AddAgency";

                    AddTextDescription_new.ucOpenTabName = "agencies";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-AddAgency";

                    if ( !Page.IsPostBack )
                    {
                        cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                        cmbAgencies.SelectedIndex = 0;

                        FileDownload31.Visible = false;
                    }
                    
                    break;
                case Action.UPDATE:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);
                    //ClearSessionPage();

                    SetInitControls();
                    SetEditForm();

                    AddTextName_Update.ucOpenTabName = "agencies";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Agency-update";

                    AddTextDescription_Update.ucOpenTabName = "agencies";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Agency-update";

                    AddTextName_new.ucOpenTabName = "agencies";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-AddAgency";

                    AddTextDescription_new.ucOpenTabName = "agencies";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-AddAgency";

                    /*if (gvConceptschemesItem.Rows.Count > 0)
                        chkIsFinal.Enabled = true;
                    else
                        chkIsFinal.Enabled = false;*/

                    break;
                case Action.VIEW:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);
                    ClearSessionPage();
                    SetViewForm();
                    AddTextName_Update.ucOpenTabName = "agencies";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Agency-update";

                    AddTextDescription_Update.ucOpenTabName = "agencies";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Agency-update";

                    AddTextName_new.ucOpenTabName = "agencies";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-AddAgency";

                    AddTextDescription_new.ucOpenTabName = "agencies";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-AddAgency";
                    /*FileDownload31.ucID = _artIdentity.ID;
                    FileDownload31.ucAgency = _artIdentity.Agency;
                    FileDownload31.ucVersion = _artIdentity.Version;
                    FileDownload31.ucArtefactType = "AgencyScheme";*/

                    break;
            }

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.AgencyScheme;
            DuplicateArtefact1.ucMaintanableArtefact = GetAgencySchemeFromSession();

            lblNoItemsPresent.DataBind();
        }

        protected void gvAgencyschemesItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAgencyschemesItem.PageSize = 12;
            gvAgencyschemesItem.PageIndex = e.NewPageIndex;
            BindData();
            Utils.AppendScript("location.href= '#agencies';");
        }

        protected void btnSaveMemoryAgencyScheme_Click(object sender, EventArgs e)
        {

            IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();
            if (agencyScheme == null) agencyScheme = GetAgencyschemeForm();
            else agencyScheme = GetAgencyschemeForm(agencyScheme);

            if (!SaveInMemory(agencyScheme)) return;

            if (!SendQuerySubmit(agencyScheme)) return;

            BindData();

            string successMessage = string.Empty;
            if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.INSERT)
            {
                successMessage = Resources.Messages.succ_agency_insert;
            }
            else if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.UPDATE)
            {
                successMessage = Resources.Messages.succ_agency_update;
            }
            Utils.ShowDialog(successMessage, 300, Resources.Messages.succ_operation);
            if ( _action == Action.INSERT )
            {
                Utils.AppendScript( "~/agencies.aspx" );
            }
        }

        protected void btnAddNewAgency_Click(object sender, EventArgs e)
        {
            IAgencySchemeMutableObject agency = GetAgencySchemeFromSession();

            agency = GetAgencyschemeForm(agency);

            // form codelist validation
            if (agency == null) 
            {
                txt_id_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                return;
            }
            agency = InsertAgencyInAgencyscheme(agency);

            if (agency == null)
            {
                //txt_id_new.Text = string.Empty;
                //AddTextName_new.ClearTextObjectListWithOutJS();
                //AddTextDescription_new.ClearTextObjectListWithOutJS();
                //lblErrorOnNewInsert.Text = string.Empty;
                //Utils.ShowDialog(Resources.Messages.err_agency_insert, 300, Resources.Messages.err_title);
                //Utils.AppendScript("location.href= '#agencies';");
                return;
            }
            
            if (!SaveInMemory(agency)) 
            {
                txt_id_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                return;
            }

            BindData();

            txt_id_new.Text = string.Empty;
            AddTextName_new.ClearTextObjectListWithOutJS();
            AddTextDescription_new.ClearTextObjectListWithOutJS();
            lblErrorOnNewInsert.Text = string.Empty;
            Utils.AppendScript("location.href='#agencies';");
        }

        protected void btnUpdateAgency_Click(object sender, EventArgs e)
        {
            // Get Input field
            string agency_id = txt_id_update.Text.Trim();
            IList<ITextTypeWrapperMutableObject> agency_names = AddTextName_Update.TextObjectList;
            IList<ITextTypeWrapperMutableObject> agency_descs = AddTextDescription_Update.TextObjectList;
            // string code_order_str = txtUpdateCodeOrder.Text.Trim();


            // Get Current Object Session
            IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();
            IEnumerable<IAgencyMutableObject> _rc = (from x in agencyScheme.Items where x.Id == agency_id select x).OfType<IAgencyMutableObject>();
            if (_rc.Count() == 0) return;

            IAgencyMutableObject agency = _rc.First();

            IAgencyMutableObject _bAgency = new AgencyMutableCore();

            int indexAgency = agencyScheme.Items.IndexOf(agency);
            int indexOrder = 0;
            try
            {

                #region CONCEPT ID
                if (!agency_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(agency_id))
                {
                    _bAgency.Id = agency_id;
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_id_format;
                    Utils.AppendScript( "openPopUp('df-Agency-update', 600 );" );
                    Utils.AppendScript("location.href= '#concepts';");
                    return;
                }
                #endregion

                #region AGENCY NAMES
                if (agency_names != null)
                {
                    foreach (var tmpName in agency_names)
                    {
                        _bAgency.AddName(tmpName.Locale, tmpName.Value);
                    }
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_list_name_format;
                    Utils.AppendScript( "openPopUp('df-Agency-update', 600 );" );
                    Utils.AppendScript("location.href= '#concepts';");
                    return;
                }
                #endregion

                #region AGENCY DESCRIPTIONS
                if (agency_descs != null)
                {
                    foreach (var tmpDescription in agency_descs)
                    {
                        _bAgency.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                    }
                }
                #endregion

                agencyScheme.Items.Remove(agency);
                agencyScheme.Items.Insert(indexAgency, _bAgency);

                var canRead = agencyScheme.ImmutableInstance;

            }
            catch (Exception ex) // ERRORE GENERICO!
            {
                agencyScheme.Items.Remove(_bAgency);
                agencyScheme.Items.Insert(indexAgency, agency);

                Utils.ShowDialog(Resources.Messages.err_agency_update, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href='#agencies';");

            }

            if (!SaveInMemory(agencyScheme))
                return;

            BindData();
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href='#agencies';");
        }

        protected void gvAgencyschemesItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "UPDATE":
                    {
                        // Svuoto i controlli
                        // ---------------------------------
                        txt_id_update.Text = string.Empty;
                        //txtUpdateCodeOrder.Text = string.Empty;
                        AddTextName_Update.ClearTextObjectList();
                        AddTextDescription_Update.ClearTextObjectList();
                        // ---------------------------------

                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        txt_id_update.Text = ((Label)gvr.Cells[1].Controls[1]).Text;

                        IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > agencyScheme.ImmutableInstance.Items.Count) return;

                        if ( gvAgencyschemesItem.PageIndex > 0 )
                        {
                            CurrentAgencyIndex = gvr.RowIndex + ( gvAgencyschemesItem.PageIndex * gvAgencyschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentAgencyIndex = gvr.RowIndex;
                        }

                        IAgency currentAgency = ((IAgency)agencyScheme.ImmutableInstance.Items[CurrentAgencyIndex]);

                        AddTextName_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Agency;
                        AddTextName_Update.TType = TextType.NAME;
                        AddTextName_Update.ClearTextObjectList();
                        AddTextName_Update.InitTextObjectList = currentAgency.Names;

                        AddTextDescription_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Agency;
                        AddTextDescription_Update.TType = TextType.DESCRIPTION;
                        AddTextDescription_Update.ClearTextObjectList();
                        AddTextDescription_Update.InitTextObjectList = currentAgency.Descriptions;
                        // txtUpdateCodeOrder.Text = (gvr.RowIndex + 1).ToString();
                        break;
                    }
                case "DELETE":
                    {

                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);

                        IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > agencyScheme.Items.Count) return;
                        agencyScheme.Items.RemoveAt(gvr.RowIndex);
                        Session[KEY_PAGE_SESSION] = agencyScheme;
                        BindData();
                        
                        Utils.AppendScript("location.href='#agencies'");
                    }
                    break;
                case "ANNOTATION":
                    {
                        IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();
                        if ( agencyScheme == null ) 
                        {
                            agencyScheme = GetAgencyschemeForm(agencyScheme);
                        }

                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);

                        if (gvr.RowIndex < 0 && gvr.RowIndex > agencyScheme.Items.Count) return;

                        if ( gvAgencyschemesItem.PageIndex > 0 )
                        {
                            CurrentAgencyIndex = gvr.RowIndex + ( gvAgencyschemesItem.PageIndex * gvAgencyschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentAgencyIndex = gvr.RowIndex;
                        }

                        //ctr_annotation_update.EditMode = !Utils.ViewMode;
                        btnSaveAnnotationCode.Visible = !Utils.ViewMode;

                        ctr_annotation_update.AnnotationObjectList = agencyScheme.Items[CurrentAgencyIndex].Annotations;

                        Utils.AppendScript("openP('agency_annotation',650);");

                        Utils.AppendScript("location.href= '#agencies';");

                    } break;


            }

        }
        protected void gvAgencyschemesItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // NULL
        }
        protected void gvAgencyschemesItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // NULL
        }
        protected void gvAgencyschemesItem_Sorting(object sender, GridViewSortEventArgs e)
        {

        }
        protected void gvCodelistsItem_Sorted(object sender, EventArgs e)
        {
            // NULL
        }

        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {
            if ( !csvFile.HasFile )
            {
                Utils.AppendScript( "openPopUp( 'importCsv' );" );
                Utils.AppendScript( "location.href='#agencies'" );
                Utils.AppendScript( string.Format( "alert( '{0}' );", Resources.Messages.err_no_file_uploaded ) );
                return;
            }

            IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();
            if (agencyScheme == null) return;
            List<csvAgency> agencies = new List<csvAgency>();
            bool errorInUploading = false;

            try
            {
                string filenameWithoutExtension = string.Format("{0}_{1}_{2}", Path.GetFileName(csvFile.FileName).Substring(0, csvFile.FileName.Length - 4), Session.SessionID, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_'));
                string filename = string.Format("{0}.csv", filenameWithoutExtension);
                string logFilename = string.Format("{0}.log", filenameWithoutExtension);
                csvFile.SaveAs(Server.MapPath("~/csv_agencyschemes_files/") + filename);

                StreamReader reader = new StreamReader(Server.MapPath("~/csv_agencyschemes_files/") + filename);
                StreamWriter logWriter = new StreamWriter(Server.MapPath("~/csv_agencyschemes_import_logs/") + logFilename, true);
                logWriter.WriteLine(string.Format("LOG RELATIVO A CARICAMENTO DEL AGENCY SCHEME [ ID => \"{0}\"  AGENCY_ID => \"{1}\"  VERSION => \"{2}\" ]  |  LINGUA SELEZIONATA: {3}\n", agencyScheme.Id.ToString(), agencyScheme.AgencyId.ToString(), agencyScheme.Version.ToString(), cmbLanguageForCsv.SelectedValue.ToString()));
                logWriter.WriteLine("-----------------------------------------------------------------------------------------------------------------------------\n");
                reader.ReadLine();
                string wrongRowsMessage = string.Empty;
                string wrongRowsMessageForUser = string.Empty;
                string wrongFileLines = string.Empty;
                int currentRow = 1;

                char separator = txtSeparator.Text.Trim().Equals( string.Empty ) ? ';' : txtSeparator.Text.Trim().ElementAt( 0 );

                while (!reader.EndOfStream)
                {
                    string  currentFileLine = reader.ReadLine();
                    string[] fields = currentFileLine.Split( separator );
                    if (fields.Length != 3)
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_line_bad_format, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_line_bad_format_gui, currentRow + 1);
                        wrongFileLines += string.Format( "{0}\n", currentFileLine );
                        logWriter.WriteLine(string.Format(Resources.Messages.err_csv_import_line_bad_format, currentRow + 1));
                        logWriter.Flush();
                        currentRow++;
                        continue;
                    }
                    if (fields[0].Trim().Equals("\"\"") || fields[0].Trim().Equals(string.Empty))
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_id_missing, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_id_missing_gui, currentRow + 1);
                        wrongFileLines += string.Format( "{0}\n", currentFileLine );
                        logWriter.WriteLine(string.Format(Resources.Messages.err_csv_import_id_missing, currentRow + 1));
                        logWriter.Flush();
                        currentRow++;
                        continue;
                    }
                    if (fields[1].Trim().Equals("\"\"") || fields[1].Trim().Equals(string.Empty))
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_name_missing, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_name_missing_gui, currentRow + 1);
                        wrongFileLines += string.Format( "{0}\n", currentFileLine );
                        logWriter.WriteLine(string.Format(Resources.Messages.err_csv_import_name_missing, currentRow + 1));
                        logWriter.Flush();
                        currentRow++;
                        continue;
                    }
                    agencies.Add(new csvAgency(fields[0].ToString().Replace("\"", string.Empty ), fields[1].ToString().Replace("\"", string.Empty ), fields[2].ToString().Replace("\"", string.Empty )));
                    currentRow++;
                }
                if ( !errorInUploading )
                {
                    logWriter.WriteLine("Andato tutto a buon fine con questo file!");
                }
                else
                {
                    lblImportCsvErrors.Text = wrongRowsMessageForUser;
                    lblImportCsvWrongLines.Text = wrongFileLines;
                    Utils.AppendScript("openP('importCsvErrors',500);");
                }
                logWriter.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                Utils.AppendScript(string.Format("Upload status: The file could not be uploaded. The following error occured: {0}", ex.Message));
            }

            foreach (csvAgency agency in agencies)
            {

                IEnumerable<IAgencyMutableObject> tmpAgencies = (from agen in agencyScheme.Items where agen.Id == agency.agency select agen).OfType<IAgencyMutableObject>();

                IAgencyMutableObject tmpAgency;

                if (!(tmpAgencies.Count() > 0))
                {
                    tmpAgency = new AgencyMutableCore();
                    tmpAgency.Id = agency.agency;
                    tmpAgency.AddName(cmbLanguageForCsv.SelectedValue.ToString(), agency.name);
                    tmpAgency.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), agency.description);
                    agencyScheme.AddItem(tmpAgency);
                }
                else
                {
                    tmpAgency = tmpAgencies.First();
                    tmpAgency.Id = agency.agency;
                    tmpAgency.AddName(cmbLanguageForCsv.SelectedValue.ToString(), agency.name);
                    tmpAgency.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), agency.description);
                }
            }

            if (!SaveInMemory(agencyScheme))
                return;
            BindData();
            if ( !errorInUploading )
            {
                Utils.ShowDialog( Resources.Messages.succ_operation );
            }
            Utils.AppendScript("location.href='#agencies';");
        }

        #endregion

        #region LAYOUT

        private void SetLabelDetail()
        {
            IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();

            if (agencyScheme == null)
                lblAgencySchemeDetail.Text = String.Format("({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
            else
            {

                lblAgencySchemeDetail.Text = String.Format("{3} ({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version, _localizedUtils.GetNameableName(agencyScheme.ImmutableInstance));
            }
        }

        private void BindData(bool isNewItem = false)
        {
            IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();

            if (agencyScheme == null) return;

            SetGeneralTab(agencyScheme.ImmutableInstance);

            LocalizedUtils localUtils = new LocalizedUtils(Utils.LocalizedCulture);
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            IList<Agency> lAgencySchemeItem = new List<Agency>();

            foreach (IAgency agency in agencyScheme.ImmutableInstance.Items)
            {
                lAgencySchemeItem.Add(new Agency(agency.Id, localUtils.GetNameableName(agency), localUtils.GetNameableDescription(agency)));
            }
            
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gvAgencyschemesItem.PageSize = numberOfRows;
            }
            else
            {
                gvAgencyschemesItem.PageSize = Utils.DetailsAgencyschemeGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, lAgencySchemeItem.Count.ToString() );
            gvAgencyschemesItem.DataSource = lAgencySchemeItem;
            gvAgencyschemesItem.DataBind();

            if ( lAgencySchemeItem.Count == 0 )
            {
                txtNumberOfRows.Visible = false;
                lblNumberOfRows.Visible = false;
                btnChangePaging.Visible = false;
                lblNoItemsPresent.Visible = true;
                lblNumberOfTotalElements.Visible = false;
            }
            else
            {
                txtNumberOfRows.Visible = true;
                lblNumberOfRows.Visible = true;
                btnChangePaging.Visible = true;
                lblNoItemsPresent.Visible = false;
                lblNumberOfTotalElements.Visible = true;
            }
        }

        private void SetGeneralTab(IAgencyScheme agencyScheme)
        {
            txtDSDID.Text = agencyScheme.Id;
            txtAgenciesReadOnly.Text = agencyScheme.AgencyId;
            txtVersion.Text = agencyScheme.Version;
            chkIsFinal.Checked = agencyScheme.IsFinal.IsTrue;


            FileDownload31.ucID = agencyScheme.Id;
            FileDownload31.ucAgency = agencyScheme.AgencyId;
            FileDownload31.ucVersion = agencyScheme.Version;
            FileDownload31.ucArtefactType = "AgencyScheme";

            txtDSDURI.Text = (agencyScheme.Uri != null) ? agencyScheme.Uri.AbsoluteUri : string.Empty;
            txtDSDURN.Text = (agencyScheme.Urn != null) ? agencyScheme.Urn.AbsoluteUri : string.Empty;
            txtValidFrom.Text = (agencyScheme.StartDate != null) ? string.Format("{0}/{1}/{2}", agencyScheme.StartDate.Date.Value.Day.ToString(), agencyScheme.StartDate.Date.Value.Month.ToString(), agencyScheme.StartDate.Date.Value.Year.ToString()) : string.Empty;
            txtValidTo.Text = (agencyScheme.EndDate != null) ? string.Format("{0}/{1}/{2}", agencyScheme.EndDate.Date.Value.Day.ToString(), agencyScheme.EndDate.Date.Value.Month.ToString(), agencyScheme.EndDate.Date.Value.Year.ToString()) : string.Empty;
            txtDSDName.Text = _localizedUtils.GetNameableName(agencyScheme);
            txtDSDDescription.Text = _localizedUtils.GetNameableDescription(agencyScheme);
            
            // Svuoto le griglie name e description
            //===========================================
            if (AddTextName.TextObjectList != null && AddTextName.TextObjectList.Count != 0)
            {
                AddTextName.ClearTextObjectList();
            }
            if (AddTextDescription.TextObjectList != null && AddTextDescription.TextObjectList.Count != 0)
            {
                AddTextDescription.ClearTextObjectList();
            }

            txtDSDID.Enabled = false;
            txtVersion.Enabled = false;
            cmbAgencies.Enabled = false;

            if (_action == Action.VIEW || agencyScheme.IsFinal.IsTrue)
            {
                AddTextName.Visible = false;
                AddTextDescription.Visible = false;
                txtAllDescriptions.Visible = true;
                txtAllNames.Visible = true;
                chkIsFinal.Enabled = false;
                txtAllDescriptions.Text = _localizedUtils.GetNameableDescription(agencyScheme);
                txtAllNames.Text = _localizedUtils.GetNameableName(agencyScheme);
            }
            else
            {
                AspConfirmationExit = "true";

                AddTextName.Visible = true;
                AddTextDescription.Visible = true;
                txtAllDescriptions.Visible = false;
                txtAllNames.Visible = false;

                /*
                 * DA IMPLEMENTARE L'ADDING NELLE GRIDS DI MASSIMILIANO 
                 */

                AddTextName.InitTextObjectList = agencyScheme.Names;
                AddTextDescription.InitTextObjectList = agencyScheme.Descriptions;
            }


            AnnotationGeneralControl.AddText_ucOpenTabName = AnnotationGeneralControl.ClientID;
            AnnotationGeneralControl.AnnotationObjectList = agencyScheme.MutableInstance.Annotations;
            AnnotationGeneralControl.EditMode = (agencyScheme.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;
            AnnotationGeneralControl.OwnerAgency =  txtAgenciesReadOnly.Text;
            ctr_annotation_update.EditMode = (agencyScheme.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;

            if (agencyScheme.IsFinal.IsTrue || _action == Action.VIEW)
            {
                txtValidFrom.Enabled = false;
                txtValidTo.Enabled = false;
                txtDSDName.Enabled = false;
                txtDSDDescription.Enabled = false;
                txtDSDURI.Enabled = false;
                chkIsFinal.Enabled = false;
            }
            else
            {
                txtValidFrom.Enabled = true;
                txtValidTo.Enabled = true;
                txtDSDName.Enabled = true;
                txtDSDDescription.Enabled = true;
                txtDSDURI.Enabled = true;
                //chkIsFinal.Enabled = true;
            }

            //===========================================

            if ( _action == Action.INSERT )
            {
                cmbAgencies.Visible = true;
                txtAgenciesReadOnly.Visible = false;
            }
            else
            {
                cmbAgencies.Visible = false;
                txtAgenciesReadOnly.Visible = true;
            }

            SetAgencyDetailPanel(agencyScheme);
        }

        private void SetAgencyDetailPanel(IAgencyScheme agencyScheme)
        {
            // Verifico se la codelist è final
            if (agencyScheme.IsFinal.IsTrue || _action == Action.VIEW)
            {
                // Se final il pulsante di add e le colonne di modifica
                // dei codici non devono apparire
                btnSaveMemoryAgencyScheme.Visible = false;
                btnAddNewAgency.Visible = false;
                AddTextName_Update.ucEditMode = false;
                AddTextDescription_Update.ucEditMode = false;
                AnnotationGeneralControl.EditMode = false;
                btnSaveAnnotationCode.Enabled = false;
                btnUpdateAgency.Enabled = false;
                //gvAgencyschemesItem.Columns[3].Visible = false;
                gvAgencyschemesItem.Columns[4].Visible = false;
                //gvAgencyschemesItem.Columns[5].Visible = false;
                cmbLanguageForCsv.Visible = false;
                imgImportCsv.Visible = false;
            }
            else
            {
                btnSaveMemoryAgencyScheme.Visible = true;
                btnAddNewAgency.Visible = true;
                gvAgencyschemesItem.Columns[3].Visible = true;
                gvAgencyschemesItem.Columns[4].Visible = true;
                gvAgencyschemesItem.Columns[5].Visible = true;
                Utils.PopulateCmbLanguages(cmbLanguageForCsv, AVAILABLE_MODES.MODE_FOR_ADD_TEXT);
                cmbLanguageForCsv.Visible = true;
                imgImportCsv.Visible = true;
            }
        }
        private void SetInsertForm()
        {
            SetEditingControl();
        }
        private void SetEditForm()
        {
            if (!IsPostBack)
            {
                SetLabelDetail();
                btnSaveMemoryAgencyScheme.Visible = true;
                SetEditingControl();
                BindData(true);
                SetSDMXObjects();
            }
            GetSDMXObjects();
        }
        private void SetViewForm()
        {
            if (!Page.IsPostBack)
            {
                SetLabelDetail();
                BindData();
                btnSaveMemoryAgencyScheme.Visible = false;
            }
        }
        private void SetEditingControl()
        {
            //txtDSDID.Enabled = true;
            //txtVersion.Enabled = true;
            txtDSDURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtDSDName.Enabled = true;
            txtDSDDescription.Enabled = true;
            cmbAgencies.Enabled = true;
            //chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;
            btnSaveMemoryAgencyScheme.Visible = true;
            //chkIsFinal.Enabled = true;

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.Visible = true;
            }

            if (Request["ACTION"] == "INSERT" && !Page.IsPostBack)
            {
                AddTextName.ClearTextObjectList();
                AddTextDescription.ClearTextObjectList();
            }
        }
        private void SetInitControls()
        {
            AddTextName.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AgencyScheme;
            AddTextName.TType = TextType.NAME;

            AddTextDescription.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.AgencyScheme;
            AddTextDescription.TType = TextType.DESCRIPTION;

            AddTextName_new.TType = TextType.NAME;
            AddTextDescription_new.TType = TextType.DESCRIPTION;

            AddTextName_Update.TType = TextType.NAME;
            AddTextDescription_Update.TType = TextType.DESCRIPTION;
        }
        private void GetSDMXObjects()
        {
            _sdmxObjects = (ISdmxObjects)Session["SDMXObjexts"];
        }
        private void SetSDMXObjects()
        {
            Session["SDMXObjexts"] = _sdmxObjects;
        }
        #endregion

        protected void btnSaveAnnotationAgency_Click(object sender, EventArgs e)
        {
            IAgencySchemeMutableObject cs = GetAgencySchemeFromSession();

            if (!SaveInMemory(cs)) return;

            BindData();

            Utils.AppendScript("location.href='#agencies';");
        }

        protected void txtNumberOfRows_TextChanged(object sender, EventArgs e)
        {
        }

        protected void btnClearFields_Click(object sender, EventArgs e)
        {
            txt_id_new.Text = string.Empty;
            AddTextName_new.ClearTextObjectListWithOutJS();
            AddTextDescription_new.ClearTextObjectListWithOutJS();
            lblErrorOnNewInsert.Text = string.Empty;
            Utils.AppendScript("location.href= '#agencies';");
        }

        protected void btnClearFieldsForUpdate_Click(object sender, EventArgs e)
        {
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href= '#agencies';");
        }

        protected void btnChangePaging_Click(object sender, EventArgs e)
        {
              IAgencySchemeMutableObject agencyScheme = GetAgencySchemeFromSession();

            if (agencyScheme == null) return;

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.AgencyScheme> lConceptscheme = eMapper.GetAgencySchemeList(_sdmxObjects);
            IList<Agency> lAgencySchemeItem = new List<Agency>();

            foreach (IAgency agency in agencyScheme.ImmutableInstance.Items)
            {
                lAgencySchemeItem.Add(new Agency(agency.Id, _localizedUtils.GetNameableName(agency), _localizedUtils.GetNameableDescription(agency)));
            }

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gvAgencyschemesItem.PageSize = numberOfRows;
                }
                else
                {
                    gvAgencyschemesItem.PageSize = Utils.DetailsAgencyschemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.DetailsAgencyschemeGridNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                Utils.AppendScript( "location.href='#agencies';" );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gvAgencyschemesItem.PageSize = Utils.DetailsAgencyschemeGridNumberRow;
                txtNumberOfRows.Text = Utils.DetailsAgencyschemeGridNumberRow.ToString();
            }
            gvAgencyschemesItem.DataSource = lAgencySchemeItem;
            gvAgencyschemesItem.DataBind();     
            Utils.AppendScript( "location.href='#agencies';" );
        }

        protected void gvAgencyschemesItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           IAgencySchemeMutableObject ags = GetAgencySchemeFromSession();
           if ( e.Row.RowIndex != -1 )
           {
               string agencyId = ((Label)e.Row.Cells[1].Controls[1]).Text;
               IAgencyMutableObject concept = ags.Items.Where( c => c.Id.ToString().Equals( agencyId ) ).First();
               Label lblNumber = (Label)e.Row.Cells[5].Controls[1];
               ImageButton btnImage = (ImageButton)e.Row.Cells[5].Controls[3];
               int numberOfAnnotation = concept.Annotations.Count;
               lblNumber.Text = numberOfAnnotation.ToString();
               if ( numberOfAnnotation == 0 && ( ags.FinalStructure.IsTrue || _action == Action.VIEW ) )
               {
                   btnImage.Enabled = false;
               }
           }
        }

    }

}