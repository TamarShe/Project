using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class VolunteerModel
    {
        public string volunteer_ID { get; set; }
        public string volunteer_full_name { get; set; }
        public string volunteer_address { get; set; }
        public string volunteer_phone { get; set; }
        public string volunteer_email { get; set; }
        public string volunteer_password { get; set; }
        public System.DateTime volunteer_birth_date { get; set; }
    }
}
