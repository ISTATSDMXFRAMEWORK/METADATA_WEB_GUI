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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using ISTATUtils;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class agencyschemes : ISTATRegistry.Classes.ISTATWebPage
    {
        ISdmxObjects _sdmxObjects;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAdd.DataBind();
            lblNumberOfRows.DataBind();
            btnChangePaging.DataBind();

            if ( !Page.IsPostBack )
            {
                txtNumberOfRows.Text = Utils.GeneralAgencyschemeGridNumberRow.ToString();
            }

            SearchBar1.BtnSearch.Click += new EventHandler(btnSearch_Click);

            try
            {
                btnAdd.Visible = !Utils.ViewMode;
                gridView.Columns[7].Visible = !Utils.ViewMode;

                _sdmxObjects = GetSdmxObjects();

                if (!IsPostBack)
                {
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
                    }
                    Utils.ShowDialog(ex.Message);
                }
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
                sdmxInput = wsModel.GetAgencyScheme(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), true,true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {
                    foreach (IAgencyScheme agency in sdmxInput.AgenciesSchemes)
                    {
                        if (localizedUtils.GetNameableName(agency).ToUpper().Contains(SearchBar1.ucName.Trim().ToUpper()))
                            mutableObj.AddAgencyScheme(agency.MutableInstance);
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

            List<ISTAT.Entity.AgencyScheme> _list = eMapper.GetAgencySchemeList(_sdmxObjects, Utils.LocalizedLanguage);

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = Utils.GeneralAgencyschemeGridNumberRow;
            }

            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, _list.Count.ToString() );
            if ( _list.Count == 0 )
            {
                lblNumberOfRows.Visible = false;
                txtNumberOfRows.Visible = false;
                btnChangePaging.Visible = false;
            }
            else
            {
                lblNumberOfRows.Visible = true;
                txtNumberOfRows.Visible = true;
                btnChangePaging.Visible = true;
            }
            
            gridView.DataSourceID = null;
            gridView.DataSource = _list;
            gridView.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (_sdmxObjects == null)
                return;
            BindData();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            // Effettua il redirect sulla pagina dei dettagli del concept scheme
            Response.Redirect("AgencySchemeItemDetails.aspx?ACTION=INSERT");
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
                    Response.Redirect("AgencySchemeItemDetails.aspx?ACTION=UPDATE&" + stringIdentity);
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
            List<ISTAT.Entity.AgencyScheme> _list = eMapper.GetAgencySchemeList(_sdmxObjects);

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
                gridView.PageSize = Utils.GeneralAgencyschemeGridNumberRow;
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
            List<ISTAT.Entity.AgencyScheme> lAgencyscheme = eMapper.GetAgencySchemeList(_sdmxObjects);
            int numberOfRows = 0;
            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gridView.PageSize = numberOfRows;
                }
                else
                {
                    gridView.PageSize = Utils.GeneralAgencyschemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.GeneralAgencyschemeGridNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gridView.PageSize = Utils.GeneralAgencyschemeGridNumberRow;
                txtNumberOfRows.Text = Utils.GeneralAgencyschemeGridNumberRow.ToString();
            }
            gridView.DataSourceID = null;
            gridView.DataSource = lAgencyscheme;
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