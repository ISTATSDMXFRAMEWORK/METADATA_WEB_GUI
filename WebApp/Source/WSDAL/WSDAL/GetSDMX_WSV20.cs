using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Header;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using System.Linq;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Manager.Query;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Factory;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Manager;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Model;
using System.Xml.Linq;
using Estat.Sri.CustomRequests.Manager;
using Estat.Sri.CustomRequests.Model;
using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
using Org.Sdmxsource.Sdmx.Api.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Xml;
using Org.Sdmxsource.Util.Io;
using ISTAT.WebClient.WidgetComplements.Model.Enum;
using ISTAT.WebClient.WidgetComplements.Model.Exceptions;
using ISTAT.WebClient.WidgetComplements.Model.Settings;

namespace ISTAT.WSDAL
{
    public class GetSDMX_WSV20
    {
        #region Constants and Fields

        /// <summary>
        /// Empty list of <see cref="CategorySchemeBean"/>
        /// </summary>
        private static readonly IList<ICategorySchemeObject> _emptyCategoryScheme = new ReadOnlyCollection<ICategorySchemeObject>(new List<ICategorySchemeObject>());
        /// <summary>
        /// Empty list of <see cref="DataflowBean"/>
        /// </summary>
        private static readonly ISet<IDataflowObject> _emptyDataflowList = new HashSet<IDataflowObject>();

        /// <summary>
        ///  The Web Service Client configuration
        /// </summary>
        private readonly EndpointSettings _config;

        /// <summary>
        /// Default Header
        /// </summary>
        private readonly IHeader _defaultHeader;

        /// <summary>
        /// The configuration retrieved from the WSDL
        /// </summary>
        private readonly WSDLSettings _wsdlConfig;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSDMX_WSV20"/> class. 
        /// </summary>
        /// <param name="config">
        /// NSIClient settings
        /// </param>
        public GetSDMX_WSV20(EndpointSettings config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(config.EndPoint))
            {
                //throw new ArgumentException(Resources.ExceptionEndpointNotSet, "config");
            }

            this._config = config;

            try
            {
                this._wsdlConfig = new WSDLSettings(config);
            }
            catch (WebException ex)
            {
                //throw NsiClientHelper.HandleWsdlException(config.EndPoint, ex, config.Wsdl);
            }
            catch (InvalidOperationException ex)
            {
                //throw NsiClientHelper.HandleWsdlException(config.EndPoint, ex, config.Wsdl);
            }
            catch (UriFormatException ex)
            {
                //throw NsiClientHelper.HandleWsdlException(config.EndPoint, ex, config.Wsdl);
            }

            this._defaultHeader = new HeaderImpl("NSIClient", "NSIClient");
            Utils.PopulateHeaderFromSettings(this._defaultHeader);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute SDMX Query against the NSI WS to retrieve SDMX-ML Data as a Stream
        /// </summary>
        /// <param name="query">
        /// The SDMX Query to execute
        /// </param>
        /// <param name="operationName">
        /// The type of operation, GetCompactData or GetCrossSectionalData
        /// </param>
        /// <exception cref="NsiClientException">Failute to execute query</exception>
        /// <returns>
        /// The SDMX-ML data as a stream.
        /// </returns>
        public void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, string tempFileName)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            try
            {
                this.SendSdmxQuery(query, operationName, tempFileName);
            }

            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionExecuteQuery);
                NsiClientHelper.TryToDelete(tempFileName);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (DataflowException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionExecuteQuery);
                NsiClientHelper.TryToDelete(tempFileName);
                throw new NsiClientException(Resources.ExceptionExecuteQuery, e);
            }

        }

        /// <summary>
        /// Execute SDMX Query against the NSI WS to retrieve SDMX-ML Data as a Stream
        /// </summary>
        /// <param name="query">
        /// The SDMX Query to execute
        /// </param>
        /// <param name="operationName">
        /// The type of operation, GetCompactData or GetCrossSectionalData
        /// </param>
        /// <param name="observationLimit">
        /// The maximum number of observations to return
        /// </param>
        /// <returns>
        /// The SDMX-ML data as a stream.
        /// </returns>
        public void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, int observationLimit, string tempFileName)
        {
            query = new DataQueryFluentBuilder().Initialize(query.DataStructure, query.Dataflow)
                .WithOrderAsc(true)
                .WithDataQuerySelectionGroup(query.SelectionGroups).WithMaxObservations(observationLimit).
                WithDataProviders(query.DataProvider).WithDataQueryDetail(query.DataQueryDetail).
                WithDimensionAtObservation(query.DimensionAtObservation).Build();

            this.ExecuteQuery(query, operationName, tempFileName);
        }

        /// <summary>
        /// Gets a bean with data about the codelist for specified dataflow and component.
        /// The dataflow can be retrieved from <see cref="RetrieveTree"/> and the component from <see cref="GetStructure"/>
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <param name="component">
        /// The component
        /// </param>
        /// <param name="criteria">
        /// The criteria includes a set of Member and MemberValue(s) for each dimension. The Member has componentRef the dimension conceptRef and the MemberValue(s) specify the selected codes for this dimension.
        /// </param>
        /// <returns>
        /// A <c>CodeListBean</c> with the requested data
        /// </returns>
        public ICodelistObject GetCodelist(IDataflowObject dataflow, IDataStructureObject dsd, IComponent component, IContentConstraintMutableObject criteria, bool Constrained)
        {
            var codelistRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList));
            var dimension = component as IDimension;
            if (dimension != null && dimension.TimeDimension)
            {
                codelistRef.MaintainableId = CustomCodelistConstants.TimePeriodCodeList;
                codelistRef.AgencyId = CustomCodelistConstants.Agency;
                codelistRef.Version = CustomCodelistConstants.Version;
            }
            else if (dimension != null && dimension.MeasureDimension && dsd is ICrossSectionalDataStructureObject)
            {
                var crossDsd = dsd as ICrossSectionalDataStructureObject;
                codelistRef.MaintainableId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.MaintainableId;
                codelistRef.AgencyId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.AgencyId;
                codelistRef.Version = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.Version;
            }
            else
            {
                if (component.HasCodedRepresentation())
                {
                    codelistRef.MaintainableId = component.Representation.Representation.MaintainableReference.MaintainableId;
                    codelistRef.AgencyId = component.Representation.Representation.MaintainableReference.AgencyId;
                    codelistRef.Version = component.Representation.Representation.MaintainableReference.Version;
                }
            }

            string info = string.Format(
                CultureInfo.InvariantCulture,
                Resources.InfoPartialCodelistFormat3,
                Utils.MakeKey(dataflow),
                component.ConceptRef,
                Utils.MakeKey(codelistRef));
            return this.GetCodelist(dataflow, codelistRef, criteria, info, Constrained);
        }

        /// <summary>
        /// Get the maximum number of observations that can be retrieved given the specified criteria
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <param name="criteria">
        /// The criteria includes a set of Member and MemberValue(s) for each dimension. Each member should have member values else they shouldn't be included. It can be null
        /// </param>
        /// <returns>
        /// The maximum number of observations or -1 if it can't be parsed or it is not available
        /// </returns>
        /// <exception cref="NsiClientException">
        /// NSI WS communication error or parsing error
        /// </exception>
        public int GetDataflowDataCount(IDataflowObject dataflow, IContentConstraintMutableObject criteria)
        {
            int count;
            if (criteria == null)
            {
                criteria = new ContentConstraintMutableCore();
                criteria.Id = CustomCodelistConstants.CountCodeList;
                criteria.AddName("en", "name");
                criteria.AgencyId = "agency";
            }

            var codelistRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList))
            {
                MaintainableId = CustomCodelistConstants.CountCodeList,
                AgencyId = CustomCodelistConstants.Agency,
                Version = CustomCodelistConstants.Version
            };
            string info = string.Format(
                CultureInfo.InvariantCulture, Resources.InfoCountFormat2, Utils.MakeKey(dataflow), Utils.MakeKey(codelistRef));
            try
            {
                ICodelistObject countCodelist = this.GetCodelist(dataflow, codelistRef, criteria, info, true);
                if (!CustomCodelistConstants.IsCountCodeList(countCodelist)
                    || !int.TryParse(countCodelist.Items[0].Id, out count))
                {
                    Logger.WarnFormat(CultureInfo.InvariantCulture, Resources.ExceptionParsingCountCodelistFormat0, info);

                    // throw new NsiClientException("Error parsing the count codelist for " + info);
                    count = -1;
                }
            }
            catch (NsiClientException ex)
            {
                Logger.Warn(ex.Message, ex);
                count = -1;
            }

            return count;
        }

        /// <summary>
        /// Retrieves all available categorisations.
        /// </summary>
        /// <returns>
        ///   a list of &amp;lt;c&amp;gt;ISdmxObjects&amp;lt;/c&amp;gt; instances; the result won&amp;apos;t be &amp;lt;c&amp;gt;null&amp;lt;/c&amp;gt; if there are no
        ///   dataflows, instead an empty list will be returned
        /// </returns>
        public ISdmxObjects RetrieveTree()
        {

            Logger.Info(Resources.InfoGettingCategorySchemes);

            ISdmxObjects response = new SdmxObjectsImpl();

            //get dataflows
            var dataflowRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));
            //get category scheme
            var catSch = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CategoryScheme));

            IList<IStructureReference> refs = new List<IStructureReference>();
            refs.Add(catSch);
            refs.Add(dataflowRefBean);
            try
            {
                response = this.SendQueryStructureRequest(refs, false);

                if (response.CategorySchemes != null && response.Dataflows != null)
                {
                    Logger.Info(Resources.InfoSuccess);
                }
            }
            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionGettingDataflow);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (DataflowException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionGettingDataflow);
                Logger.Error(e.Message, e);
                throw new NsiClientException(Resources.ExceptionGettingDataflow, e);
            }
            if (response.Dataflows != null && response.Dataflows.Count == 0)
            {
                throw new DataflowException(Resources.NoResultsFound);
            }
            return response;
        }

        /// <summary>
        /// Gets a bean with data about the key family for specified dataflow.
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <returns>
        /// a <c>StructureBean</c> instance with requested data; the result is never <c>null</c> or  incomplete, instead an exception is throwed away if something goes wrong and not all required data is successfully retrieved
        /// </returns>
        /// <remarks>
        /// The resulted bean will contain exactly one key family, but also will include any concepts and codelists referenced by the key family.
        /// </remarks>
        public ISdmxObjects GetStructure(IDataflowObject dataflow, ISet<IDataStructureObject> dataStructures)
        {
            Logger.InfoFormat(
                    CultureInfo.InvariantCulture,
                    Resources.InfoGettingStructureFormat3,
                    dataflow.AgencyId,
                    dataflow.Id,
                    dataflow.Version);
            ISdmxObjects structure;

            var keyFamilyRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd))
            {
                MaintainableId = dataflow.DataStructureRef.MaintainableReference.MaintainableId,
                AgencyId = dataflow.DataStructureRef.MaintainableReference.AgencyId,
                Version = dataflow.DataStructureRef.MaintainableReference.Version
            };

            try
            {
                ISdmxObjects response;

                structure = this.SendQueryStructureRequest(keyFamilyRefBean, false);
                NsiClientValidation.CheckifStructureComplete(structure, dataflow);
                IEnumerable<IStructureReference> conceptRefs = NsiClientHelper.BuildConceptSchemeRequest(structure.DataStructures.First());
                response = this.SendQueryStructureRequest(conceptRefs, false);

                structure.Merge(response);

                NsiClientValidation.CheckConcepts(structure);
                Logger.Info(Resources.InfoSuccess);
            }
            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionGettingStructure);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionGettingStructure);
                Logger.Error(e.Message, e);
                throw new NsiClientException(Resources.ExceptionGettingStructure, e);
            }

            return structure;
        }


        public ISdmxObjects GetStructure(string DataflowId, string DatafloAgency, string DatafloVersion)
        {
            Logger.InfoFormat(
                    CultureInfo.InvariantCulture,
                    Resources.InfoGettingStructureFormat3,
                     DatafloAgency,
                    DataflowId,
                    DatafloVersion);

            #region Dataflow
            ISdmxObjects responseDF = new SdmxObjectsImpl();
            var dataflowRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow))
            {
                MaintainableId = DataflowId,
                AgencyId = DatafloAgency,
                Version = DatafloVersion
            };
            IList<IStructureReference> refs = new List<IStructureReference>();
            refs.Add(dataflowRefBean);
            responseDF = this.SendQueryStructureRequest(refs, false);
            if (responseDF.Dataflows == null || responseDF.Dataflows.Count == 0)
                throw new Exception("Dataflow not found");
            #endregion
            IDataflowObject dataflow = responseDF.Dataflows.First();


            ISdmxObjects structure;

            var keyFamilyRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd))
            {
                MaintainableId = dataflow.DataStructureRef.MaintainableReference.MaintainableId,
                AgencyId = dataflow.DataStructureRef.MaintainableReference.AgencyId,
                Version = dataflow.DataStructureRef.MaintainableReference.Version
            };

            try
            {
                ISdmxObjects response;

                structure = this.SendQueryStructureRequest(keyFamilyRefBean, false);
                NsiClientValidation.CheckifStructureComplete(structure, dataflow);
                IEnumerable<IStructureReference> conceptRefs = NsiClientHelper.BuildConceptSchemeRequest(structure.DataStructures.First());
                response = this.SendQueryStructureRequest(conceptRefs, false);

                structure.Merge(responseDF);
                structure.Merge(response);

                NsiClientValidation.CheckConcepts(structure);
                Logger.Info(Resources.InfoSuccess);
            }
            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionGettingStructure);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionGettingStructure);
                Logger.Error(e.Message, e);
                throw new NsiClientException(Resources.ExceptionGettingStructure, e);
            }

            return structure;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a bean with data about the codelist for specified dataflow and codelist ref.
        /// The dataflow can be retrieved from <see cref="RetrieveDataflow"/>
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <param name="codelistRef">
        /// The codelist reference
        /// </param>
        /// <param name="criteria">
        /// The criteria includes a set of Member and MemberValue(s) for each dimension.
        /// </param>
        /// <param name="info">
        /// Some helper information used for logging
        /// </param>
        /// <returns>
        /// The partial codelist.
        /// </returns>
        private ICodelistObject GetCodelist(
            IDataflowObject dataflow, IStructureReference codelistRef, IContentConstraintMutableObject criteria, string info, bool Constrained)
        {
            ICodelistObject codelist;

            var refs = new List<IStructureReference>();
            refs.Add(codelistRef);
            if (Constrained)
            {

                var dataflowRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow))
                {
                    MaintainableId = dataflow.Id,
                    AgencyId = dataflow.AgencyId,
                    Version = dataflow.Version,
                };

                var dataflowRefBean = new ConstrainableStructureReference(dataflowRef, criteria.ImmutableInstance);

                Logger.InfoFormat(CultureInfo.InvariantCulture, Resources.InfoGettingCodelistFormat1, info);
                refs.Add(dataflowRefBean);
            }

            try
            {
                ISdmxObjects response;

                response = this.SendQueryStructureRequest(refs, false);

                if (response.Codelists.Count != 1)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture, Resources.ExceptionInvalidNumberOfCodeListsFormat1, info);
                    Logger.Error(message);
                    throw new NsiClientException(message);
                }

                codelist = response.Codelists.First();
                if (codelist.Items.Count == 0)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture, Resources.ExceptionZeroCodesFormat1, info);
                    Logger.Error(message);
                    throw new NsiClientException(message);
                }
            }
            catch (NsiClientException e)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture, Resources.ExceptionGetCodelistFormat2, info, e.Message);
                Logger.Error(message);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (DataflowException e)
            {
                throw;
            }
            catch (Exception e)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture, Resources.ExceptionGetCodelistFormat2, info, e.Message);
                Logger.Error(message);
                Logger.Error(e.Message, e);
                throw new NsiClientException(message, e);
            }

            return codelist;
        }

        /// <summary>
        /// Sends the specified <paramref name="reference"/> to the Web Service defined by <see cref="_config"/> 
        /// </summary>
        /// <param name="reference">
        /// The reference
        /// </param>
        /// <param name="resolveReferences">
        /// The resolve references
        /// </param>
        public ISdmxObjects SendQueryStructureRequest(IStructureReference reference, bool resolveReferences)
        {
            IList<IStructureReference> refs = new List<IStructureReference>();
            refs.Add(reference);
            return SendQueryStructureRequest(refs, resolveReferences);
        }

        /// <summary>
        /// Sends the specified <paramref name="references"/> to the Web Service defined by <see cref="_config"/> 
        /// </summary>
        /// <param name="references">The <see cref="IStructureReference"/></param>
        /// <param name="resolveReferences">
        /// The resolve references
        /// </param>
        /// <returns>The QueryStructureResponse returned by the Web Service</returns>
        public ISdmxObjects SendQueryStructureRequest(IEnumerable<IStructureReference> references, bool resolveReferences)
        {
            var queryStructureRequestBuilderManager = new QueryStructureRequestBuilderManager();

            IStructureQueryFormat<XDocument> queryFormat = new QueryStructureRequestFormat();
            var wdoc = queryStructureRequestBuilderManager.BuildStructureQuery(references, queryFormat, resolveReferences);

            var doc = new XmlDocument();
            doc.LoadXml(wdoc.ToString());

            string tempFileName = Path.GetTempFileName();

            try
            {

                this.SendRequest(doc, SDMXWSFunction.QueryStructure, tempFileName);

                ISdmxObjects structureObjects = new SdmxObjectsImpl();
                IStructureParsingManager parsingManager = new StructureParsingManager(SdmxSchemaEnumType.Null);
                using (var dataLocation = new FileReadableDataLocation(tempFileName))
                {
                    IStructureWorkspace structureWorkspace = parsingManager.ParseStructures(dataLocation);
                    structureObjects = structureWorkspace.GetStructureObjects(false);
                }

                NsiClientValidation.CheckResponse(structureObjects);
                return structureObjects;
            }
            finally
            {
                //delete the temporary file
                File.Delete(tempFileName);

            }
        }


        /// <summary>
        /// Constructs a SOAP envelope request, with a body that includes the operation as element and the W3C Document and saves the SDMX Part of the response to the specified ouput
        /// The W3C Document contains either a SDMX-ML Query or a SDMX-ML Registry Interface
        /// </summary>
        /// <param name="request">
        /// The W3C Document representation of a SDMX-ML Query or QueryStructureRequest
        /// </param>
        /// <param name="webServiceOperation">
        /// The Web Service function
        /// </param>
        /// <param name="tempFileName">
        /// The temporary file name
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// request is null
        /// </exception>
        /// <exception cref="NsiClientException">
        /// Error in server response or communication
        /// </exception>
        private void SendRequest(XmlDocument request, SDMXWSFunction webServiceOperation, string tempFileName)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            NsiClientHelper.LogSdmx(request);
            var sb = new StringBuilder();
            sb.AppendFormat(SoapConstants.SoapRequest, this._config.Prefix, this._wsdlConfig.GetTargetNamespace());
            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            string operationName = webServiceOperation.ToString();
            XmlNodeList nodes = doc.GetElementsByTagName(SoapConstants.Body, SoapConstants.Soap11Ns);
            XmlElement operation = doc.CreateElement(
                this._config.Prefix, operationName, this._wsdlConfig.GetTargetNamespace());

            XmlElement queryParent = operation;
            string parameterName = this._wsdlConfig.GetParameterName(operationName);
            if (!string.IsNullOrEmpty(parameterName))
            {
                queryParent = doc.CreateElement(
                    this._config.Prefix, parameterName, this._wsdlConfig.GetTargetNamespace());
                operation.AppendChild(queryParent);
            }

            if (request.DocumentElement != null)
            {
                XmlNode sdmxQueryNode = doc.ImportNode(request.DocumentElement, true);
                queryParent.AppendChild(sdmxQueryNode);
            }

            nodes[0].AppendChild(operation);

            var endpointUri = new Uri(this._config.EndPoint);
            var webRequest = (HttpWebRequest)WebRequest.Create(endpointUri);
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            string soapAction = this._wsdlConfig.GetSoapAction(operationName);
            if (soapAction != null)
            {
                webRequest.Headers.Add(SoapConstants.SoapAction, soapAction);
            }

            webRequest.ContentType = HttpConstants.Content;

            // webRequest.Accept = "text/xml";
            webRequest.Method = HttpConstants.Post;
            webRequest.Timeout = 1800 * 1000;
            // webRequest.CookieContainer = new CookieContainer();
            this.SetupWebRequestAuth(webRequest);

            webRequest.UseDefaultCredentials = true;
            var webProxy = System.Net.WebProxy.GetDefaultProxy();
            webProxy.UseDefaultCredentials = true;
            webRequest.Proxy = webProxy;

            using (Stream stream = webRequest.GetRequestStream())
            {
                doc.Save(stream);
            }

            try
            {
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var settings = new XmlWriterSettings();
                            settings.Indent = true;
                            using (XmlWriter writer = XmlWriter.Create(tempFileName, settings))
                            {
                                SoapUtils.ExtractSdmxMessage(stream, writer);
                            }
                            NsiClientHelper.LogSdmx(tempFileName, Resources.InfoSoapResponse);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                NsiClientHelper.HandleSoapFault(ex);
            }

        }

        /// <summary>
        /// Sends the SDMX Query Request
        /// </summary>
        /// <param name="query">
        /// The SDMX Query
        /// </param>
        /// <param name="operation">
        /// The Web Service function
        /// </param>
        /// <param name="tempFileName">
        /// The temp file name
        /// </param>
        private void SendSdmxQuery(IDataQuery query, SDMXWSFunction operation, string tempFileName)
        {
            IDataQueryBuilderManager dataQueryBuilderManager = new DataQueryBuilderManager(new DataQueryFactory());
            var xdoc = dataQueryBuilderManager.BuildDataQuery(query, new QueryMessageV2Format());
            var doc = new XmlDocument();
            doc.LoadXml(xdoc.ToString());
            this.SendRequest(doc, operation, tempFileName);
        }

        /// <summary>
        /// Setup the specified <paramref name="webRequest"/> network authentication and proxy configuration
        /// </summary>
        /// <param name="webRequest">
        /// The <see cref="WebRequest"/>
        /// </param>
        public void SetupWebRequestAuth(WebRequest webRequest)
        {

            if (this._config.EndPoint.Contains("opendata"))
            {
                int x = 0;
            }
            if (this._config.EnableHTTPAuthentication)
            {
                webRequest.Credentials = new NetworkCredential(this._config.UserName, this._config.Password, this._config.Domain);
            }

            if (this._config.EnableProxy)
            {
                if (this._config.UseSystemProxy)
                {
                    webRequest.Proxy = WebRequest.DefaultWebProxy;
                }
                else
                {
                    WebProxy proxy = new WebProxy(this._config.ProxyServer, this._config.ProxyServerPort);
                    if (!string.IsNullOrEmpty(this._config.ProxyUserName) || !string.IsNullOrEmpty(this._config.ProxyPassword))
                    {
                        proxy.Credentials = new NetworkCredential(this._config.ProxyUserName, this._config.ProxyPassword);
                    }

                    webRequest.Proxy = proxy;
                }
            }
        }
        #endregion
    }
}
