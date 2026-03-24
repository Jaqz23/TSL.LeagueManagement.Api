using Microsoft.AspNetCore.Mvc;

namespace TSL.WebApi.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
