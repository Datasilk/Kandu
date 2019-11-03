namespace Kandu
{
    public class Request : Datasilk.Core.Web.Request
    {
        private User user;
        public User User
        {
            get
            {
                if (user == null)
                {
                    user = User.Get(Context);
                }
                return user;
            }
        }
    }
}
