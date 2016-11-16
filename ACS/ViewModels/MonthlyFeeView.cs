using ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACS.ViewModels
{
    public class MonthlyFeeView
    {
        public Partner Partner { get; set; }

        public Sport Sport { get; set; }

        public List<Sport> Sports { get; set; }
    }
}