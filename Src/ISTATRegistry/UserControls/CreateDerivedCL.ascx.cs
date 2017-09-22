using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using ISTAT.WSDAL;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using ISTATUtils;
using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using ISTAT.EntityMapper;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;

namespace ISTATRegistry.UserControls
{
    public partial class CreateDerivedCL : System.Web.UI.UserControl
    {
        //private ISdmxObjects _sdmxObjects;
        private ArtefactIdentity _aiCL;
        
        private List<ISTAT.Entity.CodeItem> _lCodeItem;
        //private List<ISTAT.Entity.CodeItem> _lDerivedCodeItem;
        
        public string ucName
        {
            get
            {
                return txtSearchName.Text.Trim();
            }
            set
            {
                txtSearchName.Text = value;
            }
        }
        public string ucID
        {
            get
            {
                return txtSearchID.Text.Trim();
            }
            set
            {
                txtSearchID.Text = value;
            }
        }
        public string ucParentCode
        {
            get
            {
                return txtSearchParentCode.Text.Trim();
            }
            set
            {
                txtSearchParentCode.Text = value;
            }
        }
        
        public Button BtnSearch
        {
            get
            {
                return btnSearch;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    ICodelistMutableObject obj = (ICodelistMutableObject)Session[codelistItemDetails.KEY_PAGE_SESSION];

                    _aiCL = new ArtefactIdentity();

                    _aiCL.Agency = obj.AgencyId;
                    _aiCL.ID = obj.Id;
                    _aiCL.Version = obj.Version;
                    Session["TempCodesLeft"] = new List<CodeItem>();
                    Session["TempCodesSelected"] = new List<CodeItem>(); 
                    
                    BindData();
                }

                _lCodeItem = (List<CodeItem>)Session["CL_ORIGINAL"]; 
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }
        
        protected void btnSearch_Click(object sender, EventArgs e) 
        {
            Utils.AppendScript("$(document).ready( function() { $('#SearchPanel').css( 'display', 'block' ); $( '#imgSearch' ).attr( 'src', './images/Search_on.png' ); });");
        }

        protected void btnAddCodes_Click(object sender, EventArgs e)
        {
            List<CodeItem> lCodesLeft = (List<CodeItem>)Session["TempCodesLeft"];
            List<CodeItem> lCodesSelected = (List<CodeItem>)Session["TempCodesSelected"];

            CodeItem cItem;
            string strID= String.Empty;
            string strName = String.Empty;
            string strParentCode = String.Empty;

            //lInitialCodes = _lCodeItem;
            
            foreach (GridViewRow row in gvDerivedCL.Rows) 
            {
                CheckBox chk = row.Cells[4].Controls[1] as CheckBox;
                
                if (chk != null && chk.Checked)
                {
                    strID = ((Label)row.Cells[1].FindControl("lblCLID")).Text;
                    strName = ((Label)row.Cells[2].FindControl("lblCLName")).Text;
                    strParentCode = ((Label)row.Cells[3].FindControl("lblCLParentCode")).Text;
                    cItem = new CodeItem { Code = strID, Name = strName, ParentCode = strParentCode };
                    lCodesSelected.Add(cItem);

                    lCodesLeft.Remove(lCodesLeft.Find(x => x.Code == cItem.Code));
                }
            }

            gvDerivedCL.DataSource = lCodesLeft;
            gvDerivedCL.DataBind();

            gvFiltered.DataSource = lCodesSelected;
            gvFiltered.DataBind();

            Session["TempCodesLeft"] = lCodesLeft;
            Session["TempCodesSelected"] = lCodesSelected;
            
        }

        protected void gvFiltered_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFiltered.PageSize = 12;
            gvFiltered.PageIndex = e.NewPageIndex;
            //BindData();
            Utils.AppendScript("location.href= '#derivedCL';");
        }

        protected void gvDerivedCL_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDerivedCL.PageSize = 12;
            gvDerivedCL.PageIndex = e.NewPageIndex;
            //BindData();
            Utils.AppendScript("location.href= '#derivedCL';");
        }
        private void BindData()
        {
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            ISdmxObjects _sdmxObjects = GetSdmxObjects();
            
            _lCodeItem = eMapper.GetCodeItemList(_sdmxObjects);

            gvDerivedCL.DataSourceID = null;
            gvDerivedCL.DataSource = _lCodeItem;
            gvDerivedCL.DataBind();

            lbl_id.DataBind();
            lbl_name.DataBind();
            lbl_parentcode.DataBind();

            btnSearch.DataBind();
            Session["TempCodesLeft"] = _lCodeItem;
        }

        private ISdmxObjects GetSdmxObjects()
        {
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput;
            IMutableObjects mutableObj = new MutableObjectsImpl();
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);


            try
            {
                sdmxInput = wsModel.GetCodeList(_aiCL, false, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sdmxInput;
        }
    }
}