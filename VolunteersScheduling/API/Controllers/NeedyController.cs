using BL;
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
    [RoutePrefix("api/needy")]

    public class NeedyController : ApiController
    {
        NeedyBL needyBL = new NeedyBL();
        NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();

        [HttpGet]
        [Route("getneedies")]
        public List<NeedyModel> GetVolunteers()
        {
            return needyBL.GetAllNeedies();
        }

        [HttpGet]
        [Route("login/{needyCode}")]
        public string Login(string needy_ID)
        {
            return needyBL.GetAllNeedies().First(v => v.needy_ID.ToString() == needy_ID).needy_full_name;
        }

        [HttpGet]
        [Route("findneedy/{needyID}")]
        public MODELS.NeedyModel FindNeedy(string needyID)
        {
            return needyBL.GetAllNeedies().First(v => v.needy_ID == needyID);
        }

        [HttpGet]
        [Route("getneedinessdetailscode/{needyCode}")]
        public int GetNeedinessDetailsCode(string needy_ID)
        {
            return needinessDetailsBL.GetAllNeedinessDetails().First(v => v.needy_ID == needy_ID).neediness_details_code;
        }

        [HttpPost]
        [Route("signup")]
        public string signup(NeedyModel needy)
        {
            return needyBL.InsertNeedy(needy).ToString();
        }

        [HttpPost]
        [Route("update")]
        public string update(NeedyModel needy)
        {
            return needyBL.UpdateNeedy(needy).ToString();
        }

        [HttpGet]
        [Route("delete/{needyCode}/{needinessDetailsCode}")]
        public bool Delete(string needy_ID, int needinessDetailsCode)
        {
            //בינתים הוא לא מוחק את המתנדב עצמו אלא רק את הפרטי התנדבות שלו
            //לבדוק אם נכון להוסיף כאן תנאי שיבדוק אם זה הארגון היחיד שקשור אליו ואם כן אז למחוק אותו כדי שלא יתפוס מקום סתם
            if (needinessDetailsBL.DeleteneedinessDetailss(needinessDetailsCode))
                return needyBL.DeleteNeedy(needy_ID);
            else return false;
        }

        [HttpGet]
        [Route("sendConstraintsToManager/{needinessDetaisCode}/{subject}/{content}")]
        public bool SendConstraintsToManager(int needinessDetaisCode, string subject, string content)
        {
            return SendEmail.SendToManager(needinessDetaisCode, content, subject);
        }


        [HttpGet]
        [Route("checkIfPasswordIsPossible/{userID}/{password}")]
        public bool CheckIfPasswordIsPossible(string userID, string password)
        {
            return needyBL.CheckIfPasswordIsFree(userID, password);
        }
    }
}
