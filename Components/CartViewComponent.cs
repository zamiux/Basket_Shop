using Basket.Data;
using Basket.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basket.Components
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<ShowCartViewModel> showCarts = new List<ShowCartViewModel>();

            if (User.Identity.IsAuthenticated == true)
            {
                string UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //check faktor baz
                var order_data = _context.Orders.SingleOrDefault(o => o.isFinally == false && o.UserId == UserId);
                if (order_data != null) //faktor baz dara
                {
                    var details = _context.OrderDetails.Where(od => od.OrderId == order_data.OrderId).ToList();
                    foreach (var item in details)
                    {
                        var product = _context.Products.Find(item.ProductId);

                        showCarts.Add(new ShowCartViewModel { 
                            Count = item.Count,
                            ImageName = item.Product.ImageName,
                            Title = item.Product.Title
                        });
                    }
                }
            }

            return View("Cart", showCarts);
        }
    }
}
