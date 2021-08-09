import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { pam_set } from 'src/app/admn/models/adm.parameter.model';
import { thparty_ix } from 'src/app/admn/models/adm.thparty.model';
import { role_pm } from 'src/app/admn/models/role.model';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { prepset } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';

declare var $: any;
@Component({
  selector: 'appoub-processstock-summary',
  templateUrl: 'oub.process.stock.summary.html',
  styles: ['.dgsummary {  height:calc(100vh - 235px) !important; '],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]

})
export class oubprocessstocksummaryComponent implements OnInit, OnDestroy {
  @Output() processFinish = new EventEmitter();

  //Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;

  //Sorting 
  public lssort: string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //PageNavigate
  page = 4;
  pageSize = 200;
  slrowlmt: lov;
  lsrowlmt: lov[] = new Array();
  lsunit: lov[] = new Array(); //unit list

  //Process order object
  public proc: prepset = new prepset();

  //LOV state 
  lsstate: lov[] = new Array();
  lssubtype: lov[] = new Array();

  //Process status
  public isonProcess: number = 0;
  public parameters: pam_set[] = [];
  public allowpartialship: pam_set = new pam_set();
  public disableprocessbutton: boolean = false;
  constructor(private sv: outboundService,
    private pv: ouprepService,
    private av: authService,
    private mv: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess(); this.ngSetup();
    this.proc.orders = new Array();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }
  ngOnInit(): void {
    this.lsrowlmt = this.mv.getRowlimit();
  }
  //Decode

  ngSetup() {
    // this.mv.lov("OUBORDER","FLOW").pipe().subscribe(
    //   (res) => { this.lsstate = res; },
    //   (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
    //   () => { }
    // );
  }

  /* Row limit changing */
  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); }

  clearSelected() {
    this.proc = new prepset();
    this.disableprocessbutton = true;
  }

  checkparameter(){
    // yes data not found
    if (this.proc.orders.length == 0) {
      this.disableprocessbutton = true;

      // yes allow partial
    } else if (this.allowpartialship.pmvalue == true) {
      this.disableprocessbutton = false;

      // else not allow partial
    } else {
      var errorCount = 0;
      this.proc.orders.forEach(e=>{
        if(e.tflow != "WC") {errorCount ++;}
      });


      // check is error 
      if (errorCount > 0) {
        this.disableprocessbutton = true;
      }else{
        this.disableprocessbutton = false;
      }
    }
  }
  startprocess(o: prepset) {
    this.proc = new prepset();
    this.disableprocessbutton = true;

    this.proc = o;
    this.proc.spcarea = "ST";
    this.proc.procmodify = "prep.procsetup";
    this.proc.opsorder = this.proc.orders.length;
    this.sv.getparameter().subscribe(res => {
      this.parameters = res;this.allowpartialship = this.parameters.find(e => e.pmmodule == "outbound" && e.pmtype == "order" && e.pmcode=="allowpartialship");

      // set up preperateion list return SetNo
      this.pv.procsetup(this.proc).pipe().subscribe((res) => { 
          this.proc = res;
          this.checkparameter();
        },(err) => {
          this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true });
        }
      );
    });
  }

  launchprocess() {
    if(this.proc.setno != "" ){
    this.ngPopups.confirm('Do you confirm to process order ?').subscribe(res => {
        if (res) {
          this.isonProcess = 1;
          this.pv.procstock(this.proc.setno).pipe().subscribe(
            (res) => {
              this.proc = res;
              this.isonProcess=0;
              this.disableprocessbutton = true;
              this.processFinish.emit(true); // clear checkbox
              if( this.proc.orders.filter(x=>x.tflow=='XX').length > 0){
                this.toastr.warning("<span class='fn-1e15'>Process order with error </span>", null, { enableHtml: true });
              }else {
                this.toastr.success("<span class='fn-1e15'>Process order success </span>", null, { enableHtml: true });
              }
            },(err) => { 
                this.disableprocessbutton = false;
                this.isonProcess=0;
                this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); this.isonProcess = 0; },
            () => { }
          );
        }
      });
            
    }
  }

  ngDecUnitstock(o: string) { return this.mv.ngDecUnitstock(o); }
  ngDecIcon(o: string) { 
    try { 
      return o=='XX'?'fas fa-stop-circle text-danger':this.lsstate.find(x => x.value == o).icon; 
  } catch (excep) { 
    return o; 
  } 
  }
  ngDecStr(o: string, e: string) {
    //console.log(o);
    //console.log(e);
    try {
      switch (o) {
        case 'WC': return 'Waiting confirm';
        case 'CM': return 'Completed';
        case 'XX': return e;
        default: this.lsstate.find(x => x.value == o).desc
      }
    } catch (excep) { return o; }
  }
  ngOnDestroy(): void {
    this.processFinish.unsubscribe(); delete this.processFinish;
    this.dateformat = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.datereplan = null; delete this.datereplan;
    this.lssort = null; delete this.lssort;
    this.lsreverse = null; delete this.lsreverse;
    this.page = null; delete this.page;
    this.pageSize = null; delete this.pageSize;
    this.slrowlmt = null; delete this.slrowlmt;
    this.lsrowlmt = null; delete this.lsrowlmt;
    this.lsunit = null; delete this.lsunit;
    this.proc = null; delete this.proc;
    this.lsstate = null; delete this.lsstate;
    this.lssubtype = null; delete this.lssubtype;
  }
}