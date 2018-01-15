namespace Kandu
{
    public class Service : Datasilk.Service
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

        public Service(global::Core DatasilkCore) : base(DatasilkCore) {}
    }
}