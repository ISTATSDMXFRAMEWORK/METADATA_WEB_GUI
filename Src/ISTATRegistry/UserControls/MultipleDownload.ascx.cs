using ISTAT.Entity;
using ISTAT.WSDAL;
using ISTATRegistry.IRServiceReference;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.UserControls
{
    public partial class MultipleDownload : System.Web.UI.UserControl
    {
        #region Public Props

        public GridView ucGridview { get; set; }

        public string ucArtefactType
        {
            get
            {
                return lblArtefactType.Text;
            }
            set
            {
                lblArtefactType.Text = value;
            }
        }


        #endregion

        #region Private Props

        private DotStatProperties _dotStatProp;
        private List<ArtefactIdentity> _lArtefacts;


        #endregion

        #region Events

        protected override void OnPreRender(EventArgs e)
        {
            if (cmbWSExport.Items.Count <= 0)
            {
                // se utente autenticato e visibilità su agency da esportare
                IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;

                cmbDownloadType.Items.Add(new ListItem("Web Service", "Web Service"));

                Utils.PopulateCmbExportEndPoint(cmbWSExport);

                cmbWSExport.Items.Insert(0, "");

                hdnWsSource.Value = ((EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT]).Name;

                cmbWSExport.Items.Remove(hdnWsSource.Value);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            //_localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            //_entityMapper = new EntityMapper(Utils.LocalizedLanguage);

            _dotStatProp = new DotStatProperties()
            {
                Server = txtServer.Text,
                Directory = txtDirectory.Text,
                Theme = txtTheme.Text,
                ContactName = txtContactName.Text,
                ContactDirection = txtContactDirection.Text,
                ContactEMail = txtContactEMail.Text,
                SecurityUserGroup = txtSecurityUser.Text,
                SecurityDomain = txtSecurityDomain.Text
            };

            if (cmbDownloadType.Items.Count == 0)
            {
                LoadLogic();
            }

            if (!IsPostBack)
            {
                EndPointElement epe = (EndPointElement)Session["WSEndPoint"];

                txtContactName.Text = epe.DotStatContactName;
                txtContactDirection.Text = epe.DotStatContactDirection;
                txtContactEMail.Text = epe.DotStatContactEMail;
                txtSecurityUser.Text = epe.DotStatSecurityUserGroup;
                txtSecurityDomain.Text = epe.DotStatSecurityDomain;
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            //switch (cmbDownloadType.SelectedItem.Value)
            //{
            //    case "SDMX20":
            //    case "SDMX21":
            //        DownloadSDMX();
            //        break;
            //    case "CSV":
            //        DownloadCSV();
            //        break;
            //    case ".STAT_Codelist":
            //    case ".STAT_DSD":
            //    case ".STAT_All":
            //        DownloadDotStat();
            //        break;
            //    case "RTF":
            //        DownloadRTF();
            //        break;
            //}

            //Utils.AppendScript("$.unblockUI();");
        }

        protected void btnMultipleDownload_Click(object sender, EventArgs e)
        {
            if (ucGridview.Rows.Count <= 0)
                return;

            _lArtefacts = new List<ArtefactIdentity>();

            foreach (GridViewRow gvr in ucGridview.Rows)
            {
                Label lblID = (Label)gvr.FindControl("lblID");
                Label lblAgency = (Label)gvr.FindControl("lblAgency");
                Label lblVersion = (Label)gvr.FindControl("lblVersion");
                CheckBox chkDownload = (CheckBox)gvr.FindControl("chkDownload");

                if (chkDownload.Checked)
                {
                    _lArtefacts.Add(new ArtefactIdentity(lblID.Text, lblAgency.Text, lblVersion.Text));
                }
            }

            switch (cmbDownloadType.SelectedItem.Value)
            {
                case "SDMX20":
                case "SDMX21":
                    DownloadSDMX();
                    break;
                case "CSV":
                    DownloadCSV();
                    break;
                case ".STAT_Codelist":
                case ".STAT_DSD":
                case ".STAT_All":
                    DownloadDotStat();
                    break;
                //case "RTF":
                //    DownloadRTF();
                //    break;
            }
        }

        #endregion

        #region Private Methods

        public void LoadLogic()
        {
            cmbDownloadType.Items.Clear();
            string artefactType = ucArtefactType.ToLower();


            if (artefactType != "structureset" && artefactType != "categorization" && artefactType != "contentconstraint")
            {
                cmbDownloadType.Items.Add(new ListItem("SDMX 2.0", "SDMX20"));
            }
            cmbDownloadType.Items.Add(new ListItem("SDMX 2.1", "SDMX21"));
            if (artefactType != "keyfamily" && artefactType != "contentconstraint" && artefactType != "categorization" && artefactType != "dataflow" && artefactType != "structureset" && artefactType != "hcl")
            {
                cmbDownloadType.Items.Add(new ListItem("CSV", "CSV"));
            }
            if (artefactType == "codelist")
            {
                cmbDownloadType.Items.Add(new ListItem(".STAT_Codelist", ".STAT_Codelist"));
            }

            if (artefactType.Equals("keyfamily"))
            {
                chkExportCodeListAndConcept.Visible = true;
                lblIncludeCodeListAndConceptScheme.Visible = true;
                cmbDownloadType.Items.Add(new ListItem(".STAT_DSD", ".STAT_DSD"));
                cmbDownloadType.Items.Add(new ListItem(".STAT_All", ".STAT_All"));

                cmbDownloadType.Items.Add(new ListItem("RTF", "RTF"));
                //cmbDownloadType.Items.Add(new ListItem("RTFWP" "RTFWP"));
            }
            else
            {
                chkExportCodeListAndConcept.Visible = false;
                lblIncludeCodeListAndConceptScheme.Visible = false;
            }

        }

        private void DownloadDotStat()
        {
            if (ucArtefactType.ToUpper() == "CODELIST")
                DownloadDotStatCodelist();
            //else
            //    DownloadDotStatDSD();
        }

        //private void DownloadDotStatDSD()
        //{
        //    IOUtils file = new IOUtils();
        //    ISdmxObjects sdmxObjects = null;

        //    sdmxObjects = GetSdmxObjectsWithRef();

        //    if (sdmxObjects == null)
        //        return;

        //    file.SaveDotSTATFile(sdmxObjects, GetDotSTATExportType(), _dotStatProp);
        //}

        private void DownloadDotStatCodelist()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;

            sdmxObjects = GetSdmxObjects();

            if (sdmxObjects == null)
                return;

            List<ICodelistObject> lCL = new List<ICodelistObject>();

            foreach (ICodelistObject cl in sdmxObjects.Codelists)
            {
                lCL.Add(cl);
            }

            file.SaveMultipleDotSTATCodelistFile(lCL, _dotStatProp);

            //file.SaveDotSTATCodelistFile(sdmxObjects.Codelists.First(), _dotStatProp);
        }

        private void DownloadSDMX()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;

            sdmxObjects = GetSdmxObjects();

            if (sdmxObjects != null)
                file.SaveSDMXFile(sdmxObjects, sdmxVersion, GetFilename());
        }

        private void DownloadCSV()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;
            List<DataTable> dts = new List<DataTable>();
            List<string> sNames = new List<string>();

            sdmxObjects = GetSdmxObjects();

            if (sdmxObjects == null)
                return;

            PopolateDataTable(dts, sdmxObjects, sNames);

            string mySeparator = txtSeparator.Text.Trim().Equals(string.Empty) ? ";" : txtSeparator.Text.Trim();

            file.SaveMultipleCSVFile(dts, sNames, mySeparator, txtDelimiter.Text);
        }


        private void PopolateDataTable(List<DataTable> dts,  ISdmxObjects sdmxObjects, List<string> sNames)
        {
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            DataTable dt;

            switch (ucArtefactType.ToUpper())
            {
                case "CODELIST":
                    foreach (ICodelistObject codelist in sdmxObjects.Codelists)
                    {
                        dt = new DataTable();

                        AddExportColumns(dt);

                        sNames.Add(String.Format("Codelist-{0}_{1}_{2}.csv", codelist.Id, codelist.AgencyId, codelist.Version));

                        foreach (ICode code in codelist.Items)
                        {
                            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription(code), code.ParentCode);
                        }

                        dts.Add(dt);
                    }
                    break;
                //case "ConceptScheme":
                //    foreach (IConceptSchemeObject cs in sdmxObjects.ConceptSchemes)
                //    {
                //        foreach (IConceptObject concept in cs.Items)
                //        {
                //            dt.Rows.Add(concept.Id, localizedUtils.GetNameableName(concept), localizedUtils.GetNameableDescription(concept), concept.ParentConcept);
                //        }
                //        break;
                //    }
                //    break;
                //case "CategoryScheme":
                //    foreach (ICategorySchemeObject cs in sdmxObjects.CategorySchemes)
                //    {
                //        foreach (ICategoryObject code in cs.Items)
                //        {
                //            string completeSequence = code.Parent.ToString().Split('=')[1].Split(')')[1];
                //            if (!completeSequence.Equals(string.Empty))
                //            {
                //                completeSequence = completeSequence.Remove(0, 1);
                //            }
                //            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription(code), completeSequence);
                //            if (code.Items.Count != 0)
                //            {
                //                foreach (ICategoryObject subCode in code.Items)
                //                {
                //                    RecursiveOnItems(subCode, ref dt);
                //                }
                //            }
                //        }
                //        break;
                //    }
                //    break;
                //case "DataFlow":
                //    ISdmxObjects catObjects = GetCatObjects();
                //    foreach (IDataflowObject dataFlow in sdmxObjects.Dataflows)
                //    {
                //        dt.Rows.Add("DataflowID", dataFlow.Id);
                //        dt.Rows.Add("AgencyID", dataFlow.AgencyId);
                //        dt.Rows.Add("Version", dataFlow.Version);
                //        dt.Rows.Add("Name", localizedUtils.GetNameableName(dataFlow));
                //        dt.Rows.Add("KeyFamilyID", dataFlow.DataStructureRef.MaintainableId);
                //        dt.Rows.Add("KeyFamilyAgencyID", dataFlow.DataStructureRef.AgencyId);
                //        dt.Rows.Add("KeyFamilyVersion", dataFlow.DataStructureRef.Version);

                //        if (catObjects != null)
                //        {
                //            foreach (ICategorisationObject cat in catObjects.Categorisations)
                //            {
                //                if (cat.StructureReference.MaintainableId == dataFlow.Id &&
                //                    cat.StructureReference.AgencyId == dataFlow.AgencyId &&
                //                    cat.StructureReference.Version == dataFlow.Version)
                //                {
                //                    dt.Rows.Add("CategorySchemeID", cat.CategoryReference.MaintainableId);
                //                    dt.Rows.Add("CategorySchemeAgencyID", cat.CategoryReference.AgencyId);
                //                    dt.Rows.Add("CategorySchemeVersion", cat.CategoryReference.Version);
                //                    dt.Rows.Add("CategoryID", cat.CategoryReference.FullId);
                //                }
                //            }
                //        }
                //        break;
                //    }
                //    break;
                //case "Categorization":
                //    foreach (ICategorisationObject cat in sdmxObjects.Categorisations)
                //    {
                //        dt.Rows.Add(cat.Id, localizedUtils.GetNameableName(cat), "");
                //    }
                //    break;
                //case "AgencyScheme":
                //    foreach (IAgencyScheme agency in sdmxObjects.AgenciesSchemes)
                //    {
                //        foreach (IAgency agencyItem in agency.Items)
                //        {
                //            dt.Rows.Add(agencyItem.Id, localizedUtils.GetNameableName(agencyItem), localizedUtils.GetNameableDescription(agencyItem));
                //        }
                //        break;
                //    }
                //    break;
                //case "DataProviderScheme":
                //    foreach (IDataProviderScheme dataProviderScheme in sdmxObjects.DataProviderSchemes)
                //    {
                //        foreach (IDataProvider dataProviderSchemeItem in dataProviderScheme.Items)
                //        {
                //            dt.Rows.Add(dataProviderSchemeItem.Id, localizedUtils.GetNameableName(dataProviderSchemeItem), localizedUtils.GetNameableDescription(dataProviderSchemeItem));
                //        }
                //        break;
                //    }
                //    break;
                //case "DataConsumerScheme":
                //    foreach (IDataConsumerScheme dataConsumerScheme in sdmxObjects.DataConsumerSchemes)
                //    {
                //        foreach (IDataConsumer dataConsumerSchemeItem in dataConsumerScheme.Items)
                //        {
                //            dt.Rows.Add(dataConsumerSchemeItem.Id, localizedUtils.GetNameableName(dataConsumerSchemeItem), localizedUtils.GetNameableDescription(dataConsumerSchemeItem));
                //        }
                //        break;
                //    }
                //    break;
                //case "OrganizationUnitScheme":
                //    foreach (IOrganisationUnitSchemeObject organizationUnitScheme in sdmxObjects.OrganisationUnitSchemes)
                //    {
                //        foreach (IOrganisationUnit organizationUnitSchemeItem in organizationUnitScheme.Items)
                //        {
                //            dt.Rows.Add(organizationUnitSchemeItem.Id, localizedUtils.GetNameableName(organizationUnitSchemeItem), localizedUtils.GetNameableDescription(organizationUnitSchemeItem), organizationUnitSchemeItem.ParentUnit);
                //        }
                //        break;
                //    }
                //    break;
            }
        }

        private void AddExportColumns(DataTable dt)
        {
            switch (ucArtefactType.ToUpper())
            {
                case "CODELIST":
                case "CATEGORYSCHEME":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("ParentCode");
                    break;
                case "CONCEPTSCHEME":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("Parent");
                    break;
                case "DATAFLOW":
                    dt.Columns.Add("Field");
                    dt.Columns.Add("Value");
                    break;
                case "KEYFAMILY":
                    dt.Columns.Add("Type");
                    dt.Columns.Add("Concept");
                    dt.Columns.Add("Detail");
                    break;
                case "AGENCYSCHEME":
                case "DATAPROVIDERSCHEME":
                case "DATACONSUMERSCHEME":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    break;
                case "ORGANIZATIONUNITSCHEME":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("ParentUnit");
                    break;
            }
        }

        private string GetStrindDate()
        {
            return DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
        }

        private string GetFilename()
        {
            string sVersion = "";
            switch (sdmxVersion)
            {
                case StructureOutputFormatEnumType.Csv:
                    break;
                case StructureOutputFormatEnumType.Null:
                    break;
                case StructureOutputFormatEnumType.SdmxV21StructureDocument:
                    sVersion = "-v21";
                    break;
                case StructureOutputFormatEnumType.SdmxV2StructureDocument:
                    sVersion = "-v20";
                    break;
            }

            //string fileLang = Session[ISTATRegistry.SESSION_KEYS.KEY_LANG].ToString();

            return "MultipleSDMXFile_" + GetStrindDate();

            //return ucID + "_" + ucAgency + "_" + ucVersion + sVersion + (chkStub.Checked ? "-stub" : string.Empty) + "." + fileLang;
        }

        private StructureOutputFormatEnumType sdmxVersion
        {
            get
            {
                StructureOutputFormatEnumType retVersion = StructureOutputFormatEnumType.Null;

                switch (cmbDownloadType.SelectedItem.Value)
                {
                    case "SDMX20":
                        retVersion = StructureOutputFormatEnumType.SdmxV2StructureDocument;
                        break;
                    case "SDMX21":
                        retVersion = StructureOutputFormatEnumType.SdmxV21StructureDocument;
                        break;
                }
                return retVersion;
            }
        }

        private ISdmxObjects GetSdmxObjects()
        {
            ISdmxObjects sdmxObjects = null;
            ISdmxObjects sdmxObjectsAll = new SdmxObjectsImpl();
            WSModel dal = new WSModel();

            switch (ucArtefactType.ToUpper())
            {
                case "CODELIST":

                    foreach (ArtefactIdentity art in _lArtefacts)
                    {
                        sdmxObjects = dal.GetCodeList(art, false, false);

                        if (sdmxObjects.HasCodelists)
                            sdmxObjectsAll.AddCodelist(sdmxObjects.Codelists.First());
                    }
                    break;
                //case "CONCEPTSCHEME":
                //    sdmxObjects = dal.GetConceptScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "CATEGORYSCHEME":
                //    sdmxObjects = dal.GetCategoryScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "DATAFLOW":
                //    sdmxObjects = dal.GetDataFlow(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "KEYFAMILY":
                //    if (chkExportCodeListAndConcept.Checked || _dsdWithRef)
                //    {
                //        sdmxObjects = dal.GetDataStructureWithRef(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    }
                //    else
                //    {
                //        sdmxObjects = dal.GetDataStructure(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    }
                //    break;
                //case "CATEGORIZATION":
                //    sdmxObjects = dal.GetCategorisation(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "AGENCYSCHEME":
                //    sdmxObjects = dal.GetAgencyScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "DATAPROVIDERSCHEME":
                //    sdmxObjects = dal.GetDataProviderScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "DATACONSUMERSCHEME":
                //    sdmxObjects = dal.GetDataConsumerScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "ORGANIZATIONUNITSCHEME":
                //    sdmxObjects = dal.GetOrganisationUnitScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "CONTENTCONSTRAINT":
                //    sdmxObjects = dal.GetContentConstraint(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "STRUCTURESET":
                //    sdmxObjects = dal.GetStructureSet(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;
                //case "HCL":
                //    sdmxObjects = dal.GetHcl(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                //    break;

                default:
                    return null;
            }
            return sdmxObjectsAll;
        }

        #endregion

    }
}