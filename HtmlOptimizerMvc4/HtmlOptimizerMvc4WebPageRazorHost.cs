namespace HtmlOptimizerMvc4
{
  using System.Web.Mvc.Razor;
  using System.Web.Razor.Generator;

  internal class HtmlOptimizerMvc4WebPageRazorHost : MvcWebPageRazorHost
  {
    public HtmlOptimizerMvc4WebPageRazorHost(string virtualPath, string physicalPath, bool minifyHtml)
      : base(virtualPath, physicalPath)
    {
      MinifyHtml = minifyHtml;
    }

    public bool MinifyHtml { get; set; }

    public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
    {
      if (MinifyHtml && incomingCodeGenerator is CSharpRazorCodeGenerator)
      {
        return new HtmlOptimizerMvc4CSharpRazorCodeGenerator(
                      incomingCodeGenerator.ClassName,
                      incomingCodeGenerator.RootNamespaceName,
                      incomingCodeGenerator.SourceFileName,
                      incomingCodeGenerator.Host);
      }

      return base.DecorateCodeGenerator(incomingCodeGenerator);
    }
  }
}
