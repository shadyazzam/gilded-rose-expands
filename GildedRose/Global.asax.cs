using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GildedRose
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteTable.Routes.IgnoreRoute("{ resource}.axd/{*pathInfo}");
            RouteTable.Routes.MapMvcAttributeRoutes(); //Enables Attribute Routing
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
