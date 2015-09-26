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
using System.Data;

namespace ISTATRegistry.UserControls
{
    public class GetCodeListEventArgs : System.EventArgs
    {
        public ArtefactIdentity GetCodeListArtefactIdentity { get; set; }
        public DataTable GetCLDataTable { get; set; }
    }

    public partial class GetCodeList : System.Web.UI.UserControl
    {
        #region Public Props

        /// <summary>
        /// Il controllo target che verrà valorizzato con la Codelist
        /// </summary>
        public ITextControl TargetWebControl { get; set; }

        /// <summary>
        /// Il controllo DataTable target che verrà valorizzato con le Codelist
        /// </summary>
        public DataTable ucTargetDataTable { get; set; }

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
        /// Il tipo di icona da visualizzare
        /// </summary>
        public AddIconType ucAddIconType { get; set; }

        /// <summary>
        /// Se valorizzato a True vengono restituiti solo gli artefatti Final. 
        /// Valorizzato con false vengono restituiti tutti.
        /// Default = true
        /// </summary>
        public bool ucIsFinalArtefact = true;

        #endregion

        #region Public Events

        public event EventHandler<GetCodeListEventArgs> ucCLSelectedEH;

        public void ucCLSelected(object sender, DataTable dt)
        {
            if (ucCLSelectedEH != null)
                ucCLSelectedEH(sender, new GetCodeListEventArgs() { GetCLDataTable = dt });
        }

        #endregion

        #region Public Events

        public event EventHandler<GetCodeListEventArgs> ucCodeListSelectedEH;

        public void ucCodeListSelected(object sender, ArtefactIdentity ai)
        {
            if (ucCodeListSelectedEH != null)
                ucCodeListSelectedEH(sender, new GetCodeListEventArgs() { GetCodeListArtefactIdentity = ai });
        }

        #endregion

        #region Private Props

        protected string IconFileName;
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
            BtnSearch = SearchBar1.BtnSearch;
            BtnSearch.Click += new EventHandler(BtnSearch_Click);
            SearchBar1.FixedSearch = true;
            SearchBar1.SearchBarID = "CL";

            SearchBar1.DataBind();
            lblTitle.DataBind();

            switch (ucAddIconType)
            {
                case AddIconType.pencil:
                    IconFileName = "GetObject.png";
                    break;
                case AddIconType.cross:
                    IconFileName = "Add.png";
                    break;
            }

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
                Utils.ShowDialog(ex.Message);
            }
        }

        protected void gvCodelists_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            gvCodelists.PageIndex = e.NewPageIndex;
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

        protected void gvCodelists_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    break;
            }
        }

        protected void gvCodelists_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = (gvCodelists.SelectedRow.FindControl("lblID") as Label).Text;
            string Agency = (gvCodelists.SelectedRow.FindControl("lblAgency") as Label).Text;
            string Version = (gvCodelists.SelectedRow.FindControl("lblVersion") as Label).Text;

            if (TargetWebControl != null)
            {
                TargetWebControl.Text = ID + "," + Agency + "," + Version;

                try
                {
                    ucCodeListSelected(this, new ArtefactIdentity(ID, Agency, Version));
                }
                catch (ConstraintException ce)
                {
                    ucCodeListSelected(this, null);
                }
            }

            if (ucTargetDataTable != null)
            {
                DataRow dr = ucTargetDataTable.NewRow();
                dr["ID"] = ID;
                dr["Agency"] = Agency;
                dr["Version"] = Version;

                try
                {
                    ucTargetDataTable.Rows.Add(dr);
                    ucCLSelected(this, ucTargetDataTable);
                }
                catch (ConstraintException ce)
                {
                    ucCLSelected(this, null);
                }
            }
            else
            {
                ucCLSelected(this, null);
            }

            ExecuteJSPostback();
            Utils.RemoveLatestPopup();
            Utils.ForceBlackClosing();
        }

        #endregion

        #region Methods

        private ISdmxObjects GetSdmxObjects()
        {
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput;
            ISdmxObjects sdmxFinal;
            IMutableObjects mutableObj = new MutableObjectsImpl();
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);

            sdmxFinal = new SdmxObjectsImpl();

            try
            {
                sdmxInput = wsModel.GetCodeList(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion, ucIsFinalArtefact), true, true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {

                    foreach (ICodelistObject cl in sdmxInput.Codelists)
                    {
                        if (localizedUtils.GetNameableName(cl).Contains(SearchBar1.ucName.Trim()))
                            mutableObj.AddCodelist(cl.MutableInstance);

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

        private void OpenPopUp()
        {
            Utils.AppendScript("openP('df_CodeList" + ControlID + "'," + PopUpWidth.ToString() + ");");
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

        private void BindData()
        {
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);

            List<ISTAT.Entity.CodeList> lCodeList = eMapper.GetCodeListList(_sdmxObjects);

            gvCodelists.PageSize = 12;
            gvCodelists.DataSourceID = null;
            gvCodelists.DataSource = lCodeList;
            gvCodelists.DataBind();

        }

        #endregion


    }
}