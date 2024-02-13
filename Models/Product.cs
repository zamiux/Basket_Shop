using System.ComponentModel.DataAnnotations;

namespace Basket.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public string ImageName { get; set; }

        #region Relations
        public List<OrderDetail> OrderDetails { get; set; }
        #endregion
    }
}
