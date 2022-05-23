import { Organization } from './../Models/Organization.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  url='https://localhost:44320/api/organization'

  constructor(private http:HttpClient){}

  getAllOrganizations():Observable<Organization[]>
  {
    return this.http.get<Organization[]>(`${this.url}/getallorgs`);
  }

  getOrg(orgCode:number):Observable<Organization>
  {
    return this.http.get<Organization>(`${this.url}/getorg/${orgCode}`);
  }

  getFreeOrgs(volunteerID:string):Observable<Organization[]>
  {
    return this.http.get<Organization[]>(`${this.url}/getfreeorgs/${volunteerID}`);
  }

  getDisabledOrgs(volunteerID:string):Observable<Organization[]>
  {
    return this.http.get<Organization[]>(`${this.url}/getdisabledorgs/${volunteerID}`);
  }


  getRelevantOrgsForNeedy(needyID:string):Observable<Organization[]>
  {
    return this.http.get<Organization[]>(`${this.url}/getRelevantOrgsForNeedy/${needyID}`);
  }

  
  getRelevantOrgsForVolunteer(volunteerID:string):Observable<Organization[]>
  {
    return this.http.get<Organization[]>(`${this.url}/getRelevantOrgsForVolunteer/${volunteerID}`);
  }

  updateOranization(org:Organization):Observable<number>
  {
    return this.http.post<number>(`${this.url}/updateOrganization`,org);
  }

  addOranization(org:Organization):Observable<number>
  {
    return this.http.post<number>(`${this.url}/addOrganization`,org);
  }
}
