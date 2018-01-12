using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSLS.Domain.Abstract;
using SSLS.Domain.Concrete;

namespace SSLS.Domain.Concrete
{
    public class EFBookRepository : IBooksReopository
    {
        private 校园自助图书管理系统Entities db = new 校园自助图书管理系统Entities();
        public IQueryable<Book> Books { get { return db.Book; } }
        public IQueryable<Borrow> Borrows { get { return db.Borrow; } }
        public IQueryable<Category> Categories { get { return db.Category; } }
        public IQueryable<Class> Classes { get { return db.Class; } }
        public IQueryable<Reader> Readers{ get { return db.Reader; }}
        public IQueryable<Fine> Fines { get { return db.Fine; } }


        public void UpdateBook(Book Book)
        {
            if (Book.Id == 0)
            {
                db.Book.Add(Book);
            }
            else
            {
                Book dbEntry = db.Book.Find(Book.Id);
                if (dbEntry != null)
                {
                    dbEntry.Code = Book.Code;
                    dbEntry.AUthors = Book.AUthors;
                    dbEntry.Name = Book.Name;
                    dbEntry.CategoryId = Book.CategoryId;
                    dbEntry.Price = Book.Price;
                    dbEntry.Description = Book.Description;
                    dbEntry.ImageUrl = Book.ImageUrl;
                    dbEntry.Press = Book.Press;
                    dbEntry.PublishDate = Book.PublishDate;
                    dbEntry.Status = Book.Status;
                }
            }
            db.SaveChanges();
        }


        public void UpdateReader(Reader reader)
        {
            if (reader.Id == 0)
            {
                db.Reader.Add(reader);
            }
            else
            {
                Reader dbEntry = db.Reader.Find(reader.Id);
                if (dbEntry != null)
                {
                    dbEntry.Email = reader.Email;
                    dbEntry.ClassId = reader.ClassId;
                    dbEntry.Name = reader.Name;
                    dbEntry.Password = reader.Password;
                    dbEntry.Price = reader.Price;
                }
            }
            db.SaveChanges();
        }

        public Book DeleteBook(int id)
        {
            Book dbEntry = db.Book.Find(id);
            if (dbEntry != null)
            {
                db.Book.Remove(dbEntry);
                db.SaveChanges();
            }
            return dbEntry;
        }
        public Book FindBookById(int id)
        {
            Book dbEntry = db.Book.Find(id);
            return dbEntry;
        }

        /*public IQueryable<Book> FindBooksByCategoryId(int categoryId)
        {
            return  db.Book.Where(p => p.CategoryId == categoryId);
        }*/

        public void SaveCategory(Category category)
        {
            if (category.Id == 0)
            {
                db.Category.Add(category);
            }
            else
            {
                Category dbEntry = db.Category.Find(category.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = category.Name;

                }
            }
            db.SaveChanges();
        }
        public Category DeleteCategory(int id)
        {
            Category dbEntry = db.Category.Find(id);
            if (dbEntry != null)
            {
                db.Category.Remove(dbEntry);
                db.SaveChanges();
            }
            return dbEntry;
        }
    }
}
