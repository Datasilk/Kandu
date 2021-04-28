﻿using System.Collections.Generic;
using System.Text;
using Datasilk.Core.Web;
using Utility.Strings;

namespace Kandu
{
    public class Controller : Request, IController
    {
        public bool UsePlatform = true;
        public string Title = "Kandu";
        public string Description = "";
        public string Favicon = "/images/favicon.png";
        public string Theme = "default";
        public StringBuilder Scripts { get; set; } = new StringBuilder();
        public StringBuilder Css { get; set; } = new StringBuilder();
        private List<string> Resources = new List<string>();

        public virtual void Init() { }

        public override void Dispose()
        {
            base.Dispose();
            User.Save();
        }

        public virtual string Render(string body = "")
        {
            if (UsePlatform == true)
            {
                Scripts.Append("<script language=\"javascript\">S.svg.load('/themes/default/icons.svg?v=" + Server.Version + "');</script>");
            }
            var view = new View("/Views/Shared/layout.html");
            view["title"] = Title;
            view["description"] = Description;
            view["theme"] = Theme;
            view["head-css"] = Css.ToString();
            view["favicon"] = Favicon;
            view["body"] = body;
            if (UsePlatform)
            {
                view.Show("platform-1");
                view.Show("platform-2");
                view.Show("platform-3");
            }

            //add initialization script
            view["scripts"] = Scripts.ToString();

            return view.Render();
        }

        public void LoadHeader(ref View view, bool hasMenu = true)
        {
            if(User.userId > 0)
            {
                view.Child("header").Show("user");
                view.Child("header")["boards-menu"] = Common.Platform.Boards.RenderBoardsMenu(this);

                if (User.photo == true)
                {
                    view.Child("header")["user-photo"] = "/users/" + FileSystem.DateFolders(User.datecreated) + "/photo.jpg";
                }
                else
                {
                    view.Child("header").Show("no-user");
                }

                //apply user settings to UI layout configuration
                if(hasMenu == true)
                {
                    view.Child("header").Show("boards");
                    view.Child("header").Show("boards-2");
                    if (User.keepMenuOpen == true)
                    {
                        Scripts.Append("<script language=\"javascript\">S.head.boards.show();S.head.boards.alwaysShow(true);</script>");
                    }
                }
            }
            else
            {
                view.Child("header").Show("no-user");
            }

        }

        public void LoadPartial(ref Controller page)
        {
            page.Scripts.Append(Scripts.ToString());
            page.Css.Append(Css.ToString());
        }

        public bool CheckSecurity()
        {
            if (User.userId > 0)
            {
                return true;
            }
            return false;
        }

        public string AccessDenied<T>() where T : Datasilk.Core.Web.IController
        {
            return Datasilk.Core.Web.IController.AccessDenied<T>(this);
        }

        public virtual string AccessDenied()
        {
            throw new System.NotImplementedException();
        }

        public string Error<T>() where T : Datasilk.Core.Web.IController
        {
            Context.Response.StatusCode = 500;
            return Datasilk.Core.Web.IController.Error<T>(this);
        }

        public string Error(string message = "Error 500")
        {
            Context.Response.StatusCode = 500;
            return message;
        }

        public string Error404<T>() where T : Datasilk.Core.Web.IController
        {
            Context.Response.StatusCode = 404;
            return Datasilk.Core.Web.IController.Error404<T>(this);
        }

        public string Error404(string message = "Error 404")
        {
            Context.Response.StatusCode = 404;
            return message;
        }

        public string Redirect(string url)
        {
            return "<script language=\"javascript\">window.location.href = '" + url + "';</script>";
        }

        public void AddScript(string url, string id = "", string callback = "")
        {
            if (ContainsResource(url)) { return; }
            Scripts.Append("<script language=\"javascript\"" + (id != "" ? " id=\"" + id + "\"" : "") + " src=\"" + url + "\"" +
                (callback != "" ? " onload=\"" + callback + "\"" : "") + "></script>");
        }

        public void AddCSS(string url, string id = "")
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