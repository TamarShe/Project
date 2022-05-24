import { Observable } from 'rxjs';
import { VolunteeringDetails } from './../../Models/VolunteeringDetails.model';
import { VolunteeringDetailsService } from './../../Services/volunteering-details.service';
import { Manager } from './../../Models/Manager.model';
import { UserService } from 'src/app/Services/user.service';
import { ManagerService } from 'src/app/Services/manager.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { VolunteerService } from './../../Services/volunteer.service';
import { Volunteer } from './../../Models/Volunteer.model';
import { Component, Input, OnInit, ViewChild, ElementRef, Output ,EventEmitter} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormControl, Validators, FormBuilder, AbstractControl, FormGroup } from '@angular/forms';
@Component({
  selector: 'app-personaldetails',
  templateUrl: './personaldetails.component.html',
  styleUrls: ['./personaldetails.component.scss']
})
export class PersonaldetailsComponent {
  formattedaddress=" ";
  // options={
  //   types: ['(cities)'],
  //   componentRestrictions:{country:'uk'},
  // }
  addUpdateStatus=false;
  volunteer:Volunteer=new Volunteer();
  status:string | null | undefined;
  userID:string |null| undefined;
  showed=false;
  loading=true;
  temp="add";
  manager:Manager=new Manager();

  flag=false;
  autocomplete!:any;

  emailFormControl = new FormControl(null, [Validators.required, Validators.email]);
  passwordFormControl = new FormControl(null, [Validators.required,this.ValidatePassword.bind(this)]);

  @ViewChild ('address', {static:false}) address!: HTMLInputElement;

  constructor(private volunteeringDetailsService:VolunteeringDetailsService,private userService:UserService,private managerService:ManagerService,private formBuilder:FormBuilder,private router:Router,private route:ActivatedRoute,private volunteerService:VolunteerService,private _snackBar:MatSnackBar){
    this.route.paramMap.subscribe(a=>console.log(a))
    this.route.paramMap.subscribe(a=>{
      this.status=a.get("status");
      if(this.status=="add")
      {
        if(a.get("userid")!=""){
         this.userID=a.get("userid");}
           else{
         this.route.parent?.parent?.paramMap.subscribe(b=>
          {
            var n=b.get("userid");
            if(n!=null){
            managerService.GetOrgCodeOfManager(n+"").subscribe(c=>
            {
               this.manager.manager_org_code=c;
            })}
          })}
      }
      else
      {
        if(a.get("volunteerid")!=null)
           this.userID=a.get("volunteerid");
        else
           this.route.parent?.paramMap.subscribe(b=>{this.userID= b.get("volunteerid");});
      }

      var options = {
        types: ['(cities)'],
        componentRestrictions: {country: "IL"}
       };

       this.autocomplete = new google.maps.places.Autocomplete(this.address, options);
    });

    setTimeout(() => {
      if(this.userID!=null){
        this.volunteer.volunteer_ID=this.userID.toString();
        if(this.status=="update")
          {
            this.volunteerService.FindVolunteer(this.userID).subscribe(v=>this.volunteer=v);
          }
        }
        this.loading=false
    }, 1000);
  }


ValidatePassword(control: AbstractControl): { [key: string]: boolean }
 {
  var password=control.value+"";
  if(password!=""){
  this.volunteerService.CheckIfPasswordIsPossible(this.userID+"",password).subscribe(b=>{
    this.flag=b;
   } );}
      return  { 'invalidPassword': false };
}

  UpdateVolunteer(frm:any)
  {
    if(this.status=="add")
    {

       this.volunteerService.SignUp(this.volunteer).subscribe(n=>{
         if(n!="") {
           if (this.manager.manager_org_code!=null)
           {
             var volunteeringDetails= new VolunteeringDetails();
             volunteeringDetails.org_code=this.manager.manager_org_code;
             volunteeringDetails.volunteer_ID=this.volunteer.volunteer_ID;
             volunteeringDetails.weekly_hours=0;
             this.volunteeringDetailsService.AddVolunteeringDetails(volunteeringDetails).subscribe(a=>{
               if (a!=0) this._snackBar.open('נשמר בהצלחה','X');
               else this._snackBar.open('שגיאה','אוי חבל')
             })
           }
           else{
            this._snackBar.open('נשמר בהצלחה','X');
           }
          }
         else this._snackBar.open('שגיאה','אוי חבל')
        });
       this.addUpdateStatus=true;
      }
    else
    {
      this.volunteerService.update(this.volunteer).subscribe();
      this._snackBar.open("פרטיך עודכנו בהצלחה!","V");
      this.addUpdateStatus=true;
    }
  }

}
