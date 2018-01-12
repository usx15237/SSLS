using SSLS.Domain.Abstract;
using SSLS.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSLS.Domain.Concrete
{
    public class AnalyzeModel
    {
        public List<string> Names { set; get; }
        public List<int> Values { set; get; }
        public List<AnalyzeItem> AnalyzeItems { set; get; }
        
        //提供借阅统计数据源
        public static object GetBorrowCount(IBooksReopository repository)
        {

            List<string> names = new List<string>();
            List<int> values = new List<int>();
            List<AnalyzeItem> analyzeItems = new List<AnalyzeItem>();

            IQueryable<Category> Categorys = repository.Categories;

            foreach (Category b in Categorys.ToList())
            {
                var analyzeItem = new AnalyzeItem();
                var val = repository.Borrows.Where(e => e.Book.CategoryId == b.Id).Count();
                analyzeItem.name = b.Name;
                analyzeItem.value = val;
                names.Add(b.Name);
                values.Add(val);
                analyzeItems.Add(analyzeItem);
            }
            AnalyzeModel q = new AnalyzeModel
            {
                Values = values,
                Names = names,
                AnalyzeItems = analyzeItems
            };
            return q;
        }

        //提供图书统计数据源
        public static object GetCategoryCount(IBooksReopository repository)
        {

            List<string> names = new List<string>();
            List<int> values = new List<int>();
            List<AnalyzeItem> analyzeItems = new List<AnalyzeItem>();

            IQueryable<Category> Categories = repository.Categories;

            foreach (Category b in Categories.ToList())
            {
                var analyzeItem = new AnalyzeItem();
                var val = repository.Books.Where(e => e.CategoryId== b.Id).Count();
                analyzeItem.name = b.Name;
                analyzeItem.value = val;
                names.Add(b.Name);
                values.Add(val);
                analyzeItems.Add(analyzeItem);
            }
            AnalyzeModel q = new AnalyzeModel
            {
                Values = values,
                Names = names,
                AnalyzeItems = analyzeItems
            };
            return q;
        }

        //提供罚款统计数据源
        public static object GetFineCount(IBooksReopository repository)
        { 

            FineforCount fc = new FineforCount(repository);
            return fc;
        }
















     
     





























        //提供读者分析数据源
        public static object GetReaderAnalyze(IBooksReopository repository,int readerId)
        {
            List<string> names = new List<string>();
            List<int> values = new List<int>();
            List<AnalyzeItem> analyzeItems = new List<AnalyzeItem>();

            IQueryable<Category> Categories = repository.Categories;

            foreach (Category b in Categories)
            {
                var analyzeItem = new AnalyzeItem();
                var val = repository.Borrows.Where(e => e.ReaderId== readerId && e.Book.CategoryId==b.Id).Count();
                analyzeItem.name = b.Name;
                analyzeItem.value = val;
                names.Add(b.Name);
                values.Add(val);
                analyzeItems.Add(analyzeItem);
            }
            AnalyzeModel q = new AnalyzeModel
            {
                Values = values,
                Names = names,
                AnalyzeItems = analyzeItems
            };
            return q;
        }
        

    }
}
