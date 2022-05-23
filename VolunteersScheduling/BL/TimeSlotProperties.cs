using DAL;
using MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class TimeSlotProperties
    {
        public volunteer volunteer { get; set; }
        public needy needy { get; set; }
        public int orgCode { get; set; }
        public time_slot time { get; set; }
    }
}
