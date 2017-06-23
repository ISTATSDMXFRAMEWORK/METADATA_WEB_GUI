using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace FlyCallWS
{


    /// <summary>
    /// This class implements a NSI Web Service client
    /// </summary>
    internal class RESTServiceClient
    {
        #region Constants and Fields


        /// <summary>
        /// The HTTP HEADER Content type value
        /// </summary>
        private const string ContentTypeValue = "text/xml;charset=\"utf-8\"";


        /// <summary>
        /// This field holds the a map between a web service operation name
        /// and the parameter wrapper element name. This is used for connecting to .NET WS.
        /// </summary>
        private readonly Dictionary<string, string> _operationParameterName;

        /// <summary>
        /// This field holds the Web Service settings
        /// </summary>
        private readonly WsConfigurationSettings _settings;

        /// <summary>
        /// This field holds the web request
        /// </summary>
        private readonly HttpWebRequest _webRequest;

       

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RESTServiceClient"/> class. 
        /// Initialize a new instance of the RESTServiceClient class
        /// It sets the endpoint, WSDL URL, authentication, proxy and method information
        /// </summary>
        /// <param name="settings">
        /// The Web Service settings.
        /// </param>
        public RESTServiceClient(WsConfigurationSettings settings, string Headers)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this._settings = settings;
            this._operationParameterName = new Dictionary<string, string>();

            try
            {
                this._webRequest = this.CreateWebRequest(Headers);
            }
            catch (WebException ex)
            {
                var errorMessage = this.HandleWebException(ex);
                throw new WebException("Error accessing Web Service. " + errorMessage, ex);
            }
            catch (UriFormatException ex)
            {
                throw new InvalidOperationException("Error parsing Endpoint URL." + ex.Message, ex);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the WebRequest used by this object
        /// </summary>
        public HttpWebRequest WsWebRequest
        {
            get
            {
                return this._webRequest;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the web method with the given Soap Envelope or SDMX-ML Query
        /// </summary>
        /// <param name="soapRequest">
        /// The soap Request.
        /// </param>
        /// <returns>
        /// The Web Service response
        /// </returns>
        public HttpWebRequest InvokeMethod()
        {
            this.WsWebRequest.Timeout = int.MaxValue;
            this.WsWebRequest.ReadWriteTimeout = int.MaxValue;



            //using (Stream stream = this.WsWebRequest.GetRequestStream())
            //{
            //    //soapRequest.Save(stream);
            //}

            return this.WsWebRequest;

        }

        #endregion

        #region Methods


        /// <summary>
        /// Create a web request to the endpoint from the <see cref="_settings"/> values
        /// </summary>
        /// <returns>
        /// The HttpWebRequest object for the configured endpoint
        /// </returns>
        private HttpWebRequest CreateWebRequest(string Headers)
        {
            Uri url= new Uri(this._settings.EndPoint);
            if (!url.LocalPath.EndsWith("/"))
                url = new Uri(this._settings.EndPoint.Replace(url.LocalPath,url.LocalPath+"/"));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = Headers;
            webRequest.ContentType = ContentTypeValue;

            // webRequest.Accept = "text/xml";
            webRequest.Method = "GET";
            if (this._settings.EnableHTTPAuthenication)
            {
                webRequest.Credentials = new NetworkCredential(
                    this._settings.Username, this._settings.Password, this._settings.Domain);
            }

            if (this._settings.EnableProxy)
            {
                if (this._settings.UseSystemProxy)
                {
                    webRequest.Proxy = WebRequest.DefaultWebProxy;
                }
                else
                {
                    var proxy = new WebProxy(this._settings.ProxyServer, this._settings.ProxyServerPort);
                    if (!string.IsNullOrEmpty(this._settings.ProxyUsername)
                        || !string.IsNullOrEmpty(this._settings.ProxyPassword))
                    {
                        proxy.Credentials = new NetworkCredential(
                            this._settings.ProxyUsername, this._settings.ProxyPassword);
                    }

                    webRequest.Proxy = proxy;
                }
            }

            return webRequest;
        }

        /// <summary>
        /// Get a more user friendly error message from <paramref name="webException"/> 
        /// </summary>
        /// <param name="webException">
        /// The <see cref="WebException"/>
        /// </param>
        /// <returns>
        /// The a more user friendly error message from <paramref name="webException"/>
        /// </returns>
        private string HandleWebException(WebException webException)
        {
            var errorResponse = webException.Response as HttpWebResponse;
            string errorMessage = webException.Message;
            if (errorResponse != null)
            {
                switch (errorResponse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        errorMessage = this._settings.EnableHTTPAuthenication
                                           ? "error 401: invalid_credential"
                                           : "error 401: enable Authentication";
                        break;
                    default:
                        errorMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            "error_http",
                            (int)errorResponse.StatusCode,
                            errorResponse.StatusDescription,
                            webException.Message);
                        break;
                }
            }

            return errorMessage;
        }


        #endregion
    }
}
