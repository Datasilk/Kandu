namespace Kandu
{
    public class Page : Datasilk.Page
    {
        private Kandu.User _user = null;

        public User UserInfo
        {
            get
            {
                if (_user == null)
                {
                    _user = new Kandu.User(S);
                }
                return _user;
            }
        }

        public Page(global::Core DatasilkCore) : base(DatasilkCore) {
            title = "Kandu";
            description = "You can do everything you ever wanted";
            
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if (scripts.IndexOf("S.svg.load") < 0)
            {
                scripts += "<script language=\"javascript\">S.svg.load('/themes/default/icons.svg');</script>";
            }
            return base.Render(path, body, metadata);
        }

        public void LoadHeader(ref Scaffold scaffold, bool hasMenu = true)
        {
            UserInfo.Start();
            var service = new Services.Boards(S);

            if(S.User.userId > 0)
            {
                scaffold.Child("header").Data["user"] = "1";
                scaffold.Child("header").Data["boards-menu"] = service.BoardsMenu();

                if (S.User.photo == true)
                {
                    scaffold.Child("header").Data["user-photo"] = "/users/" + S.Util.Str.DateFolders(S.User.datecreated) + "/photo.jpg";
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
                    if (UserInfo.Settings.keepMenuOpen == true)
                    {
                        scripts += "<script language=\"javascript\">S.head.boards.show();S.head.boards.alwaysShow(true);</script>";
                    }
                }
            }
            else
            {
                scaffold.Child("header").Data["no-user"] = "1";
            }

        }
    }
}