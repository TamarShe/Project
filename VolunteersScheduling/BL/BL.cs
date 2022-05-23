using Accord.Genetic;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Classes;

namespace BL
{
    public class BL
    {

        public void Gentic(int orgCode)
        {
            DBConnection dBConnection = new DBConnection();

            //עדכון הלוח הקיים של הארגון שכל מה שיש יגמר בתאריך ההתחלה שהכניס
            List<schedule> scheduleToDelete = dBConnection.GetDbSet<schedule>().FindAll(s => s.neediness_details.organization.org_code == orgCode).ToList();
            foreach (var item in scheduleToDelete)
            {
                item.time_slot.end_at_date = DateTime.Today;
                dBConnection.Execute<schedule>(item, DBConnection.ExecuteActions.Update);
            }


            Population population = new Population(100, new TimeTableChromosome(dBConnection, orgCode),
                                                    new TimeTableChromosome.FitnessFunction(), new EliteSelection());
            int i = 0;
            while (true)
            {
                population.RunEpoch();
                i++;
                if (population.FitnessMax >= 0.99 || i >= 100)
                {
                    break;
                }
            }

            TimeSlotProperties timeSlotProperties = new TimeSlotProperties();
            List<TimeSlotProperties> value = (population.BestChromosome as TimeTableChromosome).timeSlotsPropertiesList.ToList();
            
            ////להכניס את הנתונים לDB
            //foreach (var val in value)
            //{
            //    h.InsertSettingHour(val);
            //}


            ////לבדוק האם כל השעות שהמבוגר ביקש שובצו, ואם לא- להפעיל את הפונקציה של שליחת מייל
            ////רשימה של כל קרובי המשפחה של המבוגר שהתקבל
            //var listOfRelativesToAdult = listOfRelatives.Where(r => r.adultId == adultId).ToList();
            ////רשימה של כל השעות הדרושות למבוגר שהתקבל
            //var listofDemandHoursToAdult = listOfAdultDemandHours.Where(d => d.adultId == adultId).ToList();
            ////listofDemandHoursToAdult = listofDemandHoursToAdult.Where(la => value.Find(v => v.scheduleHourCode ==la) == null);
            //List<adultDemandHours> missingHours = new List<adultDemandHours>();
            //foreach (var item in listofDemandHoursToAdult)
            //{
            //    if (value.Find(v => v.scheduleHourCode == item.scheduleHourCode) == null && missingHours.Contains(item) == false)
            //        missingHours.Add(item);
            //}
            //if (missingHours != null)
            //{
            //    List<scheduleHours> l = new List<scheduleHours>();
            //    foreach (var item in missingHours)
            //    {
            //        l.Add(listOfScheduleHours.First(s => s.code == item.scheduleHourCode));
            //    }
            //    //מכאן השינוי
            //    // List<Days> listOfDaysMissingHours = GetAllDays().Where(d => l.Find(l6=>l6.dayCode== d.code).dayCode==d.code).ToList();
            //    // List<Hours> listOfHoursMissingHours=GetAllHours().Where(h1 => l.Find(l7 => l7.hourCode == h1.code).hourCode == h1.code).ToList();
            //    //עד כאן

            //    //listOfRelativesToAdult.Select(r=>r.mail).ToList()
            //    List<string> l8 = new List<string>();
            //    l8.Add("s0556749180@gmail.com");

            //    EmailSending.SendMailToRelatives(l, l8, "מעדכן אותך אודות שעות שלא משובצות Twenty for saba application");
            //}

        }
    }
}
