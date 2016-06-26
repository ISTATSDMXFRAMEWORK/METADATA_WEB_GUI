using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISTAT.WSDAL;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using ISTAT.EntityMapper;
using ISTAT.Entity;
using ISTATUtils;
using System.Diagnostics;
using System.IO;

using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using System.Globalization;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System.Xml;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class dataFlowItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        #region Props

        ArtefactIdentity _artIdentity;
        WSModel _wsmodel = new WSModel();
        EntityMapper _entityMapper;
        LocalizedUtils _localizedUtils;
        SDMXUtils _sdmxUtils;
        Action _action;

        protected string AspConfirmationExit = "false";
        const string _snSdmxObjects = "SDMXObjects";

        #region Inserting Consts

        /// <summary>
        /// DF Inserting Session Name
        /// </summary>
        const string _snInsDf = "INSDF";

        /// <summary>
        /// No True Data String
        /// </summary>
        const string _ntdString = "WeBrEgIsTrY";

        /// <summary>
        /// No True Data DSD Version
        /// </summary>
        const string _ntdDSDVersion = "1010101010";

        #endregion

        private IDataflowMutableObject _dfMutable;

        /// <summary>
        /// Converte l'oggetto df mutable in ISdmxObjects()
        /// </summary>
        private ISdmxObjects _sdmxObjects
        {
            get
            {
                switch (_action)
                {
                    case Action.VIEW:
                        if (Session[_snSdmxObjects] == null)
                        {
                            ISdmxObjects sdmxAppo = _wsmodel.GetDataFlow(_artIdentity, false, false);
                            Session[_snSdmxObjects] = sdmxAppo;
                        }
                        return (ISdmxObjects)Session[_snSdmxObjects];
                    case Action.UPDATE:
                        if (!IsPostBack)
                        {
                            ISdmxObjects sdmxAppo = _wsmodel.GetDataFlow(_artIdentity, false, false);
                            _dfMutable = sdmxAppo.Dataflows.FirstOrDefault().MutableInstance;
                            SetDfToSession();
                        }
                        ISdmxObjects sdmxObjectsU = new SdmxObjectsImpl();
                        sdmxObjectsU.AddDataflow(_dfMutable.ImmutableInstance);
                        return sdmxObjectsU;
                        break;
                    case Action.INSERT:
                        ISdmxObjects sdmxObjectsI = new SdmxObjectsImpl();
                        sdmxObjectsI.AddDataflow(_dfMutable.ImmutableInstance);
                        return sdmxObjectsI;
                        break;
                }
                return null;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Org.Sdmxsource.Sdmx.Api.Exception.SdmxException.SetMessageResolver(new Org.Sdmxsource.Util.ResourceBundle.MessageDecoder());

            _dfMutable = null;

            _sdmxUtils = new SDMXUtils();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _entityMapper = new EntityMapper(Utils.LocalizedLanguage);
            _artIdentity = Utils.GetIdentityFromRequest(Request);

            SetAction();

            if (!IsPostBack)
            {
                CommonInit();
            }

            switch (_action)
            {
                case Action.INSERT:
                    SetInsertForm();
                    break;
                case Action.UPDATE:
                    SetEditForm();
                    break;
                case Action.VIEW:
                    SetViewForm();
                    Utils.ResetBeforeUnload();
                    break;
            }

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.Dataflow;
                DuplicateArtefact1.ucMaintanableArtefact = _dfMutable;
                DuplicateArtefact1.Visible = true;
            }
            else
                DuplicateArtefact1.ucDisable = true;


            lblDFID.DataBind();
            lblAgency.DataBind();
            lblVersion.DataBind();
            lblIsFinal.DataBind();
            lblURI.DataBind();
            lblURN.DataBind();
            lblValidFrom.DataBind();
            lblValidTo.DataBind();
            lblDSD.DataBind();
            lblDFNames.DataBind();
            lblDFDescriptions.DataBind();
            lbl_annotation.DataBind();

            btnSaveDF.DataBind();

        }


        /// <summary>
        /// Salva il DataFlow nel registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveDF_Click(object sender, EventArgs e)
        {
            /*
             * 1. Aggiungo all'oggetto _dfMutable:
             *      Le info della General
             *      La collection di Names
             *      La Collecction delle Desctiptions
             * 2. Richiamo il metodo che prende in input un ISdmxObjects che richiama il SaveStructure
             * 3. Visualizzo un messaggio di conferma,resetto le Session e faccio un redirect alla lista delle DF
             */

            if (!ValidateDFData())
                return;

            try
            {
                _dfMutable.Id = txtDFID.Text;
                _dfMutable.AgencyId = GetAgencyValue();
                _dfMutable.Version = txtVersion.Text;

                _dfMutable.Names.Clear();
                foreach (ITextTypeWrapperMutableObject name in AddTextName.TextObjectList)
                {
                    _dfMutable.Names.Add(name);
                }

                _dfMutable.Descriptions.Clear();
                if (AddTextDescription.TextObjectList != null)
                {
                    foreach (ITextTypeWrapperMutableObject descr in AddTextDescription.TextObjectList)
                    {
                        _dfMutable.Descriptions.Add(descr);
                    }
                }

                _dfMutable.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
                if (txtURI.Text != String.Empty)
                    _dfMutable.Uri = new Uri(txtURI.Text);

                if (txtValidFrom.Text != String.Empty)
                    _dfMutable.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (txtValidTo.Text != String.Empty)
                    _dfMutable.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                _dfMutable.Annotations.Clear();
                if (AnnotationGeneral.AnnotationObjectList != null)
                    foreach (var annotation in AnnotationGeneral.AnnotationObjectList)
                    {
                        _dfMutable.AddAnnotation(annotation);
                    }

                string[] DSDValues = txtDSD.Text.Split(',');

                IStructureReference dsdRef = new StructureReferenceImpl(DSDValues[1],
                DSDValues[0], DSDValues[2],
                SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd),
                new string[] { _ntdString });

                _dfMutable.DataStructureRef = dsdRef;

                SetDfToSession();

                WSModel wsModel = new WSModel();
                XmlDocument xRet = wsModel.SubmitStructure(_sdmxObjects);

                string err = Utils.GetXMLResponseError(xRet);

                if (err != "")
                {
                    Utils.ShowDialog(err);
                    return;
                }

                //Session[_snInsDf] = null;
                //Session[_snSdmxObjects] = null;

                //Utils.ShowDialog("Operation succesfully");
                Utils.ResetBeforeUnload();
                //Utils.AppendScript("location.href='./DataFlow.aspx';");


                string jsYes, jsNo;

                jsYes = String.Format("location.href='DataFlowItemDetails.aspx?ACTION=UPDATE&ID={0}&AGENCY={1}&VERSION={2}&ISFINAL={3}",
                            _dfMutable.Id, _dfMutable.AgencyId, _dfMutable.Version, _dfMutable.FinalStructure.IsTrue.ToString());
                //jsNo = "location.href='DataFlow.aspx?m=y'";

                string successMessage = Resources.Messages.succ_operation + " " + Resources.Messages.lbl_manage_categorisations;

                if (_action == Action.INSERT)
                {
                    jsNo = jsYes + "'";
                    jsYes += "&OCT=true'";
                }
                else
                {
                    jsYes += "&OCT=true'";
                    jsNo = "CloseConfirm();";
                }

                Utils.ShowConfirm(successMessage, jsYes, jsNo);

            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }


        #endregion

        #region Methods

        private bool ValidateDFData()
        {
            /*
             * 1. Effettuo i controlli prima del salvataggio e li visualizzo a video tramite un OpenP()
             *      General:
             *          ID obbligatorio
             *          Version obbligatorio
             *          Name Obbligatorio
             *          Controllo formale URI
             *          Controllo formale Date
             *          Controllo DSD Obbligatorio
            */

            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtDFID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo AGENCY
            if (cmbAgencies.Text.Trim().Equals(string.Empty))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
            }

            // Controllo versione
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + "" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo URI
            if (txtURI.Text != String.Empty && !ValidationUtils.CheckUriFormatNoRegExp(txtURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo presenza NAME
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
            }

            // Controlli su date
            bool checkForDatesCombination = true;

            // Controllo validità prima data
            if (txtValidFrom.Text != String.Empty && !ValidationUtils.CheckDateFormat(txtValidFrom.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_from_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
            }

            // Controllo validità seconda data
            if (txtValidTo.Text != String.Empty && !ValidationUtils.CheckDateFormat(txtValidTo.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_to_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
            }

            // Controllo congruenza date
            if (checkForDatesCombination && txtValidFrom.Text != String.Empty && txtValidTo.Text != String.Empty)
            {
                if (!ValidationUtils.CheckDates(txtValidFrom.Text, txtValidTo.Text))
                {
                    messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_date_diff + "<br /><br />";
                    errorCounter++;
                }
            }

            // Controllo obbligatorietà DSD
            if (txtDSD.Text == String.Empty)
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_dsd_ref_empty + "<br /><br />";
                errorCounter++;
            }

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_save_dsd);
                return false;
            }

            return true;
        }

        private void CommonInit()
        {
            Session[_snSdmxObjects] = null;
            Session[_snInsDf] = null;

            if (_action != Action.VIEW)
                Utils.PopulateCmbAgencies(cmbAgencies, true);

            AddTextName.ucOpenPopUpName = "df-name";
            AddTextDescription.ucOpenPopUpName = "df-description";

            AddTextName.ucOpenPopUpWidth = 1000;
            AddTextDescription.ucOpenPopUpWidth = 1000;

        }

        private void EnableDownload()
        {
            FileDownload31.Visible = true;
            FileDownload31.ucID = _artIdentity.ID;
            FileDownload31.ucAgency = _artIdentity.Agency;
            FileDownload31.ucVersion = _artIdentity.Version;
            FileDownload31.ucArtefactType = "DataFlow";
        }

        private void InitUserControl()
        {

            // Name User Control options
            AddTextName.ArtefactType = SdmxStructureEnumType.Dataflow;
            AddTextName.TType = TextType.NAME;

            // Description User Control options
            AddTextDescription.ArtefactType = SdmxStructureEnumType.Dataflow;
            AddTextDescription.TType = TextType.DESCRIPTION;

            GetDSD1.TargetWebControl = txtDSD;

            AnnotationGeneral.AddText_ucOpenTabName = null;
            AnnotationGeneral.AddText_ucOpenPopUpName = null;
            AnnotationGeneral.EditMode = true;

        }

        private void SetViewForm()
        {
            GetDfFromSession();
            EnableDownload();

            Categorisations.ucArtefactType = AvailableStructures.DATAFLOW;
            Categorisations.ucArtIdentity = _artIdentity;

            if (!IsPostBack)
            {
                //SetLabelDetail();
                pnlViewName.Visible = true;
                pnlViewDescription.Visible = true;
                GetDSD1.Visible = false;
                AnnotationGeneral.EditMode = false;

                BindData();
            }
        }

        private void SetEditForm()
        {
            GetDfFromSession();
            InitUserControl();
            EnableDownload();

            Categorisations.ucArtefactType = AvailableStructures.DATAFLOW;
            Categorisations.ucArtIdentity = _artIdentity;

            if (!IsPostBack)
            {
                //SetLabelDetail();
                SetEditingControl();
                BindData();
            }
        }

        private void SetInsertForm()
        {
            InitInsertObjects();
            InitUserControl();

            if (!IsPostBack)
            {
                SetEditingControl();
                BindData();
                FileDownload31.Visible = false;
            }
        }

        private void InitInsertObjects()
        {
            GetDfFromSession();

            if (_dfMutable == null)
                CreateEmptyDataFlow();
        }

        private void CreateEmptyDataFlow()
        {

            IDataflowMutableObject dfMutable;

            dfMutable = _sdmxUtils.buildDataFlow(_ntdString + "DSD_ID", _ntdString + "AGENCY", _ntdDSDVersion);
            dfMutable.AddName("en", _ntdString + "DSD_NAME");

            IStructureReference dsdRef = new StructureReferenceImpl(_ntdString,
                            _ntdString, "1.0",
                            SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd),
                            new string[] { _ntdString });

            dfMutable.DataStructureRef = dsdRef;

            _dfMutable = dfMutable;

            SetDfToSession();
        }


        /// <summary>
        /// Abilita i controlli per l'Editing
        /// </summary>
        private void SetEditingControl()
        {
            btnSaveDF.Visible = true;

            // GENERAL
            if (_action == Action.INSERT)
            {
                txtDFID.Enabled = true;
                txtVersion.Enabled = true;
                cmbAgencies.Enabled = true;
                cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                cmbAgencies.SelectedIndex = 0;
                AnnotationGeneral.ClearAnnotationsSession();
            }

            txtURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtDFName.Enabled = true;
            txtDFDescription.Enabled = true;
            chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;

        }

        //TODO: Riscrivere con le corrette inizializzazioni
        /// <summary>
        /// Imposta il title della pagina con il DF selezionato
        /// </summary>
        //private void SetLabelDetail()
        //{
        //    lblDetail.Text = String.Format("DataFlow: {0}, {1}, {2}", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
        //}

        private void BindData()
        {
            IDataflowObject df = _sdmxObjects.Dataflows.FirstOrDefault();

            if (df == null)
                return;

            if (_action == Action.VIEW)
            {
                _dfMutable = df.MutableInstance;
                SetDfToSession();
            }

            SetGeneralTab(df);
        }

        private void SetGeneralTab(IDataflowObject df)
        {
            if (!df.Id.Contains(_ntdString))
                txtDFID.Text = df.Id;

            cmbAgencies.SelectedValue = df.AgencyId;
            txtAgenciesReadOnly.Text = df.AgencyId;
            AnnotationGeneral.OwnerAgency = txtAgenciesReadOnly.Text;

            if (_action == Action.INSERT)
            {
                cmbAgencies.Visible = true;
                txtAgenciesReadOnly.Visible = false;
            }
            else
            {
                cmbAgencies.Visible = false;
                txtAgenciesReadOnly.Visible = true;
            }

            if (df.Version != _ntdDSDVersion)
                txtVersion.Text = df.Version;

            chkIsFinal.Checked = df.IsFinal.IsTrue;

            if (df.Uri != null)
                txtURI.Text = df.Uri.AbsoluteUri;

            if (df.Urn != null)
                txtURN.Text = df.Urn.AbsoluteUri;

            if (df.StartDate != null)
                txtValidFrom.Text = df.StartDate.Date.ToString();

            if (df.EndDate != null)
                txtValidTo.Text = df.EndDate.Date.ToString();

            var dataStructureRef = df.DataStructureRef;

            if (!dataStructureRef.AgencyId.Contains(_ntdString))
                txtDSD.Text = dataStructureRef.MaintainableId + "," + dataStructureRef.AgencyId + "," + dataStructureRef.Version;


            txtDFName.Text = _localizedUtils.GetNameableName(df);
            txtDFDescription.Text = _localizedUtils.GetNameableDescription(df);

            if (_action != Action.VIEW)
            {
                IDataflowMutableObject dfM = df.MutableInstance;

                foreach (ITextTypeWrapperMutableObject name in dfM.Names)
                {
                    if (name.Value.Contains(_ntdString))
                    {
                        dfM.Names.Remove(name);
                        break;
                    }
                }

                AddTextName.InitTextObjectMutableList = dfM.Names;
                AddTextDescription.InitTextObjectList = df.Descriptions;
            }

            AnnotationGeneral.AnnotationObjectList = df.MutableInstance.Annotations;

        }

        /// <summary>
        /// Recupera l'Action della pagina(Insert,update,view)
        /// </summary>
        private void SetAction()
        {
            if ((Request["ACTION"] == null || Utils.ViewMode) || (Request["ISFINAL"] != null && bool.Parse(Request["ISFINAL"])))
                _action = Action.VIEW;
            else
            {
                if (Request["ACTION"] == "UPDATE")
                {
                    IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;
                    _artIdentity = Utils.GetIdentityFromRequest(Request);
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
                {
                    _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                }
            }

            if (_action != Action.VIEW)
                AspConfirmationExit = "true";
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetDfFromSession()
        {
            if (Session[_snInsDf] != null)
                _dfMutable = (IDataflowMutableObject)Session[_snInsDf];

        }

        /// <summary>
        /// 
        /// </summary>
        private void SetDfToSession()
        {
            Session[_snInsDf] = _dfMutable;
        }

        private string GetAgencyValue()
        {
            if (_action == Action.INSERT)
            {
                return cmbAgencies.SelectedValue.ToString();
            }
            else
            {
                return txtAgenciesReadOnly.Text;
            }
        }

        #endregion


    }
}