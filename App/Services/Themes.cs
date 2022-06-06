namespace Kandu.Services
{
    public class Themes : Service
    {

        public string RefreshListMenu()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Themes.RenderList(this);
        }

        public string Change(string name)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            Query.Users.UpdateTheme(User.UserId, name.ToLower());
            User.Theme = name.ToLower();
            User.Save(true);
            return Success();
        }
    }
}
