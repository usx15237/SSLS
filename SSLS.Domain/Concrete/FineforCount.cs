using SSLS.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SSLS.Domain.Abstract;

namespace SSLS.WebUI.Models
{
    public class FineforCount
    {
        private IBooksReopository repository;

        public AnalyzeModel Sum { get; set; }
        public AnalyzeModel ToFine { get; set; }
        public AnalyzeModel Fined { get; set; }

        public FineforCount(IBooksReopository repository)
        {
            this.repository = repository;
            IQueryable<Borrow> borrowsSum = this.repository.Borrows.Where(br => br.DateShouldBeReturn < DateTime.Now);
            IQueryable<Borrow> ToFine = this.repository.Borrows.Where(br => br.DateShouldBeReturn < DateTime.Now&& br.ReturnTime == null);
            IQueryable<Borrow> Fined = this.repository.Borrows.Where(br => br.DateShouldBeReturn < DateTime.Now && br.ReturnTime!= null);
            this.Sum=FineforCount.GetProp(borrowsSum,repository);
            this.ToFine = FineforCount.GetProp(ToFine, repository);
            this.Fined = FineforCount.GetProp(Fined, repository);
        }

        public static AnalyzeModel GetProp(IQueryable<Borrow> borrows, IBooksReopository repository)
        {
            List<string> names = new List<string>();
            List<int> values = new List<int>();
            List<AnalyzeItem> analyzeItems = new List<AnalyzeItem>();

            IQueryable<Reader> Readers = repository.Readers;

            foreach (Reader b in Readers.ToList())
            {
                var analyzeItem = new AnalyzeItem();
                var val = borrows.Where(br => br.ReaderId == b.Id).Count();
                analyzeItem.name = b.Name;
                analyzeItem.value = val;
                names.Add(b.Name);
                values.Add(val);
                analyzeItems.Add(analyzeItem);
            }
            AnalyzeModel Sum = new AnalyzeModel
            {
                Values = values,
                Names = names,
                AnalyzeItems = analyzeItems
            };
            return Sum;
        }

    }
}