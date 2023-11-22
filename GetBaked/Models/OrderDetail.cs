using System.ComponentModel.DataAnnotations;

namespace GetBaked.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        // parent refs
        public Product? Product { get; set; }
        public Order? Order { get; set; }
    }
}
