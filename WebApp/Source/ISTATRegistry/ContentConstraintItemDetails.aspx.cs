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
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class ContentConstraintItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        #region Props

        ArtefactIdentity _artIdentity;
        WSModel _wsmodel = new WSModel();
        EntityMapper _entityMapper;
        LocalizedUtils _localizedUtils;
        SDMXUtils _sdmxUtils;
        Action _action;
        DataTable _dtDimension;
        const string _DimensionDTName = "DimensionDataTable";

        protected string AspConfirmationExit = "false";
        const string _snSdmxObjects = "SDMXObjects";

        #region Inserting Consts

        /// <summary>
        /// DF Inserting Session Name
        /// </summary>
        const string _snInsCC = "INSCC";

        /// <summary>
        /// No True Data String
        /// </summary>
        const string _ntdString = "WeBrEgIsTrY";

        /// <summary>
        /// No True Data DSD Version
        /// </summary>
        const string _ntdCCVersion = "1010101010";

        #endregion

        private IContentConstraintMutableObject _ccMutable;

        protected DataTable _dtArtefacts
        {
            get
            {
                if (Session["DTARTEFACTS"] != null)
                    return (DataTable)Session["DTARTEFACTS"];
                return null;
            }
            set
            {
                Session["DTARTEFACTS"] = value;
                GVIncludeArtefactDataBind();
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
                            ISdmxObjects sdmxAppo = _wsmodel.GetContentConstraint(_artIdentity, false, false);
                            Session[_snSdmxObjects] = sdmxAppo;
                        }
                        return (ISdmxObjects)Session[_snSdmxObjects];
                    case Action.UPDATE:
                        if (!IsPostBack)
                        {
                            ISdmxObjects sdmxAppo = _wsmodel.GetContentConstraint(_artIdentity, false, false);
                            _ccMutable = sdmxAppo.ContentConstraintObjects.FirstOrDefault().MutableInstance;
                            SetCCToSession();
                        }
                        ISdmxObjects sdmxObjectsU = new SdmxObjectsImpl();
                        sdmxObjectsU.AddContentConstraintObject(_ccMutable.ImmutableInstance);
                        return sdmxObjectsU;
                    case Action.INSERT:
                        ISdmxObjects sdmxObjectsI = new SdmxObjectsImpl();
                        sdmxObjectsI.AddContentConstraintObject(_ccMutable.ImmutableInstance);
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
            GetDSD1.ucDSDSelectedEH += new EventHandler<UserControls.GetDSDEventArgs>(GetDSD1_ucDSDSelectedEH);
            GetDataFlow1.ucDFSelectedEH += new EventHandler<UserControls.GetDFEventArgs>(GetDataFlow1_ucDFSelectedEH);

            _ccMutable = null;

            _sdmxUtils = new SDMXUtils();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _entityMapper = new EntityMapper(Utils.LocalizedLanguage);
            _artIdentity = Utils.GetIdentityFromRequest(Request);

            GetDSD1.ucAddIconType = AddIconType.cross;
            GetDataFlow1.ucAddIconType = AddIconType.cross;

            SetAction();

            if (!IsPostBack)
            {
                CommonInit();
            }

            switch (_action)
            {
                case Action.INSERT:
                    SetInsertForm();
                    if ( !Page.IsPostBack )
                    {
                        cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                        cmbAgencies.SelectedIndex = 0;
                        FileDownload31.Visible = false;
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

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.ContentConstraint;
            DuplicateArtefact1.ucMaintanableArtefact = _ccMutable;

        }

        private void ForceSingleConstraintAttachement()
        {
            GetDimensioneDataTableFromCache();

            if (_dtDimension == null || _dtDimension.Rows.Count <= 0)
            {
                GetDSD1.Visible = true;
                GetDataFlow1.Visible = true;
            }
            else
            {
                GetDSD1.Visible = false;
                GetDataFlow1.Visible = false;
            }
        }

        protected void btnSaveComponents_Click(object sender, EventArgs e)
        {
            IKeyValuesMutable kv;

            if (_ccMutable.IncludedCubeRegion == null)
            {
                _ccMutable.IncludedCubeRegion = new CubeRegionMutableCore();
            }

            switch (hdnSelectedComponentType.Value)
            {
                case "Dimension":
                    kv = _ccMutable.IncludedCubeRegion.KeyValues.Where(c => c.Id == lblComponentSelectedID.Text).FirstOrDefault();

                    if (kv != null)
                        _ccMutable.IncludedCubeRegion.KeyValues.Remove(kv);

                    PopolateKeyValue(ref kv);

                    if (kv.KeyValues.Count > 0)
                        _ccMutable.IncludedCubeRegion.AddKeyValue(kv);

                    break;
                case "Attribute":
                    kv = _ccMutable.IncludedCubeRegion.AttributeValues.Where(c => c.Id == lblComponentSelectedID.Text).FirstOrDefault();

                    if (kv != null)
                        _ccMutable.IncludedCubeRegion.AttributeValues.Remove(kv);

                    PopolateKeyValue(ref kv);

                    if (kv.KeyValues.Count > 0)
                        _ccMutable.IncludedCubeRegion.AddAttributeValue(kv);
                    break;
            }

            SetCCToSession();

            if (lbSource.Items.Count > 0 || lbTarget.Items.Count > 0)
            {
                PopolateGVDimension();
                //Utils.ShowDialog(Resources.Messages.succ_operation);
            }
        }

        protected void gvDimension_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDimension.PageIndex = e.NewPageIndex;
            PopolateConstraintComponents();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            List<ListItem> lListItems = new List<ListItem>();

            if (lbSource.SelectedItem != null)
            {
                foreach (ListItem li in lbSource.Items)
                    if (li.Selected)
                    {
                        lbTarget.Items.Add(li.Value);
                        lListItems.Add(li);
                    }
                foreach (ListItem liR in lListItems)
                {
                    lbSource.Items.Remove(liR);
                }
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            List<ListItem> lListItems = new List<ListItem>();

            if (lbTarget.SelectedItem != null)
            {
                foreach (ListItem li in lbTarget.Items)
                    if (li.Selected)
                    {
                        lbSource.Items.Add(li.Value);
                        lListItems.Add(li);
                    }
                foreach (ListItem liR in lListItems)
                {
                    lbTarget.Items.Remove(liR);
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

        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            foreach (ListItem li in lbTarget.Items)
                lbSource.Items.Add(li.Value);

            lbTarget.Items.Clear();
        }

        void GetDataFlow1_ucDFSelectedEH(object sender, UserControls.GetDFEventArgs e)
        {
            if (e.GetDFDataTable != null)
            {
                _dtArtefacts = e.GetDFDataTable;
                if (IsPostBack)
                    PopolateConstraintComponents();
            }
        }

        void GetDSD1_ucDSDSelectedEH(object sender, UserControls.GetDSDEventArgs e)
        {
            if (e.GetDSDDataTable != null)
            {
                _dtArtefacts = e.GetDSDDataTable;
                if (IsPostBack)
                    PopolateConstraintComponents();
            }
        }

        protected void gvIncludedArtefact_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Delete":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string ID = ((Label)gvr.FindControl("lblID")).Text;
                    string Agency = ((Label)gvr.FindControl("lblAgency")).Text;
                    string Version = ((Label)gvr.FindControl("lblVersion")).Text;

                    _dtArtefacts.Rows.Remove(_dtArtefacts.Rows.Find(new String[] { ID, Agency, Version }));
                    _dtArtefacts = _dtArtefacts;

                    break;
            }
        }

        protected void gvDimension_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "View":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string ID = ((Label)gvr.FindControl("lblID")).Text;
                    string strClIdentity = ((Label)gvr.FindControl("lblCodelist")).Text;
                    string componentType = ((Label)gvr.FindControl("lblComponentType")).Text;

                    string[] arIdentity = strClIdentity.Split(',');

                    lbSource.Items.Clear();
                    lbTarget.Items.Clear();

                    //lblComponentSelected.Visible = true;
                    lblComponentSelectedID.Text = ID;
                    hdnCLSelected.Value = strClIdentity;
                    hdnSelectedComponentType.Value = componentType;

                    ISdmxObjects SdmxObject = _wsmodel.GetCodeList(new ArtefactIdentity(arIdentity[0].Trim(), arIdentity[1].Trim(), arIdentity[2].Trim()), false, false);

                    ICodelistObject cl = SdmxObject.Codelists.FirstOrDefault();
                    foreach (ICode code in cl.Items)
                    {
                        lbSource.Items.Add(code.Id);
                    }

                    if (componentType == "Dimension")
                        MoveSelectedDimensions(ID);
                    else if (componentType == "Attribute")
                        MoveSelectedAttributes(ID);

                    DeleteSelectedComponent();

                    break;
            }

        }

        protected void gvIncludedArtefact_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            PopolateConstraintComponents();
        }

        protected void cmbArtefactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectArtefactTypeElement();
            _dtArtefacts.Clear();
            _dtArtefacts = _dtArtefacts;

            ClearConstraintComponents();

            PopolateConstraintComponents();
        }

        /// <summary>
        /// Salva il DataFlow nel registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveCC_Click(object sender, EventArgs e)
        {

            if (!ValidateCCData())
                return;

            try
            {
                _ccMutable.Id = txtCCID.Text;
                _ccMutable.AgencyId = cmbAgencies.SelectedValue;
                _ccMutable.Version = txtVersion.Text;

                _ccMutable.Names.Clear();
                foreach (ITextTypeWrapperMutableObject name in AddTextName.TextObjectList)
                {
                    _ccMutable.Names.Add(name);
                }

                _ccMutable.Descriptions.Clear();
                if (AddTextDescription.TextObjectList != null)
                {
                    foreach (ITextTypeWrapperMutableObject descr in AddTextDescription.TextObjectList)
                    {
                        _ccMutable.Descriptions.Add(descr);
                    }
                }

                _ccMutable.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
                if (txtURI.Text != String.Empty)
                    _ccMutable.Uri = new Uri(txtURI.Text);

                if (txtValidFrom.Text != String.Empty)
                    _ccMutable.StartDate = DateTime.ParseExact(txtValidFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (txtValidTo.Text != String.Empty)
                    _ccMutable.EndDate = DateTime.ParseExact(txtValidTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                _ccMutable.Annotations.Clear();
                if (AnnotationGeneral.AnnotationObjectList != null)
                    foreach (var annotation in AnnotationGeneral.AnnotationObjectList)
                    {
                        _ccMutable.AddAnnotation(annotation);
                    }

                PopolateConstraintAttachments();
                PopolateReleaseCaledar();

                SetCCToSession();

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

                //Utils.ShowDialog("Operation succesfully");
                Utils.ResetBeforeUnload();
                Utils.AppendScript("location.href='./ContentConstraint.aspx';");
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }

        private void PopolateReleaseCaledar()
        {
            _ccMutable.ReferencePeriod = new ReferencePeriodMutableCore();
            _ccMutable.ReferencePeriod.StartTime = DateTime.Parse("01/01/2001");
            _ccMutable.ReferencePeriod.EndTime = DateTime.Parse("01/01/2001");

            _ccMutable.ReleaseCalendar = new ReleaseCalendarMutableCore();

            _ccMutable.ReleaseCalendar.Periodicity = "P" + txtPeriodicity.Text + cmbPeriodicity.SelectedValue;
            _ccMutable.ReleaseCalendar.Offset = "P" + txtOffset.Text + cmbOffset.SelectedValue;
            _ccMutable.ReleaseCalendar.Tolerance = "P" + txtTolerance.Text + cmbTolerance.SelectedValue;
        }

        private void PopolateConstraintAttachments()
        {
            _ccMutable.ConstraintAttachment = new ContentConstraintAttachmentMutableCore();
            //if (_ccMutable.ConstraintAttachment != null)
            //    _ccMutable.ConstraintAttachment.StructureReference.Clear();
            //else
            //    _ccMutable.ConstraintAttachment = new ContentConstraintAttachmentMutableCore();

            IStructureReference structRef;

            foreach (GridViewRow row in gvIncludedArtefact.Rows)
            {
                string ID = ((Label)row.FindControl("lblID")).Text;
                string Agency = ((Label)row.FindControl("lblAgency")).Text;
                string Version = ((Label)row.FindControl("lblVersion")).Text;

                structRef = new StructureReferenceImpl(Agency, ID, Version,
                            SdmxStructureType.GetFromEnum((SdmxStructureEnumType)Enum.Parse(typeof(SdmxStructureEnumType), cmbArtefactType.SelectedValue)),
                            new string[] { });

                _ccMutable.ConstraintAttachment.AddStructureReference(structRef);
            }
        }

        protected void gvDimension_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ID = ((Label)e.Row.FindControl("lblID")).Text;
                string ComponentType = ((Label)e.Row.FindControl("lblComponentType")).Text;
                Label lblDimNumber = (Label)e.Row.FindControl("lblDimNumber");

                IKeyValuesMutable kv = null;

                switch (ComponentType)
                {
                    case "Dimension":
                        if (_ccMutable.IncludedCubeRegion != null && _ccMutable.IncludedCubeRegion.KeyValues != null)
                            kv = _ccMutable.IncludedCubeRegion.KeyValues.Where(c => c.Id == ID).FirstOrDefault();

                        if (kv != null && kv.KeyValues != null && kv.KeyValues.Count > 0)
                            lblDimNumber.Text = kv.KeyValues.Count.ToString();

                        break;
                    case "Attribute":
                        if (_ccMutable.IncludedCubeRegion != null && _ccMutable.IncludedCubeRegion.AttributeValues != null)
                            kv = _ccMutable.IncludedCubeRegion.AttributeValues.Where(c => c.Id == ID).FirstOrDefault();

                        if (kv != null && kv.KeyValues != null && kv.KeyValues.Count > 0)
                            lblDimNumber.Text = kv.KeyValues.Count.ToString();

                        break;
                }
            }
        }

        #endregion

        #region Methods

        private void ClearConstraintComponents()
        {
            if (_ccMutable.IncludedCubeRegion != null)
            {
                _ccMutable.IncludedCubeRegion.KeyValues.Clear();
                _ccMutable.IncludedCubeRegion.AttributeValues.Clear();
            }

            lbSource.Items.Clear();
            lbTarget.Items.Clear();

            lblComponentSelectedID.Text = "";
        }

        private void PopolateKeyValue(ref IKeyValuesMutable kv)
        {
            kv = new KeyValuesMutableImpl();
            kv.Id = lblComponentSelectedID.Text;

            foreach (ListItem li in lbTarget.Items)
            {
                kv.AddValue(li.Value);
            }
        }

        private void MoveSelectedDimensions(string componentID)
        {
            foreach (ListItem li in lbSource.Items)
            {
                if (_ccMutable.IncludedCubeRegion != null)
                {
                    IKeyValuesMutable kv = _ccMutable.IncludedCubeRegion.KeyValues.Where(c => c.Id == componentID).FirstOrDefault();
                    if (kv != null)
                        if (kv.KeyValues.Where(c => c == li.Value).FirstOrDefault() != null)
                            lbTarget.Items.Add(li.Value);
                }
            }
        }

        private void MoveSelectedAttributes(string componentID)
        {
            foreach (ListItem li in lbSource.Items)
            {
                if (_ccMutable.IncludedCubeRegion != null)
                {
                    IKeyValuesMutable kv = _ccMutable.IncludedCubeRegion.AttributeValues.Where(c => c.Id == componentID).FirstOrDefault();
                    if (kv != null)
                        if (kv.KeyValues.Where(c => c == li.Value).FirstOrDefault() != null)
                            lbTarget.Items.Add(li.Value);
                }
            }
        }

        private void DeleteSelectedComponent()
        {
            foreach (ListItem li in lbTarget.Items)
                lbSource.Items.Remove(li.Value);
        }

        private void PopolateConstraintComponents()
        {
            ISdmxObjects sdmxObjects;

            _dtDimension = new DataTable();

            _dtDimension.Columns.Add("ID");
            _dtDimension.Columns.Add("Codelist");
            _dtDimension.Columns.Add("ComponentType");
            _dtDimension.PrimaryKey = new DataColumn[] { _dtDimension.Columns["ID"], _dtDimension.Columns["Codelist"] };

            foreach (GridViewRow row in gvIncludedArtefact.Rows)
            {
                ArtefactIdentity artIdentity = Utils.GetRowKey(row);

                switch (cmbArtefactType.SelectedValue)
                {
                    case "Dsd":
                        sdmxObjects = _wsmodel.GetDataStructure(artIdentity, false, false);
                        FillComponentDataTable(sdmxObjects.DataStructures.FirstOrDefault(), _dtDimension);
                        break;
                    case "Dataflow":
                        sdmxObjects = _wsmodel.GetDataFlow(artIdentity, false, false);
                        IDataflowObject df = sdmxObjects.Dataflows.FirstOrDefault();
                        sdmxObjects = _wsmodel.GetDataStructure(new ArtefactIdentity(df.DataStructureRef.MaintainableId,
                                                                                     df.DataStructureRef.AgencyId,
                                                                                     df.DataStructureRef.Version), false, false);

                        FillComponentDataTable(sdmxObjects.DataStructures.FirstOrDefault(), _dtDimension);
                        break;
                }

            }

            SetDimensioneDataTableToCache();
            PopolateGVDimension();

            if (gvIncludedArtefact.Rows.Count == 1)
            {
                GetDSD1.Visible = false;
                GetDataFlow1.Visible = false;
            }
            else
                SelectArtefactTypeElement();

            if (IsPostBack)
                RemoveOrphanComponents(_dtDimension);
        }

        private void SetDimensioneDataTableToCache()
        {
            Cache.Insert(_DimensionDTName, _dtDimension, null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(20));
        }

        private void GetDimensioneDataTableFromCache()
        {
            object cacheItem = Cache[_DimensionDTName] as DataTable;

            if (cacheItem != null)
                _dtDimension = (DataTable)cacheItem;
        }


        private void PopolateGVDimension()
        {
            GetDimensioneDataTableFromCache();

            if (_dtDimension != null)
                gvDimension.DataSource = _dtDimension;
            
            gvDimension.DataBind();
        }

        private void RemoveOrphanComponents(DataTable dt)
        {
            if (_ccMutable.IncludedCubeRegion == null)
                return;


            List<IKeyValuesMutable> lRemoveKey = new List<IKeyValuesMutable>();


            foreach (IKeyValuesMutable key in _ccMutable.IncludedCubeRegion.KeyValues)
            {
                lRemoveKey.Add(key);

                var x = dt.AsEnumerable().Where(row => row.Field<String>("ID") == key.Id).FirstOrDefault();
                if (x != null)
                {
                    lRemoveKey.Remove(key);
                }
            }

            foreach (IKeyValuesMutable kr in lRemoveKey)
            {
                _ccMutable.IncludedCubeRegion.KeyValues.Remove(kr);
            }

            foreach (IKeyValuesMutable key in _ccMutable.IncludedCubeRegion.AttributeValues)
            {
                lRemoveKey.Add(key);

                var x = dt.AsEnumerable().Where(row => row.Field<String>("ID") == key.Id).FirstOrDefault();
                if (x != null)
                {
                    lRemoveKey.Remove(key);
                }
            }

            foreach (IKeyValuesMutable kr in lRemoveKey)
            {
                _ccMutable.IncludedCubeRegion.AttributeValues.Remove(kr);
            }

            SetCCToSession();
        }

        private void FillComponentDataTable(IDataStructureObject dsd, DataTable dt)
        {
            DataRow dr;

            foreach (IDimension dim in dsd.DimensionList.Dimensions)
            {
                if (!dim.TimeDimension && dim.Representation != null)
                {
                    dr = dt.NewRow();
                    dr["ID"] = dim.Id;
                    dr["Codelist"] = dim.Representation.Representation.MaintainableId + ", " + dim.Representation.Representation.AgencyId + ", " + dim.Representation.Representation.Version;
                    dr["ComponentType"] = "Dimension";
                    try
                    {
                        dt.Rows.Add(dr);
                    }
                    catch (ConstraintException ce)
                    {
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            foreach (IAttributeObject att in dsd.AttributeList.Attributes)
            {
                if (att.Representation != null)
                {
                    dr = dt.NewRow();
                    dr["ID"] = att.Id;
                    if (att.Representation.Representation != null)
                        dr["Codelist"] = att.Representation.Representation.MaintainableId + ", " + att.Representation.Representation.AgencyId + ", " + att.Representation.Representation.Version;
                    else
                        dr["Codelist"] = "";
                    dr["ComponentType"] = "Attribute";
                    try
                    {
                        dt.Rows.Add(dr);
                    }
                    catch (ConstraintException ce)
                    {
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        private void GVIncludeArtefactDataBind()
        {
            gvIncludedArtefact.DataSource = _dtArtefacts;
            gvIncludedArtefact.DataBind();
        }


        private bool ValidateCCData()
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
            if (!ValidationUtils.CheckIdFormat(txtCCID.Text))
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

            // Controllo presenza Constraint Attachment
            if (gvIncludedArtefact.Rows.Count <= 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_constraintattachment + "<br /><br />";
                errorCounter++;
            }

            // ++++  RELEASE CALENDAR ++++ 
            if (txtPeriodicity.Text.Trim() == String.Empty || !ValidationUtils.CheckIntegerFormat(txtPeriodicity.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_periodicity + "<br /><br />";
                errorCounter++;
            }
            if (txtOffset.Text.Trim() == String.Empty || !ValidationUtils.CheckIntegerFormat(txtOffset.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_offset + "<br /><br />";
                errorCounter++;
            }
            if (txtTolerance.Text.Trim() == String.Empty || !ValidationUtils.CheckIntegerFormat(txtTolerance.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_tolerance + "<br /><br />";
                errorCounter++;
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

            cmbArtefactType.Items.Add(new ListItem("Data Structure Definition", "Dsd"));
            cmbArtefactType.Items.Add(new ListItem("DataFlow", "Dataflow"));
            Utils.PopulateCmbAgencies(cmbAgencies, true);
            Utils.PopulatECmbReleaseCalendar(cmbPeriodicity);
            Utils.PopulatECmbReleaseCalendar(cmbOffset);
            Utils.PopulatECmbReleaseCalendar(cmbTolerance);

        }

        private void ArtefactTypeInit()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Agency");
            dt.Columns.Add("Version");
            dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"], dt.Columns["Agency"], dt.Columns["Version"] };

            if (_ccMutable.ConstraintAttachment != null)
            {
                SdmxStructureType strucType = _ccMutable.ConstraintAttachment.StructureReference.FirstOrDefault().TargetReference;
                cmbArtefactType.SelectedValue = strucType.EnumType.ToString();
            }

            if (_ccMutable.ConstraintAttachment != null)
            {
                foreach (IStructureReference str in _ccMutable.ConstraintAttachment.StructureReference)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = str.MaintainableId;
                    dr["Agency"] = str.AgencyId;
                    dr["Version"] = str.Version;
                    dt.Rows.Add(dr);
                }
            }
            _dtArtefacts = dt;
            GetDSD1.ucTargetDataTable = _dtArtefacts;
            GetDataFlow1.ucTargetDataTable = _dtArtefacts;
        }

        private void EnableDownload()
        {
            FileDownload31.Visible = true;
            FileDownload31.ucID = _artIdentity.ID;
            FileDownload31.ucAgency = _artIdentity.Agency;
            FileDownload31.ucVersion = _artIdentity.Version;
            FileDownload31.ucArtefactType = "ContentConstraint";
        }

        private void InitUserControl()
        {

            // Name User Control options
            AddTextName.ArtefactType = SdmxStructureEnumType.ContentConstraint;
            AddTextName.TType = TextType.NAME;

            // Description User Control options
            AddTextDescription.ArtefactType = SdmxStructureEnumType.ContentConstraint;
            AddTextDescription.TType = TextType.DESCRIPTION;

            AnnotationGeneral.AddText_ucOpenTabName = null;
            AnnotationGeneral.AddText_ucOpenPopUpName = null;
            AnnotationGeneral.EditMode = true;

        }

        private void SetViewForm()
        {
            GetCCFromSession();
            EnableDownload();

            if (!IsPostBack)
            {
                //SetLabelDetail();
                pnlViewName.Visible = true;
                pnlViewDescription.Visible = true;
                AnnotationGeneral.EditMode = false;
                cmbArtefactType.Enabled = false;
                GetDSD1.Visible = false;
                GetDataFlow1.Visible = false;
                btnAdd.Visible = false;
                btnRemove.Visible = false;
                btnRemoveAll.Visible = false;
                btnSaveComponents.Visible = false;
                lbSource.Visible = false;
                lbTarget.Enabled = false;

                gvIncludedArtefact.Columns[4].Visible = false;

                BindData();
            }
        }

        private void SetEditForm()
        {
            //if (_dtArtefacts != null)
            //{
            //    gvIncludedArtefact.DataSource = _dtArtefacts;
            //    gvIncludedArtefact.DataBind();
            //}

            GetCCFromSession();
            InitUserControl();
            EnableDownload();

            if (!IsPostBack)
            {
                //SetLabelDetail();
                SetEditingControl();
                BindData();
            }
            else
            {
                GetDSD1.ucTargetDataTable = _dtArtefacts;
                GetDataFlow1.ucTargetDataTable = _dtArtefacts;
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
                GetDSD1.ucTargetDataTable = _dtArtefacts;
                GetDataFlow1.ucTargetDataTable = _dtArtefacts;
            }
        }

        private void InitInsertObjects()
        {
            GetCCFromSession();

            if (_ccMutable == null)
                CreateEmptyContentConstraint();
        }

        private void CreateEmptyContentConstraint()
        {

            IContentConstraintMutableObject ccMutable;

            ccMutable = _sdmxUtils.buildContentConstraint(_ntdString + "CCS_ID", _ntdString + "AGENCY", _ntdCCVersion);
            ccMutable.AddName("en", _ntdString + "CC_NAME");

            _ccMutable = ccMutable;

            SetCCToSession();
        }


        /// <summary>
        /// Abilita i controlli per l'Editing
        /// </summary>
        private void SetEditingControl()
        {
            btnSaveCC.Visible = true;

            // GENERAL
            if (_action == Action.INSERT)
            {
                txtCCID.Enabled = true;
                txtVersion.Enabled = true;
                cmbAgencies.Enabled = true;

                AnnotationGeneral.ClearAnnotationsSession();
            }

            txtURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtCCName.Enabled = true;
            txtCCDescription.Enabled = true;
            chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;
            txtPeriodicity.Enabled = true;
            txtOffset.Enabled = true;
            txtTolerance.Enabled = true;
            cmbPeriodicity.Enabled = true;
            cmbOffset.Enabled = true;
            cmbTolerance.Enabled = true;

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
            IContentConstraintObject cc = _sdmxObjects.ContentConstraintObjects.FirstOrDefault();

            if (cc == null)
                return;

            if (_action == Action.VIEW)
            {
                _ccMutable = cc.MutableInstance;
                SetCCToSession();
            }

            SetGeneralTab(cc);
            SetConstraintElementsTab();
            SetReleaseCalendarTab();
        }

        private void SetReleaseCalendarTab()
        {
            if (_ccMutable.ReleaseCalendar == null)
                return;

            string periodicity = _ccMutable.ReleaseCalendar.Periodicity;
            string offset = _ccMutable.ReleaseCalendar.Offset;
            string tolerance = _ccMutable.ReleaseCalendar.Tolerance;

            cmbPeriodicity.SelectedValue = periodicity.Substring(periodicity.Length - 1, 1);
            cmbOffset.SelectedValue = offset.Substring(offset.Length - 1, 1);
            cmbTolerance.SelectedValue = tolerance.Substring(tolerance.Length - 1, 1);

            txtPeriodicity.Text = periodicity.Substring(1, periodicity.Length - 2);
            txtOffset.Text = offset.Substring(1, offset.Length - 2);
            txtTolerance.Text = tolerance.Substring(1, tolerance.Length - 2);

        }

        private void SetConstraintElementsTab()
        {
            ArtefactTypeInit();
            PopolateConstraintComponents();
        }

        private void SetGeneralTab(IContentConstraintObject cc)
        {
            if (!cc.Id.Contains(_ntdString))
                txtCCID.Text = cc.Id;

            cmbAgencies.SelectedValue = cc.AgencyId;
            txtAgenciesReadOnly.Text = cc.AgencyId;

            AnnotationGeneral.OwnerAgency =  txtAgenciesReadOnly.Text;

            if (cc.Version != _ntdCCVersion)
                txtVersion.Text = cc.Version;

            chkIsFinal.Checked = cc.IsFinal.IsTrue;

            if (cc.Uri != null)
                txtURI.Text = cc.Uri.AbsoluteUri;

            if (!cc.Urn.ToString().Contains(_ntdString))
                txtURN.Text = cc.Urn.AbsoluteUri;

            if (cc.StartDate != null)
                txtValidFrom.Text = cc.StartDate.Date.ToString();

            if (cc.EndDate != null)
                txtValidTo.Text = cc.EndDate.Date.ToString();

            txtCCName.Text = _localizedUtils.GetNameableName(cc);
            txtCCDescription.Text = _localizedUtils.GetNameableDescription(cc);

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.Visible = true;
            }

            if (_action != Action.VIEW)
            {
                IContentConstraintMutableObject ccM = cc.MutableInstance;

                foreach (ITextTypeWrapperMutableObject name in ccM.Names)
                {
                    if (name.Value.Contains(_ntdString))
                    {
                        ccM.Names.Remove(name);
                        break;
                    }
                }

                AddTextName.InitTextObjectMutableList = ccM.Names;
                AddTextDescription.InitTextObjectList = cc.Descriptions;
            }

            AnnotationGeneral.AnnotationObjectList = cc.MutableInstance.Annotations;

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

            if (_action != Action.VIEW)
                AspConfirmationExit = "true";
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCCFromSession()
        {
            if (Session[_snInsCC] != null)
                _ccMutable = (IContentConstraintMutableObject)Session[_snInsCC];

        }

        /// <summary>
        /// 
        /// </summary>
        private void SetCCToSession()
        {
            Session[_snInsCC] = _ccMutable;
        }

        private void SelectArtefactTypeElement()
        {
            if (cmbArtefactType.SelectedValue == "Dsd")
            {
                GetDataFlow1.Visible = false;
                GetDSD1.Visible = true;
            }
            else if (cmbArtefactType.SelectedValue == "Dataflow")
            {
                GetDataFlow1.Visible = true;
                GetDSD1.Visible = false;
            }
        }

        #endregion



    }
}