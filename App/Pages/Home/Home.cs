using System;
using System.Text;

namespace Kandu.Pages
{
    public class Home : Page
    {
        public Home(Core LegendaryCore) : base(LegendaryCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();
            html.Append(Redirect("/login"));
            return base.Render(path, html.ToString(), metadata);
        }
    }
}
