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
    public partial class dataconsumerschemeItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        public struct csvDataConsumer
        {
            public string dataConsumer;
            public string name;
            public string description;

            public csvDataConsumer(string dataConsumer, string name, string description)
            {
                this.dataConsumer = dataConsumer;
                this.name = name;
                this.description = description;
              }
        }

        public static string KEY_PAGE_SESSION = "TempDataConsumerscheme";

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

        private IDataConsumerSchemeMutableObject GetDataConsumerschemeForm()
        {
            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region DATACONSUMERSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region DATACONSUMERSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME START END DATE
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

            IDataConsumerSchemeMutableObject tmpDataConsumerscheme = new DataConsumerSchemeMutableCore();
            #region CREATE DATACONSUMERSCHEME FROM FORM

            tmpDataConsumerscheme.AgencyId = GetAgencyValue();
            tmpDataConsumerscheme.Id = txtDSDID.Text;
            tmpDataConsumerscheme.Version = txtVersion.Text;
            tmpDataConsumerscheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            tmpDataConsumerscheme.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty ) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                tmpDataConsumerscheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                tmpDataConsumerscheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                tmpDataConsumerscheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    tmpDataConsumerscheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    tmpDataConsumerscheme.AddAnnotation(annotation);
                }

            #endregion

            return tmpDataConsumerscheme;
        }

        private IDataConsumerSchemeMutableObject GetDataConsumerschemeForm(IDataConsumerSchemeMutableObject dataConsumerScheme)
        {

            if (dataConsumerScheme == null) return GetDataConsumerschemeForm();

            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region DATACONSUMERSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME AGENCY
            if ( cmbAgencies.Text.Trim().Equals( string.Empty ) )
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region DATACONSUMERSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region DATACONSUMERSCHEME START END DATE
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

            #region CREATE DATACONSUMERSCHEME FROM FORM

            dataConsumerScheme.AgencyId = GetAgencyValue();
            dataConsumerScheme.Id = txtDSDID.Text;
            dataConsumerScheme.Version = txtVersion.Text;
            dataConsumerScheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            dataConsumerScheme.Uri = (!txtDSDURI.Text.Trim().Equals( string.Empty ) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                dataConsumerScheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                dataConsumerScheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (dataConsumerScheme.Names.Count != 0)
            {
                dataConsumerScheme.Names.Clear();
            }
            if (dataConsumerScheme.Descriptions.Count != 0)
            {
                dataConsumerScheme.Descriptions.Clear();
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                dataConsumerScheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    dataConsumerScheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }

            if (dataConsumerScheme.Annotations.Count != 0)
            {
                dataConsumerScheme.Annotations.Clear();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    dataConsumerScheme.AddAnnotation(annotation);
                }

            #endregion

            return dataConsumerScheme;
        }

        private IDataConsumerSchemeMutableObject InsertDataConsumerInDataConsumerscheme(IDataConsumerSchemeMutableObject dataConsumerScheme)
        {
            if (dataConsumerScheme == null) return null;

            IDataConsumerMutableObject dataConsumer = new DataConsumerMutableCore();

            string data_consumer_id = txt_id_new.Text.Trim();

            IList<ITextTypeWrapperMutableObject> data_consumer_names = AddTextName_new.TextObjectList;
            IList<ITextTypeWrapperMutableObject> data_consumer_descs = AddTextDescription_new.TextObjectList;
            // string code_order_str = txtOrderNewCode.Text.Trim();     ----- ORDINE

            #region DATACONSUMER ID
            if (ValidationUtils.CheckIdFormat(data_consumer_id))
            {
                dataConsumer.Id = data_consumer_id;
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_format;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#dataconsumers';");
                return null;
            }

            IEnumerable<IDataConsumerMutableObject> dataConsumers = (from c in dataConsumerScheme.Items where c.Id == data_consumer_id select c).OfType<IDataConsumerMutableObject>();
            if (dataConsumers.Count() > 0)
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_exist;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#dataconsumers';");
                return null;
            }
            #endregion

            #region DATACONSUMER NAMES
            if (data_consumer_names != null)
            {
                foreach (var tmpName in data_consumer_names)
                {
                    dataConsumer.AddName(tmpName.Locale, tmpName.Value);
                }
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_list_name_format;
                Utils.AppendScript( "openPopUp('df-Dimension', 600);" );
                Utils.AppendScript("location.href= '#dataconsumers';");
                return null;
            }
            #endregion

            #region DATACONSUMER DESCRIPTIONS
            if (data_consumer_descs != null)
            {
                foreach (var tmpDescription in data_consumer_descs)
                {
                    dataConsumer.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            }
            #endregion

            dataConsumerScheme.Items.Add(dataConsumer);

            try
            {
                // Ultimo controllo se ottengo Immutable istanze validazione completa
                var canRead = dataConsumerScheme.ImmutableInstance;
            }
            catch (Exception ex)
            {
                dataConsumerScheme.Items.Remove(dataConsumer);

                return null;

            }
            return dataConsumerScheme;
        }

        private IDataConsumerSchemeMutableObject GetDataConsumerSchemeFromSession()
        {
            try
            {
                if (Session[KEY_PAGE_SESSION] == null)
                {
                    if (_artIdentity.ToString() != string.Empty)
                    {
                        WSModel wsModel = new WSModel();
                        ISdmxObjects sdmxObject = wsModel.GetDataConsumerScheme(_artIdentity, false,false);
                        IDataConsumerScheme dataConsumerScheme = sdmxObject.DataConsumerSchemes.FirstOrDefault();
                        Session[KEY_PAGE_SESSION] = dataConsumerScheme.MutableInstance;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                return (IDataConsumerSchemeMutableObject)Session[KEY_PAGE_SESSION];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveInMemory(IDataConsumerSchemeMutableObject dataConsumerScheme)
        {
            if (dataConsumerScheme == null) return false;

            Session[KEY_PAGE_SESSION] = dataConsumerScheme;

            return true;
        }

        private bool SendQuerySubmit(IDataConsumerSchemeMutableObject dataConsumerScheme)
        {

            try
            {

                ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

                sdmxObjects.AddDataConsumerScheme(dataConsumerScheme.ImmutableInstance);

                WSModel modelDataConsumerScheme = new WSModel();

                XmlDocument result = modelDataConsumerScheme.SubmitStructure(sdmxObjects);

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

        private int CurrentDataConsumerIndex { get { return (int)Session[KEY_PAGE_SESSION + "_index_data_consumer"]; } set { Session[KEY_PAGE_SESSION + "_index_data_consumer"] = value; } }

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
            btnNewDataConsumer.DataBind();
            lbl_title_update.DataBind();
            lbl_id_update.DataBind();
            lbl_name_update.DataBind();
            lbl_description_update.DataBind();
            btnUpdateDataConsumer.DataBind();
            btnAddNewDataConsumer.DataBind();
            btnSaveMemoryDataConsumerScheme.DataBind();
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
                txtNumberOfRows.Text = Utils.DetailsDataConsumerschemeGridNumberRow.ToString();
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

                    AddTextName_Update.ucOpenTabName = "dataconsumers";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "dataconsumers";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "dataconsumers";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "dataconsumers";
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

                    AddTextName_Update.ucOpenTabName = "dataconsumers";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "dataproviders";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "dataconsumers";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "dataconsumers";
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
                    AddTextName_Update.ucOpenTabName = "dataconsumers";
                    AddTextName_Update.ucOpenPopUpWidth = 600;
                    AddTextName_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextDescription_Update.ucOpenTabName = "dataconsumers";
                    AddTextDescription_Update.ucOpenPopUpWidth = 600;
                    AddTextDescription_Update.ucOpenPopUpName = "df-Dimension-update";

                    AddTextName_new.ucOpenTabName = "dataconsumers";
                    AddTextName_new.ucOpenPopUpWidth = 600;
                    AddTextName_new.ucOpenPopUpName = "df-Dimension";

                    AddTextDescription_new.ucOpenTabName = "dataconsumers";
                    AddTextDescription_new.ucOpenPopUpWidth = 600;
                    AddTextDescription_new.ucOpenPopUpName = "df-Dimension";
                    /*FileDownload31.ucID = _artIdentity.ID;
                    FileDownload31.ucAgency = _artIdentity.Agency;
                    FileDownload31.ucVersion = _artIdentity.Version;
                    FileDownload31.ucArtefactType = "DataConsumerScheme";*/

                    break;
            }
            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.DataConsumerScheme;
            DuplicateArtefact1.ucMaintanableArtefact = GetDataConsumerSchemeFromSession();

        }

        protected void gvDataConsumerschemesItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDataConsumerschemesItem.PageSize = 12;
            gvDataConsumerschemesItem.PageIndex = e.NewPageIndex;
            BindData();
            Utils.AppendScript("location.href= '#dataconsumers';");
        }

        protected void btnSaveMemoryDataConsumerScheme_Click(object sender, EventArgs e)
        {
            IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();
            if (dataConsumerScheme == null) dataConsumerScheme = GetDataConsumerschemeForm();
            else dataConsumerScheme = GetDataConsumerschemeForm(dataConsumerScheme);

            if (!SaveInMemory(dataConsumerScheme)) return;

            if (!SendQuerySubmit(dataConsumerScheme)) return;

            BindData();

            string successMessage = string.Empty;
            if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.INSERT)
            {
                successMessage = Resources.Messages.succ_data_consumer_insert;
            }
            else if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.UPDATE)
            {
                successMessage = Resources.Messages.succ_data_consumer_update;
            }
            Utils.ShowDialog(successMessage, 300, Resources.Messages.succ_operation);
            if ( _action == Action.INSERT )
            {
                Utils.AppendScript( "~/dataconsumerschemes.aspx" );
            }
        }

        protected void btnAddNewDataConsumer_Click(object sender, EventArgs e)
        {
            IDataConsumerSchemeMutableObject dataConsumer = GetDataConsumerSchemeFromSession();

            dataConsumer = GetDataConsumerschemeForm(dataConsumer);

            // form codelist validation
            if (dataConsumer == null) 
            {
                txt_id_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                return;
            }

            dataConsumer = InsertDataConsumerInDataConsumerscheme(dataConsumer);

            if (dataConsumer == null)
            {
                txt_id_new.Text = string.Empty;
                AddTextName_new.ClearTextObjectListWithOutJS();
                AddTextDescription_new.ClearTextObjectListWithOutJS();
                lblErrorOnNewInsert.Text = string.Empty;
                Utils.ShowDialog(Resources.Messages.err_agency_insert, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href= '#dataconsumers';");
                return;
            }
            
            if (!SaveInMemory(dataConsumer)) 
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
            Utils.AppendScript("location.href='#dataconsumers';");
        }

        protected void btnUpdateDataConsumer_Click(object sender, EventArgs e)
        {
            // Get Input field
            string data_consumer_id = txt_id_update.Text.Trim();
            IList<ITextTypeWrapperMutableObject> data_consumer_names = AddTextName_Update.TextObjectList;
            IList<ITextTypeWrapperMutableObject> data_consumer_descs = AddTextDescription_Update.TextObjectList;
            // string code_order_str = txtUpdateCodeOrder.Text.Trim();
            
            // Get Current Object Session
            IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();
            IEnumerable<IDataConsumerMutableObject> _rc = (from x in dataConsumerScheme.Items where x.Id == data_consumer_id select x).OfType<IDataConsumerMutableObject>();
            if (_rc.Count() == 0) return;

            IDataConsumerMutableObject dataConsumer = _rc.First();

            IDataConsumerMutableObject _bDataConsumer = new DataConsumerMutableCore();

            int indexDataConsumer = dataConsumerScheme.Items.IndexOf(dataConsumer);
            int indexOrder = 0;
            try
            {

                #region DATACONSUMER ID
                if (!data_consumer_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(data_consumer_id))
                {
                    _bDataConsumer.Id = data_consumer_id;
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_id_format;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#dataconsumers';");
                    return;
                }
                #endregion

                #region DATACONSUMER NAMES
                if (data_consumer_names != null)
                {
                    foreach (var tmpName in data_consumer_names)
                    {
                        _bDataConsumer.AddName(tmpName.Locale, tmpName.Value);
                    }
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_list_name_format;
                    Utils.AppendScript( "openPopUp('df-Dimension-update', 600 );" );
                    Utils.AppendScript("location.href= '#dataconsumers';");
                    return;
                }
                #endregion

                #region DATACONSUMER DESCRIPTIONS
                if (data_consumer_descs != null)
                {
                    foreach (var tmpDescription in data_consumer_descs)
                    {
                        _bDataConsumer.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                    }
                }
                #endregion

                dataConsumerScheme.Items.Remove(dataConsumer);
                dataConsumerScheme.Items.Insert(indexDataConsumer, _bDataConsumer);

                var canRead = dataConsumerScheme.ImmutableInstance;

            }
            catch (Exception ex) // ERRORE GENERICO!
            {
                dataConsumerScheme.Items.Remove(_bDataConsumer);
                dataConsumerScheme.Items.Insert(indexDataConsumer, dataConsumer);

                Utils.ShowDialog(Resources.Messages.err_agency_update, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href='#dataconsumers';");

            }

            if (!SaveInMemory(dataConsumerScheme))
                return;

            BindData();
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href='#dataconsumers';");
        }

        protected void gvDataConsumerschemesItem_RowCommand(object sender, GridViewCommandEventArgs e)
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

                        IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();

                        if (gvr.RowIndex < 0 && gvr.RowIndex > dataConsumerScheme.ImmutableInstance.Items.Count) return;

                        if ( gvDataConsumerschemesItem.PageIndex > 0 )
                        {
                            CurrentDataConsumerIndex = gvr.RowIndex + ( gvDataConsumerschemesItem.PageIndex * gvDataConsumerschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentDataConsumerIndex = gvr.RowIndex;
                        }

                        IDataConsumer currentDataConsumer = ((IDataConsumer)dataConsumerScheme.ImmutableInstance.Items[CurrentDataConsumerIndex]);

                        AddTextName_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataConsumer;
                        AddTextName_Update.TType = TextType.NAME;
                        AddTextName_Update.ClearTextObjectList();
                        AddTextName_Update.InitTextObjectList = currentDataConsumer.Names;

                        AddTextDescription_Update.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataConsumer;
                        AddTextDescription_Update.TType = TextType.DESCRIPTION;
                        AddTextDescription_Update.ClearTextObjectList();
                        AddTextDescription_Update.InitTextObjectList = currentDataConsumer.Descriptions;
                        // txtUpdateCodeOrder.Text = (gvr.RowIndex + 1).ToString();
                        break;
                    }
                case "DELETE":
                    {
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();
                        if (gvr.RowIndex < 0 && gvr.RowIndex > dataConsumerScheme.Items.Count) return;
                        dataConsumerScheme.Items.RemoveAt(gvr.RowIndex);
                        Session[KEY_PAGE_SESSION] = dataConsumerScheme;
                        BindData();
                        
                        Utils.AppendScript("location.href='#dataconsumers'");
                    }
                    break;
                case "ANNOTATION":
                    {
                        IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();
                        if ( dataConsumerScheme == null )
                        {
                            dataConsumerScheme = GetDataConsumerschemeForm( dataConsumerScheme );
                        }
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    
                        if (gvr.RowIndex < 0 && gvr.RowIndex > dataConsumerScheme.Items.Count) return;

                        if ( gvDataConsumerschemesItem.PageIndex > 0 )
                        {
                            CurrentDataConsumerIndex = gvr.RowIndex + ( gvDataConsumerschemesItem.PageIndex * gvDataConsumerschemesItem.PageSize );
                        }
                        else
                        {
                             CurrentDataConsumerIndex = gvr.RowIndex;
                        }

                        //ctr_annotation_update.EditMode = !Utils.ViewMode;
                        btnSaveAnnotationCode.Visible = !Utils.ViewMode;

                        ctr_annotation_update.AnnotationObjectList = dataConsumerScheme.Items[CurrentDataConsumerIndex].Annotations;

                        Utils.AppendScript("openP('data_consumer_annotation',650);");

                        Utils.AppendScript("location.href= '#dataconsumers';");

                    } break;
            }

        }
        protected void gvDataConsumerschemesItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // NULL
        }
        protected void gvDataConsumerschemesItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // NULL
        }
        protected void gvDataConsumerschemesItem_Sorting(object sender, GridViewSortEventArgs e)
        {

        }
        protected void gvDataConsumerschemesItem_Sorted(object sender, EventArgs e)
        {
            // NULL
        }

        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {
            if ( !csvFile.HasFile )
            {
                Utils.AppendScript( "openPopUp( 'importCsv' );" );
                Utils.AppendScript( "location.href='#dataconsumers';" );
                Utils.AppendScript( string.Format( "alert( '{0}' );", Resources.Messages.err_no_file_uploaded ) );
                return;
            }

            IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();
            if (dataConsumerScheme == null) return;
            List<csvDataConsumer> dataConsumers = new List<csvDataConsumer>();
            bool errorInUploading = false;

            string wrongRowsMessage = string.Empty;
            string wrongRowsMessageForUser = string.Empty;
            string wrongFileLines = string.Empty;

            try
            {
                string filenameWithoutExtension = string.Format("{0}_{1}_{2}", Path.GetFileName(csvFile.FileName).Substring(0, csvFile.FileName.Length - 4), Session.SessionID, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_'));
                string filename = string.Format("{0}.csv", filenameWithoutExtension);
                string logFilename = string.Format("{0}.log", filenameWithoutExtension);
                csvFile.SaveAs(Server.MapPath("~/csv_dataconsumerschemes_files/") + filename);

                StreamReader reader = new StreamReader(Server.MapPath("~/csv_dataconsumerschemes_files/") + filename);
                StreamWriter logWriter = new StreamWriter(Server.MapPath("~/csv_dataconsumerschemes_import_logs/") + logFilename, true);
                logWriter.WriteLine(string.Format("LOG RELATIVO A CARICAMENTO DEL DATA CONSUMER SCHEME [ ID => \"{0}\"  AGENCY_ID => \"{1}\"  VERSION => \"{2}\" ]  |  LINGUA SELEZIONATA: {3}\n", dataConsumerScheme.Id.ToString(), dataConsumerScheme.AgencyId.ToString(), dataConsumerScheme.Version.ToString(), cmbLanguageForCsv.SelectedValue.ToString()));
                logWriter.WriteLine("-----------------------------------------------------------------------------------------------------------------------------\n");
                reader.ReadLine();
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
                    dataConsumers.Add(new csvDataConsumer(fields[0].ToString().Replace( "\"", "" ), fields[1].ToString().Replace( "\"", "" ), fields[2].ToString().Replace( "\"", "" )));
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

            foreach (csvDataConsumer dataConsumer in dataConsumers)
            {
                IEnumerable<IDataConsumerMutableObject> tmpDataConsumers = (from dataCons in dataConsumerScheme.Items where dataCons.Id == dataConsumer.dataConsumer select dataCons).OfType<IDataConsumerMutableObject>();

                IDataConsumerMutableObject tmpDataConsumer;

                if (!(tmpDataConsumers.Count() > 0))
                {
                    tmpDataConsumer = new DataConsumerMutableCore();
                    tmpDataConsumer.Id = dataConsumer.dataConsumer;
                    tmpDataConsumer.AddName(cmbLanguageForCsv.SelectedValue.ToString(), dataConsumer.name);
                    tmpDataConsumer.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), dataConsumer.description);

                    dataConsumerScheme.AddItem(tmpDataConsumer);

                }
                else
                {
                    tmpDataConsumer = tmpDataConsumers.First();
                    tmpDataConsumer.Id = dataConsumer.dataConsumer;
                    tmpDataConsumer.AddName(cmbLanguageForCsv.SelectedValue.ToString(), dataConsumer.name);
                    tmpDataConsumer.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), dataConsumer.description);
                }
            }

            if (!SaveInMemory(dataConsumerScheme))
                return;
            BindData();
            if ( !errorInUploading )
            {
                Utils.ShowDialog( Resources.Messages.succ_operation );
            }
            Utils.AppendScript("location.href='#dataconsumers';");
        }

        #endregion

        #region LAYOUT

        private void SetLabelDetail()
        {
            IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();

            if (dataConsumerScheme == null)
                lblDataConsumerSchemeDetail.Text = String.Format("({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
            else
            {

                lblDataConsumerSchemeDetail.Text = String.Format("{3} ({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version, _localizedUtils.GetNameableName(dataConsumerScheme.ImmutableInstance));
            }
        }

        private void BindData(bool isNewItem = false)
        {
            IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();

            if (dataConsumerScheme == null) return;

            SetGeneralTab(dataConsumerScheme.ImmutableInstance);

            LocalizedUtils localUtils = new LocalizedUtils(Utils.LocalizedCulture);
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            IList<DataConsumer> lDataConsumerSchemeItem = new List<DataConsumer>();

            foreach (IDataConsumer dataConsumer in dataConsumerScheme.ImmutableInstance.Items)
            {
                lDataConsumerSchemeItem.Add(new DataConsumer(dataConsumer.Id, localUtils.GetNameableName(dataConsumer), localUtils.GetNameableDescription(dataConsumer)));
            }
            
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gvDataConsumerschemesItem.PageSize = numberOfRows;
            }
            else
            {
                gvDataConsumerschemesItem.PageSize = Utils.DetailsDataConsumerschemeGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, lDataConsumerSchemeItem.Count.ToString() );
            gvDataConsumerschemesItem.DataSource = lDataConsumerSchemeItem;
            gvDataConsumerschemesItem.DataBind();

            if ( lDataConsumerSchemeItem.Count == 0 )
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

        private void SetGeneralTab(IDataConsumerScheme dataConsumerScheme)
        {
            txtDSDID.Text = dataConsumerScheme.Id;
            txtAgenciesReadOnly.Text = dataConsumerScheme.AgencyId;
            txtVersion.Text = dataConsumerScheme.Version;
            chkIsFinal.Checked = dataConsumerScheme.IsFinal.IsTrue;
            
            FileDownload31.ucID = dataConsumerScheme.Id;
            FileDownload31.ucAgency = dataConsumerScheme.AgencyId;
            FileDownload31.ucVersion = dataConsumerScheme.Version;
            FileDownload31.ucArtefactType = "DataConsumerScheme";

            txtDSDURI.Text = (dataConsumerScheme.Uri != null) ? dataConsumerScheme.Uri.AbsoluteUri : string.Empty;
            txtDSDURN.Text = (dataConsumerScheme.Urn != null) ? dataConsumerScheme.Urn.AbsoluteUri : string.Empty;
            txtValidFrom.Text = (dataConsumerScheme.StartDate != null) ? string.Format("{0}/{1}/{2}", dataConsumerScheme.StartDate.Date.Value.Day.ToString(), dataConsumerScheme.StartDate.Date.Value.Month.ToString(), dataConsumerScheme.StartDate.Date.Value.Year.ToString()) : string.Empty;
            txtValidTo.Text = (dataConsumerScheme.EndDate != null) ? string.Format("{0}/{1}/{2}", dataConsumerScheme.EndDate.Date.Value.Day.ToString(), dataConsumerScheme.EndDate.Date.Value.Month.ToString(), dataConsumerScheme.EndDate.Date.Value.Year.ToString()) : string.Empty;
            txtDSDName.Text = _localizedUtils.GetNameableName(dataConsumerScheme);
            txtDSDDescription.Text = _localizedUtils.GetNameableDescription(dataConsumerScheme);

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

            if (_action == Action.VIEW || dataConsumerScheme.IsFinal.IsTrue)
            {
                AddTextName.Visible = false;
                AddTextDescription.Visible = false;
                txtAllDescriptions.Visible = true;
                txtAllNames.Visible = true;
                chkIsFinal.Enabled = false;
                txtAllDescriptions.Text = _localizedUtils.GetNameableDescription(dataConsumerScheme);
                txtAllNames.Text = _localizedUtils.GetNameableName(dataConsumerScheme);
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

                AddTextName.InitTextObjectList = dataConsumerScheme.Names;
                AddTextDescription.InitTextObjectList = dataConsumerScheme.Descriptions;
            }

            if ( _action != Action.VIEW )
            {
                DuplicateArtefact1.Visible = true;
            }

            AnnotationGeneralControl.AddText_ucOpenTabName = AnnotationGeneralControl.ClientID;
            AnnotationGeneralControl.AnnotationObjectList = dataConsumerScheme.MutableInstance.Annotations;
            AnnotationGeneralControl.EditMode = (dataConsumerScheme.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;
            AnnotationGeneralControl.OwnerAgency =  txtAgenciesReadOnly.Text;
            ctr_annotation_update.EditMode = (dataConsumerScheme.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;

            if (dataConsumerScheme.IsFinal.IsTrue || _action == Action.VIEW)
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
            SetDataConsumerDetailPanel(dataConsumerScheme);
        }

        private void SetDataConsumerDetailPanel(IDataConsumerScheme dataConsumerScheme)
        {
            // Verifico se la codelist è final
            if (dataConsumerScheme.IsFinal.IsTrue || _action == Action.VIEW)
            {
                // Se final il pulsante di add e le colonne di modifica
                // dei codici non devono apparire
                btnSaveMemoryDataConsumerScheme.Visible = false;
                btnAddNewDataConsumer.Visible = false;
                AddTextName_Update.ucEditMode = false;
                AddTextDescription_Update.ucEditMode = false;
                AnnotationGeneralControl.EditMode = false;
                btnSaveAnnotationCode.Enabled = false;
                btnUpdateDataConsumer.Enabled = false;
                //gvDataConsumerschemesItem.Columns[3].Visible = false;
                gvDataConsumerschemesItem.Columns[4].Visible = false;
                //gvDataConsumerschemesItem.Columns[5].Visible = false;
                cmbLanguageForCsv.Visible = false;
                imgImportCsv.Visible = false;
            }
            else
            {
                btnSaveMemoryDataConsumerScheme.Visible = true;
                btnAddNewDataConsumer.Visible = true;
                gvDataConsumerschemesItem.Columns[3].Visible = true;
                gvDataConsumerschemesItem.Columns[4].Visible = true;
                gvDataConsumerschemesItem.Columns[5].Visible = true;
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
                btnSaveMemoryDataConsumerScheme.Visible = true;
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
                btnSaveMemoryDataConsumerScheme.Visible = false;
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
            btnSaveMemoryDataConsumerScheme.Visible = true;

            // Svuoto le griglie di name e description
            if (Request["ACTION"] == "INSERT" && !Page.IsPostBack)
            {
                AddTextName.ClearTextObjectList();
                AddTextDescription.ClearTextObjectList();
            }
        }
        private void SetInitControls()
        {
            AddTextName.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataConsumerScheme;
            AddTextName.TType = TextType.NAME;

            AddTextDescription.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataConsumerScheme;
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

        protected void btnSaveAnnotationDataConsumer_Click(object sender, EventArgs e)
        {
            IDataConsumerSchemeMutableObject dp = GetDataConsumerSchemeFromSession();

            if (!SaveInMemory(dp)) return;

            BindData();

            Utils.AppendScript("location.href='#dataconsumers';");
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
            Utils.AppendScript("location.href= '#dataconsumers';");
        }

        protected void btnClearFieldsForUpdate_Click(object sender, EventArgs e)
        {
            lblErrorOnUpdate.Text = string.Empty;
            Utils.AppendScript("location.href= '#dataconsumers';");
        }

        protected void btnChangePaging_Click(object sender, EventArgs e)
        {
              IDataConsumerSchemeMutableObject dataConsumerScheme = GetDataConsumerSchemeFromSession();

            if (dataConsumerScheme == null) return;

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.DataConsumerScheme> lDataConsumerscheme = eMapper.GetDataConsumerSchemeList(_sdmxObjects);
            IList<DataConsumer> lDataConsumerItem = new List<DataConsumer>();

            foreach (IDataConsumer dataConsumer in dataConsumerScheme.ImmutableInstance.Items)
            {
                lDataConsumerItem.Add(new DataConsumer(dataConsumer.Id, _localizedUtils.GetNameableName(dataConsumer), _localizedUtils.GetNameableDescription(dataConsumer)));
            }

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gvDataConsumerschemesItem.PageSize = numberOfRows;
                }
                else
                {
                    gvDataConsumerschemesItem.PageSize = Utils.DetailsDataConsumerschemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.DetailsDataConsumerschemeGridNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                Utils.AppendScript( "location.href='#dataconsumers';" );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gvDataConsumerschemesItem.PageSize = Utils.DetailsDataConsumerschemeGridNumberRow;
                txtNumberOfRows.Text = Utils.DetailsDataConsumerschemeGridNumberRow.ToString();
            }
            gvDataConsumerschemesItem.DataSource = lDataConsumerItem;
            gvDataConsumerschemesItem.DataBind();     
            Utils.AppendScript( "location.href='#dataconsumers';" );
        }

        protected void gvDataConsumerschemesItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           IDataConsumerSchemeMutableObject dcs = GetDataConsumerSchemeFromSession();
           if ( e.Row.RowIndex != -1 )
           {
               string dataConsumerId = ((Label)e.Row.Cells[1].Controls[1]).Text;
               IDataConsumerMutableObject dataConsumer = dcs.Items.Where( c => c.Id.ToString().Equals( dataConsumerId ) ).First();
               Label lblNumber = (Label)e.Row.Cells[5].Controls[1];
               ImageButton btnImage = (ImageButton)e.Row.Cells[5].Controls[3];
               int numberOfAnnotation = dataConsumer.Annotations.Count;
               lblNumber.Text = numberOfAnnotation.ToString();
               if ( numberOfAnnotation == 0 && ( dcs.FinalStructure.IsTrue || _action == Action.VIEW ) )
               {
                   btnImage.Enabled = false;
               }
           }
        }

    }

}