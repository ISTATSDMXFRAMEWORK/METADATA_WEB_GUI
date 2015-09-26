using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.UserControls
{
    public partial class SearchBar : System.Web.UI.UserControl
    {
        #region Public Props

        /// <summary>
        /// Valorizzato a True rende sempre visibile il controllo sulla pagina
        /// </summary>
        public bool FixedSearch { get; set; }

        /// <summary>
        /// Da valorizzare per rendere univoco il controllo nel form
        /// </summary>
        public String SearchBarID { get; set; }

        /// <summary>
        /// ID dell'artefatto da ricercare
        /// </summary>
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

        /// <summary>
        /// Agency dell'artefatto da ricercare
        /// </summary>
        public string ucAgency
        {
            get
            {
                return txtSearchAgency.Text.Trim(); //cmbAgencies.SelectedValue.Trim();
            }
            set
            {
                txtSearchAgency.Text = value;
            }
        }

        /// <summary>
        /// Version dell'artefatto da ricercare
        /// </summary>
        public string ucVersion
        {
            get
            {
                return txtSearchVersion.Text.Trim();
            }
            set
            {
                txtSearchVersion.Text = value;
            }
        }

        /// <summary>
        /// Name dell'artefatto da ricercare
        /// </summary>
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

        /// <summary>
        /// Bottone da ampliare con Event Handler per gestire il click della ricerca
        /// </summary>
        public Button BtnSearch
        {
            get
            {
                return btnSearch;
            }
        }

        #endregion

        #region Private Props

        protected String SearchBarCompletedID
        {
            get
            {
                return "SearchBar" + SearchBarID;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                lbl_id.DataBind();
                lbl_name.DataBind();
                lbl_version.DataBind();
                lbl_agency.DataBind();
                btnSearch.DataBind();
                //Utils.PopulateCmbAgencies(cmbAgencies, false);
                //cmbAgencies.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            }

            if (FixedSearch)
            {
                pnlSearchButton.Visible = false;
                pnlFixed.Visible = true;
            }

            if (!FixedSearch)
                Utils.AppendScript("$(document).ready( function() { $('#SearchPanel').css( 'display', 'none' ); $( '#imgSearch' ).attr( 'src', './images/Search_off.png' ); });");
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchID.Text = "";
            txtSearchAgency.Text = "";
            txtSearchVersion.Text = "";
        }

        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Utils.AppendScript("$(document).ready( function() { $('#SearchPanel').css( 'display', 'block' ); $( '#imgSearch' ).attr( 'src', './images/Search_on.png' ); });");
        }

    }
}