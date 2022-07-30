using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTesting.Models
{
   
    public class Category
    {
        public int id { get; set; }
        [Display(Name = "Category Name")]
        public string categoryName { get; set; }
    }
    
    public class Product
    {
        public int id { get; set; }
        [Display(Name = "Product Name")]
        public string productName { get; set; }
        [Display(Name = "Product Description")]
        public string productDesc { get; set; }
        [ForeignKey("category"), Display(Name = "Category")]
        public int categoryId { get; set; }
        public Category category { get; set; }
        [Display(Name = "Product Price")]
        public int productPrice { get; set; }
        [Display(Name = "Product Image")]
        [NotMapped]
        public IFormFile imageFile { get; set; }
        [Display(Name = "Prod-Image")]
        public string imageURL { get; set; }
        [Display(Name = "Select")]
        public bool check { get; set; }
    }
    public class Inventory
    {
        public int id { get; set; }
        [Display(Name = "Quantity")]
        public int quantity { get; set; }
        [Display(Name = "Re-Order Level")]
        public int reOrderLevel { get; set; }
        [ForeignKey("category"), Display(Name = "Category")]
        public int categoryId { get; set; }
        public Category category { get; set; }
        [ForeignKey("product"), Display(Name = "Product")]
        public int productId { get; set; }
        public Product product { get; set; }
    }
    public class Cart
    {
        public int id { get; set; }
        [Display(Name = "Date & Time")]
        public DateTime timeStamp { get; set; }
        [Display(Name = "User-Name")]
        public string userName { get; set; }
        [Display(Name = "Total Price")]
        public float totalCartPrice { get; set; }
        [Display(Name = "Quantity")]
        public int productQuantity { get; set; }
        [ForeignKey("category"), Display(Name = "Category")]
        public int categoryId { get; set; }
        public Category category { get; set; }
        [ForeignKey("product"), Display(Name = "Product")]
        public int productId { get; set; }
        public Product product { get; set; }
    }
}
