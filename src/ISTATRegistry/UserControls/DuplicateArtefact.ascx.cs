using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using ISTAT.WSDAL;
using ISTATUtils;
using System.Xml;
using ISTAT.EntityMapper;
using ISTAT.Entity;

namespace ISTATRegistry.UserControls
{
    public partial class DuplicateArtefact : System.Web.UI.UserControl
    {
        public IMaintainableMutableObject ucMaintanableArtefact { get; set; }
        public SdmxStructureEnumType ucStructureType { get; set; }
        public bool ucReadOnlyID { get; set; }
        public bool ucReadOnlyAgency { get; set; }
        public bool ucReadOnlyVersion { get; set; }
        public bool ucDisable { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetForm();
        }

        private void SetForm()
        {
            if (ucMaintanableArtefact == null || ucDisable)
                return;

            if ( !Page.IsPostBack )
            {
                Utils.PopulateCmbAgencies(cmbAgencies, true);
                lblArtType.Text = ucStructureType.ToString();
                lblArtID.Text = ucMaintanableArtefact.Id;
                lblArtAgency.Text = ucMaintanableArtefact.AgencyId;
                lblArtVersion.Text = ucMaintanableArtefact.Version;
                txtDSDID.Text = ucMaintanableArtefact.Id;
                cmbAgencies.SelectedValue = ucMaintanableArtefact.AgencyId;
                txtVersion.Text = ucMaintanableArtefact.Version;
                if ( ucStructureType.Equals( SdmxStructureEnumType.AgencyScheme ) ||
                     ucStructureType.Equals( SdmxStructureEnumType.OrganisationUnitScheme ) ||
                     ucStructureType.Equals( SdmxStructureEnumType.DataProviderScheme ) ||
                     ucStructureType.Equals( SdmxStructureEnumType.DataConsumerScheme ) || 
                     ucStructureType.Equals( SdmxStructureEnumType.Agency ) )
                {
                    txtDSDID.Enabled = false;
                    txtVersion.Enabled = false;
                    return;
                }
            }
            txtDSDID.Enabled = !ucReadOnlyID;
            cmbAgencies.Enabled = !ucReadOnlyAgency;
            txtVersion.Enabled = !ucReadOnlyVersion;
        }

        protected void btnDuplicate_Click(object sender, EventArgs e)
        {
            string Error = ValidateDSDData();

            if (Error != String.Empty)
            {
                OpenDuplicatePopUp();
                Utils.ShowDialog(Error, 300, Resources.Messages.err_duplicate_artefact);
                return;
            }

            if ( ucMaintanableArtefact.Id.ToString().Equals( txtDSDID.Text.Trim() ) 
                &&  ucMaintanableArtefact.AgencyId.ToString().Equals( cmbAgencies.SelectedValue.ToString().Trim() ) 
                && ucMaintanableArtefact.Version.ToString().Equals( txtVersion.Text.Trim() ) )
            {
                OpenDuplicatePopUp();
                Utils.ShowDialog( Resources.Messages.equal_global_identificators );
                return;
            }

            ucMaintanableArtefact.Id = txtDSDID.Text;
            ucMaintanableArtefact.AgencyId = cmbAgencies.SelectedValue;
            ucMaintanableArtefact.Version = txtVersion.Text;
            ucMaintanableArtefact.FinalStructure = TertiaryBool.ParseBoolean(false);           

            ISdmxObjects sdmxObjects = new SdmxObjectsImpl();
            ISdmxObjects tmpSdmxObject = null;
            WSModel tmpWsModel = new WSModel();
            bool itemAdded = false;
            switch (ucStructureType)
            {
                case SdmxStructureEnumType.AgencyScheme:
                    sdmxObjects.AddAgencyScheme((Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAgencyScheme)ucMaintanableArtefact.ImmutableInstance);
                    break;
                case SdmxStructureEnumType.Categorisation:
                    sdmxObjects.AddCategorisation((Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme.ICategorisationObject)ucMaintanableArtefact.ImmutableInstance);
                    break;
                case SdmxStructureEnumType.CategoryScheme:
                    sdmxObjects.AddCategoryScheme((Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme.ICategorySchemeObject)ucMaintanableArtefact.ImmutableInstance);
                    break;
                case SdmxStructureEnumType.CodeList:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetCodeList(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                            sdmxObjects.AddCodelist((Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject)ucMaintanableArtefact.ImmutableInstance);
                            itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.ConceptScheme:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetConceptScheme(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                            sdmxObjects.AddConceptScheme((Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme.IConceptSchemeObject)ucMaintanableArtefact.ImmutableInstance);
                            itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.ContentConstraint:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetContentConstraint(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                             sdmxObjects.AddContentConstraintObject((Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry.IContentConstraintObject)ucMaintanableArtefact.ImmutableInstance);
                             itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.DataConsumerScheme:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetDataConsumerScheme(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                             sdmxObjects.AddDataConsumerScheme((Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IDataConsumerScheme)ucMaintanableArtefact.ImmutableInstance);
                             itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.DataProviderScheme:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetDataProviderScheme(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                             sdmxObjects.AddDataProviderScheme((Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IDataProviderScheme)ucMaintanableArtefact.ImmutableInstance);
                            itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.Dataflow:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetDataFlow(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                            sdmxObjects.AddDataflow((IDataflowObject)ucMaintanableArtefact.ImmutableInstance);
                             itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.Dsd:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetDataStructure(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                            sdmxObjects.AddDataStructure((IDataStructureObject)ucMaintanableArtefact.ImmutableInstance);
                            itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.HierarchicalCodelist:
                    
                    sdmxObjects.AddHierarchicalCodelist((Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.IHierarchicalCodelistObject)ucMaintanableArtefact.ImmutableInstance);
                    break;
                case SdmxStructureEnumType.OrganisationUnitScheme:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetOrganisationUnitScheme(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                           sdmxObjects.AddOrganisationUnitScheme((Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IOrganisationUnitSchemeObject)ucMaintanableArtefact.ImmutableInstance);
                         itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
                case SdmxStructureEnumType.StructureSet:
                    try
                    {
                        tmpSdmxObject = tmpWsModel.GetStructureSet(new ArtefactIdentity( txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(),txtVersion.Text.Trim() ) , true ,false );
                    }
                    catch (Exception ex)
                    {
                        if ( ex.Message.ToLower().Equals( "no results found" ) )
                        {
                            sdmxObjects.AddStructureSet((Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping.IStructureSetObject)ucMaintanableArtefact.ImmutableInstance);
                            itemAdded = true;
                        }
                    }
                    if ( !itemAdded )
                    {
                        Utils.ShowDialog( "Oggetto già presente nel database" );
                        return;
                    }
                    break;
            }

            WSModel wsModel = new WSModel();
            XmlDocument xRet = wsModel.SubmitStructure(sdmxObjects);

            string err = Utils.GetXMLResponseError(xRet);

            if (err != "")
            {
                Utils.ShowDialog(err);
                return;
            }

            ucMaintanableArtefact.Id = lblArtID.Text;
            ucMaintanableArtefact.AgencyId = lblArtAgency.Text;
            ucMaintanableArtefact.Version = lblArtVersion.Text;

            Utils.ShowDialog(Resources.Messages.succ_operation);
            Utils.ResetBeforeUnload();
            //Utils.AppendScript("location.href='./KeyFamily.aspx';");
        }

        private string ValidateDSDData()
        {
            string messagesGroup = string.Empty;    
            int errorCounter = 1;                   

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(txtDSDID.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + ")" + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            // Controllo versione
            if (!ValidationUtils.CheckVersionFormat(txtVersion.Text))
            {
                messagesGroup += Convert.ToString(errorCounter) + "" + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
            }

            return messagesGroup;
        }

        private void OpenDuplicatePopUp()
        {
            Utils.AppendScript("openP('dialog-form"+ this.ClientID +"');");
        }
    }
}