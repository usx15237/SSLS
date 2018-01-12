using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SSLS.Domain.Abstract;
using SSLS.Domain.Concrete;
using SSLS.WebUI.Models; 

namespace SSLS.WebUI.Controllers
{
    public class BookController : Controller
    {
        //如果不使用依赖注入,就需要引入EFBookRepository和DatabaseOrderProcessor,增加控制器与接口实现类间的耦合
        /*
         * private IBooksReopository reopository = new EFBookRepository();
         * private IOrderProcessor orderProcessor = new DatabaseOrderProcessor();
         */



        /* 
         * 1.MVC框架根据路由推测该用户的请求需要BookController控制器来处理
         * 2.MVC框架向NinjectDependencyResolver(自定义的依赖解析器)发送请求获取Controller类实例
         * 3.Ninject检测到BookController构造器中有对IBooksReopository,IOrderProcessor接口的依赖:public BookController(){}
         * 4.根据Ninject中的接口与实现类的绑定设置，Ninject创建DatabaseOrderProcessor和EFBookRepository实例
         * 5.DatabaseOrderProcessor和EFBookRepository实例被用来创建一个BookController的实例。
         */
        private IBooksReopository reopository;
        private IOrderProcessor orderProcessor;

        public int PageSize = 6;
        public BookController(IBooksReopository reopository, IOrderProcessor orderProcessor)
        {
            this.reopository = reopository;
            this.orderProcessor = orderProcessor; 
        }
        public ActionResult List(int categoryId = 0, int page = 1, string search = "")
        {
            IQueryable<Book> booksList;
            if (search != "")
            {
                booksList = reopository.Books.Where(b => b.AUthors.Contains(search) || b.Name.Contains(search) || b.Description.Contains(search));
            }
            else
            {
                booksList = reopository.Books;
            }
            if (categoryId > 0)
            {
                booksList = booksList.Where(b => b.CategoryId == categoryId);
            }
            BooksListViewModel viewModel = new BooksListViewModel
            {
                Books = booksList
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = booksList.Count()
                },
                CurrentCategoryId = categoryId,
            };
            return View(viewModel);
        }

        public PartialViewResult BookSummary(Reader reader, Shelf shelf, Book book)
        {
            //检查用户所选图书是否已在书架上或处于借阅状态 
            if (orderProcessor.Norepeat(book, reader) && shelf.Norepeat(book))
            {
                ViewBag.statue = "可借";
            }
            else
            {
                ViewBag.statue = "不可借";
            }
            return PartialView(book);
        }

        //查看图书详情
        public ActionResult SeeDetail(Reader reader, Shelf shelf, int Id, string returnUrl)
        {
            Book book = reopository.Books.FirstOrDefault(p => p.Id == Id);
            //检查用户所选图书是否已在书架上或处于借阅状态 
            if (orderProcessor.Norepeat(book, reader) && shelf.Norepeat(book))
            {
                ViewBag.statue = "可借";
            }
            else
            {
                ViewBag.statue = "不可借";
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(book);
        }
    }
}