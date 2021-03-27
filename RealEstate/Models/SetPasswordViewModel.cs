using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter password!")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter password!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must contain one special character, one later and one digit with minimum 8 length!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password!")]
        [Compare("Password", ErrorMessage = "Confirm Password is not matched with given password!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public int UserLoginType { get; set; }

        public bool KeepMeSigninIn { get; set; }
    }
}
