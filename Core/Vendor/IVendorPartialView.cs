using System.Collections.Generic;

namespace Kandu.Vendor
{
    public interface IVendorPartialView
    {
        /// <summary>
        /// Title to display in the header of your partial view
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Area within Kandu's UI where your partial view will be rendered
        /// </summary>
        PartialViewKeys Area { get; set; }

        /// <summary>
        /// list of menu items to display at the top-right corner of your partial view
        /// </summary>
        List<MenuItem> MenuItems { get; set; }

        /// <summary>
        /// Used to render the partial view HTML outside of a form
        /// </summary>
        /// <returns></returns>
        string Render(Core.IRequest request, Dictionary<string, object> data);

        /// <summary>
        /// Used to render the partial view HTML within a form. Use only if the partial view is part of a form, otherwise use Render() instead
        /// </summary>
        /// <returns></returns>
        string RenderForm(Core.IRequest request, Dictionary<string, object> data);

        /// <summary>
        /// If partial view is inside an HTML form, you will 
        /// have the chance to save the user's input data
        /// </summary>
        /// <param name="parameters"></param>
        void Save(Core.IRequest request, Dictionary<string, string> parameters, Dictionary<string, object> data);
    }
}
