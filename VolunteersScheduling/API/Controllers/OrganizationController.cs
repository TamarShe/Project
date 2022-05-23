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
    [RoutePrefix("api/organization")]

    public class OrganizationController : ApiController
    {
        OrganizationBL organizationBL = new OrganizationBL();
        NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
        VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();

        [HttpGet]
        [Route("getallorgs")]
        public List<MODELS.OrganizationModel> GetOrganizations()
        {
            return organizationBL.GetAllOrganizations();
        }

        [HttpGet]
        [Route("getorg/{orgCode}")]
        public MODELS.OrganizationModel GetOrg(int orgCode)
        {
            return organizationBL.GetAllOrganizations().Find(a => a.org_code == orgCode);
        }

        [HttpPost]
        [Route("updateOrganization")]
        public int UpdateOrganization(OrganizationModel org)
        {
            return organizationBL.UpdateOrganization(org);
        }

        [HttpPost]
        [Route("addOrganization")]
        public int AddOrganization(OrganizationModel org)
        {
            return organizationBL.InsertOrganization(org);
        }


        [HttpGet]
        [Route("getfreeorgs/{volunteerID}")]
        public List<MODELS.OrganizationModel> GetFreeOrsg(string volunteerID)
        {
            return organizationBL.GetFreeOrgsForVolunteer(volunteerID);
        }

        [HttpGet]
        [Route("getdisabledorgs/{volunteerID}")]
        public List<OrganizationModel> GetDisabledOrgs(string volunteerID)
        {
            return organizationBL.GetDisabledOrgsForVolunteer(volunteerID);
        }

        [HttpGet]
        [Route("getRelevantOrgsForNeedy/{needyID}")]
        public List<OrganizationModel> GetRelevantOrgsForNeedy(string needyID)
        {
            List<OrganizationModel> allOrgs = organizationBL.GetAllOrganizations();
            List<NeedinessDetailsModel> listOfNedinessDetails = needinessDetailsBL.GetAllNeedinessDetails().FindAll(n => n.needy_ID == needyID);
            List<OrganizationModel> relevantOrgs=new List<OrganizationModel>();
            bool exist = false;
            foreach (var item in allOrgs)
            {
                exist = listOfNedinessDetails.Exists(n => n.org_code == item.org_code);
                if(exist)
                {
                    relevantOrgs.Add(item);
                }
            }
            return relevantOrgs;
        }

        [HttpGet]
        [Route("getRelevantOrgsForVolunteer/{volunteerID}")]
        public List<OrganizationModel> GetRelevantOrgsForVolunteer(string volunteerID)
        {
            List<OrganizationModel> allOrgs = organizationBL.GetAllOrganizations();
            List<VolunteeringDetailsModel> listOfVolunteeringDetails = volunteeringDetailsBL.GetAllVolunteeringDetails().FindAll(n => n.volunteer_ID == volunteerID);
            List<OrganizationModel> relevantOrgs = new List<OrganizationModel>();
            bool exist = false;
            foreach (var item in allOrgs)
            {
                exist = listOfVolunteeringDetails.Exists(n => n.org_code == item.org_code);
                if (exist)
                {
                    relevantOrgs.Add(item);
                }
            }
            return relevantOrgs;
        }
    }
}
