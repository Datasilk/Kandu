using Microsoft.AspNetCore.Http;

namespace Kandu.Pages
{
    public class Logout : Page
    {
        public Logout(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            User.LogOut();

            return Redirect("/login");
        }
    }
}
