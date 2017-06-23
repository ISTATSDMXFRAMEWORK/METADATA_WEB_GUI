using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using System.Diagnostics;
using ISTATRegistry.IRServiceReference;
using ISTATUtils;

namespace ISTATRegistry.UserControls
{

    #region Delegates

    public delegate void AnnotationCreatedEventHandler(object sender, IAnnotationMutableObject e);
    public delegate void AnnotationDeletedEventHandler(object sender, IAnnotationMutableObject e);

    #endregion

    public partial class ControlAnnotations : System.Web.UI.UserControl
    {
        #region Public Events

        public event AnnotationCreatedEventHandler AnnotationCreated;
        public event AnnotationDeletedEventHandler AnnotationDeleted;

        #endregion

        #region Private Prop

        protected string _dfAnnotation = "dialog-form-annotation";
        protected string _sessionName { get { return ClientID; } }
        protected string _EditImage = "Edit_mini.png";
        IList<IAnnotationMutableObject> _annotations = null;
        private EndPointElement _epe;

        #endregion

        #region Public Prop

        public IList<IAnnotationMutableObject> AnnotationObjectList
        {
            get
            {
                GetAnnotationsFromSession();
                return _annotations;
            }
            set
            {
                _annotations = value;
                SetAnnotationsToSession();
                this.BindData();
            }
        }

        public bool EditMode
        {
            get
            {
                return (Session[_sessionName + "_editmode"] != null) ? (bool)Session[_sessionName + "_editmode"] : false;
            }
            set
            {

                Session[_sessionName + "_editmode"] = value;
                SetEditForm((bool)Session[_sessionName + "_editmode"]);

                if (value)
                {
                    _EditImage = "Edit_mini.png";

                }
                else
                {
                    _EditImage = "Details2.png";
                }
            }
        }

        public int GetCurrentIndex
        {
            get
            {
                return (Session[_sessionName + "_index"] != null) ? (int)Session[_sessionName + "_index"] : -1;
            }
            set
            {
                Session[_sessionName + "_index"] = value;
            }
        }

        public string PopUpContainer { get; set; }
        public int PopUpContainerWidth { get; set; }
        public string AddText_ucOpenTabName { get; set; }
        public string AddText_ucOpenPopUpName { get; set; }
        public int AddText_ucOpenPopUpWidth { get; set; }
        public string OwnerAgency { get; set; }

        #endregion

        #region Public Methods

        public void ClearAnnotationsSession()
        {
            if (Session[_sessionName] != null)
            {
                Session[_sessionName] = null;
                if (_annotations != null)
                    _annotations.Clear();
            }

            ClearControlInput();
            GetCurrentIndex = -1;

        }

        #endregion


        protected virtual void OnAnnotationCreated(IAnnotationMutableObject e)
        {
            if (AnnotationCreated != null)
                AnnotationCreated(this, e);
        }

        protected virtual void OnAnnotationDeleted(IAnnotationMutableObject e)
        {
            if (AnnotationDeleted != null)
                AnnotationDeleted(this, e);
        }
        
        #region Private Methods

        private IAnnotationMutableObject CreateAnnotation(string id, string title,
                                                        string type, string uri, IList<ITextTypeWrapperMutableObject> texts)
        {

            IAnnotationMutableObject annotation = new AnnotationMutableCore();

            annotation.Id = id;
            annotation.Title = title;
            annotation.Type = type;
            annotation.Uri = (uri != string.Empty) ? new Uri(uri) : null;

            foreach (ITextTypeWrapperMutableObject text in texts)
                annotation.AddText(text);

            return annotation;
        }

        private void SetEditForm(bool bView)
        {
            btnAddNewAnnotation.Visible = bView;
            btnUpdateAnnotation.Visible = (bView && GetCurrentIndex >= 0) ? bView : false;

            gridView.Columns[gridView.Columns.Count - 1].Visible = bView;
            pnlNewAnnotation.Visible = bView;
            //txt_id_annotation.Enabled = bView;
            txt_title_annotation.Enabled = bView;
            txt_type_annotation.Enabled = bView;
            txt_url_annotation.Enabled = bView;

            AddText_value_annotation.ucEditMode = bView;
            AddText_value_annotation.ucEnableSuggest = true;

            /*if ( bView == false )
            {
                _EditImage = "Details2.png";
            }
            else
            {                
                _EditImage = "Edit_mini.png";
            }*/
        }

        private void GetAnnotationsFromSession()
        {
            if (Session[_sessionName] != null)
            {
                _annotations = (IList<IAnnotationMutableObject>)Session[_sessionName];
            }
            else
            {
                _annotations = new List<IAnnotationMutableObject>();
            }
        }

        private void SetAnnotationsToSession()
        {
            Session[_sessionName] = _annotations;

            this.BindData();
        }

        private void ClearControlInput()
        {
            txt_id_annotation.Text = string.Empty;
            txt_title_annotation.Text = string.Empty;
            txt_type_annotation.Text = string.Empty;
            txt_url_annotation.Text = string.Empty;

            if (cmbAnnotationType.SelectedIndex > 0)
                cmbAnnotationType.SelectedIndex = 0;

            AddText_value_annotation.ClearTextObjectListWithOutJS();
        }

        private bool ValidateForm(bool SkipDuplicate = false)
        {
            string id = txt_id_annotation.Text;
            string title = txt_title_annotation.Text;
            string type = txt_type_annotation.Text;
            string uri = txt_url_annotation.Text;
            IList<ITextTypeWrapperMutableObject> values = AddText_value_annotation.TextObjectList;

            bool error = false;
            int errorCounter = 0;
            string sError = String.Empty;

            #region Validation
            /*if (!ValidationUtils.CheckIdFormat(id))
            {
                errorCounter++;
                error = true;
                sError += string.Format( "{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_id_format );
            }

            IEnumerable<IAnnotationMutableObject> annotations = (from c in this._annotations where c.Id == id select c).OfType<IAnnotationMutableObject>();

            if (!EditMode && annotations.Count() > 0)
            {
                errorCounter++;
                error = true;
                sError += string.Format( "{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_id_exist );
            }

            if (title == string.Empty)
            {
                errorCounter++;
                error = true;
                sError += string.Format( "{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_title_format );
            }

            if (type == string.Empty)
            {
                errorCounter++;
                error = true;
                sError += string.Format( "{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_type_format );
            }
             */
            if (!ValidationUtils.CheckUriFormat(uri))
            {
                errorCounter++;
                error = true;
                sError += string.Format("{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_uri_format);
            }

            if (values == null)
            {
                errorCounter++;
                error = true;
                sError += string.Format("{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_list_value_format);
            }

            if (!SkipDuplicate && _annotations.Where(ann => ann.Id != null && ann.Id.ToUpper() == txt_id_annotation.Text.ToUpper()).FirstOrDefault() != null)
            {
                errorCounter++;
                error = true;
                sError += string.Format("{0}) {1}<br /><br />", errorCounter.ToString(), Resources.Messages.err_duplicate_id);
            }

            #endregion

            if (sError != string.Empty)
            {
                OpenPopUpContainer();
                OpenAddAnnotationPopUp();
                Utils.ShowDialog(sError, 300, Resources.Messages.err_title);
            }
            return !error;
        }

        private void BindData()
        {
            if (_annotations == null)
            {
                gridView.DataBind();
                return;
            }

            ISTATUtils.LocalizedUtils localUtils = new ISTATUtils.LocalizedUtils(Utils.LocalizedCulture);
            ISTAT.EntityMapper.EntityMapper eMapper = new ISTAT.EntityMapper.EntityMapper(Utils.LocalizedLanguage);

            IList<ISTAT.Entity.Annotation> lAnnoations = new List<ISTAT.Entity.Annotation>();
            foreach (IAnnotationMutableObject annotation in _annotations)
            {
                if (annotation.Type == null || ((annotation.Type != null) && (annotation.Type.Trim() != "@ORDER@" && annotation.Type.Trim() != "CategoryScheme_node_order" && annotation.Type.Trim() != "NonProductionDataflow")))
                {
                    // Valore standard per le codelist immesso automaticmante
                    lAnnoations.Add(new ISTAT.Entity.Annotation(
                        annotation.Id,
                        annotation.Title,
                        annotation.Type,
                        (annotation.Uri != null) ? annotation.Uri.AbsoluteUri : string.Empty,
                        localUtils.GetLocalizedText(annotation.Text)));
                }
            }
            gridView.DataSource = lAnnoations.OrderBy(ann => ann.ID).ToList();
            gridView.DataBind();
        }

        private void OpenPopUpContainer()
        {
            int puWidth;

            if (PopUpContainerWidth != 0)
                puWidth = PopUpContainerWidth;
            else
                puWidth = 700;

            if (!string.IsNullOrEmpty(PopUpContainer))
                Utils.AppendScript("openP('" + PopUpContainer + "'," + puWidth + ");");
        }

        private void OpenAddAnnotationPopUp()
        {
            Utils.AppendScript("openP('" + _dfAnnotation + _sessionName + "',600);");
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _epe = (EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT];

            if (!Page.IsPostBack)
            {
                ClearControlInput();
            }

            lblTitle.DataBind();
            lbl_id_annotation.DataBind();
            lbl_title_annotation.DataBind();
            lbl_type_annotation.DataBind();
            lbl_url_annotation.DataBind();
            lbl_value_annotation.DataBind();
            btnAddNewAnnotation.DataBind();
            btnUpdateAnnotation.DataBind();
            btnAddNewAnnotation.Visible = true;
            //btnUpdateAnnotation.Visible = false;

            if (GetCurrentIndex >= 0)
            {
                btnAddNewAnnotation.Visible = false;
                //btnUpdateAnnotation.Visible = true;
            }

            //AddText_value_annotation.ClearTextObjectListWithOutJS();
            AddText_value_annotation.ucOpenTabName = AddText_ucOpenTabName;
            AddText_value_annotation.ucOpenPopUpWidth = 400;
            AddText_value_annotation.ucOpenPopUpName = _dfAnnotation + _sessionName;
            AddText_value_annotation.ucOpenPopUpName2 = AddText_ucOpenPopUpName;
            AddText_value_annotation.ucOpenPopUpName3 = this.PopUpContainer;
            AddText_value_annotation.ucEnableSuggest = true;

            GetAnnotationsFromSession();
            if (_annotations == null) _annotations = new List<IAnnotationMutableObject>();

            if (!Page.IsPostBack)
            {
                if (_epe.EnableAnnotationSuggest)
                {
                    cmbAnnotationType.Visible = true;
                    Utils.PopulateCmbAnnotationType(cmbAnnotationType);
                    txt_type_annotation.TextMode = TextBoxMode.MultiLine;
                    txt_type_annotation.Height = 50;
                }

                BindData();
            }
        }

        protected void cmbAnnotationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAnnotationType.SelectedValue != "")
            {
                txt_type_annotation.Text = cmbAnnotationType.SelectedValue;
            }
            OpenPopUpContainer();
            OpenAddAnnotationPopUp();
        }

        protected void OnNewClick(object sender, EventArgs e)
        {
            if (txt_id_annotation.Text.Trim().Equals(string.Empty))
            {
                OpenPopUpContainer();
                OpenAddAnnotationPopUp();
                Utils.ShowDialog(Resources.Messages.err_missing_id);
                return;
            }
            string id = txt_id_annotation.Text;
            string title = txt_title_annotation.Text;
            string type = txt_type_annotation.Text;
            string uri = txt_url_annotation.Text;
            IList<ITextTypeWrapperMutableObject> values = AddText_value_annotation.TextObjectList;
            
            IAnnotationMutableObject annotation = null;

            if (ValidateForm())
            {
                annotation = CreateAnnotation(id, title, type, uri, values);
                this._annotations.Add(annotation);
                SetAnnotationsToSession();
                ClearControlInput();
                Utils.RemoveLatestPopup();
            }

            OpenPopUpContainer();
            OnAnnotationCreated(annotation);
        }

        protected void OnUpdateClick(object sender, EventArgs e)
        {
            string id = txt_id_annotation.Text;
            string title = txt_title_annotation.Text;
            string type = txt_type_annotation.Text;
            string uri = txt_url_annotation.Text;
            IList<ITextTypeWrapperMutableObject> values = AddText_value_annotation.TextObjectList;

            if (ValidateForm(true))
            {

                if (GetCurrentIndex >= 0)
                {
                    IAnnotationMutableObject annotation = CreateAnnotation(id, title, type, uri, values);
                    IAnnotationMutableObject foundAnnotation = this._annotations.Where(tmpAnn => tmpAnn.Id != null && tmpAnn.Id.Equals(id)).First();

                    if (foundAnnotation != null)
                    {
                        this._annotations.Remove(foundAnnotation);
                    }

                    if (this._annotations.Count == 0)
                    {
                        this._annotations.Add(annotation);
                    }
                    else
                    {
                        this._annotations.Insert(GetCurrentIndex, annotation);
                    }

                    SetAnnotationsToSession();

                }
                GetCurrentIndex = -1;
                ClearControlInput();
                Utils.RemoveLatestPopup();
            }

            OpenPopUpContainer();
            //Utils.ForceBlackClosing();
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridView.PageSize = 12;
            gridView.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "UPDATE":
                    {
                        txt_id_annotation.Enabled = false;
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        GetCurrentIndex = gvr.RowIndex;
                        string id = ((Label)gvr.Cells[1].FindControl("lbl_id")).Text;
                        var foundAnnotation = (from ann in this._annotations
                                               where ann.Id != null && ann.Id.Equals(id)
                                               select ann).FirstOrDefault();

                        if (foundAnnotation == null) return;
                        txt_id_annotation.Text = foundAnnotation.Id;
                        txt_title_annotation.Text = foundAnnotation.Title;
                        txt_type_annotation.Text = foundAnnotation.Type;
                        txt_url_annotation.Text = (foundAnnotation.Uri != null) ? foundAnnotation.Uri.ToString() : string.Empty;

                        btnAddNewAnnotation.Visible = false;
                        btnUpdateAnnotation.Visible = EditMode;

                        AddText_value_annotation.ClearTextObjectListWithOutJS();
                        AddText_value_annotation.InitTextObjectMutableList = foundAnnotation.Text;
                        OpenPopUpContainer();
                        OpenAddAnnotationPopUp();

                    } break;
                case "DELETE":
                    {
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        if (this._annotations.Count < gvr.RowIndex) return;
                        string id = ((Label)(gridView.Rows[gvr.RowIndex].Cells[1].Controls[1])).Text;
                        IAnnotationMutableObject foundAnnotation = this._annotations.Where(ann => ann.Id != null && ann.Id.Equals(id)).First();
                        this._annotations.Remove(foundAnnotation);
                        SetAnnotationsToSession();
                        OnAnnotationDeleted(foundAnnotation);
                    } break;
            }

            OpenPopUpContainer();
        }

        protected void btnNewAnnotation_Click(object sender, ImageClickEventArgs e)
        {
            txt_id_annotation.Enabled = true;
            btnAddNewAnnotation.Visible = true;
            btnUpdateAnnotation.Visible = false;
            ClearControlInput();
            GetCurrentIndex = -1;

            OpenPopUpContainer();

            OpenAddAnnotationPopUp();
        }

        #endregion

        protected void gridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            ImageButton imgBtn = e.Row.FindControl("img_update") as ImageButton;
            if (imgBtn != null)
            {
                if (Session[SESSION_KEYS.USER_OK] != null && (bool)Session[SESSION_KEYS.USER_OK] == true)
                {
                    User tmpUser = Session[SESSION_KEYS.USER_DATA] as User;
                    if (tmpUser != null)
                    {
                        if (tmpUser.agencies.Where(agency => agency.id == OwnerAgency).ToList().Count != 0)
                        {
                            imgBtn.ImageUrl = "~/images/Edit_mini.png";
                        }
                        else
                        {
                            imgBtn.ImageUrl = "~/images/Details2.png";
                        }
                    }
                    else
                    {
                        imgBtn.ImageUrl = "~/images/Details2.png";
                    }
                }
                else
                {
                    imgBtn.ImageUrl = "~/images/Details2.png";
                }
            }
        }


    }
}