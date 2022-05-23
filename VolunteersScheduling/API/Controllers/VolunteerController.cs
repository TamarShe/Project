using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.Classes;
using DAL;
using MODELS;
using BL;

namespace API.Controllers
{
    [RoutePrefix("api/volunteer")]

    public class VolunteerController : ApiController
    {
        VolunteerBL volunteerBL = new VolunteerBL();
        VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();

        [HttpGet]
        [Route("getvolunteers")]
        public List<VolunteerModel> GetVolunteers()
        {
            return volunteerBL.GetAllvolunteers();
        }

        [HttpGet]
        [Route("login/{volunteerID}")]
        public string Login(string volunteerID)
        {
            return volunteerBL.GetAllvolunteers().First(v=>v.volunteer_ID==volunteerID).volunteer_full_name;
        }

        [HttpGet]
        [Route("findvolunteer/{volunteerID}")]
        public MODELS.VolunteerModel FindVolunteer(string volunteerID)
        {
            return volunteerBL.GetAllvolunteers().First(v => v.volunteer_ID == volunteerID);
        }

        [HttpGet]
        [Route("sendConstraintsToManager/{volunteeringDetailsCode}/{subject}/{content}")]
        public bool SendConstraintsToManager(int volunteeringDetailsCode, string subject,string content)
        {
            return SendEmail.SendToManager(volunteeringDetailsCode, content, subject);
        }

        [HttpPost]
        [Route("signup")]
        public string signup(VolunteerModel volunteer1)
        {
            return volunteerBL.InsertVolunteer(volunteer1);
        }

        [HttpPost]
        [Route("update")]
        public string update(VolunteerModel volunteer1)
        {
            return volunteerBL.UpdateVolunteer(volunteer1);
        }

        [HttpPost]
        [Route("deleteVolunteer/{volunteerID}")]
        public bool Delete(string volunteerID)
        {
            return volunteerBL.DeleteVolunteer(volunteerID);
        }


        [HttpGet]
        [Route("getvolunteeringdetailscode/{volunteerID}")]
        public int GetVolunteeringDetailsCode(string volunteerID)
        {
            return volunteeringDetailsBL.GetAllVolunteeringDetails().First(v => v.volunteer_ID == volunteerID).volunteering_details_code;
        }


        [HttpGet]
        [Route("delete/{volunteerID}/{volunteeringDetailsCode}")]
        public bool Delete(string volunteerID, int volunteeringDetailsCode)
        {
            //בינתים הוא לא מוחק את המתנדב עצמו אלא רק את הפרטי התנדבות שלו
            //לבדוק אם נכון להוסיף כאן תנאי שיבדוק אם זה הארגון היחיד שקשור אליו ואם כן אז למחוק אותו כדי שלא יתפוס מקום סתם
            if (volunteeringDetailsBL.DeleteVolunteeringDetails(volunteeringDetailsCode))
                return volunteerBL.DeleteVolunteer(volunteerID);
            else return false;
        }

        [HttpGet]
        [Route("checkIfPasswordIsPossible/{userID}/{password}")]
        public bool CheckIfPasswordIsPossible(string userID, string password)
        {
            return volunteerBL.CheckIfPasswordIsFree(userID, password);
        }

    }
}
