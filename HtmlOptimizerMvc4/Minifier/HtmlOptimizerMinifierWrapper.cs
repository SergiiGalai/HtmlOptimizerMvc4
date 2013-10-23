namespace HtmlOptimizerMvc4
{
  public sealed class HtmlOptimizerMinifierWrapper : IMinifier
  {
    private readonly MelezeWebMinifier _minifier = new MelezeWebMinifier();
    private MelezeWebMinifierState _state;

    public void Init(bool aggressive, bool comments)
    {
      _state = new MelezeWebMinifierState(true, false, false, false, false, false);
      _minifier.Aggressive = aggressive;
      _minifier.Comments = comments;
    }


    public string Minify(string content)
    {
      content = _minifier.Minify(content, _state);
      _state = _minifier.AnalyseContent(content, _state);
      return content;
    }
  }
}