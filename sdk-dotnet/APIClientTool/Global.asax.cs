using System.Net;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace APIClientTool
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void ValidatesCertificate()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }
        protected void Application_Start()
        {
            ValidatesCertificate();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
