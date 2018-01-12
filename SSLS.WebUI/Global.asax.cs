using SSLS.Domain.Concrete;
using SSLS.WebUI.Infrastructure;
using SSLS.WebUI.Models.Infrastructure.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

/*
 * 在系统第一次运行时，在MVC框架中添加自定义的模型绑定器ShelfModelBinder与ReaderModelBinder。
 */
namespace SSLS.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(Shelf), new ShelfModelBinder());
            ModelBinders.Binders.Add(typeof(Reader), new ReaderModelBinder());
        }
    }
}
