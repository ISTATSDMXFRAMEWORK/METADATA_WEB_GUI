using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.Classes
{
    public class IstatUpdatePanel : System.Web.UI.UpdatePanel
    {
        public IstatUpdatePanel()
        {
            // NULL
        }

        protected override Control CreateContentTemplateContainer()
        {
            MyContentTemplateContainer myContentTemplateContainer = new MyContentTemplateContainer();
            return myContentTemplateContainer;
        }

        private sealed class MyContentTemplateContainer : Control
        {
            protected override void Render(HtmlTextWriter writer)
            {   
                base.Render(writer);     
                Utils.AppendScript( "$.unblockUI();" );
            }
        }
    }
}