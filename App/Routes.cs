using Microsoft.AspNetCore.Http;
using Datasilk.Core.Web;

namespace Kandu
{
    public class Routes : Datasilk.Core.Web.Routes
    {
        public override IController FromControllerRoutes(HttpContext context, Parameters parameters, string name)
        {
            if (App.Environment == Environment.development) { ViewCache.Clear(); }
            switch (name)
            {
                //most frequently used routes first
                case "board": return new Controllers.Board();
                case "": case "home": return new Controllers.Home();
                case "login": return new Controllers.Login();
                case "boards": return new Controllers.Boards();

                //dashboard routes
                case "attachment": return new Controllers.Attachment();
                case "upload": return new Controllers.Upload();
                case "invitation": return new Controllers.Invitation();
                case "uploadtheme": return new Controllers.UploadTheme();

                //least used routes
                case "signup": return new Controllers.Signup();
                case "logout": return new Controllers.Logout();
                case "import": return new Controllers.Import();
            }
            return null;
        }

        public override IService FromServiceRoutes(HttpContext context, Parameters parameters, string name)
        {
            if(App.Environment == Environment.development) { ViewCache.Clear(); }
            return null;
        }
    }
}