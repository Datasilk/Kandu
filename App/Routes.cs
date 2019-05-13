using Microsoft.AspNetCore.Http;
using Datasilk.Mvc;

public class Routes: Datasilk.Web.Routes
{
    public override Controller FromControllerRoutes(HttpContext context, Parameters parameters, string name)
    {
        switch (name)
        {
            case "": case "home": return new Kandu.Controllers.Home(context, parameters);
            case "login": return new Kandu.Controllers.Login(context, parameters);
            case "boards": return new Kandu.Controllers.Boards(context, parameters);
            case "board": return new Kandu.Controllers.Board(context, parameters);
            case "import": return new Kandu.Controllers.Import(context, parameters);
        }
        return null;
    }
}
