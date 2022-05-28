import { MatSnackBar } from '@angular/material/snack-bar';
import { Time } from '@angular/common';
import { addHours, addDays } from 'date-fns';
import { OrganizationService } from 'src/app/Services/organization.service';
import { NeedinessDetails } from './../../Models/NeedinessDetails.model';
import { NeedinessDetailsService } from './../../Services/neediness-details.service';
import { Organization } from './../../Models/Organization.model';
import { NeedyPossibleTimeService } from './../../Services/needy-possible-time.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TimeSlot } from 'src/app/Models/TimeSlot.model';
import { TimeSlotService } from 'src/app/Services/time-slot.service';
import { NeedyPossibleTime } from 'src/app/Models/NeedyPossibleTime.model';
import { dateInputsHaveChanged } from '@angular/material/datepicker/datepicker-input-base';
import { hours } from 'src/app/Models/Hours.model';
import { FormControl } from '@angular/forms';
import { HoursService } from 'src/app/Services/hours.service';
import { IndentStyle } from 'typescript';

export interface Days{
  code:number
  name:string
  isChecked:boolean
}

export class hoursList{
  code!:number
  start!:hours
  end!:hours
}

@Component({
  selector: 'app-needyconstraints',
  templateUrl: './needyconstraints.component.html',
  styleUrls: ['./needyconstraints.component.scss']
})

export class NeedyconstraintsComponent {
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
  needinessDetailsConclusion:NeedinessDetails[]=[];
  selectedNeedinessDetails!:NeedinessDetails;
  listOfTimeSlots:TimeSlot[]=[];

  allHours:hours[]=[];

  listOfHours:hoursList[]=[{
    code:1,
    start:new hours(),
    end:new hours()
  }];
  listOfOldHours:hoursList[]=[];

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

  constructor(private hoursService:HoursService,private _snackBar:MatSnackBar,private route:ActivatedRoute,private organizationService:OrganizationService,private needyPossibleTimeService:NeedyPossibleTimeService,private needinesDetailsService:NeedinessDetailsService)
  {
    this.route.parent?.paramMap.subscribe(b=>this.userId= b.get("needyid"));
    setTimeout(() => {
      this.needinesDetailsService.GetAllNeedinessDetailsForNeedy(this.userId+"").subscribe(n=>{
        this.needinessDetailsConclusion=n;
      });
      this.hoursService.GetAllHours().subscribe(a=>this.allHours=a)
      this.organizationService.getRelevantOrgsForNeedy(this.userId+"").subscribe(o=>
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
      this.needyPossibleTimeService.getAllPossibleTimeSlots(this.selectedNeedinessDetails.neediness_details_code).subscribe(a=>
        {a.forEach(element => {
          this.dataSource.push(element);
        });
    })}
    }
  }

   async saveConstraints(frm:any)
   {
      this.loading=true;
      this.listOfHours.forEach(hour=>{
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
            })})

            setTimeout(() => {
              if(this.listOfTimeSlots.length>0){
                console.log(this.listOfTimeSlots)
                this.needyPossibleTimeService.addListOfPossibleTime(this.listOfTimeSlots,this.selectedNeedinessDetails.neediness_details_code).subscribe()
                setTimeout(() => {
                  this.listOfTimeSlots=[];
                  this.buildTable(this.selectedNeedinessDetails.neediness_details_code);
                  this._snackBar.open('נשמר בהצלחה!','X');
                  this.ToShowChangesButtons();
                }, 1000);
              }
            }, 3000);
   }

   DeleteFromDataSource(code:number)
   {
      this.needyPossibleTimeService.deletepossibletimeSlot(code).subscribe(a=>
        {setTimeout(()=>{
          this.buildTable(this.selectedNeedinessDetails.neediness_details_code);
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
     this.needinessDetailsConclusion.forEach(element => {
       if(element.org_code==orgCode)
       {
           this.selectedNeedinessDetails=element;
           this.selectedOrg=this.relevantOrganizations.find(o=>o.org_code==orgCode);
       }
     });
      setTimeout(() => {
        this.buildTable(this.selectedNeedinessDetails.neediness_details_code);
        var mone=0;
        this.hoursService.GetListOfStartAndEnd(+(this.selectedOrg?.avg_volunteering_time+"")).subscribe(t=>{
          t.forEach(element => {
            this.listOfOldHours.push({
              code:++mone,
              start:element[0],
              end:element[1]
            })
          });
         } )
      }, 1500);
      setTimeout(() => {
        console.log(this.dataSource)
      }, 2000);
   }

   ToShowChangesButtons(){
      this.needyPossibleTimeService.NeedyHasScheduleInVolunteeringDetails(this.selectedNeedinessDetails.neediness_details_code).subscribe(a=>{
        this.showChangesButtons=a;
      })
   }


  updateEndHourSelection(listCode:number,newhourCode:number){
    this.listOfHours[listCode-1].end.hour_code=newhourCode;
  }

  
  updateStartHourSelection(listCode:number,newhourCode:number){
    this.listOfHours[listCode-1].start.hour_code=newhourCode;
  }

  removeHour(index:number){
    this.listOfHours.splice(index-1,1);
  }

  addHour()
  {
    this.listOfHours.push({
      code:this.listOfHours.length+1,
      start:new hours,
      end:new hours
    })
  }
}
