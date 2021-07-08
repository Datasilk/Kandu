using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kandu.Vendor;
using Utility.Strings;

namespace Kandu.Common
{
    public static class PartialViews
    {

        public static void Save(Core.IRequest request, Dictionary<string, string> parameters, PartialViewKeys type)
        {
            if (Core.Vendors.PartialViews.ContainsKey(type))
            {
                foreach (var partialview in Core.Vendors.PartialViews[type])
                {
                    partialview.Save(request, parameters);
                }
            }
        }

        public static string Render(Core.IRequest request, PartialViewKeys type, Vendor.PartialViews.Container container = Vendor.PartialViews.Container.None)
        {
            if (Core.Vendors.PartialViews.ContainsKey(type))
            {
                var html = new StringBuilder();
                if (container != Vendor.PartialViews.Container.None)
                {
                    var view = new View("/Views/Shared/PartialViews/" + container.ToString().ToLower() + ".html");
                    foreach (var partialview in Core.Vendors.PartialViews[type])
                    {
                        view.Clear();
                        view["content"] = partialview.Render(request);
                        view["title"] = partialview.Title;
                        view["id"] = partialview.Title.ToLower().ReplaceOnlyAlphaNumeric(true, false, true).Replace(" ", "-");
                        view["menu"] = "";
                        html.Append(view.Render());
                    }
                }
                else
                {
                    foreach (var partialview in Core.Vendors.PartialViews[type])
                    {
                        html.Append(partialview.Render(request));
                    }
                }
                return html.ToString();
            }
            return "";
        }

        public static string RenderForm(Core.IRequest request, PartialViewKeys type, Vendor.PartialViews.Container container)
        {
            if (Core.Vendors.PartialViews.ContainsKey(type))
            {
                var html = new StringBuilder();
                if (container != Vendor.PartialViews.Container.None)
                {
                    var view = new View("/Views/Shared/PartialViews/" + container.ToString().ToLower() + ".html");
                    foreach (var partialview in Core.Vendors.PartialViews[type])
                    {
                        view.Clear();
                        view["content"] = partialview.RenderForm(request);
                        view["title"] = partialview.Title;
                        view["id"] = partialview.Title.ToLower().ReplaceOnlyAlphaNumeric(true, false, true).Replace(" ", "-");
                        view["menu"] = "";
                        html.Append(view.Render());
                    }
                }
                else
                {
                    foreach (var partialview in Core.Vendors.PartialViews[type])
                    {
                        html.Append(partialview.RenderForm(request));
                    }
                }
                return html.ToString();
            }
            return "";
        }

        private static string RenderMenu(IVendorPartialView partialView)
        {
            var menu = new View("Views/Shared/menu.html");
            var item = new View("Views/Shared/menu-item.html");
            menu["items"] = string.Join("\n", partialView.MenuItems.Select(a =>
            {
                item.Clear();
                item["href"] = a.Url;
                item["onclick"] = a.OnClick != "" ? " onclick=\"" + a.OnClick + "\"" : "";
                if (a.BlankTarget) { item["target"] = " target=\"_blank\""; }
                item["title"] = a.Title;
                return item.Render();
            }));
            return menu.Render();
        }
    }
}
