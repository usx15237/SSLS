using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSLS.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //属性路由
            /*
             * 在“属性路由”中，路由是由直接运用于控制器类的C#属性定义的。
             * 默认情况下“属性路由”是禁用的，通过MapMvcAttributeRoutes扩展方法可以启用它，该方法由RouteCollection对象调用。
             */
            //routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                null,
                "",
                new { controller = "Book", action = "List", categoryId = 0, page = 1 }
                );

            routes.MapRoute(
    null,
    "page{page}",
    new { controller = "Book", action = "List", categoryId = 0 },
    new { page = @"\d+" }
    );
            routes.MapRoute(
null,
"cId{categoryId}",
new { controller = "Book", action = "List", page = 1 },
new { page = @"\d+" }
);
            routes.MapRoute(
null,
"cId{categoryId}/page{page}",
new { controller = "Book", action = "List" },
new { category = @"\d+", page = @"\d+" }
);
            //必须先定义较具体的路由。
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Book", action = "List", id = UrlParameter.Optional }
            );
            /*
            routes.MapRoute(
                name:规则名称, 可以随意起名。不可以重名,
                url:url获取数据的规则,
                defaults: 初始化默认值,即设置默认页面;即使当前路由为"http://localhost:28959/"，也会显示某个指定页面 }
            );
            */
        }
    }
}

