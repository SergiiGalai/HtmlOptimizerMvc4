HtmlOptimizerMvc4
=================

The purpose of this project is minifying your Razor views in ASP.NET MVC4 projects during build time (not runtime).

Razor is a nice engine but formatting is something which is nice during developement process but it is not required in production for the user.
There are several implementations of the same issue I found:

<ol>
<li>Http module which works in runtime and strips spaces (and so on) from the output stream.
It's good but this solution is time consuming because it performs each time view is generated</li>
<li>Optimize during compiling view</li>
<li>Enabling GZIP dramatically reduce html size transported over the network but also works in runtime</li>
</ol>

This project works in the second way.

The idea was taken from the article
"ASP.NET white space cleaning with no runtime cost"
http://omari-o.blogspot.com.ar/2009/09/aspnet-white-space-cleaning-with-no.html<br>
There you can find also implementation for ASP.NET MVC 3 RTW.<br>
The problem is that solution does not work for MVC4 projects

There is also another good project on git: Meleze.Web
https://github.com/meleze/Meleze.Web

It does the same things but can do lot more - remove comments, aggressive mode, css and javascript minification
BUT... It can work only with your main views - it does NOT optimize sections in your views - respected issues were created a year ago but were not fixed.

Also lacking of the documentation - it requires installing System.Web.Optimization library (which was removed from MVC4 by default)

Actually this is the main reason creating this project

Use this project if you want to remove unnecessary line breaks and spaces.

To use the HTML Minification for Razor, you have to change your <code>Views/Web.config</code> to replace the default ASP.NET MVC factory by the HtmlOptimizerMvc4's one:

<pre>
&lt;configuration>
  &lt;system.web.webPages.razor>
    &lt;host factoryType="HtmlOptimizerMvc4.HtmlOptimizerMvc4WebRazorHostFactory, HtmlOptimizerMvc4" />
  &lt;/system.web.webPages.razor>
&lt;/configuration>
</pre>

The HtmlOptimizer behavior can be configured in the application's root <code>Web.config</code> with some appSettings keys:

<pre>
&lt;appSettings>
  &lt;add key="html-minifier" value="true" />
&lt;/appSettings>
</pre>
