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
    [RoutePrefix("api/needypossibletime")]

    public class NeedyPossibleTimeController : ApiController
    {
        NeedyPossibleTimeBL needyPossibleTimeBL = new NeedyPossibleTimeBL();
        OrganizationBL organizationBL = new OrganizationBL();
        NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
        TimeSlotBL timeSlotBL = new TimeSlotBL();


        [HttpGet]
        [Route("getallpossibletime/{needinessDetailsCode}")]
        public List<NeedyPossibleTimeModel> GetAllPossibleTime(int needinessDetailsCode)
        {
            return needyPossibleTimeBL.GetAllNeedyPossibleTime().FindAll(n => n.neediness_details_code == needinessDetailsCode);
        }

        [HttpGet]
        [Route("getallpossibletimeslots/{needinessDetailsCode}")]
        public List<TimeSlotModel> GetAllPossibleTimeSlots(int needinessDetailsCode)
        {
            return needyPossibleTimeBL.GetAllNeedyPossibleTime().Where(t=>t.neediness_details_code==needinessDetailsCode);
        }

        [HttpGet]
        [Route("deleteNeedyPossibleTimeSlot/{timeSlotCode}")]
        public bool DeleteNeedyPossibleTimeCode(int timeSlotCode)
        {
            try
            {
                NeedyPossibleTimeModel needyPossibleTime = needyPossibleTimeBL.GetAllNeedyPossibleTime().First(n => n.time_slot_code == timeSlotCode);
                if (needyPossibleTimeBL.DeleteNeedyPossibleTime(needyPossibleTime))
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

        [HttpPost]
        [Route("addListOfPossibleTime/{needinessDetailsCode}")]
        public bool AddListOfPossibleTime(List<TimeSlotModel> listOfTimeSlots,int needinessDetailsCode)
        {
            var code = 0;
            var orgCode = needinessDetailsBL.GetAllNeedinessDetails().Find(n => n.neediness_details_code == needinessDetailsCode).org_code;
            var avgVolunteeringTime= organizationBL.GetAllOrganizations().Find(o => o.org_code == orgCode).avg_volunteering_time;
            var newNeedyPossibleTime = new NeedyPossibleTimeModel();
            var newTimeSlot = new TimeSlotModel();
            int currentStartHourCode;
            try
            {
                for (int i = 0; i < listOfTimeSlots.Count; i++)
                {
                    currentStartHourCode = listOfTimeSlots[i].start_at_hour;
                    while ((listOfTimeSlots[i].end_at_hour-currentStartHourCode)>=avgVolunteeringTime/15) 
                    {
                        newTimeSlot = new TimeSlotModel();
                        newTimeSlot.start_at_date = listOfTimeSlots[i].start_at_date;
                        newTimeSlot.end_at_date = listOfTimeSlots[i].end_at_date;
                        newTimeSlot.day_of_week = listOfTimeSlots[i].day_of_week;
                        newTimeSlot.start_at_hour = currentStartHourCode;
                        newTimeSlot.end_at_hour = newTimeSlot.start_at_hour+avgVolunteeringTime/15;

                        code = timeSlotBL.InsertTimeSlot(newTimeSlot);
                        newNeedyPossibleTime = new NeedyPossibleTimeModel();
                        newNeedyPossibleTime.time_slot_code = code;
                        newNeedyPossibleTime.neediness_details_code = needinessDetailsCode;
                        needyPossibleTimeBL.InsertNeedyPossibleTime(newNeedyPossibleTime);

                        currentStartHourCode ++;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
