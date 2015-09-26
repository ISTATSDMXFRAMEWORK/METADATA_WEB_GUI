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
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using ISTAT.Entity;
using System.Xml;

namespace ISTATRegistry.UserControls
{
    public partial class ArtefactDelete : System.Web.UI.UserControl
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

        public string ucButtonClientId
        {
            get
            {
                return btnDelete.ClientID;
            }
          }

        public int ucCanDeleteThis
        {
            get
            {
                return canDeleteThis;
            }

            set
            {
                canDeleteThis = value;
            }
          }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblID.Text = ucID;
                lblAgency.Text = ucAgency;
                lblVersion.Text = ucVersion;
                lblArtefactType.Text = ucArtefactType;
            }

            lbl_title.DataBind();
            lbl_type.DataBind();
            lbl_id.DataBind();
            lbl_agency.DataBind();
            lbl_version.DataBind();
            btnDelete.DataBind();

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            /*
             * 1. Faccio il retrieve dell'artefact da eliminare
             * 2. Ottengo un SDMXObjects
             * 3. Trasformo l'SDMXObjects in un oggetto xml
             * 4. Carico il template
             * 5. Cerco il tag "structures" nell'sdmxobjects che contiene l'artefatto da eliminare
             * 6. Inserisco il contenuto dello structure nel template
             * 7. Richiamo il metodo submitstructure per effettuare l'eliminazione
             * 8. Controllo il successo dell'eliminazione e rifaccio il DataBind()
             */

            WSModel wsModel = new WSModel();
            WSUtils wsUtils = new WSUtils();
            ISdmxObjects sdmxObjects;
            XmlDocument xDocStructure;
            XmlDocument xDocTemplate;
            XmlDocument xDocResponse;

            try
            {
                //sdmxObjects = wsModel.GetCodeList(new ArtefactIdentity(ucID, ucAgency, ucVersion), true);

                sdmxObjects = GetSdmxObjects();

                xDocStructure = wsUtils.GetXMLDocFromSdmxObjects(sdmxObjects, StructureOutputFormatEnumType.SdmxV21StructureDocument);

                //Carico il template
                xDocTemplate = new XmlDocument();
                xDocTemplate.Load(Server.MapPath(@".\SdmxQueryTemplate\SubmitStructureDelete.xml"));

                // Il nodo root "Structure" del template
                XmlNode xTempStructNode = xDocTemplate.SelectSingleNode("//*[local-name()='Structures']");

                // Aggiungo al template lo structure da eliminare
                xTempStructNode.InnerXml = xDocStructure.SelectSingleNode("//*[local-name()='Structures']").InnerXml;

                // Richiamo il SubmitStructure per effettuare la Delete
                xDocResponse = wsModel.SubmitStructure(xDocTemplate);

                string CheckError = Utils.GetXMLResponseError(xDocResponse);

                if (CheckError != String.Empty)
                {
                    //Utils.ShowDialog(CheckError);
                    Utils.ShowDialog( Resources.Messages.err_artefact_is_a_reference );
                    Utils.ForceBlackClosing();
                    return;
                }

                //Utils.ExecuteScript(Parent.Page, "location.reload();");
                Utils.ReloadPage();
               //ScriptManager.RegisterStartupScript(this, typeof(Page), UniqueID, "closePopup();", true);
            }
            catch (Exception ex)
            {
                Utils.AppendScript("closePopup();");
                Utils.ShowDialog("An error occurred: " + ex.Message);
            }
        }

        #endregion

        #region Methods


        protected string ReplInvChar(string stringToCheck)
        {
            return stringToCheck.Replace('@', '_');
        }

        private ISdmxObjects GetSdmxObjects()
        {
            ISdmxObjects sdmxObjects = null;
            WSModel dal = new WSModel();

            switch (ucArtefactType)
            {
                case "CodeList":
                    sdmxObjects = dal.GetCodeList(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "ConceptScheme":
                    sdmxObjects = dal.GetConceptScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "CategoryScheme":
                    sdmxObjects = dal.GetCategoryScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "DataFlow":
                    sdmxObjects = dal.GetDataFlow(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), false, false);
                    break;
                case "KeyFamily":
                    sdmxObjects = dal.GetDataStructure(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "Categorization":    // Aggiunto per il recupero di una Categorization   ------ Fabrizio Alonzi
                    sdmxObjects = dal.GetCategorisation(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), false, false);
                    break;
                case "AgencyScheme":
                    sdmxObjects = dal.GetAgencyScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "DataProviderScheme":
                    sdmxObjects = dal.GetDataProviderScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "DataConsumerScheme":
                    sdmxObjects = dal.GetDataConsumerScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "OrganizationUnitScheme":
                    sdmxObjects = dal.GetOrganisationUnitScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "ContentConstraint":
                    sdmxObjects = dal.GetContentConstraint(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                case "StructureSet":
                    sdmxObjects = dal.GetStructureSet(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), true, false);
                    break;
                default:
                    return null;
            }

            return sdmxObjects;
        }

        #endregion

        private int canDeleteThis = 1;

    }
}