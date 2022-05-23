using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class OrganizationBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.OrganizationModel> listOfOrganizations;

        public OrganizationBL()
        {
            dbCon = new DBConnection();
            listOfOrganizations = ConvertListToModel(dbCon.GetDbSet<organization>().ToList());
        }

        public List<MODELS.OrganizationModel> GetAllOrganizations()
        {
            return listOfOrganizations;
        }

        public int InsertOrganization(MODELS.OrganizationModel Organization1)
        {
            if (listOfOrganizations.Find(o => o.org_code == Organization1.org_code) == null)
                try
                {
                    dbCon.Execute<organization>(ConvertOrganizationToEF(Organization1),
                    DBConnection.ExecuteActions.Insert);
                    listOfOrganizations = ConvertListToModel(dbCon.GetDbSet<organization>().ToList());
                    return listOfOrganizations.Max(o => o.org_code);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return 0;
        }

        public int UpdateOrganization(MODELS.OrganizationModel Organization1)
        {
            if (listOfOrganizations.Find(o => o.org_code == Organization1.org_code) != null)
                try
                {
                    dbCon.Execute<organization>(ConvertOrganizationToEF(Organization1),
                    DBConnection.ExecuteActions.Update);
                    listOfOrganizations = ConvertListToModel(dbCon.GetDbSet<organization>().ToList());
                    return listOfOrganizations.First(o => o.org_code == Organization1.org_code).org_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfOrganizations.First(o => o.org_code == Organization1.org_code).org_code;
        }

        public bool DeleteOrganization(MODELS.OrganizationModel Organization1)
        {
            if (listOfOrganizations.Find(o => o.org_code == Organization1.org_code) != null)
                try
                {
                    dbCon.Execute<organization>(ConvertOrganizationToEF(Organization1),
                    DBConnection.ExecuteActions.Update);
                    listOfOrganizations = ConvertListToModel(dbCon.GetDbSet<organization>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return true;
        }

        #region convert functions
        public static organization ConvertOrganizationToEF(MODELS.OrganizationModel o)
        {
            return new organization
            {
                org_code = o.org_code,
                org_name = o.org_name,
                org_platform = o.org_platform,
                org_min_age = o.org_min_age,
                need_scheduling = o.need_scheduling,
                activity_start_date = o.activity_start_date,
                activity_end_date = o.activity_end_date,
                avg_volunteering_time = o.avg_volunteering_time
            };
        }
        public static MODELS.OrganizationModel ConvertOrganizationToModel(organization o)
        {
            return new MODELS.OrganizationModel
            {
                org_code = o.org_code,
                org_name = o.org_name,
                org_platform = o.org_platform,
                org_min_age = o.org_min_age,
                need_scheduling=o.need_scheduling,
                activity_start_date=o.activity_start_date,
                activity_end_date=o.activity_end_date,
                avg_volunteering_time=o.avg_volunteering_time

            };
        }

        public static List<MODELS.OrganizationModel> ConvertListToModel(List<organization> li)
        {
            return li.Select(l => ConvertOrganizationToModel(l)).ToList();
        }
        #endregion

        public List<OrganizationModel> GetDisabledOrgsForVolunteer(string volunteerID)
        {
            VolunteerBL volunteerBL = new VolunteerBL();
            VolunteerModel volunteerModel = volunteerBL.GetAllvolunteers().First(v => v.volunteer_ID == volunteerID);
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            OrganizationModel orgModel;
            List<MODELS.VolunteeringDetailsModel> list1 = volunteeringDetailsBL.GetAllVolunteeringDetails().FindAll(v => v.volunteer_ID == volunteerID);
            List<MODELS.OrganizationModel> orgs = GetAllOrganizations();
            bool disable = false;
            var volunteerAge = DateTime.Today.Subtract(volunteerModel.volunteer_birth_date).TotalDays;
            List<OrganizationModel> disabledOrgs = new List<OrganizationModel>();
            for (int i = 0; i < orgs.Count; i++)
            {
              //  orgModel = GetAllOrganizations().First(o => o.org_code == orgs[i].org_code);
                disable = volunteerAge < orgs[i].org_min_age*365;
                if (!disable)
                {
                    foreach (var vd in list1)
                    {
                        if (orgs[i].org_code == vd.org_code)
                            disable = true;
                    }
                }
                if (disable)
                    disabledOrgs.Add(orgs[i]);
            }
            return disabledOrgs;
        }

        public List<OrganizationModel> GetFreeOrgsForVolunteer(string volunteerID)
        {
            List<MODELS.OrganizationModel> orgs = this.GetAllOrganizations().ToList();
            VolunteerBL volunteerBL = new VolunteerBL();
            VolunteerModel volunteer = volunteerBL.GetAllvolunteers().First(v => v.volunteer_ID == volunteerID);
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            List<MODELS.VolunteeringDetailsModel> list1 = volunteeringDetailsBL.GetAllVolunteeringDetails().FindAll(v => v.volunteer_ID == volunteerID);
            List<OrganizationModel> possibleOrgs = new List<OrganizationModel>();
            bool disable = false;
            //foreach (OrganizationModel org in orgs)
            for (int i = 0; i < orgs.Count; i++)
            {
                var volunteerAge=DateTime.Today.Subtract(volunteer.volunteer_birth_date).TotalDays;
                //foreach (VolunteeringDetailsModel vd in list1)
                //{
                //    var volunteerAge=DateTime.Today.Subtract(volunteer.volunteer_birth_date).TotalDays;
                //    var minAge =orgs[i].org_min_age * 365;
                //    if ((orgs[i].org_code == vd.org_code) || (volunteerAge > minAge))
                //        orgs.Remove(orgs[i]);
                //}
                disable = volunteerAge < orgs[i].org_min_age * 365;
                if (!disable)
                {
                    foreach (var vd in list1)
                    {
                        if (orgs[i].org_code == vd.org_code)
                            disable = true;
                    }
                }
                if (!disable)
                    possibleOrgs.Add(orgs[i]);
            }
            return possibleOrgs;
            //  return orgs.FindAll(o => list1.Find(v=>v.org_code==o.org_code));
        }
    }
}
