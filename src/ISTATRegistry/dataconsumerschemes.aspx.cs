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
using ISTATRegistry.IRServiceReference;
using ISTATUtils;

namespace ISTATRegistry
{
    public partial class dataconsumerschemes : ISTATRegistry.Classes.ISTATWebPage
    {
        ISdmxObjects _sdmxObjects;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAdd.DataBind();
            lblNumberOfRows.DataBind();
            btnChangePaging.DataBind();

            if ( !Page.IsPostBack )
            {
                txtNumberOfRows.Text = Utils.GeneralDataConsumerschemeGridNumberRow.ToString();
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
                sdmxInput = wsModel.GetDataConsumerScheme(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), true,true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {
                    foreach (IDataConsumerScheme dataConsumerScheme in sdmxInput.DataConsumerSchemes)
                    {
                        if (localizedUtils.GetNameableName(dataConsumerScheme).ToUpper().Contains(SearchBar1.ucName.Trim().ToUpper()))
                            mutableObj.AddDataConsumerScheme(dataConsumerScheme.MutableInstance);
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

            List<ISTAT.Entity.DataConsumerScheme> _list = eMapper.GetDataConsumerSchemeList(_sdmxObjects, Utils.LocalizedLanguage);

            int numberOfRows = 0;

            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = Utils.GeneralDataConsumerschemeGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format( Resources.Messages.lbl_number_of_total_rows, _list.Count.ToString() );
            gridView.DataSourceID = null;
            gridView.DataSource = _list;
            gridView.DataBind();

            if ( _list.Count == 0 )
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (_sdmxObjects == null)
                return;
            BindData();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            // Effettua il redirect sulla pagina dei dettagli del concept scheme
            Response.Redirect("DataConsumerSchemeItemDetails.aspx?ACTION=INSERT");
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
                hplDetails.NavigateUrl = "DataConsumerSchemeItemDetails.aspx?ACTION=UPDATE&" + stringIdentity;
            }
        }

        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.DataConsumerScheme> _list = eMapper.GetDataConsumerSchemeList(_sdmxObjects);

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
                gridView.PageSize = Utils.GeneralDataConsumerschemeGridNumberRow;
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
            List<ISTAT.Entity.DataConsumerScheme> lDataConsumerscheme = eMapper.GetDataConsumerSchemeList(_sdmxObjects);
            int numberOfRows = 0;
            if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                if ( numberOfRows > 0 )
                {
                    gridView.PageSize = numberOfRows;
                }
                else
                {
                    gridView.PageSize = Utils.GeneralDataConsumerschemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.GeneralDataConsumerschemeGridNumberRow.ToString();
                }
            }
            else if ( !txtNumberOfRows.Text.Trim().Equals( string.Empty ) && !int.TryParse( txtNumberOfRows.Text, out numberOfRows ) )
            {
                Utils.ShowDialog( Resources.Messages.err_wrong_rows_number_pagination );
                return;
            }
            else if ( txtNumberOfRows.Text.Trim().Equals( string.Empty ) )
            {
                gridView.PageSize = Utils.GeneralDataConsumerschemeGridNumberRow;
                txtNumberOfRows.Text = Utils.GeneralDataConsumerschemeGridNumberRow.ToString();
            }
            gridView.DataSourceID = null;
            gridView.DataSource = lDataConsumerscheme;
            gridView.DataBind();  
        }

        protected void UpdatePanel1_PreRender(object sender, EventArgs e)
        {
            Utils.AppendScript( "$.unblockUI();" );
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