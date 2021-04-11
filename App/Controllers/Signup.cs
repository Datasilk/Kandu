using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kandu.Controllers
{
    public class Signup : Controller
    {
        public override string Render(string body = "")
        {
            //load signup form
            var view = new View("/Views/Signup/signup.html");
            view["title"] = "Create a new Kandu account";
            Scripts.Append("<script src=\"/js/views/signup/signup.js?v=" + Server.Version + "\"></script>");

            //load signup page
            return base.Render(view.Render());
        }
    }
}
