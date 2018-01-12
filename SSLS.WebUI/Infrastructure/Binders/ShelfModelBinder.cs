using System.Web.Mvc;
using SSLS.Domain.Concrete;

/* 将Shelf对象写入Session,
 * 我们可以让模型绑定器自动从Session中获取该对象，实现Shelf模型绑定
 */
namespace SSLS.WebUI.Models.Infrastructure.Binders
{
    public class ShelfModelBinder : IModelBinder
    {
        private const string sessionKey = "Shelf";

        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            Shelf shelf = null;
            //如果有session数据
            if (controllerContext.HttpContext.Session != null)
            {
                shelf = (Shelf)controllerContext.HttpContext.Session[sessionKey];
            }
            //如果没有session数据
            if (shelf == null)
            {
                shelf = new Shelf();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = shelf;
                }
            }

            return shelf;
        }
    }
    }