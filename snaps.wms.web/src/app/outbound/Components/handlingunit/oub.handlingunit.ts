import { ThrowStmt } from '@angular/compiler';
import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { task_pm } from '../../../task/Models/task.movement.model';
import {
  route_hu,
  route_ls,
  route_md,
  route_pm,
} from '../../Models/oub.route.model';
import {
  handerlingunit,
  handerlingunit_gen,
  handerlingunit_item,
} from '../../Models/oub.handlingunit.model';
import { ourouteService } from '../../Services/oub.route.service';
import { ouhanderlingunitService } from '../../Services/oub.handerlingunit.service';
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

declare var $: any;
@Component({
  selector: 'appoub-handlingunit',
  templateUrl: 'oub.handlingunit.html',
  styles: [
    '.dghus { height:calc(100vh - 235px) !important; }',
    '.dglines { height:calc(100vh - 560px) !important;}',
  ],
  providers: [
    NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
  ],
})
export class oubhandlingunitComponent
  implements OnInit, OnDestroy, AfterViewInit
{
  public lsstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  //public lsmaster: lov[] = new Array();
  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;

  public lshu: handerlingunit[] = new Array();
  public lshandling: handerlingunit[] = new Array();
  public lsemptyhu: handerlingunit[] = new Array();

  public lsitem: handerlingunit_item[] = new Array();
  public pm: handerlingunit = new handerlingunit();
  public pmmaster: handerlingunit = new handerlingunit();

  public crhu: handerlingunit = new handerlingunit();
  public crgen: handerlingunit_gen = new handerlingunit_gen();
  public slcgentype: handerlingunit = new handerlingunit();
  public slhandling: handerlingunit = new handerlingunit();
  public slctype: lov = new lov();
  public slcstate: lov = new lov();

  //Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;

  //Sorting
  public lssort: string = 'spcarea';
  public lsreverse: boolean = false; // for sorting

  //PageNavigate
  page = 4;
  pageSize = 100;
  slrowlmt: lov;
  lsrowlmt: lov[] = new Array();

  /* Tab */
  crtab: Number = 1;

  toastRef: any;

  constructor(
    private sv: ouhanderlingunitService,
    private av: authService,
    private mv: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService
  ) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void {}

  ngAfterViewInit() {
    this.setupJS();
    /*setTimeout(this.toggle, 1000);*/ this.getmaster();
  }
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece',
    });
  }
  getIcon(o: string) {
    return '';
  }
  toggle() {
    $('.snapsmenu').click();
  }
  decstate(o: string) {
    return this.lsstate.find((x) => x.value == o).desc;
  }
  decstateicn(o: string) {
    return this.lsstate.find((x) => x.value == o).icon;
  }
  dectype(o: string) {
    return this.lstype.find((x) => x.value == o).desc;
  }

  getmaster() {
    this.pmmaster.priority = 0;
    this.pmmaster.hutype = 'MS';
    this.sv.find(this.pmmaster).subscribe(
      (res) => {
        this.lshandling = res;
        this.lsemptyhu = res.filter((x) => x.huno == 'XE');
        this.slcgentype = this.lsemptyhu[0];
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-1e15'>" +
            (err.error == undefined ? err.message : err.error.message) +
            '</span>',
          null,
          { enableHtml: true }
        );
      },
      () => {}
    );
    this.mv
      .getlov('HANDERLING', 'FLOW')
      .pipe()
      .subscribe(
        (res) => {
          this.lsstate = res;
        },
        (err) => {
          this.toastr.error(
            "<span class='fn-1e15'>" +
              (err.error == undefined ? err.message : err.error.message) +
              '</span>',
            null,
            { enableHtml: true }
          );
        },
        () => {}
      );

    let fixlov: lov[] = [
      {
        desc: 'XD Empty',
        icon: '',
        valopnfirst: 'XD',
        valopnfour: '',
        valopnsecond: '',
        valopnthird: '',
        value: 'XE',
      },
    ];

    this.lstype = fixlov;
    this.slctype = fixlov[0];

    // this.mv
    //   .getlov('HANDERLING', 'TYPE')
    //   .pipe()
    //   .subscribe(
    //     (res) => {
    //       this.lstype = res;
    //       this.lstype = this.lstype.filter((x) => x.value != 'XD');
    //     },
    //     (err) => {
    //       this.toastr.error(
    //         "<span class='fn-1e15'>" +
    //           (err.error == undefined ? err.message : err.error.message) +
    //           '</span>',
    //         null,
    //         { enableHtml: true }
    //       );
    //     },
    //     () => {}
    //   );
    this.mv
      .getlov('DATAGRID', 'ROWLIMIT')
      .pipe()
      .subscribe(
        (res) => {
          this.lsrowlmt = res.sort(
            (a, b) => parseInt(a.value) - parseInt(b.value)
          );
          this.slrowlmt = this.lsrowlmt.find(
            (x) => x.value == this.pageSize.toString()
          );
        },
        (err) => {
          this.toastr.error(
            "<span class='fn-07e'>" +
              (err.error == undefined ? err.message : err.error.message) +
              '</span>',
            null,
            { enableHtml: true }
          );
        },
        () => {}
      );
  }

  fnd() {
    this.pm.hutype = this.slhandling == null ? '' : this.slhandling.huno;
    this.sv.find(this.pm).subscribe(
      (res) => {
        this.lshu = res.filter((x) => x.hutype != 'MS');
        if (this.lshu.length > 0) {
          this.selhu(this.lshu[0]);
        }
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-1e15'>" +
            (err.error == undefined ? err.message : err.error.message) +
            '</span>',
          null,
          { enableHtml: true }
        );
      },
      () => {}
    );
  }
  changerowlmt() {
    this.pageSize = parseInt(this.slrowlmt.value);
  } /* Row limit */

  seltype() {
    this.crgen.mxsku = this.lsemptyhu.find(
      (x) => x.huno == this.slcgentype.huno
    ).mxsku;
    this.crgen.mxweight = this.lsemptyhu.find(
      (x) => x.huno == this.slcgentype.huno
    ).mxweight;
  }

  selhu(o: handerlingunit) {
    this.crhu = o;
    this.sv.lines(o).subscribe(
      (res) => {
        this.lsitem = res;
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-1e15'>" +
            (err.error == undefined ? err.message : err.error.message) +
            '</span>',
          null,
          { enableHtml: true }
        );
      },
      () => {}
    );
  }

  setpriority() {
    this.crhu.priority = this.crhu.priority > 0 ? 30 : 0;
  }
  confirm() {
    this.ngPopups
      .confirm('Do you confirm to generate handling unit  ?')
      .subscribe((res) => {
        if (res) {
          this.crgen.hutype = this.slctype.value;
          this.crgen.spcarea = this.slcgentype.spcarea;
          this.crgen.crcapacity = this.crhu.crcapacity;
          this.crgen.crsku = 0;
          this.crgen.crvolume = 0;
          this.crgen.crweight = 0;
          this.crgen.huno = this.slcgentype.huno;
          this.crgen.loccode = this.crgen.thcode;
          this.crgen.mxsku = this.crhu.mxsku;
          this.crgen.mxweight = this.crhu.mxweight;
          this.crgen.priority = 0;
          this.crgen.tflow = '';
          this.crgen.routeno = this.crgen.thcode;
          this.crgen.spcarea = this.slctype.valopnfirst;
          this.sv.generate(this.crgen).subscribe(
            (res) => {
              this.toastr.success(
                "<span class='fn-1e15'>generate huno success</span>",
                null,
                { enableHtml: true }
              );
              this.fnd();
            },
            (err) => {
              this.toastr.error(
                "<span class='fn-1e15'>" +
                  (err.error == undefined ? err.message : err.error.message) +
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

  getlabel(o: string) {
    this.toastRef = this.toastr.warning(
      " &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",
      null,
      {
        disableTimeOut: true,
        tapToDismiss: false,
        //toastClass: "toast border-red",
        closeButton: false,
        positionClass: 'toast-bottom-right',
        enableHtml: true,
      }
    );

    this.sv
      .gethuempty_label(this.crhu.orgcode, this.crhu.site, this.crhu.depot, o)
      .subscribe((response) => {
        let blob: any = new Blob([response], {
          type: 'text/json; charset=utf-8',
        });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', 'bgcwms_huempty_' + o + '.pdf');
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId);
      }),
      (error) => {
        this.toastr.clear(this.toastRef.ToastId);
      };
  }

  ngDecColor(o: number) {
    return this.mv.ngDecColor(o);
  }
  ngDecState(o: string) {
    if (o == 'PE') {
      return 'Prep.end';
    } else {
      return this.mv.ngDecState(o);
    }
  }
  ngDecUnitstock(o: string) {
    return this.mv.ngDecUnitstock(o);
  }
  ngOnDestroy(): void {
    this.lsstate = null;
    delete this.lsstate;
    this.lstype = null;
    delete this.lstype;
    this.instocksum = null;
    delete this.instocksum;
    this.rqedit = null;
    delete this.rqedit;
    this.crstate = null;
    delete this.crstate;
    this.lshu = null;
    delete this.lshu;
    this.lshandling = null;
    delete this.lshandling;
    this.lsitem = null;
    delete this.lsitem;
    this.pm = null;
    delete this.pm;
    this.pmmaster = null;
    delete this.pmmaster;
    this.crhu = null;
    delete this.crhu;
    this.crgen = null;
    delete this.crgen;
    this.slcgentype = null;
    delete this.slcgentype;
    this.slctype = null;
    delete this.slctype;
    this.dateformat = null;
    delete this.dateformat;
    this.dateformatlong = null;
    delete this.dateformatlong;
    this.datereplan = null;
    delete this.datereplan;
    this.lssort = null;
    delete this.lssort;
    this.lsreverse = null;
    delete this.lsreverse;
    this.page = null;
    delete this.page;
    this.pageSize = null;
    delete this.pageSize;
    this.slrowlmt = null;
    delete this.slrowlmt;
    this.lsrowlmt = null;
    delete this.lsrowlmt;
    this.crtab = null;
    delete this.crtab;
  }
}
