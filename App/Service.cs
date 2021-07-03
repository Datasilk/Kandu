using Kandu.Core;

namespace Kandu
{
    public class Service : Core.Service
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
    }
}