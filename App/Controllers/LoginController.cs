using Microsoft.AspNetCore.Mvc;

namespace Kandu.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            var S = new Core(null, HttpContext);

            if (S.Server.environment == Server.enumEnvironment.development && S.Server.resetPass == true)
            {

            }
                return View();
        }
    }
}
