import { ThrowStmt } from '@angular/compiler';
import { Input } from '@angular/core';
import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ThumbXDirective } from 'ngx-scrollbar/lib/scrollbar/thumb/thumb.directive';
import { ToastrService } from 'ngx-toastr';
import { pam_set } from 'src/app/admn/models/adm.parameter.model';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { ouselect } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';

declare var $: any;
@Component({
  selector: 'appoub-processstock-selection',
  templateUrl: 'oub.process.stock.selection.html',
  styles: ['.dgorder {  height:250px !important;', '.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]
})
export class oubprocessstockselectionComponent implements OnInit, OnDestroy {
  @Output() selectOrder = new EventEmitter();
  @Output() refreshdata = new EventEmitter();

  //List of state
  public lsstate: lov[] = new Array(); slcstate: lov;
  //List of order 
  public lsorder: outbound_ls[] = new Array();
  //List of subtype
  public lssubtype: lov[] = new Array();
  public slcordertype: lov;
  //Paramater
  public pm: outbound_pm = new outbound_pm();
  public slcorder: outbound_md; //Object 
  public slclines: outbouln_md[] = new Array();; //lines
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

  lsspcarea: lov[] = new Array(); slcspcarea: lov;
  lspriority: lov[] = new Array(); slcpriority: lov;
  lsstaging: lov[] = new Array(); slcstaging: lov;
  lsreqmsm: lov[] = new Array(); slrqmsm: lov;
  /* Requst Edit */
  rqedit: number = 0;

  public chnremark: number = 0;
  public chnrqdate: number = 0;
  public parameter: pam_set[] = [];

  constructor(private sv: outboundService,
    private pv: ouprepService,
    private av: authService,
    private mv: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;

  }
  ngOnInit(): void {
    this.lsreqmsm.push({ value: '1', desc: 'Yes', icon: '', valopnfirst: '', valopnsecond: '', valopnthird: '', valopnfour: '' });
    this.lsreqmsm.push({ value: '0', desc: 'No', icon: '', valopnfirst: '', valopnsecond: '', valopnthird: '', valopnfour: '' });
    this.lspriority.push({ value: '1', desc: 'Yes', icon: '', valopnfirst: '', valopnsecond: '', valopnthird: '', valopnfour: '' });
    this.lspriority.push({ value: '0', desc: 'No', icon: '', valopnfirst: '', valopnsecond: '', valopnthird: '', valopnfour: '' });
  }
  ngAfterViewInit() {
    // this.lssubtype = this.lssubtype.filter(e=>['OU','INOU'].includes(e.valopnfour));
    // this.mv.getArea();
  }
  decstate(o: string) { return this.lsstate.find(x => x.value == o).desc; }

  /* Order selection */
  selorder(o: outbound_ls) {
    // toggle selection
    let checked = (o.selc == true) ? false : true;
    let selected: ouselect = {
      orgcode: o.orgcode,
      site: o.site,
      depot: o.depot,
      spcarea: o.spcarea,
      ouorder: o.ouorder,
      outype: o.outype,
      ousubtype: o.ousubtype,
      thcode: o.thcode,
      selected: checked ? 1 : 0,
      selectaccn: "",
      selectdate: "",
      selectflow: o.tflow
    }
    // yet checked
    if (checked) {
      // true
      this.sv.selectorder(selected).subscribe(res => {
        console.log(o.ouorder + " selected");
        // pust data to tab 2
        o.selc = checked;
        this.selectorder(o);
      }, err => {
        o.selc = false;
        // uncheck if confirm
        this.ngPopups.confirm(err.message + "!! you want to select this Order ?").subscribe(res => {
          if (res) {
            // unselect 
            this.sv.unselectorder(selected).subscribe(res => {
              o.selc = checked;
              this.selectorder(o);
            }, err => {
              o.selc = false;
              this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true });
            });
          }
        });
      });
    } else {
      // false Un checked
      this.sv.unselectorder(selected).subscribe(res => {
        o.selc = checked;
        this.selectorder(o);
      }, err => {
        o.selc = false;
        this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true });
      });
    }
  }

  // clear selection
  clearSelorder() {
    this.lsorder = [];
    // refresh search order
    this.findorder();

  }

  public selectorder(o: outbound_ls) {
    // return tab 1
    this.selectOrder.emit(o);
  }

  /* Row limit changing */
  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); }

  /* Flag for remark */
  flagremarks() { this.chnremark = (this.chnremark == 0) ? 1 : 0; }

  public ngOn(Inlsrowlimit: lov[]) { this.lsrowlmt = Inlsrowlimit; }

  findorder() {
    this.refreshdata.emit(true);

    this.pm.spcarea = "ST";
    this.pm.ispending = "1";
    this.pm.ousubtype = (this.slcordertype == null) ? null : this.slcordertype.value;
    this.pm.spcarea =  (this.slcspcarea == null) ? null :  this.slcspcarea.value;
    this.pm.oupriority = (this.slcpriority == null) ? 100 : parseInt(this.slcpriority.value);
    this.pm.tflow =  (this.slcstate == null) ? null :   this.slcstate.value;
    this.sv.find(this.pm).subscribe(
      (res) => {
        this.lsorder = res;
        this.lsorder.forEach(x => {
          x.selc = false;
          try {
            x.ousubtypedesc = this.lssubtype.find(e => e.value == x.ousubtype).desc
          } catch (exp) {
            x.ousubtypedesc = x.ousubtype;
          }
        });
        
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
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

  getinfo(o: outbound_ls) {
    this.sv.get(o).subscribe(
      (res) => {
        this.slcorder = res;
        this.slclines = res.lines;
      },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  ngDecOrdertype(o: string) { try { return this.lssubtype.find(e => e.value == o).desc; } catch (exc) { return o; } }
  ngDecUnitstock(o: string) { return this.mv.ngDecUnitstock(o); }
  ngDecState(o: string) { return this.mv.ngDecState(o); }
  ngOnDestroy(): void {
    this.selectOrder.unsubscribe(); delete this.selectOrder;
    this.lsstate = null; delete this.lsstate;
    this.lsorder = null; delete this.lsorder;
    this.lssubtype = null; delete this.lssubtype;
    this.pm = null; delete this.pm;
    this.slcorder = null; delete this.slcorder;
    this.slclines = null; delete this.slclines;
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
    this.chnrqdate = null; delete this.chnrqdate;
    this.refreshdata.unsubscribe(); delete this.refreshdata;
  }

}