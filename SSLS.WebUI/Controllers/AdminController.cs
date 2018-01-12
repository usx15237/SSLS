using SSLS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SSLS.Domain.Concrete;
using SSLS.WebUI.Infrastructure;
using SSLS.WebUI.Models;

namespace SSLS.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IBooksReopository repository;
        private IOrderProcessor iorderProcessor;

        public AdminController(IBooksReopository BookRepository, IOrderProcessor iorderProcessor)
        {
            this.repository = BookRepository;
            this.iorderProcessor = iorderProcessor;

        }
        //
        // GET: /Admin/
        public ActionResult Index_book()
        {
            return View(repository.Books.ToList());
        }

        public ActionResult adminNavSummary()
        {
            return PartialView();
        }

        public ActionResult Edit_book(int id)
        {
            Book Book = repository.Books.FirstOrDefault(p => p.Id == id);
            ViewBag.CategoryList = GetCategorySelectList();
            return View(Book);
        }
        [HttpPost]
        public ActionResult Edit_book(Book Book, HttpPostedFileBase file)
        {
            string imageFileName = string.Empty;
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    try
                    {
                        imageFileName = Utils.GetImageSaveName(file.FileName);
                        string savePathAndName = string.Format("{0}/{1}", Server.MapPath("~/PImages"), imageFileName);
                        file.SaveAs(savePathAndName);
                        Book.ImageUrl = string.Format("/PImages/{0}", imageFileName);
                    }

                    catch
                    {
                        ModelState.AddModelError("", "图片保存失败！");
                    }
                }
                repository.UpdateBook(Book);
                TempData["msg"] = string.Format("{0}保存成功", Book.Name);
                return RedirectToAction("Index_book");
            }
            else
            {
                ViewBag.CategoryList = GetCategorySelectList();
                return View(Book);
            }
        }
        public ActionResult Create_book()
        {

            ViewBag.CategoryList = GetCategorySelectList();
            return View("Edit_book", new Book());
        }

        [HttpPost]
        public ActionResult Delete_book(int id)
        {
            Book deleteBook = repository.DeleteBook(id);
            if (deleteBook != null)
            {
                /*
                 * TempData["msg"]用于在控制器间传递数据。
                 * 当某个控制器所指向的视图中使用了TempData["msg"]时,TempData["msg"]清空。
                 * 利用上述特性，我们就可以针对用户不同的操作反馈不同的提示信息
                 * (Delete动作传递删除信息给Index动作,Index的视图页面中通过检测TempData["msg"]来判断是否该弹出提示语句)
                 */

                TempData["msg"] = string.Format("'{0}'信息删除成功", deleteBook.Name);
            }
            return RedirectToAction("Index_book");
        }

        public ActionResult Index_category()
        {
            return View(repository.Categories.ToList());
        }


        public ActionResult Edit_category(int id)
        {
            Category category = repository.Categories.FirstOrDefault(p => p.Id == id);
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit_category(Category category)
        {
            if (ModelState.IsValid)
            {
                repository.SaveCategory(category);
                TempData["msg"] = string.Format("{0}信息保存成功", category.Name);
                return RedirectToAction("Index_category");
            }
            else
            {
                return View(category);
            }
        }

        public ActionResult Create_category()
        {
            ViewBag.CategoryList = GetCategorySelectList();
            return View("Edit2_category", new Category());
        }
        public IEnumerable<SelectListItem> GetCategorySelectList()
        {
            return repository.Categories.ToList().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
        }
        public IEnumerable<SelectListItem> GetClassSelectList()
        {
            return repository.Classes.ToList().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
        }
        [HttpPost]
        public ActionResult Delete_category(int id)
        {
            Category deleteCategory = repository.DeleteCategory(id);
            if (deleteCategory != null)
            {
                TempData["msg"] = string.Format("'{0}'分类信息删除成功", deleteCategory.Name);
            }
            return RedirectToAction("Index_category");
        }

        public ActionResult Index_reader()
        {
            List<Reader> readerList = repository.Readers.ToList();
            List<ReaderandBorrowItem> rbs = new List<ReaderandBorrowItem>();

            foreach (var re in readerList)
            {
                ReaderandBorrowItem item = new ReaderandBorrowItem();
                item.reader = re;
                item.borrows = repository.Borrows.Where(b => b.ReaderId == re.Id).ToList();
                rbs.Add(item);
            }

            return View(new ReadersandBorrrows
            {
                yourRBs = rbs
            });
        }






        //借阅统计
        public ActionResult BorrowCount()
        {
            return View();
        }

        public ActionResult BorrowCount1()
        {
            return Json(AnalyzeModel.GetBorrowCount(repository));
        }

        //类别统计
        public ActionResult CategoryCount()
        {
            return View();
        }

        public ActionResult CategoryCount1()
        {
            return Json(AnalyzeModel.GetCategoryCount(repository));
        }

        //罚款统计
        public ActionResult FineCount()
        {
            return View();
        }

        public ActionResult FineCount1()
        {
            return Json(AnalyzeModel.GetFineCount(repository));
        }

        public ActionResult ToFineCount()
        {
            return View();
        }

        public ActionResult FinedCount()
        {
            return View();
        }
       




        //读者分析
        public ActionResult ReaderAnalyze(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        public ActionResult ReaderAnalyze1(int Id)
        {
            return Json(AnalyzeModel.GetReaderAnalyze(repository, Id));
        }
    }
}