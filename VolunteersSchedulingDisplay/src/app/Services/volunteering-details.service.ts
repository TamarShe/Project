import { VolunteeringDetails } from './../Models/VolunteeringDetails.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VolunteeringDetailsService {
  url='https://localhost:44320/api/volunteeringdetails'
  constructor(private http:HttpClient){}

  SignUpToOrg(newVolunteeringDetails:VolunteeringDetails):Observable<boolean>
  {
      return this.http.post<boolean>(`${this.url}/signuptoorg`,newVolunteeringDetails);
  }

  GetVolunteeringDetailsForVolunteer(volunteerID:string,orgCode: number):Observable<VolunteeringDetails>
  {
    return this.http.get<VolunteeringDetails>(`${this.url}/getvolunteeringdetailsforvolunteer/${volunteerID}/${orgCode}`);
  }

  GetAllVolunteeringDetailsForVolunteer(volunteerID:string):Observable<VolunteeringDetails[]>
  {
    return this.http.get<VolunteeringDetails[]>(`${this.url}/GetAllVolunteeringDetailsForVolunteer/${volunteerID}`);
  }


  UpdatVolunteeringDetails(volunteeringDetails:VolunteeringDetails):Observable<number>
  {
    return this.http.post<number>(`${this.url}/updatevolunteeringdetails`,volunteeringDetails);
  }


  AddVolunteeringDetails(volunteeringDetails:VolunteeringDetails):Observable<number>
  {
    return this.http.post<number>(`${this.url}/addvolunteeringdetails`,volunteeringDetails);
  }
}
