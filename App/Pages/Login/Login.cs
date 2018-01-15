using System.Collections.Generic;
using System.Linq;

namespace Kandu.Pages
{
    public class Login: Page
    {
        public Login(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if(S.User.userId > 0)
            {
                //redirect to dashboard
                return base.Render(path, Redirect("/boards/"));
            }

            //check for database reset
            var scaffold = new Scaffold("/Pages/Login/login.html", S.Server.Scaffold);

            if(S.Server.environment == Server.enumEnvironment.development && S.Server.hasAdmin == false)
            {
                //load new administrator form
                scaffold = new Scaffold("/Pages/Login/new-admin.html", S.Server.Scaffold);
                scaffold.Data["title"] = "Create an administrator account";
                scripts += "<script src=\"/js/pages/login/new-admin.js\"></script>";
            }
            else if (S.Server.environment == Server.enumEnvironment.development && S.User.resetPass == true)
            {
                //load new password form (for admin only)
                scaffold = new Scaffold("/Pages/Login/new-pass.html", S.Server.Scaffold);
                scaffold.Data["title"] = "Create an administrator password";
                scripts += "<script src=\"/js/pages/login/new-pass.js\"></script>";
            }
            else
            {
                //load login form (default)
                scripts += "<script src=\"/js/pages/login/login.js\"></script>";
            }

            //load login page
            return base.Render(path, scaffold.Render());
        }
    }
}
