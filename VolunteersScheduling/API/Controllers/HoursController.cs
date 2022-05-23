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
    [RoutePrefix("api/hours")]

    public class HoursController : ApiController
    {
        HoursBL hoursBL = new HoursBL();

        [HttpGet]
        [Route("getListOfStartAndEnd/{TimeDuration}")]
        public HourModel[,] GetListOfStartAndEnd(int timeDuration)
        {
            return hoursBL.GetListOfStartAndEnd(timeDuration);
        }

        [HttpGet]
        [Route("getAllHours")]
        public List<HourModel> GetAllHours()
        {
            return hoursBL.GetAllHours();
        }
    }
}
