using System;
using System.IO;
using Newtonsoft.Json;

namespace Kandu.Pages.Imports
{
    public class Trello : Page
    {
        public Trello(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security

            if (S.Request.QueryString.Value.Contains("?upload"))
            {
                //uploaded json file
                if (Files != null)
                {
                    if (Files.Count > 0)
                    {
                        Models.Trello.Board board = null;
                        using (var stream = Files[0].OpenReadStream())
                        {
                            var txt = (new StreamReader(stream)).ReadToEnd();
                            board = (Models.Trello.Board)S.Util.Serializer.ReadObject(txt, typeof(Models.Trello.Board));
                        }

                        if (board != null)
                        {
                            //show success page in iframe
                            var scaffold = new Scaffold("/Pages/Import/Trello/success.html", S.Server.Scaffold);
                            scaffold.Data["name"] = board.name;
                            return scaffold.Render();
                        }
                        else
                        {
                            return "Could not parse json file correctly";
                        }
                        
                    }
                }
                //show upload form in iframe
                return S.Server.LoadFileFromCache("/Pages/Import/Trello/trello.html");
            }
            else
            {
                //show upload form in iframe
                return S.Server.LoadFileFromCache("/Pages/Import/Trello/trello.html");
            }
        }
    }
}
