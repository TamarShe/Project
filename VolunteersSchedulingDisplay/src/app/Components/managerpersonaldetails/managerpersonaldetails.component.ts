import { MatSnackBar } from '@angular/material/snack-bar';
import { OrganizationService } from './../../Services/organization.service';
import { Organization } from './../../Models/Organization.model';
import { ManagerService } from './../../Services/manager.service';
import { Manager } from './../../Models/Manager.model';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Volunteer } from 'src/app/Models/Volunteer.model';
import { VolunteerService } from 'src/app/Services/volunteer.service';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-managerpersonaldetails',
  templateUrl: './managerpersonaldetails.component.html',
  styleUrls: ['./managerpersonaldetails.component.scss']
})
export class ManagerpersonaldetailsComponent {
  addUpdateStatus=false;
  manager:Manager=new Manager();
  status:string | null | undefined;
  userID:string |null| undefined;
  loading=true;
  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  org:Organization=new Organization();
  allOrgs:Organization[]=[];

  @ViewChild('ok',{static:false}) ok!:ElementRef;

  constructor(private router:Router,private route:ActivatedRoute,private managerService:ManagerService,private organizationService:OrganizationService,private _snackBar:MatSnackBar){
    this.route.paramMap.subscribe(a=>this.status=a.get("status"));
    this.route.paramMap.subscribe(b=>this.userID = b.get("managerid"));

    if(this.userID==null){
      this.route.parent?.paramMap.subscribe(b=>this.userID=b.get('userid'));
    }

    if(this.status=="update"){
       this.managerService.FindManager(this.userID+"").subscribe(n=>{this.manager=n})
        setTimeout(() => {
          this.organizationService.getOrg(this.manager.manager_org_code).subscribe(a=>{this.org=a;})
          this.loading=false;
        }, 1500);
   }
   else{
     this.organizationService.getAllOrganizations().subscribe(o=>this.allOrgs=o);
     this.loading=false;
   }}


  UpdateManager(frm:any)
  {
    if(this.status=="add")
    {
       this.managerService.SignUp(this.manager).subscribe(n=>{if(n!="") this._snackBar.open('נרשמת בהצלחה!','V');else this._snackBar.open('אוי שגיאה','כמה חבל')});
       this.addUpdateStatus=true;
      }
    else
    {
      this.managerService.update(this.manager).subscribe(n=>{if(n!="")this._snackBar.open('פרטיך עודכנו בהצלחה!','V');else this._snackBar.open('אוי שגיאה','כמה חבל')});
      this.addUpdateStatus=true;
    }
  }
}
