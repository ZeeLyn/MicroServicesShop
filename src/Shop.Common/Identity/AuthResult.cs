using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Identity
{
    public class AuthResult
    {
        public string Token { get; set; }

        public int Expire { get; set; }
    }
}
