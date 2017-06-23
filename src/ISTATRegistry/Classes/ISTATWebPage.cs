using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Globalization;

namespace ISTATRegistry.Classes
{
    public partial class ISTATWebPage : System.Web.UI.Page
    {

        public ICollection<CultureInfo> AvailableLocale { get { return SDMX_Dataloader.Main.Web.I18NSupport.Instance.AvailableLocales.Keys; } }
        public CultureInfo CurrentLocale { get { return System.Threading.Thread.CurrentThread.CurrentUICulture; } }

        private void Page_Error(object sender, EventArgs e)
        {
            Server.Transfer("GenericErrorPage.aspx", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //Utils.PrepareScript(this);
            Utils.ExecuteStaticScript(this);
        }

        protected override void InitializeCulture()
        {
            CultureInfo culture;

            culture = LocaleResolver.GetCookie(HttpContext.Current);

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            // Save cookie
            LocaleResolver.SendCookie(culture, HttpContext.Current);
        }



    }
}