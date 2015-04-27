using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecureCloud.Auth
{
    public class AuthorizationAttribute : Attribute
    {
        public string AuthCode { get; private set; }

        public AuthorizationAttribute(string code)
        {
            this.AuthCode = code;
        }
    }
}