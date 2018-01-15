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
    }
}