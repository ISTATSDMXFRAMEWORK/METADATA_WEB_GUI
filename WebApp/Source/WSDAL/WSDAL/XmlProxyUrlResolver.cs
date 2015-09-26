using System;
using System.Net;
using System.Xml;

namespace ISTAT.WSDAL
{
    internal class XmlProxyUrlResolver : XmlUrlResolver
    {
        #region Constants and Fields

        /// <summary>
        /// The NSI Client settings
        /// </summary>
        private EndpointSettings _config;

        #endregion

        #region Public Methods

        /// <summary>
        /// Maps a URI to an object containing the actual resource.
        /// An override of <see cref="System.Xml.XmlUrlResolver.GetEntity(Uri,string,Type)"/>
        /// It will use the <see cref="System.Xml.XmlUrlResolver.GetEntity(Uri,string,Type)"/> if no proxy is set.
        /// Else it will create a new <see cref="System.Net.WebRequest"/> and return the response
        /// </summary>
        /// <returns>
        /// A System.IO.Stream object or null if a type other than stream is specified.
        /// </returns>
        /// <param name="absoluteUri">The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)"/></param><param name="role">The current implementation does not use this parameter when resolving URIs. This is provided for future extensibility purposes. For example, this can be mapped to the xlink:role and used as an implementation specific argument in other scenarios. 
        /// </param><param name="objectToReturnType">The type of object to return. The current implementation only returns System.IO.Stream objects. 
        /// </param><exception cref="T:System.Xml.XmlException"><paramref name="objectToReturnType"/> is neither null nor a Stream type. 
        /// </exception><exception cref="T:System.UriFormatException">The specified URI is not an absolute URI. 
        /// </exception><exception cref="T:System.ArgumentNullException"><paramref name="absoluteUri"/> is null. 
        /// </exception><exception cref="T:System.Exception">There is a runtime error (for example, an interrupted server connection). 
        /// </exception>
        public override object GetEntity(Uri absoluteUri, string role, Type objectToReturnType)
        {
            if (!this._config.EnableProxy)
            {
                if (this._config.EnableHTTPAuthentication)
                {
                    this.Credentials = new NetworkCredential(
                        this._config.UserName, this._config.Password, this._config.Domain);
                }

                return base.GetEntity(absoluteUri, role, objectToReturnType);
            }

            WebRequest webRequest = WebRequest.Create(absoluteUri);
            if (this._config.EnableHTTPAuthentication)
            {
                webRequest.Credentials = new NetworkCredential(
                    this._config.UserName, this._config.Password, this._config.Domain);
            }

            if (this._config.UseSystemProxy)
            {
                webRequest.Proxy = WebRequest.DefaultWebProxy;
            }
            else
            {
                IWebProxy proxy = new WebProxy(this._config.ProxyServer, this._config.ProxyServerPort);
                if (!string.IsNullOrEmpty(this._config.ProxyUserName) || !string.IsNullOrEmpty(this._config.ProxyPassword))
                {
                    proxy.Credentials = new NetworkCredential(this._config.ProxyUserName, this._config.ProxyPassword);
                }

                webRequest.Proxy = proxy;
            }

            return webRequest.GetResponse().GetResponseStream();
        }

        /// <summary>
        /// Setter for NSI Client settings
        /// The settings are used for Proxy and HTTP authentication
        /// </summary>
        /// <param name="config">
        /// The NSI Client settings
        /// </param>
        public void SetConfig(EndpointSettings config)
        {
            this._config = config;
        }

        #endregion
    }
}
