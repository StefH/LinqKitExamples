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
            //optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=MyDatabase;Trusted_Connection=True;");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<IQueryable<Order>, decimal?>> expression = orders => orders.Average(o => (decimal?)o.Amount);

            using (var context = new MyDbContext())
            {
                IQueryable<Order> orders = context.Orders;
                var q = from o in orders.AsExpandable()
                        group o by o.OrderDate into g
                        select new
                        {
                            OrderDate = g.Key,
                            AggregatedAmount = expression.Invoke(g.AsQueryable())
                        };
                //ToList or ToListAsync:
                q.ToList().ForEach(Console.WriteLine);
                Console.ReadLine();
            }
        }
    }
}
