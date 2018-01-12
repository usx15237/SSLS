using SSLS.Domain.Abstract;
using SSLS.Domain.Concrete;
using SSLS.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;

namespace SSLS.WebUI.Controllers
{
    public class ShelfController : Controller
    {
        // GET: Shelf
        private IBooksReopository repository;
        private IOrderProcessor borrowProcessor;


        public ShelfController(IBooksReopository repo, IOrderProcessor Processor)
        {
            repository = repo;
            borrowProcessor = Processor;
        }

        public ViewResult Index(Shelf shelf, string returnUrl)
        {
            return View(new CurrentReaderModel
            {
                ReturnUrl = returnUrl,
                Shelf = shelf
            });
        }
         
        //加入购物车
        public ActionResult AddToShelf(Reader reader, Shelf shelf, int Id,
               string returnUrl)
        {
            //根据Id获取Book(BookSummary的表单只传递了id参数)
            Book book = repository.Books.FirstOrDefault(p => p.Id == Id);

            //将该书放入书架
            shelf.AddItem(book);
            return RedirectToAction("Index", new { returnUrl });

        }

        //从购物车中删除
        public RedirectToRouteResult RemoveFromShelf(Shelf shelf, int Id)
        {
            Book Book = repository.Books
                .FirstOrDefault(p => p.Id == Id);
            if (Book != null)
            {
                shelf.RemoveLine(Book);
            }
            return RedirectToAction("Index");
        }

        //查看本次借阅的图书 
        public ActionResult Completed(Shelf shelf, Reader reader, string returnUrl, int[] bookid)
        {
            List<Book> books = new List<Book>();

            if (reader.Id == 0)
            {
                return RedirectToAction("Login", "Reader");
            }
            else if (bookid.Length == 0)
            {
                return View("Index", new { returnUrl });
            }
            else
            {
                for (var i = 0; i < bookid.Length; i++)
                {
                    Book book = repository.FindBookById(bookid[i]);
                    books.Add(book);
                }
                //注意一定要先生成一条新的订单，再生成一条新的借阅记录(借阅记录具有外键FineId)
                borrowProcessor.ProcessFine(books);
                borrowProcessor.ProcessBorrow(books, reader);
                Session["Shelf"] = null;
                ViewBag.readerName = reader.Name;
                ViewBag.BorrowTime = DateTime.Now;
                ViewBag.DateShouldBeReturn = DateTime.Now.AddMonths(1);
                ViewBag.ReturnUrl = returnUrl;
                return View(books);
            }
        }

        //查看尚未归还的图书
        public ActionResult Borrowing(Reader reader, string returnUrl)
        {
            if (reader.Id == 0)
            {
                return RedirectToAction("Login", "Reader");
            }
            else
            {
                IEnumerable<Borrow> borrows = repository.Borrows.Where(u => u.ReaderId == reader.Id && u.ReturnTime == null);
                foreach (var br in borrows)
                {
                    /*
                     * 因为存在用户先查看"我的借阅"，再查看"我的罚款"的情况(即Borrowing()先于MyFine()执行);
                     * 因此有必要在Borrowing中进行罚款状态的数据改写
                     */
                    if (br.DateShouldBeReturn < DateTime.Now && br.NeedtoFine == "暂无需要")
                    {
                        br.Fine.FinePrice = (Decimal)(DateTime.Now.Day - br.DateShouldBeReturn.Day);
                    }
                }
                ViewBag.readerName = reader.Name; 
                ViewBag.ReturnUrl = returnUrl;
                return View(borrows);
            }
        }

        //查看已归还的图书
        public ActionResult BorrowHistory(Reader reader)
        {
            if (reader.Id == 0)
            {
                return RedirectToAction("Login", "Reader");
            }
            else
            {
                IEnumerable<Borrow> borrows = repository.Borrows.Where(u => u.ReaderId == reader.Id && u.ReturnTime != null);
                return View(borrows);
            }
        }

        //归还图书
        public ActionResult ReturnBook(int Id, Reader reader)
        { 
            Book book = repository.FindBookById(Id);
            Borrow borrow = repository.Borrows.FirstOrDefault(br => br.BookId == book.Id && br.Reader.Id == reader.Id && br.ReturnTime == null);
            //如果逾期，需要先缴罚款
            if (borrow.DateShouldBeReturn < DateTime.Now)
            {
                //前台reader修改
                Borrow br = repository.Borrows.FirstOrDefault(b => b.BookId==Id&&b.ReaderId==reader.Id&&b.ReturnTime==null);
                reader.Price = reader.Price - (Decimal)(DateTime.Now.Day - br.DateShouldBeReturn.Day);
                HttpContext.Session["Reader"] = reader;
                borrowProcessor.ProcessPayFine(reader, borrow.Fine.Id);
            }
            borrowProcessor.ProcessReturn(book, reader);
            return RedirectToAction("Borrowing", reader);
        }

        //确认续借
        public ActionResult SureAgain(int Id)
        {
            Book book = repository.FindBookById(Id);
            return View(book);
        }

        //续借 
        public ActionResult BorrowAgain(int Id, Reader reader)
        {
            Book book = repository.FindBookById(Id);
            if (repository.Borrows.FirstOrDefault(b => b.BookId == book.Id && b.ReaderId == reader.Id).WhetherToRenew == 3)
            {
                TempData["msg"] = string.Format("抱歉,'{0}'的续借上限次数为三次", book.Name);
            }
            else
            {
                borrowProcessor.ProcessBorrowAgain(book, reader);
            }          
            return RedirectToAction("Borrowing", reader);
        }


        //我的罚款
        /* 
         * 注意返回的模型类为IEnumerable<Borrow>,而非IEnumerable<Fine>
         */
        public ActionResult MyFine(Reader reader, string returnUrl)
        {
            if (reader.Id == 0)
            {
                return RedirectToAction("Login", "Reader");
            }
            else
            {
                borrowProcessor.ProcessShowFine(reader);
                IEnumerable<Borrow> borrows = repository.Borrows.Where(u => u.ReaderId == reader.Id && u.DateShouldBeReturn < DateTime.Now);
                FineViewModel fv = new FineViewModel(borrows);
                return View(fv);
            }
        }

        //确认缴纳罚款
        /*
         * Id为Fine的Id
         */
        public ActionResult SureFine(Reader reader, int Id)
        {
            Borrow br = repository.Borrows.FirstOrDefault(u => u.FineId == Id);
            return View(br);
        }

        //缴纳罚款
        public ActionResult PayFine(Reader reader, int Id)
        {
            //前台reader修改
            Borrow br = repository.Borrows.FirstOrDefault(b => b.FineId == Id);
            reader.Price = reader.Price - (Decimal)(DateTime.Now.Day - br.DateShouldBeReturn.Day);
            HttpContext.Session["Reader"] = reader;
            //后台reader修改
            /*
             * 先缴款
             * 再归还
             */
            borrowProcessor.ProcessPayFine(reader, Id);
            borrowProcessor.ProcessReturn(br.Book, reader);
            return RedirectToAction("MyFine", reader);
        }


    }
}

