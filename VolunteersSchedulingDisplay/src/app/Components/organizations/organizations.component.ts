import { tr } from 'date-fns/locale';
import { MatSnackBar } from '@angular/material/snack-bar';
import { VolunteeringDetails } from './../../Models/VolunteeringDetails.model';
import { VolunteeringDetailsService } from './../../Services/volunteering-details.service';
import { OrganizationService } from './../../Services/organization.service';
import { Component, OnInit } from '@angular/core';
import { Organization } from 'src/app/Models/Organization.model';
import { RouterLinkActive, ActivatedRoute } from '@angular/router';

export class OrgsSelection{
  org!:Organization;
  isSelected!:boolean;
  monthlyHours!:number;
}
@Component({
  selector: 'app-organizations',
  templateUrl: './organizations.component.html',
  styleUrls: ['./organizations.component.scss']
})
export class OrganizationsComponent {
  loading=false;
  orgs:OrgsSelection[]=[];
  disabledOrgs:Organization[]=[];
  userId:string|null|undefined;
  volunteeringDetails:VolunteeringDetails=new VolunteeringDetails();

  constructor(private route:ActivatedRoute,private OrganizationService:OrganizationService,private VolunteeringDetailsService:VolunteeringDetailsService,private _snackBar:MatSnackBar) {
    this.route.parent?.paramMap.subscribe(b=>this.userId= b.get("volunteerid"));
    this.buildList();
  }

  buildList()
  {
    this.orgs=[];
    this.disabledOrgs=[];
    this.loading=true;
    this.OrganizationService.getFreeOrgs(this.userId+"").subscribe(a=>{
      a.forEach(element => {
        this.orgs.push({
            org:element,
            monthlyHours:0,
            isSelected:false,
        });
      });
     })
     this.OrganizationService.getDisabledOrgs(this.userId+"").subscribe(a=>{
       this.disabledOrgs=a;
       this.loading=false;
     })
  }

  updateSelections(code:number)
  {
    this.orgs.forEach(a=>{
      if(a.org.org_code==code) {
        a.isSelected=!a.isSelected;
      }
    })
  }

  signUpToAreaInOrg()
  {
    this.orgs.forEach(a=>{
      if(a.isSelected){
      this.volunteeringDetails=new VolunteeringDetails;
      this.volunteeringDetails.org_code=a.org.org_code;
      this.volunteeringDetails.volunteer_ID=this.userId+"";
      this.volunteeringDetails.weekly_hours=a.monthlyHours;
      this.VolunteeringDetailsService.SignUpToOrg(this.volunteeringDetails).subscribe(b=>{
        console.log(this.volunteeringDetails);
        if (b==false) this._snackBar.open("שגיאה!",'אוי');
        else {
          this._snackBar.open("נרשמת בהצלחה!",'X');
          this.loading=true;
          setTimeout(() => {
              this.buildList()
        }, 1000); };
      })
    }})
  }
}
