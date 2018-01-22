using Datasilk;

public class Routes: Datasilk.Routes
{
    public Routes(Core DatasilkCore) : base(DatasilkCore) {}

    public override Page FromPageRoutes(string name)
    {
        switch (name)
        {
            case "": case "home": return new Kandu.Pages.Home(S);
            case "login": return new Kandu.Pages.Login(S);
            case "boards": return new Kandu.Pages.Boards(S);
            case "board": return new Kandu.Pages.Board(S);
            case "import": return new Kandu.Pages.Import(S);
        }
        return null;

    }

    public override Service FromServiceRoutes(string name)
    {
        return null;
    }
}
