using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyCallWS
{
    public class WsConfigurationSettings
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the Domain
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP Authentication is enabled
        /// </summary>
        public bool EnableHTTPAuthenication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Proxy is enabled or disabled
        /// </summary>
        public bool EnableProxy { get; set; }

        /// <summary>
        /// Gets or sets the collection of <c>WsConfigurationElement(s)</c>
        /// A custom XML section for an applications configuration file.
        /// </summary>
        /// <summary>
        /// Get or set the Web Service Endpoint URL
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the collection of <c>WsConfigurationElement(s)</c>
        /// A custom XML section for an applications configuration file.
        /// </summary>
        /// <summary>
        /// Get or set the Web Service Endpoint base URL
        /// </summary>
        public string RestEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the Proxy Password
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// Gets or sets the Proxy server
        /// </summary>
        public string ProxyServer { get; set; }

        /// <summary>
        /// Gets or sets the Proxy port
        /// </summary>
        public int ProxyServerPort { get; set; }

        /// <summary>
        /// Gets or sets the Proxy User name
        /// </summary>
        public string ProxyUsername { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="System.Net.WebRequest.DefaultWebProxy"/> proxy configuration is enabled or disabled
        /// </summary>
        public bool UseSystemProxy { get; set; }

        /// <summary>
        /// Gets or sets the HTTP User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        public string WSDL { get; set; }

        #endregion
    }
}
