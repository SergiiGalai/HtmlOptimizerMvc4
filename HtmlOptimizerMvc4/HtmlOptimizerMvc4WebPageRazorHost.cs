namespace HtmlOptimizerMvc4
{
  using System.Web.Mvc.Razor;
  using System.Web.Razor.Generator;

  internal class HtmlOptimizerMvc4WebPageRazorHost : MvcWebPageRazorHost
  {
    public HtmlOptimizerMvc4WebPageRazorHost(string virtualPath, string physicalPath)
      : base(virtualPath, physicalPath){}
    
    public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
    {
      if (incomingCodeGenerator is CSharpRazorCodeGenerator)
      {
        var minifier = MinifierFactory.GetMinifier();
        
        return new HtmlOptimizerMvc4CSharpRazorCodeGenerator(
                      incomingCodeGenerator.ClassName,
                      incomingCodeGenerator.RootNamespaceName,
                      incomingCodeGenerator.SourceFileName,
                      incomingCodeGenerator.Host, 
                      minifier);
      }

      return base.DecorateCodeGenerator(incomingCodeGenerator);
    }
  }
}
