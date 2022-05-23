import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-orgmanagerarea',
  templateUrl: './orgmanagerarea.component.html',
  styleUrls: ['./orgmanagerarea.component.scss']
})
export class OrgmanagerareaComponent {
  userId!:string | null ;

  constructor(private router:Router,private route:ActivatedRoute)
  {
   this.userId= this.route.snapshot.paramMap.get('userid');
  }
}
