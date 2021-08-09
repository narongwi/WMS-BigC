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
import { timestamp } from 'rxjs/operators';
import { collapseTextChangeRangesAcrossMultipleVersions } from 'typescript';
import { shareService } from 'src/app/share.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { pam_set } from 'src/app/admn/models/adm.parameter.model';

declare var $: any;
@Component({
  selector: 'appoub-preparation-stock',
  templateUrl: 'oub.preparation.stocking.html',
  styles: ['.dgchart {  height:273px  !important; }',
           '.dgprep {  height:235px  !important; }',
           '.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class oubpreparationstockComponent implements OnInit, OnDestroy {
  @ViewChild('pieChartCanvas') pieChartCanvas;

  public lstype: lov[] = new Array();
  public lsunit: lov[] = new Array();
  public lsprep: prep_ls[] = new Array();
  public crlines: prln_md[] = new Array();
  public slprep : prep_ls = new prep_ls();

  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;

  public pm: prep_pm = new prep_pm();
  public crprep: prep_md;
 

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

  public stwaitingpl: number = 0;
  public stpreparationpl: number = 0;
  public stcompletedpl: number = 0;
  public stcancelledpl: number = 0
  public pieChart: any;

    //Toast Ref
  toastRef:any;

  //Page limit
  public page = 4;
  public pageSize = 20;
  public slrowlmt:lov;
  public lsrowlmt:lov[] = new Array();

  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting

  //paramete
  public lsstate: lov[] = new Array();
  public lsyesno:lov[] = new Array();
  public lspreptype:lov[] = new Array();
  public slpreptype:lov;
  public slpriority:lov;
  public slstate:lov;

  //summary 
  public smorder:number = 0;
  public smpick:number = 0;
  public ordrowselect:number;
  constructor(private sv: ouprepService,
    private av: authService,
    private mv: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess(); this.ngSetup();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    //this.pm.prepdate = new Date();
  }

  ngOnInit(): void { }
  ngAfterViewInit() {  this.fnd(); }

  getIcon(o: string) { 
    switch(o){ 
      case "P" :return "fas fa-shopping-basket text-danger ";
      case "A" :return "fas fa-pallet text-success";
    }
  }
 
  decstate(o: string) { 
    try { 
      return this.lsstate.find(x => x.value == o).desc; 
    }catch (ex){
      return "";
    }    
  }
  decstateicn(o: string) { 
    try { 
      return this.lsstate.find(x => x.value == o).icon;
    }catch(ex){ 
      return "";
    }   
  }
  dectype(o: string) { return this.lstype.find(x => x.value == o).desc; }
  decpreptype(o: string) { return (o == 'ST') ? "Stocking" : "Cross Dock"; }

  fnd() {
    this.pm.spcarea = "ST";
    this.pm.preptype = (this.slpreptype != null) ? this.slpreptype.value : null;
    this.pm.priority = (this.slpriority != null) ? parseInt(this.slpriority.value) : null;
    this.pm.tflow = (this.slstate != null) ? this.slstate.value : null;
    this.sv.find(this.pm).subscribe(
      (res) => {
        this.lsprep = res;
        this.stwaiting = this.lsprep.filter(x => x.preptype == "P" && x.tflow == "IO").length;
        this.pcwaiting = (this.stwaiting * 100) / this.lsprep.length;
        this.stcompleted = this.lsprep.filter(x => ['PE','LD','ED'].includes(x.tflow)).length;
        this.pccompleted = (this.stcompleted * 100) / this.lsprep.length;
        this.stpreparation = this.lsprep.filter(x => x.preptype == "P" && x.tflow == "PA").length;
        this.pcpreparation = (this.stpreparation * 100) / this.lsprep.length;
        this.stcancelled = this.lsprep.filter(x => x.preptype == "P" && x.tflow == "CL").length;
        this.pccancelled = (this.stcancelled * 100) / this.lsprep.length;

        this.stwaitingpl = this.lsprep.filter(x => x.preptype != "P" && x.tflow == "IO").length;
        this.stcompleted = this.lsprep.filter(x => ['PE','LD','ED'].includes(x.tflow)).length;
        this.stpreparation = this.lsprep.filter(x => x.preptype != "P" && x.tflow == "PA").length;
        this.stcancelled = this.lsprep.filter(x => x.preptype != "P" && x.tflow == "CL").length;

        this.pieChartRender();
        if (this.lsprep.length > 0) { this.getinfo(this.lsprep[0],0); }
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  decpick(o:string) { 
    switch(o){
      case 'IO' : 'Waiting to start';
      case 'PA' : 'Pick has started';
      case 'PE' : 'Pick confirmed';
    }
  }
  getinfo(o: prep_ls,ix:number) {
    this.ordrowselect = ix;
    this.slprep = o;
    this.sv.get(o).subscribe(
      (res) => {
        this.crprep = res;
        this.crlines = this.crprep.lines;
        //this.crprep.lines.forEach(x => x.unitname = this.lsunit.find(u => u.value == x.unitprep).desc);
        this.crprep.thname = o.thname;
        this.smorder = this.crlines.reduce((a, b) => a + b.qtypuorder, 0);
        this.smpick = this.crlines.reduce((a, b) => a + b.qtypuops,0);
        //console.log(this.crlines);
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
              this.fnd();
              this.getinfo(this.slprep,this.ordrowselect);
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
              this.getinfo(this.slprep,this.ordrowselect);
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
          // BUG
          this.sv.opsPick(this.crlines).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-1e15'>Confirm pick success</span>", null, { enableHtml: true });
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
          //data: [ 50, 50, 0, 0 ],
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

    this.pieChart.update();
  }


  // getpicklist(o: string) {
  //   window.open("http://localhost/bgcwmsdocument/get/picklist?prepno=" + o, "_blank");
  // }

  getshiplabel(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });
    if (this.crprep.preptype == "P") { 
      this.sv.getshiplabel_loose(this.crprep.orgcode, this.crprep.site, this.crprep.depot,this.crprep.huno).subscribe(response => {
        let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_shipped_" + this.crprep.huno + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId); 
        }, 
        error => { 
        this.toastr.clear(this.toastRef.ToastId);
        });
    }else { 
      this.sv.getshiplabel_pallet(this.crprep.orgcode, this.crprep.site, this.crprep.depot,this.crprep.huno).subscribe(response => {
        let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_shipped_" + this.crprep.huno + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId); 
        }, 
        error => { 
        this.toastr.clear(this.toastRef.ToastId);
      });
    }

  }

  getpicklist(o:string){     
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.sv.getpicklist(this.crprep.orgcode, this.crprep.site, this.crprep.depot,this.crprep.prepno).subscribe(response => {
      let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_piclist_" + this.crprep.prepno + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId); 
       },
      error => { 
      this.toastr.clear(this.toastRef.ToastId);
    });

  }

  ngDecType(o:string){ try { return this.lstype.find(e=>e.value == o).desc } catch (exp) { return o; } }
  ngDecUnitstock(o:string){ return this.mv.ngDecUnitstock(o); }
  //ngDeclndesc(o:string){ return (o == "PC") ? "Preparing" : this.mv.ngDecState(o); }
  ngDecState(o:string) { try { return (o=="IO") ? "Waiting" : this.lsstate.find(e=>e.value == o).desc; }catch(excp){ return o;} }
  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
  ngSetup() {

    Promise.all([
      this.mv.getlov("PREP", "FLOW").toPromise(),
    ])
    .then((res) => {
      this.lsstate = res[0];
     
    });

    this.lsyesno = this.mv.getYesno();
    this.lsrowlmt = this.mv.getRowlimit();
    if (this.lsrowlmt.length == 0) { this.mv.ngIntRowlimit().subscribe((res)=> { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value)); }); }
    this.lspreptype.push({ desc : "Loose", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "", valopnthird : "", value : "P"});
    this.lspreptype.push({ desc : "Pallet", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "", valopnthird : "", value : "A"});
  }
  ngOnDestroy(): void {
    this.lsstate = null;  delete this.lsstate;
    this.lstype = null;   delete this.lstype;
    this.lsunit = null;   delete this.lsunit;
    this.lsprep = null;   delete this.lsprep;
    this.crlines = null;  delete this.crlines;
    this.instocksum = null; delete this.instocksum;
    this.rqedit = null;   delete this.rqedit;
    this.crstate = null;  delete this.crstate;
    this.pm = null;       delete this.pm;
    this.crprep = null;   delete this.crprep;
    this.dateformat = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.stwaiting = null; delete this.stwaiting;
    this.pcwaiting = null; delete this.pcwaiting;
    this.stpreparation = null; delete this.stpreparation;
    this.pcpreparation = null; delete this.pcpreparation;
    this.stcompleted = null; delete this.stcompleted;
    this.pccompleted = null; delete this.pccompleted;
    this.stcancelled = null; delete this.stcancelled;
    this.pccancelled = null; delete this.pccancelled;
    this.pieChart = null; delete this.pieChart;

   }
}
