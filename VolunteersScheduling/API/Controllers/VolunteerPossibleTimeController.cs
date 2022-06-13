using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using BL.Classes;
using MODELS;

namespace API.Controllers
{
    [RoutePrefix("api/volunteerpossibletime")]

    public class VolunteerPossibleTimeController : ApiController
    {
        VolunteerPossibleHoursBL volunteerPossibleHoursBL = new VolunteerPossibleHoursBL();
        OrganizationBL organiztionBL = new OrganizationBL();
        TimeSlotBL timeSlotBL = new TimeSlotBL();

        [HttpGet]
        [Route("getallpossibletimeslots/{volunteeringDetailsCode}")]
        public List<TimeSlotModel> GetAllPossibleTimeSlots(int volunteeringDetailsCode)
        {
            return volunteerPossibleHoursBL.GetAllPossibleTimeSlots(volunteeringDetailsCode);
        }


        [HttpPost]
        [Route("addListOfPossibleTime/{volunteeringDetailsCode}")]
        public bool AddListOfPossibleTime( int volunteeringDetailsCode, List<TimeSlotModel> listOfTimeSlots)
        {
            return volunteerPossibleHoursBL.AddListOfPossibleTime(listOfTimeSlots, volunteeringDetailsCode);
        }

        [HttpGet]
        [Route("deleteVolunteerPossibleTimeSlot/{timeSlotCode}")]
        public bool DeleteNeedyPossibleTimeCode(int timeSlotCode)
        {
            return volunteerPossibleHoursBL.DeleteVolunteerPossibleTimeCode(timeSlotCode);
        }

        [HttpGet]
        [Route("VolunteerHasConflicts/{volunteeringDetailsCode}")]
        public int VolunteerHasConflicts(int volunteeringDetailsCode)
        {
            return volunteerPossibleHoursBL.GetConflicts(volunteeringDetailsCode);
        }

        [HttpGet]
        [Route("DeleteConflicts/{volunteeringDetailsCode}")]
        public bool DeleteConflicts(int volunteeringDetailsCode)
        {
            return volunteerPossibleHoursBL.DeleteConflicts(volunteeringDetailsCode);
        }
    }
}
