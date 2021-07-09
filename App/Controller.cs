using Utility.Strings;
using Kandu.Core;

namespace Kandu
{
    public class Controller : Core.Controller
    {
        public override IUser User
        {
            get
            {
                if (user == null)
                {
                    user = Kandu.User.Get(Context);
                }
                return user;
            }
            set { user = value; }
        }

        public override string Render(string body = "")
        {
            if (App.Environment == Environment.development) { ViewCache.Clear(); }
            Scripts.Append("<script language=\"javascript\">S.svg.load('/themes/default/icons.svg?v=" + Server.Version + "');</script>");
            var view = new View("/Views/Shared/layout.html");
            view["title"] = Title;
            view["description"] = Description;
            view["theme"] = Theme;
            view["head-css"] = Css.ToString();
            view["favicon"] = Favicon;
            view["body"] = body;

            //add initialization script
            view["scripts"] = Scripts.ToString();

            return view.Render();
        }

        public new void LoadHeader(ref View view, HasMenu hasMenu = HasMenu.None)
        {
            if(User.UserId > 0)
            {
                //user logged in
                view.Child("header").Show("user");

                if (User.Photo == true)
                {
                    view.Child("header")["user-photo"] = "/users/" + FileSystem.DateFolders(User.DateCreated) + "/photo.jpg";
                }
                else
                {
                    view.Child("header").Show("no-photo");
                }

                if(hasMenu == HasMenu.Boards)
                {
                    //show drop down menu for boards list
                }
                else if (hasMenu == HasMenu.Board)
                {
                    //show drop down menu for board
                    view.Child("header").Show("boards");
                    view.Child("header")["boards-menu"] = Common.Boards.RenderSideBar(this);

                    if (User.KeepMenuOpen == true)
                    {
                        //apply user settings to UI layout configuration
                        Scripts.Append("<script language=\"javascript\">S.head.boards.show();S.head.boards.alwaysShow(true);</script>");
                    }
                }

                //load user menu
                view.Child("header")["user-menu"] = Common.User.RenderUserMenu(this);
                
                //load organization templates
                view.Child("header")["org-menu"] = Common.Organizations.RenderOrgListModal(this);
                view.Child("header")["org-templates"] = Cache.LoadFile("/Views/Organizations/templates.html"); ;
            }
            else
            {
                //user not logged in
                view.Child("header").Show("no-user");
            }

        }

        public void LoadPartial(ref Controller page)
        {
            page.Scripts.Append(Scripts);
            page.Css.Append(Css);
        }

        public bool CheckSecurity()
        {
            if (User.UserId > 0)
            {
                return true;
            }
            return false;
        }

        public override string AccessDenied()
        {
            return AccessDenied<Controllers.Login>();
        }
    }
}