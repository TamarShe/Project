using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class ScheduleModel
    {
        public int schedule_code { get; set; }
        public int time_slot_code { get; set; }
        public int volunteering_details_code { get; set; }
        public int neediness_details_code { get; set; }
    }
}
