using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class VolunteeringDetailsModel
    {
        public int volunteering_details_code { get; set; }
        public string volunteer_ID { get; set; }
        public int org_code { get; set; }
        public double weekly_hours { get; set; }
    }
}
