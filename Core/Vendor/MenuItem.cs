namespace Kandu.Core.Vendor
{
    public class MenuItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }

        /// <summary>
        /// Will generate an anchor link
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Use instead of the Url property to execute JavaScript
        /// </summary>
        public string OnClick { get; set; }
        /// <summary>
        /// Used along with the Url property to open the URL in a new browser window
        /// </summary>
        public bool BlankTarget { get; set; } = false;
    }
}
