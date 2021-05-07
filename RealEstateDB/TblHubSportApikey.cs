using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblHubSportApikey
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string HubSportApikey { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
