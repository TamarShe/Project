using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class NeedinessDetailsModel
    {
        public int neediness_details_code { get; set; }
        public string needy_ID { get; set; }
        public int org_code { get; set; }
        public double weekly_hours { get; set; }
        public string details { get; set; }

    }
}
