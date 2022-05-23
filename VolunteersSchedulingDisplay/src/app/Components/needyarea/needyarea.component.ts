import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-needyarea',
  templateUrl: './needyarea.component.html',
  styleUrls: ['./needyarea.component.scss']
})
export class NeedyareaComponent {
  userId!:string | null ;

  constructor(private router:Router,private route:ActivatedRoute)
  {
   this.userId= this.route.snapshot.paramMap.get('userid');
  }

}
