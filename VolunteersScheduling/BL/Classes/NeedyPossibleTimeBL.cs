using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class NeedyPossibleTimeBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.NeedyPossibleTimeModel> listOfNeedyPossibleTime;

        public NeedyPossibleTimeBL()
        {
            dbCon = new DBConnection();
            listOfNeedyPossibleTime = ConvertListToModel(dbCon.GetDbSet<needy_possible_time>().ToList());
        }

        public List<NeedyPossibleTimeModel> GetAllNeedyPossibleTime()
        {
            return listOfNeedyPossibleTime;
        }

        public int InsertNeedyPossibleTime(NeedyPossibleTimeModel needyPossibleTime1)
        {
            try
            {
                dbCon.Execute<needy_possible_time>(ConvertNeedyPossibleTimeToEF(needyPossibleTime1),
                DBConnection.ExecuteActions.Insert);
                listOfNeedyPossibleTime = ConvertListToModel(dbCon.GetDbSet<needy_possible_time>().ToList());
                return listOfNeedyPossibleTime.Max(a => a.needy_possible_time_code);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateNeedyPossibleTime(NeedyPossibleTimeModel needyPossibleTime1)
        {
            if (listOfNeedyPossibleTime.Find(n => n.needy_possible_time_code == needyPossibleTime1.needy_possible_time_code) != null)
                try
                {
                    dbCon.Execute<needy_possible_time>(ConvertNeedyPossibleTimeToEF(needyPossibleTime1),
                    DBConnection.ExecuteActions.Update);
                    listOfNeedyPossibleTime = ConvertListToModel(dbCon.GetDbSet<needy_possible_time>().ToList());
                    return listOfNeedyPossibleTime.First(n => n.needy_possible_time_code == needyPossibleTime1.needy_possible_time_code).needy_possible_time_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfNeedyPossibleTime.First(n => n.needy_possible_time_code == needyPossibleTime1.needy_possible_time_code).needy_possible_time_code;
        }

        public bool DeleteNeedyPossibleTime(NeedyPossibleTimeModel needyPossibleTime1)
        {
            if (listOfNeedyPossibleTime.Find(n => n.needy_possible_time_code == needyPossibleTime1.needy_possible_time_code) != null)
                try
                {
                    dbCon.Execute<needy_possible_time>(ConvertNeedyPossibleTimeToEF(needyPossibleTime1),
                    DBConnection.ExecuteActions.Delete);
                    listOfNeedyPossibleTime = ConvertListToModel(dbCon.GetDbSet<needy_possible_time>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        #region convert functions
        public static needy_possible_time ConvertNeedyPossibleTimeToEF(MODELS.NeedyPossibleTimeModel n)
        {
            return new needy_possible_time
            {
                needy_possible_time_code = n.needy_possible_time_code,
                needy_details_code = n.neediness_details_code,
                time_slot_code = n.time_slot_code,
            };
        }
        public static MODELS.NeedyPossibleTimeModel ConvertNeedyPossibleTimeToModel(needy_possible_time n)
        {
            return new MODELS.NeedyPossibleTimeModel
            {
                neediness_details_code=n.needy_details_code,
                needy_possible_time_code=n.needy_possible_time_code,
                time_slot_code=n.time_slot_code,
            };
        }

        public static List<MODELS.NeedyPossibleTimeModel> ConvertListToModel(List<needy_possible_time> li)
        {
            return li.Select(l => ConvertNeedyPossibleTimeToModel(l)).ToList();
        }
        #endregion

        public List<TimeSlotModel> GetAllPossibleTimeSlots(int needinessDetailsCode)
        {
            List<time_slot> listOfAllTimeSlots = dbCon.GetDbSet<time_slot>();
            neediness_details possibleTimesOfNeedy = dbCon.GetDbSet<neediness_details>().AsQueryable().Include(a=>a.needy).ToList().Find(n=>n.neediness_details_code==needinessDetailsCode);
           var aso= dbCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy_possible_time.time_slot", "needy" });
            //var v = dbCon.GetDBSetWithInclude<neediness_details>(new string[] { "needy","needy_possible_time"}).Where(a=>a.neediness_details_code==needinessDetailsCode);
            List<time_slot> allNeedyPossibleTimeSLots = possibleTimesOfNeedy.needy_possible_time.AsQueryable().Include("time_slot").Select(t => t.time_slot).ToList();
            List<List<time_slot>> listOfLists = allNeedyPossibleTimeSLots.GroupBy(a => a.day_of_week).Select(a => a.ToList()).ToList();
            int startIndex, endIndex;
            var listOfGroupingTimeSlots = new List<time_slot>();
            var newTimeSlot = new time_slot();
            for (int i = 0; i < listOfLists.Count; i++)
            {
                listOfLists[i] = listOfLists[i].OrderBy(li => li.start_at_hour).ToList();
                for (int j = 0; j < listOfLists[i].Count; j++)
                {
                    startIndex = i;

                    while (listOfLists[i][j].start_at_hour + 1 == listOfLists[i][j + 1].start_at_hour)
                    {
                        j++;
                    }
                    endIndex = j;

                    newTimeSlot.start_at_hour = listOfLists[i][startIndex].start_at_hour;
                    newTimeSlot.end_at_hour = listOfLists[i][endIndex].start_at_hour;
                    newTimeSlot.start_at_date = DateTime.Compare(listOfLists[i][startIndex].start_at_date,listOfLists[i][endIndex].start_at_date) >0?listOfLists[i][startIndex].start_at_date : listOfLists[i][endIndex].start_at_date;
                    newTimeSlot.end_at_date = DateTime.Compare(listOfLists[i][startIndex].end_at_date, listOfLists[i][endIndex].end_at_date) > 0 ? listOfLists[i][startIndex].end_at_date : listOfLists[i][endIndex].end_at_date;
                    newTimeSlot.day_of_week = listOfLists[i][startIndex].day_of_week;

                    listOfGroupingTimeSlots.Add(newTimeSlot);
                }
            }
           return TimeSlotBL.ConvertListToModel(listOfGroupingTimeSlots);
        }
    }
}
