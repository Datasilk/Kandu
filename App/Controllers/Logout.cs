using Microsoft.AspNetCore.Http;

namespace Kandu.Controllers
{
    public class Logout : Controller
    {
        public Logout(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            User.LogOut();

            return Redirect("/login");
        }
    }
}
