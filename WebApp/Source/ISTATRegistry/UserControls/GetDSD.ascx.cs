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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System.Data;

namespace ISTATRegistry.UserControls
{
    public class GetDSDEventArgs : System.EventArgs
    {
        public DataTable GetDSDDataTable { get; set; }
    }

    public partial class GetDSD : System.Web.UI.UserControl
    {

        #region Public Props

        /// <summary>
        /// Il controllo target che verrà valorizzato con la DSD
        /// </summary>
        public ITextControl TargetWebControl { get; set; }

        /// <summary>
        /// Il controllo DataTable target che verrà valorizzato con la DSD
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

        public event EventHandler<GetDSDEventArgs> ucDSDSelectedEH;

        public void ucDSDSelected(object sender, DataTable dt)
        {
            if (ucDSDSelectedEH != null)
                ucDSDSelectedEH(sender, new GetDSDEventArgs() { GetDSDDataTable = dt });
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
            SearchBar1.SearchBarID = "DSD";

            SearchBar1.DataBind();

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

        protected void gvDSD_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            gvDSD.PageIndex = e.NewPageIndex;
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

        protected void gvDSD_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    break;
            }
        }

        protected void gvDSD_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = (gvDSD.SelectedRow.FindControl("lblID") as Label).Text;
            string Agency = (gvDSD.SelectedRow.FindControl("lblAgency") as Label).Text;
            string Version = (gvDSD.SelectedRow.FindControl("lblVersion") as Label).Text;

            if (TargetWebControl != null)
                TargetWebControl.Text = ID + "," + Agency + "," + Version;

            if (ucTargetDataTable != null)
            {
                DataRow dr = ucTargetDataTable.NewRow();
                dr["ID"] = ID;
                dr["Agency"] = Agency;
                dr["Version"] = Version;
                //ucTargetDataTable.PrimaryKey = new DataColumn[] { ucTargetDataTable.Columns["ID"], ucTargetDataTable.Columns["Agency"], ucTargetDataTable.Columns["Version"] };

                try
                {
                    ucTargetDataTable.Rows.Add(dr);
                    ucDSDSelected(this, ucTargetDataTable);
                }
                catch (ConstraintException ce)
                {
                    ucDSDSelected(this, null);
                }
            }
            else
            {
                ucDSDSelected(this, null);
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
                sdmxInput = wsModel.GetDataStructure(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion, ucIsFinalArtefact), true, true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {

                    foreach (IDataStructureObject dsd in sdmxInput.DataStructures)
                    {
                        if (localizedUtils.GetNameableName(dsd).Contains(SearchBar1.ucName.Trim()))
                            mutableObj.AddDataStructure(dsd.MutableInstance);

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
            Utils.AppendScript("openP('df_dsd" + ControlID + "'," + PopUpWidth.ToString() + ");");
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

            List<ISTAT.Entity.KeyFamily> lDsd = eMapper.GetKeyFamilyList(_sdmxObjects);

            gvDSD.PageSize = 12;
            gvDSD.DataSourceID = null;
            gvDSD.DataSource = lDsd;
            gvDSD.DataBind();

        }

        #endregion


    }
}