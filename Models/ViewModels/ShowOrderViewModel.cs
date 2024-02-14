namespace Basket.Models.ViewModels
{
    public class ShowOrderViewModel
    {
        public int OrderDetailid { get; set; }
        public string ImageName { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public int price { get; set; }
        public int Sum { get; set; }
    }
}
