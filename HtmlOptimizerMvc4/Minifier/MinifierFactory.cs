using System.Configuration;

namespace HtmlOptimizerMvc4
{
  internal static class MinifierFactory
  {
    public static IMinifier GetMinifier()
    {
      var minifier = new HtmlOptimizerMinifierWrapper();

      var aggressive = ConfigurationManager.AppSettings["html-minifier: Aggressive"];
      var isAggerssive = aggressive == null || aggressive.ToLower() == "true";

      var comments = ConfigurationManager.AppSettings["html-minifier: Comments"];
      var isComments = comments == null || comments.ToLower() == "true";

      minifier.Init(isAggerssive, isComments);
      return minifier;
    }

    public static IMinifier GetMinifierSimple()
    {
      return new HtmlOptimizerMinifierSimple();
    }
  }
}