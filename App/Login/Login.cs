namespace Kandu.Pages
{
    public class Login: Page
    {
        public Login(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "")
        {
            if(S.User.userId > 0)
            {
                //redirect to boards list
                return base.Render(path, Redirect("/boards"));
            }

            //check for database reset
            var scaffold = new Scaffold(S, "/Login/login.html");

            if(S.Server.environment == Server.enumEnvironment.development && S.Server.hasAdmin == false)
            {
                //load new administrator form
                scaffold = new Scaffold(S, "/Login/new-admin.html");
                scaffold.Data["title"] = "Create an administrator account";
                scripts += "<script src=\"/js/login/new-admin.js\"></script>";
            }
            else if (S.Server.environment == Server.enumEnvironment.development && S.Server.resetPass == true)
            {
                //load new password form (for admin only)
                scaffold = new Scaffold(S, "/Login/new-pass.html");
                scaffold.Data["title"] = "Create an administrator password";
                scripts += "<script src=\"/js/login/new-pass.js\"></script>";
            }
            else
            {
                //load login form (default)
                scripts += "<script src=\"/js/login/login.js\"></script>";
            }

            //load login page
            return base.Render(path, scaffold.Render());
        }
    }
}
