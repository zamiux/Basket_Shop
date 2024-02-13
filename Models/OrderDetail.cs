using System.ComponentModel.DataAnnotations;

namespace Basket.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [Required] 
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Count { get; set; }


        #region Relations
        public Order Order { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
