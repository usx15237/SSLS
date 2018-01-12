using SSLS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSLS.Domain.Concrete
{
    public class DatabaseOrderProcessor : IOrderProcessor
    {
        private 校园自助图书管理系统Entities db = new 校园自助图书管理系统Entities();

        //借阅业务
        public void ProcessBorrow(List<Book> books, Reader reader)
        {
            //db是一个具有各项模型属性的上下文类
            using (var db = new 校园自助图书管理系统Entities())
            {
                //创建事务
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var book in books)
                        {
                            //插入新Borrow数据
                            Borrow borrow = new Borrow();
                            borrow.BookId = book.Id;
                            borrow.ReaderId = reader.Id;
                            borrow.BorrowTime = DateTime.Now;
                            borrow.DateShouldBeReturn = DateTime.Now.AddMonths(1);
                            borrow.FineId = db.Fine.Max(f => f.Id);
                            borrow.WhetherToRenew = 0;
                            borrow.NeedtoFine = "暂无需要";
                            borrow.Overdays = 0;

                            db.Borrow.Add(borrow);
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {

                        dbContextTransaction.Rollback();
                    }
                }
            }
        }


        //罚款业务
        public void ProcessFine(List<Book> books)
        {
            //db是一个具有各项模型属性的上下文类
            using (var db = new 校园自助图书管理系统Entities())
            {
                //创建事务
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var book in books)
                        {
                            Fine fine = new Fine();
                            fine.FinePrice = 0;
                            db.Fine.Add(fine);
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }


        //归还业务
        /*
         * 归还业务应该只改写borrow的数据状态，至于罚款业务有专门的ProcessPayFine负责
         */
        public void ProcessReturn(Book book, Reader reader)
        {
            //db是一个具有各项模型属性的上下文类 
            using (var db = new 校园自助图书管理系统Entities())
            {
                //创建事务
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //由于本图书馆不允许一位读者同时有两本相同的书尚未归还，因此borrow唯一
                        Borrow borrow = db.Borrow.FirstOrDefault(br => br.BookId == book.Id && br.Reader.Id == reader.Id && br.ReturnTime == null);
                        borrow.ReturnTime = DateTime.Now;
                        borrow.Overdays = DateTime.Now.Day - borrow.DateShouldBeReturn.Day;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        //续借业务
        public void ProcessBorrowAgain(Book book, Reader reader)
        {
            using (var db = new 校园自助图书管理系统Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //只有归还期限未到的图书才允许续借
                        Borrow borrow = db.Borrow.FirstOrDefault(br => br.BookId == book.Id && br.Reader.Id == reader.Id && br.ReturnTime == null);
                        /*if(borrow.DateShouldBeReturn>DateTime.Now)
                        {*/
                        borrow.WhetherToRenew++;
                        borrow.DateShouldBeReturn = borrow.DateShouldBeReturn.AddMonths(1);
                        /*}*/
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        //查重,判断当前图书是否曾被读者借阅且尚未归还
        public bool Norepeat(Book book, Reader reader)
        {
            if (db.Borrow.Where(br => br.BookId == book.Id && br.ReaderId == reader.Id && br.ReturnTime == null).Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //ProcessShowFine负责查找逾期天数>0的borrow数据，并改写其中NeedtoFine为"暂无需要"的NeedtoFine的数据状态
        /*
         * "暂无需要"表示该条borrow数据未经是否逾期的判断
         * "需缴款"表示该条borrow数据经逾期判断，需要缴款
         * "已缴清"表示该条borrow数据已缴纳罚款并自动归还
         */
        public void ProcessShowFine(Reader reader)
        {
            using (var db = new 校园自助图书管理系统Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    IEnumerable<Borrow> brs = null;
                    try
                    {
                        brs = db.Borrow.Where(u => u.ReaderId == reader.Id && u.DateShouldBeReturn < DateTime.Now);
                        foreach (var br in brs)
                        {
                            br.Overdays = DateTime.Now.Day - br.DateShouldBeReturn.Day;
                            if (br.NeedtoFine == "暂无需要")
                            {
                                br.Fine.FinePrice = (Decimal)(DateTime.Now.Day - br.DateShouldBeReturn.Day);
                                br.NeedtoFine = "需缴款";
                            }

                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();

                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        //缴纳罚款业务
        /*
         * 根据Id找到数据库中对应的Fine数据行，修改其FinePrice
         */
        public void ProcessPayFine(Reader reader, int Id)
        {
            using (var db = new 校园自助图书管理系统Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Reader re = db.Reader.FirstOrDefault(r => r.Id == reader.Id);
                        Fine fine = db.Fine.FirstOrDefault(f => f.Id == Id);
                        Borrow br = db.Borrow.FirstOrDefault(b => b.FineId == Id);
                        re.Price -= fine.FinePrice;
                        fine.FinePrice = 0;
                        br.NeedtoFine = "已缴清";
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        //修改密码
        public void UpdatePassword(Reader reader, string password)
        {
            //db是一个具有各项模型属性的上下文类
            using (var db = new 校园自助图书管理系统Entities())
            {
                //创建事务
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Reader re = db.Reader.FirstOrDefault(r => r.Id == reader.Id);
                        re.Password = password;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        //修改余额
        public void UpdateBalance(Reader reader, decimal price)
        {
            //db是一个具有各项模型属性的上下文类
            using (var db = new 校园自助图书管理系统Entities())
            {
                //创建事务
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Reader re = db.Reader.FirstOrDefault(r => r.Id == reader.Id);
                        re.Price = price;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        //用户注册
        public void Register(RegisterDetail register)
        {
            //db是一个具有各项模型属性的上下文类
            using (var db = new 校园自助图书管理系统Entities())
            {
                //创建事务
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //int maxID = db.Reader.Max(r => r.Id);
                        Reader re = new Reader();
                        re.Name = register.Name;
                        re.Password = register.Password;
                        re.Price = 0;
                        re.ClassId = 1;
                        re.Email = register.Email;
                        //re.Id = maxID + 1;
                        db.Reader.Add(re);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }
    }
}
