using System.Configuration;

namespace HtmlOptimizerMvc4
{
    using System.Web.Mvc;
    using System.Web.WebPages.Razor;

  public class HtmlOptimizerMvc4WebRazorHostFactory : MvcWebRazorHostFactory 
    {

        public override WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            WebPageRazorHost host = base.CreateHost(virtualPath, physicalPath);

            if (host.IsSpecialPage || host.DesignTimeMode)
                return host;
          
            var minifierSetting = ConfigurationManager.AppSettings["html-minifier"];
            var isMinifierEnabled = minifierSetting == null || minifierSetting.ToLower() == "true";

            return new HtmlOptimizerMvc4WebPageRazorHost(virtualPath, physicalPath, isMinifierEnabled);            
        }
    }
}
