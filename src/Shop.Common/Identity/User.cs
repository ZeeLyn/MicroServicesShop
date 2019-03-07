using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Common.Identity
{
    public class LoginView
    {
        [Required(ErrorMessage = "Please enter your email!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password!")]
        [MinLength(6, ErrorMessage = "Need at least 6 characters.")]
        [MaxLength(18, ErrorMessage = "Up to 18 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterView : LoginView
    {
        [Required(ErrorMessage = "Please enter your password!")]
        [MinLength(6, ErrorMessage = "Need at least 6 characters.")]
        [MaxLength(18, ErrorMessage = "Up to 18 characters long.")]
        [Compare("Password", ErrorMessage = "Inconsistent password entered twice")]
        [DataType(DataType.Password)]
        public string ReEnter { get; set; }
        [MaxLength(50)]
        public string NickName { get; set; }

    }

    public class User
    {
    }
}
