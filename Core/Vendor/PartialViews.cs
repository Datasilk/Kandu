using System;
using System.Collections.Generic;

namespace Kandu.Vendor
{
    public static class PartialViews
    {
        public enum Container
        {
            None,
            TitleMenu,
            Accordion
        }

        public static void Save(Core.IRequest request, Dictionary<string, string> parameters, PartialViewKeys type)
        {
            Core.Delegates.PartialViews.Save(request, parameters, type);
        }

        public static string Render(Core.IRequest request, PartialViewKeys type, Container container)
        {
            return Core.Delegates.PartialViews.Render(request, type, container);
        }

        public static string RenderForm(Core.IRequest request, PartialViewKeys type, Container container)
        {
            return Core.Delegates.PartialViews.RenderForm(request, type, container);
        }
    }
}
