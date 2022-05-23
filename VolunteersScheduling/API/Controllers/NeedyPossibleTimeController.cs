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
            List<TimeSlotModel> listOfAllTimeSlots = timeSlotBL.GetAllTimeSlot();
            List<TimeSlotModel> listOfTimeSlot = new List<TimeSlotModel>();
            foreach (var item in list)
            {
                listOfTimeSlot.Add(listOfAllTimeSlots.Find(slot => slot.time_slot_code == item.time_slot_code));
            }
            return listOfTimeSlot;
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
            var newNeedyPossibleTime = new NeedyPossibleTimeModel();
            try
            {
                for (int i = 0; i < listOfTimeSlots.Count; i++)
                {
                    code = timeSlotBL.InsertTimeSlot(listOfTimeSlots[i]);
                    newNeedyPossibleTime = new NeedyPossibleTimeModel();
                    newNeedyPossibleTime.time_slot_code = code;
                    newNeedyPossibleTime.neediness_details_code = needinessDetailsCode;
                    needyPossibleTimeBL.InsertNeedyPossibleTime(newNeedyPossibleTime);
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
