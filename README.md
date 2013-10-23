HtmlOptimizerMvc4
=================

The purpose of this project is minifying your Razor views in ASP.NET MVC4 projects during build time (not runtime).

Razor is a nice engine but formatting is something which is nice during developement process but it is not required in production for the user.
There are several implementations of the same issue I found:
1. Http module which works in runtime and strips spaces (and so on) from the output stream.
It's good but this solution is time consuming because it performs each time view is generated
2. Optimize during compiling view

This project works in the second way.

The idea was taken from the article
ASP.NET white space cleaning with no runtime cost 
http://omari-o.blogspot.com.ar/2009/09/aspnet-white-space-cleaning-with-no.html
There you can find also implementation for ASP.NET MVC 3 RTW
But that solution does not work for MVC4 projects

There is also another good project on git: Meleze.Web
https://github.com/meleze/Meleze.Web
It does the same things but can do lot more - remove comments, aggressive mode, css and javascript minification
BUT... It can work only with your main views - it does NOT optimize sections in your views - respected issues were created a year ago but were not fixed
Also lacking of the documentation - it requires installing System.Web.Optimization library (which was removed from MVC4 by default)

Actually this is the main reason creating this project

Use this project if you want to remove unnecessary line breaks and spaces.

To use the HTML Minification for Razor, you have to change your Views/Web.config to replace the default ASP.NET MVC factory by the HtmlOptimizerMvc4's one:

<configuration>
  <system.web.webPages.razor>
    <host factoryType="HtmlOptimizerMvc4.HtmlOptimizerMvc4WebRazorHostFactory, HtmlOptimizerMvc4" />
  </system.web.webPages.razor>
</configuration>


The HtmlOptimizer behavior can be configured in the application's root Web.config with some appSettings keys:

<appSettings>
  <add key="html-minifier" value="true" />
</appSettings>
