using Basket.Data;
using Basket.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Basket.Jobs
{
    [DisallowConcurrentExecution]
    public class RemoveCartJob : IJob
    {
        //private readonly ApplicationDbContext _dbContext;

        //public RemoveCartJob(ApplicationDbContext dbContext)
        //{
        //        _dbContext = dbContext;
        //}
        public Task Execute(IJobExecutionContext context)
        {
            var option = new DbContextOptionsBuilder<ApplicationDbContext>();
            option.UseSqlServer(@"Server=.;Database=BasketDB;Integrated Security=True;TrustServerCertificate=True");

            using (ApplicationDbContext _dbContext = new ApplicationDbContext(option.Options))
            {
                var order_data = _dbContext.Orders
                .Where(o => o.isFinally == false && o.CreateDate < DateTime.Now.AddHours(-24))
                .ToList();

                foreach (var order in order_data)
                {
                    //first delete order_details 
                    var orderdetail_data = _dbContext.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                    foreach (var orderDetail in orderdetail_data)
                    {
                        _dbContext.OrderDetails.Remove(orderDetail);
                    }

                    _dbContext.Orders.Remove(order);
                }

                _dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}
