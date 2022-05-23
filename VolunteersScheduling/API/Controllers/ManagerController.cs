using BL.Classes;
using MODELS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/manager")]

    public class ManagerController : ApiController
    {
        ManagerBL managerBL = new ManagerBL();
        NeedyBL needyBL = new NeedyBL();
        NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
        VolunteerBL volunteerBL = new VolunteerBL();
        VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();

        [HttpGet]
        [Route("getmanagers")]
        public List<ManagerModel> GetVolunteers()
        {
            return managerBL.GetAllManagers();
        }

        [HttpGet]
        [Route("login/{managerID}")]
        public string Login(string managerID)
        {
            return managerBL.GetAllManagers().First(v => v.manager_Id == managerID).manager_full_name;
        }

        [HttpGet]
        [Route("findmanager/{managerID}")]
        public MODELS.ManagerModel FindManager(string managerID)
        {
            return managerBL.GetAllManagers().First(v => v.manager_Id == managerID);
        }

        [HttpGet]
        [Route("orgcode/{managerID}")]
        public int OrgCode(string managerID)
        {
            return managerBL.GetAllManagers().First(v => v.manager_Id == managerID).manager_org_code;
        }

        [HttpGet]
        [Route("getAllManagers")]
        public List<ManagerModel> GetAllManagers()
        {
            return managerBL.GetAllManagers();
        }

        [HttpPost]
        [Route("signup")]
        public string signup(ManagerModel manager)
        {
            return managerBL.InsertManager(manager);
        }

        [HttpPost]
        [Route("update")]
        public string update(ManagerModel manager)
        {
            return managerBL.UpdateManager(manager);
        }

        [HttpPost]
        [Route("delete/{managerID}")]
        public bool Delete(string managerID)
        {
            return managerBL.DeleteManager(managerID);
        }


        [HttpPost]
        [Route("loadNeedies/{orgCode}")]
        public string[,] LoadNeedies(int orgCode)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            string path = HttpContext.Current.Server.MapPath("~/Content/Files/" + file.FileName);
            file.SaveAs(path);
           return needyBL.InsertNeediesFromExcelFile(path, orgCode);
        }

        [HttpPost]
        [Route("loadVolunteers/{orgCode}")]
        public string[,] LoadVolunteers(int orgCode)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            string path = HttpContext.Current.Server.MapPath("~/Content/Files/" + file.FileName);
            file.SaveAs(path);
            return volunteerBL.InsertVolunteersFromExcelFile(path, orgCode);
        }

        [HttpGet]
        [Route("getAllNeediesInOrg/{orgCode}")]
        public List<NeedyModel> GetAllNeediesInOrg(int orgCode)
        {
            IEnumerable<string> needies = new List<string>();
            List<NeedyModel> li = new List<NeedyModel>();
            NeedyModel n= new NeedyModel();
            needies = needinessDetailsBL.GetAllNeedinessDetails().FindAll(a=>a.org_code==orgCode).Select(b=>b.needy_ID);
            foreach (string id in needies)
            {
                n=needyBL.GetAllNeedies().FirstOrDefault(a => a.needy_ID == id);
                li.Add(n);
            }
            return li;
        }


        [HttpGet]
        [Route("getAllVolunteersInOrg/{orgCode}")]
        public List<VolunteerModel> getAllVolunteersInOrg(int orgCode)
        {
            IEnumerable<string> volunteers = new List<string>();
            List<VolunteerModel> li = new List<VolunteerModel>();
            VolunteerModel v = new VolunteerModel();
            volunteers = volunteeringDetailsBL.GetAllVolunteeringDetails().FindAll(a => a.org_code == orgCode).Select(b => b.volunteer_ID);
            foreach (string id in volunteers)
            {
                v = volunteerBL.GetAllvolunteers().FirstOrDefault(a => a.volunteer_ID == id);
                li.Add(v);
            }
            return li;
        }
    }
}
