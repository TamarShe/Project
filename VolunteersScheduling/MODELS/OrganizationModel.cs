using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class OrganizationModel
    {
        public int org_code { get; set; }
        public string org_name { get; set; }
        public string org_platform { get; set; }
        public bool need_scheduling { get; set; }
        public int org_min_age { get; set; }
        public DateTime activity_start_date { get; set; }
        public DateTime activity_end_date { get; set; }
        public int avg_volunteering_time { get; set; }

    }
}
