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
    [RoutePrefix("api/volunteeringdetails")]

    public class VolunteeringDetailsController : ApiController
    {
        VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
        [HttpGet]
        [Route("sendConstraintsToManager/{volunteeringDetailsCode}/{subject}/{content}")]
        public bool SendConstraintsToManager(int volunteeringDetailsCode, string subject, string content)
        {
            return SendEmail.SendToManager(volunteeringDetailsCode, content, subject);
        }

        [HttpGet]
        [Route("getvolunteeringdetails/{volunteeringdetailsCode}")]
        public VolunteeringDetailsModel SendConstraintsToManager(int volunteeringDetailsCode)
        {
            return volunteeringDetailsBL.GetAllVolunteeringDetails().Find(a => a.volunteering_details_code == volunteeringDetailsCode);
        }

        [HttpPost]
        [Route("signuptoorg")]
        public bool signupToOrg(VolunteeringDetailsModel volunteeringDetailsModel)
        {
            return (volunteeringDetailsBL.InsertVolunteeringDetails(volunteeringDetailsModel)!=0);
        }

        [HttpGet]
        [Route("getvolunteerid/{volunteeringDetailsCode}")]
        public string GetVolunteeringDetails(int volunteeringDetailsCode)
        {
            return volunteeringDetailsBL.GetAllVolunteeringDetails().First(a => a.volunteering_details_code == volunteeringDetailsCode).volunteer_ID;
        }

        [HttpGet]
        [Route("getvolunteeringdetailsforvolunteer/{volunteerID}/{orgCode}")]
        public VolunteeringDetailsModel getVolunteeringDetailsForVolunteer(string volunteerID, int orgCode)
        {
            return volunteeringDetailsBL.GetAllVolunteeringDetails().First(a => a.volunteer_ID == volunteerID && a.org_code == orgCode);
        }

        [HttpPost]
        [Route("updatevolunteeringdetails")]
        public int UpdateVolunteeringDetails(VolunteeringDetailsModel volunteeringDetails)
        {
            return volunteeringDetailsBL.UpdateVolunteeringDetails(volunteeringDetails);
        }

        [HttpPost]
        [Route("addvolunteeringdetails")]
        public int AddVolunteeringDetails(VolunteeringDetailsModel volunteeringDetails)
        {
            return volunteeringDetailsBL.InsertVolunteeringDetails(volunteeringDetails);
        }


        [HttpGet]
        [Route("GetAllVolunteeringDetailsForVolunteer/{volunteerID}")]
        public List<VolunteeringDetailsModel> GetAllNeedinessDetailsForNeedy(string volunteerID)
        {
            return volunteeringDetailsBL.GetAllVolunteeringDetails().FindAll(n => n.volunteer_ID == volunteerID);
        }
    }
}
