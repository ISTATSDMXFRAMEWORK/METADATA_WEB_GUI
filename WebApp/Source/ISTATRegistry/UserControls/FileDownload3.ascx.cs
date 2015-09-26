using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using ISTAT.WSDAL;
using System.Data;
using System.Threading;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using ISTAT.Entity;
using ISTATUtils;
using System.Diagnostics;

namespace ISTATRegistry.UserControls
{
    public partial class FileDownload3 : System.Web.UI.UserControl
    {
        #region Public Props

        public string ucID
        {
            get
            {
                return lblID.Text;
            }
            set
            {
                lblID.Text = value;
            }
        }

        public string ucAgency
        {
            get
            {
                return lblAgency.Text;
            }
            set
            {
                lblAgency.Text = value;
            }
        }

        public string ucVersion
        {
            get
            {
                return lblVersion.Text;
            }
            set
            {
                lblVersion.Text = value;
            }
        }

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

        #region Events

        protected override void OnPreRender(EventArgs e)
        {
            lblArtefactID.Text = ucID + "-" + ucAgency + "-" + ucVersion;
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            if ( cmbDownloadType.Items.Count == 0 )
            {
                if ( ucArtefactType.ToLower() != "structureset" && ucArtefactType.ToLower() != "categorization" && ucArtefactType.ToLower() != "contentconstraint" )
                {
                    cmbDownloadType.Items.Add(new ListItem("SDMX 2.0", "SDMX20"));
                }
                cmbDownloadType.Items.Add(new ListItem("SDMX 2.1", "SDMX21"));
                if (ucArtefactType.ToLower() != "keyfamily" && ucArtefactType.ToLower() != "contentconstraint" && ucArtefactType.ToLower() != "categorization" && ucArtefactType.ToLower() != "dataflow" && ucArtefactType.ToLower() != "structureset" && ucArtefactType.ToLower() != "hcl")
                {
                    cmbDownloadType.Items.Add(new ListItem("CSV", "CSV"));
                }
                if (ucArtefactType == "CodeList")
                {
                    cmbDownloadType.Items.Add(new ListItem(".STAT_Codelist", ".STAT_Codelist"));
                }

                if ( ucArtefactType.Equals( "KeyFamily" ) )
                {
                    chkExportCodeListAndConcept.Visible = true;
                    lblIncludeCodeListAndConceptScheme.Visible = true;
                    cmbDownloadType.Items.Add(new ListItem(".STAT_DSD", ".STAT_DSD"));
                    cmbDownloadType.Items.Add(new ListItem(".STAT_All", ".STAT_All"));
                }
                else
                {
                    chkExportCodeListAndConcept.Visible = false;
                    lblIncludeCodeListAndConceptScheme.Visible = false;
                }   
            }

            lblArtefactID.DataBind();
            lblTitle.DataBind();
            btnDownload.DataBind();
            lblStub.DataBind();
            lblExportType.DataBind();
            btnDownload.DataBind();
            lblIncludeCodeListAndConceptScheme.DataBind(); 
            lblSeparator.DataBind();
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
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
            }

            Utils.AppendScript("location.reload();");
        }


        #endregion 

        #region Methods

        private ISdmxObjects GetSdmxObjects()
        {
            ISdmxObjects sdmxObjects = null;
            WSModel dal = new WSModel();

            switch (ucArtefactType)
            {
                case "CodeList":
                    sdmxObjects = dal.GetCodeList(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked,false);
                    break;
                case "ConceptScheme":
                    sdmxObjects = dal.GetConceptScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "CategoryScheme":
                    sdmxObjects = dal.GetCategoryScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "DataFlow":
                    sdmxObjects = dal.GetDataFlow(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "KeyFamily":

                        if ( chkExportCodeListAndConcept.Checked )
                        {
                            sdmxObjects = dal.GetDataStructureWithRef(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                        }
                        else
                        {
                            sdmxObjects = dal.GetDataStructure(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                        }
                    break;
                case "Categorization":
                    sdmxObjects = dal.GetCategorisation(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "AgencyScheme":
                    sdmxObjects = dal.GetAgencyScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "DataProviderScheme":
                    sdmxObjects = dal.GetDataProviderScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "DataConsumerScheme":
                    sdmxObjects = dal.GetDataConsumerScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "OrganizationUnitScheme":
                    sdmxObjects = dal.GetOrganisationUnitScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "ContentConstraint":
                    sdmxObjects = dal.GetContentConstraint(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "StructureSet":
                    sdmxObjects = dal.GetStructureSet(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "Hcl":
                    sdmxObjects = dal.GetHcl(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;

                default:
                    return null;
            }

            return sdmxObjects;
        }

        private ISdmxObjects GetCatObjects()
        {
            try
            {
                WSModel wsModel = new WSModel();
                return wsModel.GetAllCategorisation(false);
            }
            catch (Exception ex)
            {
                return null;
            }
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
            DataTable dt = new DataTable();

            sdmxObjects = GetSdmxObjects();

            if (sdmxObjects == null)
                return;

            AddExportColumns(dt);
            PopolateDataTable(dt, sdmxObjects);

            string mySeparator = txtSeparator.Text.Trim().Equals( string.Empty ) ? ";" : txtSeparator.Text.Trim();

            file.SaveCSVFile(dt, GetFilename(), mySeparator );
        }

        private void DownloadDotStat()
        {
            if (ucArtefactType == "CodeList")
                DownloadDotStatCodelist();
            else
                DownloadDotStatDSD();
        }

        private void DownloadDotStatDSD()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;

            sdmxObjects = GetSdmxObjectsWithRef();

            if (sdmxObjects == null)
                return;

            file.SaveDotSTATFile(sdmxObjects, GetDotSTATExportType());
        }

        private void DownloadDotStatCodelist()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;

            sdmxObjects = GetSdmxObjects();

            if (sdmxObjects == null)
                return;

            file.SaveDotSTATCodelistFile(sdmxObjects.Codelists.First());
        }

        private ISdmxObjects GetSdmxObjectsWithRef()
        {
            ISdmxObjects sdmxObjects;
            ISdmxObjects sdmxObjectsTemp;
            bool stub;
            
            WSModel dal = new WSModel();
            sdmxObjects = GetSdmxObjects();

            stub = (GetDotSTATExportType() == DotStatExportType.DSD);


            foreach (IDimension dim in sdmxObjects.DataStructures.First().DimensionList.Dimensions)
            {
                if (dim.HasCodedRepresentation())
                {
                    var rep = dim.Representation.Representation;
                    sdmxObjectsTemp = dal.GetCodeList(new ArtefactIdentity(rep.MaintainableId, rep.AgencyId, rep.Version), stub,false);
                    sdmxObjects.AddCodelist(sdmxObjectsTemp.Codelists.First());
                }
            }

            foreach (IAttributeObject att in sdmxObjects.DataStructures.First().Attributes)
            {
                if (att.HasCodedRepresentation())
                {
                    var rep = att.Representation.Representation;
                    sdmxObjectsTemp = dal.GetCodeList(new ArtefactIdentity(rep.MaintainableId, rep.AgencyId, rep.Version), stub,false);
                    sdmxObjects.AddCodelist(sdmxObjectsTemp.Codelists.First());
                }
            }

            return sdmxObjects;
        }

        private DotStatExportType GetDotSTATExportType()
        {
            DotStatExportType retExp = DotStatExportType.ALL;

            switch (cmbDownloadType.SelectedItem.Value)
            {
                case ".STAT_Codelist":
                    retExp = DotStatExportType.CODELIST;
                    break;
                case ".STAT_DSD":
                    retExp = DotStatExportType.DSD;
                    break;
                case ".STAT_All":
                    retExp = DotStatExportType.ALL;
                    break;
            }

            return retExp;
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

            string fileLang = Session[ISTATRegistry.SESSION_KEYS.KEY_LANG].ToString();
            return ucID + "_" + ucAgency + "_" + ucVersion + sVersion + (chkStub.Checked ? "-stub" : string.Empty ) + "." + fileLang;
        }

        private void RecursiveOnItems( ICategoryObject code, ref DataTable dt )
        {
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            string completeSequence = code.Parent.ToString().Split( '=' )[1].Split( ')' )[1];
            if ( !completeSequence.Equals( string.Empty )  )
            {
                completeSequence = completeSequence.Remove( 0, 1 );
            }
            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription( code ), completeSequence );                   
            if ( code.Items.Count != 0 )
            {
                foreach ( ICategoryObject subCode in code.Items )
                {
                    RecursiveOnItems( subCode, ref dt );
                }
                return;
            }
            else
            {
                return;
            }
       }

        private void PopolateDataTable(DataTable dt, ISdmxObjects sdmxObjects)
        {
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);

            switch (ucArtefactType)
            {
                case "CodeList":
                    foreach (ICodelistObject codelist in sdmxObjects.Codelists)
                    {
                        foreach (ICode code in codelist.Items)
                        {
                            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription(code), code.ParentCode);
                        }
                        break;
                    }
                    break;
                case "ConceptScheme":
                    foreach (IConceptSchemeObject cs in sdmxObjects.ConceptSchemes)
                    {
                        foreach (IConceptObject concept in cs.Items)
                        {
                            dt.Rows.Add(concept.Id, localizedUtils.GetNameableName(concept), localizedUtils.GetNameableDescription( concept ), concept.ParentConcept );
                        }
                        break;
                    }
                    break;
                case "CategoryScheme":
                    foreach (ICategorySchemeObject cs in sdmxObjects.CategorySchemes)
                    {
                        foreach (ICategoryObject code in cs.Items)
                        {
                            string completeSequence = code.Parent.ToString().Split( '=' )[1].Split( ')' )[1];
                            if ( !completeSequence.Equals( string.Empty )  )
                            {
                                completeSequence = completeSequence.Remove( 0, 1 );
                            }
                            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription( code ), completeSequence );
                            if ( code.Items.Count != 0 )
                            {
                                foreach ( ICategoryObject subCode in code.Items )
                                {
                                    RecursiveOnItems( subCode, ref dt );
                                }
                            }
                        }
                        break;
                    }
                    break;
                case "DataFlow":
                    ISdmxObjects catObjects = GetCatObjects();
                    foreach (IDataflowObject dataFlow in sdmxObjects.Dataflows)
                    {
                        dt.Rows.Add("DataflowID", dataFlow.Id);
                        dt.Rows.Add("AgencyID", dataFlow.AgencyId);
                        dt.Rows.Add("Version", dataFlow.Version);
                        dt.Rows.Add("Name", localizedUtils.GetNameableName(dataFlow));
                        dt.Rows.Add("KeyFamilyID", dataFlow.DataStructureRef.MaintainableId);
                        dt.Rows.Add("KeyFamilyAgencyID", dataFlow.DataStructureRef.AgencyId);
                        dt.Rows.Add("KeyFamilyVersion", dataFlow.DataStructureRef.Version);

                        if (catObjects != null)
                        {
                            foreach (ICategorisationObject cat in catObjects.Categorisations)
                            {
                                if (cat.StructureReference.MaintainableId == dataFlow.Id &&
                                    cat.StructureReference.AgencyId == dataFlow.AgencyId &&
                                    cat.StructureReference.Version == dataFlow.Version)
                                {
                                    dt.Rows.Add("CategorySchemeID", cat.CategoryReference.MaintainableId);
                                    dt.Rows.Add("CategorySchemeAgencyID", cat.CategoryReference.AgencyId);
                                    dt.Rows.Add("CategorySchemeVersion", cat.CategoryReference.Version);
                                    dt.Rows.Add("CategoryID", cat.CategoryReference.FullId);
                                }
                            }
                        }
                        break;
                    }
                    break;
                case "Categorization":
                    foreach (ICategorisationObject cat in sdmxObjects.Categorisations)
                    {
                        dt.Rows.Add(cat.Id, localizedUtils.GetNameableName(cat), "");
                    }
                    break;
                case "AgencyScheme":
                    foreach (IAgencyScheme agency in sdmxObjects.AgenciesSchemes)
                    {
                        foreach (IAgency agencyItem in agency.Items)
                        {
                            dt.Rows.Add(agencyItem.Id, localizedUtils.GetNameableName(agencyItem), localizedUtils.GetNameableDescription( agencyItem ) );
                        }
                        break;
                    }
                    break;
                case "DataProviderScheme":
                    foreach (IDataProviderScheme dataProviderScheme in sdmxObjects.DataProviderSchemes)
                    {
                        foreach (IDataProvider dataProviderSchemeItem in dataProviderScheme.Items)
                        {
                            dt.Rows.Add(dataProviderSchemeItem.Id, localizedUtils.GetNameableName(dataProviderSchemeItem), localizedUtils.GetNameableDescription( dataProviderSchemeItem ) );
                        }
                        break;
                    }
                    break;
                case "DataConsumerScheme":
                    foreach (IDataConsumerScheme dataConsumerScheme in sdmxObjects.DataConsumerSchemes)
                    {
                        foreach (IDataConsumer dataConsumerSchemeItem in dataConsumerScheme.Items)
                        {
                            dt.Rows.Add(dataConsumerSchemeItem.Id, localizedUtils.GetNameableName(dataConsumerSchemeItem), localizedUtils.GetNameableDescription( dataConsumerSchemeItem ) );
                        }
                        break;
                    }
                    break;
                case "OrganizationUnitScheme":
                    foreach (IOrganisationUnitSchemeObject organizationUnitScheme in sdmxObjects.OrganisationUnitSchemes)
                    {
                        foreach (IOrganisationUnit organizationUnitSchemeItem in organizationUnitScheme.Items)
                        {
                            dt.Rows.Add(organizationUnitSchemeItem.Id, localizedUtils.GetNameableName(organizationUnitSchemeItem), localizedUtils.GetNameableDescription( organizationUnitSchemeItem ), organizationUnitSchemeItem.ParentUnit );
                        }
                        break;
                    }
                    break;
            }
        }

        private void GetCategoryParent(ref DataTable dt, ICategoryObject categoryObject)
        {
            foreach (ICategoryObject category in categoryObject.Items)
            {
                dt.Rows.Add(category.Id, category.Name, category.IdentifiableParent.Id);
                GetCategoryParent(ref dt, category);
            }
        }

        private void AddExportColumns(DataTable dt)
        {
            switch (ucArtefactType)
            {
                case "CodeList":
                case "CategoryScheme":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add( "Description" );
                    dt.Columns.Add("ParentCode");
                    break;
                case "ConceptScheme":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("Parent");
                    break;
                case "DataFlow":
                    dt.Columns.Add("Field");
                    dt.Columns.Add("Value");
                    break;
                case "KeyFamily":
                    dt.Columns.Add("Type");
                    dt.Columns.Add("Concept");
                    dt.Columns.Add("Detail");
                    break;
                case "AgencyScheme":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    break;
                case "DataProviderScheme":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    break;
                 case "DataConsumerScheme":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    break;
                 case "OrganizationUnitScheme":
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("ParentUnit");
                    break;
            }
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

        #endregion

    }
}