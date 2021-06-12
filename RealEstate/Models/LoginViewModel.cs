using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class LoginViewModel
    {
        public int UserId { get; set; }
        public int? UserLoginTypeId { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Please enter valid email address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }


    public enum UserLoginType
    {
        Admin = 1,
        GoogleAccount = 2,
        MicrosoftAccount = 3,
        Manual = 4,
        Company = 5
    }

    public enum RoleType
    {
        Admin = 1,
        Agent = 2,
        MicrosoftAccount = 3,
        Manual = 4,
        Company = 5
    }

    public enum EmailType
    {
        EmailTemplate = 1,
        TextTemplate = 2
    }


    public enum AuthAccountType
    {
        HubSportAuth = 1,
        GoogleAuth = 2,
        MicrosoftAuth = 3
    }
}
