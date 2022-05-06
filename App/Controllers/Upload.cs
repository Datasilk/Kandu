using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Utility.Strings;

namespace Kandu.Controllers
{
    public class Upload : Controller
    {
        public override string Render(string body = "")
        {
            if (!Parameters.ContainsKey("cardId")) { return Error("Missing parameter cardId"); }
            var cardId = int.Parse(Parameters["cardId"]);
            var card = Query.Cards.GetInfo(cardId);
            var board = Query.Boards.GetInfo(card.boardId);

            //check security
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            //check for file attachments
            if (Parameters.Files == null || (Parameters.Files != null && Parameters.Files.Count == 0)) {
                return Error("Please specify one or more files to upload"); 
            }

            var folder = App.MapPath("/Content/files/" + card.orgId + "/" + cardId + "/");
            var filenames = new List<Models.FileInfo>();
            var img = new Utility.Images();

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            foreach (var file in Parameters.Files)
            {
                var filename = file.Value.Filename.Replace(" ", "-").ToLower();
                var ext = filename.Split('.')[^1];
                filename = filename.Replace("." + ext, "").ReplaceOnlyAlphaNumeric(true, true, "-", "_");
                if (filename.Length > 58) { filename = filename.Substring(0, 58); }
                var rnd = new Random();
                var id = rnd.Next(1000, 9999);
                var finalname = filename + "." + ext;// + "_" + id + "." + ext;
                var filetype = 0; //0 = unknown, 1 = image, 2 = document, 3 = zip file, 4 = video
                switch (ext)
                {
                    case "jpg":
                    case "jpeg":
                    case "png":
                    case "gif":
                        filetype = 1;
                        break;
                    case "doc":
                    case "docx":
                    case "rtf":
                    case "pdf":
                    case "txt":
                    case "csv":
                    case "xls":
                    case "xlsx":
                        filetype = 2;
                        break;
                    case "zip":
                    case "rar":
                    case "7z":
                        filetype = 3;
                        break;
                    case "mp4":
                    case "flv":
                    case "ogg":
                    case "avi":
                    case "divx":
                    case "xvid":
                    case "mkv":
                        filetype = 4;
                        break;
                }

                //save file to disk
                using (var fw = new FileStream(folder + finalname, FileMode.OpenOrCreate))
                {
                    file.Value.WriteTo(fw);
                }

                if (filetype == 1)
                {
                    //create thumbnail image
                    if (!Directory.Exists(folder + "thumb\\"))
                    {
                        Directory.CreateDirectory(folder + "thumb\\");
                    }
                    img.Shrink(folder + finalname, folder + "thumb\\" + finalname, 400);
                }

                //add file to result list
                filenames.Add(new Models.FileInfo()
                {
                    Path = "/file/" + cardId + "/",
                    Name = finalname,
                    Type = filetype
                });
            }
            return JsonResponse(filenames);
        }
    }
}
