namespace HtmlOptimizerMvc4
{
  public sealed class MelezeWebMinifierState
  {
    public MelezeWebMinifierState(bool previousIsWhiteSpace, bool previousTokenEndsWithBlockElement, bool insidePre, bool insideScript, bool insideComment, bool preserveComment)
    {
      PreviousIsWhiteSpace = previousIsWhiteSpace;
      PreviousTokenEndsWithBlockElement = previousTokenEndsWithBlockElement;
      InsidePre = insidePre;
      InsideScript = insideScript;
      InsideComment = insideComment;
      PreserveComment = preserveComment;
    }

    public bool PreviousIsWhiteSpace { get; private set; }
    public bool PreviousTokenEndsWithBlockElement { get; private set; }
    public bool InsideScript { get; private set; }
    public bool InsidePre { get; private set; }
    public bool InsideComment { get; private set; }
    public bool PreserveComment { get; set; }
  }
}