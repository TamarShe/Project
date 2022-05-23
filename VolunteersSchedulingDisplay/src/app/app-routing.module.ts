import { GeneralManagerAreaComponent } from './Components/general-manager-area/general-manager-area.component';
import { AddUpdateVolunteerComponent } from './Components/add-update-volunteer/add-update-volunteer.component';
import { AddupdateneedyComponent } from './Components/addupdateneedy/addupdateneedy.component';
import { NeedyconstraintsComponent } from './Components/needyconstraints/needyconstraints.component';
import { NeedyScheduleComponent } from './Components/needy-schedule/needy-schedule.component';
import { OrganizationsComponent } from './Components/organizations/organizations.component';
import { VolunteerscheduleComponent } from './Components/volunteerschedule/volunteerschedule.component';
import { ManagerpersonaldetailsComponent } from './Components/managerpersonaldetails/managerpersonaldetails.component';
import { NeedypersonaldetailsComponent } from './Components/needypersonaldetails/needypersonaldetails.component';
import { OrgmanagerareaComponent } from './Components/orgmanagerarea/orgmanagerarea.component';
import { ConstraintsComponent } from './Components/constraints/constraints.component';
import { VolunteerareaComponent } from './Components/volunteerarea/volunteerarea.component';
import { PersonaldetailsComponent } from './Components/personaldetails/personaldetails.component';
import { LoginComponent } from './Components/login/login.component';
import { SignupComponent } from './Components/signup/signup.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NeedyareaComponent } from './Components/needyarea/needyarea.component';
import { ScheduleComponent } from './Components/schedule/schedule.component';

const routes: Routes = [
  {path: 'signup',component:SignupComponent,children:[
    {path: 'personal-details/:status/:userid', component: PersonaldetailsComponent},
    {path:'volunteer-area',component:VolunteerareaComponent,children:[
      {path: 'personal-details', component: PersonaldetailsComponent},
      {path: 'constraints', component: ConstraintsComponent},
    ]}
  ]},
  {path: 'login',component:LoginComponent,children:[
    { path: 'volunteer-area/:volunteerid', component: VolunteerareaComponent,children:[
      {path: 'personal-details/:status', component: PersonaldetailsComponent},
      {path: 'volunteer-schedule', component: VolunteerscheduleComponent},
      {path: 'constraints', component: ConstraintsComponent},
      {path:'signup-to-org',component:OrganizationsComponent}
    ]},
    {path:'needy-area/:needyid',component:NeedyareaComponent,children:[
      {path: 'needy-personal-details/:status', component: NeedypersonaldetailsComponent},
      {path: 'needy-schedule', component: NeedyScheduleComponent},
      {path: 'needy-constraints', component: NeedyconstraintsComponent}
    ]},
    {path:'org-manager-area/:userid',component:OrgmanagerareaComponent,children:[
      {path: 'manager-personal-details/:status', component: ManagerpersonaldetailsComponent},
      {path: 'schedule', component: ScheduleComponent},
      {path: 'constraints', component: ConstraintsComponent},
      {path: 'needies-managment', component: AddupdateneedyComponent,children:[
        {path: 'add-update-needy/:needyid/:status', component: NeedypersonaldetailsComponent},
        {path: 'add-update-needy/:status', component: NeedypersonaldetailsComponent}
      ]},
      {path: 'volunteers-managment', component: AddUpdateVolunteerComponent,children:[
        {path: 'add-update-volunteer/:volunteerid/:status', component: PersonaldetailsComponent},
        {path: 'add-update-volunteer/:status', component: PersonaldetailsComponent}
      ]}
      ]},
      {path:'general-manager-area/:userid',component:GeneralManagerAreaComponent,children:[
        {path:'add-update-manager/:managerid/:status',component:ManagerpersonaldetailsComponent},
        {path:'add-update-manager/:status',component:ManagerpersonaldetailsComponent}
      ]}
   ]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
