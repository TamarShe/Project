import { Volunteer } from './../../Models/Volunteer.model';
import { VolunteeringDetailsService } from './../../Services/volunteering-details.service';
import { DatePipe, registerLocaleData } from '@angular/common';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { CalendarEvent, CalendarView } from 'angular-calendar';
import { startOfDay, endOfDay, subDays, addDays, addHours, endOfMonth, parse } from 'date-fns';
import { Needy } from 'src/app/Models/Needy.model';
import { TimeSlot } from 'src/app/Models/TimeSlot.model';
import { NeedinessDetailsService } from 'src/app/Services/neediness-details.service';
import { NeedyService } from 'src/app/Services/needy.service';
import { ScheduleService } from 'src/app/Services/schedule.service';
import { TimeSlotService } from 'src/app/Services/time-slot.service';
import { VolunteerService } from 'src/app/Services/volunteer.service';
import localeEs from '@angular/common/locales/he';
import { EventColor, EventAction } from 'calendar-utils';
import { MatDialog } from '@angular/material/dialog';
import { NeedinessDetails } from 'src/app/Models/NeedinessDetails.model';
import { Schedule } from 'src/app/Models/Schedule.model';
import { VolunteeringDetails } from 'src/app/Models/VolunteeringDetails.model';
import { hours } from 'src/app/Models/Hours.model';
import { HoursService } from 'src/app/Services/hours.service';

registerLocaleData(localeEs);

export class MyCalendarEvents implements CalendarEvent
{
  title!: string;
  scheduleCode!:number;
  timeSlot:TimeSlot=new TimeSlot();
  start!: Date;
  needinessDetails:NeedinessDetails=new NeedinessDetails();
  volunteeringDetails:VolunteeringDetails=new VolunteeringDetails();
  volunteer:Volunteer=new Volunteer();
  end?:Date|undefined;
}

@Component({
  selector: 'app-needy-schedule',
  templateUrl: './needy-schedule.component.html',
  styleUrls: ['./needy-schedule.component.scss']
})

export class NeedyScheduleComponent{
  user:Needy=new Needy();
  viewDate: Date = new Date();
  view: CalendarView = CalendarView.Month;
  userID:string="";
  events: MyCalendarEvents[] = []
  timeSlot:TimeSlot=new TimeSlot();
  locale: string = "he";
  subject!:string;
  content="";
  details="";
  //עדכון התאריך ההתחלתי והסיום של החודש עליו רוצים את הארועים
  today=new Date();
  currentMonth=new Date(this.today.getFullYear(), this.today.getMonth(), 1);
  loading=true;
  //תאריך ואירוע נוכחיים - מתעדכן בלחיצה על אירוע
  currentDate=new Date();
  currentEvent!:MyCalendarEvents;
  allHours:hours[]=[];


    constructor(private hoursService:HoursService, public datepipe: DatePipe,private volunteeringDetailsService:VolunteeringDetailsService,public matDialog:MatDialog,private scheduleService:ScheduleService,private route:ActivatedRoute,private needinessDetailsService:NeedinessDetailsService,private timeSlotService:TimeSlotService,private needyService:NeedyService,private volunteerService:VolunteerService,private _snackBar:MatSnackBar) {
    this.route.parent?.paramMap.subscribe(b=> this.userID= b.get("needyid") as string);
    if(this.userID!=null)
    {
      console.log(this.userID)
      this.buildEvents();
      this.needyService.Findneedy(this.userID).subscribe(n=>{
        this.user=n;
        console.log(n);
      });
    }
    this.hoursService.GetAllHours().subscribe(a=>this.allHours=a)
  }

  buildEvents()
  {
    this.loading=true;
    this.events=[];
       this.scheduleService.GetScheduleForNeedy(this.userID).subscribe(a=>
       {
        a.forEach(element => {
          this.events.push(element);
          this.events[this.events.length-1].start=parse(this.events[this.events.length-1].timeSlot.start_at_date.toString().substring(0,10),'yyyy-MM-dd',new Date());
          this.events[this.events.length-1].end=parse(this.events[this.events.length-1].timeSlot.end_at_date.toString().substring(0,10),'yyyy-MM-dd',new Date());

        })})
    setTimeout(() => {
      this.loading=false;
      console.log(this.events);
    }, 3000);
  }


  setView(view: CalendarView) {
    this.view = view;
  }

displayContent()
{
  this.subject="אילוץ חדש ל"+this.user.needy_full_name;
  var dateInString=this.currentDate.toLocaleDateString();
  dateInString= dateInString.toString().substring(0,10);
  let re = /\./gi;
  dateInString=dateInString.replace(re,"-");
  console.log(dateInString)
  this.content="אני לא יכול שיתנדבו אצלי בתאריך "+"dateInString"+
                " המתנדב הוא "+ this.currentEvent.volunteer.volunteer_full_name;
}

  saveAndSendToManager()
  {
//זה רק שינוי של המקורי
var firstTimeSlot=new TimeSlot();
firstTimeSlot.time_slot_code=this.timeSlot.time_slot_code;
firstTimeSlot.day_of_week=this.timeSlot.day_of_week;
firstTimeSlot.end_at_date=new Date(this.currentDate);
firstTimeSlot.start_at_date=new Date(this.timeSlot.start_at_date);
firstTimeSlot.start_at_hour=this.timeSlot.start_at_hour;
firstTimeSlot.end_at_hour=this.timeSlot.end_at_hour;

var secondTimeSlot=new TimeSlot();
secondTimeSlot.day_of_week=this.timeSlot.day_of_week;
secondTimeSlot.end_at_date=new Date(this.timeSlot.end_at_date);
secondTimeSlot.start_at_date=new Date(this.currentDate);
secondTimeSlot.start_at_hour=this.timeSlot.start_at_hour;
secondTimeSlot.end_at_hour=this.timeSlot.end_at_hour;

firstTimeSlot.end_at_date.setDate(this.currentDate.getDate()-6);
secondTimeSlot.start_at_date.setDate(this.currentDate.getDate()+6);
console.log(firstTimeSlot.start_at_date.toLocaleDateString() + ", "
            +firstTimeSlot.end_at_date.toLocaleDateString() + " first");
console.log(secondTimeSlot.start_at_date.toLocaleDateString() + ", "
            +secondTimeSlot.end_at_date.toLocaleDateString() + " second");
        this.timeSlotService.UpdateTimeSlot(firstTimeSlot).subscribe(a=>console.log(a));
        var newScheduleSlot=new Schedule();

        this.timeSlotService.AddTimeSlot(secondTimeSlot).subscribe(newTimeSlot=>{
          var newScheduleSlot=new Schedule();
          newScheduleSlot.time_slot_code=newTimeSlot;
          newScheduleSlot.volunteering_details_code=this.currentEvent.volunteeringDetails.volunteering_details_code;
          newScheduleSlot.neediness_details_code=this.currentEvent.needinessDetails.neediness_details_code;
          this.scheduleService.AddToSchedule(newScheduleSlot).subscribe();
          this.volunteerService.SendConstraintsToManager(newScheduleSlot.volunteering_details_code,this.subject,this.content).subscribe(a=>
            {
              if(a) this._snackBar.open('נשלח בהצלחה','X');else this._snackBar.open('היתה שגיאה בעדכון המנהל','X');
              this.matDialog.closeAll();
            }
            );

          this.buildEvents();
        });

    }

  onDetailsClick(event:any,date:any)
  {
    this.currentEvent=event;
    this.currentDate=date.date;
    this.details="התנדבות על ידי  "+this.currentEvent.volunteer.volunteer_full_name+"\n"+
    "בין השעות: "+this.allHours[this.currentEvent.timeSlot.start_at_hour-1].at_hour.toString().substring(0,5)+
    " - "+this.allHours[this.currentEvent.timeSlot.end_at_hour-1].at_hour.toString().substring(0,5);

  }

  check(date:any,event:MyCalendarEvents):boolean
  {
    var today=new Date(date);
    var date1=new Date(event.timeSlot.start_at_date)
    var date2=new Date(event.timeSlot.end_at_date)
    var flag=false;
    if (event.timeSlot.day_of_week==today.getDay()+1 && date1<=today && date2>=today) flag=true;
    return  flag;
  }
}
