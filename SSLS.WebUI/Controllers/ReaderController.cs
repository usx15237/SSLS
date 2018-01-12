using SSLS.Domain.Abstract;
using SSLS.Domain.Concrete;
using SSLS.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSLS.WebUI.Controllers
{
    public class ReaderController : Controller
    {
        // GET: Reader
        private IBooksReopository Bookrepository; 
        private IOrderProcessor OrderProcessor;
        private MailProcessor MailProcessor;

        public ReaderController(IBooksReopository BookRepository, IOrderProcessor OrderProcessor, MailProcessor MailProcessor)
        {
            this.Bookrepository = BookRepository;
            this.OrderProcessor = OrderProcessor;
            this.MailProcessor = MailProcessor;
        }

        /*          * 当Login用Route属性修饰时，它不需要通过在RouteConfig.cs文件中定义的基于约定的路由来访问。         * 只要通过/DirectLogin即可访问Index动作。         */
        //[Route("DirectLogin")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //动作方法使用HttpPost注解属性来修饰的，这表明该方法将用于处理POST请求。
        /*
         * 这里传入的LoginViewModel为ViewModel,因为用户登录只需要涉及数据库的读操作(显示用户信息)；
         * 而在Register方法中，我们传入的RegisterDetail为DomainModel,因为用户注册涉及数据库的写操作(增加用户)；
         */
        public ActionResult Login(LoginViewModel model, string returnUrl, Shelf shelf)
        {
            if (ModelState.IsValid) 
            {
                Reader ReaderEntry = Bookrepository.Readers.FirstOrDefault(c => c.Name == model.Name && c.Password == model.Password);
                if (ReaderEntry != null)
                { 
                    HttpContext.Session["Reader"] = ReaderEntry;
                    if (shelf.lines.Count() == 0)
                    {
                        return Redirect(returnUrl ?? Url.Action("List", "Book"));
                    }
                    else
                    {
                        return Redirect(returnUrl ?? Url.Action("Index", "Shelf"));
                    } 
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

        public ActionResult Withdraw()
        {
            Session.Clear();
            return  RedirectToAction("List","Book");
        }

        //reader信息展示
        /*
         * /由于这里的reader是指处于session状态的reader(前台reader),而非数据库中的reader(后台reader)。
         * 因此在涉及reader的操作(余额,密码等)时,要注意顺便修改Session['Reader'],以保证Readermsg页面的及时更新
         */
        public ActionResult Readermsg(Reader reader)
        {
            if (reader.Id == 0)
            {
                return RedirectToAction("Login", "Reader");
            }
            else
            {
                return View(reader);
            }
            
        } 

        public ActionResult UpdatePassword(Reader reader)
        {
            return View(reader); 
        } 
        [HttpPost] 
        public ActionResult UpdatePassword(Reader reader, string password)
        {
            //前台处理
            Reader re = Bookrepository.Readers.FirstOrDefault(r => r.Id == reader.Id);
            re.Password = password;
            HttpContext.Session["Reader"] = re; 
            OrderProcessor.UpdatePassword(reader, password);
            return RedirectToAction("Readermsg", reader);
        } 
        public ActionResult AddBlance(Reader reader)
        {
            return View(reader); 
        }
        [HttpPost] 
        public ActionResult AddBlance(Reader reader, decimal price)
        {
            Reader re = Bookrepository.Readers.FirstOrDefault(r => r.Id == reader.Id);
            re.Price = price;
            HttpContext.Session["Reader"] = re;
            OrderProcessor.UpdateBalance(reader, price);
            return RedirectToAction("Readermsg", reader);
        }


        /* 模型验证过程如下
         * 开发者在相应类(ViewModel/DomainModel)中设置好验证注解属性
         * 开发者在需要验证的表单中添加HTML辅助器@Html.ValidationSummary() 
         * 开发者在处理表单的动作方法中添加ModelState.IsValid判断语句
         * 在表单提交后，@Html.ValidationSummary() 与ModelState.IsValid进行合作，判断表单是否通过验证
         */
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]  
        public ActionResult Register(RegisterDetail register)
        { 
            if (ModelState.IsValid)
            {
                MailProcessor.ProcessOrder(register);
                OrderProcessor.Register(register);
                Session.Clear();
                return RedirectToAction("Login");
            }
            else  
            {            
                return View();
            }
        }

    }
}