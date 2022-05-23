import { UserService } from './../../Services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Volunteer } from './../../Models/Volunteer.model';
import { VolunteerService } from './../../Services/volunteer.service';
import { Component, OnInit } from '@angular/core';
import {MatInputModule} from '@angular/material/input';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  userId!:string;
  userPassword!:string;
  userType!:string;
  show=true;
  hide=true;

  constructor(private userService:UserService,private router:Router,private route:ActivatedRoute) {}

  login(frm:any)
  {
    this.userService.Login(this.userId,this.userPassword).subscribe(a=>{
      this.userType=a;
      if(this.userType!='')
      {
        if(this.userType=="volunteer") {this.show=false; this.router.navigate(['volunteer-area',this.userId],{relativeTo:this.route});}
        else{
        if(this.userType=="manager"){this.show=false;this.router.navigate(['org-manager-area',this.userId],{relativeTo:this.route})}
        else
        if(this.userType=='needy')
        {this.show=false;this.router.navigate(['needy-area',this.userId],{relativeTo:this.route})}
        else
        {this.show=false;this.router.navigate(['general-manager-area',this.userId],{relativeTo:this.route})}}
      }
     else
     {
       alert("פרטים שגוים");
     }});
  }
}
