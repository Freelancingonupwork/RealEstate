using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblAccountCompany
    {
        public int AccountDetailsId { get; set; }
        public int? AccountId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TblAccount Account { get; set; }
    }
}
