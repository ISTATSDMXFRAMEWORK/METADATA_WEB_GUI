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
using ISTATRegistry.UserControls;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;

namespace ISTATRegistry
{
    public partial class Hcl : ISTATRegistry.Classes.ISTATWebPage
    {
        ISdmxObjects _sdmxObjects;

        public int GVRowNumber { get { return Utils.GeneralStructureSetNumberRow; } }

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
                sdmxInput = wsModel.GetHcl(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), true,true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {

                    foreach (IHierarchicalCodelistObject hcl in sdmxInput.HierarchicalCodelists)
                    {
                        if (localizedUtils.GetNameableName(hcl).ToUpper().Contains(SearchBar1.ucName.Trim().ToUpper()))
                            mutableObj.AddHierarchicalCodelist(hcl.MutableInstance);
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
            List<ISTAT.Entity.HierarchicalCodelist> lHcl = GetHclList();

            if (lHcl == null)
            {
                Utils.ShowDialog(Resources.Messages.msg_no_result_found);
                return;
            }

            gridView.PageSize = GVRowNumber;
            gridView.DataSourceID = null;
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }

            gridView.DataSource = lHcl;
            gridView.DataBind();

            if (lHcl.Count == 0)
            {
                txtNumberOfRows.Visible = false;
                lblNumberOfRows.Visible = false;
                btnChangePaging.Visible = false;
            }
            else
            {
                txtNumberOfRows.Visible = true;
                lblNumberOfRows.Visible = true;
                btnChangePaging.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if ( !Page.IsPostBack )
            {
                txtNumberOfRows.Text = GVRowNumber.ToString();
            }

            SearchBar1.BtnSearch.Click += new EventHandler(btnSearch_Click);
            
            try
            {
                _sdmxObjects = GetSdmxObjects();

                if (!IsPostBack)
                {
                    btnAdd.Visible = !Utils.ViewMode;
                    gridView.Columns[7].Visible = !Utils.ViewMode;

                    ViewState["SortExpr"] = SortDirection.Ascending;

                    BindData();
                }

            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("HclItemDetails.aspx?ACTION=INSERT");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (_sdmxObjects == null)
                return;
            BindData();
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            gridView.PageIndex = e.NewPageIndex;
            BindData();

        }
        protected void OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Details":
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);

                    string stringIdentity = Utils.GetStringKey(gridView.Rows[gvr.RowIndex]);

                    Response.Redirect("HclItemDetails.aspx?ACTION=UPDATE&" + stringIdentity);

                    break;
                case "xxxx":
                    break;
            }
        }
        protected void OnRowCreated(object sender, GridViewRowEventArgs e)
        {

            FileDownload3 fd = (e.Row.FindControl("FileDownload3") as FileDownload3);
            if (fd != null)
            {
                ScriptManager.GetCurrent(this).RegisterPostBackControl(fd);
            }
        }
        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {
            List<ISTAT.Entity.HierarchicalCodelist> _list = GetHclList();

            if ((SortDirection)ViewState["SortExpr"] == SortDirection.Ascending)
            {
                _list = _list.OrderBy(x => TypeHelper.GetPropertyValue(x, e.SortExpression)).Reverse().ToList();
                ViewState["SortExpr"] = SortDirection.Descending;
            }
            else
            {
                _list = _list.OrderBy(x => TypeHelper.GetPropertyValue(x, e.SortExpression)).ToList();
                ViewState["SortExpr"] = SortDirection.Ascending;
            }

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = GVRowNumber;
            }
            gridView.DataSourceID = null;
            gridView.DataSource = _list;
            gridView.DataBind();

        }
        protected void OnSorted(object sender, EventArgs e)
        {

        }

        protected void txtNumberOfRows_TextChanged(object sender, EventArgs e)
        {     
        }

        protected void btnChangePaging_Click(object sender, EventArgs e)
        {
            List<ISTAT.Entity.HierarchicalCodelist> lHcl = GetHclList();

            int numberOfRows = 0;
            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gridView.PageSize = numberOfRows;
                }
                else
                {
                    gridView.PageSize = GVRowNumber;
                    txtNumberOfRows.Text = GVRowNumber.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gridView.PageSize = GVRowNumber;
                txtNumberOfRows.Text = GVRowNumber.ToString();
            }
            gridView.DataSourceID = null;
            gridView.DataSource = lHcl;
            gridView.DataBind(); 
        }

        private List<ISTAT.Entity.HierarchicalCodelist> GetHclList()
        {
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            return eMapper.GetHclList(_sdmxObjects, Utils.LocalizedLanguage);
        }
    }
}