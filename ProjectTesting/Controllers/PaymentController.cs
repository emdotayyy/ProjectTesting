using Microsoft.AspNetCore.Mvc;

namespace ProjectTesting.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
