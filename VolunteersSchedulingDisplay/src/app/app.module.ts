import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule ,ReactiveFormsModule} from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { HttpClientModule } from '@angular/common/http';
import { MatSliderModule } from '@angular/material/slider';
import { MatSelectModule} from '@angular/material/select';
import { MatCheckboxModule} from '@angular/material/checkbox';
import { MatIconModule} from '@angular/material/icon';
import {MatDialogModule} from "@angular/material/dialog";
import { LoginComponent } from './Components/login/login.component';
import { SignupComponent } from './Components/signup/signup.component';
import { PersonaldetailsComponent } from './Components/personaldetails/personaldetails.component';
import {MatStepperModule,MatStep} from '@angular/material/stepper';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import { VolunteerareaComponent } from './Components/volunteerarea/volunteerarea.component';
import { ConstraintsComponent } from './Components/constraints/constraints.component';
import {MatTreeModule} from '@angular/material/tree';
import { NeedyareaComponent } from './Components/needyarea/needyarea.component';
import { OrgmanagerareaComponent } from './Components/orgmanagerarea/orgmanagerarea.component';
import { NeedypersonaldetailsComponent } from './Components/needypersonaldetails/needypersonaldetails.component';
import { ManagerpersonaldetailsComponent } from './Components/managerpersonaldetails/managerpersonaldetails.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { OrganizationsComponent } from './Components/organizations/organizations.component';
import { ScheduleComponent } from './Components/schedule/schedule.component';
import {MatListModule} from '@angular/material/list';
import {MatTabsModule} from '@angular/material/tabs';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table'
import { ChunkPipe } from './chunk.pipe';
import { VolunteerscheduleComponent } from './Components/volunteerschedule/volunteerschedule.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { EditorModule } from "@tinymce/tinymce-angular";
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { NeedyScheduleComponent } from './Components/needy-schedule/needy-schedule.component';
import { NeedyconstraintsComponent } from './Components/needyconstraints/needyconstraints.component';
import { GoogleMapsModule } from '@angular/google-maps';
import { AddupdateneedyComponent } from './Components/addupdateneedy/addupdateneedy.component'
import { GooglePlaceModule } from "ngx-google-places-autocomplete";
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatExpansionModule} from '@angular/material/expansion';
import { DatePipe } from '@angular/common';
import { AddUpdateVolunteerComponent } from './Components/add-update-volunteer/add-update-volunteer.component'
import { TeximateModule } from 'ngx-teximate';
import { CounterComponent } from './Components/counter/counter.component';
import { GeneralManagerAreaComponent } from './Components/general-manager-area/general-manager-area.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
    PersonaldetailsComponent,
    VolunteerareaComponent,
    ConstraintsComponent,
    NeedyareaComponent,
    OrgmanagerareaComponent,
    NeedypersonaldetailsComponent,
    ManagerpersonaldetailsComponent,
    OrganizationsComponent,
    ScheduleComponent,
    ChunkPipe,
    VolunteerscheduleComponent,
    NeedyScheduleComponent,
    NeedyconstraintsComponent,
    AddupdateneedyComponent,
    AddUpdateVolunteerComponent,
    CounterComponent,
    GeneralManagerAreaComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    HttpClientModule,
    MatIconModule,
    MatCheckboxModule,
    MatSelectModule,
    MatSliderModule,
    MatDialogModule,
    MatStepperModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTreeModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatTabsModule,
    MatGridListModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatTableModule,
    NgbModule,
    MatSnackBarModule,
    EditorModule,
    TeximateModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    // AgmCoreModule.forRoot({
    //   apiKey: 'AIzaSyAyEYq8aMTFp3eNcRxWj3z4rFPLx7BamYo',
    //   libraries: ['places']
    // }),
    GooglePlaceModule,
    GoogleMapsModule,
    MatAutocompleteModule,
    MatExpansionModule,
    MatDialogModule
  ],
  providers: [
    DatePipe,
  ],
  bootstrap: [AppComponent]

})
export class AppModule { }
