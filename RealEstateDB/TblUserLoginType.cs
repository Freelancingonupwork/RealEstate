using System;
using System.Collections.Generic;

#nullable disable

namespace RealEstateDB
{
    public partial class TblUserLoginType
    {
        public TblUserLoginType()
        {
            TblAgents = new HashSet<TblAgent>();
            TblUsers = new HashSet<TblUser>();
        }

        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }

        public virtual ICollection<TblAgent> TblAgents { get; set; }
        public virtual ICollection<TblUser> TblUsers { get; set; }
    }
}
