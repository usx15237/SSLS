using SSLS.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSLS.WebUI.Models
{
    public class FineViewModel
    {
        public List<Borrow> Tofine { get; set; }
        public List<Borrow> Fined { get; set; }

        public FineViewModel(IEnumerable<Borrow> borrows)
        {
            List<Borrow> Tofine = new List<Borrow>();
            List<Borrow> Fined = new List<Borrow>();
            foreach (var br in borrows)
            {
                if (br.NeedtoFine == "需缴款")
                {
                    Tofine.Add(br);
                }
                else
                {
                    Fined.Add(br);
                }   
            }
            this.Fined = Fined;
            this.Tofine = Tofine;
        }
    }
}