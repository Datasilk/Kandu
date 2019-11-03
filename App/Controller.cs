using Datasilk.Core.Web;
using Utility.Strings;

namespace Kandu
{
    public class Controller : Request, IController
    {
        public bool usePlatform = false;
        public string title = "Datasilk";
        public string description = "";
        public string favicon = "/images/favicon.png";
        public string theme = "default";

        public virtual string Render(string body = "")
        {
            if (usePlatform == true)
            {
                Scripts.Append("<script language=\"javascript\">S.svg.load('/themes/default/icons.svg?v=" + Server.Version + "');</script>");
            }
            var view = new View("/Views/Shared/layout.html");
            view["title"] = title;
            view["description"] = description;
            view["theme"] = theme;
            view["head-css"] = Css.ToString();
            view["favicon"] = favicon;
            view["body"] = body;
            if (usePlatform)
            {
                view.Show("platform-1");
                view.Show("platform-2");
                view.Show("platform-3");
            }

            //add initialization script
            view["scripts"] = Scripts.ToString();

            return view.Render();
        }

        public void LoadHeader(ref Scaffold scaffold, bool hasMenu = true)
        {
            if(User.userId > 0)
            {
                scaffold.Child("header").Show("user");
                scaffold.Child("header")["boards-menu"] = Common.Platform.Boards.RenderBoardsMenu(this);

                if (User.photo == true)
                {
                    scaffold.Child("header")["user-photo"] = "/users/" + FileSystem.DateFolders(User.datecreated) + "/photo.jpg";
                }
                else
                {
                    scaffold.Child("header").Show("no-user");
                }

                //apply user settings to UI layout configuration
                if(hasMenu == true)
                {
                    scaffold.Child("header").Show("boards");
                    scaffold.Child("header").Show("boards-2");
                    if (User.keepMenuOpen == true)
                    {
                        Scripts.Append("<script language=\"javascript\">S.head.boards.show();S.head.boards.alwaysShow(true);</script>");
                    }
                }
            }
            else
            {
                scaffold.Child("header").Show("no-user");
            }

        }

        public void LoadPartial(ref Controller page)
        {
            page.Scripts.Append(Scripts.ToString());
            page.Css.Append(Css.ToString());
        }

        public override void Unload()
        {
            User.Save();
        }

        public override bool CheckSecurity()
        {
            if (!base.CheckSecurity())
            {
                return false;
            }
            if (User.userId > 0)
            {
                return true;
            }
            return false;
        }

        public string AccessDenied<T>() where T : IController
        {
            return IController.AccessDenied<T>();
        }

        public string Redirect(string url)
        {
            return "<script language=\"javascript\">window.location.href = '" + url + "';</script>";
        }

        public override void AddScript(string url, string id = "", string callback = "")
        {
            if (ContainsResource(url)) { return; }
            Scripts.Append("<script language=\"javascript\"" + (id != "" ? " id=\"" + id + "\"" : "") + " src=\"" + url + "\"" +
                (callback != "" ? " onload=\"" + callback + "\"" : "") + "></script>");
        }

        public override void AddCSS(string url, string id = "")
        {
            if (ContainsResource(url)) { return; }
            Css.Append("<link rel=\"stylesheet\" type=\"text/css\"" + (id != "" ? " id=\"" + id + "\"" : "") + " href=\"" + url + "\"></link>");
        }

        public bool ContainsResource(string url)
        {
            if (Resources.Contains(url)) { return true; }
            Resources.Add(url);
            return false;
        }
    }
}