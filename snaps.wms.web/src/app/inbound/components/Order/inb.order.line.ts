import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { pam_inbound, pam_set } from 'src/app/admn/models/adm.parameter.model';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { HighlightSpanKind } from 'typescript';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { shareService } from '../../../share.service';
import { lov } from '../../../helpers/lov';
import { inbound_ls, inbound_md, inbound_pm } from '../../models/mdl-inbound';
import { inboundService } from '../../services/app-inbound.service';
import { ThumbXDirective } from 'ngx-scrollbar/lib/scrollbar/thumb/thumb.directive';

@Component({
  selector: 'appinb-orderline',
  templateUrl: 'inb.order.line.html',
  styles: ['.dgorder { height:calc(100vh - 645px) !important; } ', '.dglines { height:335px !important; } .dginfo { height:365px !important; }'],
  providers: [
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]
})
export class inborderlineComponent implements OnInit {
  @Input() ilsordertype: lov[];
  @Input() ilsorderflow: lov[];
  @Output() selln = new EventEmitter<inbound_md>();
  // @Input() iconstate: string;
  ison: boolean = false;
  public pm: inbound_pm = new inbound_pm();
  public crorder: inbound_md = new inbound_md();
  public lsorder: inbound_ls[] = new Array();

  public slcstaging: lov = new lov();
  public slcmsdstaging: lov = new lov();
  public msdstate: lov[] = new Array();
  public msdstaging: lov[] = new Array();
  public crstate: Boolean = false;

  //State
  public slcstate: lov;
  public slcordertype: lov;


  //Sorting 
  public lssort: string = "spcarea";
  public lsreverse: boolean = false; // for sorting

  //PageNavigate
  page = 4;
  pageSize = 200;


  // Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;

  //Master Data
  lsrowlmt: lov[] = new Array(); slrowlmt: lov;
  lsordertype: lov[] = new Array(); slordertype: lov;
  lsspcarea: lov[] = new Array(); slspcarea: lov;
  lspriority: lov[] = new Array(); slcpriority: lov;
  lsstaging: lov[] = new Array(); slstaging: lov;
  lsstate: lov[] = new Array(); slstate: lov;
  lsreqmsm: lov[] = new Array(); slrqmsm: lov;
  lsstatem: lov[] = new Array();

  //Tab
  crtab: number = 1;

  //Summary 
  smitem: number = 0;
  smpu: number = 0;
  smsku: number = 0;
  smpurec: number = 0;
  smskurec: number = 0;

  //Parameter 
  syspam: pam_inbound = new pam_inbound;
  toastRef: any;

  public ordrowselect: Number;
  // public fncrowselect: Function;
  constructor(
    private av: authService,
    private sv: inboundService,
    private mv: shareService,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdateshort;
    this.pm.orgcode = this.av.crProfile.orgcode;
    this.pm.site = this.av.crRole.site;
    this.pm.depot = this.av.crRole.depot;
    // this.pm.dateplanfrom = new Date();
    this.crorder.lines = new Array();
    this.lsrowlmt = this.mv.getRowlimit();
  }

  ngOnInit() { }
  ngAfterViewInit() { this.getmaster(); this.fndorder(); }

  ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
  ngselcoutput() { 
    this.selln.emit(this.crorder); 
  }
  SortOrder(value: string) { if (this.lssort === value) { this.lsreverse = !this.lsreverse; } this.lssort = value; }

  // selectrow = function(selindex) {this.numrowselect = selindex};
  getmaster() {
    this.lsreqmsm.push({ value: '1', desc: 'Yes', icon: '', valopnfirst: '', valopnsecond: '', valopnthird: '', valopnfour: '' });
    this.lsreqmsm.push({ value: '0', desc: 'No', icon: '', valopnfirst: '', valopnsecond: '', valopnthird: '', valopnfour: '' });

    Promise.all([
      this.mv.lov("ORDER", "SUBTYPE").toPromise(),
      this.mv.lov("INBORDER", "FLOW").toPromise(),
      this.mv.lovms("INBORDER", "FLOW").toPromise(),
      this.mv.lov("LOCATION", "SPCAREA").toPromise(),
      this.sv.getstaging(0).toPromise(),
      this.sv.getParameter().toPromise()
    ])
      .then((res) => {
        this.lsordertype = res[0];
        this.lsstate = res[1];
        this.lsstatem = res[2];
        this.lsspcarea = res[3];
        this.lsstaging = res[4];
        this.syspam = res[5];
        this.lsrowlmt = this.mv.getRowlimit();
      });

    // this.mv.lov("ORDER","SUBTYPE").pipe().subscribe(
    //   (res) => { this.lsordertype = res; }
    // );
    // this.mv.lov("INBORDER","FLOW").pipe().subscribe(
    //   (res) => { this.lsstate = res; }
    // );
    // this.mv.lovms("INBORDER","FLOW").pipe().subscribe(
    //   (res) => { this.lsstatem = res; } 
    // ); 
    // this.mv.lov("LOCATION","SPCAREA").pipe().subscribe( (res) => { this.lsspcarea = res; } );      
    // this.sv.getstaging(0).subscribe((res) =>  { this.lsstaging = res; } ); 
    // this.sv.getParameter().subscribe((res) => { this.syspam = res; } ); 

  }

  dscicon(o: string) {
    try { return this.lsstate.find(x => x.value == o).icon; } catch (exp) { return o; }
  }

  dscordtype(o: string) {
    switch (o) {
      case 'ST': return 'Stocking'
      case 'XD': return 'Crossdocking'
      case 'FW': return 'Forwarding'
      default:
        return o;
    }
  }

  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

  fndorder() {
    console.log(this.pm.dateplanfrom);
    this.pm.tflow = (this.slcstate != null) ? this.slcstate.value : "";
    this.pm.ordertype = (this.slcordertype != null) ? this.slcordertype.value : "";
    this.pm.ismeasure = (this.slrqmsm != null) ? this.slrqmsm.value : "";
    this.pm.spcarea = (this.slspcarea != null) ? this.slspcarea.value : "";
    this.pm.inpriority = (this.slcpriority != null) ? this.slcpriority.value : "";
    this.pm.dockno = (this.slcstaging != null) ? this.slcstaging.value : "";

    this.sv.find(this.pm).pipe().subscribe(res => {
      this.lsorder = res;
      if (this.lsorder.length == 0) {
        this.toastr.info("<span class='fn-07e'>Data not found with your condition</span>", null, { enableHtml: true });
      } else {
        this.selorder(this.lsorder[0], 0);
      }
    },
      (err) => { this.toastr.error(err.error.message); this.ison = false; },
      () => { }
    );
  }

  selorder(o: inbound_ls, ix: number) {
    this.ordrowselect = ix;
    this.sv.get(o).subscribe(
      (res) => {
        this.crorder = res;
        this.crorder.huestimate = this.crorder.lines.reduce((obl, val) => obl += val.huestimate, 0);
        this.smitem = this.crorder.lines.length;
        this.smpu = this.crorder.lines.reduce((obj, val) => obj += val.qtypu, 0);
        this.smsku = this.crorder.lines.reduce((obj, val) => obj += val.qtysku, 0);
        this.smpurec = this.crorder.lines.reduce((obj, val) => obj += val.qtypurec, 0);
        this.smskurec = this.crorder.lines.reduce((obj, val) => obj += val.qtyskurec, 0);
        this.datereplan = this.crorder.dateplan;
        if (['IO', 'SA'].includes(this.crorder.tflow)) {
          this.getstaging();
        }
        // default staging in dropdow
        if (this.crorder.dockrec != null) {
          this.slcmsdstaging = this.lsstaging.find(x => x.value == this.crorder.dockrec);
        }
      },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  getdocstatement(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>", null, {
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass: 'toast-bottom-right', enableHtml: true
    });

    this.sv.getdocstatement(this.pm.orgcode, this.pm.site, this.pm.depot, o).subscribe(response => {
      let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_receipt_statement_" + o + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId);
    },
      err => {
        this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true });
        this.toastr.clear(this.toastRef.ToastId);
      });

  }

  getstaging() {
    this.sv.getstaging((this.syspam.allowcontrolcapacity) ? this.crorder.huestimate : 0).subscribe(
      (res) => {
        this.msdstaging = res;

      },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  setpriority() {
    if (this.syspam.allowchangepriority) {
      this.ngPopups.confirm('Do you confirm setup priority of order ' + this.crorder.inorder + ' ?')
        .subscribe(res => {
          if (res) {
            this.crorder.inpriority = (this.crorder.inpriority == 0) ? 30 : 0;
            this.sv.setpriority(this.crorder.inorder, this.crorder.inpriority).subscribe(
              (res) => {
                this.lsorder.find(x => x.inorder == this.crorder.inorder).inpriority = this.crorder.inpriority;
                this.toastr.success("<span class='fn-07e'>Setup priority success </span>", null, { enableHtml: true }); 
                this.crorder.dockrec = this.slcmsdstaging.value;
              },
              (err) => {
                this.crorder.inpriority = (this.crorder.inpriority == 0) ? 30 : 0;
                this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true });
              },
              () => { }
            );
          }
        });
    }
  }
  setstaging() {
    this.ngPopups.confirm('Do you confirm setup staging for receipt ?')
      .subscribe(res => {
        if (res) {

          this.sv.setstaging(this.crorder.inorder, this.slcmsdstaging.value).subscribe(
            (res) => {
              if (this.crorder.tflow == "IO") { this.crorder.tflow = "SA"; }
              this.toastr.success("<span class='fn-07e'>Setup Staging success </span>", null, { enableHtml: true }); 
              this.crorder.dockrec = this.slcmsdstaging.value;
            },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }
  setremark() {
    this.ngPopups.confirm('Do you confirm remarks on Inbound order ?')
      .subscribe(res => {
        if (res) {
          this.sv.setremarks(this.crorder.inorder, this.crorder.remarkrec).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Change remarks success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }
  setreplan() {
    this.ngPopups.confirm('Do you confirm to change plan delivery date on Inbound order ?')
      .subscribe(res => {
        if (res) {
          this.sv.setreplan(this.crorder.inorder, this.datereplan).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Change plan delivery success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  setunloadstart() {
    this.ngPopups.confirm('Do you confirm start unload product from supplier ?')
      .subscribe(res => {
        if (res) {
          this.sv.setunloadstart(this.crorder.inorder).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Set to start unload success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  setunloadend() {
    this.ngPopups.confirm('Do you confirm end unload product from supplier ?')
      .subscribe(res => {
        if (res) {
          this.sv.setunloadend(this.crorder.inorder).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Set to end unload success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  setfinish() {
    this.ngPopups.confirm('Do you confirm finish receipt product from supplier ?')
      .subscribe(res => {
        if (res) {
          this.sv.setfinish(this.crorder.inorder).subscribe(
            (res) => { this.toastr.success("<span class='fn-07e'>Set to finish success </span>", null, { enableHtml: true }); },
            (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
            () => { }
          );
        }
      });
  }

  //dsczone(o:string){  try{ return this.lszone.find(x=>x.value == o).desc;  } catch (error) { return ""; } }
  validate() {
    //this.crorder.lscodefull = this.crorder.orgcode + "-" + this.crorder.site+ "-" +this.crorder.depot+ "-" +this.crorder.lsorder+ "-" +this.crorder.lscode;
    //   this.crorder.tflow = (this.crstate == true) ? "IO" : "XX";

    //   this.ngPopups.confirm('Do you accept change of order?')
    //   .subscribe(res => { 
    //       if (res) {
    //         this.ison = true;
    //         this.sv.upsertlocup(this.crorder).pipe().subscribe(            
    //             (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndorder(); },
    //             (err) => { this.toastr.error(err.error.message); this.ison = false; },
    //             () => { }
    //         );

    //       } 
    //   });
  }


  ngDecOrder(o: string) { try { return this.lsordertype.find(e => e.value == o).desc; } catch (exp) { return o; } }
  ndDecstockunti(o: string) { return this.mv.ngDecUnitstock(o); }

}
