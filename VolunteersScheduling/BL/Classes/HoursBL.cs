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

  

        #region convert functions

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
