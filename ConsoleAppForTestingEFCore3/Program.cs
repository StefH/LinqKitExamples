using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleAppForTestingEFCore3
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
    }

    /// <summary> Some simple EF DBContext item for this example</summary>
    public class MyDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("LinqKitEFCore3");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var context = new MyDbContext();
            context.Orders.Add(new Order { OrderDate = DateTime.Now.AddDays(-1).Date, Amount = 1 });
            context.Orders.Add(new Order { OrderDate = DateTime.Now.AddDays(-1).Date, Amount = 2 });
            context.SaveChanges();

            //Expression<Func<IQueryable<Order>, decimal>> expression = orders => orders.Average(o => (decimal)o.Amount);
            Expression<Func<IGrouping<DateTime, Order>, decimal>> expression = orders => orders.Average(o => (decimal)o.Amount);

            IQueryable<Order> orders = context.Orders.AsExpandable();
            var q = from o in orders
                    group o by o.OrderDate into g
                    select new
                    {
                        OrderDate = g.Key,
                        AggregatedAmount = expression.Invoke(g)
                    };

            Console.WriteLine(q.ToString());


            q.ToList().ForEach(x => Console.WriteLine($"{x.OrderDate} : {x.AggregatedAmount}"));
            Console.WriteLine("Hello World!");
        }
    }
}
