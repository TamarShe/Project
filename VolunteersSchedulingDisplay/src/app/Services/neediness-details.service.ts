import { NeedinessDetails } from './../Models/NeedinessDetails.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Schedule } from '../Models/Schedule.model';

@Injectable({
  providedIn: 'root'
})
export class NeedinessDetailsService {
  url='https://localhost:44320/api/neediness'

  constructor(private http:HttpClient){}

  GetNeedinessDetails(needinessCode: number):Observable<NeedinessDetails>
  {
    return this.http.get<NeedinessDetails>(`${this.url}/getneedinessdetails/${needinessCode}`);
  }

  GetNeedinessDetailsForNeedy(needyID:string,orgCode: number):Observable<NeedinessDetails>
  {
    return this.http.get<NeedinessDetails>(`${this.url}/getneedinessdetailsforneedy/${needyID}/${orgCode}`);
  }

  GetAllNeedinessDetailsForNeedy(needyID:string):Observable<NeedinessDetails[]>
  {
    return this.http.get<NeedinessDetails[]>(`${this.url}/GetAllNeedinessDetailsForNeedy/${needyID}`);
  }

  UpdateNeedinessDetails(needinessDetails:NeedinessDetails):Observable<number>
  {
    return this.http.post<number>(`${this.url}/updateneedinessdetails`,needinessDetails);
  }

  AddNeedinessDetails(needinessDetails:NeedinessDetails):Observable<number>
  {
    return this.http.post<number>(`${this.url}/addneedinessdetails`,needinessDetails);
  }
}


