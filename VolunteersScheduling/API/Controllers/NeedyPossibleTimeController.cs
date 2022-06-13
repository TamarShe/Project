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
            return needyPossibleTimeBL.GetAllPossibleTimeSlots(needinessDetailsCode);

        }

        [HttpGet]
        [Route("deleteNeedyPossibleTimeSlot/{timeSlotCode}")]
        public bool DeleteNeedyPossibleTimeCode(int timeSlotCode)
        {
            return needyPossibleTimeBL.DeleteNeedyPossibleTimeCode(timeSlotCode);

        }

        [HttpPost]
        [Route("addListOfPossibleTime/{needinessDetailsCode}")]
        public bool AddListOfPossibleTime(List<TimeSlotModel> listOfTimeSlots,int needinessDetailsCode)
        {
            return needyPossibleTimeBL.AddListOfPossibleTime(listOfTimeSlots,needinessDetailsCode);
        }

        [HttpGet]
        [Route("NeedyHasConflics/{needinessDetailsCode}")]
        public int NeedyHasConflics(int needinessDetailsCode)
        {
            return needyPossibleTimeBL.GetConflicts(needinessDetailsCode);
        }

        [HttpGet]
        [Route("DeleteConflicts/{needinessDetailsCode}")]
        public bool DeleteConflicts(int needinessDetailsCode)
        {
            return needyPossibleTimeBL.DeleteConflicts(needinessDetailsCode);
        }
    }
}
