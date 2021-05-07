using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class EmailTemplateViewModel
    {
        public int EmailTemplateID { get; set; }

        [Required(ErrorMessage = "Template Type is required.")]
        public int TemplateTypeId { get; set; }
        public int AccountId { get; set; }

        [Required(ErrorMessage = "From Email is required.")]
        public string FromEmail { get; set; }
        [Required(ErrorMessage = "Email Name is required.")]
        public string EmailName { get; set; }
        [Required(ErrorMessage = "Email Description is required.")]
        public string EmailTemplateDescription { get; set; }
        [Required(ErrorMessage = "Email Subject is required.")]
        public string EmailSubject { get; set; }
        [Required(ErrorMessage = "Email Body is required.")]
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
