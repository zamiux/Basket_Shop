using System.ComponentModel.DataAnnotations;

namespace Basket.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public int SumOrder { get; set; }

        public bool isFinally { get; set; }

        #region Relations
        public List<OrderDetail> OrderDetails { get; set; }
        #endregion
    }
}
