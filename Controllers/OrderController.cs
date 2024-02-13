using Basket.Data;
using Basket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basket.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public void UpdateSum_Order(int Orderid)
        {
            var order_data = _context.Orders.Find(Orderid);
            order_data.SumOrder = _context.OrderDetails
                .Where(o => o.OrderId == order_data.OrderId)
                .Select(o => o.Count * o.Price)
                .Sum();

            _context.Orders.Update(order_data);
            _context.SaveChanges();
        }
        public IActionResult AddToCard(int id)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // chekc in user is exists order ke isfinally == true ????, 
            // check kon faktor baz darad ya na agar nadarad ye new beasz 
            // agar darad in sefaresh new ro be hamoon order ezafe kon

            Order order = _context.Orders.SingleOrDefault(o => o.UserId == UserId && o.isFinally == false);
            if (order == null) // new order
            {
                order = new Order()
                {
                    CreateDate = DateTime.Now,
                    isFinally = false,
                    UserId = UserId,
                    SumOrder = 0

                };

                _context.Orders.Add(order);
                _context.SaveChanges();
                _context.OrderDetails.Add(new OrderDetail() {
                    OrderId = order.OrderId,
                    Count = 1,
                    Price = _context.Products.Find(id).Price,
                    ProductId = id
                });
                _context.SaveChanges();
            }

            // order baz darad
            else {
                // chekc order detail ke az oon product dari ya na
                var order_detail = _context.OrderDetails.SingleOrDefault(d => d.ProductId == id && d.OrderId == order.OrderId);

                if (order_detail == null) // yani too riz order az in product nabood
                {
                    _context.OrderDetails.Add(new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        Count = 1,
                        Price = _context.Products.Find(id).Price,
                        ProductId = id
                    });
                }
                else // yani az oon product dobar sefaresh dade
                {
                    order_detail.Count += 1; // yani ye doone ezafe kon
                    _context.OrderDetails.Update(order_detail);
                }

                _context.SaveChanges();
            }

            // update order price
            UpdateSum_Order(order.OrderId);

            return Redirect("/");
        }


        
    }
}
