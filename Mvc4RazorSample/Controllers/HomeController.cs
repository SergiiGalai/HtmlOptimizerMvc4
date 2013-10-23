using System.Web.Mvc;

namespace Mvc4RazorSample.Controllers
{
    public class HomeController : Controller
    {
      public ActionResult Index()
      {
        ViewBag.Message = "See page Html sources";

        return View();
      }

      public ActionResult About()
      {
        ViewBag.Message = "Your app description page.";

        return View();
      }

      public ActionResult Contact()
      {
        ViewBag.Message = "Your contact page.";

        return View();
      }

    }
}
