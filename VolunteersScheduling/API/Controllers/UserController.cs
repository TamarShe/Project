using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BL;
using BL.Classes;
using MODELS;

namespace API.Controllers
{
    [RoutePrefix("api/user")]

    public class UserController : ApiController
    {
        VolunteerBL volunteerBL = new VolunteerBL();
        ManagerBL managerBL = new ManagerBL();
        NeedyBL needyBL = new NeedyBL();
        OrganizationBL organizaionBL = new OrganizationBL();

        [HttpGet]
        [Route("login/{userID}/{userPassword}")]
        public string Login(string userID, string userPassword)
        {
            if (volunteerBL.GetAllvolunteers().Find(v => v.volunteer_ID == userID && v.volunteer_password == userPassword) != null)
                return "volunteer";
            if (managerBL.GetAllManagers().Find(v => v.manager_Id == userID && v.manager_password == userPassword && v.is_general_manager==false) != null)
               return "manager";
            if (managerBL.GetAllManagers().Find(v => v.manager_Id == userID && v.manager_password == userPassword && v.is_general_manager == true) != null)
                return "general-manager";
            if (needyBL.GetAllNeedies().Find(v => v.needy_ID == userID && v.needy_password == userPassword) != null)
                return "needy";
            return "";
        }

        [HttpGet]
        [Route("confirmPassword/{emailAddress}/")]
        public int SendTemporaryPassword(string emailAddress)
        {
            return BL.SendEmail.SendTemporaryPassword(emailAddress);
        }

        [HttpGet]
        [Route("dis/{or}/{des}")]
        public double GetDistance(string or, string des)
        {
            return BL.GoogleMaps.GetDistanceInMinutes(or,des).Result;
        }

        [HttpGet]
        [Route("getAmounts")]
        public List<int> GetAmounts()
        {
            List<int> list=new List<int>();
            list.Add(volunteerBL.GetAllvolunteers().Count);
            list.Add(needyBL.GetAllNeedies().Count);
            list.Add(organizaionBL.GetAllOrganizations().Count);
            return list;
        }
    }
}