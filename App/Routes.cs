using Microsoft.AspNetCore.Http;
using Datasilk;

public class Routes: Datasilk.Routes
{
    public Routes(HttpContext context) : base(context){ }
    public override Page FromPageRoutes(string name)
    {
        switch (name)
        {
            case "": case "home": return new Kandu.Pages.Home(context);
            case "login": return new Kandu.Pages.Login(context);
            case "boards": return new Kandu.Pages.Boards(context);
            case "board": return new Kandu.Pages.Board(context);
            case "import": return new Kandu.Pages.Import(context);
        }
        return null;

    }

    public override Service FromServiceRoutes(string name)
    {
        return null;
    }
}
