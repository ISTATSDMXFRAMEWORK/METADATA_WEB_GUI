﻿using System;
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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Base;

using ISTATUtils;
using ISTAT.EntityMapper;
using ISTAT.Entity;
using ISTAT.WSDAL;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using ISTATRegistry.IRServiceReference;
using System.Collections.Specialized;

namespace ISTATRegistry
{

    public partial class categorizationItemDetails : ISTATRegistry.Classes.ISTATWebPage
    {

        public static string KEY_PAGE_SESSION = "TempCategorization";

        ArtefactIdentity _artIdentity;
        Action _action;
        ISdmxObjects _sdmxObjects;
        LocalizedUtils _localizedUtils;
        protected string AspConfirmationExit = "false";

        private void SetAction()
        {
            if (Request["ACTION"] == null || Utils.ViewMode)
                _action = Action.VIEW;
            else
            {
                if (Request["ACTION"] == "UPDATE")
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

        private string GetAgencyValue()
        {
            if (_action == Action.INSERT)
            {
                string agencyValue = cmb_agencies.SelectedValue.ToString();
                string agencyId = agencyValue.Split('-')[0].Trim();
                return agencyId;
            }
            else
            {
                return txtAgenciesReadOnly.Text;
            }
        }

        private ICategorisationMutableObject GetCategorizationForm()
        {
            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori


            #region CATEGORIZATION ID
            if (!ValidationUtils.CheckIdFormat(txt_id.Text.Trim()))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION AGENCY
            if (cmb_agencies.Text.Trim().Equals(string.Empty))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION VERSION
            if (!ValidationUtils.CheckVersionFormat(txt_version.Text.Trim()))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region CATEGORIZATION URI
            if ((txt_uri.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txt_uri.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION START END DATE
            bool checkForDatesCombination = true;

            if (!txt_valid_from.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txt_valid_from.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_from_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txt_valid_to.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txt_valid_to.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_to_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txt_valid_from.Text.Trim().Equals(string.Empty) && !txt_valid_to.Text.Trim().Equals(string.Empty))
            {
                // Controllo congruenza date
                if (checkForDatesCombination)
                {
                    if (!ValidationUtils.CheckDates(txt_valid_from.Text, txt_valid_to.Text))
                    {
                        messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_diff + "<br /><br />";
                        errorCounter++;
                        isInError = true;
                    }
                }
            }
            #endregion

            if (isInError)
            {
                Utils.ShowDialog(messagesGroup, 300);
                return null;
            }

            ICategorisationMutableObject tmpCategorization = new CategorisationMutableCore();
            #region CREATE CATEGORIZATION FROM FORM

            tmpCategorization.AgencyId = GetAgencyValue();
            tmpCategorization.Id = txt_id.Text;
            tmpCategorization.Version = txt_version.Text;
            tmpCategorization.FinalStructure = TertiaryBool.ParseBoolean(chk_isFinal.Checked);
            tmpCategorization.Uri = (!txt_uri.Text.Trim().Equals(string.Empty) && ValidationUtils.CheckUriFormat(txt_uri.Text)) ? new Uri(txt_uri.Text) : null;
            if (!txt_valid_from.Text.Trim().Equals(string.Empty))
            {
                tmpCategorization.StartDate = DateTime.ParseExact(txt_valid_from.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpCategorization.StartDate = null;
			}
            if (!txt_valid_to.Text.Trim().Equals(string.Empty))
            {
                tmpCategorization.EndDate = DateTime.ParseExact(txt_valid_to.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				tmpCategorization.EndDate = null;
			}
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                tmpCategorization.AddName(tmpName.Locale, tmpName.Value);
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    tmpCategorization.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    tmpCategorization.AddAnnotation(annotation);
                }
            #endregion

            string kindOfStructure = Session["tmpKindOfStructure"].ToString();
            SdmxStructureEnumType typeOfTheStructure = 0;

            switch (kindOfStructure)
            {
                case "CODELIST":
                    typeOfTheStructure = SdmxStructureEnumType.CodeList;
                    break;
                case "CONCEPT_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.ConceptScheme;
                    break;
                case "CATEGORY_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.CategoryScheme;
                    break;
                case "DATAFLOW":
                    typeOfTheStructure = SdmxStructureEnumType.Dataflow;
                    break;
                case "AGENCY_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.AgencyScheme;
                    break;
                case "DATA_PROVIDER_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.DataProviderScheme;
                    break;
                case "DATA_CONSUMER_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.DataConsumerScheme;
                    break;
                case "ORGANIZATION_UNIT_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.OrganisationUnitScheme;
                    break;
                case "STRUCTURE_SET":
                    typeOfTheStructure = SdmxStructureEnumType.StructureSet;
                    break;
                case "CONTENT_CONSTRAINT":
                    typeOfTheStructure = SdmxStructureEnumType.ContentConstraint;
                    break;
                case "DSD":
                    typeOfTheStructure = SdmxStructureEnumType.Dsd;
                    break;
            }

            string referenceOfTheObject = Session["tmpReferenceOfTheObject"].ToString();
            string[] elementsOfTheReference = referenceOfTheObject.Split('-');

            IStructureReference structureRef = new StructureReferenceImpl(elementsOfTheReference[1], elementsOfTheReference[0], elementsOfTheReference[2], typeOfTheStructure);
            tmpCategorization.StructureReference = structureRef;

            string[] referenceOfTheCategory = { Session["tmpCategoryReference"].ToString() };

            string referenceOfTheCategoryScheme = Session["tmpCategorySchemeReference"].ToString();
            string[] elementsOfTheCategoryScheme = referenceOfTheCategoryScheme.Split('-');
            IStructureReference categoryRef = new StructureReferenceImpl(elementsOfTheCategoryScheme[1], elementsOfTheCategoryScheme[0], elementsOfTheCategoryScheme[2], SdmxStructureEnumType.Category, referenceOfTheCategory);
            tmpCategorization.CategoryReference = categoryRef;

            return tmpCategorization;
        }

        private ICategorisationMutableObject GetCategorizationForm(ICategorisationMutableObject cat)
        {

            if (cat == null) return GetCategorizationForm();

            bool isInError = false;                 // Indicatore di errore
            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            #region CATEGORIZATION ID
            if (!ValidationUtils.CheckIdFormat(txt_id.Text.Trim()))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION AGENCY
            if (cmb_agencies.Text.Trim().Equals(string.Empty))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION VERSION
            if (!ValidationUtils.CheckVersionFormat(txt_version.Text.Trim()))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            /* URI NOT REQUIRED */
            #region CATEGORIZATION URI
            if ((txt_uri.Text != string.Empty) && !ValidationUtils.CheckUriFormat(txt_uri.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_uri_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION NAMES
            if (AddTextName.TextObjectList == null || AddTextName.TextObjectList.Count == 0)
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_list_name_format + "<br /><br />";
                errorCounter++;
                isInError = true;
            }
            #endregion

            #region CATEGORIZATION START END DATE
            bool checkForDatesCombination = true;

            if (!txt_valid_from.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txt_valid_from.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_from_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txt_valid_to.Text.Trim().Equals(string.Empty) && !ValidationUtils.CheckDateFormat(txt_valid_to.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_to_format + "<br /><br />";
                errorCounter++;
                checkForDatesCombination = false;
                isInError = true;
            }

            if (!txt_valid_from.Text.Trim().Equals(string.Empty) && !txt_valid_to.Text.Trim().Equals(string.Empty))
            {
                // Controllo congruenza date
                if (checkForDatesCombination)
                {
                    if (!ValidationUtils.CheckDates(txt_valid_from.Text, txt_valid_to.Text))
                    {
                        messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_date_diff + "<br /><br />";
                        errorCounter++;
                        isInError = true;
                    }
                }
            }
            #endregion

            if (isInError)
            {
                Utils.ShowDialog(messagesGroup, 300);
                return null;
            }

            #region CREATE CODELIST FROM FORM

            cat.AgencyId = GetAgencyValue();
            cat.Id = txt_id.Text;
            cat.Version = txt_version.Text;
            cat.FinalStructure = TertiaryBool.ParseBoolean(chk_isFinal.Checked);
            cat.Uri = (!txt_uri.Text.Trim().Equals(string.Empty) && ValidationUtils.CheckUriFormat(txt_uri.Text)) ? new Uri(txt_uri.Text) : null;
            if (!txt_valid_from.Text.Trim().Equals(string.Empty))
            {
                cat.StartDate = DateTime.ParseExact(txt_valid_from.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				cat.StartDate = null;
			}
            if (!txt_valid_to.Text.Trim().Equals(string.Empty))
            {
                cat.EndDate = DateTime.ParseExact(txt_valid_to.Text, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
			else
			{
				cat.EndDate = null;
			}
            if (cat.Names.Count != 0)
            {
                cat.Names.Clear();
            }
            foreach (var tmpName in AddTextName.TextObjectList)
            {
                cat.AddName(tmpName.Locale, tmpName.Value);
            }
            if (cat.Descriptions.Count != 0)
            {
                cat.Descriptions.Clear();
            }
            if (AddTextDescription.TextObjectList != null)
                foreach (var tmpDescription in AddTextDescription.TextObjectList)
                {
                    cat.AddDescription(tmpDescription.Locale, tmpDescription.Value);
                }
            if (cat.Annotations.Count != 0)
            {
                cat.Annotations.Clear();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null)
                foreach (var annotation in AnnotationGeneralControl.AnnotationObjectList)
                {
                    cat.AddAnnotation(annotation);
                }

            #endregion

            string kindOfStructure = Session["tmpKindOfStructure"].ToString();
            SdmxStructureEnumType typeOfTheStructure = default(SdmxStructureEnumType);

            switch (kindOfStructure)
            {
                case "CODELIST":
                    typeOfTheStructure = SdmxStructureEnumType.CodeList;
                    break;
                case "CONCEPT_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.ConceptScheme;
                    break;
                case "CATEGORY_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.CategoryScheme;
                    break;
                case "DATAFLOW":
                    typeOfTheStructure = SdmxStructureEnumType.Dataflow;
                    break;
                case "AGENCY_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.AgencyScheme;
                    break;
                case "DATA_PROVIDER_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.DataProviderScheme;
                    break;
                case "DATA_CONSUMER_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.DataConsumerScheme;
                    break;
                case "ORGANIZATION_UNIT_SCHEME":
                    typeOfTheStructure = SdmxStructureEnumType.OrganisationUnitScheme;
                    break;
                case "STRUCTURE_SET":
                    typeOfTheStructure = SdmxStructureEnumType.StructureSet;
                    break;
                case "CONTENT_CONSTRAINT":
                    typeOfTheStructure = SdmxStructureEnumType.ContentConstraint;
                    break;
                case "DSD":
                    typeOfTheStructure = SdmxStructureEnumType.Dsd;
                    break;
            }

            string referenceOfTheObject = Session["tmpReferenceOfTheObject"].ToString();
            string[] elementsOfTheReference = referenceOfTheObject.Split('-');

            IStructureReference structureRef = new StructureReferenceImpl(elementsOfTheReference[1], elementsOfTheReference[0], elementsOfTheReference[2], typeOfTheStructure);
            cat.StructureReference = structureRef;

            string[] referenceOfTheCategory = { Session["tmpCategoryReference"].ToString() };
            string referenceOfTheCategoryScheme = Session["tmpCategorySchemeReference"].ToString();
            string[] elementsOfTheCategoryScheme = referenceOfTheCategoryScheme.Split('-');
            IStructureReference categoryRef = new StructureReferenceImpl(elementsOfTheCategoryScheme[1], elementsOfTheCategoryScheme[0], elementsOfTheCategoryScheme[2], SdmxStructureEnumType.Category, referenceOfTheCategory);
            cat.CategoryReference = categoryRef;

            return cat;
        }

        private IAnnotationMutableObject GetAnnotationOrder(int index)
        {

            IAnnotationMutableObject annotation = new AnnotationMutableCore();
            ITextTypeWrapperMutableObject iText = new TextTypeWrapperMutableCore();
            iText.Value = index.ToString();
            iText.Locale = "en";
            annotation.AddText(iText);
            annotation.Type = "@ORDER@";

            return annotation;
        }

        private ICategorisationMutableObject GetCategorizationFromSession()
        {
            try
            {
                if (Session[KEY_PAGE_SESSION] == null)
                {
                    if (_artIdentity.ToString() != string.Empty)
                    {
                        WSModel wsModel = new WSModel();
                        ISdmxObjects sdmxObject = wsModel.GetCategorisation(_artIdentity, false, false);
                        ICategorisationObject cat = sdmxObject.Categorisations.FirstOrDefault();
                        Session[KEY_PAGE_SESSION] = cat.MutableInstance;
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                return (ICategorisationMutableObject)Session[KEY_PAGE_SESSION];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveInMemory(ICategorisationMutableObject cat)
        {
            if (cat == null) return false;

            Session[KEY_PAGE_SESSION] = cat;

            return true;
        }

        private bool SendQuerySubmit(ICategorisationMutableObject cat)
        {

            try
            {

                ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

                sdmxObjects.AddCategorisation(cat.ImmutableInstance);

                WSModel modelCategorization = new WSModel();

                XmlDocument result = modelCategorization.SubmitStructure(sdmxObjects);

                if (Utils.GetXMLResponseError(result) != "")
                {
                    Utils.ShowDialog(Utils.GetXMLResponseError(result), 350, "Error");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void ClearSessionPage()
        {
            Session[KEY_PAGE_SESSION] = null;
        }

        private int CurrentCodeIndex { get { return (int)Session[KEY_PAGE_SESSION + "_index_code"]; } set { Session[KEY_PAGE_SESSION + "_index_code"] = value; } }

        #region Event Handler

        protected void Page_Load(object sender, EventArgs e)
        {
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _sdmxObjects = new SdmxObjectsImpl();

            SetAction();

            if (!IsPostBack)
            {
                if (_action != Action.VIEW)
                    Utils.PopulateCmbAgencies(cmb_agencies, true);
                ClearSessionPage();

            }

            switch (_action)
            {
                case Action.INSERT:

                    AspConfirmationExit = "true";

                    SetInitControls();
                    SetInsertForm();
                    SetStructureDetailPanel();
                    chk_isFinal.Checked = false;
                    chk_isFinal.Enabled = false;

                    if (!Page.IsPostBack)
                    {
                        cmb_agencies.Items.Insert(0, new ListItem(String.Empty, String.Empty));
                        cmb_agencies.SelectedIndex = 0;
                        FileDownload31.Visible = false;
                    }

                    break;
                case Action.UPDATE:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);

                    SetInitControls();
                    SetEditForm();
                    break;
                case Action.VIEW:

                    _artIdentity = Utils.GetIdentityFromRequest(Request);
                    ClearSessionPage();
                    SetViewForm();

                    FileDownload31.ucID = _artIdentity.ID;
                    FileDownload31.ucAgency = _artIdentity.Agency;
                    FileDownload31.ucVersion = _artIdentity.Version;
                    FileDownload31.ucArtefactType = "CodeList";
                    break;
            }

            if (!Utils.ViewMode && _action != Action.INSERT)
            {
                DuplicateArtefact1.ucStructureType = SdmxStructureEnumType.Categorisation;
                DuplicateArtefact1.ucMaintanableArtefact = GetCategorizationFromSession();
            }
            else
                DuplicateArtefact1.ucDisable = true;


            lbl_id.DataBind();
            lbl_agency.DataBind();
            lbl_version.DataBind();
            lbl_isFinal.DataBind();
            lbl_uri.DataBind();
            lbl_urn.DataBind();
            lbl_valid_from.DataBind();
            lbl_valid_to.DataBind();
            lbl_name.DataBind();
            lbl_description.DataBind();
            lbl_annotation.DataBind();
            lblCategorySchemeList.DataBind();
            lblAvailableStructures.DataBind();
            btnOpenGridDiv.DataBind();
            btnOpenTreeDiv.DataBind();
            lblSelectedCategory.DataBind();
            lblSelectedItem.DataBind();

            btnSaveMemoryCategorization.DataBind();

        }
        protected void gvCodelistsItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            BindData();
            Utils.AppendScript("location.href= '#codes';");
        }

        protected void btnSaveAnnotationCode_Click(object sender, EventArgs e)
        {

            ICategorisationMutableObject cat = GetCategorizationFromSession();

            if (!SaveInMemory(cat)) return;

            BindData();

            Utils.AppendScript("location.href='#codes';");
        }

        protected void btnSaveMemoryCategorization_Click(object sender, EventArgs e)
        {
            if (Session["tmpCategoryReference"] == null || Session["tmpCategorySchemeReference"] == null || Session["tmpKindOfStructure"] == null || Session["tmpReferenceOfTheObject"] == null)
            {
                Utils.ShowDialog(Resources.Messages.err_reference_or_structure_is_missing, 300, string.Empty);
                return;
            }

            ICategorisationMutableObject cat = GetCategorizationFromSession();
            if (cat == null) cat = GetCategorizationForm();
            else cat = GetCategorizationForm(cat);

            if (!SaveInMemory(cat)) return;

            if (!SendQuerySubmit(cat)) return;

            BindData();

            string successMessage = string.Empty;
            if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.INSERT)
            {
                successMessage = Resources.Messages.succ_categorization_insert;
            }
            else if (((Action)Enum.Parse(typeof(Action), Request["ACTION"].ToString())) == Action.UPDATE)
            {
                successMessage = Resources.Messages.succ_categorization_update;
            }
            Utils.ShowDialog(successMessage, 300, Resources.Messages.succ_operation);
            if (_action == Action.INSERT)
            {
                Utils.AppendScript("~/categorization.aspx");
            }
        }

        protected void btnAddNewCode_Click(object sender, EventArgs e)
        {
        }
        protected void btnUpdateCode_Click(object sender, EventArgs e)
        {
        }

        protected void gvCode_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }
        protected void gvCodelistsItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // NULL
        }
        protected void gvCodelistsItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // NULL
        }
        protected void gvCodelistsItem_Sorting(object sender, GridViewSortEventArgs e)
        {
            //
        }
        protected void gvCodelistsItem_Sorted(object sender, EventArgs e)
        {
            // NULL
        }
        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region LAYOUT

        private void SetLabelDetail()
        {

            ICategorisationMutableObject cat = GetCategorizationFromSession();

            if (cat == null)
                lblCategorizationDetail.Text = String.Format("({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version);
            else
            {

                lblCategorizationDetail.Text = String.Format("{3} ({0}+{1}+{2})", _artIdentity.ID, _artIdentity.Agency, _artIdentity.Version, _localizedUtils.GetNameableName(cat.ImmutableInstance));
            }

        }

        private void BindData(bool isNewItem = false)
        {

            ICategorisationMutableObject cat = GetCategorizationFromSession();

            if (cat == null) return;

            SetGeneralTab(cat.ImmutableInstance);
        }

        private void SetGeneralTab(ICategorisationObject cat)
        {
            txt_id.Text = cat.Id;
            txtAgenciesReadOnly.Text = cat.AgencyId;
            txt_version.Text = cat.Version;
            chk_isFinal.Checked = cat.IsFinal.IsTrue;

            FileDownload31.ucID = cat.Id;
            FileDownload31.ucAgency = cat.AgencyId;
            FileDownload31.ucVersion = cat.Version;
            FileDownload31.ucArtefactType = "Categorization";

            txt_uri.Text = (cat.Uri != null) ? cat.Uri.AbsoluteUri : string.Empty;
            txt_urn.Text = (cat.Urn != null) ? cat.Urn.AbsoluteUri : string.Empty;
            txt_valid_from.Text = (cat.StartDate != null) ? string.Format("{0}/{1}/{2}", cat.StartDate.Date.Value.Day.ToString(), cat.StartDate.Date.Value.Month.ToString(), cat.StartDate.Date.Value.Year.ToString()) : string.Empty;
            txt_valid_to.Text = (cat.EndDate != null) ? string.Format("{0}/{1}/{2}", cat.EndDate.Date.Value.Day.ToString(), cat.EndDate.Date.Value.Month.ToString(), cat.EndDate.Date.Value.Year.ToString()) : string.Empty;

            txt_name_locale.Text = _localizedUtils.GetNameableName(cat);
            txt_description_locale.Text = _localizedUtils.GetNameableDescription(cat);

            // Svuoto le griglie 
            //===========================================
            if (AddTextName.TextObjectList != null && AddTextName.TextObjectList.Count != 0)
            {
                AddTextName.ClearTextObjectList();
            }
            if (AddTextDescription.TextObjectList != null && AddTextDescription.TextObjectList.Count != 0)
            {
                AddTextDescription.ClearTextObjectList();
            }
            if (AnnotationGeneralControl.AnnotationObjectList != null && AnnotationGeneralControl.AnnotationObjectList.Count != 0)
            {
                AnnotationGeneralControl.ClearAnnotationsSession();
            }

            txt_id.Enabled = false;
            txt_version.Enabled = false;
            cmb_agencies.Enabled = false;

            if (Request["ACTION"] == "VIEW" || cat.IsFinal.IsTrue || Utils.ViewMode)
            {
                AddTextName.Visible = false;
                txt_all_names.Visible = true;
                txt_all_names.Text = _localizedUtils.GetNameableName(cat);

                AddTextDescription.Visible = false;
                txt_all_description.Visible = true;
                txt_all_description.Text = _localizedUtils.GetNameableDescription(cat);
            }
            else
            {
                AspConfirmationExit = "true";

                AddTextName.Visible = true;
                AddTextDescription.Visible = true;
                txt_all_description.Visible = false;
                txt_all_names.Visible = false;

                AddTextName.InitTextObjectList = cat.Names;
                AddTextDescription.InitTextObjectList = cat.Descriptions;
            }

            if (_action != Action.VIEW)
            {
                DuplicateArtefact1.Visible = false;
            }

            AnnotationGeneralControl.AddText_ucOpenTabName = AnnotationGeneralControl.ClientID;
            AnnotationGeneralControl.AnnotationObjectList = cat.MutableInstance.Annotations;
            AnnotationGeneralControl.EditMode = !cat.IsFinal.IsTrue;
            AnnotationGeneralControl.OwnerAgency = txtAgenciesReadOnly.Text;

            if (cat.IsFinal.IsTrue)
            {
                txt_valid_from.Enabled = false;
                txt_valid_to.Enabled = false;
                txt_name_locale.Enabled = false;
                txt_description_locale.Enabled = false;
                txt_uri.Enabled = false;
                chk_isFinal.Enabled = false;

            }
            else
            {
                txt_valid_from.Enabled = true;
                txt_valid_to.Enabled = true;
                txt_name_locale.Enabled = true;
                txt_description_locale.Enabled = true;
                txt_uri.Enabled = true;
                chk_isFinal.Enabled = true;
            }

            if (_action == Action.INSERT)
            {
                cmb_agencies.Visible = true;
                txtAgenciesReadOnly.Visible = false;
            }
            else
            {
                cmb_agencies.Visible = false;
                txtAgenciesReadOnly.Visible = true;
            }

            SetStructureDetailPanel();

            string categoryId = cat.CategoryReference.IdentifiableIds.First();
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(cat.CategoryReference.MaintainableId, cat.CategoryReference.AgencyId, cat.CategoryReference.Version), false, false);

            ddlCategorySchemeList.Text = string.Format("{0}-{1}-{2}", cat.CategoryReference.MaintainableId, cat.CategoryReference.AgencyId, cat.CategoryReference.Version);

            ICategorySchemeObject currentCategoryScheme = sdmxInput.CategorySchemes.First();

            txtSelectedCategory.Text = cat.CategoryReference.IdentifiableIds[0].ToString();

            SdmxStructureEnumType type = cat.StructureReference.MaintainableStructureEnumType;

            switch (type)
            {
                case SdmxStructureEnumType.CodeList:
                    ddlAvailableStructures.SelectedValue = "CODELIST";
                    break;
                case SdmxStructureEnumType.ConceptScheme:
                    ddlAvailableStructures.SelectedValue = "CONCEPT_SCHEME";
                    break;
                case SdmxStructureEnumType.CategoryScheme:
                    ddlAvailableStructures.SelectedValue = "CATEGORY_SCHEME";
                    break;
                case SdmxStructureEnumType.Dataflow:
                    ddlAvailableStructures.SelectedValue = "DATAFLOW";
                    break;
                case SdmxStructureEnumType.Dsd:
                    ddlAvailableStructures.SelectedValue = "DSD";
                    break;
                case SdmxStructureEnumType.AgencyScheme:
                    ddlAvailableStructures.SelectedValue = "AGENCY_SCHEME";
                    break;
                case SdmxStructureEnumType.DataProviderScheme:
                    ddlAvailableStructures.SelectedValue = "DATA_PROVIDER_SCHEME";
                    break;
                case SdmxStructureEnumType.DataConsumerScheme:
                    ddlAvailableStructures.SelectedValue = "DATA_CONSUMER_SCHEME";
                    break;
                case SdmxStructureEnumType.OrganisationUnitScheme:
                    ddlAvailableStructures.SelectedValue = "ORGANIZATION_UNIT_SCHEME";
                    break;
                case SdmxStructureEnumType.StructureSet:
                    ddlAvailableStructures.SelectedValue = "STRUCTURE_SET";
                    break;
                case SdmxStructureEnumType.ContentConstraint:
                    ddlAvailableStructures.SelectedValue = "CONTENT_CONSTRAINT";
                    break;
            }

            string structure = string.Format("{0}-{1}-{2}", cat.StructureReference.MaintainableId, cat.StructureReference.AgencyId, cat.StructureReference.Version);
            txtSelectedItem.Text = structure;

            Session["tmpCategoryReference"] = cat.CategoryReference.IdentifiableIds[0].ToString();
            Session["tmpCategorySchemeReference"] = string.Format("{0}-{1}-{2}", currentCategoryScheme.Id, currentCategoryScheme.AgencyId, currentCategoryScheme.Version);
            Session["tmpKindOfStructure"] = ddlAvailableStructures.SelectedValue;
            Session["tmpReferenceOfTheObject"] = structure;
            txtSelectedCategory.Visible = true;
            lblSelectedCategory.Visible = true;
            txtSelectedItem.Visible = true;
            lblSelectedItem.Visible = true;
            //===========================================
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
                    //tmpNode.Value = string.Format( "[ {0} ] {1}", subCategory.Id, _localizedUtils.GetNameableName( subCategory ) );
                    tmpNode.Value = subCategory.Id;
                    tmpNode.Text = string.Format("[ {0} ] {1}", subCategory.Id, _localizedUtils.GetNameableName(subCategory));
                    tmpNode.SelectAction = TreeNodeSelectAction.Select;
                    node.ChildNodes.Add(tmpNode);
                    CreateTreeWithRecursion(subCategory, node.ChildNodes[counter]);
                    counter++;
                }
            }
            else
            {
                return;
            }
        }

        private void SetStructureDetailPanel()
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    NameValueCollection supportedArtefacts = SupportedCategorisationArtefacts.Artefacts;
                    List<String> Artefacts = new List<string>();

                    foreach (var art in supportedArtefacts)
                    {
                        if (!bool.Parse(supportedArtefacts[art.ToString()]))
                            Artefacts.Add(art.ToString());
                    }

                    //string[] skipElements = { "CATEGORIZATION" };

//                    Utils.PopulateCmbArtefacts(ddlAvailableStructures, skipElements);
                    Utils.PopulateCmbArtefacts(ddlAvailableStructures, Artefacts.ToArray());

                    WSModel wsModel = new WSModel();
                    ISdmxObjects sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, true);




                    foreach (ICategorySchemeObject cs in sdmxInput.CategorySchemes)
                    {
                        if (cs.IsFinal.IsTrue)
                        {
                            ddlCategorySchemeList.Items.Add(new ListItem(string.Format("{0}-{1}-{2}", cs.Id.ToUpper(), cs.AgencyId.ToUpper(), cs.Version), string.Format("{0}-{1}-{2}", cs.Id, cs.AgencyId, cs.Version)));
                        }
                    }

                    string[] splittedElementsOfCategoryScheme = ddlCategorySchemeList.Text.Split('-');
                    sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(splittedElementsOfCategoryScheme[0], splittedElementsOfCategoryScheme[1], splittedElementsOfCategoryScheme[2]), false, false);
                    //sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity( "cat_sch3", "ESTAT", "1.1"), false,false);

                    ICategorySchemeObject categoryScheme = sdmxInput.CategorySchemes.First();

                    foreach (var category in categoryScheme.Items)
                    {
                        TreeNode node = new TreeNode(string.Format("[ {0} ] {1}", category.Id, _localizedUtils.GetNameableName(category)));
                        //node.Value = string.Format( "[ {0} ] {1}", category.Id, _localizedUtils.GetNameableName( category ) );
                        node.Value = category.Id;
                        node.Text = string.Format("[ {0} ] {1}", category.Id, _localizedUtils.GetNameableName(category));
                        node.SelectAction = TreeNodeSelectAction.Select;
                        CreateTreeWithRecursion(category, node);
                        TreeView1.Nodes.Add(node);
                    }

                    TreeView1.CollapseAll();

                    switch (ddlAvailableStructures.Text)
                    {
                        case "CODELIST":
                            sdmxInput = wsModel.GetCodeList(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            List<ICodelistObject> codelist = sdmxInput.Codelists.ToList();
                            structuresGrid.DataSource = codelist;
                            break;
                        case "CONCEPT_SCHEME":
                            sdmxInput = wsModel.GetConceptScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            List<IConceptSchemeObject> conceptScheme = sdmxInput.ConceptSchemes.ToList();
                            structuresGrid.DataSource = conceptScheme;
                            break;
                        case "CATEGORY_SCHEME":
                            sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            List<ICategorySchemeObject> categorySchemeFound = sdmxInput.CategorySchemes.ToList();
                            structuresGrid.DataSource = categorySchemeFound;
                            break;
                        case "DATAFLOW":
                            sdmxInput = wsModel.GetDataFlow(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "DSD":
                            sdmxInput = wsModel.GetDataStructure(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "DATA_PROVIDER_SCHEME":
                            sdmxInput = wsModel.GetDataProviderScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "DATA_CONSUMER_SCHEME":
                            sdmxInput = wsModel.GetDataConsumerScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "ORGANIZATION_UNIT_SCHEME":
                            sdmxInput = wsModel.GetOrganisationUnitScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "STRUCTURE_SET":
                            sdmxInput = wsModel.GetStructureSet(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "CONTENT_CONSTRAINT":
                            sdmxInput = wsModel.GetContentConstraint(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "AGENCY_SCHEME":
                            sdmxInput = wsModel.GetAgencyScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;
                        case "HIERARCHICAL_CODELIST":
                            sdmxInput = wsModel.GetHcl(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                            break;

                    }
                    structuresGrid.DataSourceID = null;
                    structuresGrid.DataBind();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("error: " + ex.Message);

                }

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
                btnSaveMemoryCategorization.Visible = true;
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

                btnSaveMemoryCategorization.Visible = false;
                txt_uri.Enabled = false;
                txt_valid_from.Enabled = false;
                txt_valid_to.Enabled = false;
                chk_isFinal.Enabled = false;
                ddlAvailableStructures.Enabled = false;
                ddlCategorySchemeList.Enabled = false;
                btnOpenTreeDiv.Enabled = false;
                btnOpenGridDiv.Enabled = false;
                AnnotationGeneralControl.EditMode = false;
                txt_description_locale.Visible = true;
                txt_name_locale.Visible = true;
                txt_all_names.Visible = true;
                txt_all_description.Visible = true;
            }
        }

        private void SetEditingControl()
        {
            txt_id.Enabled = true;
            txt_version.Enabled = true;
            txt_uri.Enabled = true;
            txt_valid_from.Enabled = true;
            txt_valid_to.Enabled = true;
            txt_name_locale.Enabled = true;
            txt_description_locale.Enabled = true;
            cmb_agencies.Enabled = true;
            chk_isFinal.Enabled = true;
            pnlEditName.Visible = true;
            pnlEditDescription.Visible = true;
            btnSaveMemoryCategorization.Visible = true;

            // Svuoto le griglie di name e description
            if (Request["ACTION"] == "INSERT" && !Page.IsPostBack)
            {
                AddTextName.ClearTextObjectList();
                AddTextDescription.ClearTextObjectList();
                AnnotationGeneralControl.ClearAnnotationsSession();
            }
        }

        private void SetInitControls()
        {
            AddTextName.TType = TextType.NAME;
            AddTextName.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeList;

            AddTextDescription.TType = TextType.DESCRIPTION;
            AddTextDescription.ArtefactType = Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.CodeList;

            /*AddTextName_new.TType = TextType.NAME;
            AddTextDescription_new.TType = TextType.DESCRIPTION;

            AddTextName_update.TType = TextType.NAME;
            AddTextDescription_update.TType = TextType.DESCRIPTION;*/
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

        protected void ddlCategorySchemeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] splittedElementsOfCategoryScheme = ddlCategorySchemeList.Text.Split('-');
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(splittedElementsOfCategoryScheme[0], splittedElementsOfCategoryScheme[1], splittedElementsOfCategoryScheme[2]), false, false);
            ICategorySchemeObject categoryScheme = sdmxInput.CategorySchemes.First();

            TreeView1.Nodes.Clear();

            foreach (var category in categoryScheme.Items)
            {
                TreeNode node = new TreeNode(string.Format("[ {0} ] {1}", category.Id, _localizedUtils.GetNameableName(category)));
                //node.Value = string.Format( "[ {0} ] {1}", category.Id, _localizedUtils.GetNameableName( category ) );
                node.Value = category.Id;
                node.Text = string.Format("[ {0} ] {1}", category.Id, _localizedUtils.GetNameableName(category));
                node.SelectAction = TreeNodeSelectAction.Select;
                CreateTreeWithRecursion(category, node);
                TreeView1.Nodes.Add(node);
            }

            TreeView1.CollapseAll();
            Session["tmpCategorySchemeReference"] = null;
            txtSelectedCategory.Text = string.Empty;
            Utils.AppendScript("location.href='#structure';");
        }

        protected void btnOpenTreeDiv_Click(object sender, EventArgs e)
        {
            Utils.AppendScript("openP('treeViewDiv',650);");
            Utils.AppendScript("location.href= '#structure';");
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            txtSelectedCategory.Text = GetPathNode(TreeView1.SelectedNode);
            Session["tmpCategoryReference"] = GetPathNode(TreeView1.SelectedNode);
            Session["tmpCategorySchemeReference"] = ddlCategorySchemeList.Text;
            txtSelectedCategory.Visible = true;
            lblSelectedCategory.Visible = true;
            Utils.AppendScript("location.href= '#structure';");
            Utils.ForceBlackClosing();
        }

        private string GetPathNode(TreeNode treeNode, string pathNode = "")
        {
            return treeNode.Value;

            //if (pathNode != "")
            //    pathNode = treeNode.Value + "." + pathNode;
            //else
            //    pathNode = treeNode.Value;

            //if (treeNode.Parent != null)
            //    pathNode= GetPathNode(treeNode.Parent, pathNode);

            //return pathNode;
        }

        protected void btnOpenGridDiv_Click(object sender, EventArgs e)
        {
            Utils.AppendScript("openP('gridDiv',650);");
            Utils.AppendScript("location.href= '#structure';");
        }

        protected void structuresGrid_PageIndexChanged(object sender, EventArgs e)
        {
            // NULL
        }

        protected void structuresGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            structuresGrid.PageSize = 12;
            structuresGrid.PageIndex = e.NewPageIndex;
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput;
            switch (ddlAvailableStructures.Text)
            {
                case "CODELIST":
                    sdmxInput = wsModel.GetCodeList(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<ICodelistObject> codelist = sdmxInput.Codelists.ToList();
                    structuresGrid.DataSource = codelist;
                    break;
                case "CONCEPT_SCHEME":
                    sdmxInput = wsModel.GetConceptScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IConceptSchemeObject> conceptScheme = sdmxInput.ConceptSchemes.ToList();
                    structuresGrid.DataSource = conceptScheme;
                    break;
                case "CATEGORY_SCHEME":
                    sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<ICategorySchemeObject> categorySchemeFound = sdmxInput.CategorySchemes.ToList();
                    structuresGrid.DataSource = categorySchemeFound;
                    break;
                case "AGENCY_SCHEME":
                    sdmxInput = wsModel.GetAgencyScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IAgencyScheme> agencyScheme = sdmxInput.AgenciesSchemes.ToList();
                    structuresGrid.DataSource = agencyScheme;
                    break;
                case "DATA_PROVIDER_SCHEME":
                    sdmxInput = wsModel.GetDataProviderScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataProviderScheme> dataProviderScheme = sdmxInput.DataProviderSchemes.ToList();
                    structuresGrid.DataSource = dataProviderScheme;
                    break;
                case "DATA_CONSUMER_SCHEME":
                    sdmxInput = wsModel.GetDataConsumerScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataConsumerScheme> dataConsumerScheme = sdmxInput.DataConsumerSchemes.ToList();
                    structuresGrid.DataSource = dataConsumerScheme;
                    break;
                case "ORGANIZATION_UNIT_SCHEME":
                    sdmxInput = wsModel.GetOrganisationUnitScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IOrganisationUnitSchemeObject> organizationUnitScheme = sdmxInput.OrganisationUnitSchemes.ToList();
                    structuresGrid.DataSource = organizationUnitScheme;
                    break;
                case "DSD":
                    sdmxInput = wsModel.GetDataStructure(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataStructureObject> dataStructure = sdmxInput.DataStructures.ToList();
                    structuresGrid.DataSource = dataStructure;
                    break;
                case "DATAFLOW":
                    sdmxInput = wsModel.GetDataFlow(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataflowObject> dataFlow = sdmxInput.Dataflows.ToList();
                    structuresGrid.DataSource = dataFlow;
                    break;
                case "STRUCTURE_SET":
                    sdmxInput = wsModel.GetStructureSet(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IStructureSetObject> structureSet = sdmxInput.StructureSets.ToList();
                    structuresGrid.DataSource = structureSet;
                    break;
                case "CONTENT_CONSTRAINT":
                    sdmxInput = wsModel.GetContentConstraint(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IContentConstraintObject> contentConstraint = sdmxInput.ContentConstraintObjects.ToList();
                    structuresGrid.DataSource = contentConstraint;
                    break;
            }
            structuresGrid.DataBind();
            Utils.AppendScript("openP('gridDiv',650);");
            Utils.AppendScript("location.href= '#structure';");
        }

        protected void structuresGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper().Equals("SELECT"))
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string id = ((Label)(structuresGrid.Rows[rowIndex].Cells[0].Controls[1])).Text;
                string agencyId = ((Label)(structuresGrid.Rows[rowIndex].Cells[1].Controls[1])).Text;
                string version = ((Label)(structuresGrid.Rows[rowIndex].Cells[2].Controls[1])).Text;
                string name = ((Label)(structuresGrid.Rows[rowIndex].Cells[3].Controls[1])).Text;

                string objectSelected = string.Format("{0}-{1}-{2}", id, agencyId, version);
                txtSelectedItem.Text = objectSelected;
                lblSelectedItem.Visible = true;
                txtSelectedItem.Visible = true;

                Session["tmpKindOfStructure"] = ddlAvailableStructures.Text;
                Session["tmpReferenceOfTheObject"] = string.Format("{0}-{1}-{2}", id, agencyId, version);

                Utils.ForceBlackClosing();

            }
            Utils.AppendScript("location.href= '#structure';");
        }

        protected void ddlAvailableStructures_SelectedIndexChanged(object sender, EventArgs e)
        {
            WSModel wsModel = new WSModel();
            ISdmxObjects sdmxInput;
            switch (ddlAvailableStructures.Text)
            {
                case "CODELIST":
                    sdmxInput = wsModel.GetCodeList(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<ICodelistObject> codelist = sdmxInput.Codelists.ToList();
                    structuresGrid.DataSource = codelist;
                    break;
                case "CONCEPT_SCHEME":
                    sdmxInput = wsModel.GetConceptScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IConceptSchemeObject> conceptScheme = sdmxInput.ConceptSchemes.ToList();
                    structuresGrid.DataSource = conceptScheme;
                    break;
                case "CATEGORY_SCHEME":
                    sdmxInput = wsModel.GetCategoryScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<ICategorySchemeObject> categorySchemeFound = sdmxInput.CategorySchemes.ToList();
                    structuresGrid.DataSource = categorySchemeFound;
                    break;
                case "AGENCY_SCHEME":
                    sdmxInput = wsModel.GetAgencyScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IAgencyScheme> agencyScheme = sdmxInput.AgenciesSchemes.ToList();
                    structuresGrid.DataSource = agencyScheme;
                    break;
                case "DATA_PROVIDER_SCHEME":
                    sdmxInput = wsModel.GetDataProviderScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataProviderScheme> dataProviderScheme = sdmxInput.DataProviderSchemes.ToList();
                    structuresGrid.DataSource = dataProviderScheme;
                    break;
                case "DATA_CONSUMER_SCHEME":
                    sdmxInput = wsModel.GetDataConsumerScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataConsumerScheme> dataConsumerScheme = sdmxInput.DataConsumerSchemes.ToList();
                    structuresGrid.DataSource = dataConsumerScheme;
                    break;
                case "ORGANIZATION_UNIT_SCHEME":
                    sdmxInput = wsModel.GetOrganisationUnitScheme(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IOrganisationUnitSchemeObject> organizationUnitScheme = sdmxInput.OrganisationUnitSchemes.ToList();
                    structuresGrid.DataSource = organizationUnitScheme;
                    break;
                case "DSD":
                    sdmxInput = wsModel.GetDataStructure(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataStructureObject> dataStructure = sdmxInput.DataStructures.ToList();
                    structuresGrid.DataSource = dataStructure;
                    break;
                case "DATAFLOW":
                    sdmxInput = wsModel.GetDataFlow(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IDataflowObject> dataFlow = sdmxInput.Dataflows.ToList();
                    structuresGrid.DataSource = dataFlow;
                    break;
                case "STRUCTURE_SET":
                    sdmxInput = wsModel.GetStructureSet(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IStructureSetObject> structureSet = sdmxInput.StructureSets.ToList();
                    structuresGrid.DataSource = structureSet;
                    break;
                case "CONTENT_CONSTRAINT":
                    sdmxInput = wsModel.GetContentConstraint(new ArtefactIdentity(string.Empty, string.Empty, string.Empty), true, false);
                    List<IContentConstraintObject> contentConstraint = sdmxInput.ContentConstraintObjects.ToList();
                    structuresGrid.DataSource = contentConstraint;
                    break;
            }
            structuresGrid.DataSourceID = null;
            structuresGrid.DataBind();
            lblSelectedItem.Text = string.Empty;
            txtSelectedItem.Text = string.Empty;
            Session["tmpReferenceOfTheObject"] = null;
            Utils.AppendScript("location.href='#structure'");
        }
    }

}