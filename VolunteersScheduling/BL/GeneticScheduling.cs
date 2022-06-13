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

        private List<volunteer>[,] volunteersPossibleTimeDictionary = new List<volunteer>[7, 96];

        public List<ScheduleGene> timeSlotsChromosome = new List<ScheduleGene>();
        public List<ScheduleGene> noVolunteers = new List<ScheduleGene>();

        public GeneticScheduling(DBConnection db, int orgCode, List<volunteer>[,] volunteersPossibleTimeDictionary)
        {
            DBCon = db;
            InitializeLists();
            this.currentOrg = listOfOrganizations.Find(org => org.org_code == orgCode);
            this.volunteersPossibleTimeDictionary = volunteersPossibleTimeDictionary;
            Generate();
        }

        public GeneticScheduling(DBConnection db, List<ScheduleGene> slots, int orgCode, List<volunteer>[,] volunteersPossibleTimeDictionary)
        {
            this.DBCon = db;
            this.timeSlotsChromosome = slots;
            InitializeLists();
            this.currentOrg = listOfOrganizations.Find(org => org.org_code == orgCode);
            this.volunteersPossibleTimeDictionary = volunteersPossibleTimeDictionary;
        }

        public void InitializeLists()
        {
            listOfOrganizations = DBCon.GetDbSet<organization>().ToList();
            listOfVolunteers = DBCon.GetDbSet<volunteer>().ToList();
            listOfNeedies = DBCon.GetDbSet<needy>().ToList();
            listOfVolunteeringDetails = DBCon.GetDbSetWithIncludes<volunteering_details>(new string[] { "volunteer_possible_time.time_slot", "volunteer" }).ToList();
            listOfNeedinessDetails = DBCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy_possible_time.time_slot", "needy" }).ToList();
            listOfVolunteersPossibleTime = DBCon.GetDbSetWithIncludes<volunteer_possible_time>(new string[] { "volunteering_details.volunteer", "time_slot" }).ToList();
            listOfNeediesPossibleTimes = DBCon.GetDbSetWithIncludes<needy_possible_time>(new string[] { "neediness_details", "time_slot" }).ToList();
        }

   
        public override IChromosome CreateNew()
        {
            var geneticScheduling = new GeneticScheduling(DBCon, currentOrg.org_code,volunteersPossibleTimeDictionary);
            geneticScheduling.Generate();
            return geneticScheduling;
        }

        public int RandomIndex(int limit)
        {
            return Random.Next(0, limit);
        }

        //מקבל זמן נצרך לנזקק ומחלק אותו למשמרות 
        public List<time_slot> DivideSlotToShifts(time_slot slot)
        {
            var hoursForShift = this.currentOrg.avg_volunteering_time;
            var start = slot.start_at_hour;
            var end = slot.end_at_hour;
            List<time_slot> listOfTimeSlots = new List<time_slot>();
            var newTimeSlot = new time_slot();

            while (start + (hoursForShift * 4) <= end)
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

            if (start < end)
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

        public static bool CheckIfSlotsAreOverlaps(time_slot slot1, time_slot slot2)
        {
            if (slot1.day_of_week == slot2.day_of_week)
            {
                if ((slot1.start_at_hour <= slot2.start_at_hour)
                    && (slot1.end_at_hour >= slot2.start_at_hour)
                    || (slot1.start_at_hour <= slot1.start_at_hour)
                    && (slot2.end_at_hour >= slot1.start_at_hour)
                    || (slot1.end_at_hour + 4 == slot2.start_at_hour)
                    || (slot2.end_at_hour + 4 == slot1.start_at_hour)
                    || (slot1.end_at_hour + 3 == slot2.start_at_hour)
                    || (slot2.end_at_hour + 3 == slot1.start_at_hour)
                    || (slot1.end_at_hour + 2 == slot2.start_at_hour)
                    || (slot2.end_at_hour + 2 == slot1.start_at_hour)
                    || (slot1.end_at_hour + 1 == slot2.start_at_hour)
                    || (slot2.end_at_hour + 1 == slot1.start_at_hour))
                    return true;
            }
            return false;
        }

        public override void Generate()
        {
            var ScheduleGene = new ScheduleGene();

            //רשימת הנזקקים שקשורים לארגון הזה
            var listOfNeedinessDetailsInCurrentOrg = listOfNeedinessDetails.FindAll(nd => nd.org_code == currentOrg.org_code).ToList();

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

                    //הרשימה המתאימה מהמילון
                    var possibleVolunteers = volunteersPossibleTimeDictionary[currentSlot.day_of_week - 1, currentSlot.start_at_hour - 1];

                    //איתחול משבצת זמן חדשה למאפינים המתאימים
                    ScheduleGene = new ScheduleGene();
                    ScheduleGene.needy = currentNeedy;
                    ScheduleGene.orgCode = currentOrg.org_code;

                    ScheduleGene.time = new time_slot();

                    //תאריך התחלה שווה לתאריך המאוחר יותר -תחילת פעילות הארגון או היום, אם זה שיבוץ באמצע שנה
                    int cmp = DateTime.Compare(currentOrg.activity_start_date, DateTime.Today);
                    ScheduleGene.time.start_at_date = cmp > 0 ? currentOrg.activity_start_date : DateTime.Today;
                    ScheduleGene.time.end_at_date = currentOrg.activity_end_date;

                    //שאר המאפיינים לפי המשבצת זמן הנוכחית שמתאימה גם למתנדב וגם לנזקק
                    ScheduleGene.time.start_at_hour = currentSlot.start_at_hour;
                    ScheduleGene.time.end_at_hour = currentSlot.start_at_hour + (currentOrg.avg_volunteering_time / 15);
                    ScheduleGene.time.day_of_week = currentSlot.day_of_week;

                    //אם נמצאו מתנדבים מתאימים לשעה הזאת
                    if (possibleVolunteers.Count > 0)
                    {
                        var randomVolunteerIndex = RandomIndex(possibleVolunteers.Count);
                        ScheduleGene.volunteer = possibleVolunteers[randomVolunteerIndex];
                        this.timeSlotsChromosome.Add(ScheduleGene);
                    }
                    else
                    {
                        noVolunteers.Add(ScheduleGene);
                    }
                }

            }
        }

        public override IChromosome Clone()
        {
            return new GeneticScheduling(DBCon, timeSlotsChromosome, currentOrg.org_code,volunteersPossibleTimeDictionary);
        }

        public override void Crossover(IChromosome pair)
        {
            var otherChromosome = pair as GeneticScheduling;
            var randomIndex = RandomIndex(timeSlotsChromosome.Count);

            for (int index = randomIndex; index < timeSlotsChromosome.Count; index++)
            {
                this.timeSlotsChromosome[index].volunteer = otherChromosome.timeSlotsChromosome[index].volunteer;
            }
        }

        public override void Mutate()
        {
            var indexToReplace = RandomIndex(timeSlotsChromosome.Count);
            var slotToReplace = timeSlotsChromosome[indexToReplace].time;

            //רשימת המתנדבים האפשריים בזמן הזה
            var listOfPossibleVolunteersInTheCurrentHour = volunteersPossibleTimeDictionary[slotToReplace.day_of_week - 1, slotToReplace.start_at_hour - 1];

            int randomVolunteerIndex = RandomIndex(listOfPossibleVolunteersInTheCurrentHour.Count);

            timeSlotsChromosome[indexToReplace].volunteer = listOfPossibleVolunteersInTheCurrentHour[randomVolunteerIndex];
        }

        public class FitnessFunction : IFitnessFunction
        {
            public double Evaluate(IChromosome chromosome)
            {
                DBConnection dbCon = new DBConnection();
                List<neediness_details> needinessDetailsInOrg = new List<neediness_details>();
                List<volunteering_details> volunteeringDetailsInOrg = new List<volunteering_details>();

                double score = 0;
                var values = (chromosome as GeneticScheduling).timeSlotsChromosome;
                var currentOrg = dbCon.GetDbSet<organization>().Find(o => o.org_code == values[0].orgCode);

                needinessDetailsInOrg = dbCon.GetDbSetWithIncludes<neediness_details>(new string[] { "needy" }).ToList().FindAll(nd => nd.org_code == currentOrg.org_code).ToList();
                volunteeringDetailsInOrg = dbCon.GetDbSetWithIncludes<volunteering_details>(new string[] { "volunteer" }).ToList().FindAll(vd => vd.org_code == currentOrg.org_code).ToList();

                #region בדיקה של מרחקים בין כתובות
                var volunteersOfNeedy = new List<volunteer>();
                var currentVolunteerHours = 0;

                foreach (var item in needinessDetailsInOrg)
                {
                    volunteersOfNeedy = values.FindAll(slot => slot.needy.needy_ID == item.needy_ID).Select(slot => slot.volunteer).ToList();

                    double dis = 0;
                    //עבור מרחק גדול מעשר דקות יורד ציון
                    foreach (var volunteer in volunteersOfNeedy)
                    {
                        dis = GoogleMaps.GetDistanceInMinutes(volunteer.volunteer_address, item.needy.needy_address).Result;
                        if (score > 10)
                            //על כל 10 דקות נוספות יורדת נקודה
                            score += ((dis - 10.0) / 10);
                    }
                }
                #endregion

                #region בדיקה של מספר שעות מתאים למתנדב ולא יותר מ 3 התנדבויות ליום
                var slotsOfVolunteer = new List<ScheduleGene>();
                var dailyVolunteeringsCounter = 0;

                foreach (var volunteer in volunteeringDetailsInOrg)
                {
                    slotsOfVolunteer = values.Where(slot => slot.volunteer.volunteer_ID == volunteer.volunteer_ID).ToList();
                    currentVolunteerHours = slotsOfVolunteer.Count() * (currentOrg.avg_volunteering_time) / 60;

                    score += Math.Abs(volunteer.weekly_hours - currentVolunteerHours) / 20;

                    dailyVolunteeringsCounter = 0;

                    for (int i = 1; i <= 7; i++)
                    {
                        dailyVolunteeringsCounter += slotsOfVolunteer.Where(slot => slot.time.day_of_week == i).Count();
                    }

                    if (dailyVolunteeringsCounter > 3)
                    {
                        score += (dailyVolunteeringsCounter - 3) / 10;
                    }
                }
                #endregion

                #region בדיקה של חפיפות במערכת החדשה, וכולל הפרש הגיוני בין ההתנדבויות של אותו מתנדב
                var listOfAllOverLaps = new List<ScheduleGene>();

                var GetOverLaps = new Func<ScheduleGene, List<ScheduleGene>>(current =>
                                  values.Except(new[] { current })
                                        .Where(slot => slot.volunteer != null)
                                        .Where(slot => slot.volunteer.volunteer_ID == current.volunteer.volunteer_ID
                                                  || slot.needy.needy_ID == current.needy.needy_ID)
                                        .Where(slot => slot.time.end_at_date > DateTime.Today)
                                        .Where(slot => slot.time.day_of_week == current.time.day_of_week)
                                        .Where(slot => GeneticScheduling.CheckIfSlotsAreOverlaps(slot.time, current.time))
                                        .ToList());

                foreach (var gene in values)
                {
                    var overLaps = GetOverLaps(gene);
                    listOfAllOverLaps.AddRange(overLaps);
                }

                //רשימת כל החפיפות בשיבוץ החדש בלי כפילויות
                listOfAllOverLaps = listOfAllOverLaps.Distinct().ToList();

                //כמה אחוזים החפיפות מתוך כל השיבוץ החדש
                double percents = ((double)listOfAllOverLaps.Count() / (double)values.Count()) * 10;
                if (percents > 1)
                    score += percents;
                #endregion

                score = Math.Pow(Math.Abs(score), -1);
                return score;
            }
        }
    }
}