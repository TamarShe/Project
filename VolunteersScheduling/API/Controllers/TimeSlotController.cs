using BL.Classes;
using MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/timeslot")]

    public class TimeSlotController : ApiController
    {
        TimeSlotBL timeSlotBL = new TimeSlotBL();

        [HttpPost]
        [Route("addtimeslot")]
        public int AddTimeSlot(MODELS.TimeSlotModel ts1)
        {
            return timeSlotBL.InsertTimeSlot(ts1);
        }

        [HttpPost]
        [Route("addtimeslots")]
        public List<int> AddTimeSlots(List<TimeSlotModel> ts)
        {
            List<int> timeSlotsCodes = new List<int>();
            foreach (var item in ts)
            {
                timeSlotsCodes.Add(timeSlotBL.InsertTimeSlot(item));
            }
            return timeSlotsCodes;
        }

        [HttpPost]
        [Route("updatetimeslot")]
        public int UpdateTimeSlot(MODELS.TimeSlotModel ts1)
        {
            return timeSlotBL.UpdateTimeSlot(ts1);
        }

        [HttpGet]
        [Route("gettimeslot/{timeSlotCode}")]
        public TimeSlotModel GetAllPossibleTime(int timeSlotCode)
        {
            return timeSlotBL.GetAllTimeSlot().First(t => t.time_slot_code == timeSlotCode);
        }

        [HttpGet]
        [Route("deletetimeslot/{code}")]
        public bool deleteTimeSlot(int code)
        {
            return timeSlotBL.DeleteTimeSlot(code);
        }
    }
}
