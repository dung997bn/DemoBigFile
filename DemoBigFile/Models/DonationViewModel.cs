using System;

namespace DemoBigFile.Models
{
    public class DonationViewModel
    {
        public int id { get; set; }
        public int type_id { get; set; }
        public int class_id { get; set; }
        public int source_id { get; set; }
        public int parent_id { get; set; }
        public string elec_code { get; set; }
        public string senator_id { get; set; }
        public int state { get; set; }
        public int no_history { get; set; }
        public string image_url { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public int amount { get; set; }
        public int total_number { get; set; }
        public string company_name { get; set; }
        public string representative_name { get; set; }
        public string postal_code { get; set; }
        public string address { get; set; }
        public int occupation_id { get; set; }
        public int deduction { get; set; }
        public string missed_message { get; set; }
        public DateTime created_at { get; set; }
        public int created_user_id { get; set; }
        public string created_process { get; set; }
        public DateTime updated_at { get; set; }
        public int updated_user_id { get; set; }
        public string updated_process { get; set; }
        public DateTime deleted_at { get; set; }
        public int deleted_user_id { get; set; }
        public string deleted_process { get; set; }
        public int times_object_last_batch_process { get; set; }
        public int batch_processing { get; set; }

        public string receipt_no { get; set; }
    }
}
