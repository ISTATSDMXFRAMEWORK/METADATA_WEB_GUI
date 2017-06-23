using ISTAT.Entity;
using ISTAT.WSDAL;
using ISTATRegistry.Classes;
using ISTATRegistry.IRServiceReference;
using ISTATRegistry.UserControls;
using ISTATUtils;
using Newtonsoft.Json;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;

namespace ISTATRegistry.WebServices
{
    /// <summary>
    /// Descrizione di riepilogo per IRWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente. 
    // [System.Web.Script.Services.ScriptService]
    [System.Web.Script.Services.ScriptService]
    public class IRWebService : System.Web.Services.WebService
    {

        public IRWebService()
        {
            CultureInfo culture;

            culture = LocaleResolver.GetCookie(HttpContext.Current);

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            // Save cookie
            LocaleResolver.SendCookie(culture, HttpContext.Current);
        }

        #region Public Methods

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public string WSAutenticateUser(string userName, string password, string endpoint)
        {
            //List<string> lAuthenticatedWebServices;
            Dictionary<string, UserAgency[]> dictAuthenticatedWebServices;

            if (HttpContext.Current.Session["AuthenticatedWebServices"] != null)
                dictAuthenticatedWebServices = (Dictionary<string, UserAgency[]>)HttpContext.Current.Session["AuthenticatedWebServices"];
            else
                dictAuthenticatedWebServices = new Dictionary<string, UserAgency[]>();

            if (!dictAuthenticatedWebServices.ContainsKey(endpoint))
            {
                EndPointElement epe = IRConfiguration.GetEndPointByName(endpoint);
                if (!(userName.Trim() == "" || password.Trim() == "") && AutenticateUser(userName, password, epe) != null)
                {
                    User user = AutenticateUser(userName, password, epe);

                    // aggiunta endpoint alla session
                    dictAuthenticatedWebServices.Add(endpoint, user.agencies);
                    HttpContext.Current.Session["AuthenticatedWebServices"] = dictAuthenticatedWebServices;
                }
                else
                {
                    // messaggio di errore credenziali non corrette

                    string asd = Messages.err_wrong_credentials;

                    throw new Exception(Messages.err_wrong_credentials);
                    //throw new Exception(JsonMessage.GetError("Wrong Credentials"));
                }
            }

            return JsonMessage.GetData(true);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public string GetAgencies(string endPointName)
        {

            Dictionary<string, UserAgency[]> dictAuthenticatedWebServices;

            if (HttpContext.Current.Session["AuthenticatedWebServices"] != null)
                dictAuthenticatedWebServices = (Dictionary<string, UserAgency[]>)HttpContext.Current.Session["AuthenticatedWebServices"];
            else
                return "";

            return JsonMessage.GetData(dictAuthenticatedWebServices[endPointName]);

            //ISdmxObjects sdmxAgencies;
            //List<string> lAgencies = new List<string>();
            //string sourceEndPoint = "";

            //try
            //{
            //    EndPointElement sourceEP = (EndPointElement)Session["WSEndPoint"];
            //    sourceEndPoint = sourceEP.Name;

            //    EndPointElement epe = IRConfiguration.GetEndPointByName(endPointName);

            //    Session["WSEndPoint"] = epe;

            //    WSModel wsModel = new WSModel();

            //    sdmxAgencies = GetSdmxObjects(new ArtefactIdentity() {Agency="",ID="",Version="" }, "AGENCYSCHEME", false);

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    EndPointElement epe = IRConfiguration.GetEndPointByName(sourceEndPoint);
            //    Session["WSEndPoint"] = epe;
            //}

            //if (sdmxAgencies.HasAgenciesSchemes)
            //{
            //    foreach(IAgencyScheme ags in sdmxAgencies.AgenciesSchemes)
            //    {
            //        foreach(IAgency ag in ags.Items)
            //        {
            //            if(!lAgencies.Contains(ag.Id))
            //                lAgencies.Add(ag.Id);
            //        }
            //    }

            //    return JsonMessage.GetData(lAgencies);
            //    //return new JavaScriptSerializer().Serialize(lAgencies);
            //}
            //else
            //    return JsonMessage.GetData(false);
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public string ISWSExportAuth(string endPointName)
        {

            //return JsonConvert.SerializeObject(dict);

            //            List<string> lAuthenticatedWebServices;
            Dictionary<string, UserAgency[]> dictAuthenticatedWebServices;


            //return "ciao";

            //return JsonConvert.SerializeObject("asda");

            //return JsonMessage.GetData(dict);

            //return new JavaScriptSerializer().Serialize(true);

            //return new JavaScriptSerializer().Serialize(dict);


            if (HttpContext.Current.Session["AuthenticatedWebServices"] != null)
            {
                dictAuthenticatedWebServices = (Dictionary<string, UserAgency[]>)HttpContext.Current.Session["AuthenticatedWebServices"];
                if (dictAuthenticatedWebServices.ContainsKey(endPointName))
                    return JsonMessage.GetData(true);
            }

            return JsonMessage.GetData(false);
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public string ExportMultipleArtefact(string sourceEndPoint, string targetEndPoint,
                                    string artefactType,
                                    List<ArtefactIdentity> lArtefacts)
        {

            ISdmxObjects sdmxObjects;
            ISdmxObjects soFinal = new SdmxObjectsImpl();
            List<string> lImportedArtefacts;

            foreach (ArtefactIdentity artIdentity in lArtefacts)
            {
                sdmxObjects = GetSdmxObjects(artIdentity, artefactType, false);


                Dictionary<string, UserAgency[]> dictAuthenticatedWebServices;
                UserAgency[] lUA = null;

                if (HttpContext.Current.Session["AuthenticatedWebServices"] != null)
                {
                    dictAuthenticatedWebServices = (Dictionary<string, UserAgency[]>)HttpContext.Current.Session["AuthenticatedWebServices"];
                    lUA = dictAuthenticatedWebServices[targetEndPoint];

                    if (lUA == null)
                        break;
                }

                switch (artefactType.ToUpper())
                {
                    case "CODELIST":
                        if (sdmxObjects.HasCodelists)

                            foreach (UserAgency ua in lUA)
                            {
                                if (artIdentity.Agency == ua.id)
                                {
                                    soFinal.AddCodelist(sdmxObjects.Codelists.First());
                                    break;
                                }
                            }
                        break;
                    case "CONCEPTSCHEME":
                        break;
                    case "CATEGORYSCHEME":
                        break;
                    case "DATAFLOW":
                        break;
                    case "KEYFAMILY":
                        break;
                    case "CATEGORIZATION":
                        break;
                    case "AGENCYSCHEME":
                        break;
                    case "DATAPROVIDERSCHEME":
                        break;
                    case "DATACONSUMERSCHEME":
                        break;
                    case "ORGANIZATIONUNITSCHEME":
                        break;
                    case "CONTENTCONSTRAINT":
                        break;
                    case "STRUCTURESET":
                        break;
                    case "HCL":
                        break;
                    default:
                        return null;
                }
            }


            try
            {
                EndPointElement epe = IRConfiguration.GetEndPointByName(targetEndPoint);

                Session["WSEndPoint"] = epe;

                WSModel wsModel = new WSModel();

                XmlDocument xRet = wsModel.SubmitStructure(soFinal);

                string err = Utils.GetXMLResponseError(xRet);

                if (err != "")
                {
                    throw new Exception(err);
                }

                XDocument doc = XDocument.Parse(xRet.OuterXml);

                XNamespace ns = @"http://www.sdmx.org/resources/sdmxml/schemas/v2_1/registry";

                //importedArtefactsMessage = "Artefatti importati</br>";

                lImportedArtefacts = new List<string>();

                foreach (XElement element in doc.Descendants(ns + "SubmissionResult"))
                    lImportedArtefacts.Add(element.Descendants("URN").Single().Value);
                //importedArtefactsMessage += "</br>"+ element.Descendants("URN").Single().Value + " : " + element.Descendants(ns + "StatusMessage").Single().Attribute("status").Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EndPointElement epe = IRConfiguration.GetEndPointByName(sourceEndPoint);
                Session["WSEndPoint"] = epe;
            }

            //6. Restituisco un messaggio di operazione eseguita con successo
            return JsonMessage.GetData(lImportedArtefacts); // Messages.succ_operation;
            //return JsonMessage.GetData(Messages.succ_operation);
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public string ExportArtefact(string sourceEndPoint, string targetEndPoint,
                                    string artefactType, string artID,
                                    string artAgency, string artVersion,
                                    string newArtID, string newArtAgency, string newArtVersion,
                                    bool exportWithRef)
        {

            //FileDownload3 FileDownload = new FileDownload3();

            ArtefactIdentity artIdentity = new ArtefactIdentity(artID, artAgency, artVersion);
            ArtefactIdentity newArtIdentity = new ArtefactIdentity(newArtID, newArtAgency, newArtVersion);
            //string importedArtefactsMessage;
            List<string> lImportedArtefacts;

            #region Controlli formali su ID,Agency e Version

            //* 2. Verifico la correttezza di ID,Agency e Version
            string errData = ValidateData(newArtIdentity.ID, newArtIdentity.Agency, newArtIdentity.Version);

            if (errData != string.Empty)
                throw new Exception(errData);

            #endregion

            //* 3. Recupero l'artefatto originale 
            ISdmxObjects sdmxObjects = GetSdmxObjects(artIdentity, artefactType, exportWithRef);
            try{
                EndPointElement epe = IRConfiguration.GetEndPointByName(targetEndPoint);

                Session["WSEndPoint"] = epe;

            //* 4. Modifico ID, Agency e Version
                sdmxObjects = ChangeIdentity(sdmxObjects, newArtIdentity, artefactType, exportWithRef, targetEndPoint);

            //* 5. Salvo l'artefatto nel WS di Destinazione
            
            //devo settare l'endpoint prima del punto 4. sposto il apertura del try
            //try
            //{
            //    EndPointElement epe = IRConfiguration.GetEndPointByName(targetEndPoint);

            //    Session["WSEndPoint"] = epe;

                WSModel wsModel = new WSModel();

                XmlDocument xRet = wsModel.SubmitStructure(sdmxObjects);

                string err = Utils.GetXMLResponseError(xRet);

                if (err != "")
                {
                    throw new Exception(err);
                }

                XDocument doc = XDocument.Parse(xRet.OuterXml);

                XNamespace ns = @"http://www.sdmx.org/resources/sdmxml/schemas/v2_1/registry";

                //importedArtefactsMessage = "Artefatti importati</br>";

                lImportedArtefacts = new List<string>();

                foreach (XElement element in doc.Descendants(ns + "SubmissionResult"))
                    lImportedArtefacts.Add(element.Descendants("URN").Single().Value);
                //importedArtefactsMessage += "</br>"+ element.Descendants("URN").Single().Value + " : " + element.Descendants(ns + "StatusMessage").Single().Attribute("status").Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EndPointElement epe = IRConfiguration.GetEndPointByName(sourceEndPoint);
                Session["WSEndPoint"] = epe;
            }

            //6. Restituisco un messaggio di operazione eseguita con successo
            return JsonMessage.GetData(lImportedArtefacts); // Messages.succ_operation;
            //return JsonMessage.GetData(Messages.succ_operation);
        }

        #endregion

        #region Private Methods


        private string ValidateData(string id, string agency, string version)
        {

            string messagesGroup = string.Empty;    // Stringa di raggruppamento errori
            int errorCounter = 1;                   // Contatore errori

            // Controllo ID
            if (!ValidationUtils.CheckIdFormat(id))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_id_format + "<br /><br />";
                errorCounter++;
            }

            if (!ValidationUtils.CheckIdFormat(agency))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_agency_missing + "<br /><br />";
                errorCounter++;
            }

            // Controllo versione
            if (!ValidationUtils.CheckVersionFormat(version))
            {
                messagesGroup += Convert.ToString(errorCounter) + ") " + Resources.Messages.err_version_format + "<br /><br />";
                errorCounter++;
            }

            return messagesGroup;
        }

        private ISdmxObjects ChangeIdentity(ISdmxObjects sdmxObjects, ArtefactIdentity newArtIdentity, string artefactType, bool exportWithRef, string targetEndPoint)
        {
            ISdmxObjects sdmxObjectsRet = new SdmxObjectsImpl();

            switch (artefactType.ToUpper())
            {
                case "CODELIST":
                    ICodelistObject cl = sdmxObjects.Codelists.First();
                    ICodelistMutableObject clm = cl.MutableInstance;

                    clm.Id = newArtIdentity.ID;
                    clm.AgencyId = newArtIdentity.Agency;
                    clm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddCodelist(clm.ImmutableInstance);
                    break;
                case "CONCEPTSCHEME":
                    IConceptSchemeObject cs = sdmxObjects.ConceptSchemes.First();
                    IConceptSchemeMutableObject csm = cs.MutableInstance;

                    csm.Id = newArtIdentity.ID;
                    csm.AgencyId = newArtIdentity.Agency;
                    csm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddConceptScheme(csm.ImmutableInstance);
                    break;
                case "CATEGORYSCHEME":
                    ICategorySchemeObject csc = sdmxObjects.CategorySchemes.First();
                    ICategorySchemeMutableObject cscm = csc.MutableInstance;

                    cscm.Id = newArtIdentity.ID;
                    cscm.AgencyId = newArtIdentity.Agency;
                    cscm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddCategoryScheme(cscm.ImmutableInstance);
                    break;
                case "DATAFLOW":
                    IDataflowObject df = sdmxObjects.Dataflows.First();
                    IDataflowMutableObject dfm = df.MutableInstance;

                    dfm.Id = newArtIdentity.ID;
                    dfm.AgencyId = newArtIdentity.Agency;
                    dfm.Version = newArtIdentity.Version;
                    
                    //Inizio Andrea
                    if (df.DataStructureRef != null) { 
                        IList<string> idRef = df.DataStructureRef.IdentifiableIds;
                        String[] globalId = idRef.ToArray();

                        WSModel model = new WSModel();
                        ISdmxObjects dsRef = model.GetDataStructure(new ArtefactIdentity(globalId[0], globalId[1], globalId[2]), true, false);
                        if (dsRef.DataStructures.Count == 0 || !(dsRef.DataStructures.First().IsFinal.IsTrue) ) 
                        {
                            sdmxObjectsRet.AddDataStructure(dsRef.DataStructures.First());   
                        }
                    }
                    //Fine Andrea

                    sdmxObjectsRet.AddDataflow(dfm.ImmutableInstance);
                    break;
                case "KEYFAMILY":
                    IDataStructureObject dsd = sdmxObjects.DataStructures.First();
                    IDataStructureMutableObject dsdm = dsd.MutableInstance;

                    dsdm.Id = newArtIdentity.ID;
                    dsdm.AgencyId = newArtIdentity.Agency;
                    dsdm.Version = newArtIdentity.Version;

                    //sdmxObjectsRet = sdmxObjects;
                    //sdmxObjectsRet.RemoveDataStructure(sdmxObjects.DataStructures.First());
                    sdmxObjectsRet.AddDataStructure(dsdm.ImmutableInstance);

                    //Inizio Andrea

                    //aggiunto il controllo se gli artefatti referenziati già esistono 
                    WSModel dal = new WSModel();
                    
                    foreach (IConceptSchemeObject cScheme in sdmxObjects.ConceptSchemes)
                    {
                        ArtefactIdentity aiCS = new ArtefactIdentity(cScheme.Id, cScheme.AgencyId, cScheme.Version);
                        ISdmxObjects ret;
                        try { 
                             ret = dal.GetConceptScheme(aiCS, true, false);
                             if (!(ret.ConceptSchemes.First().IsFinal.IsTrue))
                             {
                                 sdmxObjectsRet.AddConceptScheme(cScheme);
                             }
                        }
                        catch (Exception ex) 
                        {
                            sdmxObjectsRet.AddConceptScheme(cScheme); 
                        } 
                        //if (ret.ConceptSchemes.Count == 0 || !(ret.ConceptSchemes.First().IsFinal.IsTrue) )
                        //{
                        //    sdmxObjectsRet.AddConceptScheme(cScheme);
                        //}
                    }

                    foreach (ICodelistObject cList in sdmxObjects.Codelists) 
                    {
                        ArtefactIdentity aiCL = new ArtefactIdentity(cList.Id, cList.AgencyId, cList.Version);
                        ISdmxObjects ret ;
                        try
                        {
                            ret = dal.GetCodeList(aiCL, true, false);
                            if (!(ret.Codelists.First().IsFinal.IsTrue))
                            {
                                sdmxObjectsRet.AddCodelist(cList);
                            }
                        }
                        catch (Exception ex)
                        {
                            sdmxObjectsRet.AddCodelist(cList);
                        }
                        //if (ret.Codelists.Count == 0 || !(ret.Codelists.First().IsFinal.IsTrue)) 
                        //{
                        //    sdmxObjectsRet.AddCodelist(cList);
                        //}

                    }
                    
                    //questa parte viene commentata in quanto nel caso di export di dsd via ws tutti gli artefatti referenziati, se non presenti, devono essere importati
                    //if (exportWithRef)
                    //{
                    //    Dictionary<string, UserAgency[]> dictAuthenticatedWebServices;
                    //    UserAgency[] lUA = null;

                    //    if (HttpContext.Current.Session["AuthenticatedWebServices"] != null)
                    //    {
                    //        dictAuthenticatedWebServices = (Dictionary<string, UserAgency[]>)HttpContext.Current.Session["AuthenticatedWebServices"];
                    //        lUA = dictAuthenticatedWebServices[targetEndPoint];

                    //        if (lUA == null)
                    //            break;
                    //    }

                    //    if (sdmxObjects.HasConceptSchemes)
                    //    {
                    //        foreach (IConceptSchemeObject cScheme in sdmxObjects.ConceptSchemes)
                    //        {
                    //            foreach (UserAgency ua in lUA)
                    //            {
                    //                if (cScheme.AgencyId == ua.id)
                    //                {
                    //                    sdmxObjectsRet.AddConceptScheme(cScheme);
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (sdmxObjects.HasCodelists)
                    //    {
                    //        foreach (ICodelistObject cList in sdmxObjects.Codelists)
                    //        {
                    //            foreach (UserAgency ua in lUA)
                    //            {
                    //                if (cList.AgencyId == ua.id)
                    //                {
                    //                    sdmxObjectsRet.AddCodelist(cList);
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //fine Andrea
                    break;
                case "CATEGORIZATION":
                    ICategorisationObject c = sdmxObjects.Categorisations.First();
                    ICategorisationMutableObject cm = c.MutableInstance;

                    cm.Id = newArtIdentity.ID;
                    cm.AgencyId = newArtIdentity.Agency;
                    cm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddCategorisation(cm.ImmutableInstance);
                    break;
                case "AGENCYSCHEME":
                    IAgencyScheme asc = sdmxObjects.AgenciesSchemes.First();
                    IAgencySchemeMutableObject ascm = asc.MutableInstance;

                    //ascm.Id = newArtIdentity.ID;
                    ascm.AgencyId = newArtIdentity.Agency;
                    //ascm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddAgencyScheme(ascm.ImmutableInstance);
                    break;
                case "DATAPROVIDERSCHEME":
                    IDataProviderScheme dp = sdmxObjects.DataProviderSchemes.First();
                    IDataProviderSchemeMutableObject dpm = dp.MutableInstance;

                    //dpm.Id = newArtIdentity.ID;
                    dpm.AgencyId = newArtIdentity.Agency;
                    //dpm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddDataProviderScheme(dpm.ImmutableInstance);
                    break;
                case "DATACONSUMERSCHEME":
                    IDataConsumerScheme dcs = sdmxObjects.DataConsumerSchemes.First();
                    IDataConsumerSchemeMutableObject dcsm = dcs.MutableInstance;

                    //dcsm.Id = newArtIdentity.ID;
                    dcsm.AgencyId = newArtIdentity.Agency;
                    //dcsm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddDataConsumerScheme(dcsm.ImmutableInstance);
                    break;
                case "ORGANIZATIONUNITSCHEME":
                    IOrganisationUnitSchemeObject ous = sdmxObjects.OrganisationUnitSchemes.First();
                    IOrganisationUnitSchemeMutableObject ousm = ous.MutableInstance;

                    //ousm.Id = newArtIdentity.ID;
                    ousm.AgencyId = newArtIdentity.Agency;
                    //ousm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddOrganisationUnitScheme(ousm.ImmutableInstance);
                    break;
                case "CONTENTCONSTRAINT":
                    IContentConstraintObject cc = sdmxObjects.ContentConstraintObjects.First();
                    IContentConstraintMutableObject ccm = cc.MutableInstance;

                    ccm.Id = newArtIdentity.ID;
                    ccm.AgencyId = newArtIdentity.Agency;
                    ccm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddContentConstraintObject(ccm.ImmutableInstance);
                    break;
                case "STRUCTURESET":
                    IStructureSetObject ss = sdmxObjects.StructureSets.First();
                    IStructureSetMutableObject ssm = ss.MutableInstance;

                    ssm.Id = newArtIdentity.ID;
                    ssm.AgencyId = newArtIdentity.Agency;
                    ssm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddStructureSet(ssm.ImmutableInstance);
                    break;
                case "HCL":
                    IHierarchicalCodelistObject hcl = sdmxObjects.HierarchicalCodelists.First();
                    IHierarchicalCodelistMutableObject hclm = hcl.MutableInstance;

                    hclm.Id = newArtIdentity.ID;
                    hclm.AgencyId = newArtIdentity.Agency;
                    hclm.Version = newArtIdentity.Version;

                    sdmxObjectsRet.AddHierarchicalCodelist(hclm.ImmutableInstance);
                    break;
                default:
                    return null;
            }

            return sdmxObjectsRet;
        }

        private ISdmxObjects GetSdmxObjects(ArtefactIdentity artIdentity, string artefactType, bool exportWithRef)
        {
            ISdmxObjects sdmxObjects = null;
            WSModel dal = new WSModel();

            switch (artefactType.ToUpper())
            {
                case "CODELIST":
                    sdmxObjects = dal.GetCodeList(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "CONCEPTSCHEME":
                    sdmxObjects = dal.GetConceptScheme(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "CATEGORYSCHEME":
                    sdmxObjects = dal.GetCategoryScheme(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "DATAFLOW":
                    sdmxObjects = dal.GetDataFlow(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "KEYFAMILY":
                    //in caso di esportazione di una KeyFamily dobbiamo controllare se gli eventuali artefatti referenziati sono presenti già nel db di destinazione e non sono final
                    //quindi recuperiamo la DSD comprensiva di riferimenti
                    //if (exportWithRef)
                        sdmxObjects = dal.GetDataStructureWithRef(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    //else
                        //sdmxObjects = dal.GetDataStructure(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "CATEGORIZATION":
                    sdmxObjects = dal.GetCategorisation(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "AGENCYSCHEME":
                    sdmxObjects = dal.GetAgencyScheme(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "DATAPROVIDERSCHEME":
                    sdmxObjects = dal.GetDataProviderScheme(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "DATACONSUMERSCHEME":
                    sdmxObjects = dal.GetDataConsumerScheme(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "ORGANIZATIONUNITSCHEME":
                    sdmxObjects = dal.GetOrganisationUnitScheme(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "CONTENTCONSTRAINT":
                    sdmxObjects = dal.GetContentConstraint(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "STRUCTURESET":
                    sdmxObjects = dal.GetStructureSet(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;
                case "HCL":
                    sdmxObjects = dal.GetHcl(new ISTAT.Entity.ArtefactIdentity(artIdentity.ID, artIdentity.Agency, artIdentity.Version), false, false);
                    break;

                default:
                    return null;
            }
            return sdmxObjects;
        }

        //private void GetAg()
        //{

        //    EndPointElement epe = (EndPointElement)Session[SESSION_KEYS.CURRENT_ENDPOINT_OBJECT];

        //    User currentUser;

        //    if (epe.GetUsersFromFile)
        //    {
        //        currentUser = GetUserFromFile(myUserName, myPassword);
        //    }
        //    else
        //    {
        //        WSClient wsClient = new WSClient(epe.IREndPoint);
        //        IRService authClient = wsClient.GetClient();
        //        currentUser = authClient.GetUserByCredentials(myUserName, myPassword);
        //    }

        //}


        private User AutenticateUser(string userName, string password, EndPointElement epe)
        {
            if (epe.GetUsersFromFile)
            {
                return GetUserFromFile(userName, password, epe);
            }
            else
            {
                WSClient wsClient = new WSClient(epe.IREndPoint);
                IRService authClient = wsClient.GetClient();

                return authClient.GetUserByCredentials(userName, password);

                //if (authClient.GetUserByCredentials(userName, password) != null)
                //    return true;
            }

            //return false;
        }

        private User GetUserFromFile(string myUserName, string myPassword, EndPointElement epe)
        {
            User currentUser = new User();

            string filePath = epe.UsersFilePath;

            XElement xelement = XElement.Load(Server.MapPath("~/" + filePath));
            IEnumerable<XElement> users = xelement.Elements();

            var user = from u in xelement.Elements("user")
                       where (string)u.Attribute("username") == myUserName
                            && (string)u.Attribute("password") == myPassword
                       select u;

            if (user.Count() == 0)
                return null;

            var us = user.First();

            currentUser.username = us.Attribute("username").Value;
            currentUser.name = us.Attribute("name").Value;
            currentUser.surname = us.Attribute("surname").Value;

            var agencies = from ags in us.Descendants("agency")
                           select ags;

            UserAgency[] userAgencies = new UserAgency[agencies.Count()];
            int count = 0;

            foreach (string agencyId in agencies.Attributes("id"))
            {
                userAgencies[count] = new UserAgency() { id = agencyId, lang = "en" };
                ++count;
            }

            currentUser.agencies = userAgencies;

            return currentUser;
        }



        //private bool GetUserFromFile(string myUserName, string myPassword, EndPointElement epe)
        //{
        //    XElement xelement = XElement.Load(Server.MapPath(@"~\" + epe.UsersFilePath));
        //    IEnumerable<XElement> users = xelement.Elements();

        //    var user = from u in xelement.Elements("user")
        //               where (string)u.Attribute("username") == myUserName
        //                    && (string)u.Attribute("password") == myPassword
        //               select u;

        //    if (user.Count() == 0)
        //        return false;

        //    return true;
        //}

        #endregion

    }
}
