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
        /// If true, allows the user to define a custom body text & body HTML for emails sent using this message type.
        /// </summary>
        public bool UserDefinedBody{ get; set; }

        /// <summary>
        /// Name of the HTML template file used for this email message type, e.g. "/Vendors/My-Plugin/my-email-type.html" and rendered as a View
        /// </summary>
        public string TemplateFile { get; set; }
    }
}
