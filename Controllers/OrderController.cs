using Basket.Data;
using Basket.Models;
using Basket.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZarinpalSandbox;

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

        public IActionResult ShowOrder()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Order order_data = _context.Orders.SingleOrDefault(o => o.UserId == UserId && o.isFinally == false);

            List<ShowOrderViewModel> FaktorModel = new List<ShowOrderViewModel>();

            if (order_data != null) //faktor baz dara
            {
                var details = _context.OrderDetails.Where(od => od.OrderId == order_data.OrderId).ToList();
                foreach (var item in details)
                {
                    var product_data = _context.Products.Find(item.ProductId);

                    FaktorModel.Add(new ShowOrderViewModel
                    {
                        Count = item.Count,
                        ImageName = product_data.ImageName,
                        Title = product_data.Title,
                        OrderDetailid = item.OrderDetailId,
                        price = item.Price,
                        Sum = (item.Count * item.Price)
                    });
                }
            }

            return View(FaktorModel);
        }


        public IActionResult Delete_Order(int id)
        {
            var orderdetails = _context.OrderDetails.Find(id);
            
                _context.OrderDetails.Remove(orderdetails);
                _context.SaveChanges();

            return RedirectToAction("ShowOrder");
        }

        public IActionResult Command_Order(int id,string command)
        {
            var orderdetails = _context.OrderDetails.Find(id);

            switch (command)
            {
                case "up":
                    {
                        orderdetails.Count += 1;
                        _context.OrderDetails.Update(orderdetails);
                        break;
                    }
                case "down":
                    {
                        orderdetails.Count -= 1;
                        if (orderdetails.Count == 0)
                        {
                            _context.OrderDetails.Remove(orderdetails);
                        }
                        else
                        {
                            _context.OrderDetails.Update(orderdetails);
                        }
                        break;
                    }
            }
            _context.SaveChanges();

            return RedirectToAction("ShowOrder");
        }

       public IActionResult Payment()
        {
            var order_data = _context.Orders.SingleOrDefault(o => o.isFinally == false);
            if (order_data == null)
            {
                return NotFound();
            }

            var payment = new Payment(order_data.SumOrder);
            //request to zarin pal
            var res = payment.PaymentRequest($"Pardakht faktor shoamre: {order_data.OrderId}", $"http://localhost:5003/Home/OnlinePayment/{order_data.OrderId}","mohsen.1408@gmail.com","09126097035");
            if (res.Result.Status == 100) // yani ok payment is ok
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/"+res.Result.Authority);
            }
            else
            {
                return BadRequest();
            }
            
            return View();
        }
    }
}
