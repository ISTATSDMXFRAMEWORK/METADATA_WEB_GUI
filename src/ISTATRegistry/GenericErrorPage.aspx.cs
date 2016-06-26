using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry
{
    public partial class GenericErrorPage : System.Web.UI.Page
    {
        protected Exception ex = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the last error from the server
            Exception ex = Server.GetLastError();

            if (ex.InnerException != null)
            {
                pnlInnerException.Visible = true;
                innerTrace.Text = ex.InnerException.StackTrace;
                innerMessage.Text = ex.InnerException.Message;
            }
            exMessage.Text = ex.Message;
            exTrace.Text = ex.StackTrace;

            // Log the exception and notify system operators
            ISTATRegistry.Classes.ExceptionUtility.LogException(ex, "Generic Error Page");
            // Clear the error from the server
            Server.ClearError();
        }
    }
}