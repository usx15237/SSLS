using SSLS.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSLS.WebUI.Models 
{
    
        public class CurrentReaderModel
        {
           //当前用户
            public Reader Reader { get; set; }
            //当前用户的当前购物车
            public Shelf Shelf { get; set; }
            public string ReturnUrl { get; set; }
            //当前用户的罚款表
            public IEnumerable<Fine> Fines { get; set; }
            

    }
}