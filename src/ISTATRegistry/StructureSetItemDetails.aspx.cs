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
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using System.Data;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Mapping;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Mapping;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class StructureSetItemDetails : ISTATRegistry.Classes.ISTATWebPage
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
        const string _snInsSS = "INSSS";

        /// <summary>
        /// No True Data String
        /// </summary>
        const string _ntdString = "WeBrEgIsTrY";

        /// <summary>
        /// No True Data DSD Version
        /// </summary>
        const string _ntdDSDVersion = "1010101010";

        #endregion

        private IStructureSetMutableObject _ssMutable;

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
                            ISdmxObjects sdmxAppo = _wsmodel.GetStructureSet(_artIdentity, false, false);
                            Session[_snSdmxObjects] = sdmxAppo;
                        }
                        return (ISdmxObjects)Session[_snSdmxObjects];
                    case Action.UPDATE:
                        if (!IsPostBack)
                        {
                            ISdmxObjects sdmxAppo = _wsmodel.GetStructureSet(_artIdentity, false, false);
                            _ssMutable = sdmxAppo.StructureSets.FirstOrDefault().MutableInstance;
                            SetSSToSession();
                        }
                        ISdmxObjects sdmxObjectsU = new SdmxObjectsImpl();
                        sdmxObjectsU.AddStructureSet(_ssMutable.ImmutableInstance);
                        return sdmxObjectsU;
                        break;
                    case Action.INSERT:
                        ISdmxObjects sdmxObjectsI = new SdmxObjectsImpl();
                        sdmxObjectsI.AddStructureSet(_ssMutable.ImmutableInstance);
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

            _ssMutable = null;

            _sdmxUtils = new SDMXUtils();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _entityMapper = new EntityMapper(Utils.LocalizedLanguage);
            _artIdentity = Utils.GetIdentityFromRequest(Request);

            #region User COntrol Event Handler

            GetCodeListCLMSource.ucCodeListSelectedEH += new EventHandler<UserControls.GetCodeListEventArgs>(GetCodeListCLMSource_ucCodeListSelectedEH);
            GetCodeListCLMTarget.ucCodeListSelectedEH += new EventHandler<UserControls.GetCodeListEventArgs>(GetCodeListCLMTarget_ucCodeListSelectedEH);

            GetDSDSource.ucDSDSelectedEH += new EventHandler<UserControls.GetDSDEventArgs>(GetDSDSource_ucDSDSelectedEH);
            GetDSDTarget.ucDSDSelectedEH += new EventHandler<UserControls.GetDSDEventArgs>(GetDSDTarget_ucDSDSelectedEH);

            GetDataFlowSource.ucDFSelectedEH += new EventHandler<UserControls.GetDFEventArgs>(GetDataFlowSource_ucDFSelectedEH);
            GetDataFlowTarget.ucDFSelectedEH += new EventHandler<UserControls.GetDFEventArgs>(GetDataFlowTarget_ucDFSelectedEH);

            #endregion 

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

            DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.StructureSet;
            DuplicateArtefact1.ucMaintanableArtefact = _ssMutable;
            
            if (!IsPostBack)
            {
                hdnCLMSourceCodeScrollTop.Value = "0";
                lbCLMSourceCode.Attributes.Add("onclick", "GetListBoxScrollPosition();");
                lbCLMSourceCode.Attributes.Add("onfocus", "SetListBoxScrollPosition();");

                hdnCLMTargetCodeScrollTop.Value = "0";
                lbCLMTargetCode.Attributes.Add("onclick", "GetListBoxScrollPosition();");

            }
            else
            {

                switch (hdnSelectedMappingType.Value)
	            {
                    case "CLM":
                        lbCLMSourceCode.Focus();
                        break;
                    case "SM":
                        //lbSMSourceCode.Focus();
                        break;
		            default:
                break;
	            }

            }
        }


        /// <summary>
        /// Salva il DataFlow nel registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveSS_Click(object sender, EventArgs e)
        {

            if (!ValidateSSData())
                return;

            try
            {
                _ssMutable.Id = txtSSID.Text;
                _ssMutable.AgencyId = GetAgencyValue();
                _ssMutable.Version = txtVersion.Text;

                _ssMutable.Names.Clear();
                foreach (ITextTypeWrapperMutableObject name in AddTextName.TextObjectList)
                {
                    _ssMutable.Names.Add(name);
                }

                _ssMutable.Descriptions.Clear();
                if (AddTextDescription.TextObjectList != null)
                {
                    foreach (ITextTypeWrapperMutableObject descr in AddTextDescription.TextObjectList)
                    {
                        _ssMutable.Descriptions.Add(descr);
                    }
                }

                _ssMutable.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
                if (txtURI.Text != String.Empty)
                    _ssMutable.Uri = new Uri(txtURI.Text);

                if (txtValidFrom.Text != String.Empty)
                    _ssMutable.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
				else
					_ssMutable.StartDate = null;
                if (txtValidTo.Text != String.Empty)
                    _ssMutable.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
				else
					_ssMutable.EndDate = null;
				
                _ssMutable.Annotations.Clear();
                if (AnnotationGeneral.AnnotationObjectList != null)
                    foreach (var annotation in AnnotationGeneral.AnnotationObjectList)
                    {
                        _ssMutable.AddAnnotation(annotation);
                    }

                //string[] DSDValues = txtDSD.Text.Split(',');

                //IStructureReference dsdRef = new StructureReferenceImpl(DSDValues[1],
                //DSDValues[0], DSDValues[2],
                //SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd),
                //new string[] { _ntdString });

                //_ssMutable.DataStructureRef = dsdRef;

                SetSSToSession();

                WSModel wsModel = new WSModel();
                XmlDocument xRet = wsModel.SubmitStructure(_sdmxObjects);

                string err = Utils.GetXMLResponseError(xRet);

                if (err != "")
                {
                    Utils.ShowDialog(err);
                    return;
                }

                Session[_snInsSS] = null;
                Session[_snSdmxObjects] = null;

                //Utils.ShowDialog("Operation succesfully");
                Utils.ResetBeforeUnload();
                Utils.AppendScript("location.href='./StructureSet.aspx';");
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }

        protected void btnSaveCLM_Click(object sender, EventArgs e)
        {
            if (!ValidateCLM(false))
            {
                OpenAddCLMPopUp();
                return;
            }

            CreateOrUpdateCLM();
            PopolateGVCLM();
            OpenAddCLMPopUp();
            Utils.ShowDialog(Resources.Messages.succ_clm_update);
        }

        protected void btnSaveSM_Click(object sender, EventArgs e)
        {
            if (!ValidateSM(false))
            {
                OpenAddStructureMapPopUp();
                return;
            }

            CreateOrUpdateSM();
            PopolateGVSM();
            OpenAddStructureMapPopUp();
            Utils.ShowDialog(Resources.Messages.succ_sm_update);
        }

        private void GetCodeListCLMSource_ucCodeListSelectedEH(object sender, UserControls.GetCodeListEventArgs e)
        {
            if(!ValidateCLM(true))
            {
                txtCLSource.Text = "";
                return;
            }
            PopolateCLMLBSource(e.GetCodeListArtefactIdentity);
            ICodelistMapMutableObject clm = CreateOrUpdateCLM();
            UpdateCLMSource(ref clm, e.GetCodeListArtefactIdentity);
            ClearCLMMappings();
            SetSSToSession();
            PopolateGVCLM();
        }

        private void GetCodeListCLMTarget_ucCodeListSelectedEH(object sender, UserControls.GetCodeListEventArgs e)
        {
            if (!ValidateCLM(true))
            {
                txtCLSource.Text = "";
                return;
            }
            PopolateCLMLBTarget(e.GetCodeListArtefactIdentity);
            ICodelistMapMutableObject clm = CreateOrUpdateCLM();
            UpdateCLMTarget(ref clm, e.GetCodeListArtefactIdentity);
            ClearCLMMappings();
            SetSSToSession();
            PopolateGVCLM();
        }

        void GetDataFlowTarget_ucDFSelectedEH(object sender, UserControls.GetDFEventArgs e)
        {
            if (!ValidateSM(true))
            {
                txtArtefactTarget.Text = "";
                return;
            }
            ArtefactIdentity ai = Utils.GetArtefactIdentityFromString(txtArtefactTarget.Text);
            PopolateSMLBTarget(ai,AvailableStructures.DATAFLOW);

            IStructureMapMutableObject sm = CreateOrUpdateSM();
            UpdateSMTarget(ref sm, ai, "Dataflow");
            ClearSMMappings();
            SetSSToSession();
            PopolateGVSM();
        }

        void GetDataFlowSource_ucDFSelectedEH(object sender, UserControls.GetDFEventArgs e)
        {
            if (!ValidateSM(true))
            {
                txtArtefactSource.Text = "";
                return;
            }
            ArtefactIdentity ai = Utils.GetArtefactIdentityFromString(txtArtefactSource.Text);
            PopolateSMLBSource(ai, AvailableStructures.DATAFLOW);

            IStructureMapMutableObject sm = CreateOrUpdateSM();
            UpdateSMSource(ref sm, ai, "Dataflow");
            ClearSMMappings();
            SetSSToSession();
            PopolateGVSM();
        }

        void GetDSDTarget_ucDSDSelectedEH(object sender, UserControls.GetDSDEventArgs e)
        {
            if (!ValidateSM(true))
            {
                txtArtefactTarget.Text = "";
                return;
            }
            ArtefactIdentity ai = Utils.GetArtefactIdentityFromString(txtArtefactTarget.Text);
            PopolateSMLBTarget(ai, AvailableStructures.DSD);

            IStructureMapMutableObject sm = CreateOrUpdateSM();
            UpdateSMTarget(ref sm, ai, "Dsd");
            ClearSMMappings();
            SetSSToSession();
            PopolateGVSM();
        }

        void GetDSDSource_ucDSDSelectedEH(object sender, UserControls.GetDSDEventArgs e)
        {
            if (!ValidateSM(true))
            {
                txtArtefactSource.Text = "";
                return;
            }
            ArtefactIdentity ai = Utils.GetArtefactIdentityFromString(txtArtefactSource.Text);
            PopolateSMLBSource(ai, AvailableStructures.DSD);

            IStructureMapMutableObject sm = CreateOrUpdateSM();
            UpdateSMSource(ref sm, ai, "Dsd");
            ClearSMMappings();
            SetSSToSession();
            PopolateGVSM();
        }

        private void UpdateSMSource(ref IStructureMapMutableObject iStructureMapMutableObject, ArtefactIdentity artefactIdentity, string artefactType)
        {
            if (iStructureMapMutableObject == null)
                return;

            iStructureMapMutableObject.SourceRef = new StructureReferenceImpl(artefactIdentity.Agency,
                                                                            artefactIdentity.ID,
                                                                            artefactIdentity.Version,
                                                                            SdmxStructureType.GetFromEnum((SdmxStructureEnumType)Enum.Parse(typeof(SdmxStructureEnumType), artefactType)),
                                                                            null);
            SetSSToSession();

        }

        private void UpdateSMTarget(ref IStructureMapMutableObject iStructureMapMutableObject, ArtefactIdentity artefactIdentity, string artefactType)
        {
            if (iStructureMapMutableObject == null)
                return;

            iStructureMapMutableObject.TargetRef = new StructureReferenceImpl(artefactIdentity.Agency,
                                                                            artefactIdentity.ID,
                                                                            artefactIdentity.Version,
                                                                            SdmxStructureType.GetFromEnum((SdmxStructureEnumType)Enum.Parse(typeof(SdmxStructureEnumType), artefactType)),
                                                                            null);
            SetSSToSession();
        }

        private void UpdateCLMSource(ref ICodelistMapMutableObject iCodelistMapMutableObject, ArtefactIdentity artefactIdentity)
        {
            if (iCodelistMapMutableObject == null)
                return;

            iCodelistMapMutableObject.SourceRef = new StructureReferenceImpl(artefactIdentity.Agency,
                                                                            artefactIdentity.ID,
                                                                            artefactIdentity.Version,
                                                                            SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList),
                                                                            null);
            SetSSToSession();

        }

        private void UpdateCLMTarget(ref ICodelistMapMutableObject iCodelistMapMutableObject, ArtefactIdentity artefactIdentity)
        {
            if (iCodelistMapMutableObject == null)
                return;

            iCodelistMapMutableObject.TargetRef = new StructureReferenceImpl(artefactIdentity.Agency,
                                                                            artefactIdentity.ID,
                                                                            artefactIdentity.Version,
                                                                            SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList),
                                                                            null);
            SetSSToSession();
        }

        private ICodelistMapMutableObject CreateOrUpdateCLM()
        {
            if (txtCLMID.Text.Trim() == String.Empty)
                return null;

            ICodelistMapMutableObject clMap = GetCurrentCLM();

            if (clMap == null)
            {
                clMap = new CodelistMapMutableCore();
                _ssMutable.AddCodelistMap(clMap);
            }

            clMap.Id = txtCLMID.Text.Trim();

            clMap.Names.Clear();
            clMap.Descriptions.Clear();

            if (AddTextCLMNames.TextObjectList != null)
            {
                foreach (ITextTypeWrapperMutableObject txt in AddTextCLMNames.TextObjectList)
                    clMap.Names.Add(txt);
            }

            if (AddTextCLMDescription.TextObjectList != null)
            {
                foreach (ITextTypeWrapperMutableObject txt in AddTextCLMDescription.TextObjectList)
                    clMap.Descriptions.Add(txt);
            }

            if (clMap.Annotations != null && clMap.Annotations.Count <= 0 && ControlAnnotationsCLM.AnnotationObjectList.Count > 0)
            {
                foreach (IAnnotationMutableObject ann in ControlAnnotationsCLM.AnnotationObjectList)
                    clMap.AddAnnotation(ann);
            }

            SetSSToSession();

            return clMap;
        }

        private IStructureMapMutableObject CreateOrUpdateSM()
        {
            if (txtSMID.Text.Trim() == String.Empty)
                return null;

            IStructureMapMutableObject sMap = GetCurrentSM();

            if (sMap == null)
            {
                sMap = new StructureMapMutableCore();
                _ssMutable.AddStructureMap(sMap);
            }

            sMap.Id = txtSMID.Text.Trim();

            sMap.Names.Clear();
            sMap.Descriptions.Clear();

            if (AddTextSMNames.TextObjectList != null)
            {
                foreach (ITextTypeWrapperMutableObject txt in AddTextSMNames.TextObjectList)
                    sMap.Names.Add(txt);
            }

            if (AddTextSMDescription.TextObjectList != null)
            {
                foreach (ITextTypeWrapperMutableObject txt in AddTextSMDescription.TextObjectList)
                    sMap.Descriptions.Add(txt);
            }

            if (sMap.Annotations != null && sMap.Annotations.Count <= 0 && ControlAnnotationsSM.AnnotationObjectList.Count > 0)
            {
                foreach (IAnnotationMutableObject ann in ControlAnnotationsSM.AnnotationObjectList)
                    sMap.AddAnnotation(ann);
            }

            SetSSToSession();

            return sMap;
        }

        protected void btnAddCLM_Click(object sender, EventArgs e)
        {
            txtCLMID.Enabled = true;
            txtCLMID.Text = "";
            AddTextCLMNames.ClearTextObjectListWithOutJS();
            AddTextCLMDescription.ClearTextObjectListWithOutJS();
            ControlAnnotationsCLM.AnnotationObjectList = null;
            txtCLSource.Text = "";
            txtCLTarget.Text = "";
            lbCLMSourceCode.Items.Clear();
            ResetLBScrollPosition();
            lbCLMTargetCode.Items.Clear();

            gvCLMMapping.DataSource = null;
            gvCLMMapping.DataBind();

            Utils.AppendScript("tabindex = 0;");

            OpenAddCLMPopUp();
        }


        protected void btnAddCLMMap_Click(object sender, EventArgs e)
        {
            AddMapping();
            OpenAddCLMPopUp();
        }

        //TODO: da implementare
        protected void btnAddSMMap_Click(object sender, EventArgs e)
        {
            AddSMMapping();
            OpenAddStructureMapPopUp();
        }

        protected void btnAddSM_Click(object sender, EventArgs e)
        {

            txtSMID.Enabled = true;
            txtSMID.Text = "";
            AddTextSMNames.ClearTextObjectListWithOutJS();
            AddTextSMDescription.ClearTextObjectListWithOutJS();
            ControlAnnotationsSM.AnnotationObjectList = null;
            cmbArtefactType.SelectedIndex = 0;
            txtArtefactSource.Text = "";
            txtArtefactTarget.Text = "";
            lbSMSourceCode.Items.Clear();
            ResetSMLBScrollPosition();
            lbSMTargetCode.Items.Clear();

            gvSMMapping.DataSource = null;
            gvSMMapping.DataBind();

            Utils.AppendScript("tabindex = 0;");

            OpenAddStructureMapPopUp();
        }

        protected void cmbArtefactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //1. Svuoto le textbox del source e description
            //2. Visualizzo o nascondo gli usercontrol di Get
            //3. Svuoto le Listbox di Source e Target
            //4. Elimino i mapping 
            //5. Salvo l'oggetto _SSMutable
            //6. Riapro la Popup

            if (cmbArtefactType.SelectedValue == "Dsd")
            {
                GetDSDSource.Visible = true;
                GetDataFlowSource.Visible = false;
                GetDSDTarget.Visible = true;
                GetDataFlowTarget.Visible = false;
            }
            else if (cmbArtefactType.SelectedValue == "Dataflow")
            {
                GetDataFlowSource.Visible = true;
                GetDSDSource.Visible = false;
                GetDataFlowTarget.Visible = true;
                GetDSDTarget.Visible = false;
            }

            txtArtefactSource.Text = "";
            txtArtefactTarget.Text = "";

            lbSMSourceCode.Items.Clear();
            lbSMTargetCode.Items.Clear();
            ClearSMMappings();

            IStructureMapMutableObject sm = GetCurrentSM();

            if (sm != null)
            {
                sm.SourceRef = null;
                sm.TargetRef = null;
                sm.Components.Clear();

                SetSSToSession();
            }

            OpenAddStructureMapPopUp();
        }

        protected void gvCodeListMap_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCodeListMap.PageIndex = e.NewPageIndex;
        }

        protected void gvCodeListMap_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string clmID = ((Label)gvr.FindControl("lblCLMID")).Text;
                    string[] clSourceID = ((Label)gvr.FindControl("lblCLMCLSource")).Text.Split(',');
                    string[] clTargetID = ((Label)gvr.FindControl("lblCLMCLTarget")).Text.Split(',');

                    ArtefactIdentity aiCLSource = new ArtefactIdentity(clSourceID[0].Trim(), clSourceID[1].Trim(), clSourceID[2].Trim());
                    ArtefactIdentity aiCLTarget = new ArtefactIdentity(clTargetID[0].Trim(), clTargetID[1].Trim(), clTargetID[2].Trim());

                    hdCLMAction.Value = Action.UPDATE.ToString();

                    //++++ General
                    txtCLMID.Text = clmID;
                    ICodelistMapMutableObject clmap = GetCurrentCLM();

                    AddTextCLMNames.InitTextObjectMutableList = clmap.Names;
                    AddTextCLMDescription.InitTextObjectMutableList = clmap.Descriptions;
                    ControlAnnotationsCLM.AnnotationObjectList = clmap.Annotations;

                    //++++ Mapping 

                    //TODO: da rivedere
                    ArtefactTypeInit();

                    txtCLSource.Text = aiCLSource.ToString();
                    txtCLTarget.Text = aiCLTarget.ToString();

                    PopolateCLMLBSource(aiCLSource);
                    PopolateCLMLBTarget(aiCLTarget);

                    PopolateMapping();

                    Utils.AppendScript("tabindex = 0;");
                    txtCLMID.Enabled = false;
                    OpenAddCLMPopUp();

                    break;

                case "Annotation":
                    break;
            }
        }

        protected void gvCodeListMap_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string clmID = ((Label)gvCodeListMap.Rows[e.RowIndex].FindControl("lblCLMID")).Text;

            ICodelistMapMutableObject clmDelete = null;

            clmDelete = _ssMutable.CodelistMapList.Where(clm=>clm.Id == clmID).FirstOrDefault() ;

            try
            {
                if (clmDelete != null)
                {
                    _ssMutable.CodelistMapList.Remove(clmDelete);
                    SetCodeListMapTab(_ssMutable.ImmutableInstance);
                    SetSSToSession();
                }
            }
            catch (Exception ex)
            {
                _ssMutable.CodelistMapList.Add(clmDelete);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_clm);
            }

        }

        protected void gvStructureMap_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStructureMap.PageIndex = e.NewPageIndex;
        }

        protected void gvStructureMap_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string smID = ((Label)gvr.FindControl("lblSMID")).Text;
                    string[] SourceID = ((Label)gvr.FindControl("lblSMArtefactSource")).Text.Split(',');
                    string[] TargetID = ((Label)gvr.FindControl("lblSMArtefactTarget")).Text.Split(',');
                    string artType = ((Label)gvr.FindControl("lblSMArtefactType")).Text;
                    
                    AvailableStructures avStruct = AvailableStructures.DSD;

                    if (artType.ToUpper() == "DATAFLOW")
                    {
                        cmbArtefactType.SelectedIndex = 1;
                        avStruct = AvailableStructures.DATAFLOW;
                    }
                    else
                        cmbArtefactType.SelectedIndex = 1;

                    ArtefactIdentity aiSource = new ArtefactIdentity(SourceID[0].Trim(), SourceID[1].Trim(), SourceID[2].Trim());
                    ArtefactIdentity aiTarget = new ArtefactIdentity(TargetID[0].Trim(), TargetID[1].Trim(), TargetID[2].Trim());

                    hdCLMAction.Value = Action.UPDATE.ToString();

                    //++++ General
                    txtSMID.Text = smID;
                    IStructureMapMutableObject smmap = GetCurrentSM();

                    AddTextSMNames.InitTextObjectMutableList = smmap.Names;
                    AddTextSMDescription.InitTextObjectMutableList = smmap.Descriptions;
                    ControlAnnotationsSM.AnnotationObjectList = smmap.Annotations;

                    //++++ Mapping 
                    txtArtefactSource.Text = aiSource.ToString();
                    txtArtefactTarget.Text = aiTarget.ToString();

                    PopolateSMLBSource(aiSource, avStruct);
                    PopolateSMLBTarget(aiTarget, avStruct);

                    PopolateSMMapping();

                    Utils.AppendScript("tabindex = 0;");
                    txtSMID.Enabled = false;
                    OpenAddStructureMapPopUp();

                    break;

                case "Annotation":
                    break;
            }
        }


        protected void gvStructureMap_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string ID = ((Label)gvStructureMap.Rows[e.RowIndex].FindControl("lblSMID")).Text;

            IStructureMapMutableObject smDelete = null;

            smDelete = _ssMutable.StructureMapList.Where(sm => sm.Id == ID).FirstOrDefault();

            try
            {
                if (smDelete != null)
                {
                    _ssMutable.StructureMapList.Remove(smDelete);
                    SetStructureMapTab(_ssMutable.ImmutableInstance);
                    SetSSToSession();
                }
            }
            catch (Exception ex)
            {
                _ssMutable.StructureMapList.Add(smDelete);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_sm);
            }

        }

        protected void gvCLMMapping_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string clSourceCode = ((Label)gvCLMMapping.Rows[e.RowIndex].FindControl("lblCLMSourceCode")).Text;
            string clTargetCode = ((Label)gvCLMMapping.Rows[e.RowIndex].FindControl("lblCLMTargetCode")).Text;

            ICodelistMapMutableObject clm = GetCurrentCLM();

            IItemMapMutableObject itemMap = clm.Items.Where(map => map.SourceId == clSourceCode && map.TargetId == clTargetCode).FirstOrDefault();

            //MaintainScrollPositionOnPostback

            if (itemMap == null)
                return;

            try
            {
                clm.Items.Remove(itemMap);
                PopolateMapping();
                SetSSToSession();
                OpenAddCLMPopUp();
            }
            catch (Exception ex)
            {
                clm.Items.Add(itemMap);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_clm_mapping);
            }

        }

        protected void gvCLMMapping_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCLMMapping.PageIndex = e.NewPageIndex;
            PopolateMapping();
            OpenAddCLMPopUp();
        }

        protected void gvSMMapping_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string SourceCode = ((Label)gvSMMapping.Rows[e.RowIndex].FindControl("lblSMSourceDimension")).Text;
            string TargetCode = ((Label)gvSMMapping.Rows[e.RowIndex].FindControl("lblSMTargetDimension")).Text;

            IStructureMapMutableObject sm = GetCurrentSM();

            IComponentMapMutableObject itemMap = sm.Components.Where(map => map.MapConceptRef == SourceCode && map.MapTargetConceptRef == TargetCode).FirstOrDefault();


            if (itemMap == null)
                return;

            try
            {
                sm.Components.Remove(itemMap);
                PopolateSMMapping();
                SetSSToSession();
                OpenAddStructureMapPopUp();
            }
            catch (Exception ex)
            {
                sm.Components.Add(itemMap);
                Utils.ShowDialog(ex.Message, 600, Resources.Messages.err_delete_sm_mapping);
            }

        }

        protected void gvSMMapping_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCLMMapping.PageIndex = e.NewPageIndex;
            PopolateMapping();
            OpenAddCLMPopUp();
        }

        #endregion

        #region Methods

        private bool ValidateSSData()
        {

            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtSSID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
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
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_save_structureset);
                return false;
            }

            return true;
        }

        private bool ValidateSM(bool NoSourceTargetCheck)
        {
            string messagesGroup = string.Empty;
            int errorCounter = 1;

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtSMID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo presenza NAME
            if (AddTextSMNames.TextObjectList == null || AddTextSMNames.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
            }

            if (!NoSourceTargetCheck)
            {
                // Controllo presenza Source e Target Artefacts
                if (txtArtefactSource.Text.Trim() == String.Empty || txtArtefactTarget.Text.Trim() == String.Empty)
                {
                    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_sm_missing_source_target + "<br /><br />";
                    errorCounter++;
                }

                // Controllo mapping
                if (gvSMMapping.Rows == null || gvSMMapping.Rows.Count <= 0)
                {
                    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_add_mapping + "<br /><br />";
                    errorCounter++;
                }
            }

            // Bug Common API
            //if (gvSMMapping.Rows.Count > 1)
            //{
            //    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_sm_commonApi_BUG + "<br /><br />";
            //    errorCounter++;
            //}

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_save_sm);
                return false;
            }

            return true;
        }

        private bool ValidateCLM(bool NoSourceTargetCheck)
        {
            string messagesGroup = string.Empty;    
            int errorCounter = 1;                   

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtCLMID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo presenza NAME
            if (AddTextCLMNames.TextObjectList == null || AddTextCLMNames.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
            }

            if (!NoSourceTargetCheck)
            {
                // Controllo presenza Source e Target CL
                if (txtCLSource.Text.Trim() == String.Empty || txtCLTarget.Text.Trim() == String.Empty)
                {
                    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_clm_missing_source_target + "<br /><br />";
                    errorCounter++;
                }

                // Controllo mapping
                if (gvCLMMapping.Rows == null || gvCLMMapping.Rows.Count <=0)
                {
                    messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_add_mapping + "<br /><br />";
                    errorCounter++;
                }
            }

            if (messagesGroup != String.Empty)
            {
                Utils.ShowDialog(messagesGroup, 300, Resources.Messages.err_save_clm);
                return false;
            }

            return true;
        }

        private ICodelistMapMutableObject GetCurrentCLM()
        {
            string clmID = txtCLMID.Text;

            if (clmID.Trim() == String.Empty)
                return null;

            return _ssMutable.CodelistMapList.Where(clm => clm.Id == clmID).FirstOrDefault();
        }

        private IStructureMapMutableObject GetCurrentSM()
        {
            string smID = txtSMID.Text;

            if (smID.Trim() == String.Empty)
                return null;

            return _ssMutable.StructureMapList.Where(sm => sm.Id == smID).FirstOrDefault();
        }

        private void PopolateMapping()
        {

            ICodelistMapMutableObject clmap = GetCurrentCLM();

            if (clmap == null)
                return;

            DataTable dt = GetMappingDataTable();
            DataRow dr;

            foreach (IItemMapMutableObject map in clmap.Items)
            {
                dr = dt.NewRow();
                dr["SourceCode"] = map.SourceId;
                dr["TargetCode"] = map.TargetId;
                dt.Rows.Add(dr);
            }

            gvCLMMapping.DataSource = dt;
            gvCLMMapping.DataBind();
        }


        private void PopolateSMMapping()
        {
            IStructureMapMutableObject sMap = GetCurrentSM();

            if (sMap == null)
                return;

            DataTable dt = GetMappingDataTable();
            DataRow dr;

            foreach (IComponentMapMutableObject map in sMap.Components)
            {
                dr = dt.NewRow();
                dr["SourceCode"] = map.MapConceptRef;
                dr["TargetCode"] = map.MapTargetConceptRef;
                dt.Rows.Add(dr);
            }

            gvSMMapping.DataSource = dt;
            gvSMMapping.DataBind();
        }


        private DataTable GetMappingDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("SourceCode");
            dt.Columns.Add("TargetCode");
            dt.PrimaryKey = new DataColumn[] { dt.Columns["SourceCode"], dt.Columns["TargetCode"] };

            return dt;
        }

        private void AddSMMapping()
        {
            IStructureMapMutableObject sMap = GetCurrentSM();

            if (String.IsNullOrEmpty(lbSMSourceCode.SelectedValue) || String.IsNullOrEmpty(lbSMTargetCode.SelectedValue))
                return;


            if (sMap.Components.Where(map => map.MapConceptRef == lbSMSourceCode.SelectedValue && map.MapTargetConceptRef == lbSMTargetCode.SelectedValue).FirstOrDefault() != null)
            {
                Utils.ShowDialog(Resources.Messages.err_existing_mapping);
                return;
            }

            ComponentMapMutableCore smComp = new ComponentMapMutableCore();

            smComp.MapConceptRef = lbSMSourceCode.SelectedValue;
            smComp.MapTargetConceptRef = lbSMTargetCode.SelectedValue;

            sMap.AddComponent(smComp);

            PopolateSMMapping();
            SetSSToSession();
        }

        private void AddMapping()
        {
            ICodelistMapMutableObject clmap = GetCurrentCLM();

            if (String.IsNullOrEmpty(lbCLMSourceCode.SelectedValue) || String.IsNullOrEmpty(lbCLMTargetCode.SelectedValue))
                return;


            if (clmap.Items.Where(map => map.SourceId == lbCLMSourceCode.SelectedValue && map.TargetId == lbCLMTargetCode.SelectedValue).FirstOrDefault() != null)
            {
                Utils.ShowDialog(Resources.Messages.err_existing_mapping);
                return; //Mapping esistente
            }

            clmap.AddItem(new ItemMapMutableCore(new ItemMapCore(lbCLMSourceCode.SelectedValue, lbCLMTargetCode.SelectedValue, null)));

            PopolateMapping();
            SetSSToSession();
        }

        private void PopolateCLMLBTarget(ArtefactIdentity artefactIdentity)
        {
            lbCLMTargetCode.Items.Clear();
            ISdmxObjects sdmxObjects = _wsmodel.GetCodeList(artefactIdentity, false, false);

            foreach (ICode code in sdmxObjects.Codelists.FirstOrDefault().Items)
            {
                lbCLMTargetCode.Items.Add(code.Id);
            }

            ResetLBScrollPosition();
        }

        private void PopolateCLMLBSource(ArtefactIdentity artefactIdentity)
        {
            lbCLMSourceCode.Items.Clear();
            ISdmxObjects sdmxObjects = _wsmodel.GetCodeList(artefactIdentity, false, false);

            foreach (ICode code in sdmxObjects.Codelists.FirstOrDefault().Items)
            {
                lbCLMSourceCode.Items.Add(code.Id);
            }

            ResetLBScrollPosition();
        }

        private void PopolateSMLBTarget(ArtefactIdentity artefactIdentity, AvailableStructures StructureType)
        {
            lbSMTargetCode.Items.Clear();

            ISdmxObjects sdmxObjects = GetSdmoxObjectsFromAI(artefactIdentity, StructureType);

            foreach (IDimension dim in sdmxObjects.DataStructures.FirstOrDefault().DimensionList.Dimensions)
            {
                lbSMTargetCode.Items.Add(dim.Id);
            }

            ResetLBScrollPosition();
        }

        private void PopolateSMLBSource(ArtefactIdentity artefactIdentity,AvailableStructures StructureType)
        {
            lbSMSourceCode.Items.Clear();

            ISdmxObjects sdmxObjects = GetSdmoxObjectsFromAI(artefactIdentity, StructureType);

            foreach (IDimension dim in sdmxObjects.DataStructures.FirstOrDefault().DimensionList.Dimensions)
            {
                lbSMSourceCode.Items.Add(dim.Id);
            }

            ResetLBScrollPosition();
        }

        private ISdmxObjects GetSdmoxObjectsFromAI(ArtefactIdentity artefactIdentity, AvailableStructures StructureType)
        {
            ISdmxObjects sdmxObjects = null;

            switch (StructureType)
            {
                case AvailableStructures.DSD:
                    sdmxObjects = _wsmodel.GetDataStructure(artefactIdentity, false, false);
                    break;
                case AvailableStructures.DATAFLOW:
                    sdmxObjects = _wsmodel.GetDataFlow(artefactIdentity, false, false);
                    ICrossReference dsd = sdmxObjects.Dataflows.FirstOrDefault().DataStructureRef;

                    sdmxObjects = _wsmodel.GetDataStructure(new ArtefactIdentity(dsd.MaintainableId, dsd.AgencyId, dsd.Version), false, false);
                    break;
            }

            return sdmxObjects;
        }

        private void ClearSMMappings()
        {
            if (txtSMID.Text.Trim() == String.Empty)
                return;
            IStructureMapMutableObject sMap = GetCurrentSM();

            if (sMap == null)
                return;

            sMap.Components.Clear();
            gvSMMapping.DataBind();
            SetSSToSession();
        }

        private void ClearCLMMappings()
        {
            if(txtCLMID.Text.Trim() == String.Empty)
                return;
            ICodelistMapMutableObject clMap = GetCurrentCLM();

            if (clMap == null)
                return;

            clMap.Items.Clear();
            gvCLMMapping.DataBind();
            SetSSToSession();
        }

        private void CommonInit()
        {
            cmbArtefactType.Items.Add(new ListItem("Data Structure Definition", "Dsd"));
            cmbArtefactType.Items.Add(new ListItem("DataFlow", "Dataflow"));

            Session[_snSdmxObjects] = null;
            Session[_snInsSS] = null;

            Utils.PopulateCmbAgencies(cmbAgencies, true);
        }

        //TODO: Da implementare
        private void ArtefactTypeInit()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Agency");
            dt.Columns.Add("Version");
            dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"], dt.Columns["Agency"], dt.Columns["Version"] };

            //if (_ccMutable.ConstraintAttachment != null)
            //{
            //    SdmxStructureType strucType = _ccMutable.ConstraintAttachment.StructureReference.FirstOrDefault().TargetReference;
            //    cmbArtefactType.SelectedValue = strucType.EnumType.ToString();
            //}

            //if (_ccMutable.ConstraintAttachment != null)
            //{
            //    foreach (IStructureReference str in _ccMutable.ConstraintAttachment.StructureReference)
            //    {
            //        DataRow dr = dt.NewRow();
            //        dr["ID"] = str.MaintainableId;
            //        dr["Agency"] = str.AgencyId;
            //        dr["Version"] = str.Version;
            //        dt.Rows.Add(dr);
            //    }
            //}
            //_dtArtefacts = dt;
            //GetDSD1.ucTargetDataTable = _dtArtefacts;
            //GetDataFlow1.ucTargetDataTable = _dtArtefacts;
        }

        private void EnableDownload()
        {
            FileDownload31.Visible = true;
            FileDownload31.ucID = _artIdentity.ID;
            FileDownload31.ucAgency = _artIdentity.Agency;
            FileDownload31.ucVersion = _artIdentity.Version;
            FileDownload31.ucArtefactType = "StructureSet";
        }

        private void InitUserControl()
        {
            #region GENERAL

            // Name User Control options
            AddTextName.ArtefactType = SdmxStructureEnumType.StructureSet;
            AddTextName.TType = TextType.NAME;

            // Description User Control options
            AddTextDescription.ArtefactType = SdmxStructureEnumType.StructureSet;
            AddTextDescription.TType = TextType.DESCRIPTION;

            // Annotation
            AnnotationGeneral.AddText_ucOpenTabName = null;
            AnnotationGeneral.AddText_ucOpenPopUpName = null;

            #endregion

            #region CODELISTMAP

            AddTextCLMNames.TType = TextType.NAME;
            AddTextCLMNames.ucOpenPopUpName = "df-CLM";
            AddTextCLMNames.ucOpenPopUpWidth = 800;

            AddTextCLMDescription.TType = TextType.DESCRIPTION;
            AddTextCLMDescription.ucOpenPopUpName = "df-CLM";
            AddTextCLMDescription.ucOpenPopUpWidth = 800;

            ControlAnnotationsCLM.PopUpContainer = "df-CLM";
            ControlAnnotationsCLM.PopUpContainerWidth = 800;
            ControlAnnotationsCLM.AddText_ucOpenPopUpWidth = 800;

            GetCodeListCLMSource.TargetWebControl = txtCLSource;
            GetCodeListCLMSource.ucOpenPopUpName = "df-CLM";
            GetCodeListCLMSource.ucOpenPopUpWidth = 800;

            GetCodeListCLMTarget.TargetWebControl = txtCLTarget;
            GetCodeListCLMTarget.ucOpenPopUpName = "df-CLM";
            GetCodeListCLMTarget.ucOpenPopUpWidth = 800;

            #endregion

            #region STRUCTUREMAP

            AddTextSMNames.TType = TextType.NAME;
            AddTextSMNames.ucOpenPopUpName = "df-SM";
            AddTextSMNames.ucOpenPopUpWidth = 800;

            AddTextSMDescription.TType = TextType.DESCRIPTION;
            AddTextSMDescription.ucOpenPopUpName = "df-SM";
            AddTextSMDescription.ucOpenPopUpWidth = 800;

            ControlAnnotationsSM.PopUpContainer = "df-SM";
            ControlAnnotationsSM.PopUpContainerWidth = 800;
            ControlAnnotationsSM.AddText_ucOpenPopUpWidth = 800;

            GetDSDSource.TargetWebControl = txtArtefactSource;
            GetDSDSource.ucOpenPopUpName = "df-SM";
            GetDSDSource.ucOpenPopUpWidth = 800;

            GetDataFlowSource.TargetWebControl = txtArtefactSource;
            GetDataFlowSource.ucOpenPopUpName = "df-SM";
            GetDataFlowSource.ucOpenPopUpWidth = 800;

            GetDSDTarget.TargetWebControl = txtArtefactTarget;
            GetDSDTarget.ucOpenPopUpName = "df-SM";
            GetDSDTarget.ucOpenPopUpWidth = 800;

            GetDataFlowTarget.TargetWebControl = txtArtefactTarget;
            GetDataFlowTarget.ucOpenPopUpName = "df-SM";
            GetDataFlowTarget.ucOpenPopUpWidth = 800;

            #endregion

        }

        private void SetViewForm()
        {
            GetSSFromSession();
            InitUserControl();
            EnableDownload();

            if (!IsPostBack)
            {
                //SetLabelDetail();
                pnlViewName.Visible = true;
                pnlViewDescription.Visible = true;
                pnlAddCLM.Visible = false;
                pnlAddSM.Visible = false;
                AnnotationGeneral.EditMode = false;
                cmbArtefactType.Enabled = false;
                AddTextCLMNames.ucEditMode = false;
                AddTextCLMDescription.ucEditMode = false;
                ControlAnnotationsCLM.EditMode = false;
                AddTextSMNames.ucEditMode = false;
                AddTextSMDescription.ucEditMode = false;
                ControlAnnotationsSM.EditMode = false;

                GetCodeListCLMSource.Visible = false;
                GetCodeListCLMTarget.Visible = false;
                btnAddCLMMap.Visible = false;
                gvCLMMapping.Columns[2].Visible = false;

                GetDSDSource.Visible = false;
                GetDSDTarget.Visible = false;
                GetDataFlowSource.Visible = false;
                GetDataFlowTarget.Visible = false;
                btnAddSMMap.Visible = false;
                gvSMMapping.Columns[2].Visible = false;


                gvCodeListMap.Columns[3].Visible = false;
                gvStructureMap.Columns[4].Visible = false;

                BindData();
            }
        }

        private void SetEditForm()
        {
            GetSSFromSession();
            InitUserControl();
            EnableDownload();

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
            GetSSFromSession();

            if (_ssMutable == null)
                CreateEmptyStructureSet();
        }

        private void CreateEmptyStructureSet()
        {
            IStructureSetMutableObject ssMutable;

            ssMutable = _sdmxUtils.buildStructureSet(_ntdString + "SS_ID", _ntdString + "AGENCY", _ntdDSDVersion);
            ssMutable.AddName("en", _ntdString + "SS_NAME");

            //IStructureReference dsdRef = new StructureReferenceImpl(_ntdString,
            //                _ntdString, "1.0",
            //                SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd),
            //                new string[] { _ntdString });

            //ssMutable.DataStructureRef = dsdRef;

            _ssMutable = ssMutable;

            SetSSToSession();
        }


        /// <summary>
        /// Abilita i controlli per l'Editing
        /// </summary>
        private void SetEditingControl()
        {
            AnnotationGeneral.EditMode = true;
            btnSaveSS.Visible = true;

            // GENERAL
            if (_action == Action.INSERT)
            {
                txtSSID.Enabled = true;
                txtVersion.Enabled = true;
                cmbAgencies.Enabled = true;
                cmbAgencies.Enabled = true;
                cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                cmbAgencies.SelectedIndex = 0;
                AnnotationGeneral.ClearAnnotationsSession();
            }

            txtURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtSSName.Enabled = true;
            txtSSDescription.Enabled = true;
            chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;
            btnSaveCLM.Visible = true;
            btnSaveSM.Visible = true;
            btnSaveCLM2.Visible = true;
            btnSaveSM2.Visible = true;

        }


        private void BindData()
        {
            IStructureSetObject sso = _sdmxObjects.StructureSets.FirstOrDefault();

            if (sso == null)
                return;

            if (_action == Action.VIEW)
            {
                _ssMutable = sso.MutableInstance;
                SetSSToSession();
            }

            SetGeneralTab(sso);
            SetCodeListMapTab(sso);
            SetStructureMapTab(sso);
        }

        private void SetCodeListMapTab(IStructureSetObject sso)
        {
            PopolateGVCLM();
        }

        private void PopolateGVCLM()
        {
            List<ISTAT.Entity.CodeListMap> lCLM;

            try
            {
                lCLM = _entityMapper.GetCodeListMapList(_ssMutable.ImmutableInstance);

                gvCodeListMap.DataSource = lCLM;
                gvCodeListMap.DataBind();
            }
            catch (Exception ex)
            {
            }
        }

        private void SetStructureMapTab(IStructureSetObject sso)
        {
            PopolateGVSM();
        }

        private void PopolateGVSM()
        {
            List<ISTAT.Entity.StructureMap> lSM;

            try
            {
                lSM = _entityMapper.GetStructureMapList(_ssMutable.ImmutableInstance);

                gvStructureMap.DataSource = lSM;
                gvStructureMap.DataBind();
            }
            catch (Exception ex)
            {
            }
        }

        private void SetGeneralTab(IStructureSetObject sso)
        {
            if (!sso.Id.Contains(_ntdString))
                txtSSID.Text = sso.Id;

            cmbAgencies.SelectedValue = sso.AgencyId;
            txtAgenciesReadOnly.Text = sso.AgencyId;
            AnnotationGeneral.OwnerAgency = txtAgenciesReadOnly.Text;

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.Visible = true;
            }

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

            if (sso.Version != _ntdDSDVersion)
                txtVersion.Text = sso.Version;

            chkIsFinal.Checked = sso.IsFinal.IsTrue;

            if (sso.Uri != null)
                txtURI.Text = sso.Uri.AbsoluteUri;

            if (sso.Urn != null)
                txtURN.Text = sso.Urn.AbsoluteUri;

            if (sso.StartDate != null)
                txtValidFrom.Text = sso.StartDate.Date.ToString();

            if (sso.EndDate != null)
                txtValidTo.Text = sso.EndDate.Date.ToString();

            //var dataStructureRef = sso.DataStructureRef;

            //if (!dataStructureRef.AgencyId.Contains(_ntdString))
            //    txtDSD.Text = dataStructureRef.MaintainableId + "," + dataStructureRef.AgencyId + "," + dataStructureRef.Version;


            txtSSName.Text = _localizedUtils.GetNameableName(sso);
            txtSSDescription.Text = _localizedUtils.GetNameableDescription(sso);

            if (_action != Action.VIEW)
            {
                IStructureSetMutableObject ssM = sso.MutableInstance;

                foreach (ITextTypeWrapperMutableObject name in ssM.Names)
                {
                    if (name.Value.Contains(_ntdString))
                    {
                        ssM.Names.Remove(name);
                        break;
                    }
                }

                AddTextName.InitTextObjectMutableList = ssM.Names;
                AddTextDescription.InitTextObjectList = sso.Descriptions;
            }

            AnnotationGeneral.AnnotationObjectList = sso.MutableInstance.Annotations;

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
        private void GetSSFromSession()
        {
            if (Session[_snInsSS] != null)
                _ssMutable = (IStructureSetMutableObject)Session[_snInsSS];
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetSSToSession()
        {
            Session[_snInsSS] = _ssMutable;
        }


        private void OpenAddCLMPopUp()
        {
            hdnSelectedMappingType.Value = "CLM";
            Utils.AppendScript("openP('df-CLM',800);");
        }

        private void OpenAddStructureMapPopUp()
        {
            hdnSelectedMappingType.Value = "SM";
            Utils.AppendScript("openP('df-SM',800);");
        }

        private void ResetLBScrollPosition()
        {
            hdnCLMSourceCodeScrollTop.Value = "0";
        }

        private void ResetSMLBScrollPosition()
        {
            hdnSMSourceCodeScrollTop.Value = "0";
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
        #endregion


    }
}