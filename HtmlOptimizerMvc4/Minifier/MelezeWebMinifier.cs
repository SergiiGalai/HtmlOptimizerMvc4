using System;
using System.Linq;
using System.Text;

namespace HtmlOptimizerMvc4
{
  /// <summary>
  /// MelezeWebMinifier performs the HTML minification at compile time.
  /// </summary>
  /// <remarks>Original code was copied from Meleze.Web project https://github.com/meleze/Meleze.Web/blob/master/Meleze.Web/Razor/MinifyHtmlMinifier.cs </remarks>
#if DEBUG
  public sealed class MelezeWebMinifier
#else
    internal sealed class MelezeWebMinifier
#endif
  {
    private static readonly char[] LineSeparators = { '\n', '\r' };
    private static readonly char[] WhiteSpaceSeparators = { ' ', '\t', '\n', '\r' };
    private static readonly string[] CommentsMarkers = { "}", "{", "var ", "function", "[if ", "[endif" };
    private static readonly string[] BlockElementsOpenStarts;
    private static readonly string[] BlockElementsCloseStarts;

    static MelezeWebMinifier()
    {
      var blockElements = new string[] { 
            "article", "aside", "div", "dt", "caption", "footer", "form", "header", "hgroup", "html", "map", "nav", "section",
            "body", "p", "dl", "multicol", "dd", "blockquote", "figure", "address", "center",
            "title", "meta", "link", "html", "head", "body", "script", "br", "!DOCTYPE",
            "h1","h2","h3","h4","h5","h6", "pre", "ul", "menu", "dir", "ol", "li", "tr", "tbody", "thead", "tfoot", "td", "th" };

      BlockElementsOpenStarts = new string[blockElements.Length];
      BlockElementsCloseStarts = new string[blockElements.Length];
      for (int i = 0; i < blockElements.Length; i++)
      {
        BlockElementsOpenStarts[i] = "<" + blockElements[i];
        BlockElementsCloseStarts[i] = "</" + blockElements[i];
      }
    }
    
    public bool Comments { get; set; }
    public bool Aggressive { get; set; }
    
    public MelezeWebMinifierState AnalyseContent(string content, MelezeWebMinifierState state)
    {
      if (string.IsNullOrWhiteSpace(content))
        return state;
      
      var previousIsWhiteSpace = char.IsWhiteSpace(content[content.Length - 1]);
      var previousTokenEndsWithBlockElement = EndsWithBlockElement(content);

      var insideScript = IsInsideTag("<script", "</script>", state.InsideScript, content);
      var insidePre = IsInsideTag("<pre", "</pre>", state.InsidePre, content);
      var insideComment = IsInsideTag("<!--", "-->", state.InsideComment, content);
      var preserveComment = insideComment && state.PreserveComment;

      return new MelezeWebMinifierState(previousIsWhiteSpace, previousTokenEndsWithBlockElement,
        insidePre, insideScript, insideComment, preserveComment);
    }

    private static bool IsInsideTag(string tagStart, string tagEnd, bool isInsideNow, string content)
    {
      content = content.ToLower();
      var iscriptstart = content.LastIndexOf(tagStart);
      var iscriptend = content.LastIndexOf(tagEnd);
      if (!isInsideNow || (iscriptend >= 0))
        isInsideNow = iscriptstart >= 0 && iscriptstart >= iscriptend;
      return isInsideNow;
    }


    public string Minify(string content, MelezeWebMinifierState state)
    {
      if (state.InsidePre)
      {
        if (string.IsNullOrEmpty(content))
          return string.Empty;
      }
      else
      {
        if (string.IsNullOrWhiteSpace(content))
          return string.Empty;
      }
      
      var builder = new StringBuilder(content.Length);

      content = Comments 
        ? MinifyComments(content, builder, state) 
        : MinifyHtml(content, builder, state);

      return content;
    }


    /// <summary>
    /// Removes all the comments that are not Javascript or IE conditional comments.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="builder"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    private string MinifyComments(string content, StringBuilder builder, MelezeWebMinifierState state)
    {
      ProcessItem("<!--", "-->", content, state.InsideComment,
        insideComment =>
        {
          // There is a comment but it contains javascript or IE conditionals => we keep it
          if (CommentsMarkers.Any(insideComment.Contains))
            state.PreserveComment = true;

          if (state.PreserveComment)
            builder.Append(insideComment);
        },
        outsideComment => MinifyHtml(outsideComment, builder, state));

      return builder.ToString();
    }


    private string MinifyHtml(string content, StringBuilder builder, MelezeWebMinifierState state)
    {
      return Aggressive
        ? MinifyAggressivelyHtml(content, builder, state)
        : MinifySafelyHtml(content, builder, state);
    }

    /// <summary>
    /// Minify white space while keeping the HTML compatible with the given one.
    /// Blanks between tags on the same line are not minified.
    /// Just the line start/end are trimmed (the indentation).
    /// </summary>
    private static string MinifySafelyHtml(string content, StringBuilder builder, MelezeWebMinifierState state)
    {
      MinifySafelyHtmlBlock(content, builder, state.PreviousIsWhiteSpace, state.InsidePre);

      content = builder.ToString();
      return content;
    }

    private static bool MinifySafelyHtmlBlock(string content, StringBuilder builder, bool previousIsWhiteSpace, bool insidePre)
    {
      ProcessTag("pre", content, insidePre,
        preContent => builder.Append(preContent),
        htmlPreContent =>
        {
          content = htmlPreContent;
          var lines = content.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
          for (int i = 0; i < lines.Length; i++)
          {
            var line = lines[i];
            var trimmedLine = line.Trim();
            if (trimmedLine.Length == 0)
              continue;
            
            if (!previousIsWhiteSpace && char.IsWhiteSpace(line[0]) && (trimmedLine[0] != '<'))
              builder.Append(' ');
            
            builder.Append(trimmedLine);
            previousIsWhiteSpace = false;

            var endsWithWhiteSpace = char.IsWhiteSpace(line[line.Length - 1]) && (trimmedLine[trimmedLine.Length - 1] != '>');
            var hasEndOfLine = (i < lines.Length - 1) || (LineSeparators.Any(s => s == content[content.Length - 1]));

            if (hasEndOfLine && !string.IsNullOrEmpty(trimmedLine))
              builder.Append('\n');
            else if (endsWithWhiteSpace)
              builder.Append(' ');
            
            previousIsWhiteSpace = hasEndOfLine || endsWithWhiteSpace;
          }
        });

      return previousIsWhiteSpace;
    }
  

    /// <summary>
    /// Minify all the white space. Only one space is kept between attributes and words.
    /// Whitespace is completly remove arround HTML block elements while only a single
    /// one is kept arround inline elements.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static string MinifyAggressivelyHtml(string content, StringBuilder builder, MelezeWebMinifierState state)
    {
      bool previousTokenEndsWithBlockElement = state.PreviousTokenEndsWithBlockElement;
      bool previousIsWhiteSpace = state.PreviousIsWhiteSpace;

      ProcessTag("script", content, state.InsideScript,
        jsContent =>
        {
          previousIsWhiteSpace = MinifySafelyHtmlBlock(jsContent, builder, previousIsWhiteSpace, state.InsidePre);
        },
        htmlContent => ProcessTag("pre", htmlContent, state.InsidePre,
          preContent =>
          {
            builder.Append(preContent);
          },
          htmlPreContent =>
          {
            var tokens = htmlPreContent.Split(WhiteSpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            previousTokenEndsWithBlockElement |= (htmlPreContent.Length > 0) && !char.IsWhiteSpace(htmlPreContent[0]);
            previousIsWhiteSpace = false;
            for (int i = 0; i < tokens.Length; i++)
            {
              var token = tokens[i].Trim();
              if (string.IsNullOrEmpty(token))
              {
                continue;
              }
              if (!previousTokenEndsWithBlockElement && !StartsWithBlockElement(token))
              {
                // We have to keep a white space between 2 texts or an inline element and a text or between 2 inline elements
                builder.Append(' ');
              }

              builder.Append(token);

              previousTokenEndsWithBlockElement = EndsWithBlockElement(tokens, i);
            }
            if (!previousTokenEndsWithBlockElement && char.IsWhiteSpace(htmlPreContent[htmlPreContent.Length - 1]))
            {
              builder.Append(' ');
              previousIsWhiteSpace = true;
            }
          }));

      content = builder.ToString();
      return content;
    }


    /// <summary>
    /// Check content inside and outside the tag
    /// </summary>
    /// <param name="tagName">html tag name</param>
    /// <param name="content">source string</param>
    /// <param name="insideTag">is current content already inside tag (was opened previously)</param>
    /// <param name="insideTagAction">action to perform for content inside tag</param>
    /// <param name="outsideTagAction">action to perform for content outside tag</param>
    private static void ProcessTag(string tagName, string content, bool insideTag, Action<string> insideTagAction, Action<string> outsideTagAction)
    {
      var startTag = "<" + tagName;
      var endTag = string.Format("</{0}>", tagName);
      ProcessItem(startTag, endTag, content, insideTag, insideTagAction, outsideTagAction);
    }

    /// <summary>
    /// Check content inside and outside the tag
    /// </summary>
    /// <param name="startTag">html section start</param>
    /// <param name="endTag">html section end</param>
    /// <param name="content">source string</param>
    /// <param name="insideTag">is current content already inside tag (was opened previously)</param>
    /// <param name="insideTagAction">action to perform for content inside tag</param>
    /// <param name="outsideTagAction">action to perform for content outside tag</param>
    private static void ProcessItem(string startTag, string endTag,
      string content, bool insideTag,
      Action<string> insideTagAction, Action<string> outsideTagAction)
    {
      var itag = 0;
      while (itag < content.Length)
      {
        var itagStart = itag == 0 && insideTag ? 0 : content.IndexOf(startTag, itag);
        var itagEnd = content.IndexOf(endTag, itag);

        if (itag > 0 && (!insideTag || itagEnd >= 0))
          insideTag = itagStart >= 0 && itagStart >= itagEnd;

        if (insideTag && (itagEnd < 0))
          itagEnd = content.Length;

        if (itagStart < 0)
          itagStart = content.Length;

        if (insideTag || (itagStart == itag))
        {

          if (itagEnd < content.Length)
          {
            //for case when content equals start tag item
            itagEnd += content.Length < endTag.Length ? content.Length + 1 : endTag.Length;
          }
            

          if (insideTagAction != null)
          {
            var tagContent = content.Substring(itagStart, itagEnd - itagStart);
            insideTagAction(tagContent);
          }

          itag = itagEnd;
          insideTag = false;
        }
        else
        {
          if (outsideTagAction != null)
          {
            var outContent = content.Substring(itag, itagStart - itag);
            outsideTagAction(outContent);
          }

          itag = itagStart;
        }
      }
    }

    private static bool StartsWithBlockElement(string content)
    {
      return content[0] == '<' && (BlockElementsOpenStarts.Any(content.StartsWith) || BlockElementsCloseStarts.Any(content.StartsWith));
    }

    private static bool EndsWithBlockElement(string content)
    {
      if (content[content.Length - 1] != '>')
        return false;
      
      var istart = content.LastIndexOf('<');
      if (istart < 0)
        return false;
      
      return StartsWithBlockElement(content.Substring(istart));
    }

    private static bool EndsWithBlockElement(string[] tokens, int i)
    {
      var content = tokens[i];
      if (content[content.Length - 1] != '>')
        return false;
      
      int istart;
      for (istart = -1; istart < 0 && i >= 0; i--)
      {
        content = tokens[i];
        istart = content.LastIndexOf('<');
      }
      if (istart < 0)
        return false;
      
      return StartsWithBlockElement(content.Substring(istart));
    }
    
  }
}
