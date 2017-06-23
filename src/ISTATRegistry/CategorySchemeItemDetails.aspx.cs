using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.IO;

using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;

using ISTATUtils;
using ISTAT.EntityMapper;
using ISTAT.Entity;
using ISTAT.WSDAL;
using ISTATRegistry.IRServiceReference;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;

namespace ISTATRegistry
{

    public partial class categorySchemeItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {
        private const string imgDeletePath = "~/images/Trash-50.png";
        private const string imgDeleteDisablePath = "~/images/Trash-50 - disable.png";
        private const string imgUpdatePath = "~/images/Edit-50.png";
        private const string imgUpdateDisablePath = "~/images/Edit-50 - disable.png";
        private const string imgMovePath = "~/images/Cut-50.png";
        private const string imgMoveDisablePath = "~/images/Cut-50 - disable.png";
        private const string imgDeselectPath = "~/images/Cancel-50.png";
        private const string imgDeselectDisablePath = "~/images/Cancel-50 - disable.png";
        private const string imgCancelMovePath = "~/images/Cancel File-50.png";
        private const string imgCancelMoveDisablePath = "~/images/Cancel File-50 - disable.png";
        private const string imgAddPath = "~/images/Plus-50.png";
        private const string imgAddDisablePath = "~/images/Plus-50 - disable.png";
        
        public struct csvCategory
        {
            public string category;
            public string name;
            public string description;
            public string parentCategory;

            public csvCategory(string category, string name, string description, string parentCategory)
            {
                this.category = category;
                this.name = name;
                this.description = description;
                this.parentCategory = parentCategory;
            }
        }

        public static string KEY_PAGE_SESSION = "TempCategoryscheme";

        ArtefactIdentity _artIdentity;
        Action _action;
        ISdmxObjects _sdmxObjects;
        LocalizedUtils _localizedUtils;
        EndPointElement _epe;

        
        protected string AspConfirmationExit = "false";

        private void SetAction()
        {
            if (Request["ACTION"] == null || Utils.ViewMode )
                _action = Action.VIEW;
            else
            {
                if ( Request["ACTION"] == "UPDATE" )
                {
                    IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;
                    _artIdentity = Utils.GetIdentityFromRequest(Request);

                    if (currentUser != null)
                    {
                        int agencyOccurence = currentUser.agencies.Count(agency => agency.id.Equals(_artIdentity.Agency));
                        if (agencyOccurence > 0)
                        {
                            _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                        }
                        else
                        {
                            _action = Action.VIEW;
                        }
                    }
                    else
                        _action = Action.VIEW;
                }
                else
                {
                    _action = (Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString());
                }
            }
        }

        private ICategorySchemeMutableObject GetCategoryschemeForm(bool errorBypass = false)
        {
            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region CATEGORYSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_id_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME AGENCY
            if (cmbAgencies.Text.Trim().Equals(string.Empty))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_version_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region CATEGORYSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_uri_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_list_name_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME START END DATE
            bool checkForDatesCombination = true;

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidFrom.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_date_from_format);
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidTo.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidTo.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_date_to_format);
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !txtValidTo.Text.Trim().Equals(string.Empty))
            {
                // Controllo congruenza date
                if (checkForDatesCombination)
                {
                    if (!ValidationUtils.CheckDates(txtValidFrom.Text, txtValidTo.Text))
                    {
                        messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_date_diff);
                        errorCounter++;
                        isInError = true;
                    }
                }
            }
            #endregion



            if (isInError && errorBypass)
                return null;
            else if (isInError)
            {
                Utils.ShowDialog(messagesGroup, 300);
                return null;
            }

            ICategorySchemeMutableObject tmpCategoryscheme = new CategorySchemeMutableCore();
            #region CREATE CATEGORY FROM FORM

            tmpCategoryscheme.AgencyId = GetAgencyValue();
            tmpCategoryscheme.Id = txtDSDID.Text;
            tmpCategoryscheme.Version = txtVersion.Text;
            tmpCategoryscheme.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            tmpCategoryscheme.Uri = (!txtDSDURI.Text.Trim().Equals(string.Empty) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                tmpCategoryscheme.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpCategoryscheme.StartDate = null;				
			}
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                tmpCategoryscheme.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpCategoryscheme.EndDate = null;				
			}
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                tmpCategoryscheme.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    tmpCategoryscheme.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var tmpAnnotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    tmpCategoryscheme.AddAnnotation(tmpAnnotation);
                }
            #endregion

            return tmpCategoryscheme;
        }

        private string GetAgencyValue()
        {
            if (_action == Action.INSERT)
            {
                string agencyValue = cmbAgencies.SelectedValue.ToString();
                string agencyId = agencyValue.Split('-')[0].Trim();
                return agencyId;
            }
            else
            {
                return txtAgenciesReadOnly.Text;
            }
        }

        private ICategorySchemeMutableObject GetCategoryschemeForm(ICategorySchemeMutableObject cs, bool errorBypass = false)
        {

            if (cs == null) return GetCategoryschemeForm();

            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region CATEGORYSCHEME ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_id_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME AGENCY
            if (cmbAgencies.Text.Trim().Equals(string.Empty))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME VERSION
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_version_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region CATEGORYSCHEME URI
            if ((txtDSDURI.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txtDSDURI.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_uri_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_list_name_format);
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORYSCHEME START END DATE
            bool checkForDatesCombination = true;

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidFrom.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_date_from_format);
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidTo.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txtValidTo.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_date_to_format);
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txtValidFrom.Text.Trim().Equals(string.Empty) && !txtValidTo.Text.Trim().Equals(string.Empty))
            {
                // Controllo congruenza date
                if (checkForDatesCombination)
                {
                    if (!ValidationUtils.CheckDates(txtValidFrom.Text, txtValidTo.Text))
                    {
                        messagesGroup += Convert.ToString(errorCounter) + string.Format(") {0}<br /><br />", Resources.Messages.err_date_diff);
                        errorCounter++;
                        isInError = true;
                    }
                }
            }
            #endregion

            if (isInError && errorBypass)
                return null;
            else if (isInError)
            {
                Utils.ShowDialog(messagesGroup, 300);
                return null;
            }

            #region CREATE CATEGORYSCHEME FROM FORM

            cs.AgencyId = GetAgencyValue();
            cs.Id = txtDSDID.Text;
            cs.Version = txtVersion.Text;
            cs.FinalStructure = TertiaryBool.ParseBoolean(chkIsFinal.Checked);
            cs.Uri = (!txtDSDURI.Text.Trim().Equals(string.Empty) && ValidationUtils.CheckUriFormat(txtDSDURI.Text)) ? new Uri(txtDSDURI.Text) : null;
            if (!txtValidFrom.Text.Trim().Equals(string.Empty))
            {
                cs.StartDate = DateTime.ParseExact(txtValidFrom.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				cs.StartDate = null;
			}
            if (!txtValidTo.Text.Trim().Equals(string.Empty))
            {
                cs.EndDate = DateTime.ParseExact(txtValidTo.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				cs.EndDate = null;
			}
            if (cs.Names.Count != 0)
            {
                cs.Names.Clear();
            }
            if (cs.Descriptions.Count != 0)
            {
                cs.Descriptions.Clear();
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                cs.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    cs.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (cs.Annotations.Count != 0)
            {
                cs.Annotations.Clear();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var tmpAnnotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    cs.AddAnnotation(tmpAnnotation);
                }
            #endregion

            return cs;
        }

        private ICategorySchemeMutableObject DeleteCategoryInCategoryscheme(ICategorySchemeMutableObject cs, string selectedNodePathValue, string selectedParentNodePathValue)
        {
            string[] elements = selectedNodePathValue.Split('/');

            ICategoryMutableObject currentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));

            ICategoryMutableObject currentParentNodeInCategoryScheme = null;
            string[] elementsParentNode = null;

            if (!selectedParentNodePathValue.Trim().Equals(string.Empty))
            {
                elementsParentNode = selectedParentNodePathValue.Split('/');
                currentParentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));
            }

            if (elementsParentNode.Length > 1)
            {
                for (int i = 2; i < elements.Length; i++)
                {
                    currentNodeInCategoryScheme = currentNodeInCategoryScheme.Items.First(obj => obj.Id.Equals(elements[i]));
                }

                for (int i = 2; i < elementsParentNode.Length; i++)
                {
                    currentParentNodeInCategoryScheme = currentParentNodeInCategoryScheme.Items.First(obj => obj.Id.Equals(elementsParentNode[i]));
                }

                currentParentNodeInCategoryScheme.Items.Remove(currentNodeInCategoryScheme);
            }
            else
            {
                cs.Items.Remove(currentNodeInCategoryScheme);
            }

            try
            {
                // Ultimo controllo se ottengo Immutable istanze validazione completa
                var canRead = cs.ImmutableInstance;
            }
            catch (Exception ex)
            {
                //parentCategory.Items.Remove( category );

                return null;

            }

            return cs;
        }
        private ICategorySchemeMutableObject InsertCategoryInCategoryscheme(ICategorySchemeMutableObject cs)
        {
            if (cs == null) return null;

            ICategoryMutableObject category = new CategoryMutableCore();

            string category_id = txtNewCategoryId.Text.Trim();

            IList<ITextTypeWrapperMutableObject> category_names = NewCategoryAddTextName.TextObjectList;
            IList<ITextTypeWrapperMutableObject> category_descs = NewCategoryAddTextDescription.TextObjectList;
            //string category_parent_id = txtParentCategoryNewCategory.Text.Trim();
            // string code_order_str = txtOrderNewCode.Text.Trim();     ----- ORDINE

            #region CATEGORY ID
            if (ValidationUtils.CheckIdFormat(category_id))
            {
                category.Id = category_id;
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_format;
                Utils.AppendScript("openPopUp('df-Dimension', 600);");
                Utils.AppendScript("location.href= '#categories';");
                return null;
            }

            IEnumerable<ICategoryMutableObject> categories = (from c in cs.Items where c.Id == category_id select c).OfType<ICategoryMutableObject>();
            if (categories.Count() > 0)
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_id_exist;
                Utils.AppendScript("openPopUp('df-Dimension', 600);");
                Utils.AppendScript("location.href= '#categories';");
                return null;
            }
            #endregion

            #region CODE NAMES
            if (category_names != null)
            {
                foreach (var tmpName in category_names)
                {
                    category.AddName(tmpName.Locale, tmpName.Value);
                }
            }
            else
            {
                lblErrorOnNewInsert.Text = Resources.Messages.err_list_name_format;
                Utils.AppendScript("openPopUp('df-Dimension', 600);");
                Utils.AppendScript("location.href= '#categories';");
                return null;
            }
            #endregion

            #region CATEGORY DESCRIPTIONS
            if (category_descs != null)
            {
                foreach (var tmpDescription in category_descs)
                {
                    category.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            }
            #endregion

            #region CATEGORY ANNOTATIONS

            if (AnnotationNewControl.AnnotationObjectList != null)
                foreach (var currentAnnotation in AnnotationNewControl.AnnotationObjectList)
                {
                    category.Annotations.Add(currentAnnotation);
                }

            #endregion

            #region PARANT ID

            if (TreeView1.SelectedNode != null && !TreeView1.Nodes[0].Selected)
            {

                string nodesPath = TreeView1.SelectedNode.ValuePath.ToString();
                string[] elements = nodesPath.Split('/');
                ICategoryMutableObject currentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));

                for (int i = 2; i < elements.Length; i++)
                {
                    currentNodeInCategoryScheme = currentNodeInCategoryScheme.Items.First(obj => obj.Id.Equals(elements[i]));
                }
                currentNodeInCategoryScheme.Items.Add(category);
            }
            else
            {
                cs.Items.Add(category);
            }
            #endregion

            try
            {
                // Ultimo controllo se ottengo Immutable istanze validazione completa
                var canRead = cs.ImmutableInstance;
            }
            catch (Exception ex)
            {
                //parentCategory.Items.Remove( category );

                return null;

            }

            txtNewCategoryId.Text = string.Empty;
            NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
            NewCategoryAddTextName.ClearTextObjectListWithOutJS();
            lblErrorOnNewInsert.Text = string.Empty;
            AnnotationNewControl.ClearAnnotationsSession();
            return cs;

        }
        private ICategorySchemeMutableObject GetCategorySchemeFromSession()
        {
            try
            {
                if (Session[KEY_PAGE_SESSION] == null)
                {
                    if (_artIdentity.ToString() != string.Empty)
                    {
                        WSModel wsModel = new WSModel();
                        ISdmxObjects sdmxObject = wsModel.GetCategoryScheme(_artIdentity, false, false);
                        ICategorySchemeObject cs = sdmxObject.CategorySchemes.FirstOrDefault();
                        Session[KEY_PAGE_SESSION] = cs.MutableInstance;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                return (ICategorySchemeMutableObject)Session[KEY_PAGE_SESSION];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveInMemory(ICategorySchemeMutableObject cs)
        {
            if (cs == null) return false;

            Session[KEY_PAGE_SESSION] = cs;

            return true;
        }
        private bool SendQuerySubmit(ICategorySchemeMutableObject cs)
        {

            try
            {

                ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

                for (int i = 0; i < cs.Annotations.Count; )
                    if (cs.Annotations[i].Type.Trim() == "CategoryScheme_node_order")
                        cs.Annotations.RemoveAt(i);
                    else i++;

                sdmxObjects.AddCategoryScheme(cs.ImmutableInstance);

                WSModel modelCategoryScheme = new WSModel();

                XmlDocument result = modelCategoryScheme.SubmitStructure(sdmxObjects);

                Utils.GetXMLResponseError(result);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private void ClearSessionPage()
        {
            Session[KEY_PAGE_SESSION] = null;
        }

        #region Event Handler

        protected void Page_Load(object sender, EventArgs e)
        {
            // TEMPORARY LOG
            TemporaryLog("In caricamento pagina");

            _epe = (EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT];

            //Modifca UserControlCSV
            CSVImporter1.OperationComplete += CSVImporter1_OperationComplete;


            lblDSDID.DataBind();
            lblVersion.DataBind();
            lblAgency.DataBind();
            lblValidFrom.DataBind();
            lblValidTo.DataBind();
            lblDSDURI.DataBind();
            lblDSDURN.DataBind();
            lblIsFinal.DataBind();
            lblDSDName.DataBind();
            lblDSDDescription.DataBind();
            lblTitle.DataBind();
            lblNewCategoryId.DataBind();
            lblNewCategoryName.DataBind();
            lblNewCategoryDescription.DataBind();
            //lblParentCategoryNewCategory.DataBind();
            btnNewCategory.DataBind();
            lblTitleUpdateCategory.DataBind();
            lblUpdateCategoryID.DataBind();
            lblUpdateAnnotation.DataBind();
            lblUpdateCategoryName.DataBind();
            lblUpdateCategoryDescription.DataBind();
            lbl_annotation.DataBind();
            lblNewAnnotation.DataBind();
            //lblUpdateCategoryParentID.DataBind();
            btnUpdateCategory.DataBind();
            btnAddNewCategory.DataBind();
            btnSaveMemoryCategoryScheme.DataBind();
            btnDeleteCategory.DataBind();
            btnUpdateSelectedCategory.DataBind();
            lblImportCsvTitle.DataBind();
            lblCsvLanguage.DataBind();
            lblcsvFile.DataBind();
            btnImportFromCsv.DataBind();
            btnCancelMoveCategory.DataBind();
            lblCategoryOrder.DataBind();
            btnDeselectCategory.DataBind();
            btnClearFieldForUpdate.DataBind();
            btnClearFields.DataBind();
            lblSeparator.DataBind();
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);

            SetAction();

            if (!IsPostBack)
            {
                lblMoveInstructions.DataBind();
                btnMoveCategory.DataBind();

                if(!_epe.PartialArtefact)
                    Utils.PopulateCmbAgencies(cmbAgencies, true);

                ClearSessionPage();
                AnnotationNewControl.ClearAnnotationsSession();
                AnnotationUpdateControl.ClearAnnotationsSession();
            }

            ICategorySchemeMutableObject cs;

            switch (_action)
            {
                case Action.INSERT:
                    AspConfirmationExit = "true";

                    if (!Page.IsPostBack)
                    {
                        ClearSessionPage();
                    }

                    SetInitControls();
                    SetInsertForm();

                    chkIsFinal.Checked = false;
                    chkIsFinal.Enabled = false;
                    if (!Page.IsPostBack)
                    {
                        cmbAgencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                        cmbAgencies.SelectedIndex = 0;
                        FileDownload31.Visible = false;
                    }

                    cs = GetCategorySchemeFromSession();
                    if (cs == null) cs = GetCategoryschemeForm(true);
                    else cs = GetCategoryschemeForm(cs);

                    if (cs == null)
                    {
                        cs = new CategorySchemeMutableCore();
                        cs.Id = "@fitt@";
                        cs.AgencyId = "@fitt@";
                        cs.Version = "1.0";
                        cs.AddName("it", "@fitt@");
                    }

                    CSVImporter1.ucCategoryScheme = cs;
                    CSVImporter1.ucTabName = "categories";

                    break;
                case Action.UPDATE:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);

                    SetInitControls();
                    SetEditForm();

                    bool b = (bool)_artIdentity.IsFinal;

                    cs = GetCategorySchemeFromSession();
                    if (cs == null) cs = GetCategoryschemeForm(b);
                    else cs = GetCategoryschemeForm(cs, b);
                    CSVImporter1.ucCategoryScheme = cs;
                    CSVImporter1.ucTabName = "categories";


                    break;
                case Action.VIEW:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);

                    ClearSessionPage();

                    SetViewForm();

                    FileDownload31.ucID = _artIdentity.ID;
                    FileDownload31.ucAgency = _artIdentity.Agency;
                    FileDownload31.ucVersion = _artIdentity.Version;
                    FileDownload31.ucArtefactType = "CategoryScheme";

                    break;
            }

            NewCategoryAddTextName.ucOpenTabName = "categories";
            NewCategoryAddTextName.ucOpenPopUpWidth = 600;
            NewCategoryAddTextName.ucOpenPopUpName = "df-Dimension";

            NewCategoryAddTextDescription.ucOpenTabName = "categories";
            NewCategoryAddTextDescription.ucOpenPopUpWidth = 600;
            NewCategoryAddTextDescription.ucOpenPopUpName = "df-Dimension";

            AddText1.ucOpenTabName = "categories";
            AddText1.ucOpenPopUpWidth = 600;
            AddText1.ucOpenPopUpName = "df-Dimension-update";

            AddText2.ucOpenTabName = "categories";
            AddText2.ucOpenPopUpWidth = 600;
            AddText2.ucOpenPopUpName = "df-Dimension-update";

            AnnotationNewControl.AddText_ucOpenTabName = "categories";
            AnnotationNewControl.AddText_ucOpenPopUpWidth = 600;
            AnnotationNewControl.AddText_ucOpenPopUpName = AnnotationNewControl.ClientID;
            AnnotationNewControl.PopUpContainer = "df-Dimension";

            AnnotationUpdateControl.AddText_ucOpenTabName = "categories";
            AnnotationUpdateControl.AddText_ucOpenPopUpWidth = 600;
            AnnotationUpdateControl.AddText_ucOpenPopUpName = AnnotationUpdateControl.ClientID;
            AnnotationUpdateControl.PopUpContainer = "df-Dimension-update";

            if (chkIsFinal.Checked || _action == Action.VIEW)
            {
                btnAddNewCategory.Visible = false;
                btnCancelMoveCategory.Visible = false;
                btnDeleteCategory.Visible = false;
                btnDeselectCategory.Visible = false;
                btnImportFromCsv.Visible = false;
                btnUpdateSelectedCategory.Visible = false;
                btnMoveCategory.Visible = false;
                txtCategoryOrder.Visible = false;
                lblMoveInstructions.Visible = false;
                lblCategoryOrder.Visible = false;
            }
            else
            {
                btnAddNewCategory.Visible = true;
                btnCancelMoveCategory.Visible = true;
                btnDeleteCategory.Visible = true;
                btnDeselectCategory.Visible = true;
                btnImportFromCsv.Visible = true;
                btnUpdateSelectedCategory.Visible = true;
                btnMoveCategory.Visible = true;
                txtCategoryOrder.Visible = true;
                lblMoveInstructions.Visible = true;
                lblCategoryOrder.Visible = true;
            }

            if (!_epe.PartialArtefact)
            {
                DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.CategoryScheme;
                DuplicateArtefact1.ucMaintanableArtefact = GetCategorySchemeFromSession();
            }
            else
                DuplicateArtefact1.ucDisable = true;


        }

        //Modifca UserControlCSV
        protected void CSVImporter1_OperationComplete(object sender, object e)
        {
            ICategorySchemeMutableObject cs;
            ICategorySchemeMutableObject clCSV = (ICategorySchemeMutableObject)e;

            if (_action == Action.INSERT)
            {
                cs = GetCategorySchemeFromSession();
                if (cs == null) cs = GetCategoryschemeForm();
                else cs = GetCategoryschemeForm(cs);

                // Primo insert senza Item
                if (cs != null && cs.Items.Count == 0)
                {
                    List<ICategoryMutableObject> lItems = clCSV.Items.ToList();

                    foreach (ICategoryMutableObject code in lItems)
                    {
                        cs.Items.Add(code);
                    }
                }
            }
            else
                cs = clCSV;

            if (!SaveInMemory(cs)) return;

            BindData();
            //if (!errorInUploading)
            //{
            Utils.ShowDialog(Resources.Messages.succ_operation);
            //}
            Utils.AppendScript("location.href='#categories'");
        }




        protected void gvCategoryschemesItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            BindData();
            Utils.AppendScript("location.href= '#categories';");
        }

        protected void btnSaveMemoryCategoryScheme_Click(object sender, EventArgs e)
        {

            ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();
            if (cs == null) cs = GetCategoryschemeForm();
            else cs = GetCategoryschemeForm(cs);

            if (!SaveInMemory(cs)) return;

            if (!SendQuerySubmit(cs)) return;

            BindData();

            string successMessage = string.Empty;
            if (_action == Action.INSERT)
            {
                successMessage = Resources.Messages.succ_categoryscheme_insert;
            }
            else if (_action == Action.UPDATE)
            {
                successMessage = Resources.Messages.succ_categoryscheme_update;
            }
            Utils.ShowDialog(successMessage, 300, Resources.Messages.succ_operation);
            if ( _action == Action.INSERT )
            {
                Utils.AppendScript( "~/categoryschemes.aspx" );
            }
        }

        protected void btnAddNewCategory_Click(object sender, EventArgs e)
        {
            ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();

            cs = GetCategoryschemeForm(cs);

            // form codelist validation
            if (cs == null)
            {
                txtNewCategoryId.Text = string.Empty;
                NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
                NewCategoryAddTextName.ClearTextObjectListWithOutJS();
                AnnotationNewControl.ClearAnnotationsSession();
                AnnotationUpdateControl.ClearAnnotationsSession();
                return;
            }

            cs = InsertCategoryInCategoryscheme(cs);

            if (cs == null)
            {

                /*txtNewCategoryId.Text = string.Empty;
                NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
                NewCategoryAddTextName.ClearTextObjectListWithOutJS();
                AnnotationNewControl.ClearAnnotationsSession();
                AnnotationUpdateControl.ClearAnnotationsSession();*/
                txtNewCategoryId.Text = string.Empty;
                NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
                NewCategoryAddTextName.ClearTextObjectListWithOutJS();
                AnnotationNewControl.ClearAnnotationsSession();
                AnnotationNewControl.AnnotationObjectList = null;
                AnnotationUpdateControl.ClearAnnotationsSession();
                AnnotationUpdateControl.AnnotationObjectList = null;
                Utils.AppendScript("location.href= '#categories';");
                return;
            }

            if (!SaveInMemory(cs))
            {
                txtNewCategoryId.Text = string.Empty;
                NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
                NewCategoryAddTextName.ClearTextObjectListWithOutJS();
                AnnotationNewControl.ClearAnnotationsSession();
                AnnotationNewControl.AnnotationObjectList = null;
                AnnotationUpdateControl.ClearAnnotationsSession();
                AnnotationUpdateControl.AnnotationObjectList = null;
                return;
            }
            BindData();

            txtNewCategoryId.Text = string.Empty;
            NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
            NewCategoryAddTextName.ClearTextObjectListWithOutJS();
            AnnotationNewControl.ClearAnnotationsSession();
            AnnotationNewControl.AnnotationObjectList = null;
            AnnotationUpdateControl.ClearAnnotationsSession();
            AnnotationUpdateControl.AnnotationObjectList = null;
            Utils.AppendScript("location.href='#categories';");

        }

        protected void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            // TEMPORARY LOG
            TemporaryLog("In aggiornamento category");

            // Get Input field
            string category_id = txtUpdateCategoryID.Text.Trim();
            IList<ITextTypeWrapperMutableObject> category_names = AddText1.TextObjectList;
            IList<ITextTypeWrapperMutableObject> category_descs = AddText2.TextObjectList;
            // string category_parent_id = txtUpdateCategoryParentID.Text.Trim();

            // Get Current Object Session
            ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();
            cs = GetCategoryschemeForm( cs );
            string[] parentElements = TreeView1.SelectedNode.Parent.ValuePath.Split('/');
            string[] elements = TreeView1.SelectedNode.ValuePath.Split('/');
            ICategoryMutableObject currentParentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));
            ICategoryMutableObject currentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));

            for (int i = 2; i < elements.Length; i++)
            {
                currentNodeInCategoryScheme =
                    currentNodeInCategoryScheme.Items.First(
                    obj => obj.Id.Equals(elements[i]));
            }

            for (int i = 2; i < parentElements.Length; i++)
            {
                currentParentNodeInCategoryScheme =
                    currentParentNodeInCategoryScheme.Items.First(
                    obj => obj.Id.Equals(parentElements[i]));
            }

            ICategoryMutableObject _bCategory = new CategoryMutableCore();

            try
            {

                #region CATEGORY ID
                if (!category_id.Equals(string.Empty) && ValidationUtils.CheckIdFormat(category_id))
                {
                    _bCategory.Id = category_id;
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_id_format;
                    Utils.AppendScript("openPopUp('df-Dimension-update', 600 );");
                    Utils.AppendScript("location.href= '#categories';");
                    return;
                }
                #endregion

                #region CATEGORY NAMES
                if (category_names != null)
                {
                    foreach (var tmpName in category_names)
                    {
                        _bCategory.AddName(tmpName.Locale, tmpName.Value);
                    }
                }
                else
                {
                    lblErrorOnUpdate.Text = Resources.Messages.err_list_name_format;
                    Utils.AppendScript("openPopUp('df-Dimension-update', 600 );");
                    Utils.AppendScript("location.href= '#categories';");
                    return;
                }
                #endregion

                #region CATEGORY DESCRIPTIONS
                if (category_descs != null)
                {
                    foreach (var tmpDescription in category_descs)
                    {
                        _bCategory.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                    }
                }
                #endregion

                #region CATEGORY ITEMS

                foreach (var subCategory in currentNodeInCategoryScheme.Items)
                {
                    _bCategory.Items.Add(subCategory);
                }

                #endregion

                #region CATEGORY ANNOTATION

                foreach (var currentAnnotation in AnnotationUpdateControl.AnnotationObjectList)
                    _bCategory.Annotations.Add(currentAnnotation);


                #endregion

                if (parentElements.Length == 1)
                {
                    cs.Items.Remove(currentNodeInCategoryScheme);
                    cs.Items.Add(_bCategory);
                }
                else
                {
                    currentParentNodeInCategoryScheme.Items.Remove(currentNodeInCategoryScheme);
                    currentParentNodeInCategoryScheme.Items.Add(_bCategory);
                }


                var canRead = cs.ImmutableInstance;

            }
            catch (Exception ex) // ERRORE GENERICO!
            {
                currentParentNodeInCategoryScheme.Items.Remove(_bCategory);
                currentParentNodeInCategoryScheme.Items.Add(currentNodeInCategoryScheme);

                Utils.ShowDialog(Resources.Messages.err_category_update, 300, Resources.Messages.err_title);
                Utils.AppendScript("location.href='#categories';");

            }

            if (!SaveInMemory(cs))
                return;

            BindData();

            Utils.AppendScript("location.href='#categories';");
        }

        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {
            if ( !csvFile.HasFile )
            {
                Utils.AppendScript( "openPopUp( 'importCsv' );" );
                Utils.AppendScript( "location.href='#categories'" );
                Utils.AppendScript( string.Format( "alert( '{0}' );", Resources.Messages.err_no_file_uploaded ) );
                return;
            }

            ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();
            ICategoryMutableObject foundCategory = null;

            if (cs == null) return;

            List<csvCategory> categories = new List<csvCategory>();
            bool errorInUploading = false;

            string wrongRowsMessage = string.Empty;
            string wrongRowsMessageForUser = string.Empty;
            string wrongFileLines = string.Empty;

            try
            {
                string filenameWithoutExtension = string.Format("{0}_{1}_{2}", Path.GetFileName(csvFile.FileName).Substring(0, csvFile.FileName.Length - 4), Session.SessionID, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_'));
                string filename = string.Format("{0}.csv", filenameWithoutExtension);
                string logFilename = string.Format("{0}.log", filenameWithoutExtension);
                csvFile.SaveAs(Server.MapPath("~/csv_categoryschemes_files/") + filename);
                StreamReader reader = new StreamReader(Server.MapPath("~/csv_categoryschemes_files/") + filename);
                StreamWriter logWriter = new StreamWriter(Server.MapPath("~/csv_categoryschemes_import_logs/") + logFilename, true);
                logWriter.WriteLine(string.Format("LOG RELATIVO A CARICAMENTO DEL CATEGORY SCHEME [ ID => \"{0}\"  AGENCY_ID => \"{1}\"  VERSION => \"{2}\" ]  |  LINGUA SELEZIONATA: {3}\n", cs.Id.ToString(), cs.AgencyId.ToString(), cs.Version.ToString(), cmbLanguageForCsv.SelectedValue.ToString()));
                logWriter.WriteLine("-----------------------------------------------------------------------------------------------------------------------------\n");
                reader.ReadLine();
                
                int currentRow = 1;

                char separator = txtSeparator.Text.Trim().Equals( string.Empty ) ? ';' : txtSeparator.Text.Trim().ElementAt( 0 );

                while (!reader.EndOfStream)
                {
                    string  currentFileLine = reader.ReadLine();
                    string[] fields = currentFileLine.Split( separator );
                    if (fields.Length != 4)
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_line_bad_format, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_line_bad_format_gui, currentRow + 1);
                        wrongFileLines += string.Format( "{0}\n", currentFileLine );
                        logWriter.WriteLine(string.Format(Resources.Messages.err_csv_import_line_bad_format, currentRow + 1));
                        logWriter.Flush();
                        currentRow++;
                        continue;
                    }
                    if ( fields[0].Trim().Equals("\"\"") || fields[0].Trim().Equals(string.Empty))
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_id_missing, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_id_missing_gui, currentRow + 1);
                        wrongFileLines += string.Format( "{0}\n", currentFileLine );
                        logWriter.WriteLine(string.Format(Resources.Messages.err_csv_import_id_missing, currentRow + 1));
                        logWriter.Flush();
                        currentRow++;
                        continue;
                    }
                    if (fields[1].Trim().Equals("\"\"") || fields[1].Trim().Equals(string.Empty))
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_name_missing, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_name_missing_gui, currentRow + 1);
                        wrongFileLines += string.Format( "{0}\n", currentFileLine );
                        logWriter.WriteLine(string.Format(Resources.Messages.err_csv_import_name_missing, currentRow + 1));
                        logWriter.Flush();
                        currentRow++;
                        continue;
                    }
                    categories.Add(new csvCategory(fields[0].ToString().Replace("\"", ""), fields[1].ToString().Replace("\"", ""), fields[2].ToString().Replace("\"", ""), fields[3].ToString().Replace("\"", "")));
                    currentRow++;
                }
                if (!errorInUploading)
                {
                    logWriter.WriteLine("Andato tutto a buon fine con questo file!");
                }
                else
                {
                    lblImportCsvErrors.Text = wrongRowsMessageForUser;
                    Utils.AppendScript("openP('importCsvErrors',500);");
                }
                logWriter.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                Utils.AppendScript(string.Format("Upload status: The file could not be uploaded. The following error occured: {0}", ex.Message));
            }

            foreach (csvCategory category in categories)
            {
                if (category.parentCategory != string.Empty)
                {
                    string[] sequence = category.parentCategory.Split('.');
                    foundCategory = (ICategoryMutableObject)((from c in cs.Items
                                                              where c.Id.Equals(sequence[0])
                                                              select c).First());

                    for (int i = 1; i < sequence.Length; i++)
                    {
                        foundCategory = (ICategoryMutableObject)((from c in foundCategory.Items
                                                                  where c.Id.Equals(sequence[i])
                                                                  select c).First());
                    }

                    IEnumerable<ICategoryMutableObject> tmpCategories = null;
                    if (foundCategory != null)
                    {
                        tmpCategories = (from conc in foundCategory.Items where conc.Id == category.category select conc).OfType<ICategoryMutableObject>();
                    }
                    else
                    {
                        tmpCategories = (from conc in cs.Items where conc.Id == category.category select conc).OfType<ICategoryMutableObject>();
                    }

                    ICategoryMutableObject tmpCategory;

                    if (!(tmpCategories.Count() > 0))
                    {
                        tmpCategory = new CategoryMutableCore();
                        tmpCategory.Id = category.category;
                        tmpCategory.AddName(cmbLanguageForCsv.SelectedValue.ToString(), category.name);
                        tmpCategory.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), category.description);
                        if (foundCategory != null)
                        {
                            foundCategory.AddItem(tmpCategory);
                        }
                        else
                        {
                            cs.AddItem(tmpCategory);
                        }
                    }
                    else
                    {
                        tmpCategory = tmpCategories.First();
                        tmpCategory.Id = category.category;
                        tmpCategory.AddName(cmbLanguageForCsv.SelectedValue.ToString(), category.name);
                        tmpCategory.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), category.description);
                    }
                }
                else
                {
                    try
                    {
                        IEnumerable<ICategoryMutableObject> tmpCategories = (from conc in cs.Items where conc.Id == category.category select conc).OfType<ICategoryMutableObject>();
                        ICategoryMutableObject tmpCategory;

                        if (!(tmpCategories.Count() > 0))
                        {
                            tmpCategory = new CategoryMutableCore();
                            tmpCategory.Id = category.category;
                            tmpCategory.AddName(cmbLanguageForCsv.SelectedValue.ToString(), category.name);
                            tmpCategory.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), category.description);
                            cs.AddItem(tmpCategory);
                        }
                        else
                        {
                            tmpCategory = tmpCategories.First();
                            tmpCategory.Id = category.category;
                            tmpCategory.AddName(cmbLanguageForCsv.SelectedValue.ToString(), category.name);
                            tmpCategory.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), category.description);
                        }
                    }
                    catch (Exception ex)
                    {
                        // In caso di parent errato!
                        continue;
                    }

                }
            }

            if (!SaveInMemory(cs))
                return;

            BindData();
            if (!errorInUploading)
            {
                Utils.ShowDialog(Resources.Messages.succ_operation);
            }
            else
            {
                lblImportCsvErrors.Text = wrongRowsMessageForUser;
                lblImportCsvWrongLines.Text = wrongFileLines;
                Utils.AppendScript("openP('importCsvErrors',500);");
            }
            Utils.AppendScript("location.href='#categories';");
        }

        #endregion

        #region LAYOUT

        private void SetLabelDetail()
        {

            ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();

            if (cs == null)
                lblCategorySchemeDetail.Text = String.Format("({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
            else
            {

                lblCategorySchemeDetail.Text = String.Format("{3} ({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version, _localizedUtils.GetNameableName(cs.ImmutableInstance));
            }
        }

        private void CreateTreeWithRecursion(ICategoryObject category, TreeNode node)
        {
            LocalizedUtils localUtils = new LocalizedUtils(Utils.LocalizedCulture);
            if (category.Items.Count != 0)
            {
                int counter = 0;
                foreach (var subCategory in category.Items)
                {
                    TreeNode tmpNode = new TreeNode(string.Format("[ {0} ] {1}", subCategory.Id, localUtils.GetNameableName(subCategory)));
                    tmpNode.Value = subCategory.Id;
                    tmpNode.SelectAction = TreeNodeSelectAction.Select;
                    node.ChildNodes.Add(tmpNode);
                    CreateTreeWithRecursion(subCategory, node.ChildNodes[counter]);
                    counter++;
                }
            }
            else
            {
                //node.ChildNodes.Add( new TreeNode( category.Id ) );
                return;
            }
        }

        private void BindData(bool isNewItem = false)
        {
            ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();

            if (cs == null) return;

            SetGeneralTab(cs.ImmutableInstance);

            LocalizedUtils localUtils = new LocalizedUtils(Utils.LocalizedCulture);
            EntityMapper eMapper = new EntityMapper(Utils.LocalizedLanguage);

            TreeView1.Nodes.Clear();

            IList<Category> lCategorySchemeItem = new List<Category>();
            TreeNode rootNode = new TreeNode(string.Format("[ {0} ] {1}", cs.Id, localUtils.GetNameableName(cs.ImmutableInstance)));
            rootNode.Value = cs.Id;

            foreach (ICategoryObject category in cs.ImmutableInstance.Items)
            {
                //TreeNode node = new TreeNode( string.Format( "{0} - {1} - {2}", category.Id, localUtils.GetNameableName( category ), localUtils.GetNameableDescription( category ) ) );     
                TreeNode node = new TreeNode(string.Format("[ {0} ] {1}", category.Id, localUtils.GetNameableName(category)));
                node.Value = category.Id;
                node.SelectAction = TreeNodeSelectAction.Select;
                CreateTreeWithRecursion(category, node);
                rootNode.ChildNodes.Add(node);
                lCategorySchemeItem.Add(new Category(category.Id, localUtils.GetNameableName(category), localUtils.GetNameableDescription(category), (category.IdentifiableParent != null) ? category.IdentifiableParent.Id : string.Empty));
            }

            TreeView1.Nodes.Add(rootNode);
            rootNode.Expand();
        }

        private void SetGeneralTab(ICategorySchemeObject cs)
        {
            txtDSDID.Text = cs.Id;
            txtAgenciesReadOnly.Text = cs.AgencyId;
            txtVersion.Text = cs.Version;
            chkIsFinal.Checked = cs.IsFinal.IsTrue;

            FileDownload31.ucID = cs.Id;
            FileDownload31.ucAgency = cs.AgencyId;
            FileDownload31.ucVersion = cs.Version;
            FileDownload31.ucArtefactType = "CategoryScheme";

            txtDSDURI.Text = (cs.Uri != null) ? cs.Uri.AbsoluteUri : string.Empty;
            txtDSDURN.Text = (cs.Urn != null) ? cs.Urn.AbsoluteUri : string.Empty;
            txtValidFrom.Text = (cs.StartDate != null) ? string.Format("{0}/{1}/{2}", cs.StartDate.Date.Value.Day.ToString(), cs.StartDate.Date.Value.Month.ToString(), cs.StartDate.Date.Value.Year.ToString()) : string.Empty;
            txtValidTo.Text = (cs.EndDate != null) ? string.Format("{0}/{1}/{2}", cs.EndDate.Date.Value.Day.ToString(), cs.EndDate.Date.Value.Month.ToString(), cs.EndDate.Date.Value.Year.ToString()) : string.Empty;

            txtDSDName.Text = _localizedUtils.GetNameableName(cs);
            txtDSDDescription.Text = _localizedUtils.GetNameableDescription(cs);

            // Svuoto le griglie name e description
            //===========================================
            if (AddTextName.TextObjectList != null && AddTextName.TextObjectList.Count != 0)
            {
                AddTextName.ClearTextObjectList();
            }
            if (AddTextDescription.TextObjectList != null && AddTextDescription.TextObjectList.Count != 0)
            {
                AddTextDescription.ClearTextObjectList();
            }

            txtDSDID.Enabled = false;
            txtVersion.Enabled = false;
            cmbAgencies.Enabled = false;

            if (_action == Action.VIEW || cs.IsFinal.IsTrue)
            {
                AddTextName.Visible = false;
                AddTextDescription.Visible = false;
                txtAllDescriptions.Visible = true;
                txtAllNames.Visible = true;
                chkIsFinal.Enabled = false;
                txtAllDescriptions.Text = _localizedUtils.GetNameableDescription(cs);
                txtAllNames.Text = _localizedUtils.GetNameableName(cs);
            }
            else
            {
                AspConfirmationExit = "true";

                AddTextName.Visible = true;
                AddTextDescription.Visible = true;
                txtAllDescriptions.Visible = false;
                txtAllNames.Visible = false;
                /*
                 * DA IMPLEMENTARE L'ADDING NELLE GRIDS DI MASSIMILIANO 
                 */

                AddTextName.InitTextObjectList = cs.Names;
                AddTextDescription.InitTextObjectList = cs.Descriptions;
            }

            if ( _action != Action.VIEW ) 
            {
                DuplicateArtefact1.Visible = true;
            }

            AnnotationGeneralControl.AddText_ucOpenTabName = AnnotationGeneralControl.ClientID;
            AnnotationGeneralControl.AnnotationObjectList = cs.MutableInstance.Annotations;
            AnnotationGeneralControl.EditMode = (cs.IsFinal.IsTrue || _action == Action.VIEW) ? false : true;
            AnnotationGeneralControl.OwnerAgency =  txtAgenciesReadOnly.Text;

            if (_action == Action.VIEW || cs.IsFinal.IsTrue)
            {
                txtValidFrom.Enabled = false;
                txtValidTo.Enabled = false;
                txtDSDName.Enabled = false;
                txtDSDDescription.Enabled = false;
                txtDSDURI.Enabled = false;
                chkIsFinal.Enabled = false;
            }
            else
            {
                txtValidFrom.Enabled = true;
                txtValidTo.Enabled = true;
                txtDSDName.Enabled = true;
                txtDSDDescription.Enabled = true;
                txtDSDURI.Enabled = true;
                chkIsFinal.Enabled = true;
            }

            //===========================================

            if (_action == Action.INSERT)
            {
                cmbAgencies.Visible = true;
                txtAgenciesReadOnly.Visible = false;
            }
            else
            {
                cmbAgencies.Visible = false;
                txtAgenciesReadOnly.Visible = true;
            }

            SetCodeDetailPanel(cs);
        }

        private void SetCodeDetailPanel(ICategorySchemeObject cs)
        {
            // Verifico se la codelist è final
            if (cs.IsFinal.IsTrue || _action == Action.VIEW)
            {
                // Se final il pulsante di add e le colonne di modifica
                // dei codici non devono apparire
                btnSaveMemoryCategoryScheme.Visible = false;
                btnAddNewCategory.Visible = false;
                cmbLanguageForCsv.Visible = false;


                //Modifca UserControlCSV
                CSVImporter1.Visible = false;
                //imgImportCsv.Visible = false;
            }
            else
            {
                btnSaveMemoryCategoryScheme.Visible = true;
                btnAddNewCategory.Visible = true;
                Utils.PopulateCmbLanguages(cmbLanguageForCsv, AVAILABLE_MODES.MODE_FOR_ADD_TEXT);
                cmbLanguageForCsv.Visible = true;

                //Modifca UserControlCSV
                CSVImporter1.Visible = true;
                //imgImportCsv.Visible = true;
            }
        }

        private void SetInsertForm()
        {
            SetEditingControl();
        }

        private void SetEditForm()
        {
            if (!IsPostBack)
            {
                SetLabelDetail();
                btnSaveMemoryCategoryScheme.Visible = true;
                SetEditingControl();
                BindData(true);
                SetSDMXObjects();
            }
            GetSDMXObjects();
        }
        private void SetViewForm()
        {
            if (!Page.IsPostBack)
            {
                SetLabelDetail();
                BindData();
                btnSaveMemoryCategoryScheme.Visible = false;
            }
        }
        private void SetEditingControl()
        {
            txtDSDID.Enabled = true;
            txtVersion.Enabled = true;
            txtDSDURI.Enabled = true;
            txtValidFrom.Enabled = true;
            txtValidTo.Enabled = true;
            txtDSDName.Enabled = true;
            txtDSDDescription.Enabled = true;
            cmbAgencies.Enabled = true;
            chkIsFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;
            btnSaveMemoryCategoryScheme.Visible = true;

            // Svuoto le griglie di name e description
            if (Request["ACTION"] == "INSERT" && !Page.IsPostBack)
            {
                AddTextName.ClearTextObjectList();
                AddTextDescription.ClearTextObjectList();

            }

            AnnotationGeneralControl.ClearAnnotationsSession();
            AnnotationNewControl.ClearAnnotationsSession();
            AnnotationUpdateControl.ClearAnnotationsSession();
        }
        private void SetInitControls()
        {
            AddTextName.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategoryScheme;
            AddTextName.TType = TextType.NAME;
            AddTextDescription.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CategoryScheme;
            AddTextDescription.TType = TextType.DESCRIPTION;
            NewCategoryAddTextName.TType = TextType.NAME;
            NewCategoryAddTextDescription.TType = TextType.DESCRIPTION;
            AddText1.TType = TextType.NAME;
            AddText2.TType = TextType.DESCRIPTION;


        }
        private void GetSDMXObjects()
        {
            _sdmxObjects = (ISdmxObjects)Session["SDMXObjexts"];
        }
        private void SetSDMXObjects()
        {
            Session["SDMXObjexts"] = _sdmxObjects;
        }

        #endregion

        protected void TreeView1_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            // TEMPORARY LOG
            TemporaryLog("In seleziona nodo");

            if (TreeView1.Nodes[0].Selected == true)
            {
                btnDeleteCategory.Enabled = false;
                btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
                btnUpdateSelectedCategory.Enabled = false;
                btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
                if (Session["pendingOperation"] == null || !Session["pendingOperation"].ToString().Equals("conf_spost"))
                {
                    btnMoveCategory.Enabled = false;
                    btnMoveCategory.ImageUrl = imgMoveDisablePath;
                    txtCategoryOrder.Enabled = false;
                    btnDeselectCategory.Enabled = true;
                    btnDeselectCategory.ImageUrl = imgDeselectPath;
                }
            }
            else
            {
                ICategorySchemeObject cs = GetCategorySchemeFromSession().ImmutableInstance;
                ICategorySchemeMutableObject csMutable = GetCategorySchemeFromSession(); // Usato solo per annotazioni
                string[] elements = TreeView1.SelectedNode.ValuePath.Split('/');
                ICategoryObject currentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));
                ICategoryMutableObject currentNodeInCategorySchemeMutableVersion = csMutable.Items.First(obj => obj.Id.Equals(elements[1]));

                for (int i = 2; i < elements.Length; i++)
                {
                    currentNodeInCategoryScheme = currentNodeInCategoryScheme.Items.First(obj => obj.Id.Equals(elements[i]));
                    currentNodeInCategorySchemeMutableVersion = currentNodeInCategorySchemeMutableVersion.Items.First(obj => obj.Id.Equals(elements[i]));
                }

                // Svuoto i controlli
                // ---------------------------------
                txtUpdateCategoryID.Text = currentNodeInCategoryScheme.Id;

                AddText1.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Category;
                AddText1.TType = TextType.NAME;
                AddText1.ucOpenPopUpName = null;
                AddText1.ClearTextObjectList();
                AddText1.InitTextObjectList = currentNodeInCategoryScheme.Names;
                AddText2.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Category;
                AddText2.TType = TextType.DESCRIPTION;
                AddText2.ucOpenPopUpName = null;
                AddText2.ClearTextObjectList();
                AddText2.InitTextObjectList = currentNodeInCategoryScheme.Descriptions;

                AddText1.ucOpenPopUpName = "df-Dimension-update";
                AddText2.ucOpenPopUpName = "df-Dimension-update";

                AnnotationUpdateControl.AnnotationObjectList = currentNodeInCategorySchemeMutableVersion.Annotations;

                if (Session["currentNodeValuePath"] != null)
                {
                    /*TreeNode node = TreeView1.FindNode( Session["currentNodeValuePath"].ToString() );
                    node.Select();*/
                }

                if (Session["pendingOperation"] == null || !Session["pendingOperation"].ToString().Equals("conf_spost"))
                {
                    btnDeleteCategory.Enabled = true;
                    btnDeleteCategory.ImageUrl = imgDeletePath;
                    btnMoveCategory.Enabled = true;
                    btnMoveCategory.ImageUrl = imgMovePath;
                    btnUpdateSelectedCategory.Enabled = true;
                    btnUpdateSelectedCategory.ImageUrl = imgUpdatePath;
                    txtCategoryOrder.Enabled = true;
                    btnDeselectCategory.Enabled = true;
                    btnDeselectCategory.ImageUrl = imgDeselectPath;
                }
            }
            ViewState["previousSelectedNode"] = TreeView1.SelectedNode.ValuePath;
            Utils.AppendScript("location.href='#categories'");
        }

        protected void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            // TEMPORARY LOG
            TemporaryLog("In elimina nodo");

            ICategorySchemeMutableObject currentCategory = GetCategorySchemeFromSession();
            if (TreeView1.SelectedNode != null)
            {
                currentCategory = DeleteCategoryInCategoryscheme(currentCategory, TreeView1.SelectedNode.ValuePath, TreeView1.SelectedNode.Parent != null ? TreeView1.SelectedNode.Parent.ValuePath : string.Empty);
                if (!SaveInMemory(currentCategory)) return;
                BindData();
            }

            Utils.AppendScript("location.href='#categories'");
        }

        protected void btnMoveCategory_Click(object sender, EventArgs e)
        {
            if (Session["pendingOperation"] == null || Session["pendingOperation"].ToString().Equals("att_spost"))
            {
                // TEMPORARY LOG
                TemporaryLog("In settaggio spostamento");

                Session["pendingOperation"] = "conf_spost";
                Session["currentNodeValuePath"] = TreeView1.SelectedNode.ValuePath;
                btnMoveCategory.ToolTip = Resources.Messages.lbl_confirm_move_category;
                btnCancelMoveCategory.Enabled = true;
                btnCancelMoveCategory.ImageUrl = imgCancelMovePath;
                TreeView1.SelectedNode.Selected = false;
                lblMoveInstructions.Text = Resources.Messages.txt_move_instructions_second;
                btnMoveCategory.ImageUrl = "~/images/Check File-50.png";
                btnAddNewCategory.Enabled = false;
                btnAddNewCategory.ImageUrl = imgAddDisablePath;
                btnUpdateSelectedCategory.Enabled = false;
                btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
                btnDeselectCategory.Enabled = false;
                btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
                btnDeleteCategory.Enabled = false;
                btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
            }
            else
            {
                // TEMPORARY LOG
                TemporaryLog("In conferma spostamento");

                if (TreeView1.SelectedNode == null)
                {
                    Utils.ShowDialog(Resources.Messages.msg_select_destination_node);
                    Utils.AppendScript("location.href = \"#categories\";");
                    return;
                }
                int convertedIndex = 0;
                if ((!txtCategoryOrder.Text.Trim().Equals(string.Empty)) && !(int.TryParse(txtCategoryOrder.Text, out convertedIndex)))
                {
                    Utils.ShowDialog(Resources.Messages.msg_category_index_not_valid);
                    Utils.AppendScript("location.href = \"#categories\";");
                    return;
                }
                Session["pendingOperation"] = "att_spost";
                string currentNodeValuePath = Session["currentNodeValuePath"].ToString();
                Session["currentNodeValuePath"] = null;

                ICategorySchemeMutableObject cs = GetCategorySchemeFromSession();
                string[] elements = currentNodeValuePath.Split('/');
                ICategoryMutableObject currentNodeInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));

                ICategoryMutableObject currentNodeOldParentInCategoryScheme = null;
                ICategoryMutableObject currentNodeParentInCategoryScheme = null;

                if (elements.Length > 2)
                {
                    currentNodeOldParentInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(elements[1]));
                    for (int i = 2; i < elements.Length - 1; i++)
                    {
                        currentNodeOldParentInCategoryScheme = currentNodeOldParentInCategoryScheme.Items.First(obj => obj.Id.Equals(elements[i]));
                    }
                }

                for (int i = 2; i < elements.Length; i++)
                {
                    currentNodeInCategoryScheme = currentNodeInCategoryScheme.Items.First(obj => obj.Id.Equals(elements[i]));
                }

                if (TreeView1.SelectedNode == null || TreeView1.Nodes[0].Selected)
                {
                    if (elements.Length > 2)
                    {
                        currentNodeOldParentInCategoryScheme.Items.Remove(currentNodeInCategoryScheme);
                    }
                    else
                    {
                        cs.Items.Remove(currentNodeInCategoryScheme);
                    }
                    if ((!txtCategoryOrder.Text.Trim().Equals(string.Empty)) && int.TryParse(txtCategoryOrder.Text, out convertedIndex))
                    {
                        int index = Convert.ToInt32(txtCategoryOrder.Text) - 1;
                        cs.Items.Insert(index, currentNodeInCategoryScheme);
                    }
                    else
                    {
                        cs.Items.Add(currentNodeInCategoryScheme);
                    }
                }
                else
                {
                    try
                    {
                        if (elements.Length > 2)
                        {
                            currentNodeOldParentInCategoryScheme.Items.Remove(currentNodeInCategoryScheme);
                        }
                        else
                        {
                            cs.Items.Remove(currentNodeInCategoryScheme);
                        }
                        string currentParentNodeValuePath = string.Empty;
                        if (TreeView1.SelectedNode != null)
                        {
                            currentParentNodeValuePath = TreeView1.SelectedNode.ValuePath;
                            string[] parentElements = currentParentNodeValuePath.Split('/');
                            currentNodeParentInCategoryScheme = cs.Items.First(obj => obj.Id.Equals(parentElements[1]));

                            for (int i = 2; i < parentElements.Length; i++)
                            {
                                currentNodeParentInCategoryScheme = currentNodeParentInCategoryScheme.Items.First(obj => obj.Id.Equals(parentElements[i]));
                            }
                        }

                        if ((!txtCategoryOrder.Text.Trim().Equals(string.Empty)) && int.TryParse(txtCategoryOrder.Text, out convertedIndex))
                        {
                            int index = Convert.ToInt32(txtCategoryOrder.Text) - 1;
                            if (index > currentNodeParentInCategoryScheme.Items.Count)
                            {
                                currentNodeParentInCategoryScheme.Items.Add(currentNodeInCategoryScheme);
                            }
                            else
                            {
                                currentNodeParentInCategoryScheme.Items.Insert(index, currentNodeInCategoryScheme);
                            }
                        }
                        else
                        {
                            currentNodeParentInCategoryScheme.Items.Add(currentNodeInCategoryScheme);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (elements.Length > 2)
                        {
                            currentNodeOldParentInCategoryScheme.Items.Add(currentNodeInCategoryScheme);
                        }
                        else
                        {
                            cs.Items.Add(currentNodeInCategoryScheme);
                        }
                        Session["pendingOperation"] = null;
                        Session["currentNodeValuePath"] = null;
                        btnMoveCategory.ToolTip = Resources.Messages.lbl_init_move_category;
                        lblMoveInstructions.Text = Resources.Messages.txt_move_instructions_first;
                        btnMoveCategory.ImageUrl = "~/images/Cut-50.png";
                        btnAddNewCategory.Enabled = true;
                        btnAddNewCategory.ImageUrl = imgAddPath;
                        btnUpdateSelectedCategory.Enabled = false;
                        btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
                        btnDeselectCategory.Enabled = false;
                        btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
                        btnDeleteCategory.Enabled = false;
                        btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
                        btnCancelMoveCategory.Enabled = false;
                        btnCancelMoveCategory.ImageUrl = imgCancelMoveDisablePath;
                        btnDeselectCategory.Enabled = false;
                        btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
                        Utils.ShowDialog(Resources.Messages.err_generic_category_order);
                        Utils.AppendScript("location.href= \"#categories\"");
                        return;
                    }

                }

                if (!SaveInMemory(cs))
                {
                    return;
                }

                BindData();
                btnMoveCategory.ToolTip = Resources.Messages.lbl_init_move_category;
                lblMoveInstructions.Text = Resources.Messages.txt_move_instructions_first;
                btnMoveCategory.ImageUrl = "~/images/Cut-50.png";
                btnAddNewCategory.Enabled = true;
                btnAddNewCategory.ImageUrl = imgAddPath;
                btnCancelMoveCategory.Enabled = false;
                btnCancelMoveCategory.ImageUrl = imgCancelMoveDisablePath;
                btnUpdateSelectedCategory.Enabled = false;
                btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
                btnDeselectCategory.Enabled = false;
                btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
                btnDeleteCategory.Enabled = false;
                btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
                btnMoveCategory.Enabled = false;
                btnMoveCategory.ImageUrl = imgMoveDisablePath;
                txtCategoryOrder.Enabled = false;
            }            
            Utils.AppendScript("location.href='#categories'");
        }

        protected void btnDeselectCategory_Click(object sender, EventArgs e)
        {
            // TEMPORARY LOG
            TemporaryLog("In deselezione nodo");

            TreeView1.SelectedNode.Selected = false;
            btnUpdateSelectedCategory.Enabled = false;
            btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
            btnDeleteCategory.Enabled = false;
            btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
            btnMoveCategory.Enabled = false;
            btnMoveCategory.ImageUrl = imgMoveDisablePath;
            btnDeselectCategory.Enabled = false;
            btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
            Utils.AppendScript("location.href='#categories'");
        }

        protected void btnCancelMovingCategory_Click(object sender, EventArgs e)
        {
            // TEMPORARY LOG
            TemporaryLog("In annullamento spostamento");

            Session["pendingOperation"] = null;
            Session["currentNodeValuePath"] = null;
            btnMoveCategory.ToolTip = Resources.Messages.lbl_init_move_category;
            lblMoveInstructions.Text = Resources.Messages.txt_move_instructions_first;
            btnMoveCategory.ImageUrl = "~/images/Cut-50.png";
            btnAddNewCategory.Enabled = true;
            btnAddNewCategory.ImageUrl = imgAddPath;
            btnUpdateSelectedCategory.Enabled = false;
            btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
            btnCancelMoveCategory.Enabled = false;
            btnCancelMoveCategory.ImageUrl = imgCancelMoveDisablePath;
            btnDeselectCategory.Enabled = false;
            btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
            btnDeleteCategory.Enabled = false;
            btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
            btnMoveCategory.Enabled = false;
            btnMoveCategory.ImageUrl = imgMoveDisablePath;
            txtCategoryOrder.Enabled = false;
            Utils.AppendScript("location.href= \"#categories\"");
        }

        protected void TreeView1_PreRender(object sender, EventArgs e)
        {
            /* if ( ViewState["previousSelectedNode"] != null )
             {
                 if ( TreeView1.SelectedNode.ValuePath.Equals( ViewState["previousSelectedNode"].ToString() ) )
                 {
                     Utils.AppendScript( "location.href= \"#categories\"" );
                 }
             } */
        }

        // Usata per log temporaneo 
        private void TemporaryLog(string messagge)
        {
            //StreamWriter logWriter = new StreamWriter(Server.MapPath("~/temporary.txt"), true);
            //logWriter.WriteLine(messagge);
            //logWriter.Flush();
            //logWriter.Close();
        }

        protected void btnClearFieldsForUpdate_Click(object sender, EventArgs e)
        {
            lblErrorOnUpdate.Text = string.Empty;
            TreeView1.SelectedNode.Selected = false;
            btnUpdateSelectedCategory.Enabled = false;
            btnUpdateSelectedCategory.ImageUrl = imgUpdateDisablePath;
            btnDeleteCategory.Enabled = false;
            btnDeleteCategory.ImageUrl = imgDeleteDisablePath;
            btnMoveCategory.Enabled = false;
            btnMoveCategory.ImageUrl = imgMoveDisablePath;
            btnDeselectCategory.Enabled = false;
            btnDeselectCategory.ImageUrl = imgDeselectDisablePath;
            Utils.AppendScript("location.href= '#categories';");
        }

        protected void btnClearFields_Click(object sender, EventArgs e)
        {
            lblErrorOnNewInsert.Text = string.Empty;
            txtNewCategoryId.Text = string.Empty;
            NewCategoryAddTextDescription.ClearTextObjectListWithOutJS();
            NewCategoryAddTextName.ClearTextObjectListWithOutJS();
            Utils.AppendScript("location.href= '#categories';");
        }
    }
}