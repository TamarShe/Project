import { VolunteeringDetails } from 'src/app/Models/VolunteeringDetails.model';
import { Schedule } from './../../Models/Schedule.model';
import { Volunteer } from './../../Models/Volunteer.model';
import { VolunteeringDetailsService } from './../../Services/volunteering-details.service';
import { MatDialog} from '@angular/material/dialog';
import { VolunteerService } from './../../Services/volunteer.service';
import { Needy } from './../../Models/Needy.model';
import { TimeSlot } from './../../Models/TimeSlot.model';
import { TimeSlotService } from './../../Services/time-slot.service';
import { NeedinessDetails } from './../../Models/NeedinessDetails.model';
import { NeedinessDetailsService } from './../../Services/neediness-details.service';
import { NeedyService } from './../../Services/needy.service';
import { ActivatedRoute } from '@angular/router';
import { ScheduleService } from './../../Services/schedule.service';
import { Component } from '@angular/core';
import { CalendarEvent, CalendarView } from 'angular-calendar';
import {  parse} from 'date-fns';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/he';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DatePipe } from '@angular/common'

registerLocaleData(localeEs);

export class MyCalendarEvents implements CalendarEvent
{
  title!: string;
  scheduleCode!:number;
  timeSlot:TimeSlot=new TimeSlot();
  start!: Date;
  needinessDetails:NeedinessDetails=new NeedinessDetails();
  volunteeringDetails:VolunteeringDetails=new VolunteeringDetails();
  needy:Needy=new Needy();
  end?:Date|undefined;
}

@Component({
  selector: 'app-volunteerschedule',
  templateUrl: './volunteerschedule.component.html',
  styleUrls: ['./volunteerschedule.component.scss']
})

export class VolunteerscheduleComponent{
  user:Volunteer=new Volunteer();
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

    constructor( public datepipe: DatePipe,private volunteeringDetailsService:VolunteeringDetailsService,public matDialog:MatDialog,private scheduleService:ScheduleService,private route:ActivatedRoute,private needinessDetailsService:NeedinessDetailsService,private timeSlotService:TimeSlotService,private needyService:NeedyService,private volunteerService:VolunteerService,private _snackBar:MatSnackBar) {
    this.route.parent?.paramMap.subscribe(b=> this.userID= b.get("volunteerid") as string);
    if(this.userID!=null)
    {
      this.buildEvents();
      this.volunteerService.FindVolunteer(this.userID).subscribe(v=>this.user=v);
    }
  }

  buildEvents()
  {
    this.loading=true;
    this.events=[];
       this.scheduleService.GetScheduleForVolunteer(this.userID).subscribe(a=>
       {
        a.forEach(element => {
          this.events.push(element);
          this.events[this.events.length-1].start=parse(this.events[this.events.length-1].timeSlot.start_at_date.toString().substring(0,10),'yyyy-MM-dd',new Date());
          this.events[this.events.length-1].end=parse(this.events[this.events.length-1].timeSlot.end_at_date.toString().substring(0,10),'yyyy-MM-dd',new Date());
        })})
    setTimeout(() => {
      this.loading=false;
      console.log(this.events)
    }, 2000);
  }

  setView(view: CalendarView)
  {
    this.view = view;
  }

displayContent()
{
  this.subject="אילוץ חדש ל"+this.user.volunteer_full_name;
  var dateInString=this.currentDate.toLocaleDateString();
  dateInString= dateInString.toString().substring(0,10);
  let re = /\./gi;
  dateInString=dateInString.replace(re,"-");
  this.content="אני לא יכול להתנדב בתאריך "+dateInString+" אצל "+ this.currentEvent.needy.needy_full_name;
}

  saveAndSendToManager()
  {
    this.timeSlot=this.currentEvent.timeSlot;
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
    this.details="התנדבות אצל "+this.currentEvent.needy.needy_full_name+"\n"+
                "בין השעות: "+this.currentEvent.timeSlot.start_at_hour.toString()+" - "+this.currentEvent.timeSlot.end_at_hour.toString()+"\n"+
                "בכתובת "+this.currentEvent.needy.needy_address+"\n\n"+
                " פרטים: \n"+this.currentEvent.needinessDetails.details;
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
