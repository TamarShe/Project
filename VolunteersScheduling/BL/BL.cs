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

            //עדכון הלוח הקיים של הארגון שכל מה שיש עד עכשיו יגמר בתאריך של היום כי מעכשיו תהיה מערכת חדשה
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

            List<TimeSlotProperties> value = (population.BestChromosome as TimeTableChromosome).timeSlotsPropertiesList.ToList();

            //הכנסת פרטי השיבוץ למסד הנתונים
            schedule newScheduleSlot = new schedule();

            foreach (var item in value)
            {
                newScheduleSlot.time_slot_code = item.time.time_slot_code;
                newScheduleSlot.volunteering_details_code = item.volunteer.volunteering_details.First(v => v.org_code == item.orgCode).volunteering_details_code;
                newScheduleSlot.neediness_details_code = item.needy.neediness_details.First(n => n.org_code == item.orgCode).neediness_details_code;
                dBConnection.Execute<schedule>((newScheduleSlot),DBConnection.ExecuteActions.Insert);
            }
        }
    }
}
