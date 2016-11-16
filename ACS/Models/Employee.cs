using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30)]
        public string Name { get; set; }

        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(20)]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [Display(Name ="Document Type")]
        public int DocumentTypeID { get; set; }

        [StringLength(80)]
        public string Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage ="You must enter {0}")]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string FullName { get {return string.Format("{0} {1}", LastName, Name); } }

        public virtual DocumentType DocumentType { get; set; }
        public virtual ICollection<Sport> Sports { get; set; }

    }
}