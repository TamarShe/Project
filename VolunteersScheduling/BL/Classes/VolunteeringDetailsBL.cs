using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class VolunteeringDetailsBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.VolunteeringDetailsModel> listOfVolunteeringDetails;

        public VolunteeringDetailsBL()
        {
            dbCon = new DBConnection();
            listOfVolunteeringDetails = ConvertListToModel(dbCon.GetDbSet<volunteering_details>().ToList());
        }

        public List<MODELS.VolunteeringDetailsModel> GetAllVolunteeringDetails()
        {
            return listOfVolunteeringDetails;
        }

        public int InsertVolunteeringDetails(MODELS.VolunteeringDetailsModel VolunteeringDetails1)
        {
            try
            {
                dbCon.Execute<volunteering_details>(ConvertVolunteeringDetailsToEF(VolunteeringDetails1),
                DBConnection.ExecuteActions.Insert);
                listOfVolunteeringDetails = ConvertListToModel(dbCon.GetDbSet<volunteering_details>().ToList());
                return listOfVolunteeringDetails.Max(n => n.volunteering_details_code);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateVolunteeringDetails(MODELS.VolunteeringDetailsModel VolunteeringDetails1)
        {
            if (listOfVolunteeringDetails.Find(v => v.volunteering_details_code == VolunteeringDetails1.volunteering_details_code) != null)
                try
                {
                    dbCon.Execute<volunteering_details>(ConvertVolunteeringDetailsToEF(VolunteeringDetails1),
                    DBConnection.ExecuteActions.Update);
                    listOfVolunteeringDetails = ConvertListToModel(dbCon.GetDbSet<volunteering_details>().ToList());
                    return listOfVolunteeringDetails.First(v => v.volunteering_details_code == VolunteeringDetails1.volunteering_details_code).volunteering_details_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfVolunteeringDetails.First(v => v.volunteering_details_code == VolunteeringDetails1.volunteering_details_code).volunteering_details_code;
        }

        public bool DeleteVolunteeringDetails(int VolunteeringDetailsCode)
        {
            VolunteeringDetailsModel volunteeringDetailsModel = listOfVolunteeringDetails.Find(v => v.volunteering_details_code == VolunteeringDetailsCode);
            if (volunteeringDetailsModel != null)
                try
                {
                    dbCon.Execute<volunteering_details>(ConvertVolunteeringDetailsToEF(volunteeringDetailsModel),
                    DBConnection.ExecuteActions.Delete);
                    listOfVolunteeringDetails = ConvertListToModel(dbCon.GetDbSet<volunteering_details>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        #region convert functions
        public static volunteering_details ConvertVolunteeringDetailsToEF(MODELS.VolunteeringDetailsModel v)
        {
            return new volunteering_details
            {
                volunteering_details_code=v.volunteering_details_code,
                volunteer_ID=v.volunteer_ID,
                org_code=v.org_code,
                weekly_hours=v.weekly_hours
            };
        }

        public static MODELS.VolunteeringDetailsModel ConvertVolunteeringDetailsToModel(volunteering_details v)
        {
            return new MODELS.VolunteeringDetailsModel
            {
                volunteering_details_code = v.volunteering_details_code,
                volunteer_ID = v.volunteer_ID,
                org_code = v.org_code,
                weekly_hours = v.weekly_hours
            };
        }

        public static List<MODELS.VolunteeringDetailsModel> ConvertListToModel(List<volunteering_details> li)
        {
            return li.Select(l => ConvertVolunteeringDetailsToModel(l)).ToList();
        }
        #endregion
    }
}
