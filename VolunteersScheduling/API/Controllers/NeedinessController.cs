using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.Classes;
using MODELS;

namespace API.Controllers
{
    [RoutePrefix("api/neediness")]

    public class NeedinessController : ApiController
    {
        NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();

        [HttpGet]
        [Route("getneedinessdetails/{needinessDetailsCode}")]
        public NeedinessDetailsModel GetNeedinessDetails(int needinessDetailsCode)
        {
            return needinessDetailsBL.GetAllNeedinessDetails().First(a => a.neediness_details_code == needinessDetailsCode);
        }

        [HttpGet]
        [Route("getneedinessdetailsforneedy/{needyID}/{orgCode}")]
        public NeedinessDetailsModel GetNeedinessDetailsForNeedy(string needyID, int orgCode)
        {
            return needinessDetailsBL.GetAllNeedinessDetails().First(a => a.needy_ID == needyID && a.org_code == orgCode);
        }

        [HttpPost]
        [Route("updateneedinessdetails")]
        public int UpdateNeedinessDetails(NeedinessDetailsModel needinesDetails)
        {
            return needinessDetailsBL.UpdateNeedinessDetailss(needinesDetails);
        }

        [HttpPost]
        [Route("addneedinessdetails")]
        public int AddNeedinessDetails(NeedinessDetailsModel needinesDetails)
        {
            return needinessDetailsBL.Insertneediness_detailss(needinesDetails);
        }


        [HttpGet]
        [Route("GetAllNeedinessDetailsForNeedy/{needyID}")]
        public List<NeedinessDetailsModel> GetAllNeedinessDetailsForNeedy(string needyID)
        {
            return needinessDetailsBL.GetAllNeedinessDetails().FindAll(n => n.needy_ID == needyID);
        }
    }
}
