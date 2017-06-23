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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{

    public partial class organizationunitschemeItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        public struct csvOrganizationUnit
        {
            public string organizationUnit;
            public string name;
            public string description;
            public string parentOrganizationUnit;

            public csvOrganizationUnit(string organizationUnit, string name, string description, string parentOrganizationUnit)
            {
                this.organizationUnit = organizationUnit;
                this.name = name;
                this.description = description;
                this.parentOrganizationUnit = parentOrganizationUnit;
            }
        }

        public static string KEY_PAGE_SESSION = "TempOrganizationUnitscheme";

        ArtefactIdentity _artIdentity;
        Action _action;
        ISdmxObjects _sdmxObjects;
        LocalizedUtils _localizedUtils;
        
        protected string AspConfirmationExit = "false";

        private void SetAction()
        {
            if (Request["ACTION"] == null || Utils.ViewMode)
                _action = Action.VIEW;
            else
                _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
        }

        private string GetAgencyValue()
        {
            if ( _action == Action.INSERT )
            {
                return cmbAgencies.SelectedValue.ToString();
            }
            else
            {
                return txtAgenciesReadOnly.Text;
            }
        }

        private IOrganisationUnitSchemeMutableObject GetOrganizationUnitschemeForm()
        {
            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region ORGANIZATIONUNITSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region ORGANIZATIONUNITSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME START END DATE
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

            IOrganisationUnitSchemeMutableObject tmpOrganizationUnitscheme = new OrganisationUnitSchemeMutableCore();
            #region CREATE ORGANIZATIONUNITSCHEME FROM FORM

            tmpOrganizationUnitscheme.AgencyId = GetAgencyValue();
            tmpOrganizationUnitscheme.Id = txtDSDID.Text;
            tmpOrganizationUnitscheme.Version = txtVersion.Text;
            tmpOrganizationUnitscheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            tmpOrganizationUnitscheme.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty ) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                tmpOrganizationUnitscheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpOrganizationUnitscheme.StartDate = null;
			}
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                tmpOrganizationUnitscheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpOrganizationUnitscheme.EndDate = null;
			}
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                tmpOrganizationUnitscheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    tmpOrganizationUnitscheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    tmpOrganizationUnitscheme.AddAnnotation(annotation);
                }

            #endregion

            return tmpOrganizationUnitscheme;
        }

        private IOrganisationUnitSchemeMutableObject GetOrganizationUnitschemeForm(IOrganisationUnitSchemeMutableObject ous)
        {

            if (ous == null) return GetOrganizationUnitschemeForm();

            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region ORGANIZATIONUNITSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region ORGANIZATIONUNITSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region ORGANIZATIONUNITSCHEME START END DATE
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

            #region CREATE ORGANIZATIONUNITSCHEME FROM FORM

            ous.AgencyId = GetAgencyValue();
            ous.Id = txtDSDID.Text;
            ous.Version = txtVersion.Text;
            ous.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            ous.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                ous.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				ous.StartDate = null;
			}
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                ous.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				ous.EndDate = null;
			}
            if (ous.Names.Count != 0)
            {
                ous.Names.Clear();
            }
            if (ous.Descriptions.Count != 0)
            {
                ous.Descriptions.Clear();
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                ous.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    ous.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }

            if (ous.Annotations.Count != 0)
            {
                ous.Annotations.Clear();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    ous.AddAnnotation(annotation);
                }

            #endregion

            return ous;
        }

        private IOrganisationUnitSchemeMutableObject InsertOrganizationUnitInOrganizationUnitscheme(IOrganisationUnitSchemeMutableObject ous)
        {
            if (ous == null) return null;

            IOrganisationUnitMutableObject organizationUnit = new OrganisationUnitMutableCore();
            string organization_unit_id = txt_id_new.Text.Trim();

            IList<ITextTypeWrapperMutableObject> organization_unit_names = AddTextName_new.TextObjectList;
            IList<ITextTypeWrapperMutableObject> organization_unit_descs = AddTextDescription_new.TextObjectList;
            string organization_unit_parent_id = txt_parentid_new.Text.Trim();
            // string code_order_str = txtOrderNewCode.Text.Trim();     ----- ORDINE

            #region CONCEPT ID
            if (ValidationUtils.CheckIdFormat(organization_unit_id))
            {
                organizationUnit.Id = organization_unit_id;
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_format;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#organizationunits';");
                return null;
            }

            IEnumerable<IOrganisationUnitMutableObject> organizationunits = (from ou in ous.Items where ou.Id == organization_unit_id select ou).OfType<IOrganisationUnitMutableObject>();
            if ( organizationunits.Count() > 0 )
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_exist;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#organizationunits';");
                return null;
            }
            #endregion

            #region ORGANIZATION UNIT NAMES
            if (organization_unit_names != null)
            {
                foreach (var tmpName in organization_unit_names)
                {
                    organizationUnit.AddName(tmpName.Locale, tmpName.Value);
                }
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_list_name_format;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#organizationunits';");
                return null;
            }
            #endregion

            #region ORGANIZATION UNIT DESCRIPTIONS
            if (organization_unit_descs != null)
            {
                foreach (var tmpDescription in organization_unit_descs)
                {
                    organizationUnit.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            }
            #endregion

            #region PARANT ID

            if ( organization_unit_id.Equals( organization_unit_parent_id ) )
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_parent_id_same_value;
                Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                Utils.AppendScript("location.href= '#organizationunits';");
                return null;
            }

            if (!organization_unit_parent_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(organization_unit_id))
            {
                IEnumerable<IOrganisationUnitMutableObject> parentOrganizationUnit = (from ou in ous.Items where ou.Id == organization_unit_parent_id select ou).OfType<IOrganisationUnitMutableObject>();
                if (parentOrganizationUnit.Count() > 0)
                    organizationUnit.ParentUnit = organization_unit_parent_id;
                else
                {
                    lblErrorOnNewInsert.Text = Resources.Messages.err_parent_id_not_found;
                    Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                    Utils.AppendScript("location.href= '#concepts';");
                    return null;
                }
            }
            #endregion

            ous.Items.Add( organizationUnit );

            try
            {
                // Ultimo controllo se ottengo Immutable istanze validazione completa
                var canRead = ous.ImmutableInstance;
            }
            catch (Exception ex)
            {
                ous.Items.Remove(organizationUnit);

                return null;

            }

            return ous;
        }

        private IOrganisationUnitSchemeMutableObject GetOrganizationUnitSchemeFromSession()
        {
            try
            {
                if (Session[KEY_PAGE_SESSION] == null)
                {
                    if (_artIdentity.ToString() != string.Empty)
                    {
                        WSModel wsModel = new WSModel();
                        ISdmxObjects sdmxObject = wsModel.GetOrganisationUnitScheme(_artIdentity, false,false);
                        IOrganisationUnitSchemeObject ous = sdmxObject.OrganisationUnitSchemes.FirstOrDefault();
                        Session[KEY_PAGE_SESSION] = ous.MutableInstance;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                return (IOrganisationUnitSchemeMutableObject)Session[KEY_PAGE_SESSION];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveInMemory(IOrganisationUnitSchemeMutableObject ous)
        {
            if (ous == null) return false;

            Session[KEY_PAGE_SESSION] = ous;

            return true;
        }

        private bool SendQuerySubmit(IOrganisationUnitSchemeMutableObject ous)
        {

            try
            {
                ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

                sdmxObjects.AddOrganisationUnitScheme(ous.ImmutableInstance);

                WSModel modelOrganizationUnitScheme = new WSModel();

                XmlDocument result = modelOrganizationUnitScheme.SubmitStructure(sdmxObjects);

                Utils.GetXMLResponseError(result);

                return true;

            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Source + " - " + ex.Message, 300, "IR Error"); 
            }

            return false;
        }

        private void ClearSessionPage()
        {
            Session[KEY_PAGE_SESSION] = null;
        }

        private int CurrentOrganizationUnitIndex { get { return (int)Session[KEY_PAGE_SESSION + "_index_organization_unit"]; } set { Session[KEY_PAGE_SESSION + "_index_organization_unit"] = value; } }

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
            lbl_parentid_new.DataBind();
            btnNewConcept.DataBind();
            lbl_title_update.DataBind();
            lbl_id_update.DataBind();
            lbl_name_update.DataBind();
            lbl_description_update.DataBind();
            lbl_parentid_update.DataBind();
            btnUpdateOrganizationUnit.DataBind();
            btnAddNewOrganizationUnit.DataBind();
            btnSaveMemoryOrganizationUnitScheme.DataBind();
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
                txtNumberOfRows.Text = Utils.DetailsOrganizationUnitSchemeGridNumberRow.ToString();
                AnnotationGeneralControl.ClearAnnotationsSession();
            }

            switch (_action)
            {
                case Action.INSERT:

                    //ClearSessionPage();
                    AspConfirmationExit = "true";

                    SetInitControls();
                    SetInsertForm();

                    lblNumberOfRows.Visible = false;
                    txtNumberOfRows.Visible = false;
                    btnChangePaging.Visible = false;

                    chkIsFinal.Checked = false;
                    chkIsFinal.Enabled = false;

                    AddTextName_Update.ucOpenTabName = "organizationunits";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "organizationunits";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "organizationunits";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "organizationunits";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-Dimension";
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

                    AddTextName_Update.ucOpenTabName = "organizationunits";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "organizationunits";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "organizationunits";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "organizationunits";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-Dimension";

                    /*if (gvConceptschemesItem.Rows.Count > 0)
                        chkIsFinal.Enabled = true;
                    else
                        chkIsFinal.Enabled = false;*/

                    break;
                case Action.VIEW:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);
                    ClearSessionPage();
                    SetViewForm();
                    AddTextName_Update.ucOpenTabName = "organizationunits";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "organizationunits";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "organizationunits";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "organizationunits";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-Dimension";
                    /*
                    FileDownload31.ucID = _artIdentity.ID;
                    FileDownload31.ucAgency = _artIdentity.Agency;
                    FileDownload31.ucVersion = _artIdentity.Version;
                    FileDownload31.ucArtefactType = "OrganizationUnitScheme";
                     * */
                    break;
            }

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.OrganisationUnitScheme;
            DuplicateArtefact1.ucMaintanableArtefact = GetOrganizationUnitSchemeFromSession();


        }

        protected void gvOrganizationunitschemesItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrganizationunitschemesItem.PageSize = 12;
            gvOrganizationunitschemesItem.PageIndex = e.NewPageIndex;
            BindData();
            Utils.AppendScript("location.href= '#organizationunits';");
        }

        protected void btnSaveMemoryOrganizationUnitScheme_Click(object sender, EventArgs e)
        {

            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();
            if (ous == null) ous = GetOrganizationUnitschemeForm();
            else ous = GetOrganizationUnitschemeForm(ous);

            if (!SaveInMemory(ous)) return;

            if (!SendQuerySubmit(ous)) return;

            BindData();

            string successMessage = string.Empty;
            if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.INSERT)
            {
                successMessage = Resources.Messages.succ_organization_unit_insert;
            }
            else if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.UPDATE)
            {
                successMessage = Resources.Messages.succ_organization_unit_update;
            }
            Utils.ShowDialog(successMessage, 300, Resources.Messages.succ_operation);
            if ( _action == Action.INSERT )
            {
                Utils.AppendScript( "~/organizationunitschemes.aspx" );
            }
        }

        protected void btnAddNewOrganizationUnit_Click(object sender, EventArgs e)
        {

            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

            ous = GetOrganizationUnitschemeForm(ous);

            // form codelist validation
            if (ous == null) 
            {
                txt_id_new.Text = string.Empty;
                txt_parentid_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                return;
            }
            ous = InsertOrganizationUnitInOrganizationUnitscheme(ous);

            if (ous == null)
            {
                txt_id_new.Text = string.Empty;
                txt_parentid_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                Utils.ShowDialog(Resources.Messages.err_organization_unit_insert, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href= '#organizationunits';");
                return;
            }

            if (!SaveInMemory(ous)) 
            {
                txt_id_new.Text = string.Empty;
                txt_parentid_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                return;
            }
            BindData();

            txt_id_new.Text = string.Empty;
            txt_parentid_new.Text = string.Empty;
            AddTextName_new.ClearTextObjectListWithOutJS();
            AddTextDescription_new.ClearTextObjectListWithOutJS();
            lblErrorOnNewInsert.Text = string.Empty;
            Utils.AppendScript("location.href='#organizationunits';");
        }

        protected void btnUpdateOrganizationUnit_Click(object sender, EventArgs e)
        {

            // Get Input field
            string organization_unit_id = txt_id_update.Text.Trim();
            IList<ITextTypeWrapperMutableObject> organization_unit_names = AddTextName_Update.TextObjectList;
            IList<ITextTypeWrapperMutableObject> organization_unit_descs = AddTextDescription_Update.TextObjectList;
            string organization_unit_parent_id = txt_parentid_update.Text.Trim();
            // string code_order_str = txtUpdateCodeOrder.Text.Trim();


            // Get Current Object Session
            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();
            IEnumerable<IOrganisationUnitMutableObject> _rc = (from x in ous.Items where x.Id == organization_unit_id select x).OfType<IOrganisationUnitMutableObject>();
            if (_rc.Count() == 0) return;

            IOrganisationUnitMutableObject organizationUnit = _rc.First();

            IOrganisationUnitMutableObject _bOrganizationUnit = new OrganisationUnitMutableCore();

            int indexOrganisationUnit = ous.Items.IndexOf(organizationUnit);
            int indexOrder = 0;
            try
            {

                #region ORGANIZATION UNIT ID
                if (!organization_unit_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(organization_unit_id))
                {
                    _bOrganizationUnit.Id = organization_unit_id;
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_id_format;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#organizationunits';");
                    return;
                }
                #endregion

                #region CONCEPT NAMES
                if (organization_unit_names != null)
                {
                    foreach (var tmpName in organization_unit_names)
                    {
                        _bOrganizationUnit.AddName(tmpName.Locale, tmpName.Value);
                    }
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_list_name_format;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#organizationunits';");
                    return;
                }
                #endregion

                #region ORGANIZATION UNIT DESCRIPTIONS
                if (organization_unit_descs != null)
                {
                    foreach (var tmpDescription in organization_unit_descs)
                    {
                        _bOrganizationUnit.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                    }
                }
                #endregion

                #region PARANT ID

                if ( organization_unit_id.Equals( organization_unit_parent_id ) )
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_parent_id_same_value;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#organizationunits';");
                    return;
                }

                if (!organization_unit_parent_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(organization_unit_id))
                {
                    //IEnumerable<ICodeMutableObject> parentCode = (from c in cl.Items where c.Id == code_parent_id select c).OfType<ICodeMutableObject>();

                    IEnumerable<IOrganisationUnitMutableObject> parentConcept = (from c in ous.Items where c.Id == organization_unit_parent_id select c).OfType<IOrganisationUnitMutableObject>();
                    if (parentConcept.Count() > 0)
                        _bOrganizationUnit.ParentUnit = organization_unit_parent_id;
                    else
                    {

                        lblErrorOnUpdate.Text = Resources.Messages.err_parent_id_not_found;
                        Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                        Utils.AppendScript("location.href= '#organizationunits';");
                        return;
                    }
                }
                #endregion

                ous.Items.Remove(organizationUnit);
                ous.Items.Insert(indexOrganisationUnit, _bOrganizationUnit);

                var canRead = ous.ImmutableInstance;

            }
            catch (Exception ex) // ERRORE GENERICO!
            {
                ous.Items.Remove(_bOrganizationUnit);
                ous.Items.Insert(indexOrganisationUnit, organizationUnit);

                if ( ex.Message.Contains( "- 706 -" ) )
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_parent_item_is_child;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600);" );
                 }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_organization_unit_update;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600);" );
                }
                Utils.AppendScript("location.href='#organizationunits';");
                return;
            }

            if (!SaveInMemory(ous))
                return;

            BindData();
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href='#organizationunits';");
        }

        protected void gvOrganizationunitschemesItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "UPDATE":
                    {
                        // Svuoto i controlli
                        // ---------------------------------
                        txt_id_update.Text = string.Empty;
                        //txtUpdateCodeOrder.Text = string.Empty;
                        txt_parentid_update.Text = string.Empty;
                        AddTextName_Update.ClearTextObjectList();
                        AddTextDescription_Update.ClearTextObjectList();
                        // ---------------------------------

                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        txt_id_update.Text = ((Label)gvr.Cells[1].Controls[1]).Text;
                        txt_parentid_update.Text = ((Label)gvr.Cells[3].Controls[1]).Text;


                        IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > ous.ImmutableInstance.Items.Count) return;

                        if ( gvOrganizationunitschemesItem.PageIndex > 0 )
                        {
                            CurrentOrganizationUnitIndex = gvr.RowIndex + ( gvOrganizationunitschemesItem.PageIndex * gvOrganizationunitschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentOrganizationUnitIndex = gvr.RowIndex;
                        }

                        IOrganisationUnit currentOrganizationUnit = ((IOrganisationUnit)ous.ImmutableInstance.Items[CurrentOrganizationUnitIndex]);

                        AddTextName_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationUnit;
                        AddTextName_Update.TType = TextType.NAME;
                        AddTextName_Update.ClearTextObjectList();
                        AddTextName_Update.InitTextObjectList = currentOrganizationUnit.Names;

                        AddTextDescription_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationUnit;
                        AddTextDescription_Update.TType = TextType.DESCRIPTION;
                        AddTextDescription_Update.ClearTextObjectList();
                        AddTextDescription_Update.InitTextObjectList = currentOrganizationUnit.Descriptions;
                        // txtUpdateCodeOrder.Text = (gvr.RowIndex + 1).ToString();
                        break;
                    }
                case "DELETE":
                    {

                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);

                        IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > ous.Items.Count) return;

                        bool canDelete = true;
                        int selectedRecordCount = gvOrganizationunitschemesItem.PageSize * gvOrganizationunitschemesItem.PageIndex + gvr.RowIndex;

                        var parent_organization_unit = ous.Items[selectedRecordCount].Id;

                        #region PARENT ID
                        if (parent_organization_unit != null)
                        {
                            IEnumerable<IOrganisationUnitMutableObject> parentOrganizationUnit = (from c in ous.Items where c.ParentUnit == parent_organization_unit select c).OfType<IOrganisationUnitMutableObject>();
                            if (parentOrganizationUnit.Count() > 0)
                            {
                                Utils.ShowDialog(Resources.Messages.err_organization_unit_is_parent, 300, Resources.Messages.err_title);
                                Utils.AppendScript("location.href= '#organizationunits';");
                                canDelete = false;
                            }
                        }
                        #endregion

                        if (canDelete)
                        {
                            ous.Items.RemoveAt(selectedRecordCount);
                            Session[KEY_PAGE_SESSION] = ous;
                            BindData();
                        }

                        Utils.AppendScript("location.href='#organizationunits'");

                    }
                    break;
                case "ANNOTATION":
                    {
                        IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();
                        if ( ous == null )
                        {
                            ous = GetOrganizationUnitschemeForm( ous );
                        }
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);

                        if (gvr.RowIndex < 0 && gvr.RowIndex > ous.Items.Count) return;

                        if ( gvOrganizationunitschemesItem.PageIndex > 0 )
                        {
                            CurrentOrganizationUnitIndex = gvr.RowIndex + ( gvOrganizationunitschemesItem.PageIndex * gvOrganizationunitschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentOrganizationUnitIndex = gvr.RowIndex;
                        }

                        ctr_annotation_update.EditMode = !Utils.ViewMode;
                        btnSaveAnnotationCode.Visible = !Utils.ViewMode;

                        ctr_annotation_update.AnnotationObjectList = ous.Items[CurrentOrganizationUnitIndex].Annotations;

                        Utils.AppendScript("openP('organization_unit_annotation',650);");

                        Utils.AppendScript("location.href= '#organizationunits';");

                    } break;


            }

        }
        protected void gvOrganizationunitschemesItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // NULL
        }
        protected void gvOrganizationunitschemesItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // NULL
        }
        protected void gvOrganizationunitschemesItem_Sorting(object sender, GridViewSortEventArgs e)
        {

        }
        protected void gvOrganizationunitschemesItem_Sorted(object sender, EventArgs e)
        {
            // NULL
        }
        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {
            if ( !csvFile.HasFile )
            {
                Utils.AppendScript( "openPopUp( 'importCsv' );" );
                Utils.AppendScript( "location.href='#organizationunits'" );
                Utils.AppendScript( string.Format( "alert( '{0}' );", Resources.Messages.err_no_file_uploaded ) );
                return;
            }

            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

            if (ous == null) return;

            List<csvOrganizationUnit> organizationUnits = new List<csvOrganizationUnit>();
            bool errorInUploading = false;
            try
            {
                string filenameWithoutExtension = string.Format("{0}_{1}_{2}", Path.GetFileName(csvFile.FileName).Substring(0, csvFile.FileName.Length - 4), Session.SessionID, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_'));
                string filename = string.Format("{0}.csv", filenameWithoutExtension);
                string logFilename = string.Format("{0}.log", filenameWithoutExtension);
                csvFile.SaveAs(Server.MapPath("~/csv_organizationunitschemes_files/") + filename);

                StreamReader reader = new StreamReader(Server.MapPath("~/csv_organizationunitschemes_files/") + filename);
                StreamWriter logWriter = new StreamWriter(Server.MapPath("~/csv_organizationunitschemes_import_logs/") + logFilename, true);
                logWriter.WriteLine(string.Format("LOG RELATIVO A CARICAMENTO DELL\'ORGANIZATION UNIT SCHEME [ ID => \"{0}\"  AGENCY_ID => \"{1}\"  VERSION => \"{2}\" ]  |  LINGUA SELEZIONATA: {3}\n", ous.Id.ToString(), ous.AgencyId.ToString(), ous.Version.ToString(), cmbLanguageForCsv.SelectedValue.ToString()));
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
                    if (fields.Length != 4)
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format( Resources.Messages.err_csv_import_line_bad_format, currentRow + 1);
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
                    organizationUnits.Add(new csvOrganizationUnit(fields[0].ToString().Replace( "\"", "" ), fields[1].ToString().Replace( "\"", "" ), fields[2].ToString().Replace( "\"", "" ), fields[3].ToString().Replace( "\"", "" )));
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

            foreach (csvOrganizationUnit organizationUnit in organizationUnits)
            {

                IEnumerable<IOrganisationUnitMutableObject> tmpOrganisationUnits = (from conc in ous.Items where conc.Id == organizationUnit.organizationUnit select conc).OfType<IOrganisationUnitMutableObject>();

                IOrganisationUnitMutableObject tmpOrganizationUnit;

                if (!(tmpOrganisationUnits.Count() > 0))
                {
                    tmpOrganizationUnit = new OrganisationUnitMutableCore();
                    tmpOrganizationUnit.Id = organizationUnit.organizationUnit;
                    tmpOrganizationUnit.ParentUnit = organizationUnit.parentOrganizationUnit;
                    tmpOrganizationUnit.AddName(cmbLanguageForCsv.SelectedValue.ToString(), organizationUnit.name);
                    tmpOrganizationUnit.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), organizationUnit.description);

                    ous.AddItem(tmpOrganizationUnit);
                }
                else
                {
                    tmpOrganizationUnit = tmpOrganisationUnits.First();
                    tmpOrganizationUnit.Id = organizationUnit.organizationUnit;
                    tmpOrganizationUnit.ParentUnit = organizationUnit.parentOrganizationUnit;
                    tmpOrganizationUnit.AddName(cmbLanguageForCsv.SelectedValue.ToString(), organizationUnit.name);
                    tmpOrganizationUnit.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), organizationUnit.description);
                }
            }

            if (!SaveInMemory(ous))
                return;

            BindData();
            if ( !errorInUploading )
            {
                Utils.ShowDialog( Resources.Messages.succ_operation );
            }
            Utils.AppendScript("location.href='#organizationunits';");
        }

        #endregion

        #region LAYOUT

        private void SetLabelDetail()
        {

            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

            if (ous == null)
                lblOrganizationUnitSchemeDetail.Text = String.Format("({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
            else
            {
                lblOrganizationUnitSchemeDetail.Text = String.Format("{3} ({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version, _localizedUtils.GetNameableName(ous.ImmutableInstance));
            }
        }

        private void BindData(bool isNewItem = false)
        {
            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

            if (ous == null) return;

            SetGeneralTab(ous.ImmutableInstance);

            LocalizedUtils localUtils = new LocalizedUtils(Utils.LocalizedCulture);
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            IList<OrganizationUnit> lOrganizationUnitSchemeItem = new List<OrganizationUnit>();

            foreach (IOrganisationUnit organizationUnit in ous.ImmutableInstance.Items)
            {
                lOrganizationUnitSchemeItem.Add(new OrganizationUnit(organizationUnit.Id, localUtils.GetNameableName(organizationUnit), localUtils.GetNameableDescription(organizationUnit), organizationUnit.ParentUnit));
            }
            
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gvOrganizationunitschemesItem.PageSize = numberOfRows;
            }
            else
            {
                gvOrganizationunitschemesItem.PageSize = Utils.DetailsOrganizationUnitSchemeGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, lOrganizationUnitSchemeItem.Count.ToString() );
            
            gvOrganizationunitschemesItem.DataSource = lOrganizationUnitSchemeItem;
            gvOrganizationunitschemesItem.DataBind();

            if ( lOrganizationUnitSchemeItem.Count == 0 )
            {
                txtNumberOfRows.Visible = false;
                lblNumberOfRows.Visible = false;
                btnChangePaging.Visible = false;
            }
            else
            {
                txtNumberOfRows.Visible = true;
                lblNumberOfRows.Visible = true;
                btnChangePaging.Visible = true;
            }
        }

        private void SetGeneralTab(IOrganisationUnitSchemeObject ous)
        {
            txtDSDID.Text = ous.Id;
            txtAgenciesReadOnly.Text = ous.AgencyId;
            txtVersion.Text = ous.Version;
            chkIsFinal.Checked = ous.IsFinal.IsTrue;

            FileDownload31.ucID = ous.Id;
            FileDownload31.ucAgency = ous.AgencyId;
            FileDownload31.ucVersion = ous.Version;
            FileDownload31.ucArtefactType = "OrganizationUnitScheme";

            txtDSDURI.Text = (ous.Uri != null) ? ous.Uri.AbsoluteUri : string.Empty;
            txtDSDURN.Text = (ous.Urn != null) ? ous.Urn.AbsoluteUri : string.Empty;
            txtValidFrom.Text = (ous.StartDate != null) ? string.Format("{0}/{1}/{2}", ous.StartDate.Date.Value.Day.ToString(), ous.StartDate.Date.Value.Month.ToString(), ous.StartDate.Date.Value.Year.ToString()) : string.Empty;
            txtValidTo.Text = (ous.EndDate != null) ? string.Format("{0}/{1}/{2}", ous.EndDate.Date.Value.Day.ToString(), ous.EndDate.Date.Value.Month.ToString(), ous.EndDate.Date.Value.Year.ToString()) : string.Empty;
            txtDSDName.Text = _localizedUtils.GetNameableName(ous);
            txtDSDDescription.Text = _localizedUtils.GetNameableDescription(ous);

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

            if (_action == Action.VIEW || ous.IsFinal.IsTrue)
            {
                AddTextName.Visible = false;
                AddTextDescription.Visible = false;
                txtAllDescriptions.Visible = true;
                txtAllNames.Visible = true;
                chkIsFinal.Enabled = false;
                txtAllDescriptions.Text = _localizedUtils.GetNameableDescription(ous);
                txtAllNames.Text = _localizedUtils.GetNameableName(ous);
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

                AddTextName.InitTextObjectList = ous.Names;
                AddTextDescription.InitTextObjectList = ous.Descriptions;
            }

            if ( _action != Action.VIEW )
            {
                DuplicateArtefact1.Visible = true;
            }

            AnnotationGeneralControl.AddText_ucOpenTabName = AnnotationGeneralControl.ClientID;
            AnnotationGeneralControl.AnnotationObjectList = ous.MutableInstance.Annotations;
            AnnotationGeneralControl.EditMode = (ous.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;
            AnnotationGeneralControl.OwnerAgency = txtAgenciesReadOnly.Text;

            if (ous.IsFinal.IsTrue || _action == Action.VIEW)
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

            SetCodeDetailPanel(ous);
        }

        private void SetCodeDetailPanel(IOrganisationUnitSchemeObject ous)
        {
            // Verifico se la codelist è final
            if (ous.IsFinal.IsTrue || _action == Action.VIEW)
            {
                // Se final il pulsante di add e le colonne di modifica
                // dei codici non devono apparire
                btnSaveMemoryOrganizationUnitScheme.Visible = false;
                btnAddNewOrganizationUnit.Visible = false;
                AddTextName_Update.ucEditMode = false;
                AddTextDescription_Update.ucEditMode = false;
                AnnotationGeneralControl.EditMode = false;
                btnSaveAnnotationCode.Enabled = false;
                btnUpdateOrganizationUnit.Enabled = false;
                txt_parentid_update.Enabled = false;
                //gvOrganizationunitschemesItem.Columns[3].Visible = false;
                //gvOrganizationunitschemesItem.Columns[4].Visible = false;
                gvOrganizationunitschemesItem.Columns[5].Visible = false;
                //cmbLanguageForCsv.Visible = false;
                //imgImportCsv.Visible = false;
            }
            else
            {
                btnSaveMemoryOrganizationUnitScheme.Visible = true;
                btnAddNewOrganizationUnit.Visible = true;
                gvOrganizationunitschemesItem.Columns[3].Visible = true;
                gvOrganizationunitschemesItem.Columns[4].Visible = true;
                gvOrganizationunitschemesItem.Columns[5].Visible = true;
                //Utils.PopulateCmbLanguages(cmbLanguageForCsv, AVAILABLE_MODES.MODE_FOR_ADD_TEXT);
                //cmbLanguageForCsv.Visible = true;
                //imgImportCsv.Visible = true;
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
                btnSaveMemoryOrganizationUnitScheme.Visible = true;
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
                btnSaveMemoryOrganizationUnitScheme.Visible = false;
            }
        }
        private void SetEditingControl()
        {
            txtDSDID.Enabled = true;
            txtVersion.Enabled = true;
            txtDSDURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtDSDName.Enabled = true;
            txtDSDDescription.Enabled = true;
            cmbAgencies.Enabled = true;
            //chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;
            btnSaveMemoryOrganizationUnitScheme.Visible = true;

            // Svuoto le griglie di name e description
            if (Request["ACTION"] == "INSERT" && !Page.IsPostBack)
            {
                AddTextName.ClearTextObjectList();
                AddTextDescription.ClearTextObjectList();
            }
        }

        private void SetInitControls()
        {
            AddTextName.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationUnitScheme;
            AddTextName.TType = TextType.NAME;

            AddTextDescription.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.OrganisationUnitScheme;
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

        protected void btnSaveAnnotationOrganizationUnit_Click(object sender, EventArgs e)
        {
            IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

            if (!SaveInMemory(ous)) return;

            BindData();

            Utils.AppendScript("location.href='#organizationunits';");
        }

        protected void txtNumberOfRows_TextChanged(object sender, EventArgs e)
        {
        }

        protected void btnClearFields_Click(object sender, EventArgs e)
        {
            txt_id_new.Text = string.Empty;
            AddTextName_new.ClearTextObjectListWithOutJS();
            AddTextDescription_new.ClearTextObjectListWithOutJS();
            txt_parentid_new.Text = string.Empty;
            lblErrorOnNewInsert.Text = string.Empty;
            Utils.AppendScript("location.href= '#organizationunits';");
        }

        protected void btnClearFieldsForUpdate_Click(object sender, EventArgs e)
        {
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href= '#organizationunits';");
        }

        protected void btnChangePaging_Click(object sender, EventArgs e)
        {
              IOrganisationUnitSchemeMutableObject ous = GetOrganizationUnitSchemeFromSession();

            if (ous == null) return;

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.OrganizationUnitScheme> lOrganizationUnitscheme = eMapper.GetOrganizationUnitSchemeList(_sdmxObjects);
            IList<OrganizationUnit> lOrganizationUnitSchemeItem = new List<OrganizationUnit>();

            foreach (IOrganisationUnit organizationUnit in ous.ImmutableInstance.Items)
            {
                lOrganizationUnitSchemeItem.Add(new OrganizationUnit(organizationUnit.Id, _localizedUtils.GetNameableName(organizationUnit), _localizedUtils.GetNameableDescription(organizationUnit), organizationUnit.ParentUnit));
            }

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gvOrganizationunitschemesItem.PageSize = numberOfRows;
                }
                else
                {
                    gvOrganizationunitschemesItem.PageSize = Utils.DetailsOrganizationUnitSchemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.DetailsOrganizationUnitSchemeGridNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                Utils.AppendScript( "location.href='#organizationunits';" );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gvOrganizationunitschemesItem.PageSize = Utils.DetailsOrganizationUnitSchemeGridNumberRow;
                txtNumberOfRows.Text = Utils.DetailsOrganizationUnitSchemeGridNumberRow.ToString();
            }
            gvOrganizationunitschemesItem.DataSource = lOrganizationUnitSchemeItem;
            gvOrganizationunitschemesItem.DataBind();     
            Utils.AppendScript( "location.href='#organizationunits';" );
        }

        protected void gvOrganizationunitschemesItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            IOrganisationUnitSchemeMutableObject OrgUnitScheme = GetOrganizationUnitSchemeFromSession();

            if (e.Row.RowIndex != -1)
            {
                string oiId = ((Label)e.Row.FindControl("lblID")).Text;

                IOrganisationUnitMutableObject OrgUnit = OrgUnitScheme.Items.Where(d => d.Id != null && d.Id.ToString().Equals(oiId)).FirstOrDefault();

                if (OrgUnit == null)
                    return;

                Label lblNumber = (Label)e.Row.FindControl("lblNumberOfAnnotation");
                ImageButton btnImage = (ImageButton)e.Row.FindControl("img_annotation");

                int numberOfAnnotation = OrgUnit.Annotations.Where(ann => ann.Id != null).Count();
                lblNumber.Text = numberOfAnnotation.ToString();
                if (numberOfAnnotation == 0 && _action == Action.VIEW)
                {
                    btnImage.Enabled = false;
                }
            }
        }

    }

}