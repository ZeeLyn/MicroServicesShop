using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Identity
{
    public class LoginView
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class RegisterView : LoginView
    {
        public string ReEnter { get; set; }

        public string NickName { get; set; }

    }

    public class User
    {
    }
}
