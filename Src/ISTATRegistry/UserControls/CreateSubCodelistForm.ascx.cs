using ISTAT.Entity;
using ISTAT.EntityMapper;
using ISTAT.WSDAL;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.UserControls
{

    public class ListBoxItem : Object
    {
        public virtual string Text { get; set; }
        public virtual object Tag { get; set; }
        public virtual object Object { get; set; }
        public virtual string Name { get; set; }

        /// <summary>
        /// Class Constructor
        /// </summary>
        public ListBoxItem()
        {
            this.Text = string.Empty;
            this.Tag = null;
            this.Name = string.Empty;
            this.Object = null;
        }

        /// <summary>
        /// Overloaded Class Constructor
        /// </summary>
        /// <param name="Text">Object Text</param>
        /// <param name="Name">Object Name</param>
        /// <param name="Tag">Object Tag</param>
        /// <param name="Object">Object</param>
        public ListBoxItem(string Text, string Name, object Tag, object Object)
        {
            this.Text = Text;
            this.Tag = Tag;
            this.Name = Name;
            this.Object = Object;
        }

        /// <summary>
        /// Overloaded Class Constructor
        /// </summary>
        /// <param name="Object">Object</param>
        public ListBoxItem(object Object)
        {
            this.Text = Object.ToString();
            this.Name = Object.ToString();
            this.Object = Object;
        }

        /// <summary>
        /// Overridden ToString() Method
        /// </summary>
        /// <returns>Object Text</returns>
        public override string ToString()
        {
            return this.Text;
        }
    }

    public partial class CreateSubCodelistForm : System.Web.UI.UserControl
    {
        private ArtefactIdentity _aiCL;
        private ArrayList alistLeft = new ArrayList();
        private ArrayList alistSelected = new ArrayList();
        private char[] TOKEN = new char[] { '|' };

        public ListBox SELECTEDVALUES { get { return lbCodesSelected; } }

        public bool CHKParentCode { get { return chkParentCode.Checked; } }

        public string ucValue
        {
            get
            {
                return txtSearchValue.Text.Trim();
            }
            set
            {
                txtSearchValue.Text = value;
            }
        }
        
        public Button BtnSearch
        {
            get
            {
                return btnSearch;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string filterVal = txtSearchValue.Text.Trim();
            string z = ddlSearchType.SelectedItem.Value;
            string[] tmpVal;

            int pos = Convert.ToInt32(ddlSearchField.SelectedItem.Value);
            for (int i = 0; i < lbCodesLeft.Items.Count; i++)
            {
                tmpVal = lbCodesLeft.Items[i].Text.Split(TOKEN);
                if (!String.IsNullOrEmpty(filterVal) && (z.Equals("1") ? (tmpVal[pos].IndexOf(filterVal) < 0 ? false : true) : tmpVal[pos].Equals(filterVal)))
                    lbCodesLeft.Items[i].Selected = true;
                else
                    lbCodesLeft.Items[i].Selected = false;
            }
            Utils.AppendScript("location.href='#derivedCL';");
        }
        
        protected void btnRemoveCodes_Click(object sender, EventArgs e)
        {

            if (lbCodesSelected.SelectedIndex >= 0)
            {
                for (int i = 0; i < lbCodesSelected.Items.Count; i++)
                {
                    if (lbCodesSelected.Items[i].Selected)
                    {
                        if (!alistSelected.Contains(lbCodesSelected.Items[i]))
                        {
                            alistSelected.Add(lbCodesSelected.Items[i]);
                        }
                    }
                }
                for (int i = 0; i < alistSelected.Count; i++)
                {
                    if (!lbCodesLeft.Items.Contains(((ListItem)alistSelected[i])))
                    {
                        lbCodesLeft.Items.Add(((ListItem)alistSelected[i]));
                    }
                    lbCodesSelected.Items.Remove(((ListItem)alistSelected[i]));
                }
                lbCodesLeft.SelectedIndex = -1;
            }
            else { Utils.ShowDialog("Select an element at least"); }

            Utils.AppendScript("location.href= '#derivedCL';");
        }

        protected void btnAddCodes_Click(object sender, EventArgs e)
        {

            if (lbCodesLeft.SelectedIndex >= 0)
            {
                for (int i = 0; i < lbCodesLeft.Items.Count; i++)
                {
                    if (lbCodesLeft.Items[i].Selected)
                    {
                        if (!alistLeft.Contains(lbCodesLeft.Items[i]))
                        {
                            alistLeft.Add(lbCodesLeft.Items[i]);
                        }
                    }
                }
                for (int i = 0; i < alistLeft.Count; i++)
                {
                    if (!lbCodesSelected.Items.Contains(((ListItem)alistLeft[i])))
                    {
                        lbCodesSelected.Items.Add(((ListItem)alistLeft[i]));
                    }
                    lbCodesLeft.Items.Remove(((ListItem)alistLeft[i]));
                }
                lbCodesSelected.SelectedIndex = -1;
            }
            else { Utils.ShowDialog("Select an element at least"); }
            Utils.AppendScript("location.href= '#derivedCL';");
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    
                    ICodelistMutableObject objcodelist = (ICodelistMutableObject)Session[codelistItemDetails.KEY_PAGE_SESSION];

                    if (objcodelist != null)
                    {
                        _aiCL = new ArtefactIdentity();
                        _aiCL.Agency = objcodelist.AgencyId;
                        _aiCL.ID = objcodelist.Id ;
                        _aiCL.Version =objcodelist.Version;

                        Session["TempCodesLeft"] = new List<CodeItem>();
                        Session["TempCodesSelected"] = new List<CodeItem>();

                        BindData();
                    }
                }

            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }

        private void BindData()
        {
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);
            ISdmxObjects _sdmxObjects = GetSdmxObjects();

            List<CodeItem> lCodeItem = eMapper.GetCodeItemList(_sdmxObjects);

            lbCodesLeft.DataSource = lCodeItem;
            lbCodesLeft.DataBind();

            btnSearch.DataBind();
            Session["TempCodesLeft"] = lCodeItem;
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