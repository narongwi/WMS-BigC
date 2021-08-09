import { ThrowStmt, _ParseAST } from '@angular/compiler';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { accn_signup } from '../models/accn_signup';
import { resultRequest } from '../../helpers/resultRequest';
import { authService } from '../services/auth.service';
import { NgProgressComponent } from 'ngx-progressbar';
import { TranslateService } from '@ngx-translate/core';
import { lov } from 'src/app/helpers/lov';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit , AfterViewInit{
  @ViewChild(NgProgressComponent) progressBar: NgProgressComponent;

  public accn: accn_signup = new accn_signup();
  public accnval: accn_signup = new accn_signup();
  public lswarehouse: lov[] = [];
  public slcwarehouse:string = "";
  public iserr: string = "E";
  public oberr: resultRequest = new resultRequest();
  constructor(private sv: authService, private router: Router, private translate: TranslateService,private toastr: ToastrService) {
    this.accn.lang = "en";
    if (this.sv.isAuthen) { this.router.navigate(['/Luanch']); }
  }
  ngSelcompare(item, selected) { return item.value === selected } /* ngSelect compare object */

  ngOnInit(): void { 
    this.sv.getWarehouse().subscribe( (res) => { this.lswarehouse = res;});
   
  }
  ngAfterViewInit() {
   }

  // whChange(value:string){
  //   this.slcwarehouse = value;
  // }
  public signin() {
    try {
      if (this.accn.accncode == "" || this.accn.accncode == null, this.accn.password == "" || this.accn.password == null) {
        this.iserr = "A";
        this.oberr.message = "Account code or password incorrect"
      }else if(this.accn.site == null||this.accn.site == undefined ||this.accn.site == ''){
        this.oberr.message = "Warehouse is required !"
      } else {
        this.progressBar.start();
        this.iserr = "P";
        this.accn.email = this.accn.accncode;

        this.sv.verify(this.accn).pipe().subscribe(
          rsl => {
            this.iserr = "E";
            this.sv.retriveProfile(this.accn.site).subscribe((res) => { 
              this.router.navigate(['/Luanch']);
             },err=>{
              this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });
              this.sv.signout();
             });
          }, (err) => {
            this.iserr = "A";
            this.oberr.message = err;
            console.log(err);
            // if (err.status == 400) {
            //   this.oberr.message = err.error.message;
            // } else {
            //   this.oberr.message = err.message;
            // }
            this.progressBar.complete();
          }
        );
      }

    } catch (exx) {
      this.iserr = "A";
      this.oberr.message = exx.message;
      this.progressBar.complete();
    }
  }
}
