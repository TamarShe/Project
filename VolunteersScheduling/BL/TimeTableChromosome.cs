using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Genetic;
using BL.Classes;
using MODELS;
using DAL;

namespace BL
{
    public class TimeTableChromosome : ChromosomeBase
    {
        private readonly DBConnection DBCon = new DBConnection();
        public List<TimeSlotProperties> timeSlotsPropertiesList = new List<TimeSlotProperties>();
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
        

        public TimeTableChromosome(DBConnection db, int orgCode)
        {
            DBCon = db;
            InitializeLists();
            this.currentOrg = listOfOrganizations.Find(org=>org.org_code==orgCode);
            Generate();
        }

        public TimeTableChromosome(DBConnection db, List<TimeSlotProperties> slots)
        {
            this.DBCon = db;
            this.timeSlotsPropertiesList = slots;
            //if to initialize here the org and how
            InitializeLists();
        }

        public void InitializeLists()
        {
            listOfOrganizations = DBCon.GetDbSet<organization>().ToList();
            listOfVolunteers = DBCon.GetDbSet<volunteer>().ToList();
            listOfNeedies = DBCon.GetDbSet<needy>().ToList();
            listOfVolunteeringDetails = DBCon.GetDbSetWithIncludes<volunteering_details>(new string[] { "volunteer_possible_time.time_slot" }).ToList();
            listOfNeedinessDetails = DBCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy_possible_time.time_slot" }).ToList();
            listOfVolunteersPossibleTime = DBCon.GetDbSetWithIncludes<volunteer_possible_time>(new string[] { "volunteering_details", "time_slot" }).ToList();
            listOfNeediesPossibleTimes = DBCon.GetDbSetWithIncludes<needy_possible_time>(new string[]{"neediness_details","time_slot"}).ToList();
            listOfTimeSlots = DBCon.GetDbSet<time_slot>().ToList();
            listOfHours = DBCon.GetDbSet<hour>().ToList();
        }

        public override IChromosome CreateNew()
        {
            var timeTableChromosome = new TimeTableChromosome(DBCon, currentOrg.org_code);
            timeTableChromosome.Generate();
            return timeTableChromosome;
        }

        public int RandomIndex(int limit)
        {
            return Random.Next(0, limit);
        }

        public List<volunteering_details> GetPossibleVolunteersForOrgAndHour(int startHourCode,int dayOfWeek)
        {
            var listOfVolunteeringDetailsInCurrentOrg = listOfVolunteeringDetails.FindAll(vd => vd.org_code == currentOrg.org_code).ToList();
            var listOfPossibleVolunteersInTheCurrentHour = listOfVolunteeringDetailsInCurrentOrg.FindAll(
                vd => vd.volunteer_possible_time.ToList().FindAll(
                        vpt => vpt.time_slot.start_at_hour<= startHourCode 
                            && vpt.time_slot.end_at_hour>=startHourCode+(currentOrg.avg_volunteering_time/15)
                            && vpt.time_slot.day_of_week==dayOfWeek)
                .Count > 0).ToList();
            return listOfPossibleVolunteersInTheCurrentHour;
        }

        public override void Generate()
        {
            var list = new List<TimeSlotProperties>();
            var timeSlotProperties = new TimeSlotProperties();

            //רשימות פרטי הנזקקים שקשורים לארגון הזה
            var listOfNeedinessDetailsInCurrentOrg = listOfNeedinessDetails.FindAll(nd => nd.org_code == currentOrg.org_code).ToList();

            //רשימת כל הזמנים האפשרים למתנדב הרלוונטים לארגון
            var listOfPossibleVolunteersInTheCurrentHour = new List<volunteering_details>();

            var currentNeedy = new needy();
            var requiredWeeklyHoursOfNeedy = 0.0;

            //רשימת המשבצות זמן האפשריות בארגון הזה לנזקק הנוכחי
            var listOfPossibleTimeSlotsOfCurrentNeedy = new List<time_slot>();

            foreach (var currentNeedinessDetails in listOfNeedinessDetailsInCurrentOrg)
            {
                //אתחול המשתנים שיתאימו לנזקק הנוכחי
                currentNeedy = listOfNeedies.First(needy => needy.needy_ID == currentNeedinessDetails.needy_ID);

                //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי זקוק להם בארגון הזה
                listOfPossibleTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).ToList();

                //כל עוד לא עבר את המכסת שעות שהוא זכאי להן
                while(requiredWeeklyHoursOfNeedy>currentNeedinessDetails.weekly_hours)
                {
                    var randomIndex = RandomIndex(listOfPossibleTimeSlotsOfCurrentNeedy.Count);
                    
                    //השעת התחלה מוגרלת מבין השעות האפשריות בטווח שהוגרל לפי זמן התנדבות ממוצע בארגון
                    var startHour = Random.Next(listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].start_at_hour, 
                                                (listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].end_at_hour-(currentOrg.avg_volunteering_time/15))+1);

                    //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
                    listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(startHour, listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].day_of_week);
                    
                    //אם נמצאו מתנדבים מתאימים לשעה הזאת
                    if (listOfPossibleVolunteersInTheCurrentHour.Count > 0)
                    {
                        var randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);

                        //איתחול משבצת זמן חדשה למאפינים המתאימים
                        timeSlotProperties = new TimeSlotProperties();
                        timeSlotProperties.needy = currentNeedy;
                        timeSlotProperties.volunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex].volunteer;
                        timeSlotProperties.orgCode = currentOrg.org_code;

                        //תאריך התחלה שווה לתאריך המוקדם יותר -תחילת פעילות הארגון או היום, אם זה שיבוץ באמצע שנה
                        timeSlotProperties.time.start_at_date = DateTime.Compare(currentOrg.activity_start_date, DateTime.Today) <= 0 ? currentOrg.activity_start_date : DateTime.Today;
                        timeSlotProperties.time.end_at_date = currentOrg.activity_end_date;

                        //שאר המאפיינים לפי המשבצת זמן הנוכחית שמתאימה גם למתנדב וגם לנזקק
                        timeSlotProperties.time.start_at_hour = startHour;
                        timeSlotProperties.time.end_at_hour = startHour+(currentOrg.avg_volunteering_time/15);
                        timeSlotProperties.time.day_of_week = listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].day_of_week;

                        list.Add(timeSlotProperties);

                        //הורדת סך השעות ששובצו מסך השעות של הנזקק הנוכחי
                        requiredWeeklyHoursOfNeedy -= (currentOrg.avg_volunteering_time/15);
                    }
                }
            }
        }

        public override IChromosome Clone()
        {
            return new TimeTableChromosome(DBCon, timeSlotsPropertiesList);
        }

        public override void Crossover(IChromosome pair)
        {
            var randomVal = Random.Next(0, timeSlotsPropertiesList.Count - 2);
            var otherChromosome = pair as TimeTableChromosome;
            for (int index = randomVal; index < otherChromosome.timeSlotsPropertiesList.Count; index++)
            {
                timeSlotsPropertiesList[index] = otherChromosome.timeSlotsPropertiesList[index];
            }
        }

        public override void Mutate()
        {
            var indexToReplace = RandomIndex(timeSlotsPropertiesList.Count);

            //הגרלת מספר שמציין את פעולת השינוי שמבצעים ברשימה - מחיקת משבצת עדכון משבצת או הוספה
            var action = RandomIndex(3);

            //פרטי הזדקקות נוכחיים - אם זה מצב הוספה הם מוגרלים ואם שינוי - הם שוים לאלמנט שבאינדקס שהוגרל
            var currentNeedinessDetails = new neediness_details();

            switch (action)
            {
                //מחיקת המשבצת
                case 0:
                    timeSlotsPropertiesList.RemoveAt(indexToReplace);
                    return;

                //עדכון משבצת
                case 1:
                    var currentNeedy = timeSlotsPropertiesList.ElementAt(indexToReplace).needy;
                    currentNeedinessDetails = listOfNeedinessDetails.Find(n=>n.needy_ID==currentNeedy.needy_ID && n.org_code==currentOrg.org_code);
                    break;

                //הוספת משבצת
                case 2:
                    //כאן מגרילים פרטי הזדקקות רלוונטים לארגון, שעליהם מבצעים את ההוספה
                    timeSlotsPropertiesList.Add(new TimeSlotProperties());
                    indexToReplace = timeSlotsPropertiesList.Count-1;
                    var relevantNedinessDetails = listOfNeedinessDetails.FindAll(n => n.org_code == currentOrg.org_code);
                    var randomNeedinessDetailsIndex = RandomIndex(relevantNedinessDetails.Count);
                    currentNeedinessDetails = relevantNedinessDetails.ElementAt(randomNeedinessDetailsIndex);
                    break;
            }

            //מכאן הקוד מתאים גם להוספה וגם לשינוי

            //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי יכול בהם בארגון הזה
            var listOfPossibleTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).ToList();
            
            //משבצת זמן אקראית של הנזקק
            var randomTimeSlotIndex = RandomIndex(listOfPossibleTimeSlotsOfCurrentNeedy.Count);
            var randomTimeSlot = listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeSlotIndex];

            //הגרלת מתנדב אקראי שמתאים לזמן ולארגון
            var listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(randomTimeSlot.start_at_hour,randomTimeSlot.day_of_week);
            var randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);
            var randomVolunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex].volunteer;

            //אתחול משבצת חדשה
            var newTimeSlot = new TimeSlotProperties();
            newTimeSlot.needy = currentNeedinessDetails.needy;
            newTimeSlot.volunteer = randomVolunteer;
            newTimeSlot.orgCode = currentNeedinessDetails.org_code;
            newTimeSlot.time = randomTimeSlot;

            timeSlotsPropertiesList[indexToReplace] = newTimeSlot;
        }

        public class FitnessFunction : IFitnessFunction
        {
            DBConnection dbCon = new DBConnection();
            List<neediness_details> needinessDetailsInOrg = new List<neediness_details>();
            List<volunteering_details> volunteeringDetailsInOrg = new List<volunteering_details>();

            public double Evaluate(IChromosome chromosome)
            {
                double score = 1;
                var values = (chromosome as TimeTableChromosome).timeSlotsPropertiesList;
                var needinessDetailsInOrg = dbCon.GetDbSet<neediness_details>().ToList().FindAll(nd => nd.org_code == values[0].orgCode).ToList();
                var volunteeringDetailsInOrg = dbCon.GetDbSet<volunteering_details>().ToList().FindAll(vd => vd.org_code == values[0].orgCode).ToList();

                #region בדיקה של מספר שעות מתאים לנזקק ומרחקים בין כתובות
                var volunteersOfNeedy = new List<volunteer>();
                var currentNeedyHours = 0;
                var currentVolunteerHours = 0;

                foreach (var item in needinessDetailsInOrg)
                {
                    volunteersOfNeedy = values.FindAll(slot => slot.needy.needy_ID == item.needy_ID).Select(slot => slot.volunteer).ToList();
                    currentNeedyHours = volunteersOfNeedy.Count;
                    volunteersOfNeedy = volunteersOfNeedy.Distinct().ToList();
                    score -= (item.weekly_hours - currentNeedyHours);

                    //עבור מרחק גדול מעשר דקות יורד ציון
                    foreach (var volunteer in volunteersOfNeedy)
                    {
                        score -= (GoogleMaps.GetDistanceInMinutes(volunteer.volunteer_address, item.needy.needy_address).Result-10);
                    }
                }

                #endregion

                #region בדיקה של מספר שעות מתאים למתנדב
                foreach (var item in volunteeringDetailsInOrg)
                {
                    currentVolunteerHours = (values.FindAll(slot => slot.volunteer.volunteer_ID == item.volunteer_ID).ToList().Count()*(item.organization.avg_volunteering_time))/60;
                    score -= (item.weekly_hours - currentVolunteerHours);
                }
                #endregion

                #region בדיקה של חפיפות במערכת החדשה כולל הלוח הקיים, וכולל הפרש הגיוני בין ההתנדבויות של אותו מתנדב
                var allSchedule = dbCon.GetDbSet<schedule>().ToList();

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
                                        .Where(slot => slot.time.start_at_hour <= current.time.start_at_hour && slot.time.end_at_hour >= current.time.end_at_hour
                                                  || current.time.start_at_hour <= slot.time.start_at_hour && current.time.end_at_hour >= slot.time.end_at_hour
                                        ).ToList());

                foreach (var item in values)
                {
                    var overLaps = GetOverLaps(item);
                    listOfAllOverLaps.AddRange(overLaps);
                }

                //רשימת כל החפיפות בלוח, כולל השיבוץ החדש ובלי כפילויות
                listOfAllOverLaps = listOfAllOverLaps.Distinct().ToList();

                score -= listOfAllOverLaps.Count() - 1;
                #endregion

                #region בדיקה שאין למתנדב יותר מ 3 התנדבויות ליום
                volunteeringDetailsInOrg = dbCon.GetDbSet<volunteering_details>();
                var dailyVolunteeringsCounter = 0;
                var slotsOfVolunteer = new List<TimeSlotProperties>();

                foreach (var item in volunteeringDetailsInOrg)
                {
                    slotsOfVolunteer = values.Where(slot => slot.volunteer.volunteer_ID == item.volunteer_ID).Where(slot => slot.time.end_at_date >= DateTime.Today).ToList();

                    for (int i = 0; i <= 7; i++)
                    {
                        dailyVolunteeringsCounter = slotsOfVolunteer.Where(slot => slot.time.day_of_week == i).Count();
                    }

                    if (dailyVolunteeringsCounter > 3)
                    {
                        score -= (dailyVolunteeringsCounter-3);
                    }
                }
                #endregion

                return Math.Pow(Math.Abs(score), -1);
            }
        }
    }
}