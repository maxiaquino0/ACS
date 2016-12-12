using ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACS.ViewModels
{
    public class MonthlyFeeViewDetail
    {
        public Partner Partner { get; set; }

        public MonthlyFee MonthlyFee { get; set; }

        public MonthlyFeeDetail MonthlyFeeDetail { get; set; }

        public List<MonthlyFeeDetail> MonthlyFeeDetails { get; set; }
    }
}