import { Component, OnInit, ɵCompiler_compileModuleSync__POST_R3__ } from '@angular/core';
import { accn_signup } from '../models/accn_signup';
import { authService } from '../services/auth.service';
@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
  public accn:accn_signup = new accn_signup();
  public accnval:accn_signup = new accn_signup();
  constructor(private sv: authService) { this.accn.lang = "en";}
  ngOnInit(): void {

  }
  emailIsValid (email) { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email) }

  public valemail() { 
    this.accnval.email = (this.emailIsValid(this.accn.email)) ? "1" : "0";
  }
  public signup() { 
    try { 
      this.accn.accncode = this.accn.email;
      this.sv.signup(this.accn).pipe().subscribe(
        rsl => { console.log(rsl); },
        err => { }
      );
    }catch (exx){ 
      console.log(exx);
    }
  }

}
