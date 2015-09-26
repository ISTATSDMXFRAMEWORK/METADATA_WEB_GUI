using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

namespace ISTATRegistry.UserControls
{
    public partial class AddText : System.Web.UI.UserControl
    {
        #region Public Props

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
        /// Se true visualizza il combo per i template del value
        /// </summary>
        public bool ucEnableSuggest { get; set; }

        /// <summary>
        /// Se valorizzato a True visualizza il testo read only in una textbox
        /// </summary>
        public bool ucEditMode
        {
            get
            {
                return _viewMode;
            }
            set
            {
                SetEditForm(value);
                _viewMode = value;
            }
        }

        private void SetEditForm(bool bView)
        {
            pnlEditText.Visible = bView;
            pnlViewText.Visible = !bView;
            //cmbAnnotationValue.Visible = bView; 
        }


        /// <summary>
        /// Il tipo di artefatto dove viene incluso il controllo
        /// </summary>
        public SdmxStructureEnumType ArtefactType { get; set; }

        /// <summary>
        /// Il tipo di Text (Name o Description)
        /// </summary>
        public TextType TType { get; set; }

        public string ucOpenPopUpName2 { get; set; }
        public string ucOpenPopUpName3 { get; set; }

        public IList<ITextTypeWrapperMutableObject> TextObjectList
        {
            get
            {
                if (Session[_sessionName] != null)
                    return (IList<ITextTypeWrapperMutableObject>)Session[_sessionName];
                else
                    return null;
            }
        }

        public IList<ITextTypeWrapper> InitTextObjectList
        {
            set
            {
                if (_textObjectList == null)
                    _textObjectList = new List<ITextTypeWrapperMutableObject>();

                foreach (ITextTypeWrapper ttw in value)
                {
                    _textObjectList.Add(new TextTypeWrapperMutableCore(ttw.Locale, ttw.Value));
                }

                SetTextObjectToSession();
                if (Page.IsPostBack)
                    GVDataBind();
            }
        }

        public IList<ITextTypeWrapperMutableObject> InitTextObjectMutableList
        {
            set
            {
                _textObjectList = new List<ITextTypeWrapperMutableObject>();

                foreach (ITextTypeWrapperMutableObject ttw in value)
                {
                    _textObjectList.Add(ttw);
                }

                SetTextObjectToSession();
                if (Page.IsPostBack)
                    GVDataBind();
            }
        }

        #endregion

        #region Private Props

        private bool _viewMode;

        private IList<ITextTypeWrapperMutableObject> _textObjectList;

        //protected string _sessionName { get { return ArtefactType.ToString() + TType.ToString(); } }
        protected string _sessionName { get { return ClientID; } }


        #endregion

        #region Public Methods

        /// <summary>
        /// Svuota la lista dei text
        /// </summary>
        public void ClearTextObjectList()
        {
            if (Session[_sessionName] != null)
            {
                Session[_sessionName] = null;
                if (_textObjectList != null)
                    _textObjectList.Clear();
                GVDataBind();
            }
            ExecuteJSPostback();
        }

        /// <summary>
        /// Svuota la lista dei text
        /// </summary>
        public void ClearTextObjectListWithOutJS()
        {
            if (Session[_sessionName] != null)
            {
                Session[_sessionName] = null;
                if (_textObjectList != null)
                    _textObjectList.Clear();
                GVDataBind();
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            GetTextObjectFromSession();

            if (!IsPostBack)
            {
                SetForm();
                GVDataBind();
            }

            lblLanguage.DataBind();
            lblText.DataBind();
            btnAdd.DataBind();
        }

        protected void cmbAnnotationValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAnnotationValue.SelectedValue != "")
            {
                txtText.Text = cmbAnnotationValue.SelectedValue;
            }

            ExecuteJSPostback();
            OpenDialogForm();

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (_textObjectList == null)
                _textObjectList = new List<ITextTypeWrapperMutableObject>();
            else
            {
                foreach (ITextTypeWrapperMutableObject tw in _textObjectList)
                {
                    if (tw.Locale == cmbLanguage.SelectedValue)
                    {
                        OpenTab();
                        ExecuteJSPostback();
                        Utils.RemoveLatestPopup();
                        return;
                    }
                }
            }

            if (txtText.Text.Trim().Equals(string.Empty))
            {
                Utils.ShowDialog(Resources.Messages.err_empty_string);
                return;
            }
            else
            {
                _textObjectList.Add(new TextTypeWrapperMutableCore(cmbLanguage.SelectedValue, txtText.Text));
                GVDataBind();
                SetTextObjectToSession();
                txtText.Text = "";
                ExecuteJSPostback();
                Utils.RemoveLatestPopup();
            }
        }

        protected void gvText_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvText.EditIndex = e.NewEditIndex;
            GVDataBind();
            ExecuteJSPostback();
        }

        protected void gvText_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvText.EditIndex = -1;
            GVDataBind();
            ExecuteJSPostback();
        }

        protected void gvText_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //TODO: Confirm su Delete
            string locale = ((Label)gvText.Rows[e.RowIndex].FindControl("lblLanguage")).Text;
            if (_textObjectList != null)
                foreach (var tol in _textObjectList)
                {
                    if (tol.Locale == locale)
                    {
                        _textObjectList.Remove(tol);
                        break;
                    }
                }
            GVDataBind();
            SetTextObjectToSession();
            ExecuteJSPostback();
        }

        protected void gvText_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                DropDownList cmbLanguages = (DropDownList)e.Row.FindControl("cmbLanguage");
                Utils.PopulateCmbLanguages(cmbLanguages, AVAILABLE_MODES.MODE_FOR_ADD_TEXT);

                string locale = DataBinder.Eval(e.Row.DataItem, "Locale").ToString();
                cmbLanguages.Items.FindByValue(locale).Selected = true;
            }
        }

        #endregion

        #region Methods

        private void SetForm()
        {
            string labelDescription = "";
            switch (TType)
            {
                case TextType.NAME:
                    labelDescription = Resources.Messages.lbl_add_name;
                    break;
                case TextType.DESCRIPTION:
                    labelDescription = Resources.Messages.lbl_add_description;
                    break;
            }

            lblTitle.Text = labelDescription;

            if (ucEnableSuggest && ucEditMode && ((EndPointElement)Session["CurrentEndPointObject"]).EnableAnnotationSuggest)
            {
                Utils.PopulateCmbAnnotationValue(cmbAnnotationValue);
                cmbAnnotationValue.Visible = true;
            }

            Utils.PopulateCmbLanguages(cmbLanguage, AVAILABLE_MODES.MODE_FOR_ADD_TEXT);

            if (Session[SESSION_KEYS.KEY_LANG] != null)
                cmbLanguage.SelectedValue = Session[SESSION_KEYS.KEY_LANG].ToString();
        }

        private void GVDataBind()
        {
            if (_textObjectList != null)
            {
                ISTATUtils.LocalizedUtils loc = new ISTATUtils.LocalizedUtils(Utils.LocalizedCulture);
                txtViewText.Text = loc.GetLocalizedText(_textObjectList);

                gvText.DataSource = _textObjectList;
                gvText.DataBind();

            }
        }

        private void GetTextObjectFromSession()
        {
            if (Session[_sessionName] != null)
            {
                _textObjectList = (IList<ITextTypeWrapperMutableObject>)Session[_sessionName];
            }
        }

        private void SetTextObjectToSession()
        {

            if (_textObjectList != null && _textObjectList.Count > 0)
                Session[_sessionName] = _textObjectList;
            else
                Session[_sessionName] = null;
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

        private void OpenDialogForm()
        {
            Utils.AppendScript("openP('dialog-form" + _sessionName + "',900);");
        }

        private void OpenUCPopUp()
        {
            if (!String.IsNullOrEmpty(ucOpenPopUpName3))
                Utils.AppendScript("openP('" + ucOpenPopUpName3 + "'," + ucOpenPopUpWidth.ToString() + ");");

            if (!String.IsNullOrEmpty(ucOpenPopUpName2))
                Utils.AppendScript("openP('" + ucOpenPopUpName2 + "'," + ucOpenPopUpWidth.ToString() + ");");

            if (!String.IsNullOrEmpty(ucOpenPopUpName))
                Utils.AppendScript("openP('" + ucOpenPopUpName + "'," + ucOpenPopUpWidth.ToString() + ");");
            // Utils.ForceBlackClosing();
        }

        #endregion


    }

}