import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { pam_inbound, pam_list, pam_parameter } from 'src/app/admn/models/adm.parameter.model';
import { authService } from '../../../../auth/services/auth.service';
import { lov } from '../../../../helpers/lov';
import { admparameterService } from '../../../services/adm.parameter.service';

declare var $: any;
@Component({
  selector: 'app-admpaminbound',
  templateUrl: 'adm.parameter.inbound.html'

})
export class admpaminboundComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;

  public pam:pam_parameter[] = new Array();

  public pmallowcalculatemfg:pam_parameter = new pam_parameter();
  public pmallowpartail:pam_parameter = new pam_parameter();
  public pmallowoverplan : pam_parameter = new pam_parameter();
  public pmallowexpired : pam_parameter = new pam_parameter();
  public pmallowchangeunit : pam_parameter = new pam_parameter();
  public pmallowshowqtyorder : pam_parameter = new pam_parameter();
  public pmallowautostaging : pam_parameter = new pam_parameter();
  public pmallowcontrolcapacity : pam_parameter = new pam_parameter();
  public pmallowchangepriority : pam_parameter = new pam_parameter();
  public pmallowcancel : pam_parameter = new pam_parameter();
  public pmallowgendistplan : pam_parameter = new pam_parameter();
  public pmallowgenstckputaway : pam_parameter = new pam_parameter();
  public pmallowreplandelivery : pam_parameter = new pam_parameter();

  
  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;

  constructor(private sv: admparameterService,
              private av: authService, 
              private router: RouterModule,
              private toastr: ToastrService,
              private ngPopups: NgPopupsService) { 
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

        this.pam = res.filter(x=>x.pmmodule == "inbound");
        this.pmallowcalculatemfg = this.pam.find(x=>x.pmcode == "allowcalculatemfg");
        this.pmallowpartail = this.pam.find(x=>x.pmcode == "allowpartail");
        this.pmallowoverplan = this.pam.find(x=>x.pmcode == "allowoverplan");
        this.pmallowexpired = this.pam.find(x=>x.pmcode == "allowexpired");
        this.pmallowchangeunit = this.pam.find(x=>x.pmcode == "allowchangeunit");
        this.pmallowshowqtyorder = this.pam.find(x=>x.pmcode == "allowshowqtyorder");
        this.pmallowautostaging = this.pam.find(x=>x.pmcode == "allowautostaging");
        this.pmallowcontrolcapacity = this.pam.find(x=>x.pmcode == "allowcontrolcapacity");
        this.pmallowchangepriority = this.pam.find(x=>x.pmcode == "allowchangepriority");
        this.pmallowcancel = this.pam.find(x=>x.pmcode == "allowcancel");
        this.pmallowgendistplan = this.pam.find(x=>x.pmcode == "allowgendistplan");
        this.pmallowgenstckputaway = this.pam.find(x=>x.pmcode == "allowgenstckputaway");
        this.pmallowreplandelivery = this.pam.find(x=>x.pmcode == "allowreplandelivery");
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
