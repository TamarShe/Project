import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  url='https://localhost:44320/api/user';

  constructor(private http:HttpClient){}

  Login(userID: string,userPassword:string):Observable<string>
  {
    return this.http.get<string>(`${this.url}/login/${userID}/${userPassword}`);
  }

  SendPassword(emailAddress: string):Observable<number>
  {
    return this.http.get<number>(`${this.url}/confirmPassword/${emailAddress}/`);
  }

  getAmounts():Observable<number[]>{
    return this.http.get<number[]>(`${this.url}/getAmounts`);
  }


}
