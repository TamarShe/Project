using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MODELS;

namespace BL.Classes
{
    public class ScheduleBL:VolunteersSchedulingBL
    {
        DBConnection dbCon;
        List<MODELS.ScheduleModel> listOfSchedule;

        public ScheduleBL()
        {
            dbCon = new DBConnection();
            listOfSchedule = ConvertListToModel(dbCon.GetDbSet<schedule>().ToList());
        }

        public List<MODELS.ScheduleModel> GetAllSchedule()
        {
            return listOfSchedule;
        }

        public List<VolunteerCalendarEvent> GetScheduleForVolunteer(string volunteerID)
        {
            NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            NeedyBL needyBL =new NeedyBL();
            TimeSlotBL timeSlotBL = new TimeSlotBL();
            List<VolunteerCalendarEvent> volunteerCalendarEvents = new List<VolunteerCalendarEvent>();
            //רשימה שמכילה את כל המשבצות של לוז שקשורות לנזקק הזה
            List<ScheduleModel> scheduleList = this.GetAllSchedule().FindAll(v => volunteeringDetailsBL.GetAllVolunteeringDetails().Find(a => a.volunteer_ID == volunteerID).volunteering_details_code == v.volunteering_details_code);
            NeedinessDetailsModel needinessDetail = new NeedinessDetailsModel();
            TimeSlotModel timeSlot = new TimeSlotModel();
            VolunteerCalendarEvent volunteerCalendarEvent;
            for (int i = 0; i < scheduleList.Count; i++)
            {
                timeSlot = timeSlotBL.GetAllTimeSlot().First(t => t.time_slot_code == scheduleList[i].time_slot_code);
                needinessDetail = needinessDetailsBL.GetAllNeedinessDetails().First(n => n.neediness_details_code == scheduleList[i].neediness_details_code);
                volunteerCalendarEvent = new VolunteerCalendarEvent(
                    needyBL.GetAllNeedies().First(n => n.needy_ID == needinessDetail.needy_ID),
                    timeSlot,
                    volunteeringDetailsBL.GetAllVolunteeringDetails().First(v => v.volunteering_details_code == scheduleList[i].volunteering_details_code),
                    needinessDetail,
                    scheduleList[i].schedule_code,
                    timeSlot.start_at_date,
                    timeSlot.end_at_date,
                    "התנדבות");
                volunteerCalendarEvents.Add(volunteerCalendarEvent);
            }
            return volunteerCalendarEvents;
        }

        //public List<MODELS.ScheduleModel> GetScheduleForNeedy(string needyID)
        //{
        //    NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
        //    return this.GetAllSchedule().FindAll(n => needinessDetailsBL.GetAllNeedinessDetails().Find(a => a.needy_ID == needyID).neediness_details_code == n.neediness_details_code);
            
        //}

        public List<NeedyCalendarEvent> GetScheduleForNeedy(string needyID)
        {
            NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
            VolunteerBL volunteerBL = new VolunteerBL();
            TimeSlotBL timeSlotBL = new TimeSlotBL();
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            List<NeedyCalendarEvent> needyEvent = new List<NeedyCalendarEvent>();
            //רשימה שמכילה את כל המשבצות של לוז שקשורות לנזקק הזה
            List<ScheduleModel> scheduleList = this.GetAllSchedule().FindAll(n => needinessDetailsBL.GetAllNeedinessDetails().Find(a => a.needy_ID == needyID).neediness_details_code == n.neediness_details_code);
            VolunteeringDetailsModel volunteeringDetails = new VolunteeringDetailsModel();
            TimeSlotModel timeSlot = new TimeSlotModel();
            NeedyCalendarEvent needyCalendarEvent;
            for (int i = 0; i < scheduleList.Count; i++)
            {
                timeSlot = timeSlotBL.GetAllTimeSlot().First(t => t.time_slot_code == scheduleList[i].time_slot_code);
                volunteeringDetails = volunteeringDetailsBL.GetAllVolunteeringDetails().First(v => v.volunteering_details_code == scheduleList[i].volunteering_details_code);
                needyCalendarEvent = new NeedyCalendarEvent(
                    volunteerBL.GetAllvolunteers().First(v=>v.volunteer_ID==volunteeringDetails.volunteer_ID),
                    timeSlot,
                    volunteeringDetails,
                    needinessDetailsBL.GetAllNeedinessDetails().First(n => n.neediness_details_code == scheduleList[i].neediness_details_code),
                    scheduleList[i].schedule_code,
                    timeSlot.start_at_date,
                    timeSlot.end_at_date,
                    "התנדבות");
                needyEvent.Add(needyCalendarEvent);
            }
            return needyEvent;
        }

        public List<ManagerCalendarEvent> GetScheduleForMnager(string managerID)
        {
            NeedinessDetailsBL needinessDetailsBL = new NeedinessDetailsBL();
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            VolunteerBL volunteerBL = new VolunteerBL();
            NeedyBL needyBL = new NeedyBL();
            ManagerBL managerBL = new ManagerBL();
            ManagerModel manager = managerBL.GetAllManagers().First(m=> m.manager_Id == managerID);
            TimeSlotBL timeSlotBL = new TimeSlotBL();
            List<ManagerCalendarEvent> managerEvents = new List<ManagerCalendarEvent>();
            //רשימה שמכילה את כל המשבצות של לוז שקשורות למנהל הזה
            List<ScheduleModel> scheduleList = this.GetAllSchedule().FindAll(s =>
                (needinessDetailsBL.GetAllNeedinessDetails().First(n=>n.neediness_details_code==s.neediness_details_code)).org_code==manager.manager_org_code);
            VolunteeringDetailsModel volunteeringDetails = new VolunteeringDetailsModel();
            NeedinessDetailsModel needinessDetails = new NeedinessDetailsModel();

            TimeSlotModel timeSlot = new TimeSlotModel();
            ManagerCalendarEvent managerCalendarEvent;

            for (int i = 0; i < scheduleList.Count; i++)
            {
                timeSlot = timeSlotBL.GetAllTimeSlot().First(t => t.time_slot_code == scheduleList[i].time_slot_code);
                volunteeringDetails = volunteeringDetailsBL.GetAllVolunteeringDetails().First(v => v.volunteering_details_code == scheduleList[i].volunteering_details_code);
                needinessDetails = needinessDetailsBL.GetAllNeedinessDetails().First(n => n.neediness_details_code == scheduleList[i].neediness_details_code);
                managerCalendarEvent = new ManagerCalendarEvent(
                    volunteerBL.GetAllvolunteers().First(v => v.volunteer_ID == volunteeringDetails.volunteer_ID),
                    needyBL.GetAllNeedies().First(n=>n.needy_ID==needinessDetails.needy_ID),
                    timeSlot,
                    volunteeringDetails,
                    needinessDetails,
                    scheduleList[i].schedule_code,
                    timeSlot.start_at_date,
                    timeSlot.end_at_date,
                    "התנדבות");
                managerEvents.Add(managerCalendarEvent);
            }
            return managerEvents;
        }

        public int InsertSchedule(MODELS.ScheduleModel Schedule1)
        {
            if (listOfSchedule.Find(s => s.schedule_code == Schedule1.schedule_code) == null)
                try
                {
                    dbCon.Execute<schedule>(ConvertScheduleToEF(Schedule1),
                    DBConnection.ExecuteActions.Insert);
                    listOfSchedule = ConvertListToModel(dbCon.GetDbSet<schedule>().ToList());
                    return listOfSchedule.Max(s => s.schedule_code);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfSchedule.Max(s => s.schedule_code);
        }

        public int UpdateSchedule(MODELS.ScheduleModel Schedule1)
        {
            if (listOfSchedule.Find(s => s.schedule_code == Schedule1.schedule_code) != null)
                try
                {
                    dbCon.Execute<schedule>(ConvertScheduleToEF(Schedule1),
                    DBConnection.ExecuteActions.Update);
                    listOfSchedule = ConvertListToModel(dbCon.GetDbSet<schedule>().ToList());
                    return listOfSchedule.First(s => s.schedule_code == Schedule1.schedule_code).schedule_code;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return listOfSchedule.First(s => s.schedule_code == Schedule1.schedule_code).schedule_code;
        }

        public bool DeleteSchedule(int scheduleCode)
        {
            ScheduleModel Schedule1 = listOfSchedule.First(a => a.schedule_code == scheduleCode);
            if (Schedule1!=null)
                try
                {
                    dbCon.Execute<schedule>(ConvertScheduleToEF(Schedule1),
                    DBConnection.ExecuteActions.Delete);
                    listOfSchedule = ConvertListToModel(dbCon.GetDbSet<schedule>().ToList());
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            return false;
        }

        #region convert functions
        public static schedule ConvertScheduleToEF(MODELS.ScheduleModel s)
        {
            return new schedule
            {
                schedule_code=s.schedule_code,
                time_slot_code=s.time_slot_code,
                volunteering_details_code=s.volunteering_details_code,
                neediness_details_code=s.neediness_details_code
            };
        }
        public static MODELS.ScheduleModel ConvertScheduleToModel(schedule s)
        {
            return new MODELS.ScheduleModel
            {
                schedule_code = s.schedule_code,
                time_slot_code = s.time_slot_code,
                volunteering_details_code = s.volunteering_details_code,
                neediness_details_code = s.neediness_details_code
            };
        }

        public static List<MODELS.ScheduleModel> ConvertListToModel(List<schedule> li)
        {
            return li.Select(l => ConvertScheduleToModel(l)).ToList();
        }
        #endregion
    }



    public abstract class CalendarEvent
    {
        public TimeSlotModel timeSlot { get; set; }
        public VolunteeringDetailsModel volunteeringDetails { get; set; }
        public NeedinessDetailsModel needinessDetails { get; set; }
        public int scheduleCode { get; set; }

        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string title { get; set; }

        protected CalendarEvent(TimeSlotModel timeSlot, VolunteeringDetailsModel volunteeringDetailsModel, NeedinessDetailsModel needinessDetailsModel, int scheduleCode, DateTime start, DateTime end, string title)
        {
            this.timeSlot = timeSlot;
            this.volunteeringDetails = volunteeringDetailsModel;
            this.needinessDetails = needinessDetailsModel;
            this.scheduleCode = scheduleCode;
            this.start = start;
            this.end = end;
            this.title = title;
        }
    }

    public class NeedyCalendarEvent : CalendarEvent
    {
        public VolunteerModel volunteer { get; set; }

        public NeedyCalendarEvent(VolunteerModel volunteer,TimeSlotModel timeSlot, VolunteeringDetailsModel volunteeringDetailsModel, NeedinessDetailsModel needinessDetailsModel, int scheduleCode, DateTime start, DateTime end, string title)
            :base(timeSlot, volunteeringDetailsModel,needinessDetailsModel,scheduleCode,start, end, title)
        {
           this.volunteer = volunteer;
        }
    }

    public class VolunteerCalendarEvent : CalendarEvent
    {
        public NeedyModel needy { get; set; }

        public VolunteerCalendarEvent(NeedyModel needy, TimeSlotModel timeSlot, VolunteeringDetailsModel volunteeringDetailsModel, NeedinessDetailsModel needinessDetailsModel, int scheduleCode, DateTime start, DateTime end, string title)
    : base(timeSlot, volunteeringDetailsModel, needinessDetailsModel, scheduleCode, start, end, title)
        {
            this.needy = needy;
        }
    }

    public class ManagerCalendarEvent : CalendarEvent
    {
        public VolunteerModel volunteer { get; set; }
        public NeedyModel needy { get; set; }

        public ManagerCalendarEvent(VolunteerModel volunteer,NeedyModel needy, TimeSlotModel timeSlot, VolunteeringDetailsModel volunteeringDetailsModel, NeedinessDetailsModel needinessDetailsModel, int scheduleCode, DateTime start, DateTime end, string title)
    : base(timeSlot, volunteeringDetailsModel, needinessDetailsModel, scheduleCode, start, end, title)
        {
            this.needy = needy;
            this.volunteer = volunteer;
        }
    }
}
