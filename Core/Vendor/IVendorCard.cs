namespace Kandu.Core.Vendor
{
    public interface IVendorCard
    {
        /// <summary>
        /// A unique name (16 characters or less) that describes the card type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// If the user is viewing a board that contains your vendor card, the Init
        /// method will be executed once per page view
        /// </summary>
        /// <param name="request"></param>
        public void Init(IRequest request) { }

        /// <summary>
        /// Render the card in a list
        /// </summary>
        /// <returns></returns>
        public string Render(IRequest request);

        /// <summary>
        /// Render a form within the card details popup modal above the card description
        /// </summary>
        /// <returns></returns>
        public string RenderForm(IRequest request);

        /// <summary>
        /// Render content at the top of the side panel below the Card Type dropdown list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string RenderSidePanel(IRequest request);
    }
}
