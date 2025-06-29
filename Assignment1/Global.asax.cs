using Assignment1.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Assignment1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            // Handle application errors here
            Exception exception = Server.GetLastError();

            var httpContext = HttpContext.Current;

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"]?.ToString();
            var actionName = httpContext.Request.RequestContext.RouteData.Values["action"]?.ToString();

            var model = new HandleErrorInfo(exception, controllerName, actionName);

            // Log the error or redirect to an error page
            Server.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";

            // Manually invoke the controller
            var controller = new ErrorController();
            var wrapper = new HttpContextWrapper(httpContext);
            var context = new ControllerContext(wrapper, routeData, controller);
            controller.ControllerContext = context;

            var result = controller.Index(model) as ViewResult;
            result.ExecuteResult(context);
        }
    }
}
