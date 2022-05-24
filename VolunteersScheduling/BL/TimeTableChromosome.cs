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
        private int orgCode;
        static Random Random = new Random();

        List<organization> listOfOrganizations;
        List<volunteer> listOfVolunteers;
        List<needy> listOfNeedies;
        List<volunteering_details> listOfVolunteeringDetails;
        List<neediness_details> listOfNeedinessDetails;
        List<volunteer_possible_time> listOfVolunteersPossibleTime;
        List<needy_possible_time> listOfNeediesPossibleTimes;
        List<time_slot> listOfTimeSlots;
        List<hour> listOfHours;
        

        public TimeTableChromosome(DBConnection db, int orgCode)
        {
            DBCon = db;
            this.orgCode = orgCode;
            InitializeLists();
            Generate();
        }

        public TimeTableChromosome(DBConnection db, List<TimeSlotProperties> slots)
        {
            this.DBCon = db;
            this.timeSlotsPropertiesList = slots;
            InitializeLists();
        }

        public void InitializeLists()
        {
            listOfOrganizations = DBCon.GetDbSet<organization>().ToList();
            listOfVolunteers = DBCon.GetDbSet<volunteer>().ToList();
            listOfNeedies = DBCon.GetDbSet<needy>().ToList();
            listOfVolunteeringDetails = DBCon.GetDbSet<volunteering_details>().ToList();
            listOfNeedinessDetails = DBCon.GetDbSet<neediness_details>().ToList();
            listOfVolunteersPossibleTime = DBCon.GetDbSet<volunteer_possible_time>().ToList();
            listOfNeediesPossibleTimes = DBCon.GetDbSet<needy_possible_time>().ToList();
            listOfTimeSlots = DBCon.GetDbSet<time_slot>().ToList();
            listOfHours = DBCon.GetDbSet<hour>().ToList();
        }

        public override IChromosome CreateNew()
        {
            var timeTableChromosome = new TimeTableChromosome(DBCon, this.orgCode);
            timeTableChromosome.Generate();
            return timeTableChromosome;
        }

        public int RandomIndex(int limit)
        {
            return Random.Next(0, limit);
        }

        public List<volunteering_details> GetPossibleVolunteersForOrgAndHour(int orgCode,int startHourCode)
        {
            var listOfVolunteeringDetailsInCurrentOrg = listOfVolunteeringDetails.FindAll(vd => vd.org_code == orgCode).ToList();
            var listOfPossibleVolunteersInTheCurrentHour = listOfVolunteeringDetailsInCurrentOrg.FindAll(vd => vd.volunteer_possible_time.ToList().FindAll(vpt => vpt.time_slot.start_at_hour == startHourCode).Count > 0).ToList();
            return listOfPossibleVolunteersInTheCurrentHour;
        }

        public override void Generate()
        {
            var list = new List<TimeSlotProperties>();
            var timeSlotProperties = new TimeSlotProperties();

            //הארגון הנוכחי
            var organization = listOfOrganizations.First(o => o.org_code == orgCode);

            //רשימות פרטי הנזקקים שקשורים לארגון הזה
            var listOfNeedinessDetailsInCurrentOrg = listOfNeedinessDetails.FindAll(nd => nd.org_code == orgCode).ToList();

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

                //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי זקוק להם בארגון הזה ומערבב את סדר הרשימה ברנדומליות
                listOfPossibleTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).OrderBy(npt => Random.Next()).ToList();

                //כל עוד לא עבר את המכסת שעות שהוא זכאי להן
                for (int i = 0; i < listOfPossibleTimeSlotsOfCurrentNeedy.Count && requiredWeeklyHoursOfNeedy > currentNeedinessDetails.weekly_hours; i++)
                {
                    var randomIndex = Random.Next(listOfPossibleTimeSlotsOfCurrentNeedy.Count);
                    var startHour = listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].start_at_hour;

                    //רשימת המתנדבים האפשרים בשעה זו
                    listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(orgCode, startHour);
                    
                    //אם נמצאו מתנדבים מתאימים לשעה הזאת
                    if (listOfPossibleVolunteersInTheCurrentHour.Count > 0)
                    {
                        var randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);

                        //איתחול משבצת זמן חדשה למאפינים המתאימים
                        timeSlotProperties = new TimeSlotProperties();
                        timeSlotProperties.needy = currentNeedy;
                        timeSlotProperties.volunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex].volunteer;
                        timeSlotProperties.orgCode = orgCode;

                        //תאריך התחלה שווה לתאריך המוקדם יותר -תחילת פעילות הארגון או היום, אם זה שיבוץ באמצע שנה
                        timeSlotProperties.time.start_at_date = DateTime.Compare(organization.activity_start_date, DateTime.Today) <= 0 ? organization.activity_start_date : DateTime.Today;
                        timeSlotProperties.time.end_at_date = organization.activity_end_date;

                        //שאר המאפיינים לפי המשבצת זמן הנוכחית שמתאימה גם למתנדב וגם לנזקק
                        timeSlotProperties.time.start_at_hour = startHour;
                        timeSlotProperties.time.end_at_hour = listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].end_at_hour;
                        timeSlotProperties.time.day_of_week = listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].day_of_week;

                        list.Add(timeSlotProperties);

                        requiredWeeklyHoursOfNeedy -= organization.avg_volunteering_time;
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
            var slotToReplace = timeSlotsPropertiesList.ElementAt(indexToReplace);
            var currentNeedinessDetails = listOfNeedinessDetails.Find(nd => nd.organization.org_code == slotToReplace.orgCode && nd.needy_ID == slotToReplace.needy.needy_ID);

            //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי יכול בהם בארגון הזה
            var listOfPossibleTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).ToList();
            
            //משבצת זמן אקראית של הנזקק
            var randomTimeSlotIndex = RandomIndex(listOfPossibleTimeSlotsOfCurrentNeedy.Count);
            var randomTimeSlot = listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeSlotIndex];

            //הגרלת מתנדב אקראי שמתאים לזמן ולארגון
            var listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(currentNeedinessDetails.org_code,randomTimeSlot.start_at_hour);
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
                                        .Where(slot => slot.time.start_at_hour == current.time.start_at_hour
                                                  || slot.time.start_at_hour <= current.time.start_at_hour && slot.time.end_at_hour >= current.time.end_at_hour
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