using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeID { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(20)]
        [Display(Name ="Document Type")]
        public string Description { get; set; }

        public virtual ICollection<Partner> Partners { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

    }
}