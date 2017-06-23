using ISTAT.Entity;
using ISTAT.EntityMapper;
using ISTAT.WSDAL;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry
{
    public partial class CategorySchemeGraph : System.Web.UI.Page //ISTATRegistry.Classes.ISTATWebPage
    {
        #region Override

        protected override void InitializeCulture()
        {
            CultureInfo culture;

            culture = LocaleResolver.GetCookie(HttpContext.Current);

            UICulture = culture.TwoLetterISOLanguageName;
            Culture = culture.TwoLetterISOLanguageName;

            // Save cookie
            LocaleResolver.SendCookie(culture, HttpContext.Current);

            //base.InitializeCulture();
        }

        #endregion

        #region Properties

        private WSModel _wsModel;
        private ArtefactIdentity _artIdentity;
        private ISdmxObjects _sdmxCS;
        private ISdmxObjects _sdmxArtefacts;
        //private CategoryViewConfigurationSection _config;
        private LocalizedUtils _localizedUtils;
        private EntityMapper _entityMapper;
        private List<string> _lVisibleArtefacts;
        private EndPointElement _epe;

        #endregion

        #region  Events

        protected void Page_Load(object sender, EventArgs e)
        {

            _epe = (EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT];

            _wsModel = new WSModel();
            //_config = (CategoryViewConfigurationSection)System.Configuration.ConfigurationManager.GetSection("categoryViewConfigurationGroup/categoryViewConfiguration");
            _entityMapper = new EntityMapper(cmbLanguage.SelectedValue);
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(FileDownload3);

            if (!IsPostBack)
            {
                FileDownload3.ucVisible = false;

                //ISTATUtils.EndPointElement epe = new EndPointElement()
                //{
                //    ActiveEndPointType = ActiveEndPointType.SOAP,
                //    NSIEndPoint = _config.WsEndPoint,
                //    RestEndPoint = ""

                //};

                //Session["WSEndPoint"] = epe;

                _artIdentity = Utils.GetIdentityFromRequest(Request);
                FillForm();

                cmbLanguage.SelectedValue = LocaleResolver.GetCookie(HttpContext.Current).TwoLetterISOLanguageName;
            }

            _localizedUtils = new LocalizedUtils(new System.Globalization.CultureInfo(cmbLanguage.SelectedValue));

            //if (!IsPostBack)
            //    if (cmbCategorySchemes.Items.Count == 1 || cmbCategorySchemes.Enabled == false)
            //        StartSearching();
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            if (!((chkDataflow.Visible && chkDataflow.Checked) ||
                (chkCodelist.Visible && chkCodelist.Checked) ||
                (chkConcSchema.Visible && chkConcSchema.Checked) ||
                (chkDSD.Visible && chkDSD.Checked))
                )
            {
                ShowDialog("Selezionare almeno una tipologia di artefatto.");
                return;
            }
            StartSearching();
            FileDownload3.ucVisible = false;
        }

        protected void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {

            bool foundLanguage = false;
            string[] resources = Directory.GetFiles(Server.MapPath("~/App_GlobalResources"), "*.resx");
            foreach (string resourceFile in resources)
            {
                string[] splittedPath = resourceFile.Split('\\');
                string fileName = splittedPath[splittedPath.Length - 1];
                string languageName = fileName.Split('.')[1];
                if (languageName.Equals(cmbLanguage.SelectedItem.Value))
                {
                    foundLanguage = true;
                    break;
                }
            }

            if (foundLanguage)
            {
                Session[SESSION_KEYS.KEY_LANG] = cmbLanguage.SelectedItem.Value;
                Session[SESSION_KEYS.OLD_KEY_LANG] = cmbLanguage.SelectedItem.Value;
                LocaleResolver.SendCookie(new System.Globalization.CultureInfo(Session[SESSION_KEYS.KEY_LANG].ToString()), HttpContext.Current);
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                string defaultLanguage = ConfigurationManager.AppSettings["DefaultLanguageForResources"].ToString();
                if (defaultLanguage.Equals(cmbLanguage.SelectedValue))
                {
                    Session[SESSION_KEYS.KEY_LANG] = cmbLanguage.SelectedItem.Value;
                    Session[SESSION_KEYS.OLD_KEY_LANG] = cmbLanguage.SelectedItem.Value;
                    LocaleResolver.SendCookie(new System.Globalization.CultureInfo(Session[SESSION_KEYS.KEY_LANG].ToString()), HttpContext.Current);
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    cmbLanguage.SelectedValue = Session[SESSION_KEYS.OLD_KEY_LANG].ToString();
                    ShowDialog(Resources.Messages.err_missing_language_file);
                }
            }

            StartSearching();
            FileDownload3.ucVisible = false;
        }


        protected void tvCategory_SelectedNodeChanged(object sender, EventArgs e)
        {
            string[] oValues;

            if (Session["PREVIOUS_NODE"] != null)
            {
                string ndeValuePath = Session["PREVIOUS_NODE"].ToString();

                TreeNode node;

                node = tvCategory.FindNode(ndeValuePath);

                if (node != null)
                    node.SelectAction = TreeNodeSelectAction.Select;
            }
            tvCategory.SelectedNode.SelectAction = TreeNodeSelectAction.None;
            Session["PREVIOUS_NODE"] = tvCategory.SelectedNode.ValuePath;

            oValues = tvCategory.SelectedNode.Value.Split('|');
            ArtefactIdentity ai = Utils.GetArtefactIdentityFromString(oValues[1]);

            ISdmxObjects sdmxObject;

            sdmxObject = GetSdmxObjectArtefact(oValues[0], ai, false);

            pnlResult.Controls.Clear();

            switch (oValues[0].ToUpper())
            {
                case "DSD":
                    FillDSD(sdmxObject);
                    break;
                case "CONCEPTSCHEME":
                    FillConceptScheme(sdmxObject);
                    break;
                case "CODELIST":
                    FillCodelist(sdmxObject);
                    break;
                case "DATAFLOW":
                    FillDataflow(sdmxObject.Dataflows.First());
                    break;
            }

            EnableDownload();

        }


        private void ExpandToRoot(TreeNode node)
        {
            if (node != null)
            {
                node.Expand();
                if (node.Parent != null)
                {
                    ExpandToRoot(node.Parent);
                }
            }
        }

        private void EnableDownload()
        {
            string[] oValues;
            string[] oIdentity;
            string downloadArtName ="";

            oValues = tvCategory.SelectedNode.Value.Split('|');
            oIdentity = oValues[1].Split(',');

            switch (oValues[0].ToUpper())
            {
                case "DSD":
                    downloadArtName = "KeyFamily";
                    break;
                case "CONCEPTSCHEME":
                    downloadArtName = "ConceptScheme";
                    break;
                case "CODELIST":
                    downloadArtName = "CodeList";
                    break;
                case "DATAFLOW":
                    downloadArtName = "Dataflow";
                    break;
            }

            FileDownload3.Visible = true;
            FileDownload3.ucID = oIdentity[0];
            FileDownload3.ucAgency = oIdentity[1];
            FileDownload3.ucVersion = oIdentity[2];
            FileDownload3.ucArtefactType = downloadArtName;
            FileDownload3.ucVisible = true;

            FileDownload3.LoadLogic();
        }

        #endregion

        #region Methods

        private void ShowDialog(string text)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "ShowDialog('" + text + "')", true);
        }

        private void FillForm()
        {
            ISdmxObjects sdmxCS;

            try
            {
                sdmxCS = _wsModel.GetAllCategoryScheme(true);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Equals("no results found"))
                {
                    ShowDialog("No CategoryScheme Found!");
                    btnView.Enabled = false;
                }
                else
                    ShowDialog(ex.Message);
                return;
            }
            
            EntityMapper emCS = new EntityMapper();

            if (sdmxCS != null && sdmxCS.CategorySchemes.Count > 0)
            {
                foreach (ICategorySchemeObject cso in sdmxCS.CategorySchemes)
                {
                    cmbCategorySchemes.Items.Add(cso.Id + "," + cso.AgencyId + "," + cso.Version);
                }

                if (_artIdentity != null)
                    cmbCategorySchemes.SelectedValue = _artIdentity.ToString();
                else if (!_epe.CatViewEnableCategoryDropDownList)
                {
                    if (_epe.CatViewDefaultCategoryScheme != String.Empty)
                    {
                        string[] artIdentity = _epe.CatViewDefaultCategoryScheme.Split('|');
                        cmbCategorySchemes.SelectedValue = new ArtefactIdentity(artIdentity[0], artIdentity[1], artIdentity[2]).ToString();
                    }
                    cmbCategorySchemes.Enabled = false;
                }
            }

            chkCodelist.Visible = _epe.CatViewEnableCodelist;
            chkConcSchema.Visible = _epe.CatViewEnableConceptScheme;
            chkDataflow.Visible = _epe.CatViewEnableDataFlow;
            chkDSD.Visible = _epe.CatViewEnableDsd;

            string[] resources = Directory.GetFiles(Server.MapPath("~/App_GlobalResources"), "*.resx");

            Utils.PopulateCmbLanguages(cmbLanguage, AVAILABLE_MODES.MODE_FOR_GLOBAL_LOCALIZATION, resources);

        }

        private void StartSearching()
        {
            _lVisibleArtefacts = new List<string>();

            FillVisibleArtefactList(_lVisibleArtefacts);
            FillSDMXArtefacts();

            _sdmxCS = _wsModel.GetCategorySchemeWithParents(Utils.GetArtefactIdentityFromString(cmbCategorySchemes.SelectedValue), false, false);

            ICategorySchemeObject catSchema = _sdmxCS.CategorySchemes.FirstOrDefault();

            TreeNode tn = new TreeNode("[ " + catSchema.Id + " ] " + _localizedUtils.GetNameableName(catSchema));

            tn.ToolTip = string.Format("ID: {0}\n\rAgency: {1}\n\rVersion: {2}", catSchema.Id, catSchema.AgencyId, catSchema.Version);

            tn.SelectAction = TreeNodeSelectAction.None;
            FillTree(catSchema.Items, tn);

            tvCategory.Nodes.Clear();

            tvCategory.Nodes.Add(tn);
        }

        private void FillSDMXArtefacts()
        {
            _sdmxArtefacts = new SdmxObjectsImpl();

            ISdmxObjects sdmxAppo = new SdmxObjectsImpl();

            if (chkCodelist.Checked)
            {
                sdmxAppo = _wsModel.GetAllCodeLists(true);
                foreach (ICodelistObject cl in sdmxAppo.Codelists)
                    _sdmxArtefacts.AddCodelist(cl);
            }

            if (chkConcSchema.Checked)
            {
                sdmxAppo = _wsModel.GetAllConceptScheme(true);
                foreach (IConceptSchemeObject cs in sdmxAppo.ConceptSchemes)
                    _sdmxArtefacts.AddConceptScheme(cs);
            }

            if (chkDataflow.Checked)
            {
                sdmxAppo = _wsModel.GetAllDataFlow(true);
                foreach (IDataflowObject df in sdmxAppo.Dataflows)
                    _sdmxArtefacts.AddDataflow(df);
            }

            if (chkDSD.Checked)
            {
                sdmxAppo = _wsModel.GetAllDataStructure(true);
                foreach (IDataStructureObject dsd in sdmxAppo.DataStructures)
                    _sdmxArtefacts.AddDataStructure(dsd);
            }
        }

        private void FillTree(IList<ICategoryObject> catList, TreeNode tn)
        {
            foreach (ICategoryObject cat in catList)
            {
                TreeNode tChild = new TreeNode("[ " + cat.Id + " ] " + _localizedUtils.GetNameableName(cat));
                tChild.SelectAction = TreeNodeSelectAction.None;
                tn.ChildNodes.Add(tChild);

                FillArtefacts(tChild, cat);
                FillTree(cat.Items, tChild);

                if (tChild.ChildNodes.Count == 0 && String.IsNullOrEmpty(tChild.ImageUrl))
                    tn.ChildNodes.Remove(tChild);
 

            }
        }

        private void FillArtefacts(TreeNode tn, ICategoryObject cat)
        {
            var foundedCat = _sdmxCS.Categorisations.Where(c => c.CategoryReference.IdentifiableIds.LastOrDefault() == cat.Id);

            string artefactType;
            ISdmxObjects sdmxObject;
            ArtefactIdentity ai;
           
            foreach (ICategorisationObject catObj in foundedCat)
            {
                artefactType = catObj.StructureReference.MaintainableStructureEnumType.EnumType.ToString();

                if (_lVisibleArtefacts.Where(c => c.Contains(artefactType.ToUpper())).FirstOrDefault() != null)
                {
                    ai = new ArtefactIdentity(catObj.StructureReference.MaintainableId, catObj.StructureReference.AgencyId, catObj.StructureReference.Version);

                    sdmxObject = GetSdmxObjectArtefact(artefactType, ai);

                    TreeNode tnArt = new TreeNode(" " + GetArtefactName(sdmxObject));
                    tnArt.Text += String.Format(" [{0},{1},{2}]", ai.ID, ai.Agency, ai.Version);
                    tnArt.ToolTip = string.Format("ID: {0}\n\rAgency: {1}\n\rVersion: {2}", ai.ID, ai.Agency, ai.Version);
                    tnArt.Value = artefactType + "|" + ai.ToString();
                    tnArt.ImageUrl = @"./images/" + artefactType + ".png";
                    tn.ChildNodes.Add(tnArt);

                    ExpandToRoot(tnArt);

                }
            }
        }

        private ISdmxObjects GetSdmxObjectArtefact(string artefactType, ArtefactIdentity ai)
        {
            ISdmxObjects sdmxObject = new SdmxObjectsImpl();

            switch (artefactType.ToUpper())
            {
                case "DSD":
                    IDataStructureObject dsd;
                    dsd = _sdmxArtefacts.DataStructures.Where(ds => ds.AgencyId == ai.Agency && ds.Id == ai.ID && ds.Version == ai.Version).FirstOrDefault();
                    if (dsd != null)
                        sdmxObject.AddDataStructure(dsd);
                    break;
                case "CONCEPTSCHEME":
                    IConceptSchemeObject cs;
                    cs = _sdmxArtefacts.ConceptSchemes.Where(ds => ds.AgencyId == ai.Agency && ds.Id == ai.ID && ds.Version == ai.Version).FirstOrDefault();
                    if (cs != null)
                        sdmxObject.AddConceptScheme(cs);
                    break;
                case "CODELIST":
                    ICodelistObject cl;
                    cl = _sdmxArtefacts.Codelists.Where(ds => ds.AgencyId == ai.Agency && ds.Id == ai.ID && ds.Version == ai.Version).FirstOrDefault();
                    if (cl != null)
                        sdmxObject.AddCodelist(cl);
                    break;
                case "DATAFLOW":
                    IDataflowObject df;
                    df = _sdmxArtefacts.Dataflows.Where(ds => ds.AgencyId == ai.Agency && ds.Id == ai.ID && ds.Version == ai.Version).FirstOrDefault();
                    if (df != null)
                        sdmxObject.AddDataflow(df);
                    break;
            }

            return sdmxObject;
        }

        private string GetArtefactName(ISdmxObjects sdmxObject)
        {
            string name = string.Empty;

            if (sdmxObject.Codelists.Count > 0)
                name = _localizedUtils.GetNameableName(sdmxObject.Codelists.First());
            if (sdmxObject.ConceptSchemes.Count > 0)
                name = _localizedUtils.GetNameableName(sdmxObject.ConceptSchemes.First());
            if (sdmxObject.Dataflows.Count > 0)
                name = _localizedUtils.GetNameableName(sdmxObject.Dataflows.First());
            if (sdmxObject.DataStructures.Count > 0)
                name = _localizedUtils.GetNameableName(sdmxObject.DataStructures.First());

            return name == String.Empty ? "[no_name]" : name;
        }

        private void FillVisibleArtefactList(List<string> lVisibleArtefacts)
        {
            if (chkCodelist.Checked)
                lVisibleArtefacts.Add("CODELIST");
            if (chkConcSchema.Checked)
                lVisibleArtefacts.Add("CONCEPTSCHEME");
            if (chkDataflow.Checked)
                lVisibleArtefacts.Add("DATAFLOW");
            if (chkDSD.Checked)
                lVisibleArtefacts.Add("DSD");
        }

        private void FillDataflow(IDataflowObject dfObject)
        {
            FillGeneralDetail(dfObject);

            GridView gvDataflow = new GridView();
            gvDataflow.RowCreated += gvDataflow_RowCreated;
            gvDataflow.CssClass = "tableDetails";

            DataTable dt = new DataTable();

            for (int i = 0; i < 4; i++)
            {
                dt.Columns.Add("dt" + (i + 1));
            }

            dt.Rows.Add(dfObject.Id, dfObject.AgencyId, dfObject.Version, _localizedUtils.GetNameableName(dfObject));

            gvDataflow.DataSource = dt;
            gvDataflow.DataBind();

            pnlResult.Controls.Add(gvDataflow);
        }

        private void FillConceptScheme(ISdmxObjects sdmxObjects)
        {
            IConceptSchemeObject csObject = sdmxObjects.ConceptSchemes.First();

            FillGeneralDetail(csObject);

            GridView gvConceptScheme = new GridView();
            gvConceptScheme.RowCreated += gvConceptScheme_RowCreated;
            gvConceptScheme.CssClass = "tableDetails";

            DataTable dt = new DataTable();

            for (int i = 0; i < 2; i++)
            {
                dt.Columns.Add("dt" + (i + 1));
            }

            List<Concept> lCodes = _entityMapper.GetConceptList(sdmxObjects);

            foreach (Concept code in lCodes)
            {
                dt.Rows.Add(code.Name, code.Code);
            }

            gvConceptScheme.DataSource = dt;
            gvConceptScheme.DataBind();

            pnlResult.Controls.Add(gvConceptScheme);
        }

        private void FillCodelist(ISdmxObjects sdmxObjects)
        {
            ICodelistObject codelistObject = sdmxObjects.Codelists.First();

            FillGeneralDetail(codelistObject);

            GridView gvCodelist = new GridView();
            gvCodelist.RowCreated += gvCodelist_RowCreated;
            gvCodelist.CssClass = "tableDetails";

            DataTable dt = new DataTable();

            for (int i = 0; i < 3; i++)
            {
                dt.Columns.Add("dt" + (i + 1));
            }

            List<CodeItem> lCodes = _entityMapper.GetCodeItemList(sdmxObjects);

            foreach (CodeItem code in lCodes)
            {

                dt.Rows.Add(code.Name, code.Code, code.ParentCode);
            }

            gvCodelist.DataSource = dt;
            gvCodelist.DataBind();

            pnlResult.Controls.Add(gvCodelist);
        }

        private void FillDSD(ISdmxObjects sdmxObjects)
        {
            IDataStructureObject dsd = sdmxObjects.DataStructures.First();
            IConceptSchemeObject cs;

            FillGeneralDetail(dsd);

            GridView gvDimension = new GridView();
            gvDimension.RowCreated += gvDimension_RowCreated;
            gvDimension.CssClass = "tableDetails";

            GridView gvMeasures = new GridView();
            gvMeasures.RowCreated += gvMeasures_RowCreated;
            gvMeasures.CssClass = "tableDetails";

            GridView gvAttributes = new GridView();
            gvAttributes.RowCreated += gvAttributes_RowCreated;
            gvAttributes.CssClass = "tableDetails";

            pnlResult.Controls.Add(gvDimension);
            pnlResult.Controls.Add(gvMeasures);
            pnlResult.Controls.Add(gvAttributes);

            // ****** Dimension
            DataTable dt = new DataTable();

            for (int i = 0; i < 10; i++)
            {
                dt.Columns.Add("dt" + (i + 1));
            }

            //foreach (var dimension in dsd.DimensionList.Dimensions.Where(c => c.HasCodedRepresentation()))
            foreach (var dimension in dsd.DimensionList.Dimensions)
            {
                string TextFormat = String.Empty;

                if (!dimension.HasCodedRepresentation())
                {
                    TextFormat = "String";
                } else
                if (dimension.Representation.TextFormat != null)
                    TextFormat = dimension.Representation.TextFormat.TextType.EnumType.ToString();


                cs = sdmxObjects.ConceptSchemes.Where(ci => ci.Id == dimension.ConceptRef.MaintainableId
                                                    && ci.AgencyId == dimension.ConceptRef.AgencyId
                                                    && ci.Version == dimension.ConceptRef.Version).FirstOrDefault();


                if (dimension.HasCodedRepresentation())
                {
                    dt.Rows.Add(dimension.ConceptRef.FullId,
                                _localizedUtils.GetNameableName(cs),
                                cs.Id, cs.AgencyId, cs.Version,
                                dimension.Representation.Representation.MaintainableId,
                                dimension.Representation.Representation.Version,
                                dimension.Representation.Representation.AgencyId,
                                TextFormat,
                                _entityMapper.GetDimensionRole(dimension));
                }
                else
                {
                    dt.Rows.Add(dimension.ConceptRef.FullId,
                                _localizedUtils.GetNameableName(cs),
                                cs.Id, cs.AgencyId, cs.Version,
                                "","","",
                                TextFormat,
                                _entityMapper.GetDimensionRole(dimension));
                }
            }

            gvDimension.DataSource = dt;
            gvDimension.DataBind();

            // ****** Measures

            if (dsd.PrimaryMeasure != null)
            {
                dt = new DataTable();

                for (int i = 0; i < 12; i++)
                {
                    dt.Columns.Add("dt" + (i + 1));
                }

                ICrossReference conceptRef = dsd.PrimaryMeasure.ConceptRef;

                cs = sdmxObjects.ConceptSchemes.Where(ci => ci.Id == conceptRef.MaintainableId
                                                    && ci.AgencyId == conceptRef.AgencyId
                                                    && ci.Version == conceptRef.Version).FirstOrDefault();

                string clID = "", clVer = "", clAgency = "";

                if (dsd.PrimaryMeasure.Representation != null && dsd.PrimaryMeasure.Representation.Representation != null)
                {
                    ICrossReference cl = dsd.PrimaryMeasure.Representation.Representation;
                    clID = cl.MaintainableId;
                    clAgency = cl.AgencyId;
                    clVer = cl.Version;
                }

                dt.Rows.Add("Primary",
                            conceptRef.FullId,
                            _localizedUtils.GetNameableName(cs),
                            cs.Id, cs.AgencyId, cs.Version,
                            clID, clVer, clAgency,
                            "[textformat]",
                            "[measuredim]",
                            "[code]");

                gvMeasures.DataSource = dt;
                gvMeasures.DataBind();
            }

            // ****** Attributes
            if (dsd.AttributeList != null)
            {
                dt = new DataTable();

                for (int i = 0; i < 12; i++)
                {
                    dt.Columns.Add("dt" + (i + 1));
                }

                foreach (IAttributeObject attribute in dsd.AttributeList.Attributes)
                {
                    string TextFormat = String.Empty;
                    string CodeList = String.Empty;
                    string clID = "", clVer = "", clAgency = "";

                    if (attribute.Representation != null && attribute.Representation.Representation != null)
                    {
                        ICrossReference cl = attribute.Representation.Representation;
                        clID = cl.MaintainableId;
                        clAgency = cl.AgencyId;
                        clVer = cl.Version;
                        if (attribute.Representation.TextFormat != null)
                            TextFormat = attribute.Representation.TextFormat.TextType.EnumType.ToString();

                    }

                    cs = sdmxObjects.ConceptSchemes.Where(ci => ci.Id == attribute.ConceptRef.MaintainableId
                                    && ci.AgencyId == attribute.ConceptRef.AgencyId
                                    && ci.Version == attribute.ConceptRef.Version).FirstOrDefault();

                    dt.Rows.Add(
                        attribute.AttachmentLevel.ToString(),
                            attribute.ConceptRef.FullId,
                            _localizedUtils.GetNameableName(cs),
                            cs.Id, cs.AgencyId, cs.Version,
                        clID, clVer, clAgency,
                        TextFormat,
                        "[attribute_type]",
                        attribute.AssignmentStatus
                        );
                }


                gvAttributes.DataSource = dt;
                gvAttributes.DataBind();

            }

        }

        private void FillGeneralDetail(IMaintainableObject maintObject)
        {
            GridView gvGeneral = new GridView();
            gvGeneral.RowCreated += gvGeneral_RowCreated;
            gvGeneral.CssClass = "tableDetails";

            pnlResult.Controls.Add(gvGeneral);

            DataTable dt = new DataTable();

            for (int i = 0; i < 4; i++)
            {
                dt.Columns.Add();
            }

            dt.Rows.Add(_localizedUtils.GetNameableName(maintObject), maintObject.Id, maintObject.AgencyId, maintObject.Version);
            if (_localizedUtils.GetNameableDescription(maintObject) != string.Empty)
                dt.Rows.Add(_localizedUtils.GetNameableDescription(maintObject));

            gvGeneral.DataSource = dt;
            gvGeneral.DataBind();

        }

        private ISdmxObjects GetSdmxObjectArtefact(string artefactType, ArtefactIdentity ai, bool stub)
        {
            ISdmxObjects sdmxObject = null;

            switch (artefactType.ToUpper())
            {
                case "DSD":
                    if (stub)
                        sdmxObject = _wsModel.GetDataStructure(ai, stub, false);
                    else
                        sdmxObject = GetStructureFullWithStubChilds(ai);  //_wsModel.GetDataStructureWithRef(ai, true, false);
                    break;
                case "CONCEPTSCHEME":
                    sdmxObject = _wsModel.GetConceptScheme(ai, stub, false);
                    break;
                case "CODELIST":
                    sdmxObject = _wsModel.GetCodeList(ai, stub, false);
                    break;
                case "DATAFLOW":
                    sdmxObject = _wsModel.GetDataFlow(ai, stub, false);
                    break;
            }

            return sdmxObject;
        }

        private ISdmxObjects GetStructureFullWithStubChilds(ArtefactIdentity ai)
        {
            ISdmxObjects sdmxObjectFull = null, sdmxObjectDSD = null;

            sdmxObjectFull = _wsModel.GetDataStructureWithRef(ai, true, false);
            sdmxObjectFull.RemoveDataStructure(sdmxObjectFull.DataStructures.First());

            sdmxObjectDSD = _wsModel.GetDataStructure(ai, false, false);
            sdmxObjectFull.AddDataStructure(sdmxObjectDSD.DataStructures.First());

            return sdmxObjectFull;
        }

        #endregion

        #region Table Headers

        #region GENERAL

        protected void gvGeneral_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddDSDHeaderRow((GridView)sender);
            }
        }

        private void AddDSDHeaderRow(GridView gv)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();
            TableHeaderCell thc4 = new TableHeaderCell();

            string ArtefactName = "";

            string[] oValues;

            oValues = tvCategory.SelectedNode.Value.Split('|');

            switch (oValues[0].ToUpper())
            {
                case "DSD":
                    ArtefactName = Resources.Messages.lbl_ab_Dsdef;
                    break;
                case "CONCEPTSCHEME":
                    ArtefactName = Resources.Messages.lbl_ab_ConceptSchema;
                    break;
                case "CODELIST":
                    ArtefactName = Resources.Messages.lbl_ab_Codelist;
                    break;
                case "DATAFLOW":
                    ArtefactName = Resources.Messages.lbl_ab_Dataflow;
                    break;
            }

            thc1.Text = ArtefactName;
            thc1.ColumnSpan = 4;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gv.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_NameDescr;
            thc2.Text = Resources.Messages.lbl_ab_ID;
            thc3.Text = Resources.Messages.lbl_ab_Version;
            thc4.Text = Resources.Messages.lbl_ab_Agenzia;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4 });

            gv.Controls[0].Controls.AddAt(1, gr);
        }

        #endregion

        #region Dimension

        protected void gvDimension_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddDimensionHeaderRow((GridView)sender);
            }

        }

        private void AddDimensionHeaderRow(GridView gvDimension)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();
            TableHeaderCell thc4 = new TableHeaderCell();
            TableHeaderCell thc5 = new TableHeaderCell();
            TableHeaderCell thc6 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Dimensioni;
            thc1.ColumnSpan = 10;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gvDimension.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Concetti;
            thc1.ColumnSpan = 5;

            thc2.Text = Resources.Messages.lbl_ab_Representation;
            thc2.ColumnSpan = 4;

            thc3.Text = Resources.Messages.lbl_ab_DimType;
            thc3.RowSpan = 3;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3 });

            gvDimension.Controls[0].Controls.AddAt(1, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();
            thc2 = new TableHeaderCell();
            thc3 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc1.RowSpan = 2;

            thc2.Text = Resources.Messages.lbl_ab_Name;
            thc2.RowSpan = 2;

            thc3.Text = Resources.Messages.lbl_ab_ConceptSchema;
            thc3.ColumnSpan = 3;

            thc4.Text = Resources.Messages.lbl_ab_Codelist;
            thc4.ColumnSpan = 3;

            thc5.Text = Resources.Messages.lbl_ab_TextFormat;
            thc5.RowSpan = 2;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5 });

            gvDimension.Controls[0].Controls.AddAt(2, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();
            thc2 = new TableHeaderCell();
            thc3 = new TableHeaderCell();
            thc4 = new TableHeaderCell();
            thc5 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc2.Text = Resources.Messages.lbl_ab_Version;
            thc3.Text = Resources.Messages.lbl_ab_Agenzia;
            thc4.Text = Resources.Messages.lbl_ab_ID;
            thc5.Text = Resources.Messages.lbl_ab_Version;
            thc6.Text = Resources.Messages.lbl_ab_Agenzia;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5, thc6 });

            gvDimension.Controls[0].Controls.AddAt(3, gr);

        }

        #endregion

        #region Measures

        protected void gvMeasures_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddMeasuresHeaderRow((GridView)sender);
            }

        }

        private void AddMeasuresHeaderRow(GridView gvMeasures)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();
            TableHeaderCell thc4 = new TableHeaderCell();
            TableHeaderCell thc5 = new TableHeaderCell();
            TableHeaderCell thc6 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Measures;
            thc1.ColumnSpan = 12;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gvMeasures.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Type;
            thc1.RowSpan = 3;

            thc2.Text = Resources.Messages.lbl_ab_Concetti;
            thc2.ColumnSpan = 5;

            thc3.Text = Resources.Messages.lbl_ab_Representation;
            thc3.ColumnSpan = 4;

            thc4.Text = Resources.Messages.lbl_ab_MeasureDimension;
            thc4.RowSpan = 3;

            thc5.Text = Resources.Messages.lbl_ab_Code;
            thc5.RowSpan = 3;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5 });

            gvMeasures.Controls[0].Controls.AddAt(1, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();
            thc2 = new TableHeaderCell();
            thc3 = new TableHeaderCell();
            thc4 = new TableHeaderCell();
            thc5 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc1.RowSpan = 2;

            thc2.Text = Resources.Messages.lbl_ab_Name;
            thc2.RowSpan = 2;

            thc3.Text = Resources.Messages.lbl_ab_ConceptSchema;
            thc3.ColumnSpan = 3;

            thc4.Text = Resources.Messages.lbl_ab_Codelist;
            thc4.ColumnSpan = 3;

            thc5.Text = Resources.Messages.lbl_ab_TextFormat;
            thc5.RowSpan = 2;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5 });

            gvMeasures.Controls[0].Controls.AddAt(2, gr);

            /////////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();
            thc2 = new TableHeaderCell();
            thc3 = new TableHeaderCell();
            thc4 = new TableHeaderCell();
            thc5 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc2.Text = Resources.Messages.lbl_ab_Version;
            thc3.Text = Resources.Messages.lbl_ab_Agenzia;
            thc4.Text = Resources.Messages.lbl_ab_ID;
            thc5.Text = Resources.Messages.lbl_ab_Version;
            thc6.Text = Resources.Messages.lbl_ab_Agenzia;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5, thc6 });

            gvMeasures.Controls[0].Controls.AddAt(3, gr);
        }

        #endregion

        #region Attributes

        protected void gvAttributes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddAttributesHeaderRow((GridView)sender);
            }

        }

        private void AddAttributesHeaderRow(GridView gvAttributes)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();
            TableHeaderCell thc4 = new TableHeaderCell();
            TableHeaderCell thc5 = new TableHeaderCell();
            TableHeaderCell thc6 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Attributi;
            thc1.ColumnSpan = 12;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gvAttributes.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_AttachLevel;
            thc1.RowSpan = 3;

            thc2.Text = Resources.Messages.lbl_ab_Concetti;
            thc2.ColumnSpan = 5;

            thc3.Text = Resources.Messages.lbl_ab_Representation;
            thc3.ColumnSpan = 4;

            thc4.Text = Resources.Messages.lbl_ab_AttributeType;
            thc4.RowSpan = 3;

            thc5.Text = Resources.Messages.lbl_ab_assi_status;
            thc5.RowSpan = 3;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5 });

            gvAttributes.Controls[0].Controls.AddAt(1, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();
            thc2 = new TableHeaderCell();
            thc3 = new TableHeaderCell();
            thc4 = new TableHeaderCell();
            thc5 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc1.RowSpan = 2;

            thc2.Text = Resources.Messages.lbl_ab_Name;
            thc2.RowSpan = 2;

            thc3.Text = Resources.Messages.lbl_ab_ConceptSchema;
            thc3.ColumnSpan = 3;

            thc4.Text = Resources.Messages.lbl_ab_Codelist;
            thc4.ColumnSpan = 3;

            thc5.Text = Resources.Messages.lbl_ab_TextFormat;
            thc5.RowSpan = 2;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5 });

            gvAttributes.Controls[0].Controls.AddAt(2, gr);

            /////////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();
            thc2 = new TableHeaderCell();
            thc3 = new TableHeaderCell();
            thc4 = new TableHeaderCell();
            thc5 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc2.Text = Resources.Messages.lbl_ab_Version;
            thc3.Text = Resources.Messages.lbl_ab_Agenzia;
            thc4.Text = Resources.Messages.lbl_ab_ID;
            thc5.Text = Resources.Messages.lbl_ab_Version;
            thc6.Text = Resources.Messages.lbl_ab_Agenzia;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4, thc5, thc6 });

            gvAttributes.Controls[0].Controls.AddAt(3, gr);
        }

        #endregion

        #region Codelist

        protected void gvCodelist_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddCodelistRow((GridView)sender);
            }
        }

        private void AddCodelistRow(GridView gv)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Codes;
            thc1.ColumnSpan = 3;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gv.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Name;
            thc2.Text = Resources.Messages.lbl_ab_Code;
            thc3.Text = Resources.Messages.lbl_ab_Parent;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3 });

            gv.Controls[0].Controls.AddAt(1, gr);
        }

        #endregion

        #region ConceptScheme

        protected void gvConceptScheme_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddConceptSchemeHaderRow((GridView)sender);
            }
        }

        private void AddConceptSchemeHaderRow(GridView gv)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Concetti;
            thc1.ColumnSpan = 2;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gv.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Name;
            thc2.Text = Resources.Messages.lbl_ab_ID;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2 });

            gv.Controls[0].Controls.AddAt(1, gr);
        }

        #endregion

        #region Dataflow

        protected void gvDataflow_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Visible = false;

                AddDataflowHeaderRow((GridView)sender);
            }
        }

        private void AddDataflowHeaderRow(GridView gv)
        {
            GridViewRow gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();
            TableHeaderCell thc4 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_Dsdef;
            thc1.ColumnSpan = 4;

            gr.Cells.AddRange(new TableCell[] { thc1 });

            gv.Controls[0].Controls.AddAt(0, gr);

            //////

            gr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

            thc1 = new TableHeaderCell();

            thc1.Text = Resources.Messages.lbl_ab_ID;
            thc2.Text = Resources.Messages.lbl_ab_Agenzia;
            thc3.Text = Resources.Messages.lbl_ab_Version;
            thc4.Text = Resources.Messages.lbl_ab_Name;

            gr.Cells.AddRange(new TableCell[] { thc1, thc2, thc3, thc4 });

            gv.Controls[0].Controls.AddAt(1, gr);
        }

        #endregion

        #endregion
    }
}