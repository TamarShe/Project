using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class TimeSlotChromosome
    {
        public TimeSpan StartAt { get; set; }
        public TimeSpan EndAt => StartAt.Add(TimeSpan.FromHours(3));
        public string CourseId { get; set; }
        public string PlaceId { get; set; }
        public string TeacherId { get; set; }
        public List<string> Students { get; set; }
        public int Day { get; set; }
    }
}
