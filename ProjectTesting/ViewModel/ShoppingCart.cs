using ProjectTesting.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectTesting.ViewModel {
    public class ShoppingCart {
        public Product Product { get; set; }
        [Range(1,100,ErrorMessage="Please enter a value between 1 to 100")]
        public int Count { get; set; }
    }
}
