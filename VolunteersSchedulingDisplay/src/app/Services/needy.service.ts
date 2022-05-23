import { NeedinessDetails } from './../Models/NeedinessDetails.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Needy } from '../Models/Needy.model';

@Injectable({
  providedIn: 'root'
})
export class NeedyService {
  url='https://localhost:44320/api/needy'

  constructor(private http:HttpClient){}

  Login(needyCode: string):Observable<string>
  {
    return this.http.get<string>(`${this.url}/login/${needyCode}`);
  }

  Findneedy(needyID: string):Observable<Needy>
  {
    return this.http.get<Needy>(`${this.url}/findneedy/${needyID}`);
  }

  SignUp(newNeedy:Needy):Observable<string>
  {
      return this.http.post<string>(`${this.url}/signup`,newNeedy);
  }

  update(newNeedy:Needy):Observable<string>
  {
      return this.http.post<string>(`${this.url}/update`,newNeedy);
  }

  Delete(needyCode:string,needinessDetailsCode:number):Observable<boolean>
  {
      return this.http.get<boolean>(`${this.url}/delete/${needyCode}/${needinessDetailsCode}`);
  }

  GetNeedinessDetailsCode(needyCode: string):Observable<number>
  {
    return this.http.get<number>(`${this.url}/getneedinessdetailscode/${needyCode}`);
  }

  SendConstraintsToManager(needinessDetaisCode: number,subject:string,content:string):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/sendConstraintsToManager/${needinessDetaisCode}/${subject}/${content}`);
  }

  CheckIfPasswordIsPossible(userID: string,userPassword:string):Observable<boolean>
  {
    return this.http.get<boolean>(`${this.url}/checkIfPasswordIsPossible/${userID}/${userPassword}`);
  }
}
