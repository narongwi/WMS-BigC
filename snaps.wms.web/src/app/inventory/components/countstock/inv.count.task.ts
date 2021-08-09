import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { counttask_md } from '../../models/inv.count.model';
import { countService } from '../../services/Inv.count.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'appinv-countask',
  templateUrl: 'inv.count.task.html',
  styles: ['.dgtask { height:calc(100vh - 235px) !important; ', '.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]

})
export class invcountaskComponent implements OnInit, OnDestroy {
  @Output() selout = new EventEmitter<counttask_md>();
  /* List of value */
  lsrowlmt: lov[] = new Array();
  lsunit: lov[] = new Array();
  lsstate: lov[] = new Array();
  lstasktype: lov[] = new Array();

  //PageNavigate
  page = 4;
  pageSize = 200;
  slrowlmt: lov;

  public pm: counttask_md = new counttask_md();

  slctasktype: lov;
  slcflow: boolean = false;

  public crtask: counttask_md = new counttask_md();
  public lstask: counttask_md[] = new Array();
  public crstate: boolean = false;

  //Date format 
  dateformat: string = "";
  dateformatlong: string = "";
  public rowselected:number;
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
    this.lstasktype.push({
      value: "CC", desc: "Cycle count", icon: "", valopnfirst: "", valopnfour: "", valopnsecond: "", valopnthird: ""
    });
    this.lstasktype.push({
      value: "CT", desc: "Stock Take", icon: "", valopnfirst: "", valopnfour: "", valopnsecond: "", valopnthird: ""
    });
  }

  ngOnInit(): void { }

  ngAfterViewInit() { this.setupJS(); setTimeout(this.ngToggle, 1000); this.ngFind(); }
  ngToggle() { $('.snapsmenu').click(); }
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });
  }


  desstate(o: string) {
    switch (o) {
      case "ED": return "Closed";
      case "IO": return "Counting";
      case "XX": return "Cancel";
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

  ngNew() {
    this.crtask = new counttask_md();
    this.crtask.tflow = "NW";
    this.slcflow = true;
  }
  ngFind() {
    this.sv.listTask(this.pm).pipe().subscribe((res) => {
      this.lstask = res;
      this.crtask  = new counttask_md();
      this.rowselected = -1;
    },(err) => { 
        this.lstask = [];
        this.toastr.error("<span class='fn-07e'>error , " + err.message + "</span>", null, { enableHtml: true }); 
    });
  }

  ngSelc(o: counttask_md,ix:number) {
    this.rowselected = ix;
    this.crtask = o;
    this.slcflow = (this.crtask.tflow == "IO") ? true : false;
    this.selout.emit(o);
  }


  ngUpsert() {
    if(this.slctasktype==undefined||this.slctasktype==null){
      this.toastr.error("<span class='fn-07e'>Please Select Task Type </span>", null, { enableHtml: true }); 
    }else if(this.crtask.countname==undefined||this.crtask.countname==null||this.crtask.countname==''){
      this.toastr.error("<span class='fn-07e'>Please Enter Count Name</span>", null, { enableHtml: true }); 
    }else if(this.crtask.datestart==undefined||this.crtask.datestart==null||this.crtask.dateend==''){
      this.toastr.error("<span class='fn-07e'>Please Select Start Date</span>", null, { enableHtml: true }); 
    }else if(this.crtask.dateend==undefined||this.crtask.dateend==null||this.crtask.dateend==''){
      this.toastr.error("<span class='fn-07e'>Please Select End Date</span>", null, { enableHtml: true }); 
    }else {
    this.ngPopups.confirm('Do you confirm modify task ?')
      .subscribe(res => {
        if (res) {
            this.crtask.counttype = this.slctasktype.value;
            this.crtask.tflow = (this.crtask.tflow != "NW") ? (this.slcflow == true) ? "IO" : "XX" : "NW";
            this.sv.upsertTask(this.crtask).pipe().subscribe((res) => {
                this.toastr.success("<span class='fn-07e'> Create new task success </span>", null, { enableHtml: true });
                this.ngFind();
                this.ngSelc(this.crtask,this.rowselected);
              },(err) => { 
                this.lstask = [];
                this.toastr.error("<span class='fn-07e'>error , " + err.message + "</span>", null, { enableHtml: true }); 
            });
          }
        });
      }
  }
  ngClose() {
    this.ngPopups.confirm('Do you confirm close count task ?')
      .subscribe(res => {
        if (res) {
          this.crtask.tflow = "ED";
          this.sv.upsertTask(this.crtask).pipe().subscribe(
            (res) => {
              this.toastr.success("<span class='fn-07e'> Close task success </span>", null, { enableHtml: true });
              this.ngFind();
              //  this.ngSelect( this.crtask);
            },(err=>{
              this.crtask.tflow = "IO";
              this.toastr.error("<span class='fn-07e'>error , " + err.message + "</span>", null, { enableHtml: true }); 
            })
          );
        }
      },(err) => { 
        this.lstask = [];
        this.toastr.error("<span class='fn-07e'>error , " + err.message + "</span>", null, { enableHtml: true }); 
    });
  }
  ngRemove() {
    this.ngPopups.confirm('Do you confirm delete count task ?')
      .subscribe(res => {
        if (res) {
          this.sv.removeTask(this.crtask).pipe().subscribe((res) => {
              this.toastr.success("<span class='fn-07e'> Create new task success </span>", null, { enableHtml: true });
              this.ngFind();
              this.lstask = [];
              // this.ngSelc(this.crtask,this.rowselected);
            },(err) => { 
              this.lstask = [];
              this.toastr.error("<span class='fn-07e'>error , " + err.message + "</span>", null, { enableHtml: true }); 
          });
        }
      });
  }

  ngChangeRowLimit() { this.pageSize = parseInt(this.slrowlmt.value); }
  ngDecIcon(o: string) { return this.ss.ngDecIcon(o); }
  ngDecStr(o: string) { return this.ss.ngDecStr(o);; }
  ngOnDestroy(): void {
    this.lsrowlmt = null;
    this.lsstate = null;
    this.lstask = null;
    this.lstasktype = null;
    this.lsunit = null;
    this.slcflow = null;
    this.slctasktype = null;
    this.pm = null; delete this.pm;
  }
}
