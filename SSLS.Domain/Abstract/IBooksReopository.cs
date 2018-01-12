using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSLS.Domain.Concrete;

namespace SSLS.Domain.Abstract 
{
    public interface IBooksReopository
    {
        IQueryable<Book> Books { get; }
        IQueryable<Category> Categories { get; }
        IQueryable<Reader> Readers { get; }
        IQueryable<Class> Classes{ get; }
        IQueryable<Borrow> Borrows {get;}
        IQueryable<Fine> Fines { get; }

        //改Book
        void UpdateBook(Book book);
        //删Book
        Book DeleteBook(int id);
        //查Book
        Book FindBookById(int id);

        //改Reader
        void UpdateReader(Reader reader);

        /*IQueryable<Book> FindBooksByCategoryId(int categoryId);*/

        //改Category
        void SaveCategory(Category category);
        //删Category
        Category DeleteCategory(int id);
    }
}
