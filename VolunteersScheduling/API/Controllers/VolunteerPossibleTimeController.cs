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
        public List<TimeSlotModel> aa(int volunteeringDetailsCode)
        {
            List<VolunteerPossibleTimeModel> list = volunteerPossibleHoursBL.GetAllVolunteerPossibleTime().FindAll(n => n.volunteering_details_code == volunteeringDetailsCode);
            List<TimeSlotModel> listOfAllTimeSlots = timeSlotBL.GetAllTimeSlot();
            List<TimeSlotModel> listOfTimeSlot = new List<TimeSlotModel>();
            foreach (var item in list)
            {
                listOfTimeSlot.Add(listOfAllTimeSlots.Find(slot => slot.time_slot_code == item.time_slot_code));
            }
            return listOfTimeSlot;
        }


        [HttpPost]
        [Route("addListOfPossibleTime/{volunteeringDetailsCode}")]
        public bool AddListOfPossibleTime( int volunteeringDetailsCode, List<TimeSlotModel> listOfTimeSlots)
        {
            var code = 0;
            var newVolunteerPossibleTime = new VolunteerPossibleTimeModel();
            try
            {         
                for (int i = 0; i < listOfTimeSlots.Count; i++)
                {
                    code = timeSlotBL.InsertTimeSlot(listOfTimeSlots[i]);
                    newVolunteerPossibleTime = new VolunteerPossibleTimeModel();
                    newVolunteerPossibleTime.time_slot_code = code;
                    newVolunteerPossibleTime.volunteering_details_code = volunteeringDetailsCode;
                    volunteerPossibleHoursBL.InsertVolunteerPossibleTime(newVolunteerPossibleTime);
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        [HttpGet]
        [Route("deleteVolunteerPossibleTimeSlot/{timeSlotCode}")]
        public bool DeleteNeedyPossibleTimeCode(int timeSlotCode)
        {
            try
            {
                VolunteerPossibleTimeModel volunteerPossibleTime = volunteerPossibleHoursBL.GetAllVolunteerPossibleTime().First(n => n.time_slot_code == timeSlotCode);
                if (volunteerPossibleHoursBL.DeleteVolunteerPossibleTime(volunteerPossibleTime.volunteers_possible_time_code))
                {
                    timeSlotBL.DeleteTimeSlot(timeSlotCode);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        [Route("VolunteerHasScheduleInVolunteeringDetails/{volunteeringDetailsCode}")]
        public bool VolunteerHasScheduleInVolunteeringDetails(int volunteeringDetailsCode)
        {
            return volunteerPossibleHoursBL.GetConflicts(volunteeringDetailsCode);
        }
    }
}
