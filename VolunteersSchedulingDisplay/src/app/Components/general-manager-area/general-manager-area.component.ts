import { Observable } from 'rxjs';
import { el } from 'date-fns/locale';
import { Organization } from './../../Models/Organization.model';
import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { Manager } from 'src/app/Models/Manager.model';
import { ManagerService } from 'src/app/Services/manager.service';
import { OrganizationService } from 'src/app/Services/organization.service';
import { VolunteerService } from 'src/app/Services/volunteer.service';
import { VolunteeringDetailsService } from 'src/app/Services/volunteering-details.service';
import {Location} from '@angular/common';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-general-manager-area',
  templateUrl: './general-manager-area.component.html',
  styleUrls: ['./general-manager-area.component.scss']
})
export class GeneralManagerAreaComponent{

  range = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });

  userID:string|null|undefined;
  organization:Organization=new Organization();
  loading=true;
  allOrgs:Organization[]=[];
  allManagers:Manager[]=[];
  manager:Manager=new Manager();
  orgStatus="";

  constructor(public matDialog:MatDialog,private location:Location,private route:ActivatedRoute,private router:Router,private managerService:ManagerService,private organizationService:OrganizationService,private _snackBar:MatSnackBar,private volunteerService:VolunteerService,private volunteeringDetailsService:VolunteeringDetailsService){
      this.route.paramMap.subscribe(b=>{this.userID= b.get("userid");console.log(this.userID)});
      this.orgStatus='add';
      setTimeout(() => {
      this.buildTable()
    }, 2000);
    }

    buildTable()
    {
      this.loading=true;
      this.organizationService.getAllOrganizations().subscribe(o=>this.allOrgs=o);
      this.managerService.GetAllManagers().subscribe(m=>this.allManagers=m);

      setTimeout(() => {
        this.loading=false;
      }, 1000);
    }

    UpdateOrganization(frm:any)
    {
      this.organizationService.updateOranization(this.organization);
      this.orgStatus='update';
    }

    //מנתב לפרטי המנהל שמשויך לארגון הזה
    ShowOrgManagerDetails(orgCode:number)
    {
      var org=this.allManagers.find(m=>m.manager_org_code==orgCode);
      setTimeout(() => {
          this.router.navigate(['add-update-manager',org?.manager_Id,'update'],{relativeTo:this.route});
      }, 1500);
    }

    //האם יש מנהל שמשוייך לארגון הזה, כדי לדעת איזה איקון לשים בטבלה
    orgHasManager(orgCode:number):boolean{
      var org=this.allManagers.find(m=>m.manager_org_code==orgCode);
      if(org==null)
        return false;
      else
        return true;
    }

    //שיוך מנהל חדש לארגון
    AddManagerToOrg(orgCode:number)
    {
      alert('ליצור משהו לרשום מנהל חדש');
    }

    AddManager()
    {
      this.router.navigate(['add-update-manager','add'],{relativeTo:this.route});
    }

    //שמירת ארגון חדש
    saveOrg(frm:any)
    {
      if(this.orgStatus=='add'){
        this.organizationService.addOranization(this.organization).subscribe(a=>{
          if(a!=0){
              this._snackBar.open('נשמר בהצלחה!','X');
              this.organization=new Organization();
            }
          else
              this._snackBar.open('שגיאה בשמירה','X');
        });}
        else{
          this.organizationService.updateOranization(this.organization).subscribe(a=>{
            if(a!=0){
              this._snackBar.open('נשמר בהצלחה!','X');
              this.organization=new Organization();
              this.orgStatus='add';
            }
          else
              this._snackBar.open('שגיאה בשמירה','X');
          })
        }
    }

    //בלחיצה על אייקון העדכון בטבלה - מעדכן את הארגון הנוכחי להיות המתאים
    updateOrg(orgCode:number)
    {
      var org=this.allOrgs.find(o=>o.org_code==orgCode);
      if(org) this.organization=org;
    }

    navigateBack()
    {
        this.location.back();
    }
}
