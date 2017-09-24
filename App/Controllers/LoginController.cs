using Microsoft.AspNetCore.Mvc;

namespace Kandu.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
                return View();
        }
    }
}
