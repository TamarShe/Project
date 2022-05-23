using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class NeedinessDetailsBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<NeedinessDetailsModel> listOfNeedinessDetails;

        public NeedinessDetailsBL()
        {
            dbCon = new DBConnection();
            listOfNeedinessDetails = ConvertListToModel(dbCon.GetDbSet<neediness_details>().ToList());
        }

        public List<MODELS.NeedinessDetailsModel> GetAllNeedinessDetails()
        {
            return listOfNeedinessDetails;
        }

        public int Insertneediness_detailss(NeedinessDetailsModel neediness_details1)
        {
            if (listOfNeedinessDetails.Find(n => n.neediness_details_code == neediness_details1.neediness_details_code) == null)
                try
                {
                    dbCon.Execute<neediness_details>(ConvertNeedinessDetailsToEF(neediness_details1),
                    DBConnection.ExecuteActions.Insert);
                    listOfNeedinessDetails = ConvertListToModel(dbCon.GetDbSet<neediness_details>().ToList());
                    return listOfNeedinessDetails.Max(n => n.neediness_details_code);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfNeedinessDetails.Max(n => n.neediness_details_code);
        }

        public int UpdateNeedinessDetailss(NeedinessDetailsModel neediness_details1)
        {
            if (listOfNeedinessDetails.Find(n => n.neediness_details_code == neediness_details1.neediness_details_code) != null)
                try
                {
                    dbCon.Execute<neediness_details>(ConvertNeedinessDetailsToEF(neediness_details1),
                    DBConnection.ExecuteActions.Update);
                    listOfNeedinessDetails = ConvertListToModel(dbCon.GetDbSet<neediness_details>().ToList());
                    return listOfNeedinessDetails.First(n => n.neediness_details_code == neediness_details1.neediness_details_code).neediness_details_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfNeedinessDetails.First(n => n.neediness_details_code == neediness_details1.neediness_details_code).neediness_details_code;
        }

        public bool DeleteneedinessDetailss(int neediness_details_code)
        {
            NeedinessDetailsModel needinessDetailsModel = listOfNeedinessDetails.Find(n => n.neediness_details_code == neediness_details_code);
            if ( needinessDetailsModel!= null)
                try
                {
                    dbCon.Execute<neediness_details>(ConvertNeedinessDetailsToEF(needinessDetailsModel),
                    DBConnection.ExecuteActions.Delete);
                    listOfNeedinessDetails = ConvertListToModel(dbCon.GetDbSet<neediness_details>().ToList());
                    return true; 
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        #region convert functions
        public static neediness_details ConvertNeedinessDetailsToEF(MODELS.NeedinessDetailsModel n)
        {
            return new neediness_details
            {
                neediness_details_code = n.neediness_details_code,
                needy_ID = n.needy_ID,
                org_code = n.org_code,
                weekly_hours = n.weekly_hours,
                details=n.details
            };
        }
        public static MODELS.NeedinessDetailsModel ConvertNeedinessDetailsToModel(neediness_details n)
        {
            return new MODELS.NeedinessDetailsModel
            {
                neediness_details_code = n.neediness_details_code,
                needy_ID = n.needy_ID,
                org_code = n.org_code,
                weekly_hours = n.weekly_hours,
                details = n.details
            };
        }

        public static List<MODELS.NeedinessDetailsModel> ConvertListToModel(List<neediness_details> li)
        {
            return li.Select(l => ConvertNeedinessDetailsToModel(l)).ToList();
        } 
        #endregion
    }
}
