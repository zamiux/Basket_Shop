using Basket.Data;
using Basket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZarinpalSandbox;

namespace Basket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {

            return View(_context.Products);
        }

        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                //get authority
                string authority_vall = HttpContext.Request.Query["Authority"].ToString();


                var order_data = _context.Orders.SingleOrDefault(o => o.OrderId == id);
                if (order_data == null)
                {
                    return NotFound();
                }

                var payment = new Payment(order_data.SumOrder);
                var res = payment.Verification(authority_vall).Result;

                if (res.Status == 100) // okeye , pardakht shode
                {
                    order_data.isFinally = true;
                    _context.Orders.Update(order_data);
                    _context.SaveChanges();

                    // shomare peigiri 
                    ViewBag.Code = res.RefId;
                }
                return View();
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}