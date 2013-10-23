using System.Web;
using System.Web.Mvc;

namespace Mvc4RazorSample
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }
  }
}