using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSLS.Domain.Concrete
{
    public class Shelf
    {
        private List<ShelfLine> lineCollection = new List<ShelfLine>();

        public void AddItem(Book book)
        {
            ShelfLine line = lineCollection.Where(e => e.Book.Id == book.Id).FirstOrDefault();
            if (line == null)
            {
                lineCollection.Add(new ShelfLine {   Book = book });
            }
        }
        public IEnumerable<ShelfLine> lines
        {
            get { return lineCollection; }
        }

        public void RemoveLine(Book book)
        {
            lineCollection.RemoveAll(e => e.Book.Id == book.Id);
        }

        public bool Norepeat(Book book)
        {
            if (lines.Where(b => b.Book.Id == book.Id).Count() == 0)
            {
                return true;
            }
                
            else
            {
                return false;

            }
              
        }

        public void Clear()
        {
            lineCollection.Clear();
        }
    }
}
