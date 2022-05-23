import { VolunteerPossibleTime } from './../Models/VolunteerPossibleTime.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {of, Observable } from 'rxjs';
import { Organization } from '../Models/Organization.model';
import { TimeSlot } from '../Models/TimeSlot.model';

@Injectable({
  providedIn: 'root'
})
export class VolunteerPossibleTimeService {
  url='https://localhost:44320/api/volunteerpossibletime'

  constructor(private http:HttpClient){}

  getAllPossibleTimeSlots(volunteeringDetailsCode:number):Observable<TimeSlot[]>
  {
    return this.http.get<TimeSlot[]>(`${this.url}/getallpossibletimeslots/${volunteeringDetailsCode}`);
  }

  addListOfPossibleTime(TimeSlots:TimeSlot[],volunteeringDetailsCode:number):Observable<boolean>
  {
    return this.http.post<boolean>(`${this.url}/addListOfPossibleTime/${volunteeringDetailsCode}`,TimeSlots);
  }

  deletepossibletimeSlot(timeSlotCode:number):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/deleteVolunteerPossibleTimeSlot/${timeSlotCode}`);
  }

  VolunteerHasScheduleInVolunteeringDetails(voluntteringDetailsCode:number):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/VolunteerHasScheduleInVolunteeringDetails/${voluntteringDetailsCode}`);
  }

}
