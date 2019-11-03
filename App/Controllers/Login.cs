namespace Kandu.Controllers
{
    public class Login: Controller
    {
        public override string Render(string body = "")
        {
            if(User.userId > 0)
            {
                //redirect to dashboard
                return base.Render(Redirect("/boards/"));
            }

            //check for database reset
            var view = new Scaffold("/Views/Login/login.html");

            if(Server.environment == Server.Environment.development && Server.hasAdmin == false)
            {
                //load new administrator form
                view = new Scaffold("/Views/Login/new-admin.html");
                view["title"] = "Create an administrator account";
                Scripts.Append("<script src=\"/js/views/login/new-admin.js?v=" + Server.Version + "\"></script>");
            }
            else if (Server.environment == Server.Environment.development && User.resetPass == true)
            {
                //load new password form (for admin only)
                view = new Scaffold("/Views/Login/new-pass.html");
                view["title"] = "Create an administrator password";
                Scripts.Append("<script src=\"/js/views/login/new-pass.js?v=" + Server.Version + "\"></script>");
            }
            else
            {
                //load login form (default)
                Scripts.Append("<script src=\"/js/views/login/login.js?v=" + Server.Version + "\"></script>");
            }

            //load login page
            return base.Render(view.Render());
        }
    }
}
