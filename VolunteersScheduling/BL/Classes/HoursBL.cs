using DAL;
using MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Classes
{
    public class HoursBL
    {
        DBConnection dbCon;
        List<HourModel> listOfHours;

        public HoursBL()
        {
            dbCon = new DBConnection();
            listOfHours = ConvertListToModel(dbCon.GetDbSet<hour>().ToList());
        }

        public List<HourModel> GetAllHours()
        {
            return listOfHours;
        }

        public int InsertHour(HourModel Hour1)
        {
            if (listOfHours.Find(v => v.hour_code == Hour1.hour_code) == null)
                try
                {
                    dbCon.Execute<hour>(ConvertHourToEF(Hour1),
                    DBConnection.ExecuteActions.Insert);
                    listOfHours = ConvertListToModel(dbCon.GetDbSet<hour>().ToList());
                    return listOfHours.First(v => v.hour_code == Hour1.hour_code).hour_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return 0;
        }

        public int UpdateHour(HourModel Hour1)
        {
            if (listOfHours.Find(v => v.hour_code == Hour1.hour_code) != null)
                try
                {
                    dbCon.Execute<hour>(ConvertHourToEF(Hour1),
                    DBConnection.ExecuteActions.Update);
                    listOfHours = ConvertListToModel(dbCon.GetDbSet<hour>().ToList());
                    return listOfHours.First(v => v.hour_code == Hour1.hour_code).hour_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return 0;
        }

        public bool DeleteHour(int hourCode)
        {
            HourModel Hour1 = listOfHours.First(Hour => Hour.hour_code == hourCode);
            if (Hour1 != null)
                try
                {
                    dbCon.Execute<hour>(ConvertHourToEF(Hour1),
                    DBConnection.ExecuteActions.Delete);
                    listOfHours = ConvertListToModel(dbCon.GetDbSet<hour>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        #region convert functions
        public static hour ConvertHourToEF(MODELS.HourModel v)
        {
            return new hour
            {
                hour_code = v.hour_code,
                at_hour = v.at_hour,
            };
        }
        public static MODELS.HourModel ConvertHourToModel(hour v)
        {
            return new MODELS.HourModel
            {
                hour_code = v.hour_code,
                at_hour = v.at_hour,
            };
        }

        public static List<MODELS.HourModel> ConvertListToModel(List<hour> li)
        {
            return li.Select(l => ConvertHourToModel(l)).ToList();
        }
        #endregion

        public HourModel[,] GetListOfStartAndEnd(int timeDuration)
        {


            int mone = 0;
            List<HourModel> allTimeSlots = GetAllHours();
            List<HourModel[]> list = new List<HourModel[]>();
            HourModel[] startAndEnd = new HourModel[2];

            TimeSpan duration;

            foreach (var item in allTimeSlots)
            {
                startAndEnd = new HourModel[2];
                startAndEnd[0] = item;
                try
                {
                    duration = TimeSpan.FromMinutes(timeDuration);
                    startAndEnd[1] = allTimeSlots.First(ts => ts.at_hour == (item.at_hour+duration));
                    list.Add(startAndEnd);
                    mone++;
                }
                catch
                {
                    break;
                }
            }
            HourModel[,] hours = new HourModel[mone, 2];
            for (int i = 0; i < mone; i++)
            {
                hours[i, 0] = list[i][0];
                hours[i, 1] = list[i][1];
            }
            return hours;
        }
    }
}
