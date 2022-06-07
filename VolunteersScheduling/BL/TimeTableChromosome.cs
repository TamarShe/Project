﻿using System;
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


        public TimeTableChromosome(DBConnection db, int orgCode)
        {
            DBCon = db;
            InitializeLists();
            this.currentOrg = listOfOrganizations.Find(org => org.org_code == orgCode);
            Generate();
        }

        public TimeTableChromosome(DBConnection db, List<TimeSlotProperties> slots, int orgCode)
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
            var timeTableChromosome = new TimeTableChromosome(DBCon, currentOrg.org_code);
            timeTableChromosome.Generate();
            return timeTableChromosome;
        }

        public int RandomIndex(int limit)
        {
            return Random.Next(0, limit);
        }

        //מקבל שעת התחלה ויום בשבוע ומוצא את המתנדבים היכולים בזמן הזה גם מבחינת ההעדפות שלהם וגם מבחינת הלוז הזה
        public List<volunteering_details> GetPossibleVolunteersForOrgAndHour(int startHourCode, int dayOfWeek)
        {
            var listOfVolunteeringDetailsInCurrentOrg = listOfVolunteeringDetails.FindAll(vd => vd.org_code == currentOrg.org_code).ToList();
            var listOfPossibleVolunteersInTheCurrentHour = listOfVolunteeringDetailsInCurrentOrg.FindAll(
                vd => vd.volunteer_possible_time.ToList().FindAll(
                        vpt => vpt.time_slot.start_at_hour <= startHourCode
                            && vpt.time_slot.end_at_hour >= startHourCode + (currentOrg.avg_volunteering_time / 15)
                            && vpt.time_slot.day_of_week == dayOfWeek)
                .Count > 0).ToList();
         //   listOfPossibleVolunteersInTheCurrentHour.RemoveAll(vd => this.timeSlotsPropertiesList.FindAll(tsp => tsp.volunteer.volunteer_ID == vd.volunteer_ID).FindAll(tsp => checkOverlaps(startHourCode, dayOfWeek, tsp.time)).Count > 0);

            return listOfPossibleVolunteersInTheCurrentHour;
        }

        public bool checkOverlaps(time_slot slot1, time_slot slot2)
        {
            if ((slot1.day_of_week == slot2.day_of_week) &&
                (slot1.start_at_hour <= slot2.start_at_hour && slot1.end_at_hour >= slot2.start_at_hour) ||
                (slot2.start_at_hour <= slot1.start_at_hour && slot2.end_at_hour >= slot1.start_at_hour) ||
                (slot1.start_at_hour == slot2.start_at_hour) ||
                (slot1.end_at_hour == slot2.end_at_hour) ||
                (slot2.end_at_hour == slot1.end_at_hour) ||
                (slot1.end_at_hour + 1 == slot2.start_at_hour) ||
                (slot2.end_at_hour + 1 == slot1.start_at_hour) ||
                (slot1.end_at_hour + 2 == slot2.start_at_hour) ||
                (slot2.end_at_hour + 2 == slot1.start_at_hour))
                return true;
            return false;
        }

        public override void Generate()
        {
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
                requiredWeeklyHoursOfNeedy = currentNeedinessDetails.weekly_hours;

                //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי זקוק להם בארגון הזה
                listOfPossibleTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).ToList();

                //כל עוד לא עבר את המכסת שעות שהוא זכאי להן
                while (requiredWeeklyHoursOfNeedy > 0)
                {
                    var randomIndex = RandomIndex(listOfPossibleTimeSlotsOfCurrentNeedy.Count);

                    //השעת התחלה מוגרלת מבין השעות האפשריות בטווח שהוגרל לפי זמן התנדבות ממוצע בארגון
                    var startHour = Random.Next(listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].start_at_hour,
                                                (listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].end_at_hour - (currentOrg.avg_volunteering_time / 15)) + 1);

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

                        timeSlotProperties.time = new time_slot();

                        //תאריך התחלה שווה לתאריך המאוחר יותר -תחילת פעילות הארגון או היום, אם זה שיבוץ באמצע שנה
                        int cmp = DateTime.Compare(currentOrg.activity_start_date, DateTime.Today);
                        timeSlotProperties.time.start_at_date = cmp > 0 ? currentOrg.activity_start_date : DateTime.Today;
                        timeSlotProperties.time.end_at_date = currentOrg.activity_end_date;

                        //שאר המאפיינים לפי המשבצת זמן הנוכחית שמתאימה גם למתנדב וגם לנזקק
                        timeSlotProperties.time.start_at_hour = startHour;
                        timeSlotProperties.time.end_at_hour = startHour + (currentOrg.avg_volunteering_time / 15);
                        timeSlotProperties.time.day_of_week = listOfPossibleTimeSlotsOfCurrentNeedy[randomIndex].day_of_week;

                        this.timeSlotsPropertiesList.Add(timeSlotProperties);

                        //הורדת סך השעות ששובצו מסך השעות של הנזקק הנוכחי
                        requiredWeeklyHoursOfNeedy -= (currentOrg.avg_volunteering_time / 60);
                    }
                }
            }
        }

        public override IChromosome Clone()
        {
            return new TimeTableChromosome(DBCon, timeSlotsPropertiesList, currentOrg.org_code);
        }

        public override void Crossover(IChromosome pair)
        {
            var otherChromosome = pair as TimeTableChromosome;
            var randomIndex=RandomIndex(timeSlotsPropertiesList.Count);
            var toReplace = true;
            for (int index = randomIndex; index < otherChromosome.timeSlotsPropertiesList.Count ; index++)
            {
                for (int i = 0; i < timeSlotsPropertiesList.Count; i++)
                {
                    if (checkOverlaps(timeSlotsPropertiesList[i].time, otherChromosome.timeSlotsPropertiesList[index].time))
                        toReplace = false ;
                }
                if(toReplace)
                    timeSlotsPropertiesList[index] = otherChromosome.timeSlotsPropertiesList[index];
            }
        }

        public override void Mutate()
        {
            var indexToReplace = RandomIndex(timeSlotsPropertiesList.Count);
            int mone = 0;

            //פרטי הזדקקות נוכחיים - אם זה מצב הוספה הם מוגרלים ואם שינוי - הם שוים לאלמנט שבאינדקס שהוגרל
            neediness_details currentNeedinessDetails;

            var currentNeedy = timeSlotsPropertiesList.ElementAt(indexToReplace).needy;
            currentNeedinessDetails = listOfNeedinessDetails.Find(n => n.needy_ID == currentNeedy.needy_ID && n.org_code == timeSlotsPropertiesList[0].orgCode);

            //בונה את רשימת כל המשבצות זמן שהנזקק הנוכחי יכול בהם בארגון הזה
            var listOfPossibleTimeSlotsOfCurrentNeedy = listOfNeediesPossibleTimes.FindAll(npt => npt.needy_details_code == currentNeedinessDetails.neediness_details_code).ToList().Select(npt => npt.time_slot).ToList();
            List<volunteering_details> listOfPossibleVolunteersInTheCurrentHour;

            var randomTimeIndex = RandomIndex(listOfPossibleTimeSlotsOfCurrentNeedy.Count);

            //השעת התחלה מוגרלת מבין השעות האפשריות בטווח שהוגרל לפי זמן התנדבות ממוצע בארגון
            var startHour = Random.Next(listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].start_at_hour,
                                        (listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].end_at_hour - (currentOrg.avg_volunteering_time / 15)) + 1);

            //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
            listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(startHour, listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].day_of_week);
            int randomVolunteerIndex = 0;

            //כל עוד אין מתנדבים בשעה שהוגרלה וגם הוא עבר על הלולאה לא פחות מ 15 פעמים
            while ((listOfPossibleVolunteersInTheCurrentHour.Count == 0) && (mone < 15))
            {
                //משבצת זמן אפשרית לנזקק מוגרלת
                randomTimeIndex = RandomIndex(listOfPossibleTimeSlotsOfCurrentNeedy.Count);

                //השעת התחלה מוגרלת מבין השעות האפשריות בטווח שהוגרל לפי זמן התנדבות ממוצע בארגון
                startHour = Random.Next(listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].start_at_hour,
                                            (listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].end_at_hour - (currentOrg.avg_volunteering_time / 15)) + 1);

                //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
                listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(startHour, listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].day_of_week);

                randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);
                mone++;
            }

            var newTimeSlot = new TimeSlotProperties();
            newTimeSlot.time = new time_slot();

            //עבר על 15 משבצות זמן אקראיות ולא מצא להם מתנדב
            if (mone == 15)
            {
                var allHours = DBCon.GetDbSet<hour>().ToList();
                randomTimeIndex = RandomIndex(allHours.Count);
                startHour = allHours[randomTimeIndex].hour_code;
                var randomDay = RandomIndex(7);

                //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
                listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(startHour, randomDay);

                while (listOfPossibleVolunteersInTheCurrentHour.Count == 0)
                {
                    randomTimeIndex = RandomIndex(allHours.Count);
                    startHour = allHours[randomTimeIndex].hour_code;
                    randomDay = RandomIndex(7);

                    //רשימת המתנדבים האפשריים בשעה הזאת וביום הזה
                    listOfPossibleVolunteersInTheCurrentHour = GetPossibleVolunteersForOrgAndHour(startHour, randomDay);
                }

                newTimeSlot.time.day_of_week = randomDay;
            }

            else
            {
                newTimeSlot.time.day_of_week = listOfPossibleTimeSlotsOfCurrentNeedy[randomTimeIndex].day_of_week;
            }

            var randomVolunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex].volunteer;

            newTimeSlot.time = timeSlotsPropertiesList[indexToReplace].time;
            newTimeSlot.time.start_at_hour = startHour;
            newTimeSlot.time.end_at_hour = startHour + currentOrg.avg_volunteering_time / 15;

            //אתחול משבצת מתאימה
            newTimeSlot.needy = currentNeedinessDetails.needy;
            newTimeSlot.volunteer = randomVolunteer;
            newTimeSlot.orgCode = currentNeedinessDetails.org_code;

            timeSlotsPropertiesList[indexToReplace] = newTimeSlot;
        }

        public class FitnessFunction : IFitnessFunction
        {


            public double Evaluate(IChromosome chromosome)
            {
                DBConnection dbCon = new DBConnection();
                List<neediness_details> needinessDetailsInOrg = new List<neediness_details>();
                List<volunteering_details> volunteeringDetailsInOrg = new List<volunteering_details>();

                double score = 1;
                var values = (chromosome as TimeTableChromosome).timeSlotsPropertiesList;
                var currentOrg = dbCon.GetDbSet<organization>().Find(o => o.org_code == values[0].orgCode);

                needinessDetailsInOrg = dbCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy" }).ToList().FindAll(nd => nd.org_code == currentOrg.org_code).ToList();
                volunteeringDetailsInOrg = dbCon.GetDbSetWithIncludes<volunteering_details>(new string[] { "volunteer" }).ToList().FindAll(vd => vd.org_code == currentOrg.org_code).ToList();

                #region בדיקה של מספר שעות מתאים לנזקק ומרחקים בין כתובות
                var volunteersOfNeedy = new List<volunteer>();
                var currentNeedyHours = 0;
                var currentVolunteerHours = 0;

                foreach (var item in needinessDetailsInOrg)
                {
                    volunteersOfNeedy = values.FindAll(slot => slot.needy.needy_ID == item.needy_ID).Select(slot => slot.volunteer).ToList();
                    //סך ההתנדבויות ששובצו לנזקק והורדת ציון על מס שעות לא מתאים
                    currentNeedyHours = volunteersOfNeedy.Count;
                    score += Math.Abs(item.weekly_hours - currentNeedyHours) / 10;

                    //כל המתנדבים ששובצו לנזקק בלי כפיליות כדי לחשב אם מתאימים מבחינת כתובות
                    //volunteersOfNeedy = volunteersOfNeedy.Distinct().ToList();
                    //double dis = 0;
                    ////עבור מרחק גדול מעשר דקות יורד ציון
                    //foreach (var volunteer in volunteersOfNeedy)
                    //{
                    //    dis = GoogleMaps.GetDistanceInMinutes(volunteer.volunteer_address, item.needy.needy_address).Result;
                    //    score -= (dis - 10.0);
                    //}
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