using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class MonthlyFee
    {
        [Key]
        public int MonthlyFeeID { get; set; }

        public int PartnerID { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [DisplayFormat(DataFormatString = "{0:yyy/MM}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Period { get; set; }

        public MonthlyFeeStatus MonthlyFeeStatus { get; set; }


        public virtual Partner Partner { get; set; }
        public virtual ICollection<MonthlyFeeDetail> MonthlyFeeDetails { get; set; }
    }
}