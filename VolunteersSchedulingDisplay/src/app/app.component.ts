import { UserService } from './Services/user.service';
import { OrganizationsComponent } from './Components/organizations/organizations.component';
import { PersonaldetailsComponent } from './Components/personaldetails/personaldetails.component';
import { OrganizationService } from './Services/organization.service';
import { Organization } from './Models/Organization.model';
import { Observable } from 'rxjs';
import { Volunteer } from './Models/Volunteer.model';
import { VolunteerService } from './Services/volunteer.service';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Inject, OnInit, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RouteConfigLoadStart, Router } from '@angular/router';
import { fadeInRight, fadeInX, fadeOutLeft, rotateIn, rotateOutUpLeft, slideOutLeft, zoomInLeft, zoomInUp, zoomOutDown, zoomOutLeft } from 'ng-animate';
import { TextAnimation } from 'ngx-teximate';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class AppComponent {
  showButtons=true;

  volunteersSum!:number;
  neediesSum!:number;
  orgSum!:number;

  Login()
  {
    this.showButtons=false;
    this.router.navigate(['/login']);
  }

  SignUp()
  {
    this.showButtons=false;
    this.router.navigate(['/signup']);
  }

  ZefatWebSite()
  {
    this.router.navigate(['/zefat'])
  }

 constructor(private router:Router,private userService:UserService){
    this.userService.getAmounts().subscribe(a=>{
      this.volunteersSum=a[0]+1;
      this.neediesSum=a[1]+1;
      this.orgSum=a[2]+1;
    })
}

 HomePage()
 {
   this.showButtons=true;
   this.router.navigate(['/']);
 }

}
