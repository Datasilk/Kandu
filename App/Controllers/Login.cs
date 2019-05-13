using Microsoft.AspNetCore.Http;

namespace Kandu.Controllers
{
    public class Login: Controller
    {
        public Login(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if(User.userId > 0)
            {
                //redirect to dashboard
                return base.Render(path, Redirect("/boards/"));
            }

            //check for database reset
            var scaffold = new Scaffold("/Views/Login/login.html");

            if(Server.environment == Server.Environment.development && Server.hasAdmin == false)
            {
                //load new administrator form
                scaffold = new Scaffold("/Views/Login/new-admin.html");
                scaffold.Data["title"] = "Create an administrator account";
                scripts.Append("<script src=\"/js/views/login/new-admin.js\"></script>");
            }
            else if (Server.environment == Server.Environment.development && User.resetPass == true)
            {
                //load new password form (for admin only)
                scaffold = new Scaffold("/Views/Login/new-pass.html");
                scaffold.Data["title"] = "Create an administrator password";
                scripts.Append("<script src=\"/js/views/login/new-pass.js\"></script>");
            }
            else
            {
                //load login form (default)
                scripts.Append("<script src=\"/js/views/login/login.js\"></script>");
            }

            //load login page
            return base.Render(path, scaffold.Render());
        }
    }
}
