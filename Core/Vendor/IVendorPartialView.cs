using System.Collections.Generic;

namespace Kandu.Core.Vendor
{
    public interface IVendorPartialView
    {
        string Title { get; set; }
        List<MenuItem> MenuItems { get; set; }

        /// <summary>
        /// Used to render the partial view HTML outside of a form
        /// </summary>
        /// <returns></returns>
        string Render(IRequest request);

        /// <summary>
        /// Used to render the partial view HTML within a form
        /// </summary>
        /// <returns></returns>
        string RenderForm(IRequest request);

        /// <summary>
        /// If partial view is inside an HTML form, you will 
        /// have the chance to save the user's input data
        /// </summary>
        /// <param name="parameters"></param>
        void Save(IRequest request, Dictionary<string, string> parameters)
        {

        }
    }
}
