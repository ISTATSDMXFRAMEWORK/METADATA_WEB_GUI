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
using ISTATRegistry.IRServiceReference;
using Elistia.DotNetRtfWriter;
using ISTAT.EntityMapper;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;

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

        public bool ucVisible
        {
            get
            {
                return pnlDownload.Visible;
            }
            set
            {
                pnlDownload.Visible = value;
            }
        }



        #endregion

        #region Private Props

        DotStatProperties _dotStatProp;
        LocalizedUtils _localizedUtils;
        EntityMapper _entityMapper;
        bool _dsdWithRef = false;

        #endregion

        #region Events

        protected override void OnPreRender(EventArgs e)
        {
            lblArtefactID.Text = ucID + "-" + ucAgency + "-" + ucVersion;

            if (cmbWSExport.Items.Count <= 0)
            {
                // se utente autenticato e visibilità su agency da esportare
                IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;
                //if (currentUser != null)
                //{
                    //int agencyOccurence = currentUser.agencies.Count(agency => agency.id.Equals(ucAgency));
                    //if (agencyOccurence > 0)
                    //{
                        cmbDownloadType.Items.Add(new ListItem("Web Service", "Web Service"));

                        Utils.PopulateCmbExportEndPoint(cmbWSExport);

                        cmbWSExport.Items.Insert(0, "");

                        //Utils.PopulateCmbAgencies(cmbNewAgency, true);

                        txtNewID.Text = ucID;
                        txtNewVersion.Text = ucVersion;
                        //cmbNewAgency.SelectedValue = ucAgency;

                        hdnWsSource.Value = ((EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT]).Name;

                        cmbWSExport.Items.Remove(hdnWsSource.Value);
                    //}
                //}
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            _entityMapper = new EntityMapper(Utils.LocalizedLanguage);

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

            if (IsPostBack)
            {
                lblArtefactID.DataBind();
                lblTitle.DataBind();
                btnDownload.DataBind();
                lblStub.DataBind();
                lblExportType.DataBind();
                btnDownload.DataBind();
                lblIncludeCodeListAndConceptScheme.DataBind();
                lblSeparator.DataBind();
            }
            else
            {
                EndPointElement epe = (EndPointElement)Session["WSEndPoint"];

                txtContactName.Text = epe.DotStatContactName;
                txtContactDirection.Text = epe.DotStatContactDirection;
                txtContactEMail.Text = epe.DotStatContactEMail;
                txtSecurityUser.Text = epe.DotStatSecurityUserGroup;
                txtSecurityDomain.Text = epe.DotStatSecurityDomain;
            }
        }

        public void LoadLogic()
        {
            cmbDownloadType.Items.Clear();

            if (ucArtefactType.ToLower() != "structureset" && ucArtefactType.ToLower() != "categorization" && ucArtefactType.ToLower() != "contentconstraint")
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

            if (ucArtefactType.Equals("KeyFamily"))
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

            //// se utente autenticato e visibilità su agency da esportare
            //IRServiceReference.User currentUser = Session[SESSION_KEYS.USER_DATA] as User;
            //if (currentUser != null)
            //{
            //    int agencyOccurence = currentUser.agencies.Count(agency => agency.id.Equals(ucAgency));
            //    if (agencyOccurence > 0)
            //    {
            //        cmbDownloadType.Items.Add(new ListItem("Web Service", "Web Service"));

            //        Utils.PopulateCmbExportEndPoint(cmbWSExport);
            //        cmbWSExport.Items.Insert(0, "");

            //        Utils.PopulateCmbAgencies(cmbNewAgency, true);

            //        txtNewID.Text = ucID;
            //        txtNewVersion.Text = ucVersion;
            //        cmbNewAgency.SelectedValue = ucAgency;

            //        hdnWsSource.Value = ((EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT]).Name;
            //    }
            //}

            lblArtefactID.DataBind();
            lblTitle.DataBind();
            btnDownload.DataBind();
            lblStub.DataBind();
            lblExportType.DataBind();
            btnDownload.DataBind();
            lblIncludeCodeListAndConceptScheme.DataBind();
            lblSeparator.DataBind();

        }

        private void PopolateCmbWSExport()
        {
            //Caricare tutti i WS con EnableAuthentication a true
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
                case "RTF":
                    DownloadRTF();
                    break;
            }

            //Utils.AppendScript("$.unblockUI();");
        }



        #endregion

        #region Methods

        private ISdmxObjects GetSdmxObjects()
        {
            ISdmxObjects sdmxObjects = null;
            WSModel dal = new WSModel();

            switch (ucArtefactType.ToUpper())
            {
                case "CODELIST":
                    sdmxObjects = dal.GetCodeList(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "CONCEPTSCHEME":
                    sdmxObjects = dal.GetConceptScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "CATEGORYSCHEME":
                    sdmxObjects = dal.GetCategoryScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "DATAFLOW":
                    sdmxObjects = dal.GetDataFlow(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "KEYFAMILY":
                    if (chkExportCodeListAndConcept.Checked || _dsdWithRef)
                    {
                        sdmxObjects = dal.GetDataStructureWithRef(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    }
                    else
                    {
                        sdmxObjects = dal.GetDataStructure(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    }
                    break;
                case "CATEGORIZATION":
                    sdmxObjects = dal.GetCategorisation(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "AGENCYSCHEME":
                    sdmxObjects = dal.GetAgencyScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "DATAPROVIDERSCHEME":
                    sdmxObjects = dal.GetDataProviderScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "DATACONSUMERSCHEME":
                    sdmxObjects = dal.GetDataConsumerScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "ORGANIZATIONUNITSCHEME":
                    sdmxObjects = dal.GetOrganisationUnitScheme(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "CONTENTCONSTRAINT":
                    sdmxObjects = dal.GetContentConstraint(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "STRUCTURESET":
                    sdmxObjects = dal.GetStructureSet(new ISTAT.Entity.ArtefactIdentity(ucID, ucAgency, ucVersion), chkStub.Checked, false);
                    break;
                case "HCL":
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


        private void DownloadRTF()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;
            RtfDocument doc;

            _dsdWithRef = true;

            sdmxObjects = GetSdmxObjects();

            doc = CreateRTF(sdmxObjects);

            if (doc != null)
                file.SaveRTFFile(doc, GetRTFFileName());
        }

        private RtfDocument CreateRTF(ISdmxObjects sdmxObjects)
        {

            if (!sdmxObjects.HasDataStructures)
                return null;

            IDataStructureObject dsd = sdmxObjects.DataStructures.First();
            IConceptSchemeObject cs;

            var doc = new RtfDocument(PaperSize.A4, PaperOrientation.Landscape, Lcid.English);

            // Create fonts and colors for later use
            var times = doc.createFont("Times New Roman");
            var courier = doc.createFont("Courier New");
            var gray = doc.createColor(new RtfColor("BABABA"));
            var darkGray = doc.createColor(new RtfColor("AAAAAA"));

            int tableWidth = 750;

            #region /////////////////////////////////////// DSD TABLE HEADER //////////////////////////////

            RtfCharFormat cf;
            RtfParagraph parAppo;
            //var lightGray = doc.createColor(new RtfColor("BABABA"));

            RtfTable DSDTable = doc.addTable(4, 4, tableWidth, 12);
            DSDTable.Margins[Direction.Bottom] = 20;
            DSDTable.setInnerBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
            DSDTable.setOuterBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
            //DSDTable.RowAltBackgroundColour = lightGray;

            DSDTable.merge(0, 0, 1, 4);
            parAppo = DSDTable.cell(0, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Dsdef);
            parAppo.Alignment = Align.Center;
            DSDTable.cell(0, 0).BackgroundColour = darkGray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            parAppo = DSDTable.cell(1, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_NameDescr);
            parAppo.Alignment = Align.Center;
            DSDTable.cell(1, 0).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            parAppo = DSDTable.cell(1, 1).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            DSDTable.cell(1, 1).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            parAppo = DSDTable.cell(1, 2).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            DSDTable.cell(1, 2).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            parAppo = DSDTable.cell(1, 3).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            DSDTable.cell(1, 3).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            #endregion

            #region ////////// DSD TABLE DATA //////////

            // Riga 1 Name
            parAppo = DSDTable.cell(2, 0).addParagraph();
            parAppo.setText(_localizedUtils.GetNameableName(dsd));

            parAppo = DSDTable.cell(2, 1).addParagraph();
            parAppo.setText(dsd.Id);

            parAppo = DSDTable.cell(2, 2).addParagraph();
            parAppo.setText(dsd.AgencyId);

            parAppo = DSDTable.cell(2, 3).addParagraph();
            parAppo.setText(dsd.Version);

            // Riga 2 Description
            parAppo = DSDTable.cell(3, 0).addParagraph();
            if (_localizedUtils.GetNameableDescription(dsd) != string.Empty)
                parAppo.setText(_localizedUtils.GetNameableDescription(dsd));

            #endregion

            doc.addParagraph();

            #region /////////////////////////////////////// DSD TABLE DIMENSIONS //////////////////////////////

            RtfTable DSDDim = doc.addTable(dsd.DimensionList.Dimensions.Count() + 4, 10, tableWidth, 12);
            DSDDim.Margins[Direction.Bottom] = 20;
            DSDDim.setInnerBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
            DSDDim.setOuterBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);

            DSDDim.merge(0, 0, 1, 10);
            parAppo = DSDDim.cell(0, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Dimensioni);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(0, 0).BackgroundColour = darkGray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(1, 0, 1, 5);
            parAppo = DSDDim.cell(1, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Concetti);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(1, 0).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(1, 5, 1, 4);
            parAppo = DSDDim.cell(1, 5).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Representation);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(1, 5).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(1, 9, 3, 1);
            parAppo = DSDDim.cell(1, 9).addParagraph();
            DSDDim.cell(1, 9).AlignmentVertical = AlignVertical.Middle;
            parAppo.setText(Resources.Messages.lbl_ab_DimType);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(1, 9).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(2, 0, 2, 1);
            parAppo = DSDDim.cell(2, 0).addParagraph();
            DSDDim.cell(2, 0).AlignmentVertical = AlignVertical.Middle;
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(2, 0).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(2, 1, 2, 1);
            parAppo = DSDDim.cell(2, 1).addParagraph();
            DSDDim.cell(2, 1).AlignmentVertical = AlignVertical.Middle;
            parAppo.setText(Resources.Messages.lbl_ab_Name);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(2, 1).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(2, 2, 1, 3);
            parAppo = DSDDim.cell(2, 2).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ConceptSchema);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(2, 2).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(2, 5, 1, 3);
            parAppo = DSDDim.cell(2, 5).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Codelist);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(2, 5).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(2, 8, 2, 1);
            parAppo = DSDDim.cell(2, 8).addParagraph();
            DSDDim.cell(2, 8).AlignmentVertical = AlignVertical.Middle;
            parAppo.setText(Resources.Messages.lbl_ab_TextFormat);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(2, 8).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(3, 2, 1, 1);
            parAppo = DSDDim.cell(3, 2).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(3, 2).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(3, 3, 1, 1);
            parAppo = DSDDim.cell(3, 3).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(3, 3).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(3, 4, 1, 1);
            parAppo = DSDDim.cell(3, 4).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(3, 4).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(3, 5, 1, 1);
            parAppo = DSDDim.cell(3, 5).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(3, 5).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(3, 6, 1, 1);
            parAppo = DSDDim.cell(3, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(3, 6).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDDim.merge(3, 7, 1, 1);
            parAppo = DSDDim.cell(3, 7).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            DSDDim.cell(3, 7).BackgroundColour = gray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            #endregion

            #region /////////////////////////// DSD DIMENSION DATA

            int rowDimCount = 3;

            //foreach (var dimension in dsd.DimensionList.Dimensions.Where(c => c.HasCodedRepresentation()))
            foreach (var dimension in dsd.DimensionList.Dimensions)
            {
                string TextFormat = String.Empty;

                ++rowDimCount;

                if (!dimension.HasCodedRepresentation())
                {
                    TextFormat = "String";
                }
                else if (dimension.Representation.TextFormat != null)
                {
                    TextFormat = dimension.Representation.TextFormat.TextType.EnumType.ToString();
                }


                cs = sdmxObjects.ConceptSchemes.Where(ci => ci.Id == dimension.ConceptRef.MaintainableId
                                                    && ci.AgencyId == dimension.ConceptRef.AgencyId
                                                    && ci.Version == dimension.ConceptRef.Version).FirstOrDefault();

                parAppo = DSDDim.cell(rowDimCount, 0).addParagraph();
                parAppo.setText(dimension.ConceptRef.FullId);

                parAppo = DSDDim.cell(rowDimCount, 1).addParagraph();
                parAppo.setText(_localizedUtils.GetNameableName(cs));

                parAppo = DSDDim.cell(rowDimCount, 2).addParagraph();
                parAppo.setText(cs.Id);

                parAppo = DSDDim.cell(rowDimCount, 3).addParagraph();
                parAppo.setText(cs.Version);

                parAppo = DSDDim.cell(rowDimCount, 4).addParagraph();
                parAppo.setText(cs.AgencyId);

                if (dimension.HasCodedRepresentation())
                {
                    parAppo = DSDDim.cell(rowDimCount, 5).addParagraph();
                    parAppo.setText(dimension.Representation.Representation.MaintainableId);

                    parAppo = DSDDim.cell(rowDimCount, 6).addParagraph();
                    parAppo.setText(dimension.Representation.Representation.Version);

                    parAppo = DSDDim.cell(rowDimCount, 7).addParagraph();
                    parAppo.setText(dimension.Representation.Representation.AgencyId);

                }
                else
                {
                    parAppo = DSDDim.cell(rowDimCount, 5).addParagraph();
                    parAppo.setText("");

                    parAppo = DSDDim.cell(rowDimCount, 6).addParagraph();
                    parAppo.setText("");

                    parAppo = DSDDim.cell(rowDimCount, 7).addParagraph();
                    parAppo.setText("");
                }

                parAppo = DSDDim.cell(rowDimCount, 8).addParagraph();
                parAppo.setText(TextFormat);

                parAppo = DSDDim.cell(rowDimCount, 9).addParagraph();
                parAppo.setText(_entityMapper.GetDimensionRole(dimension));

            }

            #endregion

            doc.addParagraph();

            #region /////////////////////////////////////// DSD TABLE MEASURES //////////////////////////////

            RtfTable DSDMeasure = doc.addTable(5, 12, tableWidth, 12);
            DSDMeasure.Margins[Direction.Bottom] = 20;
            DSDMeasure.setInnerBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
            DSDMeasure.setOuterBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);

            DSDMeasure.merge(0, 0, 1, 12);
            parAppo = DSDMeasure.cell(0, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Measures);
            parAppo.Alignment = Align.Center;
            DSDMeasure.cell(0, 0).BackgroundColour = darkGray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(1, 0, 3, 1);
            DSDMeasure.cell(1, 0).BackgroundColour = gray;
            DSDMeasure.cell(1, 0).AlignmentVertical = AlignVertical.Middle;
            parAppo = DSDMeasure.cell(1, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Type);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(1, 1, 1, 5);
            DSDMeasure.cell(1, 1).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(1, 1).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Concetti);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(1, 6, 1, 4);
            DSDMeasure.cell(1, 6).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(1, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Representation);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(1, 10, 3, 1);
            DSDMeasure.cell(1, 10).AlignmentVertical = AlignVertical.Middle;
            DSDMeasure.cell(1, 10).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(1, 10).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_MeasureDimension);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(1, 11, 3, 1);
            DSDMeasure.cell(1, 11).AlignmentVertical = AlignVertical.Middle;
            DSDMeasure.cell(1, 11).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(1, 11).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Code);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(2, 1, 2, 1);
            DSDMeasure.cell(2, 1).AlignmentVertical = AlignVertical.Middle;
            DSDMeasure.cell(2, 1).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(2, 1).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(2, 2, 2, 1);
            DSDMeasure.cell(2, 2).AlignmentVertical = AlignVertical.Middle;
            DSDMeasure.cell(2, 2).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(2, 2).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Name);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(2, 3, 1, 3);
            DSDMeasure.cell(2, 3).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(2, 3).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ConceptSchema);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(2, 6, 1, 3);
            DSDMeasure.cell(2, 6).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(2, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Codelist);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(2, 9, 2, 1);
            DSDMeasure.cell(2, 9).AlignmentVertical = AlignVertical.Middle;
            DSDMeasure.cell(2, 9).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(2, 9).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_TextFormat);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(3, 3, 1, 1);
            DSDMeasure.cell(3, 3).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(3, 3).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(3, 4, 1, 1);
            DSDMeasure.cell(3, 4).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(3, 4).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(3, 5, 1, 1);
            DSDMeasure.cell(3, 5).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(3, 5).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(3, 6, 1, 1);
            DSDMeasure.cell(3, 6).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(3, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(3, 7, 1, 1);
            DSDMeasure.cell(3, 7).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(3, 7).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDMeasure.merge(3, 8, 1, 1);
            DSDMeasure.cell(3, 8).BackgroundColour = gray;
            parAppo = DSDMeasure.cell(3, 8).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            #endregion

            #region /////////////////////////// DSD MEASURE DATA

            if (dsd.PrimaryMeasure != null)
            {
                int rowMeasureCount = 3;

                ICrossReference conceptRef = dsd.PrimaryMeasure.ConceptRef;

                cs = sdmxObjects.ConceptSchemes.Where(ci => ci.Id == conceptRef.MaintainableId
                                                    && ci.AgencyId == conceptRef.AgencyId
                                                    && ci.Version == conceptRef.Version).FirstOrDefault();

                string clID = "", clVer = "", clAgency = "";

                if (dsd.PrimaryMeasure.Representation != null && dsd.PrimaryMeasure.Representation.Representation != null)
                {
                    ICrossReference cl = dsd.PrimaryMeasure.Representation.Representation;
                    clID = cl.MaintainableId;
                    clAgency = cl.AgencyId;
                    clVer = cl.Version;
                }

                ++rowMeasureCount;

                parAppo = DSDMeasure.cell(rowMeasureCount, 0).addParagraph();
                parAppo.setText("Primary");

                parAppo = DSDMeasure.cell(rowMeasureCount, 1).addParagraph();
                parAppo.setText(conceptRef.FullId);

                parAppo = DSDMeasure.cell(rowMeasureCount, 2).addParagraph();
                parAppo.setText(_localizedUtils.GetNameableName(cs));

                parAppo = DSDMeasure.cell(rowMeasureCount, 3).addParagraph();
                parAppo.setText(cs.Id);

                parAppo = DSDMeasure.cell(rowMeasureCount, 4).addParagraph();
                parAppo.setText(cs.Version);

                parAppo = DSDMeasure.cell(rowMeasureCount, 5).addParagraph();
                parAppo.setText(cs.AgencyId);

                parAppo = DSDMeasure.cell(rowMeasureCount, 6).addParagraph();
                parAppo.setText(clID);

                parAppo = DSDMeasure.cell(rowMeasureCount, 7).addParagraph();
                parAppo.setText(clVer);

                parAppo = DSDMeasure.cell(rowMeasureCount, 8).addParagraph();
                parAppo.setText(clAgency);

                parAppo = DSDMeasure.cell(rowMeasureCount, 9).addParagraph();
                parAppo.setText("");
                //parAppo.setText("[textformat]");

                parAppo = DSDMeasure.cell(rowMeasureCount, 10).addParagraph();
                parAppo.setText("");
                //parAppo.setText("[measuredim]");

                parAppo = DSDMeasure.cell(rowMeasureCount, 11).addParagraph();
                parAppo.setText("");
                //parAppo.setText("[code]");

            }

            #endregion

            doc.addParagraph();

            #region /////////////////////////////////////// DSD TABLE ATTRIBUTES //////////////////////////////

            RtfTable DSDAttr = doc.addTable(dsd.Attributes.Count() + 4, 12, tableWidth, 12);
            DSDAttr.Margins[Direction.Bottom] = 20;
            DSDAttr.setInnerBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
            DSDAttr.setOuterBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);

            DSDAttr.merge(0, 0, 1, 12);
            parAppo = DSDAttr.cell(0, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Attributi);
            parAppo.Alignment = Align.Center;
            DSDAttr.cell(0, 0).BackgroundColour = darkGray;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(1, 0, 3, 1);
            DSDAttr.cell(1, 0).AlignmentVertical = AlignVertical.Middle;
            DSDAttr.cell(1, 0).BackgroundColour = gray;
            parAppo = DSDAttr.cell(1, 0).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_AttachLevel);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(1, 1, 1, 5);
            DSDAttr.cell(1, 1).BackgroundColour = gray;
            parAppo = DSDAttr.cell(1, 1).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Concetti);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(1, 6, 1, 4);
            DSDAttr.cell(1, 6).BackgroundColour = gray;
            parAppo = DSDAttr.cell(1, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Representation);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(1, 10, 3, 1);
            DSDAttr.cell(1, 10).AlignmentVertical = AlignVertical.Middle;
            DSDAttr.cell(1, 10).BackgroundColour = gray;
            parAppo = DSDAttr.cell(1, 10).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_AttributeType);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(1, 11, 3, 1);
            DSDAttr.cell(1, 11).AlignmentVertical = AlignVertical.Middle;
            DSDAttr.cell(1, 11).BackgroundColour = gray;
            parAppo = DSDAttr.cell(1, 11).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_assi_status);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(2, 1, 2, 1);
            DSDAttr.cell(2, 1).AlignmentVertical = AlignVertical.Middle;
            DSDAttr.cell(2, 1).BackgroundColour = gray;
            parAppo = DSDAttr.cell(2, 1).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(2, 2, 2, 1);
            DSDAttr.cell(2, 2).AlignmentVertical = AlignVertical.Middle;
            DSDAttr.cell(2, 2).BackgroundColour = gray;
            parAppo = DSDAttr.cell(2, 2).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Name);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(2, 3, 1, 3);
            DSDAttr.cell(2, 3).BackgroundColour = gray;
            parAppo = DSDAttr.cell(2, 3).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ConceptSchema);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(2, 6, 1, 3);
            DSDAttr.cell(2, 6).BackgroundColour = gray;
            parAppo = DSDAttr.cell(2, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Codelist);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(2, 9, 2, 1);
            DSDAttr.cell(2, 9).AlignmentVertical = AlignVertical.Middle;
            DSDAttr.cell(2, 9).BackgroundColour = gray;
            parAppo = DSDAttr.cell(2, 9).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_TextFormat);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(3, 3, 1, 1);
            DSDAttr.cell(3, 3).BackgroundColour = gray;
            parAppo = DSDAttr.cell(3, 3).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(3, 4, 1, 1);
            DSDAttr.cell(3, 4).BackgroundColour = gray;
            parAppo = DSDAttr.cell(3, 4).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(3, 5, 1, 1);
            DSDAttr.cell(3, 5).BackgroundColour = gray;
            parAppo = DSDAttr.cell(3, 5).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(3, 6, 1, 1);
            DSDAttr.cell(3, 6).BackgroundColour = gray;
            parAppo = DSDAttr.cell(3, 6).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_ID);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(3, 7, 1, 1);
            DSDAttr.cell(3, 7).BackgroundColour = gray;
            parAppo = DSDAttr.cell(3, 7).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Version);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            DSDAttr.merge(3, 8, 1, 1);
            DSDAttr.cell(3, 8).BackgroundColour = gray;
            parAppo = DSDAttr.cell(3, 8).addParagraph();
            parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
            parAppo.Alignment = Align.Center;
            cf = parAppo.addCharFormat();
            cf.FontStyle.addStyle(FontStyleFlag.Bold);

            #endregion

            #region /////////////////////////// DSD ATTRIBUTES DATA

            if (dsd.AttributeList != null)
            {
                int rowMAttrCount = 3;

                foreach (IAttributeObject attribute in dsd.AttributeList.Attributes)
                {
                    string TextFormat = String.Empty;
                    string CodeList = String.Empty;
                    string clID = "", clVer = "", clAgency = "";

                    ++rowMAttrCount;

                    if (attribute.Representation != null && attribute.Representation.Representation != null)
                    {
                        ICrossReference cl = attribute.Representation.Representation;
                        clID = cl.MaintainableId;
                        clAgency = cl.AgencyId;
                        clVer = cl.Version;
                        if (attribute.Representation.TextFormat != null)
                            TextFormat = attribute.Representation.TextFormat.TextType.EnumType.ToString();

                    }

                    cs = sdmxObjects.ConceptSchemes.Where(ci => ci.Id == attribute.ConceptRef.MaintainableId
                                    && ci.AgencyId == attribute.ConceptRef.AgencyId
                                    && ci.Version == attribute.ConceptRef.Version).FirstOrDefault();

                    parAppo = DSDAttr.cell(rowMAttrCount, 0).addParagraph();
                    parAppo.setText(attribute.AttachmentLevel.ToString());

                    parAppo = DSDAttr.cell(rowMAttrCount, 1).addParagraph();
                    parAppo.setText(attribute.ConceptRef.FullId);

                    parAppo = DSDAttr.cell(rowMAttrCount, 2).addParagraph();
                    parAppo.setText(_localizedUtils.GetNameableName(cs));

                    parAppo = DSDAttr.cell(rowMAttrCount, 3).addParagraph();
                    parAppo.setText(cs.Id);

                    parAppo = DSDAttr.cell(rowMAttrCount, 4).addParagraph();
                    parAppo.setText(cs.Version);

                    parAppo = DSDAttr.cell(rowMAttrCount, 5).addParagraph();
                    parAppo.setText(cs.AgencyId);

                    parAppo = DSDAttr.cell(rowMAttrCount, 6).addParagraph();
                    parAppo.setText(clID);

                    parAppo = DSDAttr.cell(rowMAttrCount, 7).addParagraph();
                    parAppo.setText(clVer);

                    parAppo = DSDAttr.cell(rowMAttrCount, 8).addParagraph();
                    parAppo.setText(clAgency);

                    parAppo = DSDAttr.cell(rowMAttrCount, 9).addParagraph();
                    parAppo.setText(TextFormat);

                    parAppo = DSDAttr.cell(rowMAttrCount, 10).addParagraph();
                    parAppo.setText("");
                    //parAppo.setText("[attribute_type]");

                    parAppo = DSDAttr.cell(rowMAttrCount, 11).addParagraph();
                    parAppo.setText(attribute.AssignmentStatus);

                }

            }

            #endregion

            doc.addParagraph();

            #region ////////////////////// CODELISTS /////////////////////

            if (sdmxObjects.HasCodelists)
            {
                RtfTable csTable;

                foreach (ICodelistObject cl in sdmxObjects.Codelists)
                {
                    doc.addParagraph();

                    csTable = doc.addTable(cl.Items.Count() + 5, 3, tableWidth, 12);

                    csTable.Margins[Direction.Bottom] = 20;
                    csTable.setInnerBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
                    csTable.setOuterBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);

                    csTable.merge(0, 0, 1, 3);
                    parAppo = csTable.cell(0, 0).addParagraph();
                    csTable.cell(0, 0).BackgroundColour = darkGray;
                    parAppo.setText(Resources.Messages.lbl_ab_Codelist);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(1, 0).addParagraph();
                    csTable.cell(1, 0).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_ID);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(1, 1).addParagraph();
                    csTable.cell(1, 1).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(1, 2).addParagraph();
                    csTable.cell(1, 2).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Version);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(2, 0).addParagraph();
                    parAppo.setText(cl.Id);

                    parAppo = csTable.cell(2, 1).addParagraph();
                    parAppo.setText(cl.AgencyId);

                    parAppo = csTable.cell(2, 2).addParagraph();
                    parAppo.setText(cl.Version);

                    csTable.merge(3, 0, 1, 3);
                    parAppo = csTable.cell(3, 0).addParagraph();
                    csTable.cell(3, 0).BackgroundColour = darkGray;
                    parAppo.setText(Resources.Messages.lbl_ab_Codes);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(4, 0).addParagraph();
                    csTable.cell(4, 0).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_ID);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(4, 1).addParagraph();
                    csTable.cell(4, 1).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Name);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(4, 2).addParagraph();
                    csTable.cell(4, 2).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Parent);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    int rowCodeCount = 4;

                    foreach (ICode code in cl.Items)
                    {
                        ++rowCodeCount;

                        parAppo = csTable.cell(rowCodeCount, 0).addParagraph();
                        parAppo.setText(code.Id);

                        parAppo = csTable.cell(rowCodeCount, 1).addParagraph();
                        parAppo.setText(_localizedUtils.GetNameableName(code));

                        parAppo = csTable.cell(rowCodeCount, 2).addParagraph();
                        parAppo.setText(code.ParentCode);
                    }
                }
            }

            #endregion

            doc.addParagraph();

            #region ////////////////////// CONCEPT SCHEMES /////////////////////

            if (sdmxObjects.HasConceptSchemes)
            {
                RtfTable csTable;

                foreach (IConceptSchemeObject csc in sdmxObjects.ConceptSchemes)
                {
                    doc.addParagraph();

                    csTable = doc.addTable(csc.Items.Count() + 5, 3, tableWidth, 12);

                    csTable.Margins[Direction.Bottom] = 20;
                    csTable.setInnerBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);
                    csTable.setOuterBorder(Elistia.DotNetRtfWriter.BorderStyle.Single, 1f);

                    csTable.merge(0, 0, 1, 3);
                    parAppo = csTable.cell(0, 0).addParagraph();
                    csTable.cell(0, 0).BackgroundColour = darkGray;
                    parAppo.setText(Resources.Messages.lbl_ab_ConceptSchema);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(1, 0).addParagraph();
                    csTable.cell(1, 0).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_ID);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(1, 1).addParagraph();
                    csTable.cell(1, 1).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Agenzia);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(1, 2).addParagraph();
                    csTable.cell(1, 2).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Version);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(2, 0).addParagraph();
                    parAppo.setText(csc.Id);

                    parAppo = csTable.cell(2, 1).addParagraph();
                    parAppo.setText(csc.AgencyId);

                    parAppo = csTable.cell(2, 2).addParagraph();
                    parAppo.setText(csc.Version);

                    csTable.merge(3, 0, 1, 3);
                    parAppo = csTable.cell(3, 0).addParagraph();
                    csTable.cell(3, 0).BackgroundColour = darkGray;
                    parAppo.setText(Resources.Messages.lbl_ab_Concetti);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(4, 0).addParagraph();
                    csTable.cell(4, 0).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_ID);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(4, 1).addParagraph();
                    csTable.cell(4, 1).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Name);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    parAppo = csTable.cell(4, 2).addParagraph();
                    csTable.cell(4, 2).BackgroundColour = gray;
                    parAppo.setText(Resources.Messages.lbl_ab_Parent);
                    parAppo.Alignment = Align.Center;
                    cf = parAppo.addCharFormat();
                    cf.FontStyle.addStyle(FontStyleFlag.Bold);

                    int rowCodeCount = 4;

                    foreach (IConceptObject code in csc.Items)
                    {
                        ++rowCodeCount;

                        parAppo = csTable.cell(rowCodeCount, 0).addParagraph();
                        parAppo.setText(code.Id);

                        parAppo = csTable.cell(rowCodeCount, 1).addParagraph();
                        parAppo.setText(_localizedUtils.GetNameableName(code));

                        parAppo = csTable.cell(rowCodeCount, 2).addParagraph();
                        parAppo.setText(code.ParentConcept);
                    }
                }
            }

            #endregion

            return doc;
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

            string mySeparator = txtSeparator.Text.Trim().Equals(string.Empty) ? ";" : txtSeparator.Text.Trim();

            file.SaveCSVFile(dt, GetFilename(), mySeparator, txtDelimiter.Text);
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

            file.SaveDotSTATFile(sdmxObjects, GetDotSTATExportType(), _dotStatProp);
        }

        private void DownloadDotStatCodelist()
        {
            IOUtils file = new IOUtils();
            ISdmxObjects sdmxObjects = null;

            sdmxObjects = GetSdmxObjects();

            if (sdmxObjects == null)
                return;

            file.SaveDotSTATCodelistFile(sdmxObjects.Codelists.First(), _dotStatProp);
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
                    sdmxObjectsTemp = dal.GetCodeList(new ArtefactIdentity(rep.MaintainableId, rep.AgencyId, rep.Version), stub, false);
                    sdmxObjects.AddCodelist(sdmxObjectsTemp.Codelists.First());
                }
            }

            foreach (IAttributeObject att in sdmxObjects.DataStructures.First().Attributes)
            {
                if (att.HasCodedRepresentation())
                {
                    var rep = att.Representation.Representation;
                    sdmxObjectsTemp = dal.GetCodeList(new ArtefactIdentity(rep.MaintainableId, rep.AgencyId, rep.Version), stub, false);
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
            return ucID + "_" + ucAgency + "_" + ucVersion + sVersion + (chkStub.Checked ? "-stub" : string.Empty) + "." + fileLang;
        }

        private string GetRTFFileName()
        {
            string fileLang = Session[ISTATRegistry.SESSION_KEYS.KEY_LANG].ToString();
            return ucID + "_" + ucAgency + "_" + ucVersion + "-rtf-" + fileLang;
        }

        private void RecursiveOnItems(ICategoryObject code, ref DataTable dt)
        {
            LocalizedUtils localizedUtils = new LocalizedUtils(Utils.LocalizedCulture);
            string completeSequence = code.Parent.ToString().Split('=')[1].Split(')')[1];
            if (!completeSequence.Equals(string.Empty))
            {
                completeSequence = completeSequence.Remove(0, 1);
            }
            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription(code), completeSequence);
            if (code.Items.Count != 0)
            {
                foreach (ICategoryObject subCode in code.Items)
                {
                    RecursiveOnItems(subCode, ref dt);
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
                            dt.Rows.Add(concept.Id, localizedUtils.GetNameableName(concept), localizedUtils.GetNameableDescription(concept), concept.ParentConcept);
                        }
                        break;
                    }
                    break;
                case "CategoryScheme":
                    foreach (ICategorySchemeObject cs in sdmxObjects.CategorySchemes)
                    {
                        foreach (ICategoryObject code in cs.Items)
                        {
                            string completeSequence = code.Parent.ToString().Split('=')[1].Split(')')[1];
                            if (!completeSequence.Equals(string.Empty))
                            {
                                completeSequence = completeSequence.Remove(0, 1);
                            }
                            dt.Rows.Add(code.Id, localizedUtils.GetNameableName(code), localizedUtils.GetNameableDescription(code), completeSequence);
                            if (code.Items.Count != 0)
                            {
                                foreach (ICategoryObject subCode in code.Items)
                                {
                                    RecursiveOnItems(subCode, ref dt);
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
                            dt.Rows.Add(agencyItem.Id, localizedUtils.GetNameableName(agencyItem), localizedUtils.GetNameableDescription(agencyItem));
                        }
                        break;
                    }
                    break;
                case "DataProviderScheme":
                    foreach (IDataProviderScheme dataProviderScheme in sdmxObjects.DataProviderSchemes)
                    {
                        foreach (IDataProvider dataProviderSchemeItem in dataProviderScheme.Items)
                        {
                            dt.Rows.Add(dataProviderSchemeItem.Id, localizedUtils.GetNameableName(dataProviderSchemeItem), localizedUtils.GetNameableDescription(dataProviderSchemeItem));
                        }
                        break;
                    }
                    break;
                case "DataConsumerScheme":
                    foreach (IDataConsumerScheme dataConsumerScheme in sdmxObjects.DataConsumerSchemes)
                    {
                        foreach (IDataConsumer dataConsumerSchemeItem in dataConsumerScheme.Items)
                        {
                            dt.Rows.Add(dataConsumerSchemeItem.Id, localizedUtils.GetNameableName(dataConsumerSchemeItem), localizedUtils.GetNameableDescription(dataConsumerSchemeItem));
                        }
                        break;
                    }
                    break;
                case "OrganizationUnitScheme":
                    foreach (IOrganisationUnitSchemeObject organizationUnitScheme in sdmxObjects.OrganisationUnitSchemes)
                    {
                        foreach (IOrganisationUnit organizationUnitSchemeItem in organizationUnitScheme.Items)
                        {
                            dt.Rows.Add(organizationUnitSchemeItem.Id, localizedUtils.GetNameableName(organizationUnitSchemeItem), localizedUtils.GetNameableDescription(organizationUnitSchemeItem), organizationUnitSchemeItem.ParentUnit);
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
                    dt.Columns.Add("Description");
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