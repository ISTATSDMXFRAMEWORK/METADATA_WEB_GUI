using ISTAT.Entity;
using ISTAT.WSDAL;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ISTATRegistry.UserControls
{
    public partial class CreateSubCodelistConfirm : System.Web.UI.UserControl
    {
        public IMaintainableMutableObject ucMaintanableArtefact { get; set; }
        public SdmxStructureEnumType ucStructureType { get; set; }
        public bool ucReadOnlyID { get; set; }
        public bool ucReadOnlyAgency { get; set; }
        public bool ucReadOnlyVersion { get; set; }
        public bool ucDisable { get; set; }
        public CreateSubCodelistForm uc1;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            SetForm();
        }

        private void SetForm()
        {
            if (ucMaintanableArtefact == null || ucDisable)
                return;

            if (!Page.IsPostBack)
            {
                Utils.PopulateCmbAgencies(cmbAgencies, true);
                lblArtType.Text = ucStructureType.ToString();
                lblArtID.Text = ucMaintanableArtefact.Id;
                lblArtAgency.Text = ucMaintanableArtefact.AgencyId;
                lblArtVersion.Text = ucMaintanableArtefact.Version;
                txtDSDID.Text = ucMaintanableArtefact.Id;
                cmbAgencies.SelectedValue = ucMaintanableArtefact.AgencyId;
                txtVersion.Text = ucMaintanableArtefact.Version;
                if (ucStructureType.Equals(SdmxStructureEnumType.AgencyScheme) ||
                     ucStructureType.Equals(SdmxStructureEnumType.OrganisationUnitScheme) ||
                     ucStructureType.Equals(SdmxStructureEnumType.DataProviderScheme) ||
                     ucStructureType.Equals(SdmxStructureEnumType.DataConsumerScheme) ||
                     ucStructureType.Equals(SdmxStructureEnumType.Agency))
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
        
        private void OpenCreateSubCodelistPopUp()
        {
            Utils.AppendScript("openP('dialog-form" + this.ClientID + "');");
            Utils.AppendScript("location.href= '#derivedCL';");
        }

        private string ValidateArtId()
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string Error = String.Empty;

            uc1 = (CreateSubCodelistForm)this.Parent.FindControl("SubCodelistForm1");

            if (uc1 == null || uc1.SELECTEDVALUES.Items.Count == 0) 
            {
                Error = Resources.Messages.err_create_subcodelist;
                OpenCreateSubCodelistPopUp();
                Utils.ShowDialog(Error, 300, "Create Sub Codelist");
                return;
            }

            Error = ValidateArtId();

            if (Error != String.Empty)
            {
                OpenCreateSubCodelistPopUp();
                Utils.ShowDialog(Error, 300, Resources.Messages.err_duplicate_artefact);
                return;
            }

            if (ucMaintanableArtefact.Id.ToString().Equals(txtDSDID.Text.Trim())
                && ucMaintanableArtefact.AgencyId.ToString().Equals(cmbAgencies.SelectedValue.ToString().Trim())
                && ucMaintanableArtefact.Version.ToString().Equals(txtVersion.Text.Trim()))
            {
                OpenCreateSubCodelistPopUp();
                Utils.ShowDialog(Resources.Messages.equal_global_identificators);
                return;
            }

            ucMaintanableArtefact.Id = txtDSDID.Text;
            ucMaintanableArtefact.AgencyId = cmbAgencies.SelectedValue;
            ucMaintanableArtefact.Version = txtVersion.Text;
            ucMaintanableArtefact.FinalStructure = TertiaryBool.ParseBoolean(false);

            ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

            ICodelistMutableObject sessionCLMutable = (ICodelistMutableObject)Session[codelistItemDetails.KEY_PAGE_SESSION];
           
            ICodelistMutableObject clMutable = new CodelistMutableCore();

            clMutable.AgencyId = ucMaintanableArtefact.AgencyId;
            clMutable.Version = ucMaintanableArtefact.Version;
            clMutable.Id = ucMaintanableArtefact.Id;
            
            foreach(ITextTypeWrapperMutableObject n in sessionCLMutable.Names)
            {
                clMutable.AddName(n.Locale, n.Value);
            }

            List<string> pCodeList = new List<string>();
            List<string> codeList = new List<string>();

            foreach (ListItem code in uc1.SELECTEDVALUES.Items)
            {
                string[] c = code.Text.Split(new char[] { '|' });
                codeList.Add(c[0]);
                ICodeMutableObject codeMutable;
                codeMutable = clMutable.CreateItem(c[0], c[1]);
                if (!String.IsNullOrEmpty(c[3]))
                {
                    codeMutable.ParentCode = c[3];
                    pCodeList.Add(c[3]);
                }
            }

            //identifico tutti i codici parent non selezionati dall'utente
            List<string> codeToAdd = pCodeList.Where(p => !codeList.Any(p2 => p2 == p)).ToList();

            

            //recupero le informazioni dei codici padri non selezionati
            List<CodeItem> lCodeItem = (List<CodeItem>)Session["TempCodesLeft"];

            //Se il checkbox è selezionato aggiungo alla lista dei codici selezionati gli eventuali parent code non scelti dall'utente
            //per evitare di creare un errore al momento dell'inserimento della codelist
            if (uc1.CHKParentCode)
            {
                //aggiungo i codici parent no selezionati
                foreach (string code in codeToAdd)
                {
                    CodeItem cItem = lCodeItem.Find(x => x.Code == code);
                    clMutable.CreateItem(cItem.Code, cItem.Name);
                }
            }
            else if (lCodeItem.Count() > 0) 
            {
                Utils.ShowDialog("Attention. You have selected a code to import, but not its parent code.");
                Utils.AppendScript("location.href= '#derivedCL';");
                return;
            }
            
            
            sdmxObjects.AddCodelist(clMutable.ImmutableInstance);

            //ISdmxObjects tmpSdmxObject = null;
            //WSModel tmpWsModel = new WSModel();
            //bool itemAdded = false;
            //switch (ucStructureType)
            //{
            //    case SdmxStructureEnumType.CodeList:
            //        try
            //        {
            //        //    tmpSdmxObject = tmpWsModel.GetCodeList(new ArtefactIdentity(txtDSDID.Text.Trim(), cmbAgencies.SelectedValue.ToString().Trim(), txtVersion.Text.Trim()), true, false);
            //            clMutable.AgencyId = ucMaintanableArtefact.AgencyId;
            //            clMutable.Version = ucMaintanableArtefact.Version;
            //            clMutable.Id = ucMaintanableArtefact.Id;
            //            clMutable.AddName("en", "Sub_en");
            //            clMutable.AddName("it", "Sub_it");
            //            foreach (CodeItem code in uc1.SELECTEDVALUES) 
            //            {
            //                clMutable.AddItem(new ICodeMutableObject() {Id=code.Code, Names=code.Name, });
            //            }
            //            clMutable.CreateItem("", "");
            //        }
            //        catch (Exception ex)
            //        {
            //        //    if (ex.Message.ToLower().Equals("no results found"))
            //        //    {
            //        //        sdmxObjects.AddCodelist((Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject)ucMaintanableArtefact.ImmutableInstance);
            //        //        itemAdded = true;
            //        //    }
            //        //}
            //        //if (!itemAdded)
            //        //{
            //        //    Utils.ShowDialog("Oggetto già presente nel database");
            //        //    return;
            //        }
            //        break;
              
            //}

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
        }
    }
}