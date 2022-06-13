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

        TimeSlotBL timeSlotBL = new TimeSlotBL();

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


        public bool DeleteVolunteerPossibleTimeCode(int timeSlotCode)
        {
            try
            {
                VolunteerPossibleTimeModel volunteerPossibleTime = GetAllVolunteerPossibleTime().First(t => t.time_slot_code == timeSlotCode);
                if (this.DeleteVolunteerPossibleTime(volunteerPossibleTime.time_slot_code))
                {
                    timeSlotBL.DeleteTimeSlot(timeSlotCode);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public List<TimeSlotModel> GetAllPossibleTimeSlots(int volunteeringDetailsCode)
        {
            List<volunteer_possible_time> listOfVolunteerPossibleTime = dbCon.GetDbSetWithIncludes<volunteer_possible_time>(new string[] { "time_slot" });
            listOfVolunteerPossibleTime = listOfVolunteerPossibleTime.FindAll(t => t.volunteering_details_code == volunteeringDetailsCode);
            return TimeSlotBL.ConvertListToModel(listOfVolunteerPossibleTime.Select(n => n.time_slot).ToList()).ToList();
        }

        public bool AddListOfPossibleTime(List<TimeSlotModel> listOfTimeSlots, int volunteeringDetailsCode)
        {
            int timeSlotCode;
            var volunteerPosisibleTimeModel = new VolunteerPossibleTimeModel();
            volunteerPosisibleTimeModel.volunteering_details_code = volunteeringDetailsCode;

            try
            {
                foreach (var item in listOfTimeSlots)
                {
                    timeSlotCode = timeSlotBL.InsertTimeSlot(item);
                    volunteerPosisibleTimeModel.time_slot_code = timeSlotCode;
                    InsertVolunteerPossibleTime(volunteerPosisibleTimeModel);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public int GetConflicts(int volunteeringDetailsCode)
        {
            var listOfSchedule = dbCon.GetDbSetWithIncludes<schedule>(new string[] { "time_slot" })
                                      .FindAll(s=>s.volunteering_details_code==volunteeringDetailsCode)
                                      .ToList();

            var listOfPossibleTime = dbCon.GetDbSetWithIncludes<volunteer_possible_time>(new string[] { "time_slot" })
                                          .FindAll(vpt => vpt.volunteering_details_code == volunteeringDetailsCode)
                                          .ToList();
            int conflictsCounter = 0;

            foreach (var schedule in listOfSchedule)
            {
                foreach (var possibleTime in listOfPossibleTime)
                {
                    if (!GeneticScheduling.CheckIfSlotsAreOverlaps(schedule.time_slot, possibleTime.time_slot))
                        conflictsCounter++;
                }
            }
            return conflictsCounter;
        }

        public bool DeleteConflicts(int volunteeringDetailsCode)
        {
            var listOfSchedule = dbCon.GetDbSetWithIncludes<schedule>(new string[] { "time_slot" })
                                      .FindAll(s => s.volunteering_details_code == volunteeringDetailsCode)
                                      .ToList();

            var listOfPossibleTime = dbCon.GetDbSetWithIncludes<volunteer_possible_time>(new string[] { "time_slot" })
                                          .FindAll(vpt => vpt.volunteering_details_code == volunteeringDetailsCode)
                                          .ToList();

            foreach (var schedule in listOfSchedule)
            {
                foreach (var possibleTime in listOfPossibleTime)
                {
                    if (!GeneticScheduling.CheckIfSlotsAreOverlaps(schedule.time_slot, possibleTime.time_slot))
                        this.DeleteToDB<schedule>(schedule);
                }
            }
            return true;
        }
    }
}
