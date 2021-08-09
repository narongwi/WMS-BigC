import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { confirmline_md, countcorrection_md, counttask_md } from '../../models/inv.count.model';
import { countService } from '../../services/Inv.count.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
@Component({
  selector: 'appinv-countconfirm',
  templateUrl: './inv.count.confirm.html',
  styles: ['.dgtask { height:calc(100vh - 235px) !important; ', '.dglines { height:calc(100vh - 685px) !important; }'],
})
export class InvcountConfirmComponent implements OnInit {
  @Output() selout = new EventEmitter<counttask_md>();
  // @Output() reftask = new EventEmitter();

  /* List of value */
  lsrowlmt: lov[] = new Array();
  lsunit: lov[] = new Array();
  lsstate: lov[] = new Array();
  lstasktype: lov[] = new Array();

  //PageNavigate
  page = 4;
  pageSize = 200;
  slrowlmt: lov;

  //Date format 
  dateformat: string = "";
  dateformatlong: string = "";

  disabledButton: boolean = false;
  // data object
  public crtask: counttask_md = new counttask_md();
  public cntask: confirmline_md = new confirmline_md();
  public coline: countcorrection_md[] = [];
  public ttin: number = 0;
  public ttout: number = 0;
  public ttart: number = 0;
  public ttqty: number = 0;
  constructor(private sv: countService,
    private av: authService,
    private mv: adminService,
    private ss: shareService,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.lsrowlmt = this.ss.getRowlimit();
    this.lsunit = this.ss.getUnit();
  }
  ngOnInit(): void {
  }
  ngSetup() {
    this.mv.getlov("COUNT", "RESULT").pipe().subscribe(
      (res) => { this.lsstate = res; },
      (err) => { },
      () => { }
    );
  }
  ngSelc(o: counttask_md) {
    // reset model
    this.crtask = new counttask_md();
    this.cntask = new confirmline_md();
    this.coline = new Array();
    this.disabledButton = true;
    this.ttin = 0;
    this.ttout = 0;
    this.ttart = 0;
    this.ttqty = 0;

    // check task type is stock take
    if (o.counttype == 'CT') {
      // assign current task
      this.crtask = o;

      // get confirm line
      this.ngFind();
    }
  }

  ngFind() {
    // call service
    this.sv.listConfirm(this.crtask).pipe().subscribe((res) => {
      // binding data
      this.coline = res;
      // display task information
      Object.assign(this.crtask, this.coline[0]);

      // disable button if no line confirm
      this.disabledButton = this.coline.length == 0 ? true : false;

      // total summary
      this.ttart = this.coline.filter((cn, i, arr) => arr.indexOf(arr.find(t => t.article === cn.article)) === i).length;
      this.ttin = this.coline.reduce((acc, cur) => acc + (cur.corcode == "+" ? cur.corqty : 0), 0);
      this.ttout = this.coline.reduce((acc, cur) => acc + (cur.corcode == "-" ? cur.corqty : 0), 0);
      this.ttqty = this.ttin + this.ttout;
    },
      (err) => { this.toastr.error("<span class='fn-07e'> get line count error , please try again.</span>", null, { enableHtml: true }); console.log(err); },
      () => { }
    );
  }

  ngConfirm() {
    this.ngPopups.confirm('Do you confirm correction count task ?')
      .subscribe(res => {
        if (res) {
          // stock take percentage
          this.sv.countConfirm(this.crtask).pipe().subscribe(
            (res) => {
              this.toastr.success("<span class='fn-07e'>Confirm Task success </span>", null, { enableHtml: true });
              this.ngSelc(this.crtask) 
              // this.reftask.emit(this.crtask);
            },
            (err) => {
              this.toastr.error("<span class='fn-07e'>Confirm Task error , " + err + "</span>", null, { enableHtml: true });
            },
            () => { }
          );
        }
      });

    console.log("Start Confirm");
    console.log(this.crtask);
  }
  desstate(o: string) {
    switch (o) {
      case "ED": return "Closed";
      case "IO": return "Counting";
      default: o;
    }
  }
  destask(o: string) {
    switch (o) {
      case "CC": return "Cycle count";
      case "CT": return "Stock take";
      default: o;
    }
  }

  decIcon(o: string) {
    if (o == "ED") {
      return "fas fa-check-circle text-primary";
    } else {
      return this.ss.ngDecIcon(o);
    }
  }
  decTflow(o: string) {
    if (o == "XX") {
      return "Cancel";
    } else if (o == "ED") {
      return "Validated";
    } else {
      return this.ss.ngDecStr(o);
    }
  }
  ngDecIcon(o: string) { return (o == "ED") ? "fa-check-circle fas text-primary" : this.ss.ngDecIcon(o); }
  ngDecStr(o: string) { return (o == 'XX') ? "Cancel" : (o == 'ED') ? "Validated" : this.ss.ngDecStr(o);; }
  ngDeclnIcon(o: string) { return this.lsstate.find(e => e.value == o).icon; }
  ngDeclnStr(o: string) { return this.lsstate.find(e => e.value == o).desc; }
  ngOnDestroy(): void {
    this.cntask = null; delete this.cntask;
    this.crtask = null; delete this.cntask;
    this.coline = null; delete this.cntask;
    this.selout.unsubscribe; delete this.selout;
    // this.reftask.unsubscribe ;delete this.reftask;
  }
}
