using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.ErrorPages
{
    public partial class Oops : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var x = Server.GetLastError();

            //ISTATRegistry.Classes.LatestError error = (ISTATRegistry.Classes.LatestError)Session["LatestError"];

            //Response.Write(error.Url);
            //Response.Write(error.Description);
        }
    }
}