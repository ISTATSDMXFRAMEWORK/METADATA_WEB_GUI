using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace ISTATRegistry
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XElement xelement = XElement.Load(Server.MapPath(@".\UserFiles\ws_istat.xml"));
            IEnumerable<XElement> users = xelement.Elements();
            // Read the entire XML
            foreach (var user in users)
            {
                string username = user.Attribute("username").Value;
                string password = user.Attribute("password").Value;
                bool isadmin = bool.Parse(user.Attribute("isadmin").Value);
            }

            var us = from u in xelement.Elements("user")
                     where (string)u.Attribute("username") == "admin"
                            && (string)u.Attribute("password") == "admin"
                     select u;

            var us2 = from u in xelement.Elements("user")
                     where (string)u.Attribute("username") == "poldo"
                     select u;


            var us3 = from u in xelement.Elements("user")
                     where (string)u.Attribute("username") == "admin"
                            && (string)u.Attribute("password") == "passwordasd"
                     select u;

            var x = us.FirstOrDefault();
            var y = us2.FirstOrDefault();
            var z = us3.FirstOrDefault();

            var agencies = from ags in x.Elements("agencies")
                           select ags;

            var agencies2 = from ags2 in x.Elements("agencies").Elements("agency")
                           select ags2;

            foreach (string id in agencies2.Attributes("id"))
            {
                string el = id;
            }
        }
    }
}