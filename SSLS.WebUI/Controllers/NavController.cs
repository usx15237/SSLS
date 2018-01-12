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
    public class NavController : Controller
    {
        private IBooksReopository repository;
        private IOrderProcessor borrowProcessor;

        public NavController(IBooksReopository repository, IOrderProcessor borrowProcessor)
        {
            this.repository = repository;
            this.borrowProcessor = borrowProcessor;
        }

        //图书分类分部视图,注意该动作方法返回类型为PartialViewResult
        public PartialViewResult categorySummary(int categoryId = 0)
        {
            ViewBag.CurrentCategoryId = categoryId;
            IEnumerable<Category> categories = repository.Categories.ToList();
            return PartialView(categories);
        }
         

        //用户界面导航分布视图
        public PartialViewResult fontNavSummary(Shelf shelf, Reader reader)
        {
            List<Fine> fines = new List<Fine>();
            foreach (var br in repository.Borrows.Where(b => b.ReaderId == reader.Id))
            {
                if (br.DateShouldBeReturn < DateTime.Now)
                {
                    fines.Add(repository.Fines.FirstOrDefault(f => f.Id == br.Fine.Id));
                }
            } 
            return PartialView(new CurrentReaderModel
            {
                Reader = reader,
                Fines = fines,
                Shelf = shelf,
            });
        }

    }
}