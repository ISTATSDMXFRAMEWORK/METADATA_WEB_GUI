using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISTAT.EntityMapper;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using ISTAT.WSDAL;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using ISTATUtils;
using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;

namespace ISTATRegistry.UserControls
{
    public partial class GetConcept : System.Web.UI.UserControl
    {
        #region Public Props

        /// <summary>
        /// Il controllo target che verrà valorizzato con il concept
        /// </summary>
        public ITextControl TargetWebControl { get; set; }

        /// <summary>
        /// Il nome del Tab Jquery da riaprire dopo il postback
        /// </summary>
        public String ucOpenTabName { get; set; }

        /// <summary>
        /// Il nome della PopUp Jquery da riaprire dopo il postback
        /// </summary>
        public String ucOpenPopUpName { get; set; }

        /// <summary>
        /// La larghezza della PopUp Jquery da riaprire dopo il postback
        /// </summary>
        public int ucOpenPopUpWidth { get; set; }

        /// <summary>
        /// Valorizzato con true permette la selezione del Concept, con false quella del ConceptScheme
        /// </summary>
        public bool IsConceptSelection { get; set; }

        /// <summary>
        /// Se valorizzato a True vengono restituiti solo gli artefatti Final. 
        /// Valorizzato con false vengono restituiti tutti.
        /// Default = true
        /// </summary>
        public bool ucIsFinalArtefact = true;


        public TextBox TargetWebControlID { get; set; }

        #endregion

        #region Private Props

        protected const int PopUpWidth = 780;
        private ISdmxObjects _sdmxObjects;
        private Button BtnSearch;

        /// <summary>
        /// Rende univoco il controllo nel form
        /// </summary>
        protected String ControlID { get { return ClientID; } }


        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            SearchBar1.FixedSearch = true;
            SearchBar1.SearchBarID = "CS";
            BtnSearch = SearchBar1.BtnSearch;
            BtnSearch.Click += new EventHandler(BtnSearch_Click);
            lbBack.DataBind();
            SearchBar1.DataBind();
            lblTitle.DataBind();

            if (TargetWebControl != null && TargetWebControl.Text != "")
                btnDeleteText.Visible = true;
            else
                btnDeleteText.Visible = false;

            try
            {
                _sdmxObjects = GetSdmxObjects();

                if (!IsPostBack)
                {
                    BindData();
                }
            }
            catch (Exception ex)
            {
                //Utils.ShowDialog(ex.Message);
            }

        }

        protected void btnDeleteText_Click(object sender, ImageClickEventArgs e)
        {
            TargetWebControl.Text = "";
            btnDeleteText.Visible = false;
            ExecuteJSPostback();
        }

        protected void gvConceptScheme_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvConceptScheme.PageIndex = e.NewPageIndex;
            BindData();
            ExecuteJSPostback();
            OpenPopUp();
        }

        void BtnSearch_Click(object sender, EventArgs e)
        {
            if (_sdmxObjects == null)
                return;
            BindData();
            ExecuteJSPostback();
            OpenPopUp();
        }

        protected void btnSearchConcept_Click(object sender, EventArgs e)
        {
            _sdmxObjects = GetSdmxObjects();
            BindData();

            OpenPopUp();
            ExecuteJSPostback();
            Utils.ForceBlackClosing();


            //if (_sdmxObjects == null)
            //    return;
            //BindData();
            //ExecuteJSPostback();
            //OpenPopUp();
        }
        

        protected void gvConceptScheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = (gvConceptScheme.SelectedRow.FindControl("lblID") as Label).Text;
            string Agency = (gvConceptScheme.SelectedRow.FindControl("lblAgency") as Label).Text;
            string Version = (gvConceptScheme.SelectedRow.FindControl("lblVersion") as Label).Text;

            if (!IsConceptSelection)
            {
                TargetWebControl.Text = ID + "," + Agency + "," + Version;
                if (TargetWebControlID != null && TargetWebControlID.Text.Trim() == "")
                    TargetWebControlID.Text = ID;
                
                ViewBtnDelete();
                ExecuteJSPostback();
                Utils.ForceBlackClosing();
                return;
            }

            SearchBar1.Visible = false;
            gvConceptScheme.Visible = false;
            gvConcepts.Visible = true;
            pnlSearchConcept.Visible = true;

            lblTitle.Text = "Selected ConceptScheme: " + ID + " - " + Agency + " - " + Version;

            SearchBar1.ucID = ID;
            SearchBar1.ucAgency = Agency;
            SearchBar1.ucVersion = Version;
            SearchBar1.ucName = "";

            hdnViewMode.Value = "Concept";

            _sdmxObjects = GetSdmxObjects();
            BindData();

            OpenPopUp();
            ExecuteJSPostback();
            Utils.ForceBlackClosing();
        }

        protected void gvConcepts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = SearchBar1.ucID;
            string Agency = SearchBar1.ucAgency;
            string Version = SearchBar1.ucVersion;
            string ConceptID = (gvConcepts.SelectedRow.FindControl("lblConceptID") as Label).Text;

            TargetWebControl.Text = ID + "," + Agency + "," + Version + " - " + ConceptID;
            if (TargetWebControlID != null && TargetWebControlID.Text.Trim() == "")
                TargetWebControlID.Text = ConceptID;

            ViewBtnDelete();
            ResetControl();
            Utils.ForceBlackClosing();
            ExecuteJSPostback();
        }

        protected void gvConceptScheme_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    break;
            }
        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            ResetControl();

            _sdmxObjects = GetSdmxObjects();
            BindData();

            ExecuteJSPostback();
            OpenPopUp();
        }

        protected void gvConcepts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvConcepts.PageSize = 35;
            gvConcepts.PageIndex = e.NewPageIndex;
            BindData();

            ExecuteJSPostback();
            OpenPopUp();
        }

        #endregion

        #region Methods

        private void ViewBtnDelete()
        {
            btnDeleteText.Visible = true;
        }

        private ISdmxObjects GetSdmxObjects()
        {
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput;
            ISdmxObjects sdmxFinal;
            IMutableObjects mutableObj = new MutableObjectsImpl();
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            bool stub = true;

            sdmxFinal = new SdmxObjectsImpl();

            if (hdnViewMode.Value == "Concept")
                stub = false;

            try
            {
                sdmxInput = wsModel.GetConceptScheme(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion, ucIsFinalArtefact), stub, true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {

                    foreach (IConceptSchemeObject cs in sdmxInput.ConceptSchemes)
                    {
                        if (localizedUtils.GetNameableName(cs).Contains(SearchBar1.ucName.Trim()))
                            mutableObj.AddConceptScheme(cs.MutableInstance);

                    }
                    sdmxFinal = mutableObj.ImmutableObjects;

                }
                else
                    sdmxFinal = sdmxInput;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sdmxFinal;
        }

        private void BindData()
        {
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);

            if (hdnViewMode.Value == "Concept")
            {
                List<ISTAT.Entity.Concept> lItem = eMapper.GetConceptList(_sdmxObjects);
                List<ISTAT.Entity.Concept> lFilteredItem;

                if (txtSearchID.Text.Trim() != String.Empty || txtSearchName.Text.Trim() != String.Empty)
                {
                    lFilteredItem = lItem.Where(i => i.Code.ToUpper().Contains(txtSearchID.Text.Trim().ToUpper()) || txtSearchID.Text.Trim() == String.Empty).ToList();
                    lFilteredItem = lFilteredItem.Where(i => i.Name.ToUpper().Contains(txtSearchName.Text.Trim().ToUpper()) || txtSearchName.Text.Trim() == String.Empty).ToList();
                }
                else
                    lFilteredItem = lItem;

                gvConcepts.PageSize = 35;
                gvConcepts.DataSourceID = null;
                gvConcepts.DataSource = lFilteredItem;
                gvConcepts.DataBind();
            }
            else
            {
                List<ISTAT.Entity.ConceptScheme> lConcept = eMapper.GetConceptSchemeList(_sdmxObjects);

                gvConceptScheme.PageSize = 12;
                gvConceptScheme.DataSourceID = null;
                gvConceptScheme.DataSource = lConcept;
                gvConceptScheme.DataBind();
            }
        }

        private void ResetControl()
        {
            SearchBar1.Visible = true;
            gvConceptScheme.Visible = true;
            gvConcepts.Visible = false;
            pnlSearchConcept.Visible = false;

            lblTitle.Text = "Select Concept";

            SearchBar1.ucID = "";
            SearchBar1.ucAgency = "";
            SearchBar1.ucVersion = "";
            SearchBar1.ucName = "";

            hdnViewMode.Value = "";
        }

        private void OpenPopUp()
        {
            Utils.AppendScript("openP('df_Concept" + ControlID + "'," + PopUpWidth.ToString() + ");");
        }


        private void ExecuteJSPostback()
        {
            OpenTab();
            OpenUCPopUp();
        }

        private void OpenTab()
        {
            if (!String.IsNullOrEmpty(ucOpenTabName))
                Utils.AppendScript("location.href='#" + ucOpenTabName + "';");
        }

        private void OpenUCPopUp()
        {
            if (!String.IsNullOrEmpty(ucOpenPopUpName))
                Utils.AppendScript("openP('" + ucOpenPopUpName + "'," + ucOpenPopUpWidth.ToString() + ");");
        }

        #endregion

    }
}