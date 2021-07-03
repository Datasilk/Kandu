namespace Kandu.Vendor
{
    public class EmailType
    {
        /// <summary>
        /// Used to identify the message type (e.g. "payment-confirmation")
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The human-readable name of your email message type, used to display in Kandu's Email Action Settings section within the Website Settings tab.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A small summary of what your email message is used for.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If true, allows the user to define a custom subject line for emails sent using this message type.
        /// </summary>
        public bool UserDefinedSubject { get; set; }

        /// <summary>
        /// Name of the HTML template file used for this email message type. All HTML templates are located in the /Content/emails/ folder.
        /// In your vendor plugin folder, create the folder /emails/ and include that folder in the publishToPlatform method located in your
        /// gulpfile.js file. When your plugin is installed within Kandu, all HTML files within your /emails/ folder will be copied to
        /// Kandu's /Content/emails/ folder, so make sure your HTML filenames are unique to your plugin so they do not overwrite HTML files
        /// that already exist in the /Content/emails/ folder.
        /// </summary>
        public string TemplateFile { get; set; }
    }
}
