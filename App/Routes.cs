using Microsoft.AspNetCore.Http;
using Datasilk.Core.Web;

namespace Kandu
{
    public class Routes : Datasilk.Core.Web.Routes
    {
        public override IController FromControllerRoutes(HttpContext context, Parameters parameters, string name)
        {
            switch (name)
            {
                case "": case "home": return new Controllers.Home();
                case "login": return new Controllers.Login();
                case "boards": return new Controllers.Boards();
                case "board": return new Controllers.Board();
                case "import": return new Controllers.Import();
            }
            return null;
        }

        public override IService FromServiceRoutes(HttpContext context, Parameters parameters, string name)
        {
            return null;
        }
    }
}