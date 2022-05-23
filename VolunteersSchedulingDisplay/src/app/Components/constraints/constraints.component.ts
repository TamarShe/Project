import { OrganizationService } from './../../Services/organization.service';
import { VolunteerPossibleTime } from './../../Models/VolunteerPossibleTime.model';
import { Time } from '@angular/common';
import { TimeSlotService } from './../../Services/time-slot.service';
import { TimeSlot } from './../../Models/TimeSlot.model';
import { Volunteer } from 'src/app/Models/Volunteer.model';
import { VolunteerPossibleTimeService } from './../../Services/volunteer-possible-time.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { concat, Observable } from 'rxjs';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Organization } from 'src/app/Models/Organization.model';
import { VolunteeringDetails } from 'src/app/Models/VolunteeringDetails.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { VolunteeringDetailsService } from 'src/app/Services/volunteering-details.service';
import { HoursService } from 'src/app/Services/hours.service';
import { hours } from 'src/app/Models/Hours.model';
import { tr } from 'date-fns/locale';

export interface Days{
  code:number
  name:string
  isChecked:boolean
}

export class hoursList{
  code!:number
  start!:hours
  end!:hours
  selected!:boolean
}

@Component({
  selector: 'app-constraints',
  templateUrl: './constraints.component.html',
  styleUrls: ['./constraints.component.scss']
})
export class ConstraintsComponent {
  hours = new FormControl();

  userId:string|null|undefined;
  //לאינפוט של שעה
  time = {hour: 13, minute: 30};
  startDate:Date=new Date;
  endDate:Date=new Date;
  startHour!:Time;
  endHour!:Time;
  //לטבלת הסיכום
  dataSource: TimeSlot[]=[];
  loading=true;
  moneSelectedDays=0;
  //לקומבובוקס של הארגונים שהוא רשום בהם
  relevantOrganizations:Organization[]=[];
  selectedOrg:Organization | undefined;
  volunteeringDetailsConclusion:VolunteeringDetails[]=[];
  selectedVolunteeringDetails!:VolunteeringDetails;
  listOfTimeSlots:TimeSlot[]=[];

  allHours:hours[]=[];

  listOfHours:hoursList[]=[];

  showChangesButtons=false;

  days:Days[]=[
    {code:1,name:'ראשון',isChecked:false},
    {code:2,name:'שני',isChecked:false},
    {code:3,name:'שלישי',isChecked:false},
    {code:4,name:'רביעי',isChecked:false},
    {code:5,name:'חמישי',isChecked:false},
    {code:6,name:'שישי',isChecked:false},
    {code:7,name:'שבת',isChecked:false},
  ];

  constructor( private hoursService:HoursService,private _snackBar:MatSnackBar,private route:ActivatedRoute,private organizationService:OrganizationService,private volunteerPossibleTimeService:VolunteerPossibleTimeService,private volunteeringDetailsService:VolunteeringDetailsService)
  {
    this.route.parent?.paramMap.subscribe(b=>this.userId= b.get("volunteerid"));
    setTimeout(() => {
      this.volunteeringDetailsService.GetAllVolunteeringDetailsForVolunteer(this.userId+"").subscribe(n=>{
        this.volunteeringDetailsConclusion=n;
      });
      this.hoursService.GetAllHours().subscribe(a=>this.allHours=a)
      this.organizationService.getRelevantOrgsForVolunteer(this.userId+"").subscribe(o=>
        this.relevantOrganizations=o);
     setTimeout(() => {
       this.buildTable(0);
     }, 1000);
      this.loading=false;
    }, 1000);
  }

  buildTable(selectedNeedinessDetails:number)
  {
    if(selectedNeedinessDetails!=0){
    this.dataSource=[];
    if(this.userId!=null)
    {
      this.volunteerPossibleTimeService.getAllPossibleTimeSlots(this.selectedVolunteeringDetails.volunteering_details_code).subscribe(a=>
        {console.log(a);
        a.forEach(element => {
          this.dataSource.push(element);
        });
    })}
    }
  }

   async saveConstraints(frm:any)
   {
      this.loading=true;
      this.listOfHours.forEach(hour=>{
        if(hour.selected){
          this.days.forEach(day =>
            {
              if(day.isChecked)
              {
                this.listOfTimeSlots.push({
                  time_slot_code:0,
                  start_at_date:this.startDate,
                  end_at_date:this.endDate,
                  start_at_hour:hour.start.hour_code,
                  end_at_hour:hour.end.hour_code,
                  day_of_week:day.code
                })
              }
            })}})

            setTimeout(() => {
              if(this.listOfTimeSlots.length>0){
                console.log(this.listOfTimeSlots)
                this.volunteerPossibleTimeService.addListOfPossibleTime(this.listOfTimeSlots,this.selectedVolunteeringDetails.volunteering_details_code).subscribe()
                setTimeout(() => {
                  this.listOfTimeSlots=[];
                  this.buildTable(this.selectedVolunteeringDetails.volunteering_details_code);
                  this._snackBar.open('נשמר בהצלחה!','X');
                  this.ToShowChangesButtons();
                }, 1000);
              }
            }, 3000);
   }

   DeleteFromDataSource(code:number)
   {
      this.volunteerPossibleTimeService.deletepossibletimeSlot(code).subscribe(a=>
        {setTimeout(()=>{
          this.buildTable(this.selectedVolunteeringDetails.volunteering_details_code);
          this.loading=false;
        },3000);}
      );
    }

   //כשמשנה את הבחירה של היום
   selectionChange(dayCode:number)
   {
     if(this.days[dayCode-1].isChecked){
       this.moneSelectedDays--;
       this.days[dayCode-1].isChecked=false;
     }
     else{
      this.moneSelectedDays++;
      this.days[dayCode-1].isChecked=true;
     }
   }

   //כשמשנה את הבחירה של ארגון
   updateSelection(orgCode:number)
   {
     this.volunteeringDetailsConclusion.forEach(element => {
       if(element.org_code==orgCode)
       {
           this.selectedVolunteeringDetails=element;
           this.selectedOrg=this.relevantOrganizations.find(o=>o.org_code==orgCode);
       }
     });
      setTimeout(() => {
        this.buildTable(this.selectedVolunteeringDetails.volunteering_details_code);
        var mone=0;
        this.hoursService.GetListOfStartAndEnd(+(this.selectedOrg?.avg_volunteering_time+"")).subscribe(t=>{
          t.forEach(element => {
            this.listOfHours.push({
              code:++mone,
              start:element[0],
              end:element[1],
              selected:false
            })
          });
         } )
      }, 1500);
   }

   ToShowChangesButtons(){
      this.volunteerPossibleTimeService.VolunteerHasScheduleInVolunteeringDetails(this.selectedVolunteeringDetails.volunteering_details_code).subscribe(a=>{
        this.showChangesButtons=a;
      })
   }

   updateHoursSelection(code:number){
      this.listOfHours[code-1].selected=!this.listOfHours[code-1].selected;   
   }
}
