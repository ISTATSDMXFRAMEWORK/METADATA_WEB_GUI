using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ISTATRegistry
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Org.Sdmxsource.Sdmx.Api.Exception.SdmxException.SetMessageResolver(new Org.Sdmxsource.Util.ResourceBundle.MessageDecoder());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Handle HTTP errors
            //if (exc.GetType() == typeof(HttpException))
            //{
            //    Server.Transfer("HttpErrorPage.aspx");
            //    Server.ClearError();
            //    return;
            //}

            // For other kinds of errors give the user some information
            // but stay on the default page
            //Response.Write("<h2>Global Page Error</h2>\n");
            //Response.Write(
            //    "<p>" + exc.Message + "</p>\n");
            //Response.Write("Return to the <a href='Default.aspx'>" +
            //    "Default Page</a>\n");

            ////// Log the exception and notify system operators
            //ISTATRegistry.Classes.ExceptionUtility.LogException(exc, "DefaultPage");
            ////ExceptionUtility.NotifySystemOps(exc);

            //// Clear the error from the server
            //Server.ClearError();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}