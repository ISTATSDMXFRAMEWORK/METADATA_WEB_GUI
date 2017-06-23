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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using ISTATUtils;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    
    public partial class categorization : ISTATRegistry.Classes.ISTATWebPage
    {      
        ISdmxObjects _sdmxObjects;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAddCategorization.DataBind();
            lblNumberOfRows.DataBind();
            btnChangePaging.DataBind();

            if ( !Page.IsPostBack )
            {
                txtNumberOfRows.Text = Utils.GeneralCategorizationGridNumberRow.ToString();
            }
            SearchBar1.BtnSearch.Click += new EventHandler(btnSearch_Click);

            try
            {
                _sdmxObjects = GetSdmxObjects();

                if (!IsPostBack)
                {
                    btnAddCategorization.Visible = !Utils.ViewMode;
                    gridView.Columns[7].Visible = !Utils.ViewMode;
                    ViewState["SortExpr"] = SortDirection.Ascending;     
                    BindData();
                }                
            }
            catch (Exception ex)
            {
                if ( !Page.IsPostBack )
                {
                    if ( ex.Message.ToLower().Equals( "no results found" ) )
                    {
                        txtNumberOfRows.Visible = false;
                        lblNumberOfRows.Visible = false;
                        btnChangePaging.Visible = false;
                        btnAddCategorization.Visible = !Utils.ViewMode;
                    }
                    Utils.ShowDialog(ex.Message);
                }
                return;
            }
        }

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
                sdmxInput = wsModel.GetCategorisation(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), false,true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {
                    foreach (ICategorisationObject cat in sdmxInput.Categorisations)
                    {
                        if (localizedUtils.GetNameableName(cat).ToUpper().Contains(SearchBar1.ucName.Trim().ToUpper()))
                            mutableObj.AddCategorisation(cat.MutableInstance);

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
            List<ISTAT.Entity.Categorization> lCategorization = eMapper.GetCategorizationList(_sdmxObjects);
            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = Utils.GeneralCategorizationGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, lCategorization.Count.ToString() );
            gridView.DataSourceID = null;
            gridView.DataSource = lCategorization;
            gridView.DataBind();

            if ( lCategorization.Count == 0 )
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

        protected void btnAddCategorization_Click(object sender, EventArgs e)
        {
            //Session[categorizationItemDetails.KEY_PAGE_SESSION] = null;

            // Effettua il redirect sulla pagina dei dettagli di codelist
            Response.Redirect("categorizationItemDetails.aspx?ACTION=INSERT");
        }

        void btnSearch_Click(object sender, EventArgs e)
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

        protected void OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            FileDownload3 fd = (e.Row.FindControl("FileDownload3") as FileDownload3);
            if (fd != null)
            {
                ScriptManager.GetCurrent(this).RegisterPostBackControl(fd);
            }
            HyperLink hplDetails = (e.Row.FindControl("hplDetails") as HyperLink);
            if (hplDetails != null)
            {
                string stringIdentity = Utils.GetStringKey((ArtefactIdentity)e.Row.DataItem);
                hplDetails.NavigateUrl = "categorizationItemDetails.aspx?ACTION=UPDATE&" + stringIdentity;
            }
        }

        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.Categorization> lCategorization = eMapper.GetCategorizationList(_sdmxObjects);

            if ((SortDirection)ViewState["SortExpr"] == SortDirection.Ascending)
            {
                lCategorization = lCategorization.OrderBy(x => TypeHelper.GetPropertyValue(x, e.SortExpression)).Reverse().ToList();
                ViewState["SortExpr"] = SortDirection.Descending;
            }
            else
            {
                lCategorization = lCategorization.OrderBy(x => TypeHelper.GetPropertyValue(x, e.SortExpression)).ToList();
                ViewState["SortExpr"] = SortDirection.Ascending;
            }

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = Utils.GeneralCategorizationGridNumberRow;
            }

            gridView.DataSourceID = null;
            gridView.DataSource = lCategorization;
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
                List<ISTAT.Entity.Categorization> lCategorization = eMapper.GetCategorizationList(_sdmxObjects);
                int numberOfRows = 0;
                if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
                {
                    if ( numberOfRows > 0 )
                    {
                        gridView.PageSize = numberOfRows;
                    }
                    else
                    {
                        gridView.PageSize = Utils.GeneralCategorizationGridNumberRow;
                        txtNumberOfRows.Text = Utils.GeneralCategorizationGridNumberRow.ToString();
                    }
                }
                else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
                {
                    Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                    return;
                }
                else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
                {
                    gridView.PageSize = Utils.GeneralCategorizationGridNumberRow;
                    txtNumberOfRows.Text = Utils.GeneralCategorizationGridNumberRow.ToString();
                }
                gridView.DataSourceID = null;
                gridView.DataSource = lCategorization;
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