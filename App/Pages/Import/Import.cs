using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kandu.Pages
{
    public class Import : Page
    {
        public Import(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            Datasilk.Page page = null;
            switch (path[1].ToLower())
            {
                case "trello":
                    page = new Imports.Trello(S);
                    break;
                    
            }
            if(page == null) { return Error404(); }
            LoadPartial(ref page);
            return page.Render(path, body, metadata);
        }
    }
}
