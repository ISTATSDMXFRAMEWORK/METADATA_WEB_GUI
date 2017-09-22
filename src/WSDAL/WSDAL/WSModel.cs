using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using System.Xml;
using Org.Sdmxsource.Util.Io;
using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
using Org.Sdmxsource.Sdmx.Api.Model;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using FlyCallWS;
using System.IO;
using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using ISTATUtils;

namespace ISTAT.WSDAL
{
    public class WSModel
    {
        //TODO: implementare la gestione degli errori
        #region "Constructors"

        public WSModel() { }

        #endregion

        #region "Public Methods"

        public ISdmxObjects GetAgencyScheme(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetAgencyScheme, stub, withLike);
        }

        public ISdmxObjects GetAllAgencyScheme(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetAgencyScheme, stub, false);
        }

        public ISdmxObjects GetCategorisation(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetCategorisation, stub, withLike);
        }

        public ISdmxObjects GetAllCategorisation(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetCategorisation, stub, false);
        }

        public ISdmxObjects GetCategoryScheme(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetCategoryScheme, stub, withLike);
        }

        public ISdmxObjects GetAllCategoryScheme(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetCategoryScheme, stub, false);
        }

        public ISdmxObjects GetCategorySchemeWithParents(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetCategorySchemeWithParents, stub, withLike);
        }

        public ISdmxObjects GetCodeList(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetCodelist, stub, withLike);
        }

        public ISdmxObjects GetAllCodeLists(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetCodelist, stub, false);
        }

        public ISdmxObjects GetConceptScheme(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetConceptScheme, stub, withLike);
        }

        public ISdmxObjects GetContentConstraint(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetContentConstraint, stub, withLike);
        }

        public ISdmxObjects GetAllContentConstraint(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetContentConstraint, stub, false);
        }

        public ISdmxObjects GetAllConceptScheme(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetConceptScheme, stub, false);
        }

        public ISdmxObjects GetDataConsumerScheme(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetDataConsumerScheme, stub, withLike);
        }

        public ISdmxObjects GetAllDataConsumerScheme(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetDataConsumerScheme, stub, false);
        }

        public ISdmxObjects GetDataFlow(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetDataflow, stub, withLike);
        }

        public ISdmxObjects GetAllDataFlow(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetDataflow, stub, false);
        }

        public ISdmxObjects GetDataProviderScheme(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetDataProviderScheme, stub, withLike);
        }

        public ISdmxObjects GetAllDataProviderScheme(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetDataProviderScheme, stub, false);
        }

        public ISdmxObjects GetDataStructure(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetDataStructure, stub, withLike);
        }

        public ISdmxObjects GetDataStructureWithRef(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetDataStructureWithRef, stub, withLike);
        }

        public ISdmxObjects GetAllDataStructure(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetDataStructure, stub, false);
        }

        public ISdmxObjects GetHcl(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetHierarchicalCodelist, stub, withLike);
        }

        public ISdmxObjects GetAllHcl(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetHierarchicalCodelist, stub, false);
        }

        public ISdmxObjects GetOrganisationUnitScheme(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetOrganisationUnitScheme, stub, withLike);
        }

        public ISdmxObjects GetAllOrganisationUnitScheme(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetOrganisationUnitScheme, stub, false);
        }

        public ISdmxObjects GetStructureSet(ArtefactIdentity artIdentity, bool stub, bool withLike)
        {
            return WSExecute(artIdentity, WSConstants.wsOperation.GetStructureSet, stub, withLike);
        }

        public ISdmxObjects GetAllStructureSet(bool stub)
        {
            return WSExecute(null, WSConstants.wsOperation.GetStructureSet, stub, false);
        }

        public XmlDocument SubmitStructure(XmlDocument xDom)
        {
            return WSSubmitStructure(xDom);
        }

        public XmlDocument SubmitStructure(ISdmxObjects sdmxObjects)
        {
            WSUtils wsUtils = new WSUtils();

            if (FinalArtefactExists(sdmxObjects))
                return CreateXMLError("The artefact exists and is final!");

            return WSSubmitStructure(wsUtils.GetXmlMessage(sdmxObjects));
        }

        public XmlDocument SubmitStructureWS(ISdmxObjects sdmxObjects)
        {
            WSUtils wsUtils = new WSUtils();

            //if (FinalArtefactExists(sdmxObjects))
            //    return CreateXMLError("The artefact exists and is final!");

            return WSSubmitStructure(wsUtils.GetXmlMessage(sdmxObjects));
        }
        #endregion

        #region "Private Methods"

        private XmlDocument CreateXMLError(string error)
        {
            string xmlString;
            xmlString = String.Format("<xml>" +
                                        "    <StatusMessage status=\"Failure\" >" +
                                        "        <err>{0}</err>" +
                                        "    </StatusMessage>     " +
                                        "</xml>", error);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            return doc;
        }

        private bool FinalArtefactExists(ISdmxObjects sdmxObjects)
        {
            ISdmxObjects sdmxContainer;

            try
            {
                if (sdmxObjects.HasCodelists)
                {
                    Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject cl = sdmxObjects.Codelists.FirstOrDefault();
                    sdmxContainer = GetCodeList(new ArtefactIdentity(cl.Id, cl.AgencyId, cl.Version), true, false);
                    if (sdmxContainer.Codelists != null)
                        return sdmxContainer.Codelists.First().IsFinal.IsTrue;
                }
                if (sdmxObjects.HasConceptSchemes)
                {
                    Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme.IConceptSchemeObject cs = sdmxObjects.ConceptSchemes.FirstOrDefault();
                    sdmxContainer = GetConceptScheme(new ArtefactIdentity(cs.Id, cs.AgencyId, cs.Version), true, false);
                    if (sdmxContainer.ConceptSchemes != null)
                        return sdmxContainer.ConceptSchemes.First().IsFinal.IsTrue;
                }
                if (sdmxObjects.HasDataStructures)
                {
                    Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IDataStructureObject dsd = sdmxObjects.DataStructures.FirstOrDefault();
                    sdmxContainer = GetDataStructure(new ArtefactIdentity(dsd.Id, dsd.AgencyId, dsd.Version), true, false);
                    if (sdmxContainer.DataStructures != null)
                        return sdmxContainer.DataStructures.First().IsFinal.IsTrue;
                }
                if (sdmxObjects.HasCategorySchemes)
                {
                    Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme.ICategorySchemeObject cso = sdmxObjects.CategorySchemes.FirstOrDefault();
                    sdmxContainer = GetCategoryScheme(new ArtefactIdentity(cso.Id, cso.AgencyId, cso.Version), true, false);
                    if (sdmxContainer.CategorySchemes != null)
                        return sdmxContainer.CategorySchemes.First().IsFinal.IsTrue;
                }
                if (sdmxObjects.HasDataflows)
                {
                    Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IDataflowObject df = sdmxObjects.Dataflows.FirstOrDefault();
                    sdmxContainer = GetDataFlow(new ArtefactIdentity(df.Id, df.AgencyId, df.Version), true, false);
                    if (sdmxContainer.Dataflows != null)
                        return sdmxContainer.Dataflows.First().IsFinal.IsTrue;
                }
                if (sdmxObjects.HasCategorisations)
                {
                    Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme.ICategorisationObject cat = sdmxObjects.Categorisations.FirstOrDefault();
                    sdmxContainer = GetCategorisation(new ArtefactIdentity(cat.Id, cat.AgencyId, cat.Version), true, false);
                    if (sdmxContainer.Categorisations != null)
                        return sdmxContainer.Categorisations.First().IsFinal.IsTrue;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }


        private XmlDocument WSSubmitStructure(XmlDocument xDom)
        {

            XmlDocument xDomOutput = new XmlDocument();

            // Quirini Fix (DataType Double)

            FixXmlDoc FixXml = new FixXmlDoc();

            xDom = FixXml.FixDataType(xDom);

            // ----------------------------

            try
            {
                WSUtils wsUtils = new WSUtils();

                WsConfigurationSettings wsSettings = wsUtils.GetSettings(WSConstants.wsOperation.SubmitStructure.ToString());

                string OutputFilePath = HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + HttpContext.Current.Session.SessionID + ".xml");

                FlyCallWS.Streaming.CallWS objWS = new FlyCallWS.Streaming.CallWS(OutputFilePath, WSConstants.MaxOutputFileLength);

                xDomOutput.InnerXml = objWS.SendSOAPQuery(xDom, wsSettings);

                File.Delete(OutputFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return xDomOutput;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="operation"></param>
        private ISdmxObjects WSExecute(ArtefactIdentity artIdentity, WSConstants.wsOperation operation, bool stub, bool withLike)
        {
            XmlDocument xDomOutput = new XmlDocument();

            ISdmxObjects SdmxObject, FilteredSdmxObject;

            FilteredSdmxObject = new SdmxObjectsImpl();

            try
            {
                var ret = CallWebService(operation, withLike, artIdentity, stub);

                if (ret == null || IsNotResult(ret))
                    throw new Exception("no results found");

                xDomOutput.InnerXml = ret;

                // Quirini Fix ----------------

                FixXmlDoc FixXml = new FixXmlDoc();

                xDomOutput = FixXml.FixDataType(xDomOutput);
                xDomOutput = FixXml.FixLocale(xDomOutput, "la", "lo");

                // ----------------------------

                SdmxObject = LoadSDMXObject(xDomOutput);

                if (withLike && !IsNullOrEmptyArtefactIdentity(artIdentity))
                    switch (operation)
                    {
                        case WSConstants.wsOperation.GetAgencyScheme:
                            var filteredAgency = SdmxObject.AgenciesSchemes.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAgencyScheme ag in filteredAgency)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && ag.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddAgencyScheme(ag);
                            }
                            break;
                        case WSConstants.wsOperation.GetCategorisation:
                            var filteredCat = SdmxObject.Categorisations.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme.ICategorisationObject cat in filteredCat)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && cat.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddCategorisation(cat);
                            }
                            break;
                        case WSConstants.wsOperation.GetCategoryScheme:
                            var filteredCS = SdmxObject.CategorySchemes.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme.ICategorySchemeObject cat in filteredCS)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && cat.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddCategoryScheme(cat);
                            }
                            break;
                        case WSConstants.wsOperation.GetCodelist:
                            var filteredCL = SdmxObject.Codelists.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.ICodelistObject cl in filteredCL)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && cl.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddCodelist(cl);
                            }
                            break;
                        case WSConstants.wsOperation.GetConceptScheme:
                            var filteredCoS = SdmxObject.ConceptSchemes.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme.IConceptSchemeObject x in filteredCoS)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddConceptScheme(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetDataflow:
                            var filteredDF = SdmxObject.Dataflows.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IDataflowObject x in filteredDF)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddDataflow(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetStructureSet:
                            var filteredSS = SdmxObject.StructureSets.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping.IStructureSetObject x in filteredSS)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddStructureSet(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetContentConstraint:
                            var filteredCC = SdmxObject.ContentConstraintObjects.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry.IContentConstraintObject x in filteredCC)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddContentConstraintObject(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetOrganisationUnitScheme:
                            var filteredOU = SdmxObject.OrganisationUnitSchemes.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IOrganisationUnitSchemeObject x in filteredOU)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddOrganisationUnitScheme(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetDataConsumerScheme:
                            var filteredDC = SdmxObject.DataConsumerSchemes.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IDataConsumerScheme x in filteredDC)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddDataConsumerScheme(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetDataProviderScheme:
                            var filteredDP = SdmxObject.DataProviderSchemes.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IDataProviderScheme x in filteredDP)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddDataProviderScheme(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetDataStructureWithRef:
                        case WSConstants.wsOperation.GetDataStructure:
                            var filteredDSD = SdmxObject.DataStructures.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure.IDataStructureObject x in filteredDSD)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddDataStructure(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetHierarchicalCodelist:
                            var filteredHcl = SdmxObject.HierarchicalCodelists.Where(i => i.Id.ToUpper().Contains(artIdentity.ID.ToUpper()) && i.AgencyId.ToUpper().Contains(artIdentity.Agency.ToUpper()) && i.Version.ToUpper().Contains(artIdentity.Version.ToUpper()));
                            foreach (Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist.IHierarchicalCodelistObject x in filteredHcl)
                            {
                                if ((artIdentity.IsFinal == null || !(bool)artIdentity.IsFinal) || (bool)artIdentity.IsFinal && x.IsFinal.IsTrue)
                                    FilteredSdmxObject.AddHierarchicalCodelist(x);
                            }
                            break;
                        case WSConstants.wsOperation.GetStructures:
                            break;
                        default:
                            FilteredSdmxObject = SdmxObject;
                            break;
                    }
                else
                    FilteredSdmxObject = SdmxObject;

                return FilteredSdmxObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private bool IsNotResult(string message)
        {
            return message.IndexOf("Could not find requested structures") >= 0 || message.IndexOf("ErrorMessage code=\"100\"") >= 0;
        }

        private string CallWebService(WSConstants.wsOperation operation, bool withLike, ArtefactIdentity artIdentity, bool stub)
        {
            EndPointElement epe = (EndPointElement)HttpContext.Current.Session["WSEndPoint"];

            if (epe.ActiveEndPointType == ActiveEndPointType.SOAP)
                return CallSOAPWebService(operation,withLike,artIdentity,stub);
            else
                return CallRESTWebService(operation, withLike, artIdentity, stub);
        }

        private string CallRESTWebService(WSConstants.wsOperation operation, bool withLike, ArtefactIdentity artIdentity, bool stub)
        {

            WsConfigurationSettings wsSettings = null;
            WSUtils wsUtils = new WSUtils();
            wsSettings = wsUtils.GetSettings("");

            // File di appoggio per la creazione del xDom
            string OutputFilePath = HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + HttpContext.Current.Session.SessionID + ".xml");

            string restquery = ComposeRestQuery(operation, withLike, artIdentity, stub);

            FlyCallWS.Streaming.CallWS objWS = new FlyCallWS.Streaming.CallWS(OutputFilePath, WSConstants.MaxOutputFileLength);

            File.Delete(OutputFilePath);

            return objWS.SendRESTQuery(restquery, @"application/vnd.sdmx.structure+xml;version=2.1", wsSettings);
        }

        private string ComposeRestQuery(WSConstants.wsOperation operation, bool withLike, ArtefactIdentity artIdentity, bool stub)
        {
            string QueryRest = "";

            switch (operation)
            {
                case WSConstants.wsOperation.GetAgencyScheme:
                    QueryRest += "agencyscheme";
                    break;
                case WSConstants.wsOperation.GetCategorisation:
                    QueryRest += "categorisation";
                    break;
                case WSConstants.wsOperation.GetCategorySchemeWithParents:
                case WSConstants.wsOperation.GetCategoryScheme:
                    QueryRest += "categoryScheme";
                    break;
                case WSConstants.wsOperation.GetCodelist:
                    QueryRest += "codelist";
                    break;
                case WSConstants.wsOperation.GetConceptScheme:
                    QueryRest += "conceptscheme";
                    break;
                case WSConstants.wsOperation.GetContentConstraint:
                    QueryRest += "contentconstraint";
                    break;
                case WSConstants.wsOperation.GetDataConsumerScheme:
                    QueryRest += "dataconsumerscheme";
                    break;
                case WSConstants.wsOperation.GetDataflow:
                    QueryRest += "dataflow";
                    break;
                case WSConstants.wsOperation.GetDataProviderScheme:
                    QueryRest += "dataproviderscheme";
                    break;
                case WSConstants.wsOperation.GetDataStructureWithRef:
                case WSConstants.wsOperation.GetDataStructure:
                    QueryRest += "datastructure";
                    break;
                case WSConstants.wsOperation.GetHierarchicalCodelist:
                    QueryRest += "hierarchicalcodelist";
                    break;
                case WSConstants.wsOperation.GetOrganisationUnitScheme:
                    QueryRest += "organisationunitscheme";
                    break;
                case WSConstants.wsOperation.GetStructureSet:
                    QueryRest += "structureset";
                    break;
                case WSConstants.wsOperation.GetStructures:
                    QueryRest += "xxx";
                    break;
                case WSConstants.wsOperation.SubmitStructure:
                    QueryRest += "xxx";
                    break;
                default:
                    break;
            }

            //@"codelist/ESTAT/CL_COUNTRY/"

            // Imposto ID,Agency e Version
            if (!withLike && artIdentity != null)
            {
                QueryRest += @"/" + (artIdentity.Agency != string.Empty ? artIdentity.Agency : "all");
                QueryRest += @"/" + (artIdentity.ID != string.Empty ? artIdentity.ID : "all");
                //QueryRest += @"/" + (artIdentity.Version != string.Empty ? artIdentity.Version : "LATEST");
            }
            else
                QueryRest += @"/all/all";

            if (operation == WSConstants.wsOperation.GetDataStructureWithRef)
                QueryRest += @"/?detail=full&references=children";
            else if(operation == WSConstants.wsOperation.GetCategorySchemeWithParents)
                QueryRest += @"/?detail=full&references=parents";
            else
            {
                if (stub)
                    QueryRest += @"/?detail=allstubs";
                else
                    QueryRest += @"/?detail=full";
            }

            return QueryRest;
        }

        private string CallSOAPWebService(WSConstants.wsOperation operation, bool withLike, ArtefactIdentity artIdentity, bool stub)
        {
            XmlDocument xDom = new XmlDocument();
            WSUtils wsUtils = new WSUtils();
            WsConfigurationSettings wsSettings = null;

            // File di appoggio per la creazione del xDom
            string OutputFilePath = HttpContext.Current.Server.MapPath(@"~\OutputFiles" + "\\" + HttpContext.Current.Session.SessionID + ".xml");

            // Carico il template
            xDom.Load(getTemplate(operation));

            if (operation == WSConstants.wsOperation.GetDataStructureWithRef)
            {
                wsSettings = wsUtils.GetSettings(WSConstants.wsOperation.GetDataStructure.ToString());
            }
            else if (operation == WSConstants.wsOperation.GetCategorySchemeWithParents)
            {
                wsSettings = wsUtils.GetSettings(WSConstants.wsOperation.GetCategoryScheme.ToString());
            }
            else
            {
                wsSettings = wsUtils.GetSettings(FindException(operation));
            }

            // Imposto ID,Agency e Version
            if (!withLike && artIdentity != null)
                SetKey(ref xDom, artIdentity);
            else
                RemoveFilter(xDom);

            if (stub)
                SetStub(ref xDom);

            FlyCallWS.Streaming.CallWS objWS = new FlyCallWS.Streaming.CallWS(OutputFilePath, WSConstants.MaxOutputFileLength);

            File.Delete(OutputFilePath);

            return objWS.SendSOAPQuery(xDom, wsSettings);
        }


        private string FindException(WSConstants.wsOperation operation)
        {
            string sOperation = string.Empty;

            switch (operation)
            {
                case WSConstants.wsOperation.GetAgencyScheme:
                case WSConstants.wsOperation.GetDataConsumerScheme:
                case WSConstants.wsOperation.GetDataProviderScheme:
                case WSConstants.wsOperation.GetOrganisationUnitScheme:
                    sOperation = "GetOrganisationScheme";
                    break;
                case WSConstants.wsOperation.GetContentConstraint:
                    sOperation = "GetConstraint";
                    break;
                default:
                    return operation.ToString();
            }
            return sOperation;
        }

        private void SetStub(ref XmlDocument xDom)
        {
            XmlNode xNodeID = xDom.SelectSingleNode("//query:ReturnDetails", GetNamespaceManager(xDom));
            xNodeID.Attributes["detail"].Value = "Stub";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDom"></param>
        private void RemoveFilter(XmlDocument xDom)
        {
            XmlNode xNodeID = xDom.SelectSingleNode("//query:ID", GetNamespaceManager(xDom));

            for (int i = xNodeID.ParentNode.ChildNodes.Count - 1; i >= 0; i--)
                xNodeID.ParentNode.RemoveChild(xNodeID.ParentNode.ChildNodes[i]);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private XmlNamespaceManager GetNamespaceManager(XmlDocument xDom)
        {
            XmlNamespaceManager xManag = new XmlNamespaceManager(xDom.NameTable);
            xManag.AddNamespace("query", @"http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query");

            return xManag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private string getTemplate(WSConstants.wsOperation operation)
        {
            string fileName;

            switch (operation)
            {
                case WSConstants.wsOperation.GetCategorySchemeWithParents:
                    fileName = "CategorySchemeWithParents.xml";
                    break;
                default:
                    fileName = operation.ToString().Replace("Get", "") + ".xml";
                    break;
            }

            return HttpContext.Current.Server.MapPath(@"~\SdmxQueryTemplate") + "\\" + fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDom"></param>
        /// <param name="operation"></param>
        private void SetKey(ref XmlDocument xDom, ArtefactIdentity artIdentity)
        {
            //string xPathQuery = "//*[local-name()='" + WSConstants.DictWhere[operation] + "']";

            //XmlNode xTempStructNode = xDom.SelectSingleNode(xPathQuery);
            XmlNamespaceManager xManag = GetNamespaceManager(xDom);

            XmlNode xNodeID = xDom.SelectSingleNode("//query:ID", xManag);
            XmlNode xNodeAgency = xDom.SelectSingleNode("//query:AgencyID", xManag);
            XmlNode xNodeVersion = xDom.SelectSingleNode("//query:Version", xManag);

            if (!String.IsNullOrEmpty(artIdentity.ID.Trim()))
                xNodeID.InnerText = artIdentity.ID;
            else
                xNodeID.ParentNode.RemoveChild(xNodeID);

            if (!String.IsNullOrEmpty(artIdentity.Agency.Trim()))
                xNodeAgency.InnerText = artIdentity.Agency;
            else
                xNodeAgency.ParentNode.RemoveChild(xNodeAgency);

            if (!String.IsNullOrEmpty(artIdentity.Version.Trim()))
                xNodeVersion.InnerText = artIdentity.Version;
            else
                xNodeVersion.ParentNode.RemoveChild(xNodeVersion);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDomSource"></param>
        /// <returns></returns>
        private ISdmxObjects LoadSDMXObject(XmlDocument xDomSource)
        {
            // Il documento template che verrà caricato con gli artefatti da importare
            XmlDocument xDomTemp = new XmlDocument();

            // Creo gli elementi del file template
            xDomTemp.InnerXml = WSConstants.xmlTemplate;

            // Il nodo root "Structure" del template
            XmlNode xTempStructNode = xDomTemp.SelectSingleNode("//*[local-name()='Structure']");

            // Creo il nodo "Structures" che conterrà gli artefatti
            XmlNode xSourceStructNode = xDomTemp.CreateNode(XmlNodeType.Element, "Structures", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message");

            // Inserisco nel nodo "Structures" gli aertefatti presenti nell' sdmx passato in input
            xSourceStructNode.InnerXml = xDomSource.SelectSingleNode("//*[local-name()='Structures']").InnerXml;

            // Aggiungo al template l'elemento "Structures" con gli artefatti da caricare
            xTempStructNode.AppendChild(xSourceStructNode);

            // Converto il documento in un MemoryReadableLocation
            MemoryReadableLocation mRL = new MemoryReadableLocation(WSUtils.ConvertToBytes(xDomTemp));

            IStructureParsingManager _parsingManager = new StructureParsingManager();

            // Parse structures IStructureParsingManager is an instance field.
            IStructureWorkspace structureWorkspace = _parsingManager.ParseStructures(mRL);

            // Get immutable objects from workspace
            return structureWorkspace.GetStructureObjects(false);
        }


        private ISdmxObjects LoadSDMXRet(XmlDocument xDomSource)
        {
            // Converto il documento in un MemoryReadableLocation
            MemoryReadableLocation mRL = new MemoryReadableLocation(WSUtils.ConvertToBytes(xDomSource));

            IStructureParsingManager _parsingManager = new StructureParsingManager();

            // Parse structures IStructureParsingManager is an instance field.
            IStructureWorkspace structureWorkspace = _parsingManager.ParseStructures(mRL);

            // Get immutable objects from workspace
            return structureWorkspace.GetStructureObjects(false);
        }

        private bool IsNullOrEmptyArtefactIdentity(ArtefactIdentity aIObject)
        {
            if (aIObject == null)
                return true;

            if (String.IsNullOrEmpty(aIObject.ID) && String.IsNullOrEmpty(aIObject.Agency) && String.IsNullOrEmpty(aIObject.Version) && aIObject.IsFinal == null)
                return true;

            return false;
        }

        #endregion

    }
}