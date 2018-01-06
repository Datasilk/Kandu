using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Kandu.Pages
{
    public class Logout : Page
    {
        public Logout(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            S.User.LogOut();

            return Redirect("/login");
        }
    }
}
