using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class TimeSlotModel
    {
        public int time_slot_code { get; set; }
        public int day_of_week { get; set; }
        public System.DateTime start_at_date { get; set; }
        public System.DateTime end_at_date { get; set; }
        public int end_at_hour { get; set; }
        public int start_at_hour { get; set; }
    }
}
