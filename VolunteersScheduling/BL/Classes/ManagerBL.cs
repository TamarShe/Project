using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Genetic;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class ManagerBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.ManagerModel> listOfManagers;

        public ManagerBL()
        {
            dbCon = new DBConnection();
            listOfManagers =ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
        }

        public List<MODELS.ManagerModel> GetAllManagers()
        {
            return listOfManagers;
        }

        public string InsertManager(MODELS.ManagerModel manager1)
        {
            if (listOfManagers.Find(m => m.manager_Id == manager1.manager_Id) == null)
                try
                {
                    dbCon.Execute<manager>(ConvertManagerToEF(manager1),
                    DBConnection.ExecuteActions.Insert);
                    listOfManagers = ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
                    return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
        } 

        public string UpdateManager(MODELS.ManagerModel manager1)
        {
            if (listOfManagers.Find(m => m.manager_Id == manager1.manager_Id) != null)
                try
                {
                    dbCon.Execute<manager>(ConvertManagerToEF(manager1),
                    DBConnection.ExecuteActions.Update);
                    listOfManagers = ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
                    return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
                }
                catch (Exception ex)
                {
                    return "";
                }
            return listOfManagers.First(m => m.manager_Id == manager1.manager_Id).manager_Id;
        } 

        public bool DeleteManager(string managerID)
        {
            ManagerModel manager1 = listOfManagers.First(manager => manager.manager_Id == managerID);
            if (manager1!= null)
                try
                {
                    dbCon.Execute<manager>(ConvertManagerToEF(manager1),
                    DBConnection.ExecuteActions.Insert);
                    listOfManagers = ConvertListToModel(dbCon.GetDbSet<manager>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        } 

        #region convert functions
        public static manager ConvertManagerToEF(MODELS.ManagerModel m)
        {
            return new manager
            {
                manager_ID = m.manager_Id,
                manager_full_name = m.manager_full_name,
                manager_email = m.manager_email,
                manager_phone = m.manager_phone,
                manager_org_code = m.manager_org_code,
                manager_password = m.manager_password,
                is_general_manager = m.is_general_manager,
            };
        }
        public static MODELS.ManagerModel ConvertManagerToModel(manager m)
        {
            return new MODELS.ManagerModel
            {
                manager_Id = m.manager_ID,
                manager_full_name = m.manager_full_name,
                manager_email = m.manager_email,
                manager_phone = m.manager_phone,
                manager_org_code = m.manager_org_code,
                manager_password = m.manager_password,
                is_general_manager = m.is_general_manager,
            };
        }

        public static List<MODELS.ManagerModel> ConvertListToModel(List<manager> li)
        {
            return li.Select(l => ConvertManagerToModel(l)).ToList();
        }
        #endregion

        public void Gentic(int orgCode)
        {
            DBConnection dBConnection = new DBConnection();

            //עדכון הלוח הקיים של הארגון שכל מה שיש עד עכשיו יגמר בתאריך של היום כי מעכשיו תהיה מערכת חדשה
            List<schedule> scheduleToDelete = dBConnection.GetDbSetWithIncludes<schedule>(new string[] { "neediness_details.organization","time_slot" }).FindAll(s => s.neediness_details.organization.org_code == orgCode).ToList();
            foreach (var item in scheduleToDelete)
            {
                item.time_slot.end_at_date = DateTime.Today;
                dBConnection.Execute<time_slot>(item.time_slot, DBConnection.ExecuteActions.Update);

                //dBConnection.Execute<schedule>(item, DBConnection.ExecuteActions.Delete);
            }

            Population population = new Population(300, new TimeTableChromosome(dBConnection, orgCode), new TimeTableChromosome.FitnessFunction(), new EliteSelection());
            int i = 0;
            while (true)
            {
                population.RunEpoch();
                i++;
                if (population.FitnessMax >= 0.99 || i >= 1000)
                {
                    break;
                }
            }

            List<TimeSlotProperties> value = (population.BestChromosome as TimeTableChromosome).timeSlotsPropertiesList.ToList();

            //הכנסת פרטי השיבוץ למסד הנתונים
            schedule newScheduleSlot = new schedule();
            time_slot newTimeSlot;
            TimeSlotBL timeSlotBL = new TimeSlotBL();
            List<neediness_details> listOfNeedinessDetails = dbCon.GetDbSet<neediness_details>();
            List<volunteering_details> listOfVolunteeringDetails = dbCon.GetDbSet<volunteering_details>();

            foreach (var item in value)
            {
                newTimeSlot = new time_slot();
                newTimeSlot.start_at_date = item.time.start_at_date;
                newTimeSlot.end_at_date = item.time.end_at_date;
                newTimeSlot.start_at_hour = item.time.start_at_hour;
                newTimeSlot.end_at_hour = item.time.end_at_hour;
                newTimeSlot.day_of_week = item.time.day_of_week;
                newScheduleSlot.time_slot_code = timeSlotBL.InsertTimeSlot(TimeSlotBL.ConvertTimeSlotToModel(newTimeSlot));
                newScheduleSlot.volunteering_details_code = listOfVolunteeringDetails.Find(v => v.volunteer_ID == item.volunteer.volunteer_ID && v.org_code == item.orgCode).volunteering_details_code;
                newScheduleSlot.neediness_details_code = listOfNeedinessDetails.Find(n=>n.needy_ID==item.needy.needy_ID && n.org_code==item.orgCode).neediness_details_code;              
                dBConnection.Execute<schedule>((newScheduleSlot), DBConnection.ExecuteActions.Insert);
            }
        }
    }  
}
