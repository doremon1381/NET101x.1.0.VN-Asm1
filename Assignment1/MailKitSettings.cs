using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment1
{
    /// <summary>
    /// for how to use Mailkit, see https://mailtrap.io/blog/asp-net-core-send-email/
    /// </summary>
    public static class MailKitSettings
    {
        public const string HOST = "smtp.gmail.com";
        public const bool DEFAULTCREDENTIALS = false;
        public const int PORT = 587;
        public const bool ENABLESSL = true;
        public const string USERNAME = "UserName";
        public const string NAME = "Doremon The Blue";
        /// <summary>
        /// for how to get it: https://support.google.com/accounts/answer/185833?hl=en
        /// </summary>
        // TODO: you need to add your own password here
        public const string PASSWORD= "YourPasswordHere"; // replace with your actual password
        // and do the same with the EMAILID below
        public const string EMAILID = "your_email_here";

    }
}