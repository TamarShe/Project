import { NeedinessDetails } from 'src/app/Models/NeedinessDetails.model';
import { NeedypersonaldetailsComponent } from './../needypersonaldetails/needypersonaldetails.component';
import { MatDialog } from '@angular/material/dialog';
import { NeedinessDetailsService } from 'src/app/Services/neediness-details.service';
import { NeedyService } from 'src/app/Services/needy.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Manager } from './../../Models/Manager.model';
import { OrganizationService } from './../../Services/organization.service';
import { Needy } from './../../Models/Needy.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ManagerService } from './../../Services/manager.service';
import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, OnInit, Renderer2, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import * as XLSX from 'xlsx';
import * as saveAs from 'file-saver';
const EXCEL_TYPE = 'application/octet-stream';
import { Subject } from 'rxjs/Subject';
import { stringify } from 'querystring';

export class Passwords
{
  ID!:string;
  password!:string;
}

export class rowInTable
{
  needy!:Needy;
  needinessDetails!:NeedinessDetails;
}
@Component({
  selector: 'app-addupdateneedy',
  templateUrl: './addupdateneedy.component.html',
  styleUrls: ['./addupdateneedy.component.scss']
})
export class AddupdateneedyComponent {
  dataSource: rowInTable[]=[];
  userID:string|null|undefined;
  manager:Manager=new Manager();
  loading=true;
  show=true;
  download=false;
  href="";
  table:Passwords[]=[];
  currentNeedinessDetails:NeedinessDetails=new NeedinessDetails();
  //flag=false;



  @ViewChild('t') table1!: ElementRef;


  constructor(public matDialog:MatDialog,private route:ActivatedRoute,private router:Router,private managerService:ManagerService,private organizationService:OrganizationService,private _snackBar:MatSnackBar,private needyService:NeedyService,private needniessDetailsService:NeedinessDetailsService){
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
    this.managerService.uploadNeedies(fileInput.files[0],this.manager.manager_org_code).subscribe(data=>
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
      this._snackBar.open(this.table.length+'  חברים נוספו בהצלחה! ','X');
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
      this.managerService.GetAllNeediesInOrg(this.manager.manager_org_code).subscribe(a=>{
        a.forEach(a1 => {
          this.needniessDetailsService.GetNeedinessDetailsForNeedy(a1.needy_ID,this.manager.manager_org_code).subscribe(nd=>{
             this.dataSource.push({needy:a1,needinessDetails:nd});
        })});
      })
    };

    EditFromDataSource(row:rowInTable)
    {
      var needyID=row.needy.needy_ID;
      var status="update";
      this.currentNeedinessDetails=row.needinessDetails;
      this.router.navigate(['add-update-needy',needyID,status],{relativeTo:this.route});
      //this.matDialog.open(NeedypersonaldetailsComponent,{data:{needyid:needyID,status:status}})
    }

    DeleteFromDataSource(needyCode:string)
    {
      this.needyService.GetNeedinessDetailsCode(needyCode).subscribe(b=>{
        this.needyService.Delete(needyCode,b).subscribe(a=>{
          if(a)
          this._snackBar.open('נמחק בהצלחה!','X');
          else
          this._snackBar.open('כישלון קל','X')
        })})
    }

    editNeedinessDetails(frm:any)
    {
      this.needniessDetailsService.UpdateNeedinessDetails(this.currentNeedinessDetails).subscribe(a=>{
        this._snackBar.open("נשמר בהצלחה!",'X');
        this.currentNeedinessDetails=new NeedinessDetails;
        this.matDialog.closeAll();
      });
    }

    addNeedy(){
      var status="add";
      this.router.navigate(['add-update-needy',status],{relativeTo:this.route});
    }
  }
