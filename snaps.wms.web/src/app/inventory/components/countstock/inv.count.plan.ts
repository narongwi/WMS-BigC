import { ThrowStmt } from '@angular/compiler';
import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  Output,
  EventEmitter,
  Input,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import {
  stock_md,
  stock_ls,
  stock_pm,
  stock_info,
} from '../../models/inv.stock.model';
import { inventoryService } from '../../services/inv.stock.service';
import { correction_md } from '../../models/inv.correction.mode';
import {
  NgbDateAdapter,
  NgbDateParserFormatter,
  NgbPaginationConfig,
} from '@ng-bootstrap/ng-bootstrap';
import {
  CustomAdapter,
  CustomDateParserFormatter,
} from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import {
  countline_md,
  countplan_md,
  counttask_md,
} from '../../models/inv.count.model';
import { countService } from '../../services/Inv.count.service';
import { collapseTextChangeRangesAcrossMultipleVersions } from 'typescript';

declare var $: any;
@Component({
  selector: 'appinv-counplan',
  templateUrl: 'inv.count.plan.html',
  styles: [
    '.dgtask { height:235px !important; } ',
    '.dgplan { height:277px !important; } ',
    '.dglines { height:calc(100vh - 485px) !important; }',
    '.px-50{width:50px;margin-left:5px;margin-right:5px;}',
    '.px-60{width:60px;margin-left:5px;margin-right:5px;}',
    '.px-70{width:70px;margin-left:5px;margin-right:5px;}',
    '.px-80{width:80px;margin-left:5px;margin-right:5px;}',
    '.px-90{width:90px;margin-left:5px;margin-right:5px;}',
    '.px-100{width:100px;margin-left:5px;margin-right:5px;}',
    '.px-110{width:110px;margin-left:5px;margin-right:5px;}',
    '.px-120{width:120px;margin-left:5px;margin-right:5px;}',
    '.px-130{width:130px;margin-left:5px;margin-right:5px;}',
    '.row-p-0{display:flex;flex-wrap: wrap;padding-right:10px;padding-left:2px;margin:0;line-height: 2.2}',
    '.btn-xs.active{background-color: #a7c95758;font-weight: bold;color: #386641;border: 1px solid #9ab754a1}',
  ],
  providers: [
    NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
  ],
})
export class invcounplanComponent implements OnInit, OnDestroy {
  @Output() selout = new EventEmitter<counttask_md>();
  public crrtask: counttask_md = new counttask_md();
  public crstate: boolean = false;
  public lspctval: lov[] = new Array();
  public slcpct: lov;
  public lszone: lov[] = new Array();
  public lsstate: lov[] = new Array();
  public lsplan: countplan_md[] = new Array();
  public crplan: countplan_md = new countplan_md();
  public lsline: countline_md[] = new Array();
  public slczone: lov = new lov();
  // public dateformat: string;
  // public dateformatlong: string;
  @Input() dateformat: string;
  @Input() dateformatlong: string;
  public swroaming: boolean = false;
  public swblock: boolean = false;
  public swdatemfg: boolean = false;
  public swdateexp: boolean = false;
  public swbatchno: boolean = false;
  public swserialno: boolean = false;
  public swallowscanhu: boolean = false;
  public swalloddeven: boolean = false;
  public rowselected: number;
  public planprogress: number = 0;
  public planTotal: number = 0;
  public planvalidated: number = 0;

  public isbulkplan: boolean = false;
  constructor(
    private sv: countService,
    private av: authService,
    private mv: adminService,
    private ss: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService
  ) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void {
    this.ngSetup();
  }
  ngSetup() {
    this.mv
      .getlov('COUNT', 'VALIDATE')
      .pipe()
      .subscribe(
        (res) => {
          this.lspctval = res.sort(
            (a, b) => Number(a.valopnfirst) - Number(b.valopnfirst)
          );
        },
        (err) => {},
        () => {}
      );
    this.mv
      .getlov('COUNT', 'RESULT')
      .pipe()
      .subscribe(
        (res) => {
          this.lsstate = res;
        },
        (err) => {},
        () => {}
      );
    this.ss
      .storagezone()
      .pipe()
      .subscribe(
        (res) => {
          this.lszone = res;
        },
        (err) => {},
        () => {}
      );
  }
  ngNew() {
    this.crplan = new countplan_md();
    this.lsline = new Array();
    this.crplan.tflow = 'NW';
    this.crstate = true;
    this.crplan.countcode = this.crrtask.countcode;
    this.toastr.info("<span class='fn-07e'> Setup new plan  </span>", null, {
      enableHtml: true,
    });
  }
  ngFind() {
    this.sv.listPlan(this.crrtask).subscribe(
      (res) => {
        this.lsplan = res;
        /* progress calulate */ this.planProgress();
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-07e'> Get Count plan error , " + err + '</span>',
          null,
          { enableHtml: true }
        );
      }
    );
  }
  zoneChange() {
    if (!this.isEmpty(this.slczone.valopnsecond)) {
      this.isbulkplan = this.slczone.valopnsecond == 'BL' ? true : false;
      console.log('Is Bulk Plan ' + this.isbulkplan);
    }
  }
  planProgress() {
    if (this.lsplan) {
      let validated: number = 0;
      let total: number = 0;
      this.lsplan.forEach((e) => {
        console.log(e.tflow);
        if (e.tflow == 'ED') {
          validated++;
        }
        if (e.tflow != 'XX') {
          total++;
        }
      });
      let progress = Number((validated * 100) / total);
      this.planTotal = total;
      this.planvalidated = validated;
      this.planprogress = progress > 100 ? 100 : progress;

      // console.log("this.planTotal:" + this.planTotal);
      // console.log("this.planvalidated:" + this.planvalidated);
      // console.log("this.planprogress:" + this.planprogress);
    }
  }

  ngSelc(o: counttask_md) {
    // this.pm.countcode = o.countcode;
    this.crrtask = o;
    this.lsline = [];
    this.ngFind();
  }
  ngUpsert() {
    if (this.isEmpty(this.slczone.value)) {
      this.toastr.warning(
        "<span class='fn-07e'>please select zone </span>",
        null,
        { enableHtml: true }
      );
    } else if (
      !this.isbulkplan &&
      (this.isEmpty(this.crplan.saisle) || this.isEmpty(this.crplan.eaisle))
    ) {
      this.toastr.warning(
        "<span class='fn-07e'>Aisle is required</span>",
        null,
        { enableHtml: true }
      );
    } else if (
      !this.isbulkplan &&
      (this.isEmpty(this.crplan.ebay) || this.isEmpty(this.crplan.ebay))
    ) {
      this.toastr.warning("<span class='fn-07e'>Bay is required</span>", null, {
        enableHtml: true,
      });
    } else if (
      !this.isbulkplan &&
      (this.isEmpty(this.crplan.elevel) || this.isEmpty(this.crplan.elevel))
    ) {
      this.toastr.warning(
        "<span class='fn-07e'>Level is required</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm modify plan count ?')
        .subscribe((res) => {
          if (res) {
            if (this.crplan.tflow == 'NW') {
              this.crplan.szone = this.slczone.value;
              this.crplan.isblock = this.swblock == true ? 1 : 0;
              this.crplan.isdatemfg = this.swdatemfg == true ? 1 : 0;
              this.crplan.isdatemfg = this.swdateexp == true ? 1 : 0;
              this.crplan.isbatchno = this.swbatchno == true ? 1 : 0;
              this.crplan.isserialno = this.swserialno == true ? 1 : 0;
              this.crplan.allowscanhu = this.swallowscanhu == true ? 1 : 0;
              this.crplan.isoddeven = this.swalloddeven == true ? 1 : 0;
            }
            this.crplan.tflow =
              this.crplan.tflow != 'NW'
                ? this.crstate == true
                  ? 'IO'
                  : 'XX'
                : 'NW';
            this.sv
              .upsertPlan(this.crplan)
              .pipe()
              .subscribe(
                (res) => {
                  this.toastr.success(
                    "<span class='fn-07e'> Create new Plan success </span>",
                    null,
                    { enableHtml: true }
                  );
                  // this.ngFind();
                  // this.ngLine();

                  // re-select or refresh task
                  this.selout.emit(this.crrtask);
                },
                (err: any) => {
                  this.toastr.error(
                    "<span class='fn-07e'> Generate Count plan error , " +
                      err.message +
                      '</span>',
                    null,
                    { enableHtml: true }
                  );
                }
              );
          }
        });
    }
  }

  isEmpty(value) {
    return (
      // null or undefined
      value == null ||
      value == undefined ||
      // has length and it's zero
      (value.hasOwnProperty('length') && value.length === 0) ||
      // is an Object and has no keys
      (value.constructor === Object && Object.keys(value).length === 0)
    );
  }

  ngValidate() {
    if (this.crrtask.counttype == 'CT' && this.slcpct == undefined) {
      this.toastr.error(
        "<span class='fn-07e'>Validate percentage is required !</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm validate plan count ?')
        .subscribe((res) => {
          if (res) {
            // stock take percentage
            if (this.crrtask.counttype == 'CT') {
              this.crplan.pctvld = Number.parseInt(this.slcpct.value);
            }
            this.sv
              .validatePlan(this.crplan)
              .pipe()
              .subscribe(
                (res) => {
                  this.toastr.success(
                    "<span class='fn-07e'> Validate plan count success </span>",
                    null,
                    { enableHtml: true }
                  );
                  // this.ngFind();
                  this.ngLine();
                  this.crplan.tflow = 'ED';

                  this.selout.emit(this.crrtask);
                },
                (err) => {
                  this.toastr.error(
                    "<span class='fn-07e'> Validate plan error , " +
                      err.message +
                      '</span>',
                    null,
                    { enableHtml: true }
                  );
                },
                () => {}
              );
          }
        });
    }
  }

  cancel() {
    // this.ngLine();
    if (this.lsplan.length > 0) {
      this.ngSelect(this.lsplan[0], this.rowselected);
    }
  }
  /// cancel plan
  ngCancel() {
    this.ngPopups
      .confirm('Do you confirm cancel plan count ?')
      .subscribe((res) => {
        if (res) {
          this.sv
            .removePlan(this.crplan)
            .pipe()
            .subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-07e'> cancel plan count success </span>",
                  null,
                  { enableHtml: true }
                );
                this.crplan.tflow = 'XX';
                // this.ngFind();
                // this.ngSelect(this.crplan);

                // re-select or refresh task
                this.selout.emit(this.crrtask);
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-07e'> Cancel plan error , " +
                    err +
                    '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => {}
            );
        }
      });
  }
  // select plan item
  ngSelect(o: countplan_md, ix: number) {
    this.lsline = [];
    this.rowselected = ix;
    this.crplan = o;
    this.ngLine();
  }
  // list line
  ngLine() {
    this.sv
      .listLineAsync(this.crplan)
      .pipe()
      .subscribe(
        (res) => {
          this.lsline = res;
        },
        (err) => {
          this.toastr.error(
            "<span class='fn-07e'> List plan error , " + err + '</span>',
            null,
            { enableHtml: true }
          );
        }
      );
  }

  decIcon(o: string) {
    if (o == 'ED') {
      return 'fas fa-check-circle text-success';
    } else if (o == 'WQ') {
      return 'fas fa-exclamation text-danger';
    } else {
      return this.ss.ngDecIcon(o);
    }
  }
  decTflow(o: string) {
    if (o == 'XX') {
      return 'Cancel';
    } else if (o == 'ED') {
      return 'Validated';
    } else if (o == 'WQ') {
      return 'Recount';
    } else {
      return this.ss.ngDecStr(o);
    }
  }
  ngDecIcon(o: string) {
    return o == 'ED'
      ? 'fa-check-circle fas text-primary'
      : this.ss.ngDecIcon(o);
  }
  ngDecStr(o: string) {
    return o == 'XX' ? 'Cancel' : o == 'ED' ? 'Validated' : this.ss.ngDecStr(o);
  }
  ngDeclnIcon(o: string) {
    return this.lsstate.find((e) => e.value == o).icon;
  }
  ngDeclnStr(o: string) {
    return this.lsstate.find((e) => e.value == o).desc;
  }
  ngOnDestroy(): void {
    this.selout.unsubscribe;
    delete this.selout;
    this.crrtask = null;
    delete this.crrtask;
  }
}
