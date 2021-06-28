using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblSmtp
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
