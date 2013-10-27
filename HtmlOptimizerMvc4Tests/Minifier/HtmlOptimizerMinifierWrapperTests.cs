using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HtmlOptimizerMvc4.Tests
{
  [TestClass()]
  public class HtmlOptimizerMinifierWrapperTests
  {
    [TestMethod()]
    public void MinifyCssClass()
    {
      const string src = " class=\"content-wrapper\"";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);
      var actual = target.Minify(src);
      Assert.AreEqual(" class=\"content-wrapper\"", actual);

      target.Init(false, true);
      actual = target.Minify(src);
      Assert.AreEqual("class=\"content-wrapper\"", actual);
    }


    [TestMethod()]
    public void MinifyScript()
    {
      const string src1 = "<script";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);

      var actual = target.Minify(src1);
      Assert.AreEqual(src1, actual);


      target.Init(false, true);

      actual = target.Minify(src1);
      Assert.AreEqual(src1, actual);
    }

    [TestMethod()]
    public void MinifyDiv()
    {
      const string src = ">\r\n        <div";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);
      var actual = target.Minify(src);
      Assert.AreEqual("><div", actual);

      target.Init(false, true);
      actual = target.Minify(src);
      Assert.AreEqual(">\n<div", actual);
    }

    [TestMethod()]
    public void MinifyAggressiveLink()
    {
      const string src = " href=\"http://go.microsoft.com/fwlink/?LinkId=245153\"";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);
      var actual = target.Minify(src);
      Assert.AreEqual(" href=\"http://go.microsoft.com/fwlink/?LinkId=245153\"", actual);

      target.Init(false, true);
      actual = target.Minify(src);
      Assert.AreEqual("href=\"http://go.microsoft.com/fwlink/?LinkId=245153\"", actual);
    }

    [TestMethod()]
    public void MinifyAggressivePreWholeItem()
    {
      const string src =
        ">\r\n      <pre>\r\n        Formatting in PRE\r\n        preserved\r\n        \r\n        ANYWAY\r\n      </pre>\r\n      ";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);
      var actual = target.Minify(src);
      Assert.AreEqual("> <pre>\r\n        Formatting in PRE\r\n        preserved\r\n        \r\n        ANYWAY\r\n      </pre> ", actual);
      
      target.Init(false, true);
      actual = target.Minify(src);
      Assert.AreEqual(">\n<pre>\r\n        Formatting in PRE\r\n        preserved\r\n        \r\n        ANYWAY\r\n      </pre>", actual);
    }

    [TestMethod()]
    public void MinifyAggressivePreSplittedItem()
    {
      const string src1 = "\r\n      <pre> \r\n";
      const string src2 = "     ";
      const string src3 = "\r\n        Some other text\r\n      </pre>\r\n    </li>\r\n</ol>\r\n";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);
      
      var actual = target.Minify(src1);
      Assert.AreEqual(" <pre> \r\n", actual);

      actual = target.Minify(src2);
      Assert.AreEqual("     ", actual);

      actual = target.Minify(src3);
      Assert.AreEqual("\r\n        Some other text\r\n      </pre></li></ol>", actual);


      target.Init(false, true);
      actual = target.Minify(src1);
      Assert.AreEqual("<pre> \r\n", actual);

      actual = target.Minify(src2);
      Assert.AreEqual("     ", actual);

      actual = target.Minify(src3);
      Assert.AreEqual("\r\n        Some other text\r\n      </pre></li>\n</ol>\n", actual);
    }

    [TestMethod()]
    public void MinifyAggressiveBigScript()
    {
      const string src1 = "\r\n  <script";
      const string src2 = " type=\"text/javascript\"";
      const string src3 = ">\r\n    //<![CDATA[\r\n    jQuery(document).ready(function () {\r\n      $(\"#login\").hide();\r\n    });\r\n    //]]>\r\n</script>\r\n";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);

      var actual = target.Minify(src1);
      Assert.AreEqual(" <script", actual);

      actual = target.Minify(src2);
      Assert.AreEqual(" type=\"text/javascript\"", actual);

      actual = target.Minify(src3);
      Assert.AreEqual(">\n//<![CDATA[\njQuery(document).ready(function () {\n$(\"#login\").hide();\n});\n//]]>\n</script> "
, actual);


      target.Init(false, true);

      actual = target.Minify(src1);
      Assert.AreEqual("<script", actual);

      actual = target.Minify(src2);
      Assert.AreEqual(" type=\"text/javascript\"", actual);

      actual = target.Minify(src3);
      Assert.AreEqual(">\n//<![CDATA[\njQuery(document).ready(function () {\n$(\"#login\").hide();\n});\n//]]>\n</script>\n"
, actual);
    }


    [TestMethod()]
    public void MinifyComment()
    {
      const string src1 = "<!--comment in the text-->\r\n<h3>We suggest the following:</h3>\r\n<ol";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);

      var actual = target.Minify(src1);
      Assert.AreEqual("<h3>We suggest the following:</h3><ol", actual);


      target.Init(false, true);

      actual = target.Minify(src1);
      Assert.AreEqual("<h3>We suggest the following:</h3>\n<ol", actual);
    }

    [TestMethod()]
    public void MinifyCommentInside()
    {
      const string src1 = "<h1>header</h1>\r\n<!--comment in the text-->\r\n<h3>We suggest the following:</h3>\r\n<ol";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);

      var actual = target.Minify(src1);
      Assert.AreEqual("<h1>header</h1><h3>We suggest the following:</h3><ol", actual);


      target.Init(false, true);

      actual = target.Minify(src1);
      Assert.AreEqual("<h1>header</h1>\n<h3>We suggest the following:</h3>\n<ol", actual);
    }

    [TestMethod()]
    public void MinifyIeSpecificComment()
    {
      const string src1 = "</title>\r\n  <!--[if lte IE 8]>\r\n  <script";
      const string src2 = " src=\"http://ie7-js.googlecode.com/svn/version/2.1(beta4)/IE8.js\"";
      const string src3 = "></script>\r\n  <![endif]-->\r\n";

      var target = new HtmlOptimizerMinifierWrapper();
      target.Init(true, true);
      
      var actual = target.Minify(src1);
      Assert.AreEqual("</title><!--[if lte IE 8]>\r\n  <script", actual);

      actual = target.Minify(src2);
      Assert.AreEqual(" src=\"http://ie7-js.googlecode.com/svn/version/2.1(beta4)/IE8.js\"", actual);

      actual = target.Minify(src3);
      Assert.AreEqual("></script>\r\n  <![endif]-->", actual);


      target.Init(false, true);

      actual = target.Minify(src1);
      Assert.AreEqual("</title>\n<!--[if lte IE 8]>\r\n  <script", actual);

      actual = target.Minify(src2);
      Assert.AreEqual(" src=\"http://ie7-js.googlecode.com/svn/version/2.1(beta4)/IE8.js\"", actual);

      actual = target.Minify(src3);
      Assert.AreEqual("></script>\r\n  <![endif]-->", actual);
    }

  }
}
