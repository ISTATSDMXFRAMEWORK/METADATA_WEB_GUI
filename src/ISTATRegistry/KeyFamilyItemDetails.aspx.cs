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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using System.Collections;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System.Xml;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using System.Globalization;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class keyFamilyItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {
        #region Props

        ArtefactIdentity _artIdentity;
        WSModel _wsmodel = new WSModel();
        EntityMapper _entityMapper;
        LocalizedUtils _localizedUtils;
        SDMXUtils _sdmxUtils;
        Action _action;
        EndPointElement _epe;


        protected string AspConfirmationExit = "false";

        const string _snSdmxObjects = "SDMXObjexts";

        #region Inserting Consts

        /// <summary>
        /// DSD Inserting Session Name
        /// </summary>
        const string _snInsDSD = "INSDSD";

        /// <summary>
        /// Dimension list Session Name
        /// </summary>
        const string _snInsDimensionList = "INSDL";

        /// <summary>
        /// No True Data String
        /// </summary>
        const string _ntdString = "WeBrEgIsTrY";

        /// <summary>
        /// No True Data DSD Version
        /// </summary>
        const string _ntdDSDVersion = "1010101010";

        /// <summary>
        /// No True Data Time Dimension Name
        /// </summary>
        private string _ntdTDName { get { return _ntdString + "_TD"; } }

        /// <summary>
        /// No True Data Normal Dimension Name
        /// </summary>
        private string _ntdNRName { get { return _ntdString + "_NR"; } }

        /// <summary>
        /// No True Data Primary Measure 
        /// </summary>
        private string _ntdPMName { get { return _ntdString + "_PM"; } }


        #endregion

        private IDataStructureMutableObject _dsdMutable;

        /// <summary>
        /// Converte l'oggetto dsd mutable in ISdmxObjects()
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
                            ISdmxObjects sdmxAppo = _wsmodel.GetDataStructure(_artIdentity, false, false);
                            Session[_snSdmxObjects] = sdmxAppo;
                        }
                        return (ISdmxObjects)Session[_snSdmxObjects];
                    case Action.UPDATE:
                        if (!IsPostBack)
                        {
                            ISdmxObjects sdmxAppo = _wsmodel.GetDataStructure(_artIdentity, false, false);
                            _dsdMutable = sdmxAppo.DataStructures.FirstOrDefault().MutableInstance;
                            SetDsdToSession();
                        }
                        ISdmxObjects sdmxObjectsU = new SdmxObjectsImpl();
                        sdmxObjectsU.AddDataStructure(_dsdMutable.ImmutableInstance);
                        return sdmxObjectsU;
                        break;
                    case Action.INSERT:
                        ISdmxObjects sdmxObjectsI = new SdmxObjectsImpl();
                        sdmxObjectsI.AddDataStructure(_dsdMutable.ImmutableInstance);
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
            _epe = (EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT];

            _dsdMutable = null;

            _sdmxUtils = new SDMXUtils();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _entityMapper = new EntityMapper(Utils.LocalizedLanguage);
            _artIdentity = Utils.GetIdentityFromRequest(Request);

            SetAction();

            CommonInitUserControl();

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

            ucAnnotationDimension.AnnotationCreated += ucAnnotationDimension_AnnotationCreated;
            ucAnnotationDimension.AnnotationDeleted += ucAnnotationDimension_AnnotationDeleted;
            ucAnnotationAttribute.AnnotationCreated += ucAnnotationAttribute_AnnotationCreated;
            ucAnnotationAttribute.AnnotationDeleted += ucAnnotationAttribute_AnnotationDeleted;
            ucAnnotationGroup.AnnotationCreated += ucAnnotationGroup_AnnotationCreated;
            ucAnnotationGroup.AnnotationDeleted += ucAnnotationGroup_AnnotationDeleted;

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.Dsd;
            DuplicateArtefact1.ucMaintanableArtefact = _dsdMutable;

            lblDSDDescription.DataBind();
            lblDSDName.DataBind();
            lblDSDID.DataBind();
            lblAgency.DataBind();
            lblVersion.DataBind();
            lblIsFinal.DataBind();
            lblDSDURI.DataBind();
            lblValidTo.DataBind();
            lblValidFrom.DataBind();
            lblDSDURN.DataBind();
            lblDSDURI.DataBind();
            lblPMID.DataBind();
            lblCodelist.DataBind();
            lblConcept.DataBind();
            lblTitle.DataBind();
            lblDimType.DataBind();
            lblDimID.DataBind();
            lblDimConceptReference.DataBind();
            lblDimConcept.DataBind();
            lblCodeConceptScheme.DataBind();
            lblCodeCodelist.DataBind();
            lblCodeCodeRappresentation.DataBind();
            btnAddDimension.DataBind();
            btnSaveDSD.DataBind();
            lblAddGroupTitle.DataBind();
            lblGroupID.DataBind();
            lblGroupDimension.DataBind();
            btnAddGroup.DataBind();
            lbl_annotation.DataBind();
            lblAnnotaionPM.DataBind();
        }

        void ucAnnotationGroup_AnnotationDeleted(object sender, IAnnotationMutableObject e)
        {
            SetGroupTab(_dsdMutable.ImmutableInstance);
        }

        void ucAnnotationGroup_AnnotationCreated(object sender, IAnnotationMutableObject e)
        {
            SetGroupTab(_dsdMutable.ImmutableInstance);
        }

        void ucAnnotationAttribute_AnnotationDeleted(object sender, IAnnotationMutableObject e)
        {
            SetAttributeTab(_dsdMutable.ImmutableInstance);
        }

        void ucAnnotationAttribute_AnnotationCreated(object sender, IAnnotationMutableObject e)
        {
            SetAttributeTab(_dsdMutable.ImmutableInstance);
        }

        void ucAnnotationDimension_AnnotationDeleted(object sender, IAnnotationMutableObject e)
        {
            SetDimensionTab(_dsdMutable.ImmutableInstance);
        }

        private void ucAnnotationDimension_AnnotationCreated(object sender, IAnnotationMutableObject e)
        {
            SetDimensionTab(_dsdMutable.ImmutableInstance);
        }

        protected void btnOpenAttributePopUp_Click(object sender, ImageClickEventArgs e)
        {
            txtAttributeID.Enabled = true;
            txtAttributeID.Text = "";
            txtAttributeConcept.Text = "";
            txtAttributeCodelist.Text = "";
            txtAttOrder.Text = "";

            cmbAssignmentStatus.SelectedIndex = 0;
            cmbAttachLevel.SelectedIndex = 0;

            if (cmbAttachedGroupID.Items.Count > 0)
                cmbAttachedGroupID.SelectedIndex = 0;

            lbAttachmentDimension.ClearSelection();

            pnlAttachmentDimension.Visible = false;
            pnlAttachedGroupID.Visible = false;

            OpenAddAttributePopUp();

        }
        protected void gvGroups_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGroups.PageIndex = e.NewPageIndex;
            SetGroupTab(_dsdMutable.ImmutableInstance);
            Utils.AppendScript("location.href = '#Groups';");
        }

        protected void gvDimension_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDimension.PageIndex = e.NewPageIndex;
            SetDimensionTab(_dsdMutable.ImmutableInstance);
            OpenDimensionTab();
        }

        protected void gvAttribute_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttribute.PageIndex = e.NewPageIndex;
            SetAttributeTab(_dsdMutable.ImmutableInstance);
            Utils.AppendScript("location.href = '#Attributes';");
        }

        protected void gvGroups_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string groupID = ((Label)gvGroups.Rows[e.RowIndex].FindControl("lblGroupKeyID")).Text;

            IGroupMutableObject grpDelete = null;

            foreach (IGroupMutableObject group in _dsdMutable.Groups)
            {
                if (group.Id == groupID)
                {
                    grpDelete = group;
                    _dsdMutable.Groups.Remove(group);
                    break;
                }
            }

            try
            {
                SetGroupTab(_dsdMutable.ImmutableInstance);
                SetDsdToSession();
                OpenGroupTab();
            }
            catch (Exception ex)
            {
                _dsdMutable.Groups.Add(grpDelete);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_goup);
            }


        }

        protected void gvAttribute_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            /* 1. Recupero l'ID dell'attributo
             * 2. Mi ciclo tutti gli attributi di _dsdMutable
             * 3. Se trovo l'attributo con lo stesso ID lo elimino
             * 4. Salvo l'oggetto in session
             * 5. Rieseguo il databind della gv degli attributi
             * 6. Riapro il tab degli attributi
             * CONTROLLI:
             *      Messaggio di conferma eliminazione
             */

            string attrID = ((Label)gvAttribute.Rows[e.RowIndex].FindControl("lblGVAttributeID")).Text;

            IAttributeMutableObject attrDelete = null;

            foreach (IAttributeMutableObject attr in _dsdMutable.Attributes)
            {
                if (attr.Id == attrID)
                {
                    attrDelete = attr;
                    _dsdMutable.Attributes.Remove(attr);
                    break;
                }
            }

            try
            {
                SetAttributeTab(_dsdMutable.ImmutableInstance);
                SetDsdToSession();
                OpenAttributeTab();
            }
            catch (Exception ex)
            {
                _dsdMutable.Attributes.Add(attrDelete);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_attribute);
            }

        }

        protected void gvAttribute_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string orderID = ((Label)gvr.FindControl("lblOrderID")).Text;
                    string attrID = ((Label)gvr.FindControl("lblGVAttributeID")).Text;
                    string concept = ((Label)gvr.FindControl("lblAttrConcept")).Text;
                    string codedRepresentation = ((Label)gvr.FindControl("lblAttrCodelist")).Text;
                    string assStatus = ((Label)gvr.FindControl("lblAssStatus")).Text;
                    string attachLevel = ((Label)gvr.FindControl("lblAttachLevel")).Text;

                    IAttributeMutableObject attrEdit = null;

                    if (_dsdMutable.Attributes != null && _dsdMutable.Attributes.Count > 0)
                    {
                        foreach (IAttributeMutableObject attr in _dsdMutable.Attributes)
                        {
                            if (attr.Id == attrID)
                            {
                                attrEdit = attr;
                                break;
                            }
                        }
                    }

                    if (attrEdit == null)
                        return;

                    txtAttOrder.Text = orderID;
                    txtAttributeID.Enabled = false;
                    txtAttributeID.Text = attrEdit.Id;
                    txtAttributeConcept.Text = concept;
                    txtAttributeCodelist.Text = codedRepresentation;
                    txtAttOrder.Enabled = !(_action == Action.VIEW);
                    cmbAssignmentStatus.SelectedValue = assStatus;
                    cmbAttachLevel.SelectedValue = attachLevel;

                    if (cmbAttachedGroupID.Items.Count > 0)
                        cmbAttachedGroupID.SelectedIndex = 0;

                    lbAttachmentDimension.ClearSelection();

                    switch (attrEdit.AttachmentLevel)
                    {
                        case AttributeAttachmentLevel.DimensionGroup:

                            foreach (string dim in attrEdit.DimensionReferences)
                            {
                                foreach (ListItem li in lbAttachmentDimension.Items)
                                {
                                    if (li.Value == dim)
                                        li.Selected = true;
                                }
                            }

                            pnlAttachmentDimension.Visible = true;
                            pnlAttachedGroupID.Visible = false;

                            break;
                        case AttributeAttachmentLevel.Group:
                            cmbAttachedGroupID.SelectedValue = attrEdit.AttachmentGroup;

                            pnlAttachmentDimension.Visible = false;
                            pnlAttachedGroupID.Visible = true;

                            break;
                        default:
                            pnlAttachmentDimension.Visible = false;
                            pnlAttachedGroupID.Visible = false;
                            break;
                    }
                    hdnEditAttribute.Value = "true";
                    OpenAddAttributePopUp();

                    break;

                case "Annotation":
                    GridViewRow gvr2 = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string attributeID = ((Label)gvAttribute.Rows[gvr2.RowIndex].FindControl("lblGVAttributeID")).Text;

                    var att = _dsdMutable.Attributes.Where(d => d.Id == attributeID).FirstOrDefault();

                    ucAnnotationAttribute.AnnotationObjectList = att.Annotations;

                    if (_action == Action.VIEW)
                        ucAnnotationAttribute.EditMode = false;
                    else
                        ucAnnotationAttribute.EditMode = true;

                    lblAnnotationAttribute.Text = Resources.Messages.lbl_annotation_attribute + " '" + attributeID + "'";

                    OpenAnnotationAttribute();

                    break;
            }
        }

        protected void gvDimension_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            /* 1. Recupero l'ID della dimensione
             * 2. Mi ciclo tutte le dimensioni di _dsdMutable
             * 3. Se trovo la dimensione con lo stesso id la elimino
             * 4. Salvo l'oggetto in session
             * 5. Rieseguo il databind della gv delle dimensioni
             * 6. Riapro il tab delle dimensioni
             * CONTROLLI:
             *      Messaggio di conferma eliminazione
             *      Controllo su ID duplicato(IN ADD)
             */

            if (_dsdMutable.Dimensions.Count == 1)
            {
                Utils.ShowDialog(Resources.Messages.err_dimension_count);
                OpenDimensionTab();
                return;
            }


            string dimID = ((Label)gvDimension.Rows[e.RowIndex].FindControl("lblDimID")).Text;
            IDimensionMutableObject dimDelete = null;

            foreach (IDimensionMutableObject dim in _dsdMutable.Dimensions)
            {
                if (dim.Id == dimID)
                {
                    dimDelete = dim;
                    _dsdMutable.Dimensions.Remove(dim);
                    break;
                }
            }

            try
            {
                SetDimensionTab(_dsdMutable.ImmutableInstance);
                SetDsdToSession();
                PopulateLBDimensionList(_dsdMutable, lbAttachmentDimension);
                PopulateLBDimensionList(_dsdMutable, lbGroupDimension);

                OpenDimensionTab();
            }
            catch (Exception ex)
            {
                _dsdMutable.Dimensions.Add(dimDelete);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_dimension);
            }
        }

        protected void gvDimension_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string orderID = ((Label)gvr.FindControl("lblOrderID")).Text;
                    string dimenID = ((Label)gvr.FindControl("lblDimID")).Text;
                    string concept = ((Label)gvr.FindControl("lblDimConcept")).Text;
                    string codedRep = ((Label)gvr.FindControl("lblCodelist2")).Text;
                    string role = ((Label)gvr.FindControl("lblDimRole")).Text;


                    IDimensionMutableObject dimEdit = null;

                    if (_dsdMutable.Dimensions != null && _dsdMutable.Dimensions.Count > 0)
                    {
                        foreach (IDimensionMutableObject dimen in _dsdMutable.Dimensions)
                        {
                            if (dimen.Id == dimenID)
                            {
                                dimEdit = dimen;
                                break;
                            }
                        }
                    }

                    if (dimEdit == null)
                        return;

                    switch (role)
                    {
                        case "Dimension":
                            cmbDimType.SelectedValue = "NORMAL";
                            break;
                        case "FrequencyDimension":
                            cmbDimType.SelectedValue = "FREQUENCY";
                            break;
                        case "TimeDimension":
                            cmbDimType.SelectedValue = "TIME";
                            break;
                        case "MeasureDimension":
                            cmbDimType.SelectedValue = "MEASURE";
                            break;
                    }
                    SetDimensionForm();

                    txtDimID.Enabled = false;
                    txtDimID.Text = dimEdit.Id;
                    txtDimConcept.Text = concept;
                    txtDimCodelist.Text = codedRep;
                    txtDimConceptScheme.Text = codedRep;
                    txtDimOrder.Text = orderID;
                    txtDimOrder.Enabled = !(_action == Action.VIEW);

                    cmbDimType.Enabled = false;

                    hdnEditDimension.Value = "true";
                    OpenAddDimensionPopUp();

                    break;

                case "Annotation":
                    GridViewRow gvr2 = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string dimID = ((Label)gvDimension.Rows[gvr2.RowIndex].FindControl("lblDimID")).Text;

                    var dim = _dsdMutable.Dimensions.Where(d => d.Id == dimID).FirstOrDefault();

                    ucAnnotationDimension.AnnotationObjectList = dim.Annotations;

                    if (_action == Action.VIEW)
                        ucAnnotationDimension.EditMode = false;
                    else
                        ucAnnotationDimension.EditMode = true;

                    lblAnnotationDimension.Text = Resources.Messages.lbl_annotation_dimension + " '" + dimID + "'";

                    OpenAnnotationDimension();

                    break;
            }
        }

        protected void gvGroup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {

                case "Details":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string grpID = ((Label)gvr.FindControl("lblGroupKeyID")).Text;
                    string dimList = ((Label)gvr.FindControl("lblDimensionList")).Text;

                    string[] stringSeparators = new string[] { "<br/>" };
                    string[] lDim;

                    lDim = dimList.Split(stringSeparators, StringSplitOptions.None);

                    IGroupMutableObject groupEdit = null;

                    if (_dsdMutable.Groups != null && _dsdMutable.Groups.Count > 0)
                    {
                        foreach (IGroupMutableObject grp in _dsdMutable.Groups)
                        {
                            if (grp.Id == grpID)
                            {
                                groupEdit = grp;
                                break;
                            }
                        }
                    }

                    if (groupEdit == null)
                        return;

                    txtGroupID.Enabled = false;
                    txtGroupID.Text = groupEdit.Id;

                    lbGroupDimension.ClearSelection();

                    foreach (ListItem item in lbGroupDimension.Items)
                    {
                        if (lDim.Contains(item.Text))
                            item.Selected = true;
                    }

                    hdnEditGroup.Value = "true";
                    OpenAddGroupPopUp();

                    break;


                case "Annotation":
                    GridViewRow gvr2 = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string groupID = ((Label)gvGroups.Rows[gvr2.RowIndex].FindControl("lblGroupKeyID")).Text;

                    var group = _dsdMutable.Groups.Where(d => d.Id == groupID).FirstOrDefault();

                    ucAnnotationGroup.AnnotationObjectList = group.Annotations;

                    if (_action == Action.VIEW)
                        ucAnnotationGroup.EditMode = false;
                    else
                        ucAnnotationGroup.EditMode = true;

                    lblAnnotationGroup.Text = Resources.Messages.lbl_annotation_group + " '" + groupID + "'";

                    OpenAnnotationGroup();

                    break;
            }
        }

        protected void gvDimension_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Nasconde le dimensioni NTD
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblConcept = (Label)e.Row.Cells[1].FindControl("lblDimConcept");

                if (lblConcept.Text.Contains(_ntdString))
                    e.Row.Visible = false;
            }

            IDataStructureMutableObject dsd = _dsdMutable;
            if (e.Row.RowIndex != -1)
            {
                string dimensionId = ((Label)e.Row.FindControl("lblDimID")).Text;
                IDimensionMutableObject dimension = dsd.DimensionList.Dimensions.Where(d => d.Id != null && d.Id.ToString().Equals(dimensionId)).FirstOrDefault();
                if (dimension == null)
                    return;

                Label lblNumber = (Label)e.Row.FindControl("lblNumberOfAnnotationDimensions");
                ImageButton btnImage = (ImageButton)e.Row.FindControl("ibDimAnnotation");
                int numberOfAnnotation = dimension.Annotations.Where(ann => ann.Id != null && ann.Type != "@ORDER@").Count();
                lblNumber.Text = numberOfAnnotation.ToString();
                if (numberOfAnnotation == 0 && _action == Action.VIEW)
                {
                    btnImage.Enabled = false;
                }
            }
        }

        protected void gvGroups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDimensionList = (Label)e.Row.FindControl("lblDimensionList");
                lblDimensionList.Text = lblDimensionList.Text.Replace(",", "<br/>");
            }

            IDataStructureMutableObject dsd = _dsdMutable;
            if (e.Row.RowIndex != -1)
            {
                string groupId = ((Label)e.Row.FindControl("lblGroupKeyID")).Text;
                IGroupMutableObject group = dsd.Groups.Where(g => g.Id.ToString().Equals(groupId)).FirstOrDefault();
                if (group == null)
                    return;
                Label lblNumber = (Label)e.Row.FindControl("lblNumberOfAnnotationGroups");
                ImageButton btnImage = (ImageButton)e.Row.FindControl("ibDimGroup");
                int numberOfAnnotation = group.Annotations.Where(ann => ann.Id != null).Count();
                lblNumber.Text = numberOfAnnotation.ToString();
                if (numberOfAnnotation == 0 && _action == Action.VIEW)
                {
                    btnImage.Enabled = false;
                }
            }

        }

        protected void cmbAttachLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbAttachLevel.SelectedValue)
            {
                case "Observation":
                case "DataSet":
                    pnlAttachmentDimension.Visible = false;
                    pnlAttachedGroupID.Visible = false;
                    break;
                case "DimensionGroup":
                    pnlAttachmentDimension.Visible = true;
                    pnlAttachedGroupID.Visible = false;
                    break;
                case "Group":
                    pnlAttachmentDimension.Visible = false;
                    pnlAttachedGroupID.Visible = true;
                    break;
                default:
                    pnlAttachmentDimension.Visible = false;
                    pnlAttachedGroupID.Visible = false;
                    break;
            }
            OpenAddAttributePopUp();
        }

        protected void cmbDimType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDimensionForm();

            OpenAddDimensionPopUp();
        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            if (!ValidateGroupData())
            {
                OpenAddGroupPopUp();
                return;
            }

            IGroupMutableObject group = new GroupMutableCore();
            group.Id = txtGroupID.Text;

            foreach (ListItem li in lbGroupDimension.Items)
            {
                if (li.Selected)
                    group.DimensionRef.Add(li.Value);
            }

            try
            {
                if (hdnEditGroup.Value == "true")
                {
                    foreach (IGroupMutableObject grpDel in _dsdMutable.Groups)
                    {
                        if (grpDel.Id == txtGroupID.Text)
                        {
                            _dsdMutable.Groups.Remove(grpDel);
                            break;
                        }
                    }
                }

                _dsdMutable.AddGroup(group);
                SetDsdToSession();
                SetGroupTab(_dsdMutable.ImmutableInstance);

                PopulateAttachedGroup(_dsdMutable);

                txtGroupID.Text = "";
                lbGroupDimension.ClearSelection();

                Utils.ForceBlackClosing();
            }
            catch (Exception ex)
            {
                _dsdMutable.Groups.Remove(group);
                OpenAddGroupPopUp();
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_add_group);
            }
        }

        protected void btnAddAttribute_Click(object sender, EventArgs e)
        {
            if (!ValidateAttributeData())
            {
                OpenAddAttributePopUp();
                return;
            }

            /*
             * Creo l'attribute
             * ID
             * Concept
             * Eventuale Codelist
             * Assignment status
             * Attachmentlevel
             *      DimensionGroup
             *          Attachment Dimension
             *      Group
             *          Attached Group ID
             * */

            IAttributeMutableObject attr = new AttributeMutableCore();
            attr.Id = txtAttributeID.Text;

            #region "***** Concept Reference ******"

            ArtefactIdentity csIdentity = new ArtefactIdentity();
            string[] conceptData = txtAttributeConcept.Text.Split(',');

            if (conceptData.Length > 0)
            {
                csIdentity.ID = conceptData[0];
                csIdentity.Agency = conceptData[1];
                csIdentity.Version = conceptData[2].Substring(0, conceptData[2].IndexOf(' '));

                string conceptID = conceptData[2].Substring(conceptData[2].LastIndexOf(' ') + 1);

                IStructureReference conceptRef = new StructureReferenceImpl(csIdentity.Agency,
                                                                            csIdentity.ID,
                                                                            csIdentity.Version,
                                                                            SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Concept),
                                                                            new string[] { conceptID });

                attr.ConceptRef = conceptRef;
            }

            #endregion

            #region "***** Codelist Reference ******"

            if (txtAttributeCodelist.Text != string.Empty)
            {
                ArtefactIdentity clIdentity = new ArtefactIdentity();
                string[] clData = txtAttributeCodelist.Text.Split(',');

                clIdentity.ID = clData[0];
                clIdentity.Agency = clData[1];
                clIdentity.Version = clData[2];

                IStructureReference codelistRef = new StructureReferenceImpl(clIdentity.Agency,
                                                                                clIdentity.ID,
                                                                                clIdentity.Version,
                                                                                SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList),
                                                                                null);

                IRepresentationMutableObject representationRef = new RepresentationMutableCore();
                representationRef.Representation = codelistRef;

                attr.Representation = representationRef;
            }

            #endregion

            attr.AssignmentStatus = cmbAssignmentStatus.SelectedValue;
            attr.AttachmentLevel = (AttributeAttachmentLevel)Enum.Parse(typeof(AttributeAttachmentLevel), cmbAttachLevel.SelectedValue);

            switch (attr.AttachmentLevel)
            {
                case AttributeAttachmentLevel.DataSet:
                    break;
                case AttributeAttachmentLevel.DimensionGroup:
                    // Aggiungo le dimensioni selezionate nella list box
                    IList<string> lDim = new List<string>();

                    foreach (ListItem li in lbAttachmentDimension.Items)
                    {
                        if (li.Selected)
                            attr.DimensionReferences.Add(li.Value);
                    }

                    break;
                case AttributeAttachmentLevel.Group:
                    // Aggiungo il gruppo selezionato nella combo
                    attr.AttachmentGroup = cmbAttachedGroupID.SelectedValue;
                    break;
                case AttributeAttachmentLevel.Null:
                    break;
                case AttributeAttachmentLevel.Observation:
                    break;
                default:
                    break;
            }

            try
            {
                if (hdnEditAttribute.Value == "true")
                {
                    foreach (IAttributeMutableObject attrDel in _dsdMutable.Attributes)
                    {
                        if (attrDel.Id == txtAttributeID.Text)
                        {
                            _dsdMutable.Attributes.Remove(attrDel);
                            break;
                        }
                    }
                }

                int orderID = 0;

                if (Int32.TryParse(txtAttOrder.Text, out orderID))
                {
                    if (orderID <= 0)
                        orderID = 1;
                    _dsdMutable = ReOrderAttribute(_dsdMutable, attr, orderID);
                }
                else
                    _dsdMutable.AddAttribute(attr);

                SetDsdToSession();
                SetAttributeTab(_dsdMutable.ImmutableInstance);

                txtAttributeID.Text = "";
                txtAttributeConcept.Text = "";
                txtAttributeCodelist.Text = "";
                cmbAssignmentStatus.SelectedIndex = 0;
                cmbAttachLevel.SelectedIndex = 0;

                lbAttachmentDimension.ClearSelection();
                if (cmbAttachedGroupID.Items.Count > 0)
                    cmbAttachedGroupID.SelectedIndex = 0;
                pnlAttachedGroupID.Visible = false;
                pnlAttachmentDimension.Visible = false;

                Utils.ForceBlackClosing();
            }
            catch (Exception ex)
            {
                _dsdMutable.Attributes.Remove(attr);
                OpenAddAttributePopUp();
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_add_attribute);
            }

        }

        protected void btnAddDimension_Click(object sender, EventArgs e)
        {
            if (!ValidateDimensionData())
            {
                OpenAddDimensionPopUp();
                return;
            }

            IDimensionMutableObject dim = new DimensionMutableCore();
            dim.Id = txtDimID.Text;

            switch (cmbDimType.SelectedValue)
            {
                case "FREQUENCY":
                    dim.FrequencyDimension = true;
                    break;
                case "MEASURE":
                    dim.MeasureDimension = true;
                    break;
                case "TIME":
                    dim.TimeDimension = true;
                    break;
            }

            #region "***** Concept Reference ******"

            ArtefactIdentity csIdentity = new ArtefactIdentity();
            string[] conceptData = txtDimConcept.Text.Split(',');

            if (conceptData.Length > 0)
            {
                csIdentity.ID = conceptData[0];
                csIdentity.Agency = conceptData[1];
                csIdentity.Version = conceptData[2].Substring(0, conceptData[2].IndexOf(' '));

                string conceptID = conceptData[2].Substring(conceptData[2].LastIndexOf(' ') + 1);

                IStructureReference conceptRef = new StructureReferenceImpl(csIdentity.Agency,
                                                                            csIdentity.ID,
                                                                            csIdentity.Version,
                                                                            SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Concept),
                                                                            new string[] { conceptID });

                dim.ConceptRef = conceptRef;
            }
            #endregion

            switch (cmbDimType.SelectedValue)
            {
                case "NORMAL":
                case "FREQUENCY":

                    // Remove NTD Dimension
                    if (_dsdMutable.Dimensions != null)
                        foreach (IDimensionMutableObject dimF in _dsdMutable.Dimensions)
                        {
                            if (dimF.ConceptRef.MaintainableId == _ntdNRName)
                            {
                                _dsdMutable.Dimensions.Remove(dimF);
                                break;
                            }
                        }

                    #region "***** Codelist Reference ******"

                    if (txtDimCodelist.Text != string.Empty)
                    {
                        ArtefactIdentity clIdentity = new ArtefactIdentity();
                        string[] clData = txtDimCodelist.Text.Split(',');

                        clIdentity.ID = clData[0];
                        clIdentity.Agency = clData[1];
                        clIdentity.Version = clData[2];

                        IStructureReference codelistRef = new StructureReferenceImpl(clIdentity.Agency,
                                                                                        clIdentity.ID,
                                                                                        clIdentity.Version,
                                                                                        SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList),
                                                                                        null);


                        IRepresentationMutableObject representationRef = new RepresentationMutableCore();
                        representationRef.Representation = codelistRef;

                        dim.Representation = representationRef;

                    }

                    #endregion

                    break;
                case "MEASURE":

                    #region "***** ConceptScheme Reference ******"

                    if (txtDimConceptScheme.Text != string.Empty)
                    {
                        ArtefactIdentity cSchemeIdentity = new ArtefactIdentity();
                        string[] clData = txtDimConceptScheme.Text.Split(',');

                        cSchemeIdentity.ID = clData[0];
                        cSchemeIdentity.Agency = clData[1];
                        cSchemeIdentity.Version = clData[2];

                        IStructureReference cSchemeRef = new StructureReferenceImpl(cSchemeIdentity.Agency,
                                                                                    cSchemeIdentity.ID,
                                                                                    cSchemeIdentity.Version,
                                                                                    SdmxStructureType.GetFromEnum(SdmxStructureEnumType.ConceptScheme),
                                                                                    null);


                        IRepresentationMutableObject representationRef = new RepresentationMutableCore();
                        representationRef.Representation = cSchemeRef;

                        dim.Representation = representationRef;

                    }

                    #endregion

                    break;
                case "TIME":

                    // Remove NTD Time Dimensione
                    //foreach (IDimensionMutableObject dimF in _dsdMutable.Dimensions)
                    //{
                    //    if (dimF.ConceptRef.MaintainableId == _ntdTDName)
                    //    {
                    //        _dsdMutable.Dimensions.Remove(dimF);
                    //        break;
                    //    }
                    //}

                    break;
            }

            try
            {
                if (hdnEditDimension.Value == "true")
                {
                    foreach (IDimensionMutableObject dimDel in _dsdMutable.Dimensions)
                    {
                        if (dimDel.Id == txtDimID.Text)
                        {
                            foreach (AnnotationMutableCore ads in dimDel.Annotations)
                            {
                                if (ads.Type != "@ORDER@")
                                    dim.AddAnnotation(ads);
                            }
                            _dsdMutable.Dimensions.Remove(dimDel);
                            break;
                        }
                    }
                }

                int orderID = 0;

                if (Int32.TryParse(txtDimOrder.Text, out orderID))
                {
                    if (orderID <= 0)
                        orderID = 1;
                    _dsdMutable = ReOrderDimension(_dsdMutable, dim, orderID);
                }
                else
                    _dsdMutable.AddDimension(dim);

                SetDsdToSession();
                SetDimensionTab(_dsdMutable.ImmutableInstance);
                PopulateLBDimensionList(_dsdMutable, lbAttachmentDimension);
                PopulateLBDimensionList(_dsdMutable, lbGroupDimension);

                txtDimID.Text = "";
                txtDimConcept.Text = "";
                txtDimCodelist.Text = "";
                txtDimConceptScheme.Text = "";
                cmbDimType.SelectedIndex = 0;
                txtDimID.Enabled = true;

                Utils.ForceBlackClosing();
            }
            catch (Exception ex)
            {
                _dsdMutable.Dimensions.Remove(dim);
                OpenAddDimensionPopUp();
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_add_dimension);
            }
        }



        //private IDataStructureMutableObject ReorderDimension(IDataStructureMutableObject _dsdMutable)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Salva la DSD nel registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveDSD_Click(object sender, EventArgs e)
        {
            /*
             * 1. Aggiungo all'oggetto _dsdMutable:
             *      Le info della General
             *      La collection di Names
             *      La Collecction delle Descriptions
             *      La Primary Measure
             *      Le Dimension sono presenti nell'oggetto _dsdMutable
             *      I Gruppi sono presenti nell'oggetto _dsdMutable
             *      Gli Attributes sono presenti nell'oggetto _dsdMutable
             * 2. Richiamo il metodo che prende in input un ISdmxObjects che richiama il SaveStructure
             * 3. Visualizzo un messaggio di conferma,resetto le Session e faccio un redirect alla lista delle DSD
             */

            if (!ValidateDSDData())
                return;

            try
            {
                _dsdMutable.Id = txtDSDID.Text;
                _dsdMutable.AgencyId = cmbAgencies.SelectedValue;
                _dsdMutable.Version = txtVersion.Text;

                _dsdMutable.Names.Clear();
                foreach (ITextTypeWrapperMutableObject name in AddTextName.TextObjectList)
                {
                    _dsdMutable.Names.Add(name);
                }

                _dsdMutable.Descriptions.Clear();
                if (AddTextDescription.TextObjectList != null)
                {
                    foreach (ITextTypeWrapperMutableObject descr in AddTextDescription.TextObjectList)
                    {
                        _dsdMutable.Descriptions.Add(descr);
                    }
                }

                _dsdMutable.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
                if (txtDSDURI.Text != String.Empty)
                    _dsdMutable.Uri = new Uri(txtDSDURI.Text);

                if (txtValidFrom.Text != String.Empty)
                    _dsdMutable.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                else
                    _dsdMutable.StartDate = null;
                if (txtValidTo.Text != String.Empty)
                    _dsdMutable.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                else
                    _dsdMutable.EndDate = null;
                // PM 
                string[] pmValues = txtConcept.Text.Substring(0, txtConcept.Text.IndexOf(" ")).Split(',');
                string pmConcept = txtConcept.Text.Substring(txtConcept.Text.LastIndexOf(" ") + 1);
                _dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl(pmValues[1], pmValues[0], pmValues[2], SdmxStructureEnumType.Concept, pmConcept));

                if (txtCodelist.Text != "")
                {
                    string[] pmCLValues = txtCodelist.Text.Split(',');

                    IRepresentationMutableObject rep = new RepresentationMutableCore();
                    rep.Representation = new StructureReferenceImpl(pmCLValues[1], pmCLValues[0], pmCLValues[2], SdmxStructureEnumType.CodeList, null);

                    _dsdMutable.PrimaryMeasure.Representation = rep;
                }

                _dsdMutable.Annotations.Clear();
                if (AnnotationGeneral.AnnotationObjectList != null)
                    foreach (var annotation in AnnotationGeneral.AnnotationObjectList)
                    {
                        _dsdMutable.AddAnnotation(annotation);
                    }

                _dsdMutable.PrimaryMeasure.Annotations.Clear();
                if (AnnotationPrimaryMeasure.AnnotationObjectList != null)
                    foreach (var annotation in AnnotationPrimaryMeasure.AnnotationObjectList)
                    {
                        _dsdMutable.PrimaryMeasure.AddAnnotation(annotation);
                    }

                RemoveAnnotationOrder();

                SetDsdToSession();

                WSModel wsModel = new WSModel();
                XmlDocument xRet = wsModel.SubmitStructure(_sdmxObjects);

                string err = Utils.GetXMLResponseError(xRet);

                if (err != "")
                {
                    Utils.ShowDialog(err);
                    return;
                }

                //Session[_snInsDSD] = null;
                //Session[_snSdmxObjects] = null;

                //Utils.ShowDialog("Operation succesfully");
                Utils.ResetBeforeUnload();
                //Utils.ShowDialog(successMessage, 300);
                //Utils.AppendScript("location.href='./KeyFamily.aspx';");

                string jsYes, jsNo, script;

                script = "CloseConfirm();";

                jsNo = String.Format("location.href='KeyFamilyItemDetails.aspx?ACTION=UPDATE&ID={0}&AGENCY={1}&VERSION={2}&ISFINAL={3}'",
                                        _dsdMutable.Id, _dsdMutable.AgencyId, _dsdMutable.Version, _dsdMutable.FinalStructure.IsTrue.ToString());
                //jsYes = "location.href='KeyFamily.aspx?m=y'";


                if (_action == Action.INSERT)
                {
                    script = jsNo;
                }

                Utils.ShowDialogBeforeScript(Resources.Messages.succ_operation, script);

            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }

        private void RemoveAnnotationOrder()
        {
            List<IAnnotationMutableObject> annToRemove;

            foreach (var dim in _dsdMutable.Dimensions)
            {
                annToRemove = dim.Annotations.Where(an => an.Type == "@ORDER@").ToList();
                foreach (var ann in annToRemove)
                    dim.Annotations.Remove(ann);
            }

            if (_dsdMutable.Attributes != null)
                foreach (var attr in _dsdMutable.Attributes)
                {
                    annToRemove = attr.Annotations.Where(an => an.Type == "@ORDER@").ToList();
                    foreach (var ann in annToRemove)
                        attr.Annotations.Remove(ann);
                }
        }

        protected void gvAttribute_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            IDataStructureMutableObject dsd = _dsdMutable;
            if (e.Row.RowIndex != -1)
            {
                string attributeId = ((Label)e.Row.FindControl("lblGVAttributeID")).Text;
                IAttributeMutableObject attribute = dsd.AttributeList.Attributes.Where(a => a.Id.ToString().Equals(attributeId)).FirstOrDefault();
                if (attribute == null)
                    return;
                Label lblNumber = (Label)e.Row.FindControl("lblNumberOfAnnotationAttributes");
                ImageButton btnImage = (ImageButton)e.Row.FindControl("ibDimAttribute");
                int numberOfAnnotation = attribute.Annotations.Where(ann => ann.Id != null).Count();
                lblNumber.Text = numberOfAnnotation.ToString();
                if (numberOfAnnotation == 0 && _action == Action.VIEW)
                {
                    btnImage.Enabled = false;
                }
            }
        }

        #endregion

        #region Methods

        private void PopulateLBDimensionList(IDataStructureMutableObject dsd, ListBox lb)
        {
            if (dsd.Dimensions != null && dsd.Dimensions.Count > 0)
            {
                lb.Items.Clear();
                foreach (IDimensionMutableObject dim in dsd.Dimensions)
                {
                    if (!dim.ConceptRef.FullId.Contains(_ntdString))
                        lb.Items.Add(dim.Id);
                }
            }
        }

        private void PopulateAttachedGroup(IDataStructureMutableObject dsd)
        {
            if (dsd.Groups != null && dsd.Groups.Count > 0)
            {
                cmbAttachedGroupID.Items.Clear();
                cmbAttachedGroupID.Items.Add("");
                foreach (IGroupMutableObject group in dsd.Groups)
                {
                    cmbAttachedGroupID.Items.Add(group.Id);
                }
            }
        }

        private bool ValidateGroupData()
        {

            string messagesGroup = string.Empty;
            int clSelected = 0;

            foreach (ListItem li in lbGroupDimension.Items)
            {
                if (li.Selected)
                    ++clSelected;
            }

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtGroupID.Text))
            {
                messagesGroup += Resources.Messages.err_id_group_format + "<br /><br />";
            }

            // Nessuna dimensione selezionata
            if (clSelected == 0)
            {
                messagesGroup += Resources.Messages.err_dimension_count + "<br /><br />";
            }

            // Tutte le dimensioni selezionate
            if (clSelected == lbGroupDimension.Items.Count)
            {
                messagesGroup += Resources.Messages.err_dimension_count_out + "<br /><br />";
            }

            //Gruppo formato con le stesse dimensioni di un altro non permesso
            //if (clSelected == lbGroupDimension.Items.Count)
            //{
            //    messagesGroup += "It is not possible to select all Dimensions<br /><br />";
            //}

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_add_group);
                return false;
            }

            return true;
        }

        private bool ValidateDimensionData()
        {

            //TODO: Controlli
            //  Controllare che non venganop inserite + volte le tipologie Frequenxy,Measure,Time
            //  Dimensione con lo stesso ID
            //  ID Obbligatorio
            //  ID privo di caratteri speciali(tipo lo spazio)

            string messagesGroup = string.Empty;

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtDimID.Text))
            {
                messagesGroup += Resources.Messages.err_id_group_format + "<br /><br />";
            }

            // Concept Reference obbligatorio
            if (txtDimConcept.Text == String.Empty)
            {
                messagesGroup += Resources.Messages.err_concept_ref_empty + "<br /><br />";
            }

            // Concept Reference obbligatorio
            if (cmbDimType.SelectedValue == "MEASURE" && txtDimConceptScheme.Text == String.Empty)
            {
                messagesGroup += Resources.Messages.err_conceptscheme_ref_empty + "<br /><br />";
            }

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_add_dimension);
                return false;
            }

            return true;
        }

        private bool ValidateAttributeData()
        {

            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtAttributeID.Text))
            {
                messagesGroup += Resources.Messages.err_id_format + "<br /><br />";
            }

            // Concept Reference obbligatorio
            if (txtAttributeConcept.Text == String.Empty)
            {
                messagesGroup += Resources.Messages.err_concept_ref_empty + "<br /><br />";
            }

            if (cmbAssignmentStatus.SelectedIndex == 0)
            {
                messagesGroup += Resources.Messages.err_assign_status_empty + "<br /><br />";
            }

            if (cmbAttachLevel.SelectedIndex == 0)
            {
                messagesGroup += Resources.Messages.err_attachment_level_empty + "<br /><br />";
            }


            switch (cmbAttachLevel.SelectedValue)
            {
                case "DimensionGroup":
                    if (lbAttachmentDimension.SelectedValue == "")
                        messagesGroup += Resources.Messages.err_attribute_attachment_level_empty + "<br /><br />";
                    break;
                case "Group":
                    if (cmbAttachedGroupID.SelectedIndex == 0)
                        messagesGroup += Resources.Messages.err_group_attachment_level_empty + "<br /><br />";
                    break;
            }

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_add_attribute);
                return false;
            }

            return true;
        }

        private bool ValidateDSDData()
        {
            /*
             * 1. Effettuo i controlli prima del salvataggio e li visualizzo a video tramite un OpenP()
             *      General:
             *          ID obbligatorio
             *          Version obbligatorio
             *          Name Obbligatorio
             *          Controllo formale URI
             *          Controllo formale Date
             *      Primary Measure
             *          Concept Obbligatorio
             *      Dimension
             *          TimeDimension obbligatoria
             *          Una dimensione obbligatoria
            */

            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo versione
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo URI
            if (txtDSDURI.Text != String.Empty && !ValidationUtils.CheckUriFormatNoRegExp(txtDSDURI.Text))
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

            if (txtConcept.Text == String.Empty)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_primary_measure_empty + "<br /><br />";
                errorCounter++;
            }

            // Almeno una dimension
            //if (_dsdMutable.Dimensions.Where(d => !d.ConceptRef.MaintainableId.Contains(_ntdString)).Count() <= 0)
            //{
            //    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_add_least_dimension + "<br /><br />";
            //    errorCounter++;
            //}


            //bool haveTimeDimension = false;
            bool haveNormalDimension = false;

            foreach (IDimensionMutableObject dim in _dsdMutable.Dimensions)
            {
                if (!dim.ConceptRef.MaintainableId.Contains(_ntdString))
                {
                    //if (dim.TimeDimension)
                    //    haveTimeDimension = true;

                    if (!dim.TimeDimension && !dim.MeasureDimension && !dim.FrequencyDimension)
                        haveNormalDimension = true;
                }
            }

            if (!haveNormalDimension)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_add_normal_dimension + "<br /><br />";
                errorCounter++;
            }

            //if (!haveTimeDimension)
            //{
            //    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_add_time_dimension + "<br /><br />";
            //    errorCounter++;
            //}

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
            Session[_snInsDSD] = null;
            //andrea 07/06/2017
            //if (!_epe.PartialArtefact)
                Utils.PopulateCmbAgencies(cmbAgencies, true);

            cmbDimType.Items.Add("NORMAL");
            cmbDimType.Items.Add("FREQUENCY");
            cmbDimType.Items.Add("MEASURE");
            cmbDimType.Items.Add("TIME");

            cmbAssignmentStatus.Items.Add("");
            cmbAssignmentStatus.Items.Add("Conditional");
            cmbAssignmentStatus.Items.Add("Mandatory");

            cmbAttachLevel.Items.Add("");
            cmbAttachLevel.Items.Add("DataSet");
            cmbAttachLevel.Items.Add("DimensionGroup");
            cmbAttachLevel.Items.Add("Observation");
            cmbAttachLevel.Items.Add("Group");
        }

        private void OpenDimensionTab()
        {
            Utils.AppendScript("location.href='#Dimensions';");
        }

        private void OpenAttributeTab()
        {
            Utils.AppendScript("location.href='#Attributes';");
        }

        private void OpenGroupTab()
        {
            Utils.AppendScript("location.href='#Groups';");
        }

        private void OpenAddDimensionPopUp()
        {
            Utils.AppendScript("openP('df-Dimension',600);");
        }

        private void OpenAddAttributePopUp()
        {
            Utils.AppendScript("openP('df-Attribute',600);");
        }

        private void OpenAddGroupPopUp()
        {
            Utils.AppendScript("openP('df-Group',600);");
        }

        private void OpenAnnotationDimension()
        {
            Utils.AppendScript("openP('df-anndimension',550);");
        }

        private void OpenAnnotationGroup()
        {
            Utils.AppendScript("openP('df-anngroup',550);");
        }

        private void OpenAnnotationAttribute()
        {
            Utils.AppendScript("openP('df-annattribute',550);");
        }

        private void EnableDownload()
        {
            FileDownload31.Visible = true;
            FileDownload31.ucID = _artIdentity.ID;
            FileDownload31.ucAgency = _artIdentity.Agency;
            FileDownload31.ucVersion = _artIdentity.Version;
            FileDownload31.ucArtefactType = "KeyFamily";
        }

        private void InitUserControl()
        {

            #region //////  GENERAL TAB ///////

            // Name User Control options
            AddTextName.ArtefactType = SdmxStructureEnumType.Dsd;
            AddTextName.TType = TextType.NAME;

            // Description User Control options
            AddTextDescription.ArtefactType = SdmxStructureEnumType.Dsd;
            AddTextDescription.TType = TextType.DESCRIPTION;

            AnnotationGeneral.AddText_ucOpenTabName = null;
            AnnotationGeneral.AddText_ucOpenPopUpName = null;
            AnnotationGeneral.EditMode = true;

            #endregion

            #region ////// PRIMARY MEASURE TAB //////

            // GetCodeList User Control options
            GetCodeList1.TargetWebControl = txtCodelist;
            GetCodeList1.ucOpenTabName = "PrimaryMeasure";

            // GetConcept User Control options
            GetConcept1.TargetWebControl = txtConcept;
            GetConcept1.ucOpenTabName = "PrimaryMeasure";
            GetConcept1.IsConceptSelection = true;

            AnnotationPrimaryMeasure.AddText_ucOpenTabName = "PrimaryMeasure";
            AnnotationPrimaryMeasure.AddText_ucOpenPopUpName = null;
            AnnotationPrimaryMeasure.EditMode = true;


            #endregion

            #region ////// DIMENSION TAB //////

            // Get Dimension Concept User Control options
            GetDimConcept.TargetWebControl = txtDimConcept;
            GetDimConcept.ucOpenTabName = "Dimensions";
            GetDimConcept.ucOpenPopUpName = "df-Dimension";
            GetDimConcept.ucOpenPopUpWidth = 600;
            GetDimConcept.IsConceptSelection = true;
            GetDimConcept.TargetWebControlID = txtDimID;

            // GetCodeList User Control options
            GetDimCodeList.TargetWebControl = txtDimCodelist;
            GetDimCodeList.ucOpenTabName = "Dimensions";
            GetDimCodeList.ucOpenPopUpName = "df-Dimension";
            GetDimCodeList.ucOpenPopUpWidth = 600;

            // GetConceptScheme User Control options
            GetDimConceptScheme.TargetWebControl = txtDimConceptScheme;
            GetDimConceptScheme.ucOpenTabName = "Dimensions";
            GetDimConceptScheme.ucOpenPopUpName = "df-Dimension";
            GetDimConceptScheme.ucOpenPopUpWidth = 600;
            GetDimConceptScheme.IsConceptSelection = false;

            #endregion

            #region ////// ATTRIBUTE TAB //////

            // Get Attribute Concept User Control options
            GetConceptAttribute.TargetWebControl = txtAttributeConcept;
            GetConceptAttribute.ucOpenTabName = "Attributes";
            GetConceptAttribute.ucOpenPopUpName = "df-Attribute";
            GetConceptAttribute.ucOpenPopUpWidth = 600;
            GetConceptAttribute.IsConceptSelection = true;
            GetConceptAttribute.TargetWebControlID = txtAttributeID;

            // Get Attribute CodeList User Control options
            GetCodeListAttribute.TargetWebControl = txtAttributeCodelist;
            GetCodeListAttribute.ucOpenTabName = "Attributes";
            GetCodeListAttribute.ucOpenPopUpName = "df-Attribute";
            GetCodeListAttribute.ucOpenPopUpWidth = 600;

            #endregion



        }

        private void CommonInitUserControl()
        {
            ucAnnotationDimension.PopUpContainer = "df-anndimension";
            ucAnnotationDimension.PopUpContainerWidth = 550;

            ucAnnotationGroup.PopUpContainer = "df-anngroup";
            ucAnnotationGroup.PopUpContainerWidth = 550;

            ucAnnotationAttribute.PopUpContainer = "df-annattribute";
            ucAnnotationAttribute.PopUpContainerWidth = 550;

        }

        private void SetViewForm()
        {
            GetDsdFromSession();
            EnableDownload();

            Categorisations.ucArtefactType = AvailableStructures.DSD;
            Categorisations.ucArtIdentity = _artIdentity;

            if (!IsPostBack)
            {
                SetLabelDetail();
                pnlViewName.Visible = true;
                pnlViewDescription.Visible = true;
                GetConcept1.Visible = false;
                GetCodeList1.Visible = false;
                pnlAddDimension.Visible = false;
                pnlAddAttribute.Visible = false;

                gvDimension.Columns[7].Visible = false;
                gvAttribute.Columns[7].Visible = false;
                gvGroups.Columns[2].Visible = false;

                AnnotationGeneral.EditMode = false;
                AnnotationPrimaryMeasure.EditMode = false;

                btnAddDimension.Visible = false;
                btnAddGroup.Visible = false;

                // Attribute
                txtAttributeID.Enabled = false;
                txtAttributeConcept.Enabled = false;
                txtAttributeCodelist.Enabled = false;
                cmbAssignmentStatus.Enabled = false;
                cmbAttachLevel.Enabled = false;
                lbAttachmentDimension.Attributes.Add("disabled", "true");
                cmbAttachedGroupID.Enabled = false;
                btnAddAttribute.Visible = false;
                GetConceptAttribute.Visible = false;
                GetCodeListAttribute.Visible = false;
                pnlAddGroup.Visible = false;

                GetConcept1.Visible = false;
                GetConceptAttribute.Visible = false;
                GetConcept1.Visible = false;
                GetConceptAttribute.Visible = false;
                GetDimCodeList.Visible = false;
                GetDimConcept.Visible = false;
                GetDimConceptScheme.Visible = false;


                BindData();
            }
        }

        private void SetEditForm()
        {
            GetDsdFromSession();
            InitUserControl();
            EnableDownload();

            Categorisations.ucArtefactType = AvailableStructures.DSD;
            Categorisations.ucArtIdentity = _artIdentity;

            if (!IsPostBack)
            {
                AnnotationGeneral.EditMode = true;
                AnnotationPrimaryMeasure.EditMode = true;

                SetLabelDetail();
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
                AnnotationGeneral.EditMode = true;
                AnnotationPrimaryMeasure.EditMode = true;

                SetEditingControl();
                BindData();

                FileDownload31.Visible = false;
            }
        }

        private void InitInsertObjects()
        {
            GetDsdFromSession();

            if (_dsdMutable == null)
                CreateEmptyDSD();
        }

        private void CreateEmptyDSD()
        {

            IDataStructureMutableObject dsdMutable;

            dsdMutable = _sdmxUtils.buildDataStructure(_ntdString + "DSD_ID", _ntdString + "AGENCY", _ntdDSDVersion);
            dsdMutable.AddName("en", _ntdString + "DSD_NAME");

            //AnnotationGeneralControl.ClearAnnotationsSession();

            #region NormalDimension

            IDimensionMutableObject dim = new DimensionMutableCore();
            dim.Id = _ntdNRName;

            IStructureReference conceptRef = new StructureReferenceImpl(_ntdNRName,
                                        _ntdNRName, "1.0",
                                        SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Concept),
                                        new string[] { _ntdNRName });

            dim.ConceptRef = conceptRef;

            dsdMutable.AddDimension(dim);

            #endregion

            #region TimeDimension

            //IDimensionMutableObject dim2 = new DimensionMutableCore();
            //dim2.Id = _ntdTDName;
            //dim2.TimeDimension = true;

            //IStructureReference conceptRef2 = new StructureReferenceImpl(_ntdTDName,
            //                            _ntdTDName, "1.0",
            //                            SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Concept),
            //                            new string[] { _ntdTDName });

            //dim2.ConceptRef = conceptRef2;

            //dsdMutable.AddDimension(dim2);

            #endregion

            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl(_ntdPMName, _ntdPMName, "1.0", SdmxStructureEnumType.Concept, _ntdPMName));

            _dsdMutable = dsdMutable;

            SetDsdToSession();
        }

        /// <summary>
        /// Abilita i controlli per l'Editing
        /// </summary>
        private void SetEditingControl()
        {
            btnSaveDSD.Visible = true;

            // GENERAL
            if (_action == Action.INSERT)
            {
                txtDSDID.Enabled = true;
                txtVersion.Enabled = true;
                cmbAgencies.Enabled = true;
                cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                cmbAgencies.SelectedIndex = 0;
                //AnnotationGeneralControl.ClearAnnotationsSession();
            }

            txtDSDURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtDSDName.Enabled = true;
            txtDSDDescription.Enabled = true;
            chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;

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

            // DIMENSIONS
            txtDimID.Enabled = true;
        }

        /// <summary>
        /// Imposta il title della pagina con la DSD Selezionata
        /// </summary>
        private void SetLabelDetail()
        {
            lblDetail.Text = String.Format("KeyFamily: {0}, {1}, {2}", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
        }


        private void BindData()
        {
            IDataStructureObject ds = _sdmxObjects.DataStructures.FirstOrDefault();

            if (ds == null)
                return;

            if (_action == Action.VIEW)
            {
                _dsdMutable = ds.MutableInstance;
                SetDsdToSession();
            }

            SetGeneralTab(ds);
            SetPMTab(ds);
            SetDimensionTab(ds);
            SetGroupTab(ds);
            SetAttributeTab(ds);

        }

        private void SetGeneralTab(IDataStructureObject ds)
        {
            if (!ds.Id.Contains(_ntdString))
                txtDSDID.Text = ds.Id;

            cmbAgencies.SelectedValue = ds.AgencyId;
            txtAgenciesReadOnly.Text = ds.AgencyId;
            AnnotationGeneral.OwnerAgency = txtAgenciesReadOnly.Text;

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.Visible = true;
            }
            else
                DuplicateArtefact1.ucDisable = true;

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

            if (ds.Version != _ntdDSDVersion)
                txtVersion.Text = ds.Version;

            chkIsFinal.Checked = ds.IsFinal.IsTrue;

            if (ds.Uri != null)
                txtDSDURI.Text = ds.Uri.AbsoluteUri;

            if (ds.Urn != null && !ds.Id.Contains(_ntdString))
                txtDSDURN.Text = ds.Urn.AbsoluteUri;

            if (ds.StartDate != null)
                txtValidFrom.Text = ds.StartDate.Date.ToString();

            if (ds.EndDate != null)
                txtValidTo.Text = ds.EndDate.Date.ToString();

            txtDSDName.Text = _localizedUtils.GetNameableName(ds);
            txtDSDDescription.Text = _localizedUtils.GetNameableDescription(ds);


            if (_action != Action.VIEW)
            {
                IDataStructureMutableObject dsM = ds.MutableInstance;
                foreach (ITextTypeWrapperMutableObject name in dsM.Names)
                {
                    if (name.Value.Contains(_ntdString))
                    {
                        dsM.Names.Remove(name);
                        break;
                    }
                }

                AddTextName.InitTextObjectMutableList = dsM.Names;
                AddTextDescription.InitTextObjectList = ds.Descriptions;
            }

            AnnotationGeneral.AnnotationObjectList = ds.MutableInstance.Annotations;

        }

        private void SetPMTab(IDataStructureObject ds)
        {
            if (ds.PrimaryMeasure == null)
                return;

            ICrossReference conceptRef = ds.PrimaryMeasure.ConceptRef;

            txtPMID.Text = ds.PrimaryMeasure.Id;

            if (!conceptRef.FullId.Contains(_ntdString))
                txtConcept.Text = conceptRef.MaintainableId + "," + conceptRef.AgencyId + "," + conceptRef.Version + " - " + conceptRef.FullId;


            if (ds.PrimaryMeasure.Representation != null && ds.PrimaryMeasure.Representation.Representation != null)
            {
                ICrossReference cl = ds.PrimaryMeasure.Representation.Representation;
                txtCodelist.Text = cl.MaintainableId + "," + cl.AgencyId + "," + cl.Version;
            }

            AnnotationPrimaryMeasure.AnnotationObjectList = ds.MutableInstance.PrimaryMeasure.Annotations;

        }

        private void SetDimensionTab(IDataStructureObject ds)
        {
            List<Dimension> lDimension;

            ds = OrderDimension(ds);

            lDimension = _entityMapper.GetDimensionList(ds);

            gvDimension.DataSource = lDimension;
            gvDimension.DataBind();

        }

        private IAnnotationMutableObject CreateAnnotation(string id, string title,
                                                        string type, string uri, IList<ITextTypeWrapperMutableObject> texts)
        {

            IAnnotationMutableObject annotation = new AnnotationMutableCore();

            annotation.Id = id;
            annotation.Title = title;
            annotation.Type = type;
            annotation.Uri = (uri != string.Empty) ? new Uri(uri) : null;

            foreach (ITextTypeWrapperMutableObject text in texts)
                annotation.AddText(text);

            return annotation;
        }

        private IDataStructureObject OrderDimension(IDataStructureObject ds)
        {
            IDataStructureMutableObject dsMut = ds.MutableInstance;

            if (dsMut.Dimensions == null)
                return ds;

            var sortedDimension = dsMut.DimensionList.Dimensions.OrderBy(
            o =>
            {
                var corder = o.Annotations.FirstOrDefault(mutableObject => string.Equals(mutableObject.Type, "@ORDER@"));
                return corder != null ? int.Parse(corder.Text[0].Value) : 0;
            }).ToArray();

            dsMut.Dimensions.Clear();

            foreach (IDimensionMutableObject dim in sortedDimension)
            {
                dsMut.AddDimension(dim);
            }

            return dsMut.ImmutableInstance;
        }

        private IDataStructureMutableObject ReOrderDimension(IDataStructureMutableObject dsMut, IDimensionMutableObject dimMut, int inputOrder)
        {

            var sortedDimension = dsMut.DimensionList.Dimensions.OrderBy(
            o =>
            {
                var corder = o.Annotations.FirstOrDefault(mutableObject => string.Equals(mutableObject.Type, "@ORDER@"));
                return corder != null ? int.Parse(corder.Text[0].Value) : 0;
            }).ToArray();

            dsMut.Dimensions.Clear();

            int orderID = 1;

            IAnnotationMutableObject annOrder;

            foreach (IDimensionMutableObject dim in sortedDimension)
            {
                if (orderID == inputOrder)
                {
                    annOrder = dimMut.Annotations.Where(a => a.Type == "@ORDER@").FirstOrDefault();

                    if (annOrder != null)
                        dimMut.Annotations.Remove(annOrder);

                    dimMut.Annotations.Add(CreateAnnotation("order" + orderID.ToString(),
                                                        "",
                                                        "@ORDER@",
                                                        "", new List<ITextTypeWrapperMutableObject>(){
                                                            new TextTypeWrapperMutableCore("en",orderID.ToString()) }
                                                         )
                                        );
                    dsMut.AddDimension(dimMut);

                    ++orderID;
                }

                annOrder = dim.Annotations.Where(a => a.Type == "@ORDER@").FirstOrDefault();

                if (annOrder != null)
                    dim.Annotations.Remove(annOrder);

                dim.Annotations.Add(CreateAnnotation("order" + orderID.ToString(),
                                                    "",
                                                    "@ORDER@",
                                                    "", new List<ITextTypeWrapperMutableObject>(){
                                                            new TextTypeWrapperMutableCore("en",orderID.ToString()) }
                                                     )
                                    );
                dsMut.AddDimension(dim);

                ++orderID;
            }

            if (orderID <= inputOrder)
            {
                annOrder = dimMut.Annotations.Where(a => a.Type == "@ORDER@").FirstOrDefault();

                if (annOrder != null)
                    dimMut.Annotations.Remove(annOrder);

                dimMut.Annotations.Add(CreateAnnotation("order" + orderID.ToString(),
                                                    "",
                                                    "@ORDER@",
                                                    "", new List<ITextTypeWrapperMutableObject>(){
                                                            new TextTypeWrapperMutableCore("en",orderID.ToString()) }
                                                     )
                                    );
                dsMut.AddDimension(dimMut);
            }


            return dsMut;

        }

        private IDataStructureMutableObject ReOrderAttribute(IDataStructureMutableObject dsMut, IAttributeMutableObject attMut, int inputOrder)
        {
            if (dsMut.Attributes == null)
                return dsMut;

            var sortedAttributes = dsMut.AttributeList.Attributes.OrderBy(
            o =>
            {
                var corder = o.Annotations.FirstOrDefault(mutableObject => string.Equals(mutableObject.Type, "@ORDER@"));
                return corder != null ? int.Parse(corder.Text[0].Value) : 0;
            }).ToArray();

            dsMut.Attributes.Clear();

            int orderID = 1;

            IAnnotationMutableObject annOrder;

            foreach (IAttributeMutableObject att in sortedAttributes)
            {
                if (orderID == inputOrder)
                {
                    annOrder = attMut.Annotations.Where(a => a.Type == "@ORDER@").FirstOrDefault();

                    if (annOrder != null)
                        attMut.Annotations.Remove(annOrder);

                    attMut.Annotations.Add(CreateAnnotation("order" + orderID.ToString(),
                                                        "",
                                                        "@ORDER@",
                                                        "", new List<ITextTypeWrapperMutableObject>(){
                                                            new TextTypeWrapperMutableCore("en",orderID.ToString()) }
                                                         )
                                        );
                    dsMut.AddAttribute(attMut);

                    ++orderID;
                }

                annOrder = att.Annotations.Where(a => a.Type == "@ORDER@").FirstOrDefault();

                if (annOrder != null)
                    att.Annotations.Remove(annOrder);

                att.Annotations.Add(CreateAnnotation("order" + orderID.ToString(),
                                                    "",
                                                    "@ORDER@",
                                                    "", new List<ITextTypeWrapperMutableObject>(){
                                                            new TextTypeWrapperMutableCore("en",orderID.ToString()) }
                                                     )
                                    );
                dsMut.AddAttribute(att);

                ++orderID;
            }

            if (orderID <= inputOrder)
            {
                annOrder = attMut.Annotations.Where(a => a.Type == "@ORDER@").FirstOrDefault();

                if (annOrder != null)
                    attMut.Annotations.Remove(annOrder);

                attMut.Annotations.Add(CreateAnnotation("order" + orderID.ToString(),
                                                    "",
                                                    "@ORDER@",
                                                    "", new List<ITextTypeWrapperMutableObject>(){
                                                            new TextTypeWrapperMutableCore("en",orderID.ToString()) }
                                                     )
                                    );
                dsMut.AddAttribute(attMut);
            }

            return dsMut;
        }


        private void SetAttributeTab(IDataStructureObject ds)
        {
            List<ISTAT.Entity.Attribute> lAttribute;

            lAttribute = _entityMapper.GetAttributeList(ds);

            gvAttribute.DataSource = lAttribute;
            gvAttribute.DataBind();

            PopulateLBDimensionList(ds.MutableInstance, lbAttachmentDimension);
            PopulateAttachedGroup(ds.MutableInstance);
        }

        private void SetGroupTab(IDataStructureObject ds)
        {
            List<ISTAT.Entity.Group> lGroup;

            lGroup = _entityMapper.GetGroupList(ds);

            gvGroups.DataSource = lGroup;
            gvGroups.DataBind();

            PopulateLBDimensionList(ds.MutableInstance, lbGroupDimension);
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

            if (_action != Action.VIEW)
                AspConfirmationExit = "true";


        }


        /// <summary>
        /// 
        /// </summary>
        private void GetDsdFromSession()
        {
            if (Session[_snInsDSD] != null)
                _dsdMutable = (IDataStructureMutableObject)Session[_snInsDSD];

        }

        /// <summary>
        /// 
        /// </summary>
        private void SetDsdToSession()
        {
            Session[_snInsDSD] = _dsdMutable;
        }

        #endregion

        protected void btnOpenAddDimensionPopUp_Click(object sender, EventArgs e)
        {
            txtDimID.Text = "";
            txtDimConcept.Text = "";
            txtDimCodelist.Text = "";
            txtDimConceptScheme.Text = "";
            txtDimOrder.Text = "";
            cmbDimType.SelectedIndex = 0;
            cmbDimType.Enabled = true;
            txtDimID.Enabled = true;
            hdnEditDimension.Value = "";

            SetDimensionForm();
            OpenAddDimensionPopUp();
        }

        protected void btnOpenAddGroupPopUp_Click(object sender, ImageClickEventArgs e)
        {
            txtGroupID.Enabled = true;
            txtGroupID.Text = "";
            lbGroupDimension.ClearSelection();
            hdnEditGroup.Value = "";
            OpenAddGroupPopUp();
        }

        private void SetDimensionForm()
        {
            switch (cmbDimType.SelectedValue)
            {
                case "NORMAL":
                    txtDimID.Text = "";
                    txtDimID.Enabled = true;
                    pnlCodedRepresentationLabel.Visible = true;
                    pnlCodedRepresentationValue.Visible = true;
                    pnlConceptSchemeLabel.Visible = false;
                    pnlConceptSchemeValue.Visible = false;
                    break;
                case "FREQUENCY":
                    txtDimID.Text = "FREQ";
                    txtDimID.Enabled = false;
                    pnlCodedRepresentationLabel.Visible = true;
                    pnlCodedRepresentationValue.Visible = true;
                    pnlConceptSchemeLabel.Visible = false;
                    pnlConceptSchemeValue.Visible = false;
                    break;
                case "MEASURE":
                    txtDimID.Text = "";
                    txtDimID.Enabled = true;
                    pnlCodedRepresentationLabel.Visible = false;
                    pnlCodedRepresentationValue.Visible = false;
                    pnlConceptSchemeLabel.Visible = true;
                    pnlConceptSchemeValue.Visible = true;
                    break;
                case "TIME":
                    txtDimID.Text = "TIME_PERIOD";
                    txtDimID.Enabled = false;
                    pnlCodedRepresentationLabel.Visible = false;
                    pnlCodedRepresentationValue.Visible = false;
                    pnlConceptSchemeLabel.Visible = false;
                    pnlConceptSchemeValue.Visible = false;
                    break;
                default:
                    break;
            }
        }
    }
}