import { Organization } from './../Models/Organization.model';
import { Volunteer } from './../Models/Volunteer.model';
import { Needy } from './../Models/Needy.model';
import { Manager } from './../Models/Manager.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class ManagerService {

  url='https://localhost:44320/api/manager'

  constructor(private http:HttpClient){}

  Login(managerId: string):Observable<string>
  {
    return this.http.get<string>(`${this.url}/login/${managerId}`);
  }

  FindManager(managerId: string):Observable<Manager>
  {
    return this.http.get<Manager>(`${this.url}/findmanager/${managerId}`);
  }

  SignUp(newmanager:Manager):Observable<string>
  {
      return this.http.post<string>(`${this.url}/signup`,newmanager);
  }

  update(newmanager:Manager):Observable<string>
  {
      return this.http.post<string>(`${this.url}/update`,newmanager);
  }

  uploadNeedies (needies: File,orgCode:number):Observable<string[][]>{
    let formData = new FormData();
    formData.append('needies',needies);
    return this.http.post<string[][]>(`${this.url}/loadNeedies/${orgCode}`,formData);
 }

 uploadVolunteers (volunteers: File,orgCode:number):Observable<string[][]>{
  let formData = new FormData();
  formData.append('volunteers',volunteers);
  return this.http.post<string[][]>(`${this.url}/loadVolunteers/${orgCode}`,formData);
}

 GetAllNeediesInOrg(orgCode: number):Observable<Needy[]>
 {
   return this.http.get<Needy[]>(`${this.url}/getAllNeediesInOrg/${orgCode}`);
 }

 GetAllVolunteersInOrg(orgCode: number):Observable<Volunteer[]>
 {
   return this.http.get<Volunteer[]>(`${this.url}/getAllVolunteersInOrg/${orgCode}`);
 }

 GetOrgCodeOfManager(managerID:string):Observable<number>{
  return this.http.get<number>(`${this.url}/orgcode/${managerID}`);
 }

 GetAllManagers():Observable<Manager[]>{
  return this.http.get<Manager[]>(`${this.url}/getAllManagers`);
 }

}
