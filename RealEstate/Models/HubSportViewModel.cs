using RealEstateDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class HubSportViewModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        [Required(ErrorMessage = "API key is required.")]
        public string HubSportAPIKey { get; set; }

        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblUser User { get; set; }
    }
}
