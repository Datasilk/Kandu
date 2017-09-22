using Microsoft.AspNetCore.Mvc;

namespace Kandu.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
