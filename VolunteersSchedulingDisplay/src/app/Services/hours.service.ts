import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { hours } from '../Models/Hours.model';

@Injectable({
  providedIn: 'root'
})
export class HoursService {
  url='https://localhost:44320/api/hours'

  constructor(private http:HttpClient){}

  GetListOfStartAndEnd(TimeDuration:number):Observable<hours[][]>
  {
    return this.http.get<hours[][]>(`${this.url}/getListOfStartAndEnd/${TimeDuration}`);
  }

  GetAllHours():Observable<hours[]>
  {
    return this.http.get<hours[]>(`${this.url}/getAllHours`);
  }
}
