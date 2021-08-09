import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ɵangular_packages_platform_browser_platform_browser_d } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { prep_ls, prep_md, prep_pm, prln_md } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { ChartType } from 'chart.js';
import * as Chart from 'chart.js';

declare var $: any;
@Component({
  selector: 'appoub-mobile-dist',
  templateUrl: 'oub.mobile.dist.html'

})
export class oubmobiledistComponent implements OnInit {

  public lsstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  public lsunit: lov[] = new Array();
  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;
  public lsprep: prep_ls[] = new Array();
  public pm: prep_pm = new prep_pm();
  public crprep: prep_md;
  public crlines: prln_md[] = new Array();

  public dateformat: String;
  public dateformatlong: String;

  public stwaiting: number = 0;
  public pcwaiting: number = 0;
  public stpreparation: number = 0;
  public pcpreparation: number = 0;
  public stcompleted: number = 0;
  public pccompleted: number = 0;
  public stcancelled: number = 0;
  public pccancelled: number = 0;

  public pieChart: any;

  //public doughnutChartLabels: Label[] = ['BMW', 'Ford', 'Tesla'];
  //public doughnutChartData: MultiDataSet = [
  //  [55, 25, 20]
  //];
  //public doughnutChartType: ChartType = 'pie';

  constructor(private sv: ouprepService,
    private av: authService,
    private mv: adminService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.pm.spcarea = "";
    this.pm.routeno = "";
    this.pm.huno = "";
    this.pm.preptype = "";
    this.pm.prepno = "";
    this.pm.prepdate = "";
    this.pm.thcode = "";
    this.pm.spcorder = "";
    this.pm.spcarticle = "";
    this.pm.dateassign = "";
    this.pm.tflow = "";
    this.pm.deviceID = "";

  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() { this.getstate(); this.gettype(); this.getunit(); }

  getIcon(o: string) { return ""; }

  decstate(o: string) { return this.lsstate.find(x => x.value == o).desc; }
  decstateicn(o: string) { return this.lsstate.find(x => x.value == o).icon; }
  dectype(o: string) { return this.lstype.find(x => x.value == o).desc; }
  decpreptype(o: string) { return (o == 'ST') ? "Stocking" : "Cross Dock"; }
  getstate() {
    this.mv.getlov("OUORDER", "FLOW").pipe().subscribe(
      (res) => { this.lsstate = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  gettype() {
    this.mv.getlov("TASK", "TYPE").pipe().subscribe(
      (res) => { this.lstype = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  getunit() {
    this.mv.getlov("UNIT", "KEEP").pipe().subscribe(
      (res) => { this.lsunit = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  fnd() {
    this.pm.preptype = "ST";
    this.pm.prepno = "19535";//hardcode
    this.sv.find(this.pm).subscribe(
      (res) => {
        this.lsprep = res; this.getinfo(this.lsprep[0]);
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  getinfo(o: prep_ls) {
    this.sv.get(o).subscribe(
      (res) => {
        this.crprep = res;
        this.crlines = this.crprep.lines;
        this.crprep.lines.forEach(x => x.unitname = this.lsunit.find(u => u.value == x.unitprep).desc);
        this.crprep.thname = o.thname;
        console.log(this.crlines);
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  setpriority() {
    this.ngPopups.confirm('Do you confirm to set priority  ?')
      .subscribe(res => {
        if (res) {
          this.sv.setpriority(this.crprep).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>set priority success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }
  setstart() {
    this.ngPopups.confirm('Do you confirm to start preparation  ?')
      .subscribe(res => {
        if (res) {
          this.sv.setStart(this.crprep).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>Start preparation success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }
  setend() {
    this.ngPopups.confirm('Do you confirm to end preparation  ?')
      .subscribe(res => {
        if (res) {
          this.sv.setEnd(this.crprep).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>End preparation success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  opspick() {
    this.ngPopups.confirm('Do you confirm pick ?')
      .subscribe(res => {
        if (res) {
          this.crlines.forEach(x => x.qtyskuops = x.rtoskuofpu * x.qtypuops);
          this.sv.opsPick(this.crlines[0]).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>Confirm pick success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  getpicklist(o: string) {
    window.open("http://localhost/bgcwmsdocument/get/picklist?prepno=" + o, "_blank");
  }

  getlabel(o: string) {
    window.open("http://localhost/bgcwmsdocument/get/labelshipped?huno=" + o, "_blank");
  }



}
