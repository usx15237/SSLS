using SSLS.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSLS.WebUI.Models
{
    public class ReaderandBorrowItem
    {
        public Reader reader;
        public List<Borrow> borrows;
    }
}