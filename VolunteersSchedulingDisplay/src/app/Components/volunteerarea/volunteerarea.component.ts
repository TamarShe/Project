import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-volunteerarea',
  templateUrl: './volunteerarea.component.html',
  styleUrls: ['./volunteerarea.component.scss']
})
export class VolunteerareaComponent {
  userId!:string | null ;

  constructor(private router:Router,private route:ActivatedRoute)
  {
   this.userId= this.route.snapshot.paramMap.get('volunteerid');
  }

}
