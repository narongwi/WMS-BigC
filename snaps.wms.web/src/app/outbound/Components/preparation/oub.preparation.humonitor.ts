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
import { MultiDataSet, Label } from 'ng2-charts';
import * as Chart from 'chart.js';
import { handerlingunit, handerlingunit_item } from '../../Models/oub.handlingunit.model';
import { ouhanderlingunitService } from '../../Services/oub.handerlingunit.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'appoub-preparation-humonitor',
  templateUrl: 'oub.preparation.humonitor.html',
  styles: ['.dgprep {  height:325px !important; } ','.dglines { height:calc(100vh - 650px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class oubpreparationhumonComponent implements OnInit, OnDestroy {
  @ViewChild('pieChartCanvas') pieChartCanvas;
  public lsstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  public lsstatem:lov[] = new Array();

  public slcstate:lov;

  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;
  //public lsprep: prep_ls[] = new Array();
  //public pm: prep_pm = new prep_pm();
  public crprep: prep_md;
  public crlines: handerlingunit_item[] = new Array();

  public lshu: handerlingunit[] = new Array();
  public pmhu: handerlingunit = new handerlingunit();
  public crhu: handerlingunit = new handerlingunit();

  public dateformat: String;
  public dateformatlong: String;

  public stfree: number = 0;
  public pcfree: number = 0;
  public stwaiting: number = 0;
  public pcwaiting: number = 0;
  public stcompleted: number = 0;
  public pccompleted: number = 0;
  public stload: number = 0;
  public pcload: number = 0;
  public stsum: number = 0;
  public crsumlnhu : number = 0;

  public streach: number = 0;

  public pieChart: any;

  public pmprep: handerlingunit = new handerlingunit();

  //Toast Ref
  toastRef:any;

  //public doughnutChartLabels: Label[] = ['BMW', 'Ford', 'Tesla'];
  //public doughnutChartData: MultiDataSet = [
  //  [55, 25, 20]
  //];
  //public doughnutChartType: ChartType = 'pie';
  public ordrowselect:number;
  constructor(private sv: ouhanderlingunitService,
    private av: authService,
    private mv: shareService,
    private ov: ouprepService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.pmhu.hutype = "XE";
    this.pmhu.spcarea = "XD";
    this.pmprep.routeno = "";
    this.pmprep.huno = "";
    this.pmprep.thcode = "";
    this.pmprep.tflow = "";
    this.pmprep.spcarea = "XD";
    this.pmprep.hutype = "XE";
    this.lsstatem.push({ value : "IO", desc : "Active", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "",valopnthird : "" });
    this.lsstatem.push({ value : "LD", desc : "Loading", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "",valopnthird : "" });
    this.lsstatem.push({ value : "PE", desc : "Close", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "",valopnthird : "" });

  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() { 
    //  this.getstate(); this.gettype(); this.fnd();
  }

  getIcon(o: string) { return ""; }

  decstate(o: string) { return this.lsstate.find(x => x.value == o).desc; }
  decstateicn(o: string) { return this.lsstate.find(x => x.value == o).icon; }
  dectype(o: string) { return this.lstype.find(x => x.value == o).desc; }
  decpreptype(o: string) { return (o == 'ST') ? "Stocking" : "Cross Dock"; }
  getstate() {
    this.mv.getlov("HANDERLING", "FLOW").pipe().subscribe(
      (res) => { this.lsstate = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  gettype() {
    this.mv.getlov("HANDERLING", "TYPE").pipe().subscribe(
      (res) => { this.lstype = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  fnd() {
    this.pmprep.tflow = (this.slcstate != null) ? this.slcstate.value : null;
    this.sv.find(this.pmprep).subscribe(
      (res) => {
        this.lshu = res;
        this.stfree = this.lshu.filter(x => x.tflow == "IO" && x.crsku == 0).length;
        this.pcfree = (this.stfree * 100) / this.lshu.length;
        this.stwaiting = this.lshu.filter(x => x.tflow == "IO" && x.crsku > 0).length;
        this.pcwaiting = (this.stwaiting * 100) / this.lshu.length;
        this.stcompleted = this.lshu.filter(x => x.tflow == "PE").length;
        this.pccompleted = (this.stcompleted * 100) / this.lshu.length;
        this.stload = this.lshu.filter(x => x.tflow == "LD").length;
        this.pcload = (this.stload * 100) / this.lshu.length;
        this.streach = this.lshu.filter(e=> e.crcapacity > 80 && e.tflow == "IO").length;
        this.stsum = this.lshu.length;
        this.pieChartRender(); 
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  getinfo(o: handerlingunit,ix:number) {
    this.ordrowselect = ix;
    this.crhu = o;
    // this.pmprep.huno = o.huno;
    // this.pmprep.spcarea = "XD";
    this.sv.linesnonsum(o).subscribe(
      (res) => {
        //this.crprep = res;
        this.crlines = res;
        this.crsumlnhu = this.crlines.reduce((obj,val) => obj += val.qtypu,0);
        //this.crlines.forEach(x => x.unitname = "SKU");
        //this.crprep.thname = o.thname;
        //console.log(this.crlines);
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  opsbase() {
    //close
    this.ngPopups.confirm('Do you confirm to Close ')
     .subscribe(res => {
       if (res) {

       }
     });
  }

  closehu() {
    this.ngPopups.confirm('Do you confirm close distribution empty ?')
      .subscribe(res => {
        if (res) {
          this.sv.close(this.crhu).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>Close HU success</span>", null, { enableHtml: true });
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
        labels: ['Waiting', 'Completed', 'Loaded'],
        datasets: [{
          label: '# of Votes',
          data: [this.pcfree,this.pcwaiting, this.pccompleted, this.pcload],
          backgroundColor: [
            'rgb(188, 71, 73)',
            'rgb(167, 201, 87)',
            'rgb(106, 153, 78)',
            'rgb(56, 102, 65)',
          ],
        }]
      },
      options: {
        legend: { display: false },
        title: { display: false }
      }
    });
  }

  // getlabel(o: string) {
  //   window.open("http://localhost/bgcwmsdocument/get/labelshipped?huno=" + o, "_blank");
  // }


  getdistlabel(){     
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.ov.getshiplabel_distribution(this.crhu.orgcode, this.crhu.site, this.crhu.depot,this.crhu.huno).subscribe(response => {
      let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_shiplabel_" + this.crhu.huno + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId); 
      }, error => { 
      this.toastr.clear(this.toastRef.ToastId);
      });
  }

  gethuemptylabel(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.sv.gethuempty_label(this.crhu.orgcode, this.crhu.site, this.crhu.depot,o).subscribe(response => {
        let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_huempty_" + o + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId); 
      }, error => { 
          this.toastr.clear(this.toastRef.ToastId);
      });
  }

  ngDecType(o:string){ try { return this.lstype.find(e=>e.value == o).desc } catch (exp) { return o; } }
  ngDecState(o:string) { switch(o) { case 'IO': return 'Active'; case 'PA': return 'Distributing'; case 'PE': return 'Close'; case 'LD': return 'Loading'; case 'XX': return 'Cancelled'; default: return o; } }
  ngDecIcon(o:string)  { switch(o) { case 'IO': return 'fa fa-pallet'; case 'PE': return 'fa fa-check-circle text-success'; case 'LD': return 'fa fa-truck text-danger'; case 'XX': return 'Cancelled'; default: return o; } }
  ngDecSquare(s:string, o:number){ 
    if (s=='IO' && o == 0) { return 'fas fa-square fn-palette-5' } 
    else if (s=='IO' && o >= 80) { return 'fas fa-sqaure text-warning'} 
    else if (['PE','ED'].includes(s)) { return 'fas fa-square fn-palette-1'} 
    else if (['LD'].includes(s)) { return 'fas fa-square fn-palette-1'} 
    else { return "fas fa-square" } }
  ngDecUnitstock(o:string){ return this.mv.ngDecUnitstock(o); }
  ngDeclndesc(o:string){ return (o == "PA") ? "Distributing" : this.mv.ngDecState(o); }
  ngDestroy():void{ 
    this.lsstate        = null; delete this.lsstate;
    this.lstype         = null; delete this.lstype;
    this.lsstatem       = null; delete this.lsstatem;
    this.slcstate       = null; delete this.slcstate;
    this.instocksum     = null; delete this.instocksum;
    this.rqedit         = null; delete this.rqedit;
    this.crstate        = null; delete this.crstate;
    this.crprep         = null; delete this.crprep;
    this.crlines        = null; delete this.crlines;
    this.lshu           = null; delete this.lshu;
    this.pmhu           = null; delete this.pmhu;
    this.crhu           = null; delete this.crhu;
    this.dateformat     = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.stfree         = null; delete this.stfree;
    this.pcfree         = null; delete this.pcfree;
    this.stwaiting      = null; delete this.stwaiting;
    this.pcwaiting      = null; delete this.pcwaiting;
    this.stcompleted    = null; delete this.stcompleted;
    this.pccompleted    = null; delete this.pccompleted;
    this.stload         = null; delete this.stload;
    this.pcload         = null; delete this.pcload;
    this.stsum          = null; delete this.stsum;
    this.streach        = null; delete this.streach;
    this.pieChart       = null; delete this.pieChart;
    this.pmprep         = null; delete this.pmprep;
    this.toastRef       = null; delete this.toastRef ;     
  }

}
