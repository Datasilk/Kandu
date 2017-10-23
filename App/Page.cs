using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kandu
{
    public class Page
    {
        public Core S;
        
        public string title = "Kandu";
        public string description = "";
        public string headCss = "";
        public string colorsCss = "/css/themes/default.css";
        public string favicon = "";
        public string svgIcons = "";
        public string scripts = "";

        public Page(Core KanduCore)
        {
            S = KanduCore;
            svgIcons = S.Server.LoadFileFromCache("/content/themes/default/icons.svg");
        }

        public virtual string Render(string[] path, string body = "", object metadata = null)
        {
            //renders HTML layout
            var scaffold = new Scaffold(S, "/layout.html");
            scaffold.Data["title"] = title;
            scaffold.Data["description"] = description;
            scaffold.Data["head-css"] = headCss;
            scaffold.Data["colors-css"] = colorsCss;
            scaffold.Data["favicon"] = favicon;
            scaffold.Data["svg-icons"] = svgIcons;
            scaffold.Data["body"] = body;

            //add initialization script
            scaffold.Data["scripts"] = scripts;

            return scaffold.Render();
        }

        public string AccessDenied()
        {
            if(S.User.userId <= 0)
            {
                var login = new Pages.Login(S);
                return login.Render(new string[] { });
            }
            return "Access Denied";
        }

        public string Redirect(string url)
        {
            return "<script language=\"javascript\">window.location.href = '" + url + "';</script>";
        }
    }
}
