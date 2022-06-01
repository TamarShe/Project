import { Manager } from './../../Models/Manager.model';
import { ManagerService } from './../../Services/manager.service';
import { VolunteeringDetails } from 'src/app/Models/VolunteeringDetails.model';
import { Observable } from 'rxjs';
import { Schedule } from './../../Models/Schedule.model';
import { Volunteer } from './../../Models/Volunteer.model';
import { VolunteeringDetailsService } from './../../Services/volunteering-details.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { VolunteerService } from './../../Services/volunteer.service';
import { Needy } from './../../Models/Needy.model';
import { TimeSlot } from './../../Models/TimeSlot.model';
import { TimeSlotService } from './../../Services/time-slot.service';
import { NeedinessDetails } from './../../Models/NeedinessDetails.model';
import { NeedinessDetailsService } from './../../Services/neediness-details.service';
import { NeedyService } from './../../Services/needy.service';
import { ActivatedRoute } from '@angular/router';
import { ScheduleService } from './../../Services/schedule.service';
import { Component, Injectable, OnInit, ViewChild, ElementRef, ContentChild, TemplateRef } from '@angular/core';
import { CalendarEvent, CalendarView, CalendarModule, CalendarDateFormatter, DateFormatterParams } from 'angular-calendar';
import { addDays, addHours, endOfDay, endOfMonth, parse, startOfDay, sub, subDays } from 'date-fns';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/he';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DatePipe } from '@angular/common'
import {map, startWith} from 'rxjs/operators';
import { Days } from '../constraints/constraints.component';
import { FormGroup, FormControl } from '@angular/forms';
import { hours } from 'src/app/Models/Hours.model';
import { HoursService } from 'src/app/Services/hours.service';
import { el } from 'date-fns/locale';

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
  volunteer:Volunteer=new Volunteer();
  end?:Date|undefined;
}

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.scss']
})

export class ScheduleComponent implements OnInit{
  neediesControl = new FormControl();
  volunteersControl=new FormControl();
  user:Manager=new Manager();
  viewDate: Date = new Date();
  view: CalendarView = CalendarView.Month;
  userID:string="";
  events: MyCalendarEvents[] = []
  timeSlot:TimeSlot=new TimeSlot();
  schedule:Schedule=new Schedule();
  locale: string = "he";
  details="";
  //עדכון התאריך ההתחלתי והסיום של החודש עליו רוצים את הארועים
  today=new Date();
  currentMonth=new Date(this.today.getFullYear(), this.today.getMonth(), 1);
  loading=true;
  //תאריך ואירוע נוכחיים - מתעדכן בלחיצה על אירוע
  currentDate=new Date();
  currentEvent!:MyCalendarEvents;
  volunteers!:Volunteer[];
  filteredVolunteers!: Observable<Volunteer[]>;
  needies!:Needy[];
  filteredNeedies!: Observable<Needy[]>;
  allHours:hours[]=[];


  days:Days[]=[
    {code:1,name:'ראשון',isChecked:false},
    {code:2,name:'שני',isChecked:false},
    {code:3,name:'שלישי',isChecked:false},
    {code:4,name:'רביעי',isChecked:false},
    {code:5,name:'חמישי',isChecked:false},
    {code:6,name:'שישי',isChecked:false},
    {code:7,name:'שבת',isChecked:false},
  ];

  range = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });

  newTimeSlot:TimeSlot=new TimeSlot();

    constructor( private hoursService:HoursService,public datepipe: DatePipe,private managerService:ManagerService,private volunteeringDetailsService:VolunteeringDetailsService,public matDialog:MatDialog,private scheduleService:ScheduleService,private route:ActivatedRoute,private needinessDetailsService:NeedinessDetailsService,private timeSlotService:TimeSlotService,private needyService:NeedyService,private volunteerService:VolunteerService,private _snackBar:MatSnackBar) {
    this.route.parent?.paramMap.subscribe(b=> this.userID= b.get("userid") as string);
    if(this.userID!=null)
    {
      this.buildEvents();
      this.managerService.FindManager(this.userID).subscribe(m=>{
        this.user=m;
        this.managerService.GetAllVolunteersInOrg(m.manager_org_code).subscribe(v=>this.volunteers=v);
        this.managerService.GetAllNeediesInOrg(m.manager_org_code).subscribe(n=>this.needies=n);
      });
    }
    this.hoursService.GetAllHours().subscribe(a=>this.allHours=a)
  }


  ngOnInit() {
    this.filteredNeedies = this.neediesControl.valueChanges.pipe(startWith(''),
    map(value => this.needies.filter(option => option.needy_full_name.includes(value))));
    this.filteredVolunteers = this.volunteersControl.valueChanges.pipe(startWith(''),
      map(value => this.volunteers.filter(option => option.volunteer_full_name.includes(value))));
  }

  buildEvents()
  {
    this.loading=true;
    this.events=[];
       this.scheduleService.GetScheduleForMnager(this.userID).subscribe(a=>
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


  saveChanges()
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
        this.timeSlotService.UpdateTimeSlot(firstTimeSlot).subscribe(a=>console.log(a));

        this.timeSlotService.AddTimeSlot(secondTimeSlot).subscribe(newTimeSlot=>{
          var newScheduleSlot=new Schedule();
          newScheduleSlot.time_slot_code=newTimeSlot;
          newScheduleSlot.volunteering_details_code=this.currentEvent.volunteeringDetails.volunteering_details_code;
          newScheduleSlot.neediness_details_code=this.currentEvent.needinessDetails.neediness_details_code;
          this.scheduleService.AddToSchedule(newScheduleSlot).subscribe(a=>{
           setTimeout(() => {
             this._snackBar.open('נשמר בהצלחה!','X');
            this.matDialog.closeAll();
           }, 3000);
          });
          this.buildEvents();
        });

    }

  onDetailsClick(event:any,date:any)
  {
    this.currentEvent=event;
    this.currentDate=date.date;
    this.details="התנדבות של  "+this.currentEvent.volunteer.volunteer_full_name+"\n"+
                " אצל "+this.currentEvent.needy.needy_full_name+"\n"+
                "בין השעות: "+this.allHours[this.currentEvent.timeSlot.start_at_hour-1].at_hour.toString().substring(0,5)+
                " - "+this.allHours[this.currentEvent.timeSlot.end_at_hour-1].at_hour.toString().substring(0,5)+"\n"+
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

  GenerateSchedule()
  {
    this.loading=true;
    this.managerService.startGeneticScheduling(this.user.manager_org_code).subscribe(a=>{
      if(a){
       this._snackBar.open('המערכת שובצה בהצלחה','X');
       this.loading=false;
      }
       else{
       this._snackBar.open('שגיאה בשיבוץ','X');
       this.loading=false;
      }
    })
  }

  saveNewTimeSlot(frm:any){
    this.timeSlotService.AddTimeSlot(this.newTimeSlot).subscribe(t=>{
      this.schedule.time_slot_code=t;
      setTimeout(() => {
        this.scheduleService.AddToSchedule(this.schedule).subscribe();
        this.loading=true;
        this.buildEvents();
        this._snackBar.open('נשמר בהצלחה','X');
        this.matDialog.closeAll();
      }, 1000);
    })
  }


  updateNSelection(needyID:any)
  {
    this.needinessDetailsService.GetNeedinessDetailsForNeedy(needyID,this.user.manager_org_code).subscribe(n=>this.schedule.neediness_details_code=n.neediness_details_code);
  }

  updateVSelection(volunteerID:any)
  {
    this.volunteeringDetailsService.GetVolunteeringDetailsForVolunteer(volunteerID,this.user.manager_org_code).subscribe(v=>this.schedule.volunteering_details_code= v.volunteering_details_code)
  }

  updateDaySelection(dayCode:any)
  {
    this.newTimeSlot.day_of_week=dayCode;
  }

  updateEndHourSelection(newhourCode:number){
    this.newTimeSlot.end_at_hour=newhourCode;
  }

  
  updateStartHourSelection(newhourCode:number){
    this.newTimeSlot.start_at_hour=newhourCode;
  }

}


