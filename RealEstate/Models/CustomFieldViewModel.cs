using RealEstateDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class CustomFieldViewModel
    {
        public int Id { get; set; }
        public int FieldTypeId { get; set; }
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Field Name is required.")]
        public string FieldName { get; set; }
        public virtual TblCustomFieldType CustomFieldType { get; set; }
        public bool HideIfEmpty { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
