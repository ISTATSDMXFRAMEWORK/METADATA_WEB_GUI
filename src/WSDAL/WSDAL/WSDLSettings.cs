//using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace ISTAT.WSDAL
{
    internal class WSDLSettings
    {
        #region Constants and Fields

        /// <summary>
        /// This field holds the a map between a web service operation name
        /// and the parameter wrapper element name. This is used for connecting to .NET WS.
        /// </summary>
        private readonly Dictionary<string, string> _operationParameterName = new Dictionary<string, string>();

        /// <summary>
        /// This field holds the a map between a web service operation name
        /// and the soap action
        /// </summary>
        private readonly Dictionary<string, string> _soapAction = new Dictionary<string, string>();

        /// <summary>
        /// Holds the service description of the WSDL set in <see cref="EndpointSettings.Wsdl"/>
        /// </summary>
        private ServiceDescription _wsdl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WSDLSettings"/> class. 
        /// Initialize a new instance of the WSDLSettings class with EndpointSettings configuration
        /// </summary>
        /// <param name="config">
        /// The <see cref="EndpointSettings"/> config used to retrieve the WSDL URL and other information such as proxy
        /// </param>
        public WSDLSettings(EndpointSettings config)
        {
            this.GetWSDL(config);
            this.BuildOperationParameterName();
            this.ΒuildSoapActionMap();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the parameter element name if one exists for the given operation
        /// </summary>
        /// <param name="operationName">
        /// The name of the operation or web method or function
        /// </param>
        /// <returns>
        /// null if there isn't a parameter element name e.g. in Java or the parameter element name (e.g. in .net) 
        /// </returns>
        public string GetParameterName(string operationName)
        {
            string ret;
            this._operationParameterName.TryGetValue(operationName, out ret);
            return ret;
        }

        /// <summary>
        /// Get the soapAction URI if one exists for the given operation
        /// </summary>
        /// <param name="operationName">
        /// The name of the operation or web method or function
        /// </param>
        /// <returns>
        /// The "http://schemas.xmlsoap.org/wsdl/soap/" soapAction value or null if there isn't a soapAction 
        /// </returns>
        public string GetSoapAction(string operationName)
        {
            string ret;
            this._soapAction.TryGetValue(operationName, out ret);
            return ret;
        }

        /// <summary>
        /// Get the WSDL target namespace
        /// </summary>
        /// <returns>
        /// The target namespace
        /// </returns>
        public string GetTargetNamespace()
        {
            return this._wsdl.TargetNamespace;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populate the <see cref="_operationParameterName"/> dictionary with operation.Name to Parameter name
        /// </summary>
        private void BuildOperationParameterName()
        {
            // Tested with .net and java NSI WS WSDL
            foreach (XmlSchema schema in this._wsdl.Types.Schemas)
            {
                foreach (XmlSchemaElement element in schema.Elements.Values)
                {
                    if (element.RefName.IsEmpty)
                    {
                        var complexType = element.SchemaType as XmlSchemaComplexType;
                        if (complexType != null)
                        {
                            var seq = complexType.Particle as XmlSchemaSequence;
                            if (seq != null && seq.Items.Count == 1)
                            {
                                var body = seq.Items[0] as XmlSchemaElement;
                                if (body != null && !string.IsNullOrEmpty(body.Name))
                                {
                                    if (!this._operationParameterName.ContainsKey(element.Name))
                                    {
                                        this._operationParameterName.Add(element.Name, body.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the WSDL.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        private void GetWSDL(EndpointSettings config)
        {
            try
            {
                var resolver = new XmlProxyUrlResolver();
                resolver.SetConfig(config);

                var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true, XmlResolver = resolver };
                string wsdlUrl = config.Wsdl;

                if (string.IsNullOrEmpty(wsdlUrl))
                {
                    wsdlUrl = string.Format(CultureInfo.InvariantCulture, "{0}?wsdl", config.EndPoint);
                }


                //System.Net.WebProxy myProxy = new System.Net.WebProxy();
                //myProxy.UseDefaultCredentials = true;
                //System.Uri newUri = new System.Uri("http://proxy.istat.it:3128");
                //myProxy.Address = newUri;

                System.Net.WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                System.Net.WebRequest request = System.Net.WebRequest.Create(wsdlUrl);

                request.UseDefaultCredentials = true;

                var webProxy = System.Net.WebProxy.GetDefaultProxy();
                webProxy.UseDefaultCredentials = true;
                request.Proxy = webProxy;

                using (System.Net.WebResponse response = request.GetResponse())
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    this._wsdl = ServiceDescription.Read(reader);
                }


                //using (XmlReader reader = XmlReader.Create(wsdlUrl, settings))
                //{
                //    this._wsdl = ServiceDescription.Read(reader);
                //}

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Populate the <see cref="_soapAction"/> dictionary with operation.Name to soapAction URI
        /// </summary>
        private void ΒuildSoapActionMap()
        {
            foreach (Binding binding in this._wsdl.Bindings)
            {
                foreach (OperationBinding operation in binding.Operations)
                {
                    foreach (object ext in operation.Extensions)
                    {
                        var operationBinding = ext as SoapOperationBinding;
                        if (operationBinding != null)
                        {
                            if (!this._soapAction.ContainsKey(operation.Name))
                            {
                                this._soapAction.Add(operation.Name, operationBinding.SoapAction);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
