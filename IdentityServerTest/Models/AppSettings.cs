using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServerTest.Models
{
    public class AppSettings
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
        public string Scope { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}

    