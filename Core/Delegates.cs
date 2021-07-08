using System;
using System.Collections.Generic;
using System.Net.Mail;
using Kandu.Vendor;

namespace Kandu.Core
{
    /// <summary>
    /// Class used by Kandu to handle Core functionality
    /// </summary>
    public static class Delegates
    {
        public static class Email
        {
            /// <summary>
            /// Used by Kandu to delegate execution of Core.Email.Send. Please do not modify this field.
            /// </summary>
            public static Action<MailMessage, string> Send { get; set; }
        }

        public static class Log
        {
            public static Action<int, string, string, string, string> Error { get; set; }
        }

        public static class PartialViews
        {
            public static Func<IRequest, PartialViewKeys, Vendor.PartialViews.Container, string> Render { get; set; }
            public static Func<IRequest, PartialViewKeys, Vendor.PartialViews.Container, string> RenderForm { get; set; }
            public static Action<IRequest, Dictionary<string, string>, PartialViewKeys> Save { get; set; }
        }
    }
}
