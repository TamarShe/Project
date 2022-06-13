import { TimeSlot } from 'src/app/Models/TimeSlot.model';
import { NeedyPossibleTime } from './../Models/NeedyPossibleTime.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VolunteerPossibleTime } from '../Models/VolunteerPossibleTime.model';

@Injectable({
  providedIn: 'root'
})
export class NeedyPossibleTimeService {
  url='https://localhost:44320/api/needypossibletime'

  constructor(private http:HttpClient){}

  getAllPossibleTime(needinessDetailsCode:number):Observable<NeedyPossibleTime[]>
  {
    return this.http.get<NeedyPossibleTime[]>(`${this.url}/getallpossibletime/${needinessDetailsCode}`);
  }

  getAllPossibleTimeSlots(needinessDetailsCode:number):Observable<TimeSlot[]>
  {
    return this.http.get<TimeSlot[]>(`${this.url}/getallpossibletimeslots/${needinessDetailsCode}`);
  }

  addListOfPossibleTime(TimeSlots:TimeSlot[],needinessDetailsCode:number):Observable<boolean>
  {
    return this.http.post<boolean>(`${this.url}/addListOfPossibleTime/${needinessDetailsCode}`,TimeSlots);
  }

  deletepossibletimeSlot(timeSlotCode:number):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/deleteNeedyPossibleTimeSlot/${timeSlotCode}`);
  }

  NeedyHasConflics(needinessDetailscode:number):Observable<number>
  {
    return this.http.get<number>(`${this.url}/NeedyHasConflics/${needinessDetailscode}`);
  }

  DeleteConflics(needinessDetailscode:number):Observable<number>
  {
    return this.http.get<number>(`${this.url}/DeleteConflicts/${needinessDetailscode}`);
  }
}
