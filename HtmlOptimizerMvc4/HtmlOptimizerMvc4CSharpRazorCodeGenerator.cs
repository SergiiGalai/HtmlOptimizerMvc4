using System.CodeDom;
using System.Web.Razor.Generator;
using System.Web.Razor.Text;
using System.Web.Razor.Tokenizer.Symbols;

namespace HtmlOptimizerMvc4
{
    using System.Text.RegularExpressions;
    using System.Web.Mvc.Razor;
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;

  internal class HtmlOptimizerMvc4CSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
      public HtmlOptimizerMvc4CSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
          var webPageRazorHost = host as MvcWebPageRazorHost;
          if (webPageRazorHost == null || webPageRazorHost.IsSpecialPage)
            return;
          this.SetBaseType("dynamic");
        }

        private void SetBaseType(string modelTypeName)
        {
          var codeTypeReference = new CodeTypeReference(this.Context.Host.DefaultBaseClass + "<" + modelTypeName + ">");
          this.Context.GeneratedClass.BaseTypes.Clear();
          this.Context.GeneratedClass.BaseTypes.Add(codeTypeReference);
        }
    
        public override void VisitSpan(Span span)
        {
            if (span.Kind == SpanKind.Markup)
            {
                string content = span.Content;
              
                content = Regex.Replace(content, @"\s*\n\s*", "\n", RegexOptions.Multiline);
                content = Regex.Replace(content, @"\s{2,}", " ", RegexOptions.Multiline);
              
                // We replace the content with the minified markup
                // and then let the CSharp/VB generator do their jobs.
                var builder = new SpanBuilder() { CodeGenerator = span.CodeGenerator, EditHandler = span.EditHandler, Kind = span.Kind, Start = span.Start };
                var symbol = new MarkupSymbol() { Content = content };
                builder.Accept(symbol);
                span.ReplaceWith(builder);
            }
       
            base.VisitSpan(span);
        }

        /// <summary>
        /// From Meleze.Web project - https://github.com/meleze/Meleze.Web
        /// </summary>
        private sealed class MarkupSymbol : ISymbol
        {
          private string _content;
          private SourceLocation _start = SourceLocation.Zero;

          public void ChangeStart(SourceLocation newStart)
          {
            _start = newStart;
          }

          public string Content
          {
            get { return _content; }
            set { _content = value; }
          }

          public void OffsetStart(SourceLocation documentStart)
          {
            _start = documentStart;
          }

          public SourceLocation Start
          {
            get { return _start; }
          }
        }
    }


}
