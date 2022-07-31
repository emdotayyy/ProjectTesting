 using Microsoft.AspNetCore.Mvc;
using ProjectTesting.Data;
using ProjectTesting.Helper;
using ProjectTesting.Models;
using Stripe;

namespace ProjectTesting.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }
        [TempData]
        public string TotalAmount { get; set; }
        public IActionResult Index()
        {
            List<Cart> list = _context.carts.ToList();
            ViewData["Cart"] = list;
            ViewData["DollarAmount"] = list.Select(e => e.totalCartPrice);
            //var clist = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "carts");
            //ViewBag.cart = clist;
            //ViewBag.DollarAmount = clist.Select(item => item.totalCartPrice);
            //ViewBag.total = Math.Round(ViewBag.DollarAmount, 2) * 100;
            //ViewBag.total = Convert.ToInt32(ViewBag.DollarAmount);
            int? total = ViewBag.total;
            TotalAmount = total.ToString();
            return View();
        }
        [HttpPost]

        public IActionResult Processing(string stripeToken, string stripeEmail)
        {
            var optionsCust = new CustomerCreateOptions
            {
                Email=stripeEmail,
                Name="Adeeb",
                Phone="04-234567"
            };
            var serviceCust = new CustomerService();
            Customer customer = serviceCust.Create(optionsCust);
            var optiopnscharge = new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(TempData["totalAmount"]),
                Currency="USD",
                Description="Buying Product(s)",
                Source=stripeToken,
                ReceiptEmail=stripeEmail,
            };
            var service = new ChargeService();
            Charge charge = service.Create(optiopnscharge);
            if(charge.Status == "succeeded")
            {
                string balanceTransactionId = charge.BalanceTransactionId;
                ViewBag.AmountPaid = Convert.ToDecimal(charge.Amount) % 100 / 100 + (charge.Amount) * 100;
                ViewBag.BalanceTxId = balanceTransactionId;
                ViewBag.Customer = customer.Name;
            }
            return View();
        }
    }
}
