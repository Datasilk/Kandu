using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Kandu
{
    public class Core
    {
        public Server Server;
        public Utility.Util Util;
        public User User;
        public HttpContext Context;
        public HttpRequest Request;
        public HttpResponse Response;
        public ISession Session;
        private string _connString = "";

        public Core(Server server, HttpContext context)
        {
            Server = server;
            Util = server.Util;
            Context = context;
            Request = context.Request;
            Response = context.Response;
            Session = context.Session;
            User = new User();

            //load user session
            if (Session.Get("user") != null)
            {
                User = (User)Util.Serializer.ReadObject(Util.Str.GetString(Session.Get("user")), User.GetType());
            }
            User.Init(this);
        }

        public void Unload()
        {
            if (User.saveSession == true)
            {
                Session.Set("user", Util.Serializer.WriteObject(User));
            }
        }

        public string SqlConnectionString{
            get {
                if(_connString == "")
                {
                    var config = new ConfigurationBuilder()
                    .AddJsonFile(Server.MapPath("config.json"))
                    .Build();

                    var sqlActive = config.GetSection("Data:Active").Value;
                    _connString = config.GetSection("Data:" + sqlActive).Value;
                }
                return _connString;
            }
        }
    }

}
