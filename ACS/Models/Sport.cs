using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class Sport
    {
        [Key]
        public int SportID { get; set; }

        [StringLength(30)]
        [Required(ErrorMessage = "You must enter {0}")]
        [Display(Name ="Sport")]
        public string Description { get; set; }

        [Display(Name = "Fee Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode =false)]
        public decimal FeeAmount { get; set; }

        [Display(Name ="Employee")]
        public int EmployeeID { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual ICollection<MonthlyFeeDetail> MonthlyFeeDetails { get; set; }

    }
}