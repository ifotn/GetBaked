using System.ComponentModel.DataAnnotations;

namespace GetBaked.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }

        // ms currency format: {0:c}
        // 0 - represents the value we are formatting
        // c is ms shorthand for currency

        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }
        
        public int CategoryId { get; set; }
        public string? Photo { get; set; }

        // parent reference - connection to the 1 category that a specific product belongs to
        public Category? Category { get; set; }
        
        // child references (Product is the parent in these other relationships)
        public List<CartItem>? CartItems { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
