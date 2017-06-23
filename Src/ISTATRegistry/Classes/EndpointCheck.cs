using ISTATUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace ISTATRegistry.Classes
{
    public class EndpointCheck
    {
        public static string IsEndpointUp(EndPointElement EPElement)
        {
            string message = "";
            string url = "";

            if (EPElement.ActiveEndPointType == ActiveEndPointType.REST)
            {
                url = EPElement.RestEndPoint;
            }
            else
            {
                url = EPElement.NSIEndPoint;
            }

            url = url.Replace("/rest/", "/rest"); // rest url fix

            Thread thread = new Thread(() =>
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 7000;

                //EndPointElement EPElement = IRConfiguration.GetEndPointByName(Endpoint.EPName);

                if (EPElement.EnableHTTPAuthenication)
                {
                    request.Credentials = new NetworkCredential(
                        EPElement.HTTPUsername, EPElement.HTTPPassword, EPElement.HTTPDomain);
                }

                if (EPElement.EnableProxy)
                {
                    if (EPElement.UseSystemProxy)
                    {
                        request.Proxy = WebRequest.DefaultWebProxy;
                    }
                    else
                    {
                        var proxy = new WebProxy(EPElement.ProxyServer, Convert.ToInt32(EPElement.ProxyServerPort));
                        if (!string.IsNullOrEmpty(EPElement.ProxyUsername) || !string.IsNullOrEmpty(EPElement.ProxyPassword))
                        {
                            proxy.Credentials = new NetworkCredential(EPElement.ProxyUsername, EPElement.ProxyPassword);
                        }

                        request.Proxy = proxy;
                    }
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();

                    message = response.StatusDescription;
                }
                catch (WebException wex)
                {
                    if (wex.Status.ToString() == "Timeout")
                    {
                        message = "TIMEOUT";
                    }
                    else
                    {
                        message = wex.Message;
                    }
                }
            });

            thread.Start();
            thread.Join();

            return message;
        }

    }
}