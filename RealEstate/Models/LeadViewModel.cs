using RealEstateDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class LeadViewModel
    {
        public int LeadId { get; set; }
        public int? AgentId { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //public int? StageId { get; set; }

        [Required(ErrorMessage = "Please select a stage")]
        public Nullable<int> StageId { get; set; }

        [Required(ErrorMessage = "Please Select Lead Source")]
        public string LeadSource { get; set; }
        [Required(ErrorMessage = "Please Select Lead Status")]
        public string LeadStatus { get; set; }
        [Required(ErrorMessage = "Please Select Industry")]
        public string Industry { get; set; }

        //[Required(ErrorMessage = "Please Select Stage")]
        //public string Stage { get; set; }
        [Required(ErrorMessage = "Please Select Owner")]
        public string LeadOwner { get; set; }

        public string OwnerImg { get; set; }

        [Required(ErrorMessage = "Please Enter Company Name")]
        public string Company { get; set; }
        [Required(ErrorMessage = "Please Enter First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please Enter Last Name")]
        public string LastName { get; set; }
        public string Title { get; set; }
        [Required(ErrorMessage = "Please Enter Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Please enter a phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        [Url(ErrorMessage = "Please enter valid website link")]
        public string Website { get; set; }

        public int? NoOfEmp { get; set; }
        public decimal? AnnualRevenue { get; set; }
        public int? Rating { get; set; }
        public bool EmailOptOut { get; set; }
        public string SkypeId { get; set; }
        public string TwitterId { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter valid email address")]
        public string SecondaryEmail { get; set; }
        [Required(ErrorMessage = "Please enter your street name")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Please enter your state name")]
        public string State { get; set; }
        [Required(ErrorMessage = "Please enter your country name")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please enter your city name")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter your zip code")]
        public string ZipCode { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string TagsName { get; set; }
        public virtual TblAgent Agent { get; set; }
        public virtual TblStage Stage { get; set; }
    }

    public class LeadTagViewModel
    {
        public int LeadTagId { get; set; }
        public Nullable<int> LeadId { get; set; }
        public Nullable<int> TagId { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}
