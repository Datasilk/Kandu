﻿namespace Kandu.Vendor
{
    public enum EmailClientDataType
    {
        Text = 0,
        Number = 1,
        List = 2,
        Password = 3,
        UserOrEmail = 4,
        Boolean = 5
    }

    public class EmailClientParameter
    {
        /// <summary>
        /// Human-readable name of parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Based on the data type you select, Kandu will display the appropriate form field to the user
        /// </summary>
        public EmailClientDataType DataType { get; set; } = EmailClientDataType.Text;

        /// <summary>
        /// Determines if the parameter is a required field that must be filled out by the user
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// Short summary of what the parameter is used for
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// The default value of your parameter
        /// </summary>
        public string DefaultValue { get; set; } = "";

        /// <summary>
        /// The placeholder to display within the field to use as an example of what kind of data user should input
        /// </summary>
        public string Placeholder { get; set; } = "";

        /// <summary>
        /// If DataType = List, define a list of option values to select from
        /// </summary>
        public string[] ListOptions { get; set; }
    }

}
