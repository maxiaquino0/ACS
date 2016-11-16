using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class MonthlyFeeDetail
    {
        [Key]
        public int MonthlyFeeDetailID { get; set; }

        public int MonthlyFeeID { get; set; }

        public int SportID { get; set; }

        [StringLength(30)]
        [Required(ErrorMessage = "You must enter {0}")]
        public string Description { get; set; }

        [Display(Name = "Fee Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal FeeAmount { get; set; }


        public virtual MonthlyFee MonthlyFee { get; set; }
        public virtual Sport Sport { get; set; }
    }
}