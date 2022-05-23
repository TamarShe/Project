import { VolunteeringDetailsService } from './../../Services/volunteering-details.service';
import { VolunteerService } from 'src/app/Services/volunteer.service';
import { VolunteeringDetails } from './../../Models/VolunteeringDetails.model';
import { Volunteer } from './../../Models/Volunteer.model';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { Manager } from 'src/app/Models/Manager.model';
import { NeedinessDetails } from 'src/app/Models/NeedinessDetails.model';
import { ManagerService } from 'src/app/Services/manager.service';
import { NeedinessDetailsService } from 'src/app/Services/neediness-details.service';
import { OrganizationService } from 'src/app/Services/organization.service';
import * as XLSX from 'xlsx';

export class Passwords
{
  ID!:string;
  password!:string;
}

export class rowInTable
{
  volunteer!:Volunteer;
  volunteeringDetails!:VolunteeringDetails;
}
@Component({
  selector: 'app-add-update-volunteer',
  templateUrl: './add-update-volunteer.component.html',
  styleUrls: ['./add-update-volunteer.component.scss']
})

export class AddUpdateVolunteerComponent{
  dataSource: rowInTable[]=[];
  userID:string|null|undefined;
  manager:Manager=new Manager();
  loading=true;
  show=true;
  download=false;
  href="";
  table:Passwords[]=[];
  currentVolunteeringDetails:VolunteeringDetails=new VolunteeringDetails();

  @ViewChild('t') table1!: ElementRef;


  constructor(public matDialog:MatDialog,private route:ActivatedRoute,private router:Router,private managerService:ManagerService,private organizationService:OrganizationService,private _snackBar:MatSnackBar,private volunteerService:VolunteerService,private volunteeringDetailsService:VolunteeringDetailsService){
      this.route.parent?.paramMap.subscribe(b=>{this.userID= b.get("userid");console.log(this.userID)});
      this.managerService.FindManager(this.userID+"").subscribe(m=>{this.manager=m});
      setTimeout(() => {
      this.buildTable()
      this.loading=false;
    }, 2000);
    }


 async upload(fileInput:any)
  {
    this.loading=true;
    this.managerService.uploadVolunteers(fileInput.files[0],this.manager.manager_org_code).subscribe(data=>
      {
       data.forEach(row => {
          this.table.push({ID:row[0].toString(),password:row[1].toString()})
        });
        this.checkFlag(data.length);
      });
  }

  checkFlag(length:number) {
    if(this.table.length !== length) {
       window.setTimeout(this.checkFlag, 200);
    } else {
      this._snackBar.open(this.table.length+'  מתנדבים נוספו בהצלחה! ','X');
      this.download=true;
      this.loading=false;
    }
}

  saveExcel(){
    const ws: XLSX.WorkSheet=XLSX.utils.table_to_sheet(this.table1.nativeElement);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'גליון1');
    XLSX.writeFile(wb, 'סיסמאות.xlsx');
  }

  buildTable()
  {
    this.dataSource=[];
      this.managerService.GetAllVolunteersInOrg(this.manager.manager_org_code).subscribe(a=>{
        console.log(a);
        a.forEach(a1 => {
           this.volunteeringDetailsService.GetVolunteeringDetailsForVolunteer(a1.volunteer_ID,this.manager.manager_org_code).subscribe(nd=>{
             this.dataSource.push({volunteer:a1,volunteeringDetails:nd});
        })});
      })
    };

    EditFromDataSource(row:rowInTable)
    {
      var needyID=row.volunteer.volunteer_ID;
      var status="update";
      this.currentVolunteeringDetails=row.volunteeringDetails;
      this.router.navigate(['add-update-volunteer',needyID,status],{relativeTo:this.route});
    }

    DeleteFromDataSource(volunteerID:string)
    {
      this.volunteerService.GetVolunteeringDetailsCode(volunteerID).subscribe(b=>{
        this.volunteerService.Delete(volunteerID,b).subscribe(a=>{
          if(a)
          this._snackBar.open('נמחק בהצלחה!','X');
          else
          this._snackBar.open('כישלון קל','X')
        })})
    }

    editVolunteeringDetails(frm:any)
    {
      this.volunteeringDetailsService.UpdatVolunteeringDetails(this.currentVolunteeringDetails).subscribe(a=>{
        this._snackBar.open("נשמר בהצלחה!",'X');
        this.currentVolunteeringDetails=new VolunteeringDetails;
        this.matDialog.closeAll();
      });
    }

    addVolunteer(){
      var status="add";
      this.router.navigate(['add-update-volunteer',status],{relativeTo:this.route});
    }

}
