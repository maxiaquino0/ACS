using ACS.Models;
using System.Collections.Generic;

namespace ACS.ViewModels
{
    public class MonthlyFeeView
    {
        public Partner Partner { get; set; }

        public Sport Sport { get; set; }

        public List<Sport> Sports { get; set; }
    }
}