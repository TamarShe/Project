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
            List<NeedyPossibleTimeModel> list= needyPossibleTimeBL.GetAllNeedyPossibleTime().FindAll(n => n.neediness_details_code == needinessDetailsCode);
            var orgCode = needinessDetailsBL.GetAllNeedinessDetails().Find(n => n.neediness_details_code == needinessDetailsCode).org_code;
            var avgVolunteeringTime = organizationBL.GetAllOrganizations().Find(o => o.org_code == orgCode).avg_volunteering_time;
            List<TimeSlotModel> listOfAllTimeSlots = timeSlotBL.GetAllTimeSlot();
            List<TimeSlotModel> listOfTimeSlots = new List<TimeSlotModel>();
            List<TimeSlotModel> shortListOfTimeSlots = new List<TimeSlotModel>();
            TimeSlotModel currentTimeSlot, previousTimeSlot=new TimeSlotModel(),newTimeSlot = new TimeSlotModel();
            List<List<TimeSlotModel>> groupingSlots = new List<List<TimeSlotModel>>();

            foreach (var item in list)
            {
                listOfTimeSlots.Add(listOfAllTimeSlots.Find(slot => slot.time_slot_code == item.time_slot_code));
            }

            if (listOfTimeSlots!=null)
            {
                groupingSlots = listOfTimeSlots.GroupBy(a => a.day_of_week).Select(a=>a.ToList()).ToList();
            }

            
            foreach (var item in groupingSlots)
            {
                previousTimeSlot = item[0];

                for (int i = 1; i < item.Count; i++)
                {
                    currentTimeSlot = item[i];

                    while ((previousTimeSlot.start_at_hour + 1 == currentTimeSlot.start_at_hour))
                    {
                        i ++;
                        previousTimeSlot = new TimeSlotModel(currentTimeSlot);
                        currentTimeSlot = item[i];
                    }

                    newTimeSlot = new TimeSlotModel();
                    newTimeSlot.start_at_hour = previousTimeSlot.start_at_hour;
                    newTimeSlot.end_at_hour = item[i].end_at_hour;
                    newTimeSlot.day_of_week = item[i].day_of_week;
                    newTimeSlot.start_at_date = item[i].start_at_date;
                    newTimeSlot.end_at_date = item[i].end_at_date;

                    shortListOfTimeSlots.Add(newTimeSlot);
                }
            }
            return shortListOfTimeSlots;
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
