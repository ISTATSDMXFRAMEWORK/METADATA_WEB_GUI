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
    public partial class categorySchemes : ISTATRegistry.Classes.ISTATWebPage
    {
        ISdmxObjects _sdmxObjects;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAddCategoryScheme.DataBind();
            lblNumberOfRows.DataBind();
            btnChangePaging.DataBind();

            if (!Page.IsPostBack)
            {
                txtNumberOfRows.Text = Utils.GeneralCategorizationGridNumberRow.ToString();
            }
            SearchBar1.BtnSearch.Click += new EventHandler(btnSearch_Click);

            try
            {
                btnAddCategoryScheme.Visible = !Utils.ViewMode;
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
                if (!Page.IsPostBack)
                {
                    if (ex.Message.ToLower().Equals("no results found"))
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
                sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), true, true);
                //sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(SearchBar1.ucID, SearchBar1.ucAgency, SearchBar1.ucVersion), false, true);

                if (SearchBar1.ucName.Trim() != string.Empty)
                {

                    foreach (ICategorySchemeObject cs in sdmxInput.CategorySchemes)
                    {
                        if (localizedUtils.GetNameableName(cs).ToUpper().Contains(SearchBar1.ucName.Trim().ToUpper()))
                            mutableObj.AddCategoryScheme(cs.MutableInstance);

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

            List<ISTAT.Entity.CategoryScheme> lCategoryScheme = eMapper.GetCategorySchemeList(_sdmxObjects, Utils.LocalizedLanguage);

            int numberOfRows = 0;

            if (!txtNumberOfRows.Text.Trim().Equals(string.Empty) && int.TryParse(txtNumberOfRows.Text, out numberOfRows))
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = Utils.GeneralCategoryschemeGridNumberRow;
            }
            lblNumberOfTotalElements.Text = string.Format(Resources.Messages.lbl_number_of_total_rows, lCategoryScheme.Count.ToString());
            gridView.DataSourceID = null;
            gridView.DataSource = lCategoryScheme;
            gridView.DataBind();

            if (lCategoryScheme.Count == 0)
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

        protected void btnAddCategoryScheme_Click(object sender, EventArgs e)
        {
            Session[codelistItemDetails.KEY_PAGE_SESSION] = null;

            // Effettua il redirect sulla pagina dei dettagli di codelist
            Response.Redirect("CategorySchemeItemDetails.aspx?ACTION=INSERT");
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

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                (e.Row.FindControl("btnGraphics") as ImageButton).PostBackUrl = "~/CategorySchemeGraph.aspx?id=" + DataBinder.Eval(e.Row.DataItem, "ID") +
                    "&agency=" + DataBinder.Eval(e.Row.DataItem, "Agency") +
                    "&version=" + DataBinder.Eval(e.Row.DataItem, "Version");
            }

            HyperLink hplDetails = (e.Row.FindControl("hplDetails") as HyperLink);
            if (hplDetails != null)
            {
                string stringIdentity = Utils.GetStringKey((ArtefactIdentity)e.Row.DataItem);
                hplDetails.NavigateUrl = "CategorySchemeItemDetails.aspx?ACTION=UPDATE&" + stringIdentity;
            }

        }
        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {

            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            List<ISTAT.Entity.CategoryScheme> lCategoryscheme = eMapper.GetCategorySchemeList(_sdmxObjects);

            if ((SortDirection)ViewState["SortExpr"] == SortDirection.Ascending)
            {
                lCategoryscheme = lCategoryscheme.OrderBy(x => TypeHelper.GetPropertyValue(x, e.SortExpression)).Reverse().ToList();
                ViewState["SortExpr"] = SortDirection.Descending;
            }
            else
            {
                lCategoryscheme = lCategoryscheme.OrderBy(x => TypeHelper.GetPropertyValue(x, e.SortExpression)).ToList();
                ViewState["SortExpr"] = SortDirection.Ascending;
            }

            int numberOfRows = 0;

            if (!txtNumberOfRows.Text.Trim().Equals(string.Empty) && int.TryParse(txtNumberOfRows.Text, out numberOfRows))
            {
                gridView.PageSize = numberOfRows;
            }
            else
            {
                gridView.PageSize = Utils.GeneralCategoryschemeGridNumberRow;
            }
            gridView.DataSourceID = null;
            gridView.DataSource = lCategoryscheme;
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
            List<ISTAT.Entity.CategoryScheme> lCategoryscheme = eMapper.GetCategorySchemeList(_sdmxObjects);
            int numberOfRows = 0;
            if (!txtNumberOfRows.Text.Trim().Equals(string.Empty) && int.TryParse(txtNumberOfRows.Text, out numberOfRows))
            {
                if (numberOfRows > 0)
                {
                    gridView.PageSize = numberOfRows;
                }
                else
                {
                    gridView.PageSize = Utils.GeneralCategoryschemeGridNumberRow;
                    txtNumberOfRows.Text = Utils.GeneralCategoryschemeGridNumberRow.ToString();
                }
            }
            else if (!txtNumberOfRows.Text.Trim().Equals(string.Empty) && !int.TryParse(txtNumberOfRows.Text, out numberOfRows))
            {
                Utils.ShowDialog(Resources.Messages.err_wrong_rows_number_pagination);
                return;
            }
            else if (txtNumberOfRows.Text.Trim().Equals(string.Empty))
            {
                gridView.PageSize = Utils.GeneralCategoryschemeGridNumberRow;
                txtNumberOfRows.Text = Utils.GeneralCategoryschemeGridNumberRow.ToString();
            }
            gridView.DataSourceID = null;
            gridView.DataSource = lCategoryscheme;
            gridView.DataBind();
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ArtefactDelete deleteObject = e.Row.FindControl("ArtDelete") as ArtefactDelete;
            if (deleteObject != null)
            {
                if (Session[SESSION_KEYS.USER_OK] != null && (bool)Session[SESSION_KEYS.USER_OK] == true)
                {
                    string tmpAgency = ((Label)e.Row.FindControl("lblAgency")).Text;
                    User tmpUser = Session[SESSION_KEYS.USER_DATA] as User;

                    if (tmpUser.agencies.Where(agency => agency.id.Equals(tmpAgency)).ToList().Count != 0)
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