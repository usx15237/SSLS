using SSLS.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace SSLS.Domain.Abstract
{ 
    public interface IOrderProcessor
    {


        void ProcessBorrow(List<Book> books,Reader reader);
        void ProcessFine(List<Book> books);
        void ProcessReturn(Book book, Reader reader);
        void ProcessBorrowAgain(Book book, Reader reader);
        bool Norepeat(Book book,Reader reader);
        void ProcessShowFine(Reader reader);
        void ProcessPayFine(Reader reader,int Id);
        void UpdatePassword(Reader reader,string password);
        void UpdateBalance(Reader reader,decimal price);
        void Register(RegisterDetail register);
    }
    
}
