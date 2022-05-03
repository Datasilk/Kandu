using System.IO;
using Utility.Strings;

namespace Kandu.Controllers
{
    public class Attachment: Controller
    {
        public override string Render(string body = "")
        {
            if (!Parameters.ContainsKey("c") || !Parameters.ContainsKey("f")) { return Error("missing required parameter"); }
            var cardId = int.Parse(Parameters["c"]);
            var filename = Parameters["f"];
            var ext = filename.GetFileExtension();
            var size = Parameters.ContainsKey("s") ? Parameters["s"] : "";
            var card = Query.Cards.GetInfo(cardId);

            //check security
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardCanView.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanView.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            var fullsize = size != "thumb";
            var img = false;
            var attachment = false;

            //set content type
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    Context.Response.ContentType = "image/" + ext.Replace("jpg", "jpeg");
                    img = true;
                    break;
                case "svg":
                    Context.Response.ContentType = "image/svg+xml";
                    attachment = true;
                    break;
                default:
                    Context.Response.ContentType = "application/" + ext;
                    attachment = true;
                    break;
            }

            if (attachment)
            {
                Context.Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + filename + "\"");
            }

            //serve file
            using (FileStream fs = new FileStream(App.MapPath("/Content/files/" + cardId + "/" + (img && !fullsize ? "thumb/" : "") + filename), FileMode.Open))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    Context.Response.Body.WriteAsync(ms.ToArray());
                }
            }
            return "";
        }
    }
}
