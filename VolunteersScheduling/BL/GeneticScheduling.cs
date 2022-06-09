using Accord.Genetic;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class GeneticScheduling : ChromosomeBase
    {
        private readonly DBConnection DBCon = new DBConnection();
        private organization currentOrg;
        private static Random Random = new Random();

        private List<organization> listOfOrganizations;
        private List<volunteer> listOfVolunteers;
        private List<needy> listOfNeedies;
        private List<volunteering_details> listOfVolunteeringDetails;
        private List<neediness_details> listOfNeedinessDetails;
        private List<volunteer_possible_time> listOfVolunteersPossibleTime;
        private List<needy_possible_time> listOfNeediesPossibleTimes;
        private List<time_slot> listOfTimeSlots;
        private List<hour> listOfHours;

        public List<TimeSlotProperties> timeSlotsPropertiesList = new List<TimeSlotProperties>();

        public GeneticScheduling(DBConnection db, int orgCode)
        {
            DBCon = db;
            InitializeLists();
            this.currentOrg = listOfOrganizations.Find(org => org.org_code == orgCode);
            Generate();
        }

        public GeneticScheduling(DBConnection db, List<TimeSlotProperties> slots, int orgCode)
        {
            this.DBCon = db;
            this.timeSlotsPropertiesList = slots;
            InitializeLists();
            this.currentOrg = listOfOrganizations.Find(org => org.org_code == orgCode);
        }

        public void InitializeLists()
        {
            listOfOrganizations = DBCon.GetDbSet<organization>().ToList();
            listOfVolunteers = DBCon.GetDbSet<volunteer>().ToList();
            listOfNeedies = DBCon.GetDbSet<needy>().ToList();
            listOfVolunteeringDetails = DBCon.GetDbSetWithIncludes<volunteering_details>(new string[] { "volunteer_possible_time.time_slot", "volunteer" }).ToList();
            listOfNeedinessDetails = DBCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy_possible_time.time_slot", "needy" }).ToList();
            listOfVolunteersPossibleTime = DBCon.GetDbSetWithIncludes<volunteer_possible_time>(new string[] { "volunteering_details", "time_slot" }).ToList();
            listOfNeediesPossibleTimes = DBCon.GetDbSetWithIncludes<needy_possible_time>(new string[] { "neediness_details", "time_slot" }).ToList();
            listOfTimeSlots = DBCon.GetDbSet<time_slot>().ToList();
            listOfHours = DBCon.GetDbSet<hour>().ToList();
        }

        public override IChromosome CreateNew()
        {
            var geneticScheduling = new GeneticScheduling(DBCon, currentOrg.org_code);
            geneticScheduling.Generate();
            return geneticScheduling;
        }

        public int RandomIndex(int limit)
        {
            return Random.Next(0, limit);
        }

        //מוצא את המתנדבים היכולים בזמן מסוים, מבחינת ההעדפות
        public List<volunteering_details> GetPossibleVolunteersForTimeSlot(time_slot timeSlot)
        {
            var listOfVolunteeringDetailsInCurrentOrg = listOfVolunteeringDetails.FindAll(vd => vd.org_code == currentOrg.org_code).ToList();
            var listOfPossibleVolunteersInTheCurrentHour = listOfVolunteeringDetailsInCurrentOrg.FindAll(
                vd => vd.volunteer_possible_time.ToList().FindAll(
                        vpt => vpt.time_slot.start_at_hour <= timeSlot.start_at_hour
                            && vpt.time_slot.end_at_hour >= timeSlot.start_at_hour + (currentOrg.avg_volunteering_time / 15)
                            && vpt.time_slot.day_of_week == timeSlot.day_of_week)
                .Count > 0).ToList();

            return listOfPossibleVolunteersInTheCurrentHour;
        }

        //מקבל זמן נצרך לנזקק ומחלק אותו למשמרות 
        public List<time_slot> DivideSlotToShifts(time_slot slot)
        {
            var hoursForShift = this.currentOrg.avg_volunteering_time;
            var start = slot.start_at_hour;
            var end = slot.end_at_hour;
            List<time_slot> listOfTimeSlots = new List<time_slot>();
            var newTimeSlot = new time_slot();

            while (start+(hoursForShift*4)<=end)
            {
                newTimeSlot = new time_slot();
                newTimeSlot.day_of_week = slot.day_of_week;
                newTimeSlot.start_at_hour = start;
                newTimeSlot.end_at_hour = start + (hoursForShift * 4);
                newTimeSlot.start_at_date = slot.start_at_date;
                newTimeSlot.end_at_date = slot.end_at_date;
                listOfTimeSlots.Add(newTimeSlot);
                start += (hoursForShift * 4);
            }
            if(start<end)
            {
                var diff = end - start;
                if (diff < (currentOrg.avg_volunteering_time / 15) / 2)
                    listOfTimeSlots[listOfTimeSlots.Count - 1].end_at_hour += diff;
                else
                {
                    newTimeSlot = new time_slot();
                    newTimeSlot.day_of_week = slot.day_of_week;
                    newTimeSlot.start_at_hour = start;
                    newTimeSlot.end_at_hour = start + diff;
                    newTimeSlot.start_at_date = slot.start_at_date;
                    newTimeSlot.end_at_date = slot.end_at_date;
                    listOfTimeSlots.Add(newTimeSlot);
                }
            }

            return listOfTimeSlots;
        }
        public override void Generate()
        {
            var timeSlotProperties = new TimeSlotProperties();

            //רשימת הנזקקים שקשורים לארגון הזה
            var listOfNeedinessDetailsInCurrentOrg = listOfNeedinessDetails.FindAll(nd => nd.org_code == currentOrg.org_code).ToList();

            //רשימת כל הזמנים האפשרים למתנדב הרלוונטים לארגון
            var listOfPossibleVolunteersInTheCurrentHour = new List<volunteering_details>();

            var currentNeedy = new needy();

            //רשימת המשבצות זמן הדרושות לנזקק הנוכחי
            var listOfRequiredTimeSlotsOfCurrentNeedy = new List<time_slot>();

            foreach (var currentNeedinessDetails in listOfNeedinessDetailsInCurrentOrg)
            {
                //אתחול המשתנים שיתאימו לנזקק הנוכחי
                currentNeedy = listOfNeedies.First(needy => needy.needy_ID == currentNeedinessDetails.needy_ID);

                //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי זקוק להם בארגון הזה
                listOfRequiredTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).ToList();
                
                //כל המשמרות שיוצא שהוא צריך
                var listOfShiftsOfNeedy = new List<time_slot>();

                for (int i = 0; i < listOfRequiredTimeSlotsOfCurrentNeedy.Count; i++)
                {
                    listOfShiftsOfNeedy.AddRange(DivideSlotToShifts(listOfRequiredTimeSlotsOfCurrentNeedy[i]));
                }

                //עבור כל אחת מהמשמרות
                for (int i = 0; i < listOfShiftsOfNeedy.Count; i++)
                {
                    var currentSlot = listOfShiftsOfNeedy[i];

                    //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
                    listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForTimeSlot(currentSlot);

                    //אם נמצאו מתנדבים מתאימים לשעה הזאת
                    if (listOfPossibleVolunteersInTheCurrentHour.Count > 0)
                    {
                        var randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);

                        //איתחול משבצת זמן חדשה למאפינים המתאימים
                        timeSlotProperties = new TimeSlotProperties();
                        timeSlotProperties.needy = currentNeedy;
                        timeSlotProperties.volunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex].volunteer;
                        timeSlotProperties.orgCode = currentOrg.org_code;

                        timeSlotProperties.time = new time_slot();

                        //תאריך התחלה שווה לתאריך המאוחר יותר -תחילת פעילות הארגון או היום, אם זה שיבוץ באמצע שנה
                        int cmp = DateTime.Compare(currentOrg.activity_start_date, DateTime.Today);
                        timeSlotProperties.time.start_at_date = cmp > 0 ? currentOrg.activity_start_date : DateTime.Today;
                        timeSlotProperties.time.end_at_date = currentOrg.activity_end_date;

                        //שאר המאפיינים לפי המשבצת זמן הנוכחית שמתאימה גם למתנדב וגם לנזקק
                        timeSlotProperties.time.start_at_hour = currentSlot.start_at_hour;
                        timeSlotProperties.time.end_at_hour = currentSlot.start_at_hour + (currentOrg.avg_volunteering_time / 15);
                        timeSlotProperties.time.day_of_week = currentSlot.day_of_week;

                        this.timeSlotsPropertiesList.Add(timeSlotProperties);
                    }
                }

            }
        }

        public override IChromosome Clone()
        {
            return new GeneticScheduling(DBCon, timeSlotsPropertiesList, currentOrg.org_code);
        }

        public override void Crossover(IChromosome pair)
        {
            var otherChromosome = pair as GeneticScheduling;
            var randomIndex = RandomIndex(timeSlotsPropertiesList.Count);

            for (int index = randomIndex; index < timeSlotsPropertiesList.Count; index++)
            {
                this.timeSlotsPropertiesList[index].volunteer = otherChromosome.timeSlotsPropertiesList[index].volunteer;
            }
        }

        public override void Mutate()
        {
            var indexToReplace = RandomIndex(timeSlotsPropertiesList.Count);
            var slotToReplace = timeSlotsPropertiesList[indexToReplace].time;

            //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
            var listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForTimeSlot(slotToReplace);
            int randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);

            timeSlotsPropertiesList[indexToReplace].volunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex].volunteer;
        }

        public class FitnessFunction : IFitnessFunction
        {
            public double Evaluate(IChromosome chromosome)
            {
                DBConnection dbCon = new DBConnection();
                List<neediness_details> needinessDetailsInOrg = new List<neediness_details>();
                List<volunteering_details> volunteeringDetailsInOrg = new List<volunteering_details>();

                double score = 1;
                var values = (chromosome as GeneticScheduling).timeSlotsPropertiesList;
                var currentOrg = dbCon.GetDbSet<organization>().Find(o => o.org_code == values[0].orgCode);

                needinessDetailsInOrg = dbCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy" }).ToList().FindAll(nd => nd.org_code == currentOrg.org_code).ToList();
                volunteeringDetailsInOrg = dbCon.GetDbSetWithIncludes<volunteering_details>(new string[] { "volunteer" }).ToList().FindAll(vd => vd.org_code == currentOrg.org_code).ToList();

                #region בדיקה של מרחקים בין כתובות
                var volunteersOfNeedy = new List<volunteer>();
                var currentVolunteerHours = 0;

                foreach (var item in needinessDetailsInOrg)
                {
                    volunteersOfNeedy = values.FindAll(slot => slot.needy.needy_ID == item.needy_ID).Select(slot => slot.volunteer).ToList();

                   // כל המתנדבים ששובצו לנזקק בלי כפיליות כדי לחשב אם מתאימים מבחינת כתובות
                    volunteersOfNeedy = volunteersOfNeedy.Distinct().ToList();
                    double dis = 0;
                    //עבור מרחק גדול מעשר דקות יורד ציון
                    foreach (var volunteer in volunteersOfNeedy)
                    {
                        dis = GoogleMaps.GetDistanceInMinutes(volunteer.volunteer_address, item.needy.needy_address).Result;
                        score -= (dis - 10.0);
                    }
                }
                #endregion

                #region בדיקה של מספר שעות מתאים למתנדב
                foreach (var item in volunteeringDetailsInOrg)
                {
                    currentVolunteerHours = (values.FindAll(slot => slot.volunteer.volunteer_ID == item.volunteer_ID).ToList().Count() * (currentOrg.avg_volunteering_time)) / 60;
                    score += Math.Abs(item.weekly_hours - currentVolunteerHours) / 10;
                }
                #endregion

                #region בדיקה של חפיפות במערכת החדשה כולל הלוח הקיים, וכולל הפרש הגיוני בין ההתנדבויות של אותו מתנדב
                var allSchedule = dbCon.GetDbSetWithIncludes<schedule>(new string[] { "time_slot", "neediness_details.needy", "volunteering_details.volunteer" }).ToList();

                //איחוד המערכת הכללית למערכת הנוכחית
                var timeSlotProperties = new TimeSlotProperties();
                var scheduleAsTimeSlotPropertiesList = new List<TimeSlotProperties>();
                var listOfAllOverLaps = new List<TimeSlotProperties>();

                foreach (var scheduleSlot in allSchedule)
                {
                    timeSlotProperties = new TimeSlotProperties();
                    timeSlotProperties.needy = scheduleSlot.neediness_details.needy;
                    timeSlotProperties.volunteer = scheduleSlot.volunteering_details.volunteer;
                    timeSlotProperties.orgCode = scheduleSlot.neediness_details.org_code;
                    timeSlotProperties.time = scheduleSlot.time_slot;

                    scheduleAsTimeSlotPropertiesList.Add(timeSlotProperties);
                }
                values.AddRange(scheduleAsTimeSlotPropertiesList);

                var GetOverLaps = new Func<TimeSlotProperties, List<TimeSlotProperties>>(current =>
                                  values.Except(new[] { current })
                                        .Where(slot => slot.volunteer.volunteer_ID == current.volunteer.volunteer_ID
                                                  || slot.needy.needy_ID == current.needy.needy_ID)
                                        .Where(slot => slot.time.end_at_date > DateTime.Today)
                                        .Where(slot => slot.time.day_of_week == current.time.day_of_week)
                                        //בדיקות של שעות התחלה וסיום חופפות - זהות או שאחד מתחיל באמצע השני או שההפרש בינהם קטן משעה
                                        .Where(slot => slot.time.start_at_hour <= current.time.start_at_hour && slot.time.end_at_hour >= current.time.start_at_hour
                                                  || current.time.start_at_hour <= slot.time.start_at_hour && current.time.end_at_hour >= slot.time.start_at_hour
                                                  || current.time.start_at_hour == slot.time.start_at_hour
                                                  || current.time.end_at_hour == slot.time.end_at_hour
                                                  || current.time.end_at_hour + 1 == slot.time.start_at_hour
                                                  || slot.time.end_at_hour + 1 == slot.time.start_at_hour
                                                  || current.time.end_at_hour + 2 == slot.time.start_at_hour
                                                  || slot.time.end_at_hour + 2 == slot.time.start_at_hour
                                                  || current.time.end_at_hour + 3 == slot.time.start_at_hour
                                                  || slot.time.end_at_hour + 3 == slot.time.start_at_hour
                                        ).ToList());

                foreach (var item in values)
                {
                    var overLaps = GetOverLaps(item);
                    listOfAllOverLaps.AddRange(overLaps);
                }

                //רשימת כל החפיפות בלוח, כולל השיבוץ החדש ובלי כפילויות
                listOfAllOverLaps = listOfAllOverLaps.Distinct().ToList();

                double percents = ((double)listOfAllOverLaps.Count() / (double)values.Count()) * 10;
                if (percents > 1)
                    score += percents;
                #endregion

                #region בדיקה שאין למתנדב יותר מ 3 התנדבויות ליום
                volunteeringDetailsInOrg = dbCon.GetDbSet<volunteering_details>();
                var dailyVolunteeringsCounter = 0;
                var slotsOfVolunteer = new List<TimeSlotProperties>();

                foreach (var item in volunteeringDetailsInOrg)
                {
                    slotsOfVolunteer = values.Where(slot => slot.volunteer.volunteer_ID == item.volunteer_ID).Where(slot => slot.time.end_at_date >= DateTime.Today).ToList();
                    dailyVolunteeringsCounter = 0;

                    for (int i = 0; i <= 7; i++)
                    {
                        dailyVolunteeringsCounter += slotsOfVolunteer.Where(slot => slot.time.day_of_week == i).Count();
                    }

                    if (dailyVolunteeringsCounter > 3)
                    {
                        score += (dailyVolunteeringsCounter - 3) / 10;
                    }
                }
                #endregion

                score = Math.Pow(Math.Abs(score) / 10, -1);
                return score;
            }
        }
    }
}