import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { Volunteer } from '../Models/Volunteer.model';

@Injectable({
  providedIn: 'root'
})

export class VolunteerService {

  url='https://localhost:44320/api/volunteer'
  constructor(private http:HttpClient){}

  Login(volunteerId: string):Observable<string>
  {
    return this.http.get<string>(`${this.url}/login/${volunteerId}`);
  }

  FindVolunteer(volunteerId: string):Observable<Volunteer>
  {
    return this.http.get<Volunteer>(`${this.url}/findvolunteer/${volunteerId}`);
  }

  SendConstraintsToManager(volunteeringDetailsCode: number,subject:string,content:string):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/sendConstraintsToManager/${volunteeringDetailsCode}/${subject}/${content}`);
  }

  SignUp(newVolunteer:Volunteer):Observable<string>
  {
      return this.http.post<string>(`${this.url}/signup`,newVolunteer);
  }

  update(newVolunteer:Volunteer):Observable<string>
  {
      return this.http.post<string>(`${this.url}/update`,newVolunteer);
  }

  Delete(volunteerID:string,volunteeringDetailsCode:number):Observable<boolean>
  {
      return this.http.get<boolean>(`${this.url}/delete/${volunteerID}/${volunteeringDetailsCode}`);
  }

  GetVolunteeringDetailsCode(volunteerID: string):Observable<number>
  {
    return this.http.get<number>(`${this.url}/getvolunteeringdetailscode/${volunteerID}`);
  }

  CheckIfPasswordIsPossible(userID: string,userPassword:string):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/checkIfPasswordIsPossible/${userID}/${userPassword}`);
  }


}
