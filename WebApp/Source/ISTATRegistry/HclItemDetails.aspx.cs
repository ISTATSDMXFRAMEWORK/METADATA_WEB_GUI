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
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using System.Data;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Reference;


namespace ISTATRegistry
{
    public partial class HclItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        #region Props

        ArtefactIdentity _artIdentity;
        WSModel _wsmodel = new WSModel();
        EntityMapper _entityMapper;
        LocalizedUtils _localizedUtils;
        SDMXUtils _sdmxUtils;
        Action _action;

        const string _snSdmxObjects = "SDMXObjects";

        #region Inserting Consts

        /// <summary>
        /// DF Inserting Session Name
        /// </summary>
        const string _snInsCC = "INSHcl";

        /// <summary>
        /// No True Data String
        /// </summary>
        const string _ntdString = "WeBrEgIsTrY";

        /// <summary>
        /// No True Data DSD Version
        /// </summary>
        const string _ntdCCVersion = "1010101010";

        #endregion

        #region Treeview

        private EditingAction _currentEditingAction;

        private enum EditingAction
        {
            Add,
            Cut,
            Copy,
            Paste
        }

        private EditingAction CurrentEditingAction
        {
            get
            {
                return _currentEditingAction;
            }
            set
            {
                _currentEditingAction = value;

                switch (_currentEditingAction)
                {
                    case EditingAction.Add:
                        btnCut.Enabled = true;
                        btnPaste.Enabled = false;
                        btnCancel.Enabled = false;
                        hdnCopiedNode.Value = "";
                        break;
                    case EditingAction.Cut:
                        btnCut.Enabled = false;
                        btnPaste.Enabled = true;
                        btnCancel.Enabled = true;
                        hdnCopiedNode.Value = TreeView1.SelectedNode.ValuePath;
                        break;
                    case EditingAction.Copy:
                        break;
                    case EditingAction.Paste:
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        private IHierarchicalCodelistMutableObject _hclMutable;

        protected DataTable _dtCodelists
        {
            get
            {
                if (Session["DTCODELISTSTS"] != null)
                    return (DataTable)Session["DTCODELISTSTS"];
                return null;
            }
            set
            {
                Session["DTCODELISTSTS"] = value;
                GVIncludeCodelistDataBind();
            }
        }

        /// <summary>
        /// Converte l'oggetto cc mutable in ISdmxObjects()
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
                            ISdmxObjects sdmxAppo = _wsmodel.GetHcl(_artIdentity, false, false);
                            Session[_snSdmxObjects] = sdmxAppo;
                        }
                        return (ISdmxObjects)Session[_snSdmxObjects];
                    case Action.UPDATE:
                        if (!IsPostBack)
                        {
                            ISdmxObjects sdmxAppo = _wsmodel.GetHcl(_artIdentity, false, false);
                            _hclMutable = sdmxAppo.HierarchicalCodelists.FirstOrDefault().MutableInstance;
                            SetHclToSession();
                        }
                        ISdmxObjects sdmxObjectsU = new SdmxObjectsImpl();
                        sdmxObjectsU.AddHierarchicalCodelist(_hclMutable.ImmutableInstance);
                        return sdmxObjectsU;
                    case Action.INSERT:
                        ISdmxObjects sdmxObjectsI = new SdmxObjectsImpl();
                        sdmxObjectsI.AddHierarchicalCodelist(_hclMutable.ImmutableInstance);
                        return sdmxObjectsI;
                }
                return null;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Org.Sdmxsource.Sdmx.Api.Exception.SdmxException.SetMessageResolver(new Org.Sdmxsource.Util.ResourceBundle.MessageDecoder());
            GetCodeList1.ucCLSelectedEH += new EventHandler<UserControls.GetCodeListEventArgs>(GetCodeList1_ucCLSelectedEH);


            _hclMutable = null;

            _sdmxUtils = new SDMXUtils();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _entityMapper = new EntityMapper(Utils.LocalizedLanguage);
            _artIdentity = Utils.GetIdentityFromRequest(Request);

            GetCodeList1.ucAddIconType = AddIconType.cross;

            SetAction();

            if (!IsPostBack)
            {
                CommonInit();
                CurrentEditingAction = EditingAction.Add;
                TreeView1.Nodes.Add(new TreeNode("root", "@root@"));
                TreeView1.Nodes[0].Selected = true;
            }

            EditingAction a = CurrentEditingAction;

            switch (_action)
            {
                case Action.INSERT:
                    SetInsertForm();
                    if (!Page.IsPostBack)
                    {
                        cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                        cmbAgencies.SelectedIndex = 0;
                    }
                    break;
                case Action.UPDATE:
                    SetEditForm();
                    break;
                case Action.VIEW:
                    SetViewForm();
                    Utils.ResetBeforeUnload();
                    break;
            }

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.HierarchicalCodelist;
            DuplicateArtefact1.ucMaintanableArtefact = _hclMutable;

        }

        void GetCodeList1_ucCLSelectedEH(object sender, UserControls.GetCodeListEventArgs e)
        {
            if (e.GetCLDataTable != null)
            {
                _dtCodelists = e.GetCLDataTable;
                GVIncludeCodelistDataBind();
                //if (IsPostBack)
                //PopolateConstraintComponents();
            }
        }


        /// <summary>
        /// Salva la HCL nel Registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveHcl_Click(object sender, EventArgs e)
        {

            if (!ValidateHclData())
                return;

            try
            {
                _hclMutable.Id = txtHclID.Text;
                _hclMutable.AgencyId = cmbAgencies.SelectedValue;
                _hclMutable.Version = txtVersion.Text;

                _hclMutable.Names.Clear();
                foreach (ITextTypeWrapperMutableObject name in AddTextName.TextObjectList)
                {
                    _hclMutable.Names.Add(name);
                }

                _hclMutable.Descriptions.Clear();
                if (AddTextDescription.TextObjectList != null)
                {
                    foreach (ITextTypeWrapperMutableObject descr in AddTextDescription.TextObjectList)
                    {
                        _hclMutable.Descriptions.Add(descr);
                    }
                }

                _hclMutable.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
                if (txtURI.Text != String.Empty)
                    _hclMutable.Uri = new Uri(txtURI.Text);

                if (txtValidFrom.Text != String.Empty)
                    _hclMutable.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (txtValidTo.Text != String.Empty)
                    _hclMutable.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                _hclMutable.Annotations.Clear();
                if (AnnotationGeneral.AnnotationObjectList != null)
                    foreach (var annotation in AnnotationGeneral.AnnotationObjectList)
                    {
                        _hclMutable.AddAnnotation(annotation);
                    }


                SetHclToSession();

                WSModel wsModel = new WSModel();
                XmlDocument xRet = wsModel.SubmitStructure(_sdmxObjects);

                string err = Utils.GetXMLResponseError(xRet);

                if (err != "")
                {
                    Utils.ShowDialog(err);
                    return;
                }

                Session[_snInsCC] = null;
                Session[_snSdmxObjects] = null;

                Utils.ResetBeforeUnload();
                Utils.AppendScript("location.href='./Hcl.aspx';");
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }

        protected void gvIncludedCodelist_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
            string ID = ((Label)gvr.FindControl("lblID")).Text;
            string Agency = ((Label)gvr.FindControl("lblAgency")).Text;
            string Version = ((Label)gvr.FindControl("lblVersion")).Text;

            switch (e.CommandName)
            {
                case "Delete":

                    _dtCodelists.Rows.Remove(_dtCodelists.Rows.Find(new String[] { ID, Agency, Version }));
                    _dtCodelists = _dtCodelists;

                    break;
                case "Details":
                    lblCLSelected.Text = ID + ", " + Agency + ", " + Version;
                    PopolateGVCodes(false);
                    break;
            }
        }


        protected void gvIncludedCodelist_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //PopolateConstraintComponents();
        }

        #region Treeview

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            TreeNode sNode = TreeView1.SelectedNode;
            if (sNode.Value == "@root@")
                return;
            sNode.Parent.ChildNodes.Remove(sNode);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            TreeView1.SelectedNode.ChildNodes.AddAt(TreeView1.SelectedNode.ChildNodes.Count, new TreeNode(TextBox1.Text, TextBox1.Text));
        }

        protected void btnCut_Click(object sender, EventArgs e)
        {
            CurrentEditingAction = EditingAction.Cut;
        }

        protected void btnPaste_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode sourceNode = TreeView1.FindNode(hdnCopiedNode.Value);
                TreeNode targetNode = TreeView1.SelectedNode;

                sourceNode.Parent.ChildNodes.Remove(sourceNode);

                targetNode.ChildNodes.AddAt(targetNode.ChildNodes.Count, sourceNode);

                //TreeView1.DataBind();

                CurrentEditingAction = EditingAction.Add;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CurrentEditingAction = EditingAction.Add;
        }

        protected void gvCodes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCodes.PageIndex = e.NewPageIndex;
            PopolateGVCodes(true);
        }

        protected void gvCodes_RowCommand(object sender, GridViewCommandEventArgs e)
        {


            switch (e.CommandName)
            {
                case "AddToHcl":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string Code = ((Label)gvr.FindControl("lblCode")).Text;

                    // Altri presi da label

                    break;
            }

        }

        protected void gvCodes_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }


        #endregion

        #endregion

        #region Methods


        private void PopolateGVCodes(bool fromCache)
        {
            ICodelistObject cl = null;

            if (!fromCache)
            {
                String[] identity = lblCLSelected.Text.Split(',');

                ISdmxObjects SdmxObjects = _wsmodel.GetCodeList(new ArtefactIdentity(identity[0].Trim(), identity[1].Trim(), identity[2].Trim()), false, false);
                cl = SdmxObjects.Codelists.FirstOrDefault();

                Cache.Insert("CLSELECTED", cl, null,
                            System.Web.Caching.Cache.NoAbsoluteExpiration,
                            TimeSpan.FromMinutes(20));
            }
            else
            {
                object cacheItem = Cache["CLSELECTED"] as ICodelistObject;
                if (cacheItem != null)
                    cl = (ICodelistObject)cacheItem;
            }

            gvCodes.DataSource = cl.Items;
            gvCodes.DataBind();
        }

        private bool ValidateHclData()
        {
            /*
             * 1. Effettuo i controlli prima del salvataggio e li visualizzo a video tramite un OpenP()
             *      General:
             *          ID obbligatorio
             *          Version obbligatorio
             *          Name Obbligatorio
             *          Controllo formale URI
             *          Controllo formale Date
            */

            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtHclID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo Agency
            if (cmbAgencies.SelectedValue == String.Empty)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
            }

            // Controllo versione
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo URI
            if (txtURI.Text != String.Empty && !ValidationUtils.CheckUriFormatNoRegExp(txtURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_uri_format + "<br /><br />";
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
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_from_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
            }

            // Controllo validità seconda data
            if (txtValidTo.Text != String.Empty && !ValidationUtils.CheckDateFormat(txtValidTo.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_to_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
            }

            // Controllo congruenza date
            if (checkForDatesCombination && txtValidFrom.Text != String.Empty && txtValidTo.Text != String.Empty)
            {
                if (!ValidationUtils.CheckDates(txtValidFrom.Text, txtValidTo.Text))
                {
                    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_diff + "<br /><br />";
                    errorCounter++;
                }
            }

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_save_contentconstraint);
                return false;
            }

            return true;
        }

        private void CommonInit()
        {
            Session[_snSdmxObjects] = null;
            Session[_snInsCC] = null;

            Utils.PopulateCmbAgencies(cmbAgencies, true);
        }

        private void EnableDownload()
        {
            FileDownload31.Visible = true;
            FileDownload31.ucID = _artIdentity.ID;
            FileDownload31.ucAgency = _artIdentity.Agency;
            FileDownload31.ucVersion = _artIdentity.Version;
            FileDownload31.ucArtefactType = "Hcl";
        }

        private void InitUserControl()
        {

            // Name User Control options
            AddTextName.ArtefactType = SdmxStructureEnumType.HierarchicalCodelist;
            AddTextName.TType = TextType.NAME;

            // Description User Control options
            AddTextDescription.ArtefactType = SdmxStructureEnumType.HierarchicalCodelist;
            AddTextDescription.TType = TextType.DESCRIPTION;

            AnnotationGeneral.AddText_ucOpenTabName = null;
            AnnotationGeneral.AddText_ucOpenPopUpName = null;
            AnnotationGeneral.EditMode = true;

        }

        private void SetViewForm()
        {
            GetHclFromSession();
            EnableDownload();

            if (!IsPostBack)
            {
                //SetLabelDetail();
                pnlViewName.Visible = true;
                pnlViewDescription.Visible = true;
                AnnotationGeneral.EditMode = false;

                BindData();
            }
        }

        private void SetEditForm()
        {

            GetHclFromSession();
            InitUserControl();
            EnableDownload();

            if (!IsPostBack)
            {
                SetEditingControl();
                BindData();
            }
            else
            {
                GetCodeList1.ucTargetDataTable = _dtCodelists;
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
            }
            else
            {
                GetCodeList1.ucTargetDataTable = _dtCodelists;
            }

        }

        private void InitInsertObjects()
        {
            GetHclFromSession();

            if (_hclMutable == null)
                CreateEmptyHcl();
        }

        private void CreateEmptyHcl()
        {

            IHierarchicalCodelistMutableObject hclMutable;

            hclMutable = _sdmxUtils.buildHcl(_ntdString + "HCLS_ID", _ntdString + "AGENCY", _ntdCCVersion);
            hclMutable.AddName("en", _ntdString + "HCL_NAME");

            _hclMutable = hclMutable;

            SetHclToSession();

            //hcl.CodelistRef[0]   // CL Reference(N)

            //hcl.Hierarchies[0].HierarchicalCodeObjects[0].CodeReference   //ref del nodo corrente
            //hcl.Hierarchies[0].HierarchicalCodeObjects[0].CodeRefs[0] // ref dei nodi figlio 

        }

        private void GVIncludeCodelistDataBind()
        {
            gvIncludedCodelist.DataSource = _dtCodelists;
            gvIncludedCodelist.DataBind();
        }

        /// <summary>
        /// Abilita i controlli per l'Editing
        /// </summary>
        private void SetEditingControl()
        {
            btnSaveHcl.Visible = true;

            // GENERAL
            if (_action == Action.INSERT)
            {
                txtHclID.Enabled = true;
                txtVersion.Enabled = true;
                cmbAgencies.Enabled = true;

                AnnotationGeneral.ClearAnnotationsSession();
            }

            txtURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtHclName.Enabled = true;
            txtHclDescription.Enabled = true;
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
            IHierarchicalCodelistObject hcl = _sdmxObjects.HierarchicalCodelists.FirstOrDefault();

            if (hcl == null)
                return;

            if (_action == Action.VIEW)
            {
                _hclMutable = hcl.MutableInstance;
                SetHclToSession();
            }

            SetGeneralTab(hcl);
            SetHierarchyTab(hcl);

        }

        private void SetHierarchyTab(IHierarchicalCodelistObject hcl)
        {
            SetHierarchyElementsTab();

        }

        private void SetGeneralTab(IHierarchicalCodelistObject hcl)
        {
            if (!hcl.Id.Contains(_ntdString))
                txtHclID.Text = hcl.Id;

            cmbAgencies.SelectedValue = hcl.AgencyId;

            if (hcl.Version != _ntdCCVersion)
                txtVersion.Text = hcl.Version;

            chkIsFinal.Checked = hcl.IsFinal.IsTrue;

            if (hcl.Uri != null)
                txtURI.Text = hcl.Uri.AbsoluteUri;

            if (!hcl.Urn.ToString().Contains(_ntdString))
                txtURN.Text = hcl.Urn.AbsoluteUri;

            if (hcl.StartDate != null)
                txtValidFrom.Text = hcl.StartDate.Date.ToString();

            if (hcl.EndDate != null)
                txtValidTo.Text = hcl.EndDate.Date.ToString();

            txtHclName.Text = _localizedUtils.GetNameableName(hcl);
            txtHclDescription.Text = _localizedUtils.GetNameableDescription(hcl);

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.Visible = true;
            }

            if (_action != Action.VIEW)
            {
                IHierarchicalCodelistMutableObject hclM = hcl.MutableInstance;

                foreach (ITextTypeWrapperMutableObject name in hclM.Names)
                {
                    if (name.Value.Contains(_ntdString))
                    {
                        hclM.Names.Remove(name);
                        break;
                    }
                }

                AddTextName.InitTextObjectMutableList = hclM.Names;
                AddTextDescription.InitTextObjectList = hcl.Descriptions;
            }

            AnnotationGeneral.AnnotationObjectList = hcl.MutableInstance.Annotations;

        }

        /// <summary>
        /// Recupera l'Action della pagina(Insert,update,view)
        /// </summary>
        private void SetAction()
        {
            if (Request["ACTION"] == null || Utils.ViewMode)
            {
                _action = Action.VIEW;
                return;
            }

            if (Request["ISFINAL"] != null && bool.Parse(Request["ISFINAL"]))
                _action = Action.VIEW;
            else
                _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHclFromSession()
        {
            if (Session[_snInsCC] != null)
                _hclMutable = (IHierarchicalCodelistMutableObject)Session[_snInsCC];

        }

        /// <summary>
        /// 
        /// </summary>
        private void SetHclToSession()
        {
            Session[_snInsCC] = _hclMutable;
        }

        private void SetHierarchyElementsTab()
        {
            ArtefactTypeInit();
            //PopolateConstraintComponents();
        }

        private void ArtefactTypeInit()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Agency");
            dt.Columns.Add("Version");
            dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"], dt.Columns["Agency"], dt.Columns["Version"] };

            if (_hclMutable.CodelistRef != null)
            {
                foreach (ICodelistRefMutableObject clr in _hclMutable.CodelistRef)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = clr.CodelistReference.MaintainableId;
                    dr["Agency"] = clr.CodelistReference.AgencyId;
                    dr["Version"] = clr.CodelistReference.Version;
                    dt.Rows.Add(dr);
                }
            }
            _dtCodelists = dt;
            GetCodeList1.ucTargetDataTable = _dtCodelists;
        }


        #endregion

    }
}