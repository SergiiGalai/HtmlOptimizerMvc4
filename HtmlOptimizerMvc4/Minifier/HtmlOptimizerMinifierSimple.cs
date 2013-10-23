using System.Text.RegularExpressions;

namespace HtmlOptimizerMvc4
{
#if DEBUG
  public sealed class HtmlOptimizerMinifierSimple : IMinifier
#else
    internal sealed class HtmlOptimizerMinifierSimple : IMinifier
#endif
  {
    public string Minify(string content)
    {
      var r = Regex.Replace(content, @"\s*\n\s*", "\n", RegexOptions.Multiline);
      r = Regex.Replace(r, @"\s{2,}", " ", RegexOptions.Multiline);
      return r;
    }
  }
}