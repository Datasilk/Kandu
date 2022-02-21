namespace Kandu.Common
{
    public static class Accordion
    {
        public static string Render(string title, string contents, string classname, string icon, string menu, bool expanded)
        {
            var view = new View("/Views/Shared/accordion.html");
            view["title"] = title;
            view["contents"] = contents;
            view["class-name"] = classname;
            view["menu"] = menu;
            view["icon"] = icon;
            if(icon != "")
            {
                view.Show("has-icon");
            }
            if (expanded)
            {
                view.Show("expanded");
            }
            return view.Render();
        }
    }
}
