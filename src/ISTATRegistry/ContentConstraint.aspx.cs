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
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class ContentConstraint : ISTATRegistry.Classes.ISTATWebPage
    {
        ISdmxObjects _sdmxObjects;

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
                sdmxInput = wsModel.GetContentConstraint(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), true,true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {

                    foreach (IContentConstraintObject ccs in sdmxInput.ContentConstraintObjects)
                    {
                        if (localizedUtils.GetNameableName(ccs).ToUpper().Contains(SearchBar1.ucName.Trim().ToUpper()))
                            mutableObj.AddContentConstraint(ccs.MutableInstance);
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

            List<ISTAT.Entity.ContentConstraint> lContConstr = eMapper.GetContentConstraintList(_sdmxObjects, Utils.LocalizedLanguage);

            if (lContConstr == null)
            {
                Utils.ShowDialog(Resources.Messages.msg_no_result_found);
                return;
            }

            gridView.PageSize = Utils.GeneralContentConstraintNumberRow;
            gridView.DataSourceID = null;
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }

            gridView.DataSource = lContConstr;
            gridView.DataBind();

            if ( lContConstr.Count == 0 )
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
                txtNumberOfRows.Text = Utils.GeneralContentConstraintNumberRow.ToString();
            }

            SearchBar1.BtnSearch.Click += new EventHandler(btnSearch_Click);
            
            try
            {
                _sdmxObjects = GetSdmxObjects();

                if (!IsPostBack)
                {                    
                    gridView.Columns[7].Visible = !Utils.ViewMode;

                    ViewState["SortExpr"] = SortDirection.Ascending;

                    BindData();
                }

            }
            catch (Exception ex)
            {
                if ( ex.Message.ToLower().Equals( "no results found" ) )
                    {
                        txtNumberOfRows.Visible = false;
                        lblNumberOfRows.Visible = false;
                        btnChangePaging.Visible = false;
                    }
                Utils.ShowDialog(ex.Message);
            }
            finally
            {
                btnAdd.Visible = !Utils.ViewMode;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("ContentConstraintItemDetails.aspx?ACTION=INSERT");
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

                    Response.Redirect("ContentConstraintItemDetails.aspx?ACTION=UPDATE&" + stringIdentity);

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

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.ContentConstraint> _list = eMapper.GetContentConstraintList(_sdmxObjects, Utils.LocalizedLanguage);

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
                gridView.PageSize = Utils.GeneralContentConstraintNumberRow;
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
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.ContentConstraint> lContConstr = eMapper.GetContentConstraintList(_sdmxObjects,Utils.LocalizedLanguage);
            int numberOfRows = 0;
            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gridView.PageSize = numberOfRows;
                }
                else
                {
                    gridView.PageSize = Utils.GeneralContentConstraintNumberRow;
                    txtNumberOfRows.Text = Utils.GeneralContentConstraintNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gridView.PageSize = Utils.GeneralContentConstraintNumberRow;
                txtNumberOfRows.Text = Utils.GeneralContentConstraintNumberRow.ToString();
            }
            gridView.DataSourceID = null;
            gridView.DataSource = lContConstr;
            gridView.DataBind(); 
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ArtefactDelete deleteObject = e.Row.FindControl( "ArtDelete" ) as ArtefactDelete;
            if ( deleteObject != null )
            {
                if ( Session[SESSION_KEYS.USER_OK] != null && (bool)Session[SESSION_KEYS.USER_OK] == true )
                {
                    string tmpAgency = ((Label)e.Row.FindControl( "lblAgency" )).Text;
                    User tmpUser = Session[SESSION_KEYS.USER_DATA] as User;

                    if ( tmpUser.agencies.Where( agency => agency.id.Equals( tmpAgency ) ).ToList().Count != 0 )
                    {
                        deleteObject.ucCanDeleteThis = 1;
                    }
                    else
                    {
                        deleteObject.ucCanDeleteThis = 0;
                    }                
                }    
            }
        }

    }
}