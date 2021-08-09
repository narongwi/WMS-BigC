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
import { MultiDataSet, Label, ThemeService } from 'ng2-charts';
import * as Chart from 'chart.js';
import { ouhanderlingunitService } from '../../Services/oub.handerlingunit.service';
import { handerlingunit } from '../../Models/oub.handlingunit.model';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'appoub-preparation-distribute',
  templateUrl: 'oub.preparation.distribute.html',
  styles: ['.dgprep {  height:325px !important; } ' ,'.dglines { height:calc(100vh - 615px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class oubpreparationdistComponent implements OnInit {
  @ViewChild('pieChartCanvas') pieChartCanvas;
  public lsstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  public lsstatem: lov[] = new Array();
  public slcstate:lov;

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
  public stsum : number = 0;
  public linesumorder : number = 0;

  public pieChart: any;

  public pmhu:handerlingunit = new handerlingunit();

  public lshuempty:handerlingunit[] = new Array(); //List of HU
  public slchuempty:handerlingunit; // Selection of HU
  public slcline : prln_md = new prln_md();

  //public doughnutChartLabels: Label[] = ['BMW', 'Ford', 'Tesla'];
  //public doughnutChartData: MultiDataSet = [
  //  [55, 25, 20]
  //];
  //public doughnutChartType: ChartType = 'pie';

  //Toast Ref
  toastRef:any;
  public ordrowselect:number;
  public locrowselect:number;

  constructor(private sv: ouprepService,
    private av: authService,
    private mv: shareService,
    private hv: ouhanderlingunitService,
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
    this.lsstatem.push({ value : "IO", desc : "Waiting", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "",valopnthird : "" });
    this.lsstatem.push({ value : "PE", desc : "Finished", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "",valopnthird : "" });
    this.lsstatem.push({ value : "PA", desc : "Distributing", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "",valopnthird : "" });
  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() {
    // this.getstate(); this.gettype(); this.fnd(); 
    this.fnd();
  }

  getIcon(o: string) { return ""; }

  decstate(o: string,v:number) {    
    return this.lsstate.find(x => x.value == o).desc;   
   }
  //decstateicn(o: string) { return this.lsstate.find(x => x.value == o).icon; }
  //dectype(o: string) { return this.lstype.find(x => x.value == o).desc; }
  //decpreptype(o: string) { return (o == 'ST') ? "Stocking" : "Cross Dock"; }
  decpct(o:number) { 
    if (o == 0) { return "text-muted"; } else if (o > 0 && o <= 50) { return "text-danger"; } else if ( o > 51 && o <= 99) { return "text-warning"; } else { return "text-success"; }
  }
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
  fnd() {
    this.pm.spcarea = "XD";
    this.pm.preptype = "P";
    this.pm.tflow = (this.slcstate == null) ? null : this.slcstate.value;
    this.sv.find(this.pm).subscribe(
      (res) => {
        this.lsprep = res;
        this.stwaiting = this.lsprep.filter(x => x.tflow == "IO").length;
        this.pcwaiting = (this.stwaiting * 100) / this.lsprep.length;
        this.stcompleted = this.lsprep.filter(x => x.tflow == "PE").length;
        this.pccompleted = (this.stcompleted * 100) / this.lsprep.length;
        this.stpreparation = this.lsprep.filter(x => x.tflow == "PA").length;
        this.pcpreparation = (this.stpreparation * 100) / this.lsprep.length;
        this.stcancelled = this.lsprep.filter(x => x.tflow == "CL").length;
        this.pccancelled = (this.stcancelled * 100) / this.lsprep.length;
        this.stsum = this.lsprep.length;
        this.pieChartRender();
        if (this.lsprep.length > 0) { this.getinfo(this.lsprep[0],0); }
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  getinfo(o:prep_ls,ix:number) {
    this.ordrowselect = ix;
    this.sv.get(o).subscribe(
      (res) => {
        this.crprep = res;
        this.crlines = this.crprep.lines;
        this.crprep.thname = o.thname;
        this.linesumorder = this.crlines.reduce((a, b) => a + b.qtypuorder, 0);
        //console.log(this.crlines);
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  slcLoc(o:prln_md,ix:number) {
    this.locrowselect = ix;
    //console.log(o);
    this.pmhu.loccode = o.loccode;
    this.pmhu.hutype = "XE";
    this.pmhu.tflow = "IO";
    this.slcline = o;
    this.slchuempty = null;
    this.hv.find(this.pmhu).subscribe(
      (res) => { 
        this.lshuempty = res; 
        try {  this.slchuempty = this.lshuempty.find(e=>e.huno == this.slcline.huno )}catch (excp){ console.log("can not find empty "); }
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
              this.fnd();
              this.getinfo(this.crprep,this.ordrowselect);
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
              this.fnd();
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
              this.fnd();
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  opspick() {
    if(!this.ngValid(this.slchuempty.huno)){
      this.toastr.error("<span class='fn-1e15'>please selected empty huno</span>", null, { enableHtml: true });
    }else {
      this.ngPopups.confirm('Do you confirm put product ?')
      .subscribe(res => {
        if (res) {
          this.slcline.huno = this.slchuempty.huno;
          this.sv.opsPut(this.slcline).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>Put product to grid success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
    }
    
  }

  opscancel(){ 
    this.ngPopups.confirm('Do you cancel distribution plan ?')
    .subscribe(res => {
      if (res) {
        this.sv.opsCancel(this.crprep).subscribe(
          (res) => {
            this.toastr.success("<span class='fn-1e15'>Cancel plan success</span>", null, { enableHtml: true });
            this.fnd();
          },
          (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
          () => { }
        );
      }
    });
  }

  pieChartRender() {
    this.pieChart = new Chart(this.pieChartCanvas.nativeElement, {
      type: 'pie',
      data: {
        labels: ['Waiting', 'Stocking', 'Distribute', 'Completed', 'Cancelled'],
        datasets: [{
          label: '# of Votes',
          data: [this.pcwaiting, this.pcpreparation, this.pccompleted, this.pccancelled],
          backgroundColor: [
            'rgb(188, 71, 73)',
            'rgb(167, 201, 87)',
            'rgb(56, 102, 65)',
            'rgb(44, 54, 63)'
          ],
        }]
      },
      options: {
        legend: { display: false },
        title: { display: false }
      }
    });
  }


  // getdistributionlist(o: string) {
  //   window.open("http://localhost/bgcwmsdocument/get/distributelist?prepno=" + o, "_blank");
  // }

  getdistlist(){     
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.sv.getdistlist(this.crprep.orgcode, this.crprep.site, this.crprep.depot,this.crprep.prepno).subscribe(response => {
      let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_distribution_" + this.crprep.prepno + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId); 
      }), 
      error => { 
      this.toastr.clear(this.toastRef.ToastId);
      }

  }

  public ngValid(model: any) {
    var isvalid: boolean = false;
    if (model === undefined) {
      isvalid = false;
    } else if (model === null) {
      isvalid = false;
    } else if (model === "") {
      isvalid = false;
    } else if (this.ngTrim(model) === "") {
      isvalid = false;
    } else {
      isvalid = true;
    }
    return isvalid;
  }
  public ngTrim(str) {
    return str.toString().replace(/^\s+|\s+$/gm, "");
  }
  ngDecType(o:string){ try { return this.lstype.find(e=>e.value == o).desc } catch (exp) { return o; } }
  ngDecState(o:string) {  switch(o) { case 'IO': return 'Waiting'; case 'PA': return 'Distributing'; case 'PE': return 'Finished'; case 'CL': return 'Cancelled'; default: return o; } }
  ngDecIcon(o:string)  {  switch(o) { case 'IO': return 'fas fa-clock text-warning'; case 'PA': return 'fas fa-dolly text-danger'; case 'PE': return 'fas fa-clipboard-check text-success'; case 'CL': return 'fas fa-trash text-danger'; default: return o; } }
  ngDecUnitstock(o:string){ return this.mv.ngDecUnitstock(o); }
  ngDeclndesc(o:string){ return (o == "PA") ? "Distributing" : this.mv.ngDecState(o); }
  ngDestroy():void{ 

  }
}
