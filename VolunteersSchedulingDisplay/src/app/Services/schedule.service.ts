import { Schedule } from './../Models/Schedule.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  url='https://localhost:44320/api/schedule'

  constructor(private http:HttpClient){}

  GetScheduleForVolunteer(volunteerID: string):Observable<any[]>
  {
    return this.http.get<any[]>(`${this.url}/getScheduleforvolunteer/${volunteerID}`);
  }

  GetScheduleForNeedy(needyID: string):Observable<any[]>
  {
    return this.http.get<any[]>(`${this.url}/getScheduleforneedy/${needyID}`);
  }

  GetScheduleForMnager(managerID: string):Observable<any[]>
  {
    return this.http.get<any[]>(`${this.url}/getScheduleformanager/${managerID}`);
  }

  DeleteFromSchedule(scheduleCode: number):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/deletefromschedule/${scheduleCode}`);
  }


  AddToSchedule(schedule: Schedule):Observable<number>
  {
    return this.http.post<number>(`${this.url}/addtoschedule`,schedule);
  }

}
