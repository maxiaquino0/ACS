using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS.Models
{
    public class PartnerSport
    {
        [Key]
        public int PartnersSportsID { get; set; }

        public int PartnerID { get; set; }

        public int SportID { get; set; }

    }
}
