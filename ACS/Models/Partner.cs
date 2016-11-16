using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class Partner
    {
        [Key]
        public int PartnerID { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required(ErrorMessage ="You must enter {0}")]
        [StringLength(15)]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage ="You must enter {0}")]
        public int DocumentTypeID { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [StringLength(80)]
        public string Address { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30)]
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }

        public string FullName { get { return string.Format("{0} {1}", LastName, Name); } }

        public virtual DocumentType DocumentType { get; set; }
        public virtual ICollection<MonthlyFee> MonthlyFees { get; set; }

    }
}