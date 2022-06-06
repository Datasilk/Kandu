using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kandu.Core;
using Utility.Strings;

namespace Kandu.Common
{
    public static class Themes
    {
        private static List<Models.Theme> _themes;

        public static List<Models.Theme> GetList
        {
            get
            {
                if( _themes == null) { 
                    _themes = new List<Models.Theme>();
                    var d = new DirectoryInfo(App.MapPath("/wwwroot/css/themes"));
                    var files = d.GetFiles("*.css");
                    foreach(var f in files)
                    {
                        _themes.Add(new Models.Theme()
                        {
                            Name = f.Name.Replace(".css", "").Capitalize(),
                            Filename = f.FullName,
                        });
                    }
                }
                return _themes;
            }
        }

        public static string RenderList(IRequest request, string onclick = "S.head.themes.change")
        {
            var section = new View("/Views/Themes/list.html");
            section["list"] = RenderListItems(request, onclick);
            return section.Render();
        }

        public static string RenderListItems(IRequest request, string onclick = "S.head.themes.change")
        {
            var html = new StringBuilder();
            var item = new View("/Views/Themes/list-item.html");
            var themes = GetList.OrderBy(a => a.Name).ToList();
            foreach (var theme in themes)
            {
                item.Clear();
                item.Bind(new { theme });
                if(request.User.Theme == theme.Name.ToLower())
                {
                    item.Show("selected");
                }
                item["onclick"] = (onclick != "" ? onclick : "S.head.themes.change") + "('" + theme.Name + "');S.head.user.hide();";
                html.Append(item.Render());
            }
            return html.ToString();
        }
    }
}
