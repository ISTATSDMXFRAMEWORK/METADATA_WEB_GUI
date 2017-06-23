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

    public partial class dataproviderschemeItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        public struct csvDataProvider
        {
            public string dataProvider;
            public string name;
            public string description;

            public csvDataProvider(string dataProvider, string name, string description)
            {
                this.dataProvider = dataProvider;
                this.name = name;
                this.description = description;
              }
        }

        public static string KEY_PAGE_SESSION = "TempDataProviderscheme";

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
                    if (currentUser != null)
                    {
                        int agencyOccurence = currentUser.agencies.Count(agency => agency.id.Equals(_artIdentity.Agency));
                        if (agencyOccurence > 0)
                        {
                            _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                        }
                        else
                        {
                            _action = Action.VIEW;
                        }
                    }
                    else
                        _action = Action.VIEW;
                }
                else
                {
                    _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                }
            }
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

        private IDataProviderSchemeMutableObject GetDataProviderschemeForm()
        {
            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region DATAPROVIDERSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region DATAPROVIDERSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME START END DATE
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

            IDataProviderSchemeMutableObject tmpDataProviderscheme = new DataProviderSchemeMutableCore();
            #region CREATE DATAPROVIDERSCHEME FROM FORM

            tmpDataProviderscheme.AgencyId = GetAgencyValue();
            tmpDataProviderscheme.Id = txtDSDID.Text;
            tmpDataProviderscheme.Version = txtVersion.Text;
            tmpDataProviderscheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            tmpDataProviderscheme.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty ) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                tmpDataProviderscheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpDataProviderscheme.StartDate = null;
			}
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                tmpDataProviderscheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpDataProviderscheme.EndDate = null;
			}
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                tmpDataProviderscheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    tmpDataProviderscheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    tmpDataProviderscheme.AddAnnotation(annotation);
                }

            #endregion

            return tmpDataProviderscheme;
        }

        private IDataProviderSchemeMutableObject GetDataProviderschemeForm(IDataProviderSchemeMutableObject dataProviderScheme)
        {

            if (dataProviderScheme == null) return GetDataProviderschemeForm();

            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region DATAPROVIDERSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region DATAPROVIDERSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATAPROVIDERSCHEME START END DATE
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

            #region CREATE DATAPROVIDERSCHEME FROM FORM

            dataProviderScheme.AgencyId = GetAgencyValue();
            dataProviderScheme.Id = txtDSDID.Text;
            dataProviderScheme.Version = txtVersion.Text;
            dataProviderScheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            dataProviderScheme.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty ) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                dataProviderScheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				dataProviderScheme.StartDate = null;
			}
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                dataProviderScheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				dataProviderScheme.EndDate = null;
			}
            if (dataProviderScheme.Names.Count != 0)
            {
                dataProviderScheme.Names.Clear();
            }
            if (dataProviderScheme.Descriptions.Count != 0)
            {
                dataProviderScheme.Descriptions.Clear();
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                dataProviderScheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    dataProviderScheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }

            if (dataProviderScheme.Annotations.Count != 0)
            {
                dataProviderScheme.Annotations.Clear();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    dataProviderScheme.AddAnnotation(annotation);
                }

            #endregion

            return dataProviderScheme;
        }

        private IDataProviderSchemeMutableObject InsertDataProviderInDataProviderscheme(IDataProviderSchemeMutableObject dataProviderScheme)
        {
            if (dataProviderScheme == null) return null;

            IDataProviderMutableObject dataProvider = new DataProviderMutableCore();

            string data_provider_id = txt_id_new.Text.Trim();

            IList<ITextTypeWrapperMutableObject> data_provider_names = AddTextName_new.TextObjectList;
            IList<ITextTypeWrapperMutableObject> data_provider_descs = AddTextDescription_new.TextObjectList;
            // string code_order_str = txtOrderNewCode.Text.Trim();     ----- ORDINE

            #region DATAPROVIDER ID
            if (ValidationUtils.CheckIdFormat(data_provider_id))
            {
                dataProvider.Id = data_provider_id;
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_format;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#dataproviders';");
                return null;
            }

            IEnumerable<IDataProviderMutableObject> dataProviders = (from c in dataProviderScheme.Items where c.Id == data_provider_id select c).OfType<IDataProviderMutableObject>();
            if (dataProviders.Count() > 0)
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_exist;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#dataproviders';");
                return null;
            }
            #endregion

            #region DATAPROVIDER NAMES
            if (data_provider_names != null)
            {
                foreach (var tmpName in data_provider_names)
                {
                    dataProvider.AddName(tmpName.Locale, tmpName.Value);
                }
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_list_name_format;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#dataproviders';");
                return null;
            }
            #endregion

            #region DATAPROVIDER DESCRIPTIONS
            if (data_provider_descs != null)
            {
                foreach (var tmpDescription in data_provider_descs)
                {
                    dataProvider.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            }
            #endregion

            dataProviderScheme.Items.Add(dataProvider);

            try
            {
                // Ultimo controllo se ottengo Immutable istanze validazione completa
                var canRead = dataProviderScheme.ImmutableInstance;
            }
            catch (Exception ex)
            {
                dataProviderScheme.Items.Remove(dataProvider);

                return null;

            }
            return dataProviderScheme;
        }

        private IDataProviderSchemeMutableObject GetDataProviderSchemeFromSession()
        {
            try
            {
                if (Session[KEY_PAGE_SESSION] == null)
                {
                    if (_artIdentity.ToString() != string.Empty)
                    {
                        WSModel wsModel = new WSModel();
                        ISdmxObjects sdmxObject = wsModel.GetDataProviderScheme(_artIdentity, false,false);
                        IDataProviderScheme dataProviderScheme = sdmxObject.DataProviderSchemes.FirstOrDefault();
                        Session[KEY_PAGE_SESSION] = dataProviderScheme.MutableInstance;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                return (IDataProviderSchemeMutableObject)Session[KEY_PAGE_SESSION];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveInMemory(IDataProviderSchemeMutableObject dataProviderScheme)
        {
            if (dataProviderScheme == null) return false;

            Session[KEY_PAGE_SESSION] = dataProviderScheme;

            return true;
        }

        private bool SendQuerySubmit(IDataProviderSchemeMutableObject dataProviderScheme)
        {

            try
            {

                ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

                sdmxObjects.AddDataProviderScheme(dataProviderScheme.ImmutableInstance);

                WSModel modelDataProviderScheme = new WSModel();

                XmlDocument result = modelDataProviderScheme.SubmitStructure(sdmxObjects);

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

        private int CurrentDataProviderIndex { get { return (int)Session[KEY_PAGE_SESSION + "_index_data_provider"]; } set { Session[KEY_PAGE_SESSION + "_index_data_provider"] = value; } }

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
            btnNewDataProvider.DataBind();
            lbl_title_update.DataBind();
            lbl_id_update.DataBind();
            lbl_name_update.DataBind();
            lbl_description_update.DataBind();
            btnUpdateDataProvider.DataBind();
            btnAddNewDataProvider.DataBind();
            btnSaveMemoryDataProviderScheme.DataBind();
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
                txtNumberOfRows.Text = Utils.DetailsDataProviderschemeGridNumberRow.ToString();
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

                    /*chkIsFinal.Checked = false;
                    chkIsFinal.Enabled = false;*/

                    AddTextName_Update.ucOpenTabName = "dataproviders";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "dataproviders";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "dataproviders";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "dataproviders";
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

                    AddTextName_Update.ucOpenTabName = "dataproviders";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "dataproviders";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "dataproviders";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "dataproviders";
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
                    AddTextName_Update.ucOpenTabName = "dataproviders";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "dataproviders";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "dataproviders";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "dataproviders";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-Dimension";
                    /*FileDownload31.ucID = _artIdentity.ID;
                    FileDownload31.ucAgency = _artIdentity.Agency;
                    FileDownload31.ucVersion = _artIdentity.Version;
                    FileDownload31.ucArtefactType = "DataProviderScheme";*/

                    break;
            }

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.DataProviderScheme;
            DuplicateArtefact1.ucMaintanableArtefact = GetDataProviderSchemeFromSession();



        }

        protected void gvDataProviderschemesItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDataProviderschemesItem.PageSize = 12;
            gvDataProviderschemesItem.PageIndex = e.NewPageIndex;
            BindData();
            Utils.AppendScript("location.href= '#dataproviders';");
        }

        protected void btnSaveMemoryDataProviderScheme_Click(object sender, EventArgs e)
        {
            IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();
            if (dataProviderScheme == null) dataProviderScheme = GetDataProviderschemeForm();
            else dataProviderScheme = GetDataProviderschemeForm(dataProviderScheme);

            if (!SaveInMemory(dataProviderScheme)) return;

            if (!SendQuerySubmit(dataProviderScheme)) return;

            BindData();

            string successMessage = string.Empty;
            if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.INSERT)
            {
                successMessage = Resources.Messages.succ_data_provider_insert;
            }
            else if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.UPDATE)
            {
                successMessage = Resources.Messages.succ_data_provider_update;
            }
            Utils.ShowDialog(successMessage, 300, Resources.Messages.succ_operation);
            if ( _action == Action.INSERT )
            {
                Utils.AppendScript( "~/dataproviderschemes.aspx" );
            }
        }

        protected void btnAddNewDataProvider_Click(object sender, EventArgs e)
        {
            IDataProviderSchemeMutableObject dataProvider = GetDataProviderSchemeFromSession();

            dataProvider = GetDataProviderschemeForm(dataProvider);

            // form codelist validation
            if (dataProvider == null) 
            {
                txt_id_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                return;
            }
            dataProvider = InsertDataProviderInDataProviderscheme(dataProvider);

            if (dataProvider == null)
            {
                txt_id_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                Utils.ShowDialog(Resources.Messages.err_data_provider_insert, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href= '#dataproviders';");
                return;
            }
            
            if (!SaveInMemory(dataProvider)) 
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
            Utils.AppendScript("location.href='#dataproviders';");
        }

        protected void btnUpdateDataProvider_Click(object sender, EventArgs e)
        {
            // Get Input field
            string data_provider_id = txt_id_update.Text.Trim();
            IList<ITextTypeWrapperMutableObject> data_provider_names = AddTextName_Update.TextObjectList;
            IList<ITextTypeWrapperMutableObject> data_provider_descs = AddTextDescription_Update.TextObjectList;
            // string code_order_str = txtUpdateCodeOrder.Text.Trim();
            
            // Get Current Object Session
            IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();
            IEnumerable<IDataProviderMutableObject> _rc = (from x in dataProviderScheme.Items where x.Id == data_provider_id select x).OfType<IDataProviderMutableObject>();
            if (_rc.Count() == 0) return;

            IDataProviderMutableObject dataProvider = _rc.First();

            IDataProviderMutableObject _bDataProvider = new DataProviderMutableCore();

            int indexDataProvider = dataProviderScheme.Items.IndexOf(dataProvider);
            int indexOrder = 0;
            try
            {

                #region DATAPROVIDER ID
                if (!data_provider_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(data_provider_id))
                {
                    _bDataProvider.Id = data_provider_id;
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_id_format;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#dataproviders';");
                    return;
                }
                #endregion

                #region DATAPROVIDER NAMES
                if (data_provider_names != null)
                {
                    foreach (var tmpName in data_provider_names)
                    {
                        _bDataProvider.AddName(tmpName.Locale, tmpName.Value);
                    }
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_list_name_format;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#dataproviders';");
                    return;
                }
                #endregion

                #region DATAPROVIDER DESCRIPTIONS
                if (data_provider_descs != null)
                {
                    foreach (var tmpDescription in data_provider_descs)
                    {
                        _bDataProvider.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                    }
                }
                #endregion

                dataProviderScheme.Items.Remove(dataProvider);
                dataProviderScheme.Items.Insert(indexDataProvider, _bDataProvider);

                var canRead = dataProviderScheme.ImmutableInstance;

            }
            catch (Exception ex) // ERRORE GENERICO!
            {
                dataProviderScheme.Items.Remove(_bDataProvider);
                dataProviderScheme.Items.Insert(indexDataProvider, dataProvider);

                Utils.ShowDialog(Resources.Messages.err_agency_update, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href='#dataproviders';");

            }

            if (!SaveInMemory(dataProviderScheme))
                return;

            BindData();
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href='#dataproviders';");
        }

        protected void gvDataProviderschemesItem_RowCommand(object sender, GridViewCommandEventArgs e)
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

                        IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > dataProviderScheme.ImmutableInstance.Items.Count) return;

                        if ( gvDataProviderschemesItem.PageIndex > 0 )
                        {
                            CurrentDataProviderIndex = gvr.RowIndex + ( gvDataProviderschemesItem.PageIndex * gvDataProviderschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentDataProviderIndex = gvr.RowIndex;
                        }

                        IDataProvider currentDataProvider = ((IDataProvider)dataProviderScheme.ImmutableInstance.Items[CurrentDataProviderIndex]);

                        AddTextName_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataProvider;
                        AddTextName_Update.TType = TextType.NAME;
                        AddTextName_Update.ClearTextObjectList();
                        AddTextName_Update.InitTextObjectList = currentDataProvider.Names;

                        AddTextDescription_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataProvider;
                        AddTextDescription_Update.TType = TextType.DESCRIPTION;
                        AddTextDescription_Update.ClearTextObjectList();
                        AddTextDescription_Update.InitTextObjectList = currentDataProvider.Descriptions;
                        // txtUpdateCodeOrder.Text = (gvr.RowIndex + 1).ToString();
                        break;
                    }
                case "DELETE":
                    {

                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);

                        IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > dataProviderScheme.Items.Count) return;

                        int selectedRecordCount = gvDataProviderschemesItem.PageSize * gvDataProviderschemesItem.PageIndex + gvr.RowIndex;

                        dataProviderScheme.Items.RemoveAt(selectedRecordCount);
                        Session[KEY_PAGE_SESSION] = dataProviderScheme;
                        BindData();
                        
                        Utils.AppendScript("location.href='#dataproviders'");
                    }
                    break;
                case "ANNOTATION":
                    {

                        IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();
                        if ( dataProviderScheme == null )
                        {
                            dataProviderScheme = GetDataProviderschemeForm( dataProviderScheme );
                        }
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);                   
                        if (gvr.RowIndex < 0 && gvr.RowIndex > dataProviderScheme.Items.Count) return;

                        if ( gvDataProviderschemesItem.PageIndex > 0 )
                        {
                            CurrentDataProviderIndex = gvr.RowIndex + ( gvDataProviderschemesItem.PageIndex * gvDataProviderschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentDataProviderIndex = gvr.RowIndex;
                        }

                        //ctr_annotation_update.EditMode = !Utils.ViewMode;
                        btnSaveAnnotationCode.Visible = !Utils.ViewMode;

                        ctr_annotation_update.AnnotationObjectList = dataProviderScheme.Items[CurrentDataProviderIndex].Annotations;

                        Utils.AppendScript("openP('data_provider_annotation',650);");

                        Utils.AppendScript("location.href= '#dataproviders';");

                    } break;


            }

        }
        protected void gvDataProviderschemesItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // NULL
        }
        protected void gvDataProviderschemesItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // NULL
        }
        protected void gvDataProviderschemesItem_Sorting(object sender, GridViewSortEventArgs e)
        {

        }
        protected void gvDataProviderschemesItem_Sorted(object sender, EventArgs e)
        {
            // NULL
        }

        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {
            if ( !csvFile.HasFile )
            {
                Utils.AppendScript( "openPopUp( 'importCsv' );" );
                Utils.AppendScript( "location.href='#dataproviders'" );
                Utils.AppendScript( string.Format( "alert( '{0}' );", Resources.Messages.err_no_file_uploaded ) );
                return;
            }

            IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();
            if (dataProviderScheme == null) return;
            List<csvDataProvider> dataProviders = new List<csvDataProvider>();
            bool errorInUploading = false;

            try
            {
                string filenameWithoutExtension = string.Format("{0}_{1}_{2}", Path.GetFileName(csvFile.FileName).Substring(0, csvFile.FileName.Length - 4), Session.SessionID, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_'));
                string filename = string.Format("{0}.csv", filenameWithoutExtension);
                string logFilename = string.Format("{0}.log", filenameWithoutExtension);
                csvFile.SaveAs(Server.MapPath("~/csv_dataproviderschemes_files/") + filename);

                StreamReader reader = new StreamReader(Server.MapPath("~/csv_dataproviderschemes_files/") + filename);
                StreamWriter logWriter = new StreamWriter(Server.MapPath("~/csv_dataproviderschemes_import_logs/") + logFilename, true);
                logWriter.WriteLine(string.Format("LOG RELATIVO A CARICAMENTO DEL DATA PROVIDER SCHEME [ ID => \"{0}\"  AGENCY_ID => \"{1}\"  VERSION => \"{2}\" ]  |  LINGUA SELEZIONATA: {3}\n", dataProviderScheme.Id.ToString(), dataProviderScheme.AgencyId.ToString(), dataProviderScheme.Version.ToString(), cmbLanguageForCsv.SelectedValue.ToString()));
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
                    if (fields[0].Trim().Equals( "\"\"" ) || fields[0].Trim().Equals(string.Empty))
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
                    if (fields[1].Trim().Equals( "\"\"" ) || fields[1].Trim().Equals(string.Empty))
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
                    dataProviders.Add(new csvDataProvider(fields[0].ToString().Replace( "\"", string.Empty ), fields[1].ToString().Replace( "\"", string.Empty ), fields[2].ToString().Replace( "\"", string.Empty )));
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

            foreach (csvDataProvider dataProvider in dataProviders)
            {
                IEnumerable<IDataProviderMutableObject> tmpDataProviders = (from dataProv in dataProviderScheme.Items where dataProvider.dataProvider == dataProv.Id select dataProv).OfType<IDataProviderMutableObject>();

                IDataProviderMutableObject tmpDataProvider;

                if (!(tmpDataProviders.Count() > 0))
                {
                    tmpDataProvider = new DataProviderMutableCore();
                    tmpDataProvider.Id = dataProvider.dataProvider;
                    tmpDataProvider.AddName(cmbLanguageForCsv.SelectedValue.ToString(), dataProvider.name);
                    tmpDataProvider.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), dataProvider.description);

                    dataProviderScheme.AddItem(tmpDataProvider);

                }
                else
                {
                    tmpDataProvider = tmpDataProviders.First();
                    tmpDataProvider.Id = dataProvider.dataProvider;
                    tmpDataProvider.AddName(cmbLanguageForCsv.SelectedValue.ToString(), dataProvider.name);
                    tmpDataProvider.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), dataProvider.description);
                }


            }

            if (!SaveInMemory(dataProviderScheme))
                return;
            BindData();
            if ( !errorInUploading )
            {
                Utils.ShowDialog( Resources.Messages.succ_operation );
            }
            Utils.AppendScript("location.href='#dataproviders';");
        }

        #endregion

        #region LAYOUT

        private void SetLabelDetail()
        {
            IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();

            if (dataProviderScheme == null)
                lblDataProviderSchemeDetail.Text = String.Format("({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
            else
            {

                lblDataProviderSchemeDetail.Text = String.Format("{3} ({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version, _localizedUtils.GetNameableName(dataProviderScheme.ImmutableInstance));
            }
        }

        private void BindData(bool isNewItem = false)
        {
            IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();

            if (dataProviderScheme == null) return;

            SetGeneralTab(dataProviderScheme.ImmutableInstance);

            LocalizedUtils localUtils = new LocalizedUtils(Utils.LocalizedCulture);
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            IList<DataProvider> lDataProviderSchemeItem = new List<DataProvider>();

            foreach (IDataProvider dataProvider in dataProviderScheme.ImmutableInstance.Items)
            {
                lDataProviderSchemeItem.Add(new DataProvider(dataProvider.Id, localUtils.GetNameableName(dataProvider), localUtils.GetNameableDescription(dataProvider)));
            }
            
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gvDataProviderschemesItem.PageSize = numberOfRows;
            }
            else
            {
                gvDataProviderschemesItem.PageSize = Utils.DetailsDataProviderschemeGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, lDataProviderSchemeItem.Count.ToString() );
            gvDataProviderschemesItem.DataSource = lDataProviderSchemeItem;
            gvDataProviderschemesItem.DataBind();

            if ( lDataProviderSchemeItem.Count == 0 )
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

        private void SetGeneralTab(IDataProviderScheme dataProviderScheme)
        {
            txtDSDID.Text = dataProviderScheme.Id;
            txtAgenciesReadOnly.Text = dataProviderScheme.AgencyId;
            txtVersion.Text = dataProviderScheme.Version;
            chkIsFinal.Checked = dataProviderScheme.IsFinal.IsTrue;


            FileDownload31.ucID = dataProviderScheme.Id;
            FileDownload31.ucAgency = dataProviderScheme.AgencyId;
            FileDownload31.ucVersion = dataProviderScheme.Version;
            FileDownload31.ucArtefactType = "DataProviderScheme";

            txtDSDURI.Text = (dataProviderScheme.Uri != null) ? dataProviderScheme.Uri.AbsoluteUri : string.Empty;
            txtDSDURN.Text = (dataProviderScheme.Urn != null) ? dataProviderScheme.Urn.AbsoluteUri : string.Empty;
            txtValidFrom.Text = (dataProviderScheme.StartDate != null) ? string.Format("{0}/{1}/{2}", dataProviderScheme.StartDate.Date.Value.Day.ToString(), dataProviderScheme.StartDate.Date.Value.Month.ToString(), dataProviderScheme.StartDate.Date.Value.Year.ToString()) : string.Empty;
            txtValidTo.Text = (dataProviderScheme.EndDate != null) ? string.Format("{0}/{1}/{2}", dataProviderScheme.EndDate.Date.Value.Day.ToString(), dataProviderScheme.EndDate.Date.Value.Month.ToString(), dataProviderScheme.EndDate.Date.Value.Year.ToString()) : string.Empty;
            txtDSDName.Text = _localizedUtils.GetNameableName(dataProviderScheme);
            txtDSDDescription.Text = _localizedUtils.GetNameableDescription(dataProviderScheme);

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

            if (_action == Action.VIEW || dataProviderScheme.IsFinal.IsTrue)
            {
                AddTextName.Visible = false;
                AddTextDescription.Visible = false;
                txtAllDescriptions.Visible = true;
                txtAllNames.Visible = true;
                chkIsFinal.Enabled = false;
                txtAllDescriptions.Text = _localizedUtils.GetNameableDescription(dataProviderScheme);
                txtAllNames.Text = _localizedUtils.GetNameableName(dataProviderScheme);
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

                AddTextName.InitTextObjectList = dataProviderScheme.Names;
                AddTextDescription.InitTextObjectList = dataProviderScheme.Descriptions;
            }

            if ( _action != Action.VIEW )
            {
                DuplicateArtefact1.Visible = true;
            }

            AnnotationGeneralControl.AddText_ucOpenTabName = AnnotationGeneralControl.ClientID;
            AnnotationGeneralControl.AnnotationObjectList = dataProviderScheme.MutableInstance.Annotations;
            AnnotationGeneralControl.EditMode = (dataProviderScheme.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;
            AnnotationGeneralControl.OwnerAgency =  txtAgenciesReadOnly.Text;
            ctr_annotation_update.EditMode = (dataProviderScheme.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;

            if (dataProviderScheme.IsFinal.IsTrue || _action == Action.VIEW)
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
            SetDataProviderDetailPanel(dataProviderScheme);
        }

        private void SetDataProviderDetailPanel(IDataProviderScheme dataProviderScheme)
        {
            // Verifico se la codelist è final
            if (dataProviderScheme.IsFinal.IsTrue || _action == Action.VIEW)
            {
                // Se final il pulsante di add e le colonne di modifica
                // dei codici non devono apparire
                btnSaveMemoryDataProviderScheme.Visible = false;
                btnAddNewDataProvider.Visible = false;
                AddTextName_Update.ucEditMode = false;
                AddTextDescription_Update.ucEditMode = false;
                AnnotationGeneralControl.EditMode = false;
                btnSaveAnnotationCode.Enabled = false;
                btnUpdateDataProvider.Enabled = false;
                //gvDataProviderschemesItem.Columns[3].Visible = false;
                gvDataProviderschemesItem.Columns[4].Visible = false;
                //gvDataProviderschemesItem.Columns[5].Visible = false;
                //cmbLanguageForCsv.Visible = false;
                //imgImportCsv.Visible = false;
            }
            else
            {
                btnSaveMemoryDataProviderScheme.Visible = true;
                btnAddNewDataProvider.Visible = true;
                gvDataProviderschemesItem.Columns[3].Visible = true;
                gvDataProviderschemesItem.Columns[4].Visible = true;
                gvDataProviderschemesItem.Columns[5].Visible = true;
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
                btnSaveMemoryDataProviderScheme.Visible = true;
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
                btnSaveMemoryDataProviderScheme.Visible = false;
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
            btnSaveMemoryDataProviderScheme.Visible = true;

            // Svuoto le griglie di name e description
            if (Request["ACTION"] == "INSERT" && !Page.IsPostBack)
            {
                AddTextName.ClearTextObjectList();
                AddTextDescription.ClearTextObjectList();
            }
        }
        private void SetInitControls()
        {
            AddTextName.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataProviderScheme;
            AddTextName.TType = TextType.NAME;

            AddTextDescription.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataProviderScheme;
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

        protected void btnSaveAnnotationDataProvider_Click(object sender, EventArgs e)
        {
            IDataProviderSchemeMutableObject dp = GetDataProviderSchemeFromSession();

            if (!SaveInMemory(dp)) return;

            BindData();

            Utils.AppendScript("location.href='#dataproviders';");
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
            Utils.AppendScript("location.href= '#dataproviders';");
        }

        protected void btnClearFieldsForUpdate_Click(object sender, EventArgs e)
        {
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href= '#dataproviders';");
        }

        protected void btnChangePaging_Click(object sender, EventArgs e)
        {
              IDataProviderSchemeMutableObject dataProviderScheme = GetDataProviderSchemeFromSession();

            if (dataProviderScheme == null) return;

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.DataProviderScheme> lDataProviderscheme = eMapper.GetDataProviderSchemeList(_sdmxObjects);
            IList<DataProvider> lDataProviderItem = new List<DataProvider>();

            foreach (IDataProvider dataProvider in dataProviderScheme.ImmutableInstance.Items)
            {
                lDataProviderItem.Add(new DataProvider(dataProvider.Id, _localizedUtils.GetNameableName(dataProvider), _localizedUtils.GetNameableDescription(dataProvider)));
            }

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gvDataProviderschemesItem.PageSize = numberOfRows;
                }
                else
                {
                    gvDataProviderschemesItem.PageSize = Utils.DetailsDataProviderschemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.DetailsDataProviderschemeGridNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                Utils.AppendScript( "location.href='#dataproviders';" );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gvDataProviderschemesItem.PageSize = Utils.DetailsAgencyschemeGridNumberRow;
                txtNumberOfRows.Text = Utils.DetailsAgencyschemeGridNumberRow.ToString();
            }
            gvDataProviderschemesItem.DataSource = lDataProviderItem;
            gvDataProviderschemesItem.DataBind();     
            Utils.AppendScript( "location.href='#dataproviders';" );
        }

        protected void gvDataProviderschemesItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           IDataProviderSchemeMutableObject dps = GetDataProviderSchemeFromSession();
           if ( e.Row.RowIndex != -1 )
           {
               string dataProviderId = ((Label)e.Row.Cells[1].Controls[1]).Text;
               IDataProviderMutableObject dataProvider = dps.Items.Where( c => c.Id.ToString().Equals( dataProviderId ) ).First();
               Label lblNumber = (Label)e.Row.Cells[5].Controls[1];
               ImageButton btnImage = (ImageButton)e.Row.Cells[5].Controls[3];
               int numberOfAnnotation = dataProvider.Annotations.Count;
               lblNumber.Text = numberOfAnnotation.ToString();
               if ( numberOfAnnotation == 0 && ( dps.FinalStructure.IsTrue || _action == Action.VIEW ) )
               {
                   btnImage.Enabled = false;
               }
           }
        }

    }

}