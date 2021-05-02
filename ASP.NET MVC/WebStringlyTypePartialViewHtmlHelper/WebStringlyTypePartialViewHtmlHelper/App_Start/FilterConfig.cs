using System.Web;
using System.Web.Mvc;

namespace WebStringlyTypePartialViewHtmlHelper
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
