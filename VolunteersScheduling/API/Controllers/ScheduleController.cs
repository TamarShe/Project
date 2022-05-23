using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.Classes;
using MODELS;


namespace API.Controllers
{
    [RoutePrefix("api/schedule")]
    public class ScheduleController : ApiController
    {
        ScheduleBL scheduleBL = new ScheduleBL();
        TimeSlotBL timeSlotBL = new TimeSlotBL();

        [HttpGet]
        [Route("getScheduleforvolunteer/{volunteerID}")]
        public List<VolunteerCalendarEvent> GetScheduleForVolunteer(string volunteerID)
        {
            return scheduleBL.GetScheduleForVolunteer(volunteerID);
        }

        [HttpGet]
        [Route("getScheduleforneedy/{needyID}")]
        public List<NeedyCalendarEvent> GetScheduleForNeedy(string needyID)
        {
            return scheduleBL.GetScheduleForNeedy(needyID);
        }

        [HttpGet]
        [Route("getScheduleformanager/{managerID}")]
        public List<ManagerCalendarEvent> GetScheduleForManager(string managerID)
        {
            return scheduleBL.GetScheduleForMnager(managerID);
        }


        [HttpGet]
        [Route("deletefromschedule/{scheduleCode}")]
        public bool deleteFromSchedule(int scheduleCode)
        {
            return scheduleBL.DeleteSchedule(scheduleCode);
        }

        [HttpPost]
        [Route("addtoschedule")]
        public int AddToSchedule(ScheduleModel schedule)
        {
            return scheduleBL.InsertSchedule(schedule);
        }
    }
}
