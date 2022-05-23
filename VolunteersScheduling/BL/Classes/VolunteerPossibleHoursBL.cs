using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class VolunteerPossibleHoursBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.VolunteerPossibleTimeModel> listOfVolunteerPossibleTime;

        public VolunteerPossibleHoursBL()
        {
            dbCon = new DBConnection();
            listOfVolunteerPossibleTime = ConvertListToModel(dbCon.GetDbSet<volunteer_possible_time>().ToList());
        }

        public List<MODELS.VolunteerPossibleTimeModel> GetAllVolunteerPossibleTime()
        {
            return listOfVolunteerPossibleTime;
        }

        public int InsertVolunteerPossibleTime(MODELS.VolunteerPossibleTimeModel volunteerPossibleTime1)
        {
            //volunteerPossibleTime1.volunteers_possible_time_code = (listOfVolunteerPossibleTime.Max(a => a.volunteers_possible_time_code)) + 1;
            try
            {
                dbCon.Execute<volunteer_possible_time>(ConvertVolunteerPossibleTimeToEF(volunteerPossibleTime1), DBConnection.ExecuteActions.Insert);
                listOfVolunteerPossibleTime = ConvertListToModel(dbCon.GetDbSet<volunteer_possible_time>().ToList());
                return listOfVolunteerPossibleTime.Max(a => a.volunteers_possible_time_code);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateVolunteerPossibleTime(MODELS.VolunteerPossibleTimeModel volunteerPossibleTime1)
        {
            if (listOfVolunteerPossibleTime.Find(v => v.volunteers_possible_time_code == volunteerPossibleTime1.volunteers_possible_time_code) != null)
                try
                {
                    dbCon.Execute<volunteer_possible_time>(ConvertVolunteerPossibleTimeToEF(volunteerPossibleTime1),
                    DBConnection.ExecuteActions.Update);
                    listOfVolunteerPossibleTime = ConvertListToModel(dbCon.GetDbSet<volunteer_possible_time>().ToList());
                    return listOfVolunteerPossibleTime.First(v => v.volunteers_possible_time_code == volunteerPossibleTime1.volunteers_possible_time_code).volunteers_possible_time_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfVolunteerPossibleTime.First(v => v.volunteers_possible_time_code == volunteerPossibleTime1.volunteers_possible_time_code).volunteers_possible_time_code;
        }

        public bool DeleteVolunteerPossibleTime(int code)
        {
            VolunteerPossibleTimeModel volunteerPossibleTime1 = listOfVolunteerPossibleTime.First(a => code == a.volunteers_possible_time_code);
            if (listOfVolunteerPossibleTime.Find(v => v.volunteers_possible_time_code == volunteerPossibleTime1.volunteers_possible_time_code) != null)
                try
                {
                    dbCon.Execute<volunteer_possible_time>(ConvertVolunteerPossibleTimeToEF(volunteerPossibleTime1),
                    DBConnection.ExecuteActions.Delete);
                    listOfVolunteerPossibleTime = ConvertListToModel(dbCon.GetDbSet<volunteer_possible_time>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        #region convert functions
        public static volunteer_possible_time ConvertVolunteerPossibleTimeToEF(MODELS.VolunteerPossibleTimeModel v)
        {
            return new volunteer_possible_time
            {
                volunteers_possible_time_code=v.volunteers_possible_time_code,
                volunteering_details_code=v.volunteering_details_code,
                time_slot_code=v.time_slot_code,
            };
        }
        public static MODELS.VolunteerPossibleTimeModel ConvertVolunteeringDetailsToModel(volunteer_possible_time v)
        {
            return new MODELS.VolunteerPossibleTimeModel
            {
                volunteers_possible_time_code = v.volunteers_possible_time_code,
                time_slot_code = v.time_slot_code,
                volunteering_details_code=v.volunteering_details_code
            };
        }

        public static List<MODELS.VolunteerPossibleTimeModel> ConvertListToModel(List<volunteer_possible_time> li)
        {
            return li.Select(l => ConvertVolunteeringDetailsToModel(l)).ToList();
        }
        #endregion

        public bool GetConflicts(int volunteeringDetailsCode)
        {
            bool findConflicts = false;
            ScheduleBL scheduleBL = new ScheduleBL();
            TimeSlotBL timeSlotBL = new TimeSlotBL();
            List<TimeSlotModel> allTimeSlots = timeSlotBL.GetAllTimeSlot();
            List<ScheduleModel> vScehduleStarts = scheduleBL.GetAllSchedule().FindAll(a => a.volunteering_details_code == volunteeringDetailsCode).ToList();
            List<VolunteerPossibleTimeModel> vPossibleTimeStarts = GetAllVolunteerPossibleTime().FindAll(v => v.volunteering_details_code == volunteeringDetailsCode).ToList();
            List<TimeSlotModel> slotsOfSchedule = new List<TimeSlotModel>();
            List<TimeSlotModel> slotsOfPossibleTime =new List<TimeSlotModel>();
            //foreach (var item in allTimeSlots)
            //{
            //    if(slotsOfSchedule.Find(a=>a.start_at_hour==))
            //}


            //לגמור את זה ולהעתיק לזמן נזקק
            return slotsOfSchedule.Intersect(slotsOfPossibleTime).Count()>0;
        }
    }
}
