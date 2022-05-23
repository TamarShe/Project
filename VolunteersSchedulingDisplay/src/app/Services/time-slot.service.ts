import { TimeSlot } from './../Models/TimeSlot.model';
import { HttpClient } from '@angular/common/http';
import { Injectable, Input } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimeSlotService {

  url='https://localhost:44320/api/timeslot';

  constructor(private http:HttpClient){}

  AddTimeSlot(timeSlot:TimeSlot):Observable<number>
  {
    return this.http.post<number>(`${this.url}/addtimeslot`,timeSlot);
  }


  AddTimeSlots(timeSlots:TimeSlot[]):Observable<number[]>
  {
    return this.http.post<number[]>(`${this.url}/addtimeslots`,timeSlots);
  }

  FindTimeSlot(timeSlotCode:number):Observable<TimeSlot>
  {
    return this.http.get<TimeSlot>(`${this.url}/gettimeslot/${timeSlotCode}`);
  }

  GetTimeSlots(timeSlotsCodes:number[]):Observable<TimeSlot[]>
  {
    return this.http.get<TimeSlot[]>(`${this.url}/gettimeslots/${timeSlotsCodes}`);
  }


  DeleteTimeSlot(timeSlotCode:number):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/deletetimeslot/${timeSlotCode}`);
  }

  UpdateTimeSlot(timeSlot:TimeSlot):Observable<number>
  {
    return this.http.post<number>(`${this.url}/updatetimeslot`,timeSlot);
  }
}


