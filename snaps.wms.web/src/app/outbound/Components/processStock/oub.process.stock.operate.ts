import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { FakeMissingTranslationHandler } from '@ngx-translate/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { prepset, prepsln } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';

declare var $: any;
@Component({
  selector: 'appoub-processstock-operate',
  templateUrl: 'oub.process.stock.operate.html',
  styles: ['.dgorder {  height:250px !important;', '.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]

})
export class oubprocessstockoperateComponent implements OnInit, OnDestroy {
  @Output() selecProcess = new EventEmitter();

  //List of state
  public lsstate: lov[] = new Array();
  //List of order 
  public lsorder: outbound_ls[] = new Array();
  //Order object
  public slcorder: outbound_md;
  public slclines: outbouln_md[] = new Array();
  public slcline: outbouln_md;

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

  //flag for revise order line 
  public rqedit: number = 0;
  //flag remarks 
  public chnremark: number = 0;

  //Process order object
  public proc: prepset = new prepset();

  //Change request date 
  public chnrqdate: number = 0;
  public lssubtype: lov[] = new Array();

  constructor(private sv: outboundService,
    private av: authService,
    private mv: shareService,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }
  ngOnInit(): void { }

  //Todo get data form 
  // ? tab 2
  // Todo Applen to List
  selorder(o: outbound_ls) {
    let isOrder = this.lsorder.filter(x => x.ouorder == o.ouorder);
    if (isOrder.length == 0) {
      this.lsorder.push(o);// add
      this.getinfo(o, true);
    } else {
      this.lsorder = this.lsorder.filter(x => x.ouorder != o.ouorder); // remove
      this.proc.orders = this.proc.orders.filter(x => x.ouorder != o.ouorder);
    }
  }

  clearSelorder() {
    this.lsorder = [];
    this.slclines = [];
    this.proc = new prepset();
  }


  getinfo(o: outbound_ls, e: boolean = false) {
    this.sv.get(o).subscribe(
      (res) => {
        this.slcorder = res;
        this.slclines = res.lines;
        if (e) {
          this.slclines.forEach(x => {
            this.proc.orders.push(this.dsctoprocln(this.slcorder.thcode, x));
          });
        }
      },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  dsctoprocln(thcode: string, s: outbouln_md) {
    console.log(s);
    var t = new prepsln();
    t.orgcode = s.orgcode;
    t.site = s.site;
    t.depot = s.depot;
    t.spcarea = "ST";
    t.routeno = "";
    t.thcode = thcode;
    t.ouorder = s.ouorder;
    t.ouln = s.ouln;
    t.barcode = s.barcode;
    t.article = s.article;
    t.pv = s.pv;
    t.lv = s.lv;
    t.unitprep = s.unitops;
    t.qtyskuorder = s.qtysku;
    t.qtypuorder = s.qtyreqpu;
    t.qtyweightorder = s.qtyweight;
    t.qtyvolumeorder = 0;
    t.qtyskuops = 0;
    t.qtypuops = 0;
    t.qtyweightops = 0;
    t.qtyvolumeops = 0;
    t.batchno = s.batchno;
    t.lotno = s.lotno;
    t.datemfg = s.datemfg;
    t.dateexp = s.dateexp;
    t.serialno = s.serialno;
    t.description = s.articledsc;
    return t;
  }
  setedit(o: outbouln_md) { this.rqedit = 1; this.slcline = o; }
  flagremarks() { this.chnremark = (this.chnremark == 0) ? 1 : 0; }
  /* Row limit changing */
  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); }

  changereqdate() {
    this.ngPopups.confirm('Do you confirm change request delivery date')
      .subscribe(res => {
        if (res) {
          // this.crstock.tflow = (this.crstate == true) ? "IO" : "XX";
          this.sv.changeRequest(this.slcorder).subscribe((res) => {
              this.toastr.success("<span class='fn-1e15'>Change request delivery date success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  setpriority() {
    this.ngPopups.confirm('Do you confirm to set priority of an order  ?')
      .subscribe(res => {
        if (res) {
          this.sv.setpriority(this.slcorder).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-07e'>modify stock line success</span>", null, { enableHtml: true });
            },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  setremarks() {
    this.ngPopups.confirm('Do you confirm to set remarks of an order  ?')
      .subscribe(res => {
        if (res) {
          this.sv.setremarks(this.slcorder).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-07e'>set remarks success</span>", null, { enableHtml: true }); this.chnremark = 0;
            },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); this.chnremark = 0; },
            () => { }
          );
        }
      });
  }

  setlineorder() {
    this.ngPopups.confirm('Do you confirm to set info on order line ?')
      .subscribe(res => {
        if (res) {
          this.sv.setlineorder(this.slcline).subscribe(
            (res) => {
              this.toastr.success("<span class='fn-07e'>set line info success</span>", null, { enableHtml: true });
              this.rqedit = 0;
            },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  ulcorder() { this.rqedit = 0; }
  blockovqty() {
    if (this.slcline.qtyreqpu > this.slcline.qtypndpu) {
      this.slcline.qtyreqpu = this.slcline.qtypndpu;
      this.toastr.warning("<span class='fn-07e'>Quantity is over order</span>", null, { enableHtml: true });
    } else if (this.slcline.qtyreqpu < 0) {
      this.slcline.qtyreqpu = this.slcline.qtypndpu;
      this.toastr.warning("<span class='fn-07e'>Quantity cannot lass than zero</span>", null, { enableHtml: true });
    }
  }

  preprocessorder() { 
    let proc_ls = Object.assign({},this.proc);
    this.selecProcess.emit(proc_ls); 
  }

  ngDecOrdertype(o: string) { try { return this.lssubtype.find(e => e.value == o).desc; } catch (exc) { return o; } }
  ngDecUnitstock(o: string) { return this.mv.ngDecUnitstock(o); }
  ngDecState(o: string) { return this.mv.ngDecState(o); }
  ngOnDestroy(): void {
    this.selecProcess.unsubscribe();
    this.lsstate = null; delete this.lsstate;
    this.lsorder = null; delete this.lsorder;
    this.slcorder = null; delete this.slcorder;
    this.slclines = null; delete this.slcline;
    this.slcline = null; delete this.slcline;
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
    this.rqedit = null; delete this.rqedit;
    this.chnremark = null; delete this.chnremark;
    this.proc = null; delete this.proc;
    this.chnrqdate = null; delete this.chnrqdate;
    this.lssubtype = null; delete this.lssubtype;

  }

}