using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ISTAT.WSDAL;
using System.Xml;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Api.Model;
using ISTATUtils;
using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System.Diagnostics;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Mapping;
using System.Threading;
using ISTATRegistry.IRServiceReference;

namespace ISTATRegistry
{
    public partial class UploadStructure : ISTATRegistry.Classes.ISTATWebPage
    {
        #region PRIVATE PROP
        private LocalizedUtils localizedUtils;
        private const string IMPORTED_ITEMS_STRING = "IMPORTED_ITEMS";
        private const string IMPORTED_SDMX_OBJECT = "IMPORTED_SDMX_OBJECT";
        private const string REPORT_ITEMS = "REPORT_ITEMS";
        #endregion

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Server.ScriptTimeout = 3600;
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                lblWait.DataBind();
                lbl.DataBind();
                btnConfirmImport.DataBind();
                btnUploadFile.DataBind();
                lblLoadFileAsmx.DataBind();
                lblSelectAll.DataBind();
                lblNoItemsAllowed.DataBind();
            }
            localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridView.PageIndex = e.NewPageIndex;
            if (Session[IMPORTED_ITEMS_STRING] != null)
            {
                List<ImportedItem> importedItems = Session[IMPORTED_ITEMS_STRING] as List<ImportedItem>;
                if (importedItems != null)
                {
                    gridView.DataSource = importedItems;
                    gridView.DataBind();
                }
            }

        }

        protected void btnConfirmImport_Click(object sender, EventArgs e)
        {
            bool oneRowIsOk = false;
            GridViewRowCollection rows = gridView.Rows;

            foreach (GridViewRow row in rows)
            {
                if (((CheckBox)row.Cells[4].Controls[1]).Checked)
                {
                    oneRowIsOk = true;
                    break;
                }
            }

            if (oneRowIsOk)
            {
                WSModel wsModel = new WSModel();
                XmlDocument xDocStructure = new XmlDocument();
                XmlDocument xDocMessage = new XmlDocument();
                XmlDocument xRet;
                WSUtils utils = new WSUtils();
                List<ImportedItem> myItems = Session[IMPORTED_ITEMS_STRING] as List<ImportedItem>;
                ISdmxObjects sdmxObjects = (ISdmxObjects)Session[IMPORTED_SDMX_OBJECT];
                ISdmxObjects newSdmxObjects = new SdmxObjectsImpl();
                List<ImportedItem> reportItems = new List<ImportedItem>();
                string objectsSummary = string.Empty;

                foreach (GridViewRow row in gridView.Rows)
                {
                    string currentId = ((Label)row.Cells[0].Controls[1]).Text;
                    string currentAgency = ((Label)row.Cells[1].Controls[1]).Text;
                    string currentVersion = ((Label)row.Cells[2].Controls[1]).Text;
                    ImportedItem myCurrentItem = myItems.Find(item => item.ID.Equals(currentId) && item.Agency.Equals(currentAgency) && item.Version.Equals(currentVersion));

                    if (((CheckBox)row.Cells[4].Controls[1]).Checked)
                    {
                        switch (myCurrentItem._type)
                        {
                            case "CODELIST":
                                ICodelistObject tmpCodeList = sdmxObjects.Codelists.First(codelist => codelist.Id.Equals(currentId) && codelist.AgencyId.Equals(currentAgency) && codelist.Version.Equals(currentVersion));
                                newSdmxObjects.AddCodelist(tmpCodeList);
                                objectsSummary += string.Format(Resources.Messages.msg_codelist_imported, tmpCodeList.Id, tmpCodeList.AgencyId, tmpCodeList.Version);
                                break;
                            case "CONCEPT_SCHEME":
                                IConceptSchemeObject tmpConceptScheme = sdmxObjects.ConceptSchemes.First(conceptScheme => conceptScheme.Id.Equals(currentId) && conceptScheme.AgencyId.Equals(currentAgency) && conceptScheme.Version.Equals(currentVersion));
                                newSdmxObjects.AddConceptScheme(tmpConceptScheme);
                                objectsSummary += string.Format(Resources.Messages.msg_concept_scheme_imported, tmpConceptScheme.Id, tmpConceptScheme.AgencyId, tmpConceptScheme.Version);
                                break;
                            case "CATEGORY_SCHEME":
                                ICategorySchemeObject tmpCategoryScheme = sdmxObjects.CategorySchemes.First(categoryScheme => categoryScheme.Id.Equals(currentId) && categoryScheme.AgencyId.Equals(currentAgency) && categoryScheme.Version.Equals(currentVersion));
                                newSdmxObjects.AddCategoryScheme(tmpCategoryScheme);
                                objectsSummary += string.Format(Resources.Messages.msg_category_scheme_imported, tmpCategoryScheme.Id, tmpCategoryScheme.AgencyId, tmpCategoryScheme.Version);
                                break;
                            case "DSD":
                                IDataStructureObject tmpDataStructure = sdmxObjects.DataStructures.First(dataStructure => dataStructure.Id.Equals(currentId) && dataStructure.AgencyId.Equals(currentAgency) && dataStructure.Version.Equals(currentVersion));
                                newSdmxObjects.AddDataStructure(tmpDataStructure);
                                objectsSummary += string.Format(Resources.Messages.msg_data_structure_imported, tmpDataStructure.Id, tmpDataStructure.AgencyId, tmpDataStructure.Version);
                                break;
                            case "AGENCY_SCHEME":
                                IAgencyScheme tmpAgencyScheme = sdmxObjects.AgenciesSchemes.First(agencyScheme => agencyScheme.Id.Equals(currentId) && agencyScheme.AgencyId.Equals(currentAgency) && agencyScheme.Version.Equals(currentVersion));
                                newSdmxObjects.AddAgencyScheme(tmpAgencyScheme);
                                objectsSummary += string.Format(Resources.Messages.msg_agency_scheme_imported, tmpAgencyScheme.Id, tmpAgencyScheme.AgencyId, tmpAgencyScheme.Version);
                                break;
                            case "DATA_PROVIDER_SCHEME":
                                IDataProviderScheme tmpDataProviderScheme = sdmxObjects.DataProviderSchemes.First(dataProviderScheme => dataProviderScheme.Id.Equals(currentId) && dataProviderScheme.AgencyId.Equals(currentAgency) && dataProviderScheme.Version.Equals(currentVersion));
                                newSdmxObjects.AddDataProviderScheme(tmpDataProviderScheme);
                                objectsSummary += string.Format(Resources.Messages.msg_data_provider_scheme_imported, tmpDataProviderScheme.Id, tmpDataProviderScheme.AgencyId, tmpDataProviderScheme.Version);
                                break;
                            case "DATA_CONSUMER_SCHEME":
                                IDataConsumerScheme tmpDataConsumerScheme = sdmxObjects.DataConsumerSchemes.First(dataConsumerScheme => dataConsumerScheme.Id.Equals(currentId) && dataConsumerScheme.AgencyId.Equals(currentAgency) && dataConsumerScheme.Version.Equals(currentVersion));
                                newSdmxObjects.AddDataConsumerScheme(tmpDataConsumerScheme);
                                objectsSummary += string.Format(Resources.Messages.msg_data_consumer_scheme_imported, tmpDataConsumerScheme.Id, tmpDataConsumerScheme.AgencyId, tmpDataConsumerScheme.Version);
                                break;
                            case "ORGANIZATION_UNIT_SCHEME":
                                IOrganisationUnitSchemeObject tmpOrganizationUnitScheme = sdmxObjects.OrganisationUnitSchemes.First(organizationUnitScheme => organizationUnitScheme.Id.Equals(currentId) && organizationUnitScheme.AgencyId.Equals(currentAgency) && organizationUnitScheme.Version.Equals(currentVersion));
                                newSdmxObjects.AddOrganisationUnitScheme(tmpOrganizationUnitScheme);
                                objectsSummary += string.Format(Resources.Messages.msg_organization_unit_scheme_imported, tmpOrganizationUnitScheme.Id, tmpOrganizationUnitScheme.AgencyId, tmpOrganizationUnitScheme.Version);
                                break;
                            case "STRUCTURE_SET":
                                IStructureSetObject tmpStructureSet = sdmxObjects.StructureSets.First(structureSet => structureSet.Id.Equals(currentId) && structureSet.AgencyId.Equals(currentAgency) && structureSet.Version.Equals(currentVersion));
                                newSdmxObjects.AddStructureSet(tmpStructureSet);
                                objectsSummary += string.Format(Resources.Messages.msg_structure_set_imported, tmpStructureSet.Id, tmpStructureSet.AgencyId, tmpStructureSet.Version);
                                break;
                            case "CONTENT_CONSTRAINT":
                                IContentConstraintObject tmpContentConstraint = sdmxObjects.ContentConstraintObjects.First(contentConstraint => contentConstraint.Id.Equals(currentId) && contentConstraint.AgencyId.Equals(currentAgency) && contentConstraint.Version.Equals(currentVersion));
                                newSdmxObjects.AddContentConstraintObject(tmpContentConstraint);
                                objectsSummary += string.Format(Resources.Messages.msg_content_constraint_imported, tmpContentConstraint.Id, tmpContentConstraint.AgencyId, tmpContentConstraint.Version);
                                break;
                            case "HIERARCHICAL_CODELIST":
                                IHierarchicalCodelistObject tmpHierarchicalCodelist = sdmxObjects.HierarchicalCodelists.First(hierarchicalCodelist => hierarchicalCodelist.Id.Equals(currentId) && hierarchicalCodelist.AgencyId.Equals(currentAgency) && hierarchicalCodelist.Version.Equals(currentVersion));
                                newSdmxObjects.AddHierarchicalCodelist(tmpHierarchicalCodelist);
                                objectsSummary += string.Format(Resources.Messages.msg_hierarchical_codelist_imported, tmpHierarchicalCodelist.Id, tmpHierarchicalCodelist.AgencyId, tmpHierarchicalCodelist.Version);    
                                break;
                            case "CATEGORISATION":
                                ICategorisationObject tmpCategorisation = sdmxObjects.Categorisations.First(cat => cat.Id.Equals(currentId) && cat.AgencyId.Equals(currentAgency) && cat.Version.Equals(currentVersion));
                                newSdmxObjects.AddCategorisation(tmpCategorisation);
                                objectsSummary += string.Format(Resources.Messages.msg_categorisation_imported, tmpCategorisation.Id, tmpCategorisation.AgencyId, tmpCategorisation.Version);
                                break;
                        }
                        reportItems.Add(myCurrentItem);
                    }
                }
                Session[REPORT_ITEMS] = reportItems;

                // Carico l'SDMXObject in un XML Message da passare al WS
                //xDocMessage = utils.GetXmlMessage(newSdmxObjects);

                newSdmxObjects = PopolateAnnotationID(newSdmxObjects);

                //Richiamo il metodo SubmitStructure per l'inserimento nel DB
                try
                {
                    xRet = wsModel.SubmitStructure(newSdmxObjects);
                }
                catch (Exception ex)
                {
                    Utils.ShowDialog(ex.Message);
                    Utils.ForceBlackClosing();
                    return;
                }

                string CheckError = Utils.GetXMLResponseError(xRet);

                if (CheckError != String.Empty)
                {
                    Utils.ShowDialog(CheckError);
                    Utils.ForceBlackClosing();
                    return;
                }

                // lblInfo.Text = CreateArtefactImportedString(sdmxObjects);
                lblInfo.Text = objectsSummary;
                gridView.DataSource = null;
                gridView.DataBind();
            }
            else
            {
                Utils.ShowDialog( Resources.Messages.err_at_least_one_row );
                Utils.AppendScript("openPopUp('importedItemsGridDiv', 600);");  
                return;
            }
            Utils.AppendScript("openPopUp('dialog-form', 600);");
            /*gridView.DataSource = myItems;
            gridView.DataBind();*/
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {

            if (uploadedFiles.HasFile)
            {
                User currentUser = Session[SESSION_KEYS.USER_DATA] as User;

                try
                {
                    //ResetSdmxSession();
                    WSModel wsModel = new WSModel();
                    XmlDocument xDocStructure = new XmlDocument();
                    XmlDocument xDocMessage = new XmlDocument();
                    XmlDocument xRet;
                    WSUtils utils = new WSUtils();
                    ISdmxObjects sdmxObjects;

                    string FilePath = SaveFile();

                    // Carico il file in un XMLDOcument
                    xDocStructure.Load(FilePath);

                    // Carico l'XMLDocument in un SDMXObjects
                    sdmxObjects = utils.GetSdmxObjectsFromXML(xDocStructure);

                    List<ImportedItem> items = new List<ImportedItem>();
                    bool itemIsOk;
                    foreach (ICodelistObject tmpCodeList in sdmxObjects.Codelists)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetCodeList(new ArtefactIdentity(tmpCodeList.Id, tmpCodeList.AgencyId, tmpCodeList.Version), true, false);
                            if (checkedObject.Codelists.Count > 0)
                            {
                                ICodelistObject checkedCodelist = checkedObject.Codelists.First();
                                if (checkedCodelist.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }

                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpCodeList.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpCodeList), tmpCodeList.Id, tmpCodeList.AgencyId, tmpCodeList.Version, "CODELIST", itemIsOk));
                        }
                    }

                    foreach (IConceptSchemeObject tmpConceptScheme in sdmxObjects.ConceptSchemes)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetConceptScheme(new ArtefactIdentity(tmpConceptScheme.Id, tmpConceptScheme.AgencyId, tmpConceptScheme.Version), true, false);
                            if (checkedObject.ConceptSchemes.Count > 0)
                            {
                                IConceptSchemeObject checkedConcepScheme = checkedObject.ConceptSchemes.First();
                                if (checkedConcepScheme.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }

                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpConceptScheme.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpConceptScheme), tmpConceptScheme.Id, tmpConceptScheme.AgencyId, tmpConceptScheme.Version, "CONCEPT_SCHEME", itemIsOk));
                        }
                    }

                    foreach (ICategorySchemeObject tmpCategoryScheme in sdmxObjects.CategorySchemes)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetCategoryScheme(new ArtefactIdentity(tmpCategoryScheme.Id, tmpCategoryScheme.AgencyId, tmpCategoryScheme.Version), true, false);
                            if (checkedObject.CategorySchemes.Count > 0)
                            {
                                ICategorySchemeObject checkedCategoryScheme = checkedObject.CategorySchemes.First();
                                if (checkedCategoryScheme.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }

                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpCategoryScheme.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpCategoryScheme), tmpCategoryScheme.Id, tmpCategoryScheme.AgencyId, tmpCategoryScheme.Version, "CATEGORY_SCHEME", itemIsOk));
                        }
                    }

                    foreach (IDataStructureObject tmpDataStructure in sdmxObjects.DataStructures)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetDataStructure(new ArtefactIdentity(tmpDataStructure.Id, tmpDataStructure.AgencyId, tmpDataStructure.Version), true, false);
                            if (checkedObject.DataStructures.Count > 0)
                            {
                                IDataStructureObject checkedDataStructure = checkedObject.DataStructures.First();
                                if (checkedDataStructure.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpDataStructure.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpDataStructure), tmpDataStructure.Id, tmpDataStructure.AgencyId, tmpDataStructure.Version, "DSD", itemIsOk));
                        }
                    }

                    foreach (IAgencyScheme tmpAgencyScheme in sdmxObjects.AgenciesSchemes)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetAgencyScheme(new ArtefactIdentity(tmpAgencyScheme.Id, tmpAgencyScheme.AgencyId, tmpAgencyScheme.Version), true, false);
                            if (checkedObject.AgenciesSchemes.Count > 0)
                            {
                                IAgencyScheme checkedAgencyScheme = checkedObject.AgenciesSchemes.First();
                                if (checkedAgencyScheme.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpAgencyScheme.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpAgencyScheme), tmpAgencyScheme.Id, tmpAgencyScheme.AgencyId, tmpAgencyScheme.Version, "AGENCY_SCHEME", itemIsOk));
                        }
                    }

                    foreach (IDataProviderScheme tmpDataProviderScheme in sdmxObjects.DataProviderSchemes)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetDataProviderScheme(new ArtefactIdentity(tmpDataProviderScheme.Id, tmpDataProviderScheme.AgencyId, tmpDataProviderScheme.Version), true, false);
                            if (checkedObject.DataProviderSchemes.Count > 0)
                            {
                                IDataProviderScheme checkedDataProviderScheme = checkedObject.DataProviderSchemes.First();
                                if (checkedDataProviderScheme.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpDataProviderScheme.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpDataProviderScheme), tmpDataProviderScheme.Id, tmpDataProviderScheme.AgencyId, tmpDataProviderScheme.Version, "DATA_PROVIDER_SCHEME", itemIsOk));
                        }
                    }

                    foreach (IDataConsumerScheme tmpDataConsumerScheme in sdmxObjects.DataConsumerSchemes)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetDataConsumerScheme(new ArtefactIdentity(tmpDataConsumerScheme.Id, tmpDataConsumerScheme.AgencyId, tmpDataConsumerScheme.Version), true, false);
                            if (checkedObject.DataConsumerSchemes.Count > 0)
                            {
                                IDataConsumerScheme checkedDataConsumerScheme = checkedObject.DataConsumerSchemes.First();
                                if (checkedDataConsumerScheme.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }

                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpDataConsumerScheme.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpDataConsumerScheme), tmpDataConsumerScheme.Id, tmpDataConsumerScheme.AgencyId, tmpDataConsumerScheme.Version, "DATA_CONSUMER_SCHEME", itemIsOk));
                        }
                    }

                    foreach (IOrganisationUnitSchemeObject tmpOrganizationUnitScheme in sdmxObjects.OrganisationUnitSchemes)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetOrganisationUnitScheme(new ArtefactIdentity(tmpOrganizationUnitScheme.Id, tmpOrganizationUnitScheme.AgencyId, tmpOrganizationUnitScheme.Version), true, false);
                            if (checkedObject.OrganisationUnitSchemes.Count > 0)
                            {
                                IOrganisationUnitSchemeObject checkedOrganisationUnitScheme = checkedObject.OrganisationUnitSchemes.First();
                                if (checkedOrganisationUnitScheme.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }

                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpOrganizationUnitScheme.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpOrganizationUnitScheme), tmpOrganizationUnitScheme.Id, tmpOrganizationUnitScheme.AgencyId, tmpOrganizationUnitScheme.Version, "ORGANIZATION_UNIT_SCHEME", itemIsOk));
                        }
                    }

                    foreach (IContentConstraintObject tmpContentConstraint in sdmxObjects.ContentConstraintObjects)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetContentConstraint(new ArtefactIdentity(tmpContentConstraint.Id, tmpContentConstraint.AgencyId, tmpContentConstraint.Version), true, false);
                            if (checkedObject.ContentConstraintObjects.Count > 0)
                            {
                                IContentConstraintObject checkedContentConstraint = checkedObject.ContentConstraintObjects.First();
                                if (checkedContentConstraint.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpContentConstraint.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpContentConstraint), tmpContentConstraint.Id, tmpContentConstraint.AgencyId, tmpContentConstraint.Version, "CONTENT_CONSTRAINT", itemIsOk));
                        }
                    }

                    foreach (IStructureSetObject tmpStructureSet in sdmxObjects.StructureSets)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetStructureSet(new ArtefactIdentity(tmpStructureSet.Id, tmpStructureSet.AgencyId, tmpStructureSet.Version), true, false);
                            if (checkedObject.StructureSets.Count > 0)
                            {
                                IStructureSetObject checkedStructureSet = checkedObject.StructureSets.First();
                                if (checkedStructureSet.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpStructureSet.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpStructureSet), tmpStructureSet.Id, tmpStructureSet.AgencyId, tmpStructureSet.Version, "STRUCTURE_SET", itemIsOk));
                        }   
                    }

                    foreach (IHierarchicalCodelistObject tmpHierarchicalCodelist in sdmxObjects.HierarchicalCodelists)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetHcl(new ArtefactIdentity(tmpHierarchicalCodelist.Id, tmpHierarchicalCodelist.AgencyId, tmpHierarchicalCodelist.Version), true, false);
                            if (checkedObject.HierarchicalCodelists.Count > 0)
                            {
                                IHierarchicalCodelistObject checkedHierarchicalCodelist = checkedObject.HierarchicalCodelists.First();
                                if (checkedHierarchicalCodelist.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                          where agency.id.Equals( tmpHierarchicalCodelist.AgencyId.ToString() )
                                          select agency).ToList<UserAgency>();
                        
                        if ( foundAgency.Count > 0 )
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpHierarchicalCodelist), tmpHierarchicalCodelist.Id, tmpHierarchicalCodelist.AgencyId, tmpHierarchicalCodelist.Version, "HIERARCHICAL_CODELIST", itemIsOk));
                        }
                    }

                    foreach (ICategorisationObject tmpCatObj in sdmxObjects.Categorisations)
                    {
                        itemIsOk = true;
                        try
                        {
                            ISdmxObjects checkedObject = wsModel.GetCategorisation(new ArtefactIdentity(tmpCatObj.Id, tmpCatObj.AgencyId, tmpCatObj.Version), true, false);
                            if (checkedObject.Categorisations.Count > 0)
                            {
                                ICategorisationObject checkedCategorisation = checkedObject.Categorisations.First();
                                if (checkedCategorisation.IsFinal.IsTrue)
                                {
                                    itemIsOk = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            itemIsOk = true;
                        }
                        var foundAgency = (from agency in currentUser.agencies
                                           where agency.id.Equals(tmpCatObj.AgencyId.ToString())
                                           select agency).ToList<UserAgency>();

                        if (foundAgency.Count > 0)
                        {
                            items.Add(new ImportedItem(localizedUtils.GetNameableName(tmpCatObj), tmpCatObj.Id, tmpCatObj.AgencyId, tmpCatObj.Version, "CATEGORISATION", itemIsOk));
                        }
                    }
                    Session[IMPORTED_ITEMS_STRING] = items;
                    Session[IMPORTED_SDMX_OBJECT] = sdmxObjects;
                    gridView.DataSource = items;
                    gridView.DataBind();
                    if ( items.Count != 0 )
                    {    
                        btnConfirmImport.Visible = true;
                        lblNoItemsAllowed.Visible = false;
                    }
                    else
                    {
                        Utils.AppendScript( "$('#chbSelectAll').hide();" );
                        lblSelectAll.Visible = false;
                        lblNoItemsAllowed.Visible = true;
                        btnConfirmImport.Visible = false;
                    }

                    Utils.AppendScript("openPopUp('importedItemsGridDiv', 600);");
                }
                catch (Exception ex)
                {
                    string msg;
                    if (ex.InnerException != null)
                        msg = ex.InnerException.Message;
                    else
                        msg = ex.Message;

                    Utils.ShowDialog(msg);
                }
            }
            else
            {
                Utils.ShowDialog( Resources.Messages.err_no_file_uploaded );
            }
        }

        #endregion

        #region METHODS

        private string SaveFile()
        {
            string FilePath = MapPath("~/UploadedFiles/") + uploadedFiles.FileName;
            uploadedFiles.SaveAs(FilePath);

            return FilePath;
        }

        private ISdmxObjects PopolateAnnotationID(ISdmxObjects sdmxObjects)
        {
            ISdmxObjects SdmxObjectsTemp = new SdmxObjectsImpl();
            IMutableObjects MutableObjects = SdmxObjectsTemp.MutableObjects;

            foreach (ICodelistObject tmpCodeList in sdmxObjects.Codelists)
            {
                ICodelistMutableObject codelist = tmpCodeList.MutableInstance;
                FillAnnotationID(codelist);

                foreach (ICodeMutableObject code in codelist.Items)
                    FillAnnotationID(code);

                MutableObjects.AddCodelist(codelist);
            }

            foreach (IConceptSchemeObject cs in sdmxObjects.ConceptSchemes)
            {
                IConceptSchemeMutableObject csM = cs.MutableInstance;
                FillAnnotationID(csM);

                foreach (IConceptMutableObject code in csM.Items)
                    FillAnnotationID(code);

                MutableObjects.AddConceptScheme(csM);
            }

            foreach (ICategorySchemeObject cs in sdmxObjects.CategorySchemes)
            {
                ICategorySchemeMutableObject csM = cs.MutableInstance;
                FillAnnotationID(csM);

                foreach (ICategoryMutableObject cat in csM.Items)
                    FillAnnotationID(cat);

                MutableObjects.AddCategoryScheme(csM);
            }

            foreach (IDataflowObject df in sdmxObjects.Dataflows)
            {
                IDataflowMutableObject dfM = df.MutableInstance;
                FillAnnotationID(dfM);

                MutableObjects.AddDataflow(dfM);
            }

            foreach (ICategorisationObject cat in sdmxObjects.Categorisations)
            {
                ICategorisationMutableObject catM = cat.MutableInstance;
                FillAnnotationID(catM);

                MutableObjects.AddCategorisation(catM);
            }

            foreach (IAgencyScheme aSch in sdmxObjects.AgenciesSchemes)
            {
                IAgencySchemeMutableObject aSchM = aSch.MutableInstance;
                FillAnnotationID(aSchM);

                foreach (IAgencyMutableObject ag in aSchM.Items)
                    FillAnnotationID(ag);

                MutableObjects.AddAgencyScheme(aSchM);
            }

            foreach (IDataProviderScheme dp in sdmxObjects.DataProviderSchemes)
            {
                IDataProviderSchemeMutableObject dpM = dp.MutableInstance;
                FillAnnotationID(dpM);

                foreach (IDataProviderMutableObject dat in dpM.Items)
                    FillAnnotationID(dat);

                MutableObjects.AddDataProviderScheme(dpM);
            }

            foreach (IDataConsumerScheme dc in sdmxObjects.DataConsumerSchemes)
            {
                IDataConsumerSchemeMutableObject dcM = dc.MutableInstance;
                FillAnnotationID(dcM);

                foreach (IDataConsumerMutableObject dat in dcM.Items)
                    FillAnnotationID(dat);

                MutableObjects.AddDataConsumerScheme(dcM);
            }

            foreach (IOrganisationUnitSchemeObject oi in sdmxObjects.OrganisationUnitSchemes)
            {
                IOrganisationUnitSchemeMutableObject oiM = oi.MutableInstance;
                FillAnnotationID(oiM);

                foreach (IOrganisationUnitMutableObject org in oiM.Items)
                    FillAnnotationID(org);

                MutableObjects.AddOrganisationUnitScheme(oiM);
            }

            foreach (IDataStructureObject dsd in sdmxObjects.DataStructures)
            {
                IDataStructureMutableObject dsdM = dsd.MutableInstance;
                FillAnnotationID(dsdM);
                FillAnnotationID(dsdM.PrimaryMeasure);

                if (dsdM.DimensionList != null)
                    foreach (IDimensionMutableObject dim in dsdM.DimensionList.Dimensions)
                        FillAnnotationID(dim);

                if (dsdM.AttributeList != null)
                    foreach (IAttributeMutableObject att in dsdM.AttributeList.Attributes)
                        FillAnnotationID(att);

                if (dsdM.Groups != null)
                    foreach (IGroupMutableObject group in dsdM.Groups)
                        FillAnnotationID(group);

                MutableObjects.AddDataStructure(dsdM);
            }

            foreach (IContentConstraintObject cc in sdmxObjects.ContentConstraintObjects)
            {
                IContentConstraintMutableObject catM = cc.MutableInstance;
                FillAnnotationID(catM);

                MutableObjects.AddContentConstraint(catM);
            }

            foreach (IStructureSetObject ss in sdmxObjects.StructureSets)
            {
                IStructureSetMutableObject ssM = ss.MutableInstance;
                FillAnnotationID(ssM);

                if (ssM.CodelistMapList != null)
                    foreach (ICodelistMapMutableObject clm in ssM.CodelistMapList)
                        FillAnnotationID(clm);

                if (ssM.StructureMapList != null)
                    foreach (IStructureMapMutableObject sm in ssM.StructureMapList)
                        FillAnnotationID(sm);

                MutableObjects.AddStructureSet(ssM);
            }

            foreach (IHierarchicalCodelistObject hcl in sdmxObjects.HierarchicalCodelists)
            {
                IHierarchicalCodelistMutableObject hclM = hcl.MutableInstance;
                FillAnnotationID(hclM);

                foreach (IHierarchyMutableObject hi in hclM.Hierarchies)
                    FillAnnotationID(hi);

                MutableObjects.AddHierarchicalCodelist(hclM);
            }

            return MutableObjects.ImmutableObjects;
        }

        private void FillAnnotationID(IAnnotableMutableObject AnnotableObject)
        {
            int AnnotationCounter = 0;
            int MatchingAnnotationCounter = 0;
            List<String> lIDS = new List<string>();

            foreach (IAnnotationMutableObject ann in AnnotableObject.Annotations)
            {
                if ((ann.Id == null || ann.Id.Trim() == String.Empty) && ann.Type != "@ORDER@")
                {
                    ++AnnotationCounter;
                    ann.Id = "@" + AnnotationCounter.ToString() + "@";
                }
                else
                {
                    if (lIDS.Contains(ann.Id))
                    {
                        ++MatchingAnnotationCounter;
                        ann.Id = ann.Id + "_" + MatchingAnnotationCounter.ToString();
                    }
                }
                lIDS.Add(ann.Id);
            }
        }

        private string CreateArtefactImportedString(ISdmxObjects sdmxObjectsSource)
        {
            string sRet = string.Empty;

            if (sdmxObjectsSource.Agencies.Count > 0)
                sRet += "<br/>Agency: " + sdmxObjectsSource.Agencies.Count.ToString();

            if (sdmxObjectsSource.Codelists.Count > 0)
                sRet += "<br/>CodeList: " + sdmxObjectsSource.Codelists.Count.ToString();

            if (sdmxObjectsSource.ConceptSchemes.Count > 0)
                sRet += "<br/>ConceptScheme: " + sdmxObjectsSource.ConceptSchemes.Count.ToString();

            if (sdmxObjectsSource.CategorySchemes.Count > 0)
                sRet += "<br/>CategoryScheme: " + sdmxObjectsSource.CategorySchemes.Count.ToString();

            if (sdmxObjectsSource.DataStructures.Count > 0)
                sRet += "<br/>DataStructures: " + sdmxObjectsSource.DataStructures.Count.ToString();

            if (sdmxObjectsSource.Dataflows.Count > 0)
                sRet += "<br/>Dataflows: " + sdmxObjectsSource.Dataflows.Count.ToString();

            if (sdmxObjectsSource.Categorisations.Count > 0)
                sRet += "<br/>Categorisations: " + sdmxObjectsSource.Categorisations.Count.ToString();

            if (sdmxObjectsSource.HierarchicalCodelists.Count > 0)
                sRet += "<br/>HierarchicalCodelists: " + sdmxObjectsSource.HierarchicalCodelists.Count.ToString();

            if (sdmxObjectsSource.DataConsumerSchemes.Count > 0)
                sRet += "<br/>DataConsumerSchemes: " + sdmxObjectsSource.DataConsumerSchemes.Count.ToString();

            if (sdmxObjectsSource.AgenciesSchemes.Count > 0)
                sRet += "<br/>AgenciesSchemes: " + sdmxObjectsSource.AgenciesSchemes.Count.ToString();

            if (sdmxObjectsSource.DataProviderSchemes.Count > 0)
                sRet += "<br/>DataProviderSchemes: " + sdmxObjectsSource.DataProviderSchemes.Count.ToString();

            if (sdmxObjectsSource.OrganisationUnitSchemes.Count > 0)
                sRet += "<br/>OrganisationUnitSchemes: " + sdmxObjectsSource.OrganisationUnitSchemes.Count.ToString();

            if (sdmxObjectsSource.StructureSets.Count > 0)
                sRet += "<br/>StructureSets: " + sdmxObjectsSource.StructureSets.Count.ToString();

            if (sdmxObjectsSource.HierarchicalCodelists.Count > 0)
                sRet += "<br/>HierarchicalCodelist: " + sdmxObjectsSource.HierarchicalCodelists.Count.ToString();

            //foreach (ArtefactImportStatus ais in artefactImportStatuses)
            //{
            //    //sRet += ReplaceInvalidJScriptChar(ais.ImportMessage.Message) + @"\n\r"; ;
            //    sRet += ais.ImportMessage.Message + @"\n\r";
            //}

            return sRet;
        }

        #endregion

    }
}


