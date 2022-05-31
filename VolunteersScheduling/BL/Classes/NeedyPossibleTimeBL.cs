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

        TimeSlotBL timeSlotBL = new TimeSlotBL();

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

        public bool DeleteNeedyPossibleTimeCode(int timeSlotCode)
        {
            try
            {
                NeedyPossibleTimeModel needyPossibleTime = GetAllNeedyPossibleTime().First(n => n.time_slot_code == timeSlotCode);
                if (this.DeleteNeedyPossibleTime(needyPossibleTime))
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

        public bool AddListOfPossibleTime(List<TimeSlotModel> listOfTimeSlots, int needinessDetailsCode)
        {
            int timeSlotCode;
            var needyPosisibleTimeModel = new NeedyPossibleTimeModel();
            needyPosisibleTimeModel.neediness_details_code = needinessDetailsCode;

            try
            {
                foreach (var item in listOfTimeSlots)
                {
                    timeSlotCode = timeSlotBL.InsertTimeSlot(item);
                    needyPosisibleTimeModel.time_slot_code = timeSlotCode;
                    InsertNeedyPossibleTime(needyPosisibleTimeModel);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<TimeSlotModel> GetAllPossibleTimeSlots(int needinessDetailsCode)
        {
            List<needy_possible_time> listOfNeedyPossibleTime = dbCon.GetDbSetWithIncludes<needy_possible_time>(new string[] { "time_slot" });
            listOfNeedyPossibleTime = listOfNeedyPossibleTime.FindAll(t => t.needy_details_code == needinessDetailsCode);
            return TimeSlotBL.ConvertListToModel(listOfNeedyPossibleTime.Select(n=>n.time_slot).ToList()).ToList();
        }
    }
}
