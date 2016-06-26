using ISTAT.Entity;
using ISTAT.EntityMapper;
using ISTAT.WSDAL;
using ISTATRegistry.IRServiceReference;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.UserControls
{
    public partial class Categorisations : System.Web.UI.UserControl
    {
        public AvailableStructures ucArtefactType { get; set; }
        public ArtefactIdentity ucArtIdentity { get; set; }

        private WSModel _wsModel = new WSModel();
        private EntityMapper _eMapper = new EntityMapper(Utils.LocalizedLanguage);
        private LocalizedUtils _localizedUtils;
        private Action _action;

        protected void Page_Load(object sender, EventArgs e)
        {
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);

            SetAction();

            if(!IsPostBack)
            {
                if(HttpContext.Current.Request.QueryString["OCT"] != null)
                    Utils.AppendScript("location.href='#categorisation';");
            }

            switch (_action)
            {
                case Action.INSERT:
                    pnlAll.Visible = false;
                    pnlArtefactMustBeSaved.Visible = true;
                    break;
                case Action.UPDATE:
                    pnlAll.Visible = true;
                    pnlArtefactMustBeSaved.Visible = false;
                    btnAddCategorisation.Visible = true;
                    btnSaveCategorisation.Visible = true;
                    break;
                case Action.VIEW:
                    pnlAll.Visible = true;
                    pnlArtefactMustBeSaved.Visible = false;
                    btnAddCategorisation.Visible = false;
                    btnSaveCategorisation.Visible = false;

                    if (gvCategorisations.Columns != null || gvCategorisations.Columns.Count > 0)
                        gvCategorisations.Columns[5].Visible = false;
                    break;
                default:
                    break;
            }

            if (cmbCategorySchemes.Items.Count <=0)
            {
                PopolateCmbCategoryScheme();
                PopolateGVCategorisations();
            }
        }

        private void SetAction()
        {
            if (Utils.ViewMode)
                _action = Action.VIEW;
            else if (ucArtIdentity == null)
                _action = Action.INSERT;
            else
            {
                IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;
                int agencyOccurence = currentUser.agencies.Count(agency => agency.id.Equals(ucArtIdentity.Agency));
                if (agencyOccurence > 0)
                    _action = Action.UPDATE;
                else
                    _action = Action.VIEW;
            }
        }


        private void PopolateGVCategorisations()
        {
            // Prendere solo le Categorisation legate all'artefatto
            if (ucArtIdentity == null || ucArtIdentity.ID == "")
                return;

            ISdmxObjects sdmxCats;

            try
            {
                sdmxCats = _wsModel.GetAllCategorisation(false);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No Results Found")
                    return;
                else
                    throw ex;
            }

            if (sdmxCats.Categorisations == null || sdmxCats.Categorisations.Count <= 0)
                return;

            ISdmxObjects sdmxFiltered = new SdmxObjectsImpl();

            SdmxStructureType st = GetStructureType();

            IEnumerable<ICategorisationObject> lCats = sdmxCats.Categorisations.Where(c => c.StructureReference.MaintainableStructureEnumType == st
                                                            && c.StructureReference.MaintainableId == ucArtIdentity.ID
                                                            && c.StructureReference.AgencyId == ucArtIdentity.Agency
                                                            && c.StructureReference.Version == ucArtIdentity.Version);

            foreach (var cat in lCats)
                sdmxFiltered.AddCategorisation(cat);

            if (sdmxFiltered.Categorisations == null || sdmxFiltered.Categorisations.Count <= 0)
                return;

            List<ISTAT.Entity.Categorization> lCategorisation = _eMapper.GetCategorizationList(sdmxFiltered);

            gvCategorisations.PageSize = Utils.GeneralCategorizationGridNumberRow;

            //            lblNumberOfTotalElements.Text = string.Format(Resources.Messages.lbl_number_of_total_rows, lCategorization.Count.ToString());
            gvCategorisations.DataSource = lCategorisation;
            gvCategorisations.DataBind();

            //if (lCategorisation.Count == 0)
            //{
            //    txtNumberOfRows.Visible = false;
            //    lblNumberOfRows.Visible = false;
            //    btnChangePaging.Visible = false;
            //}
            //else
            //{
            //    txtNumberOfRows.Visible = true;
            //    lblNumberOfRows.Visible = true;
            //    btnChangePaging.Visible = true;
            //}
        }

        private SdmxStructureType GetStructureType()
        {
            SdmxStructureType sType = null;

            switch (ucArtefactType)
            {
                case AvailableStructures.CODELIST:
                    sType = SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList);
                    break;
                case AvailableStructures.DATAFLOW:
                    sType = SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow);
                    break;
                case AvailableStructures.KEY_FAMILY:
                    sType = SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd);
                    break;
                case AvailableStructures.CONCEPT_SCHEME:
                    sType = SdmxStructureType.GetFromEnum(SdmxStructureEnumType.ConceptScheme);
                    break;
            }

            return sType;
        }

        private void PopolateCmbCategoryScheme()
        {
            ISdmxObjects sdmxCats;

            try
            {
                sdmxCats = _wsModel.GetAllCategoryScheme(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No Results Found")
                    return;
                else
                    throw ex;
            }

            if (sdmxCats.CategorySchemes == null || sdmxCats.CategorySchemes.Count <= 0)
                return;

            cmbCategorySchemes.Items.Add("");

            foreach (var cat in sdmxCats.CategorySchemes)
            {
                if (cat.IsFinal.IsTrue)
                    cmbCategorySchemes.Items.Add(string.Format("{0},{1},{2}", cat.Id, cat.AgencyId, cat.Version));
            }
        }

        private void StartSearching()
        {
            ISdmxObjects sdmxCS = _wsModel.GetCategoryScheme(Utils.GetArtefactIdentityFromString(cmbCategorySchemes.SelectedValue), false, false);

            ICategorySchemeObject catSchema = sdmxCS.CategorySchemes.FirstOrDefault();

            TreeNode tn = new TreeNode("[ " + catSchema.Id + " ] " + _localizedUtils.GetNameableName(catSchema));

            tn.ToolTip = string.Format("ID: {0}\n\rAgency: {1}\n\rVersion: {2}", catSchema.Id, catSchema.AgencyId, catSchema.Version);


            tn.SelectAction = TreeNodeSelectAction.None;
            FillTree(catSchema.Items, tn);

            tvCategory.Nodes.Clear();

            tvCategory.Nodes.Add(tn);
        }

        private void FillTree(IList<ICategoryObject> catList, TreeNode tn)
        {
            foreach (ICategoryObject cat in catList)
            {
                TreeNode tChild = new TreeNode("[ " + cat.Id + " ] " + _localizedUtils.GetNameableName(cat));
                tChild.Value = cat.Id;
                //tChild.SelectAction = TreeNodeSelectAction;
                tn.ChildNodes.Add(tChild);

                FillTree(cat.Items, tChild);
            }
        }

        protected void cmbCategorySchemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartSearching();
            if (cmbCategorySchemes.Items[0].Value == "")
                cmbCategorySchemes.Items.Remove(cmbCategorySchemes.Items[0]);

            btnSaveCategorisation.Enabled = false;
        }

        protected void btnSaveCategorisation_Click(object sender, EventArgs e)
        {
            // Controllo su selezione categoria
            if (String.IsNullOrEmpty(tvCategory.SelectedValue))
                return;

            // Creo la Categorisation e la salvo

            // Creazione delle info di base della categorisation
            // Recupero della category selezionata

            string catsID = cmbCategorySchemes.SelectedValue.Replace(',', '_').Replace(".", "") + "_" +
                                        tvCategory.SelectedValue + "-" +
                                        ucArtIdentity.ID + "_" +
                                        ucArtIdentity.Agency + "_" +
                                        ucArtIdentity.Version.Replace(".", "");

            ICategorisationMutableObject categorisation = new CategorisationMutableCore();
            categorisation.Id = catsID;
            categorisation.AgencyId = ucArtIdentity.Agency;
            categorisation.Version = ucArtIdentity.Version;
            categorisation.AddName("en", "Categorisation " + ucArtIdentity.ToString());

            IStructureReference structureRef = new StructureReferenceImpl(ucArtIdentity.Agency,
                                                                            ucArtIdentity.ID,
                                                                            ucArtIdentity.Version,
                                                                            GetStructureType());
            categorisation.StructureReference = structureRef;

            ArtefactIdentity artIDCS = Utils.GetArtefactIdentityFromString(cmbCategorySchemes.SelectedValue);

            IStructureReference categoryRef = new StructureReferenceImpl(artIDCS.Agency,
                                                                        artIDCS.ID,
                                                                        artIDCS.Version,
                                                                        SdmxStructureEnumType.Category,
                                                                        tvCategory.SelectedValue);
            categorisation.CategoryReference = categoryRef;

            ISdmxObjects sdmxCategorisationInsert = new SdmxObjectsImpl();

            sdmxCategorisationInsert.AddCategorisation(categorisation.ImmutableInstance);

            _wsModel.SubmitStructure(sdmxCategorisationInsert);

            // Controllo su errore

            // Messaggio di inserimento avvenuto con successo

            // Refresh della Gridview
            PopolateGVCategorisations();
            ResetCatPanel();

        }

        private void ResetCatPanel()
        {
            pnlAdd.Visible = false;
            cmbCategorySchemes.Items.Insert(0, "");
            cmbCategorySchemes.SelectedIndex = 0;
            btnSaveCategorisation.Enabled = false;
            tvCategory.Nodes.Clear();
        }

        protected void btnAddCategorisation_Click(object sender, ImageClickEventArgs e)
        {
            pnlAdd.Visible = true;
        }

        protected void tvCategory_SelectedNodeChanged(object sender, EventArgs e)
        {
            btnSaveCategorisation.Enabled = true;
        }
    }
}


