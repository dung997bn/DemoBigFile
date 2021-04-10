using System;

namespace DemoBigFile.Models
{
    public class DonationViewModel
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public string elec_code { get; set; }
        public string senator_id { get; set; }
        public int? amount { get; set; }
        public string representative_name { get; set; }
        public string postal_code { get; set; }
        public string address { get; set; }
        public int? occupation_id { get; set; }  
        public string receipt_no { get; set; }
    }
}
