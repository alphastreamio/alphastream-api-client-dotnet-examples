using AlphaStream.ApiClient.Watchlists.Models;
using AutoMapper;
using MvcExample.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperConfig.RegisterMapper();
        }
    }

    public class AutoMapperConfig
    {
        public static void RegisterMapper()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Watchlist, WatchlistViewModel>());
        }
    }
}
