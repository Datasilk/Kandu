using Microsoft.AspNetCore.Http;
using Utility.Strings;

namespace Kandu
{
    public class Page : Datasilk.Page
    {

        public Page(HttpContext context) : base(context)
        {
            title = "Kandu";
            description = "You can do everything you ever wanted";
            
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            scripts.Append("<script language=\"javascript\">S.svg.load('/themes/default/icons.svg');</script>");
            return base.Render(path, body, metadata);
        }

        public void LoadHeader(ref Scaffold scaffold, bool hasMenu = true)
        {
            if(User.userId > 0)
            {
                scaffold.Child("header").Data["user"] = "1";
                scaffold.Child("header").Data["boards-menu"] = Common.Platform.Boards.RenderBoardsMenu(this);

                if (User.photo == true)
                {
                    scaffold.Child("header").Data["user-photo"] = "/users/" + FileSystem.DateFolders(User.datecreated) + "/photo.jpg";
                }
                else
                {
                    scaffold.Child("header").Data["no-user"] = "1";
                }

                //apply user settings to UI layout configuration
                if(hasMenu == true)
                {
                    scaffold.Child("header").Data["boards"] = "1";
                    scaffold.Child("header").Data["boards-2"] = "1";
                    if (User.keepMenuOpen == true)
                    {
                        scripts.Append("<script language=\"javascript\">S.head.boards.show();S.head.boards.alwaysShow(true);</script>");
                    }
                }
            }
            else
            {
                scaffold.Child("header").Data["no-user"] = "1";
            }

        }

        public void LoadPartial(ref Page page)
        {
            page.scripts.Append(scripts.ToString());
            page.headCss.Append(headCss.ToString());
        }
    }
}