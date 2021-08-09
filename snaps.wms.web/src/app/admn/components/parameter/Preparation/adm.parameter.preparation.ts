import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { distributionSharesl, pam_parameter } from 'src/app/admn/models/adm.parameter.model';
import { admparameterService } from 'src/app/admn/services/adm.parameter.service';
import { authService } from '../../../../auth/services/auth.service';
import { lov } from '../../../../helpers/lov';
import { admwarehouseService } from '../../../services/adm.warehouse.service';

declare var $: any;
@Component({
  selector: 'app-admpampreparation',
  templateUrl: 'adm.parameter.preparation.html'

})
export class admpampreparationComponent implements OnInit {
  public lslog: lov[] = new Array();
  public fixval: boolean = true;
  public pam: pam_parameter[] = new Array();
  //Stock
  stallowincludestaging: pam_parameter = new pam_parameter();
  stallowpartialprocess: pam_parameter = new pam_parameter();
  stallowstocklessthenorder: pam_parameter = new pam_parameter();
  stallowconsolidateorder: pam_parameter = new pam_parameter();
  stallowprocessbyselectline: pam_parameter = new pam_parameter();
  stmobiledigitIOloc: pam_parameter = new pam_parameter();
  stmobilecheckdigit: pam_parameter = new pam_parameter();
  stmobilescanbarcode: pam_parameter = new pam_parameter();
  stmobilefullypick: pam_parameter = new pam_parameter();
  stmobilerepickforshortage: pam_parameter = new pam_parameter();

  //Distribute
  dtallowincludestaging: pam_parameter = new pam_parameter();
  dtallowpartialprocess: pam_parameter = new pam_parameter();
  dtallowstocklessthanorder: pam_parameter = new pam_parameter();
  dtallowprocessbyselectline: pam_parameter = new pam_parameter();
  dtmobiledigitIOlc: pam_parameter = new pam_parameter();
  dtmobilecheckdigit: pam_parameter = new pam_parameter();
  dtmobilescanbarcode: pam_parameter = new pam_parameter();
  dtmobilefullypick: pam_parameter = new pam_parameter();

  //Operate
  opallowcancel: pam_parameter = new pam_parameter();
  opallowautoassign: pam_parameter = new pam_parameter();

  //Distribution Shared
  distributeShare: pam_parameter = new pam_parameter();
  distributeSharesl: distributionSharesl[] = [];
  //Date format
  public dateformat: string;
  public dateformatlong: string;
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

    this.distributeSharesl = [
      { title: 'Priority', subtitle: 'Share by store priority', name: 'priority' },
      { title: 'Average', subtitle: 'Share by average quantity', name: 'average' },
      { title: 'Ratio', subtitle: 'Share by ratio of order quantity', name: 'ratio'}
    ];
  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() { this.getMaster(); }
  getMaster() {
    this.sv.find().pipe().subscribe(
      (res) => {
        this.pam = res.filter(x => x.pmmodule == "preparation");

        //Stock
        this.stallowincludestaging = this.pam.find(x => x.pmtype == "stock" && x.pmcode == "allowincludestaging");
        this.stallowpartialprocess = this.pam.find(x => x.pmtype == "stock" && x.pmcode == "allowpartialprocess");
        this.stallowstocklessthenorder = this.pam.find(x => x.pmtype == "stock" && x.pmcode == "allowstocklessthenorder");
        this.stallowconsolidateorder = this.pam.find(x => x.pmtype == "stock" && x.pmcode == "allowconsolidateorder");
        this.stmobiledigitIOloc = this.pam.find(x => x.pmtype == "stock_mobile" && x.pmcode == "mobiledigitIOloc");
        this.stmobilecheckdigit = this.pam.find(x => x.pmtype == "stock_mobile" && x.pmcode == "mobilecheckdigit");
        this.stmobilescanbarcode = this.pam.find(x => x.pmtype == "stock_mobile" && x.pmcode == "mobilescanbarcode");
        this.stmobilefullypick = this.pam.find(x => x.pmtype == "stock_mobile" && x.pmcode == "mobilefullypick");
        this.stmobilerepickforshortage = this.pam.find(x => x.pmtype == "stock_mobile" && x.pmcode == "mobilerepickforshortage");

        //Distribute
        this.dtallowincludestaging = this.pam.find(x => x.pmtype == "distribute" && x.pmcode == "allowincludestaging");
        this.dtallowpartialprocess = this.pam.find(x => x.pmtype == "distribute" && x.pmcode == "allowpartialprocess");
        this.dtallowstocklessthanorder = this.pam.find(x => x.pmtype == "distribute" && x.pmcode == "allowstocklessthanorder");
        this.dtallowprocessbyselectline = this.pam.find(x => x.pmtype == "distribute" && x.pmcode == "allowprocessbyselectline");
        this.dtmobiledigitIOlc = this.pam.find(x => x.pmtype == "distribute_mobile" && x.pmcode == "mobiledigitIOlc");
        this.dtmobilecheckdigit = this.pam.find(x => x.pmtype == "distribute_mobile" && x.pmcode == "mobilecheckdigit");
        this.dtmobilescanbarcode = this.pam.find(x => x.pmtype == "distribute_mobile" && x.pmcode == "mobilescanbarcode");
        this.dtmobilefullypick = this.pam.find(x => x.pmtype == "distribute_mobile" && x.pmcode == "mobilefullypick");

        //Operate
        this.opallowcancel = this.pam.find(x => x.pmtype == "opereate" && x.pmcode == "allowcancel");
        this.opallowautoassign = this.pam.find(x => x.pmtype == "opereate" && x.pmcode == "allowautoassign");

        // distribuition shared
        this.distributeShare = this.pam.find(x => x.pmtype == "distribute" && x.pmcode == "distribute_share");
        this.distributionShareInit();
      },
      (err) => { this.toastr.error(err.error.message); },
      () => { }
    );
  }

  save(o: pam_parameter) {
    this.ngPopups.confirm('Please confirm to modify parameter ?')
      .subscribe(res => {
        if (res) {
          this.sv.set(o).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Modify success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        } else {
          o.pmvalue = !o.pmvalue;
        }
      });
  }
  distributionShareInit(){
    if(this.distributeShare == null) {
      // default shared
      this.distributeSharesl[0].checked = true;
    }else {
      this.distributeSharesl.forEach(e => {
        if (e.name == this.distributeShare.pmoption) {
          e.checked = true;
        }else{
          e.checked = false;
        }
      });
    }
  }
  slcShare(sl:distributionSharesl) {
    this.distributeSharesl.forEach(e => {
      if (e.name == sl.name) {
        e.lastchecked = sl.checked;
      }else{
        e.lastchecked = e.checked;
        e.checked = false;
      }
    });

    this.ngPopups.confirm('Please confirm to modify parameter ?').subscribe(res => {
      if (!res) {
        // reset 
        this.distributeSharesl.forEach(e => {
          e.checked =  e.lastchecked;
        });
      }else {
        // save
        let _distributeSharesl = this.distributeSharesl.find(x=>x.checked);
        if(_distributeSharesl != null){
          this.distributeShare.pmoption = _distributeSharesl.name;
          this.sv.set(this.distributeShare).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Modify success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      }
    });
  }
}

