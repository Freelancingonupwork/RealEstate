using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAccountIntegration
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
    }
}
