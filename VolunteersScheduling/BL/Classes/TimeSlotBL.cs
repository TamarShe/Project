using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class TimeSlotBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.TimeSlotModel> listOfTimeSlot;

        public TimeSlotBL()
        {
            dbCon = new DBConnection();
            listOfTimeSlot = ConvertListToModel(dbCon.GetDbSet<time_slot>().ToList());
        }

        public List<MODELS.TimeSlotModel> GetAllTimeSlot()
        {
            return listOfTimeSlot;
        }

        public int InsertTimeSlot(MODELS.TimeSlotModel TimeSlot1)
        {
            try
            {
                dbCon.Execute<time_slot>(ConvertTimeSlotToEF(TimeSlot1),DBConnection.ExecuteActions.Insert);
                listOfTimeSlot = ConvertListToModel(dbCon.GetDbSet<time_slot>().ToList());
                return listOfTimeSlot.Max(a => a.time_slot_code);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateTimeSlot(MODELS.TimeSlotModel TimeSlot1)
        {
            if (listOfTimeSlot.Find(t => t.time_slot_code == TimeSlot1.time_slot_code) != null)
                try
                {
                    dbCon.Execute<time_slot>(ConvertTimeSlotToEF(TimeSlot1),
                    DBConnection.ExecuteActions.Update);
                    listOfTimeSlot = ConvertListToModel(dbCon.GetDbSet<time_slot>().ToList());
                    return listOfTimeSlot.First(t => t.time_slot_code == TimeSlot1.time_slot_code).time_slot_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfTimeSlot.First(t => t.time_slot_code == TimeSlot1.time_slot_code).time_slot_code;
        }

        public bool DeleteTimeSlot(int code)
        {
            TimeSlotModel TimeSlot1 = listOfTimeSlot.Find(t => t.time_slot_code == code);
            if (TimeSlot1 != null)
                try
                {
                    dbCon.Execute<time_slot>(ConvertTimeSlotToEF(TimeSlot1),
                    DBConnection.ExecuteActions.Delete);
                    listOfTimeSlot = ConvertListToModel(dbCon.GetDbSet<time_slot>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return true;
        }

        #region convert functions
        public static time_slot ConvertTimeSlotToEF(MODELS.TimeSlotModel t)
        {
            return new time_slot
            {
                time_slot_code=t.time_slot_code,
                day_of_week=t.day_of_week,
                start_at_date=t.start_at_date,
                end_at_date=t.end_at_date,
                start_at_hour=t.start_at_hour,
                end_at_hour=t.end_at_hour
            };
        }
        public static MODELS.TimeSlotModel ConvertTimeSlotToModel(time_slot t)
        {
            return new MODELS.TimeSlotModel
            {
                time_slot_code = t.time_slot_code,
                day_of_week = t.day_of_week,
                start_at_date = t.start_at_date,
                end_at_date = t.end_at_date,
                start_at_hour = t.start_at_hour,
                end_at_hour = t.end_at_hour
            };
        }

        public static List<MODELS.TimeSlotModel> ConvertListToModel(List<time_slot> li)
        {
            return li.Select(l => ConvertTimeSlotToModel(l)).ToList();
        }
        #endregion

        //מקבלת רשימה של זמנים אפשרים לנזקק ומספר שעות התנדבות בארגון ומחזירה רשימה של כל הצירופים האפשרים שיכול להיות לפי הזמן התנדבות
        //public List<TimeSlotModel> GetListOfDividedTimeSlots(List<TimeSlotModel> listOfTimeSlots, int volunteeringDuration)
        //{
        //    List<TimeSlotModel> dividedTimeSlots = new List<TimeSlotModel>();
        //    var newTimeSlot = new TimeSlotModel();
        //    TimeSpan start = new TimeSpan();
        //    TimeSpan end = new TimeSpan();
        //    TimeSpan currentEnd = new TimeSpan();

        //    foreach (var item in listOfTimeSlots)
        //    {
        //        start = item.start_at_hour;
        //        end = item.end_at_hour;
        //        TimeSpan duration = TimeSpan.FromMinutes(volunteeringDuration);
        //        TimeSpan quarter = TimeSpan.FromMinutes(15);

        //        //כל עוד ההפרש בין השעת התחלה לסיום גדול או שווה לזמן ההתנדבות
        //        while (end.Subtract(start).TotalMinutes >= volunteeringDuration)
        //        {
        //            currentEnd = start + duration;
        //            newTimeSlot = new TimeSlotModel();
        //            newTimeSlot.start_at_date = item.start_at_date;
        //            newTimeSlot.end_at_date = item.end_at_date;
        //            newTimeSlot.day_of_week = item.day_of_week;
        //            newTimeSlot.start_at_hour = start;
        //            newTimeSlot.end_at_hour = currentEnd;
        //            dividedTimeSlots.Add(newTimeSlot);
        //            start += quarter;
        //        }
        //    }
        //    return dividedTimeSlots;
        //}
    }
}
