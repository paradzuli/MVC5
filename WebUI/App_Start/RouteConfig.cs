using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //-- / lists the first page of products from all categories
            routes.MapRoute(null, "", new
            {
                controller = "Product",
                action = "List",
                category = (string)null,
                page = 1
            });

            //-- /Page2 lists specified page 2 from all categories
            routes.MapRoute(null,"Page{page}", new { controller = "Product", action = "List", category = (string)null }, new {page=@"\d+"});


            //-- /Soccer shows the first page of items from specified category, Soccer
            routes.MapRoute(null, "{category}", new { controller = "Product", action = "List", page = 1 });

            //-- /Soccer/Page2 shows the specified page 2 of items in the category of Product
            routes.MapRoute(
                name:null,
                url: "{category}/Page{page}",
                defaults: new { Controller="Product",action="List"},
                constraints: new {page=@"\d+"}
                );

            routes.MapRoute(null, "{controller}/{action}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Product", action = "List", id = UrlParameter.Optional }
            //);
        }
    }
}
