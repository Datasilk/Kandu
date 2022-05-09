using System;
using System.IO;
using System.Linq;
using Utility.Strings;

namespace Kandu.Controllers
{
    public class UploadTheme : Controller
    {
        public override string Render(string body = "")
        {
            if (!Parameters.ContainsKey("orgId")) { return Error("Missing parameter orgId"); }
            if (!Parameters.ContainsKey("type")) { return Error("Missing parameter type"); }
            var orgId = int.Parse(Parameters["orgId"]);
            var type = Parameters["type"];

            //check security
            if (!User.CheckSecurity(orgId, new string[] { Security.Keys.OrgCanEditTheme.ToString(), Security.Keys.OrgFullAccess.ToString() }, Models.Scope.Organization, orgId)
            ) { return AccessDenied(); }

            //check for file attachments
            if (Parameters.Files == null || (Parameters.Files != null && Parameters.Files.Count == 0)) {
                return Error("Please specify one or more files to upload"); 
            }

            var folder = App.MapPath("/wwwroot/themes/orgs/" + orgId + "/");

            //allowed resource file extensions
            var allowed = new string[] { "jpg", "jpeg", "png", "gif", "svg", "avif", "webp" };

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (var file in Parameters.Files)
            {
                var filename = file.Value.Filename.Replace(" ", "-").ToLower();
                var ext = filename.Split('.')[^1];

                //save file to disk
                switch (type)
                {
                    case "css":
                    case "js":
                        filename = "theme";
                        break;
                    default:
                        if (!allowed.Contains(ext))
                        {
                            return Error("Unknown file type. Only upload files compatible with themes");
                        }
                        filename = filename.Replace("." + ext, "").ReplaceOnlyAlphaNumeric(true, true, "-", "_");
                        if (filename.Length > 58) { filename = filename.Substring(0, 58); }
                        break;
                }
                var finalname = filename + "." + ext;
                using (var fw = new FileStream(folder + finalname, FileMode.OpenOrCreate))
                {
                    file.Value.WriteTo(fw);
                }
            }
            return "Success";
        }
    }
}
