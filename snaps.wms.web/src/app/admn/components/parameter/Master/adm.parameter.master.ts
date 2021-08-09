import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { pam_list, pam_parameter } from 'src/app/admn/models/adm.parameter.model';
import { admparameterService } from 'src/app/admn/services/adm.parameter.service';
import { authService } from '../../../../auth/services/auth.service';
import { lov } from '../../../../helpers/lov';
import { admwarehouseService } from '../../../services/adm.warehouse.service';

declare var $: any;
@Component({
  selector: 'app-admpammaster',
  templateUrl: 'adm.parameter.master.html'

})
export class admpammasterComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public pam:pam_parameter[] = new Array();

  //Product 
  prallowchangeofexsource:pam_parameter = new pam_parameter();
  prallowchangehirachy:pam_parameter = new pam_parameter();
  prallowchangedimension:pam_parameter = new pam_parameter();
  prallowchangedlc:pam_parameter = new pam_parameter();
  prallowchangeunit:pam_parameter = new pam_parameter();
  //third party
  thallowchangeofexsource:pam_parameter = new pam_parameter();
  thallowchangeplandate:pam_parameter = new pam_parameter();
  thallowchangestate:pam_parameter = new pam_parameter();
  //Barcode 
  brallowchangeofexsource:pam_parameter = new pam_parameter(); 
  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;

  constructor(
    private sv: admparameterService,
    private av: authService, 
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService
  ) { 
    this.av.retriveAccess(); 
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ this.getMaster(); }


  getMaster() { 
    this.sv.find().pipe().subscribe(            
      (res) => { 
        this.pam = res.filter(x=>x.pmmodule == "master");
        //this.prallowchangeofexsource = this.pam.find(x=>x.pmtype == "product" && x.pmcode == "allowchangeofexsource");
        this.prallowchangehirachy = this.pam.find(x=>x.pmtype == "product" && x.pmcode == "allowchangehirachy");
        this.prallowchangedimension = this.pam.find(x=>x.pmtype == "product" && x.pmcode == "allowchangedimension");
        this.prallowchangedlc = this.pam.find(x=>x.pmtype == "product" && x.pmcode == "allowchangedlc");
        this.prallowchangeunit = this.pam.find(x=>x.pmtype == "product" && x.pmcode == "allowchangeunit");
        this.thallowchangeofexsource = this.pam.find(x=>x.pmtype == "thirdparty" && x.pmcode == "allowchangeofexsource");
        this.thallowchangeplandate = this.pam.find(x=>x.pmtype == "thirdparty" && x.pmcode == "allowchangeplandate");
        this.thallowchangestate = this.pam.find(x=>x.pmtype == "thirdparty" && x.pmcode == "allowchangestate");
        this.brallowchangeofexsource = this.pam.find(x=>x.pmtype == "barcode" && x.pmcode == "allowchangeofexsource");
      },
      (err) => { this.toastr.error(err.error.message);  },
      () => { }
  );
   }
  save(o:pam_parameter) { 
      this.ngPopups.confirm('Please confirm to modify parameter ?')
      .subscribe(res => { 
      if (res) {
        this.sv.set(o).subscribe(
          (res) => {  this.toastr.success("<span class='fn-07e'>Modify success </span>",null,{ enableHtml : true });},
          (err) => {  this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });},
          () => {}
          );
        }else { 
          o.pmvalue = !o.pmvalue;
        }
      });
  }

}
