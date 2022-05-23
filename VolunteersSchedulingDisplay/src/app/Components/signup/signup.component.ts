import { PersonaldetailsComponent } from './../personaldetails/personaldetails.component';
import { UserService } from './../../Services/user.service';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild ,Directive, ElementRef} from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { MatStep, MatStepper } from '@angular/material/stepper';
import { ErrorStateMatcher } from '@angular/material/core';

// /** Error when invalid control is dirty, touched, or submitted. */
// export class MyErrorStateMatcher implements ErrorStateMatcher {
//   isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
//     const isSubmitted = form && form.submitted;
//     return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
//   }
// }

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit{
  userId!:string;
  userEmail!:string;
  sendMail=false;
  show=true;
  TemporaryPassword!:number;
  TemporaryPasswordVertification!:number;
  firstFormGroup!: FormGroup;
  secondFormGroup!: FormGroup;
  ThirdFormGroup!:FormGroup;
  public loadingStep: boolean = false;
  isOptional = false;

  // idFormControl = new FormControl('', [Validators.required]);
  // matcher = new MyErrorStateMatcher();

  @ViewChild ('stepper') myStepper!:MatStepper;

  constructor(private _formBuilder: FormBuilder,private userService:UserService,private router:Router,private  route:ActivatedRoute) {}

  ngOnInit() {
    this.firstFormGroup = this._formBuilder.group({
      firstCtrl: [''],
    });
    this.secondFormGroup = this._formBuilder.group({
      secondCtrl: [''],
    });
    this.ThirdFormGroup = this._formBuilder.group({
      thirdCtrl: [''],
    });
  }

  async sendEmail()
  {
    this.userService.SendPassword(this.userEmail).subscribe(a=>this.TemporaryPassword=a);
  }

  isValidID(id:string) {
    var id = String(id).trim();
    if (id.length > 9 || id.length < 5 || isNaN(+id)) return false;

      id = id.length < 9 ? ("00000000" + id).slice(-9) : id;

      return Array
              .from(id, Number)
            .reduce((counter, digit, i) => {
              const step = digit * ((i % 2) + 1);
                          return counter + (step > 9 ? step - 9 : step);
                    }) % 10 === 0;
  }

  volunteerArea()
  {
    this.show=false;
    this.router.navigate(['volunteer-area'],{relativeTo:this.route});
  }

  vertifyPassword()
  {
    if (this.TemporaryPassword==this.TemporaryPasswordVertification)
    {
        this.myStepper.next();
        const add="add";
        this.loadingStep = true;
        //Wait 1 sec. before showing the step
        setTimeout(() => {
          this.router.navigate(['personal-details',add,this.userId],{relativeTo:this.route});
          this.loadingStep = false;
        }, 1000);

    }
    else
    {
        alert('סיסמה שגויה');
    }
  }

}
