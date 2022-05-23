import { NeedinessDetailsService } from './../../Services/neediness-details.service';
import { NeedinessDetails } from './../../Models/NeedinessDetails.model';
import { ManagerService } from './../../Services/manager.service';
import { Manager } from './../../Models/Manager.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NeedyService } from './../../Services/needy.service';
import { Needy } from './../../Models/Needy.model';
import { Component, ElementRef, OnInit, ViewChild, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { el } from 'date-fns/locale';

@Component({
  selector: 'app-needypersonaldetails',
  templateUrl: './needypersonaldetails.component.html',
  styleUrls: ['./needypersonaldetails.component.scss']
})
export class NeedypersonaldetailsComponent {
  addUpdateStatus=false;
  needy:Needy=new Needy();
  status:string | null | undefined;
  userID:string |null| undefined;
  loading=true;
  showed=false;
  manager:Manager=new Manager();
  needinessDetails:NeedinessDetails=new NeedinessDetails();


  emailFormControl = new FormControl('', [Validators.required, Validators.email]);

  constructor(private router:Router,private route:ActivatedRoute,private needinessDetailsService:NeedinessDetailsService,private managetService:ManagerService,private needyService:NeedyService,private _snackBar:MatSnackBar,private _location: Location){
    this.route.paramMap.subscribe(a=>{
      this.status=a.get("status");
      if(this.status=="add")
      {
        this.route.parent?.parent?.paramMap.subscribe(b=>
          {
            var n=b.get("userid");
            managetService.GetOrgCodeOfManager(n+"").subscribe(c=>
            {
               this.manager.manager_org_code=c;
            })
          })
      }
      else
      {
        if(a.get("needyid")!=null)
           this.userID=a.get("needyid");
        else
           this.route.parent?.paramMap.subscribe(b=>{this.userID= b.get("needyid");});
      }
    });

    setTimeout(() => {
      if(this.userID!=null && this.showed==false){
        this.needy.needy_ID=this.userID.toString();
        if(this.status=="update")
          {
            this.needyService.Findneedy(this.userID).subscribe(n=>this.needy=n);
          }
        }
        this.loading=false
    }, 1000);
   }

  Updateneedy(frm:any)
  {
    if(this.status=="add")
    {
       this.needyService.SignUp(this.needy).subscribe(n=>{
         if(n!="") {
           this.needinessDetails.needy_ID=this.needy.needy_ID;
           this.needinessDetails.org_code=this.manager.manager_org_code;
           this.needinessDetails.details="";
           this.needinessDetails.weekly_hours=0;
           console.log(this.needinessDetails);
           console.log(this.manager);
           setTimeout(() => {
             this.needinessDetailsService.AddNeedinessDetails(this.needinessDetails).subscribe(a=>{
              this._snackBar.open('נשמר בהצלחה','X');
              alert(a)
             });
           }, 1000);
         }
         else this._snackBar.open('שגיאה','אוי חבל');
        });
       this.addUpdateStatus=true;
      }
    else
    {
      this.needyService.update(this.needy).subscribe(n=>console.log(n));
      this._snackBar.open("פרטיך עודכנו בהצלחה!","V");
      this.addUpdateStatus=true;
    }
  }
}
