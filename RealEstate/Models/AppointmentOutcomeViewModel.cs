using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class AppointmentOutcomeViewModel
    {
        public int AppointmentOutcomeId { get; set; }
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string AppointmentOutcomeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
