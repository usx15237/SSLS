using SSLS.Domain.Concrete;
using SSLS.WebUI.Infrastructure.Abstract;
using SSLS.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSLS.WebUI.Controllers
{
    public class AccountController : Controller
    {
        IAuthProvider authProvider;
        public AccountController(IAuthProvider auth)
        {
            this.authProvider = auth;
        }
        public ActionResult Login()
        {
            return View(); 
        } 
        /*
         * ASP.NET MVC 框架会检查验证约束，这些约束是用数据注解属性应用于RegisterDetail类。
         * 如果用户输入不符合在RegisterDetail类定义的约束，则通过ModelState属性把非法情况传递给动作方法。
         */
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid) 
            {   
                if (authProvider.Authenticate(model.Name, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("Index_book", "Admin"));
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码不正确！");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Account/

    }
}