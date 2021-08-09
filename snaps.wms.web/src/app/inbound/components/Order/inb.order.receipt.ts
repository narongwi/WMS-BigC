import { DatePipe } from '@angular/common';
import { ThrowStmt, _ParseAST } from '@angular/compiler';
import { Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ThumbXDirective } from 'ngx-scrollbar/lib/scrollbar/thumb/thumb.directive';
import { ToastrService } from 'ngx-toastr';
import { pam_inbound, pam_set } from 'src/app/admn/models/adm.parameter.model';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import {
  inbouln_md,
  inboulx,
  inbound_ls,
  inbound_md,
  inbound_pm,
} from '../../models/mdl-inbound';
import { inboundService } from '../../services/app-inbound.service';

declare var $: any;
@Component({
  selector: 'appinb-orderreceipt',
  templateUrl: 'inb.order.receipt.html',
  styles: [
    '.dgordline { height:370px !important;',
    '.dgstock { height:calc(100vh - 815px) !important;overflow:auto; }',
  ],
})
export class inborderreceiptComponent implements OnInit {
  @Output() refrln = new EventEmitter<inbound_md>();

  public lslog: lov[] = new Array();
  public opsline: inboulx = new inboulx();
  public opsorder: inbound_md = new inbound_md();

  public crline: inbouln_md = new inbouln_md();
  public crttrcv: number = 0;

  public slcproduct: inbouln_md;

  public slcdate: Date | String;
  public slcacp: String;

  //Date format
  public dateformat: string;
  public dateformatlong: string;

  //List of value
  public lsratio: lov[] = new Array();
  slratio: lov = new lov();
  public lsstaging: lov[] = new Array();
  slstaging: lov;

  //summary
  smitems: number = 0;
  smpu: number = 0;
  smpurec: number = 0;
  smpupnd: number = 0;
  smlnpu: number = 0;
  smlnsku: number = 0;
  smlnitem: number = 0;
  smlnhu: number = 0;

  //Enable config
  isconf: number = 0;

  //Product convertion
  public slcmfgdate: Date;
  public slcexpdate: Date;
  public acpmfgdate: Date;
  public acpexpdate: Date;

  // SA setstartload
  // SS save
  // SE close po
  // ED confirm
  // SV save line
  // public stateops:string[] = ["SA","SE",""];
  // public isstarted:boolean = false;
  // public isconfim:boolean = false;
  // public isfinish:boolean = false;
  // public isclose:boolean = false;
  public isbtnfinish: boolean = false;
  public isbtnsave: boolean = false;
  public isbtnconf: boolean = false;
  public ispndsave: boolean = false;

  //Toast Ref
  toastRef: any;

  //Parameter
  syspam: pam_inbound = new pam_inbound();
  ordpam: inbound_ls = new inbound_ls();
  public itmrowselect: Number;

  constructor(
    private sv: inboundService,
    private av: authService,
    private ss: shareService,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,
    public datepipe: DatePipe
  ) {
    this.opsorder.lines = new Array();
    this.slcdate = new Date();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.getmaster();

    this.sv.getParameter().subscribe((res) => {
      // console.log(res);
    });
  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() { }
  ngRefresh() {
    // get order
    this.crline = new inbouln_md();
    this.sv.get(this.ordpam).subscribe((res) => {
      this.opsorder = res;
      // check enable finishd button
      this.isbtnfinish = Number(this.opsorder.pendinginf) > 0 ? true : false;

      // this.ispndsave = this.opsorder.lines.forEach(l=>{
      //   l.details.filter(x=>x.)
      // })
    });

    // check pending
    //  let _qtypending:number = 0;
    //  this.opsorder.lines.forEach((el,index)=>_qtypending=_qtypending + el.qtypnd);
    //  this.ispending = _qtypending > 0 ?true:false;
  }

  // fndorder(){
  //     this.pm.tflow = (this.slcstate != null) ? this.slcstate.value : "";
  //     this.pm.ordertype = (this.slcordertype != null) ? this.slcordertype.value : "";
  //     this.pm.ismeasure = (this.slrqmsm != null) ? this.slrqmsm.value : "";
  //     this.pm.spcarea = (this.slspcarea != null) ? this.slspcarea.value : "";
  //     this.pm.inpriority = (this.slcpriority != null) ? this.slcpriority.value : "";
  //     this.pm.dockno = (this.slcstaging != null) ? this.slcstaging.value : "";

  //     this.sv.find(this.pm).pipe().subscribe( res => {
  //           this.lsorder = res;
  //           if (this.lsorder.length == 0){
  //             this.toastr.info("<span class='fn-07e'>Data not found with your condition</span>",null,{ enableHtml : true });
  //           }else {
  //             this.selorder(this.lsorder[0],0);
  //           }
  //         },
  //         (err) => { this.toastr.error(err.error.message); this.ison = false; },
  //         () => { }
  //     );
  // }
  public selorder(o: inbound_md) {
    this.itmrowselect = -1;
    this.ordpam.orgcode = o.orgcode;
    this.ordpam.site = o.site;
    this.ordpam.depot = o.depot;
    this.ordpam.spcarea = o.spcarea;
    this.ordpam.inorder = o.inorder;
    // this.opsorder = o;
    // console.log(this.opsorder);

    this.ngRefresh();

    this.isbtnfinish = Number(this.opsorder.pendinginf) > 0 ? true : false;

    this.isconf = 0;
    this.opsorder.lines.forEach((ln) => {
      if (ln.details == null) {
        ln.details = new Array();
      }
    });
    this.smitems = this.opsorder.lines.length;
    this.smpu = this.opsorder.lines.reduce((obj, val) => (obj += val.qtypu), 0);
    this.smpurec = this.opsorder.lines.reduce(
      (obj, val) => (obj += val.qtypurec),
      0
    );
    this.smpupnd = this.opsorder.lines.reduce(
      (obj, val) => (obj += val.qtypnd),
      0
    );
    this.slstaging = this.lsstaging.find(
      (x) => x.value == this.opsorder.dockrec
    );
    // reset line
    this.newline();
    // this.selline(this.opsorder.lines[0], 0);
  }
  selline(o: inbouln_md, ix: number) {


    console.log(this.opsorder.tflow);
    if (this.opsorder.tflow == 'IO' || this.opsorder.tflow == 'SA') {
      console.log("Click Start botton for receipt");
    } else if (this.opsorder.tflow == 'ED') {
      console.log("Order is already complted");
    } else {
      this.itmrowselect = ix;

      this.crline = o;
      this.crline.rtoskuofpu = 1; //hardcode
      this.slcdate = new Date();
      this.slcacp = this.datepipe.transform(
        this.slcdate.setDate(Number.parseInt('-' + this.crline.dlcfactory)),
        'yyyy.MM.dd'
      );
      this.newline();
      this.getratio();
      this.getline();
      this.acpmfgdate = new Date();
      this.acpexpdate = new Date();
      this.acpmfgdate = new Date(
        this.acpmfgdate.setDate(
          this.acpmfgdate.getDate() - this.crline.dlcfactory
        )
      );
      this.acpexpdate = new Date(
        this.acpexpdate.setDate(
          this.acpexpdate.getDate() + this.crline.dlcwarehouse
        )
      );
      this.slratio = this.lsratio.find((x) => x.valopnfirst == o.unitreceipt);
    }

  }

  cvSKU() {
    this.opsline.qtyskurec =
      Number(this.opsline.qtypurec) * Number(this.slratio.value);
    this.opsline.qtyhurec = Math.trunc(
      Number(this.opsline.qtyskurec) /
      Number(this.lsratio.find((e) => e.valopnfirst == '5').value)
    );
    this.opsline.qtyhurec =
      this.opsline.qtyhurec == 0 ? 1 : this.opsline.qtyhurec;
  }

  newline() {
    this.opsline.lnix = 0;
    this.opsline.orgcode = this.crline.orgcode;
    this.opsline.site = this.crline.site;
    this.opsline.depot = this.crline.depot;
    this.opsline.spcarea = this.crline.spcarea;
    this.opsline.inorder = this.crline.inorder;
    this.opsline.inln = this.crline.inln;
    this.opsline.inrefno = this.crline.inrefno;
    this.opsline.inrefln = this.crline.inrefln;
    this.opsline.barcode = this.crline.barcode;
    this.opsline.article = this.crline.article;
    this.opsline.pv = this.crline.pv;
    this.opsline.lv = this.crline.lv;
    this.opsline.unitops = this.crline.unitops;
    this.opsline.qtyskurec = 0;
    this.opsline.qtypurec = 0;
    this.opsline.qtyweightrec = 0;
    this.opsline.qtynaturalloss = 0;
    this.opsline.daterec = new Date();
    this.opsline.datemfg = new Date();
    this.opsline.dateexp = new Date();
    this.opsline.batchno = '';
    this.opsline.lotno = '';
    this.opsline.serialno = '';
    this.opsline.datecreate = new Date();
    this.opsline.accncreate = '';
    this.opsline.datemodify = new Date();
    this.opsline.accnmodify = '';
    this.opsline.procmodify = '';
    this.opsline.unitops = this.crline.unitops;
    this.opsline.inagrn = this.crline.inagrn;
    this.opsline.inseq = this.crline.inseq;
  }
  getmaster() {
    this.sv.getstaging(0).subscribe(
      (res) => {
        this.lsstaging = res;
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-08e'>" +
          (err.error == undefined ? err.message : err.error.message) +
          '</span>',
          null,
          { enableHtml: true }
        );
      },
      () => { }
    );
    this.sv.getParameter().subscribe(
      (res) => {
        this.syspam = res;
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-08e'>" +
          (err.error == undefined ? err.message : err.error.message) +
          '</span>',
          null,
          { enableHtml: true }
        );
      },
      () => { }
    );
  }
  getratio() {
    this.sv
      .getproductratio(
        this.crline.article,
        this.crline.pv.toString(),
        this.crline.lv.toString()
      )
      .subscribe(
        (res) => {
          this.lsratio = res;
          // this.crline.details.forEach(x=>{
          //   try {  x.unitopsdsc = this.lsratio.find(e=>e.valopnfirst == x.unitops).desc } catch(ex) { x.unitopsdsc = x.unitops + "Err"; }
          // });
        },
        (err) => {
          this.toastr.error(
            "<span class='fn-08e'>" +
            (err.error == undefined ? err.message : err.error.message) +
            '</span>',
            null,
            { enableHtml: true }
          );
        },
        () => { }
      );
  }
  getline() {
    this.sv.getlx(this.opsline).subscribe(
      (res) => {
        this.crline.details = res;
        this.crttrcv = this.crline.details.reduce(
          (obl, val) => (obl += val.qtyskurec),
          0
        );
        this.getratio();

        this.smlnitem = this.crline.details.length;
        this.smlnhu = this.crline.details.reduce(
          (obl, val) => (obl += val.qtyhurec),
          0
        );
        this.smlnpu = this.crline.details.reduce(
          (obl, val) => (obl += val.qtypurec),
          0
        );
        this.smlnsku = this.crline.details.reduce(
          (obl, val) => (obl += val.qtyskurec),
          0
        );

        if (this.opsorder.tflow == 'IO' || this.opsorder.tflow == 'SA') {
          this.isbtnfinish = false;
          this.isbtnsave = false;
        } else {
          // check pending
          this.enableSaveCheck();

          this.crline.details.filter((m) => m.tflow == 'SV');


          this.isbtnfinish =
            Number(this.opsorder.pendinginf) > 0 ? true : false;
          this.isbtnconf =
            this.crline.details.filter((m) => m.tflow == 'SV').length > 0
              ? true
              : false;

          // check enable finishd button
          this.isbtnfinish = Number(this.opsorder.pendinginf) > 0 ? true : false;
          //this.isbtnsave = this.crline.details.length == 0 || pndrec > 0 ?true:(_qtypnd > 0 ?true:false);

          // this.isbtnconf = this.crline.details.filter(m=>m.tflow!="ED").length == 0 ?false:true;
        }
      },
      (err) => {
        this.toastr.error(
          "<span class='fn-08e'>" +
          (err.error == undefined ? err.message : err.error.message) +
          '</span>',
          null,
          { enableHtml: true }
        );
      },
      () => { }
    );

  }
  saveline() {
    var now = new Date();

    if (
      this.crline.qtysku <
      this.crline.details.reduce((obl, val) => (obl += val.qtyskurec), 0) +
      this.opsline.qtyskurec
    ) {
      this.toastr.warning(
        "<span class='fn-08e'> Quantity more than order  </span>",
        null,
        { enableHtml: true }
      );
      //}else if (this.crline.qtypu == 0 || this.crline.qtysku == 0) { *** force to check sku only coz incase not full packsize
    } else if (this.crline.qtysku == 0) {
      this.toastr.warning(
        "<span class='fn-08e'> Quantity must more than 0  </span>",
        null,
        { enableHtml: true }
      );
    } else if (this.crline.isdlc == 1 && this.slcmfgdate == null) {
      this.toastr.warning(
        "<span class='fn-08e'> MFG date is require  </span>",
        null,
        { enableHtml: true }
      );
      // }else if (this.crline.isdlc == 1 && this.slcmfgdate != null && this.slcmfgdate > now) {
      //   this.toastr.warning("<span class='fn-08e'> MFG must less than today  </span>",null,{ enableHtml : true });
    } else if (this.crline.isdlc == 1 && this.slcexpdate == null) {
      this.toastr.warning(
        "<span class='fn-08e'> Expire date is require  </span>",
        null,
        { enableHtml: true }
      );
      // }else if (this.crline.isdlc == 1 && this.slcexpdate != null && this.slcexpdate < now) {
      //   this.toastr.warning("<span class='fn-08e'> Expire must more than today  </span>",null,{ enableHtml : true });
    } else if (
      this.crline.isbatchno == 1 &&
      (this.opsline.batchno == null || this.opsline.batchno == '')
    ) {
      this.toastr.warning(
        "<span class='fn-08e'> Batch no is require  </span>",
        null,
        { enableHtml: true }
      );
    } else if (
      this.crline.isunique == 1 &&
      (this.opsline.serialno == null || this.opsline.serialno == '')
    ) {
      this.toastr.warning(
        "<span class='fn-08e'> Serial no is require  </span>",
        null,
        { enableHtml: true }
      );
    } else {
      if (this.opsorder.dockrec == '') {
        this.toastr.warning(
          "<span class='fn-08e'> Dock receipt must be setp before </span>",
          null,
          { enableHtml: true }
        );
      } else if (this.opsline.qtypurec <= 0) {
        this.toastr.warning(
          "<span class='fn-08e'> Quantity must grater than 0 </span>",
          null,
          { enableHtml: true }
        );
      } else {
        this.opsline.datemfg = this.slcmfgdate;
        this.opsline.dateexp = this.slcexpdate;
        this.opsline.unitops = this.slratio.valopnfirst;
        this.sv.upsertlx(this.opsline).subscribe(
          (res) => {
            //update order display
            this.ngRefresh();
            this.crline.details = res;
            this.crttrcv = this.crline.details.reduce(
              (obl, val) => (obl += val.qtyskurec),
              0
            );
            this.opsline.qtypurec = 0;
            this.isbtnconf =
              this.crline.details.filter((m) => m.tflow == 'SV').length > 0
                ? true
                : false;
            this.toastr.success(
              "<span class='fn-08e'> Save line receipt success </span>",
              null,
              { enableHtml: true }
            );

            this.enableSaveCheck();
          },
          (err) => {
            this.toastr.error(
              "<span class='fn-08e'>" +
              (err.error == undefined ? err.message : err.error.message) +
              '</span>',
              null,
              { enableHtml: true }
            );
          },
          () => { }
        );
      }
    }
  }

  enableSaveCheck() {
    // pending + received
    let saveqty = 0;
    this.crline.details.forEach(e => { 
      saveqty = saveqty + e.qtypurec;
    });

    let orderqty = 0;
    this.opsorder?.lines.filter(x => x.article == this.crline.article)
      .forEach(e =>{ 
        orderqty = orderqty + e.qtypu
      });


    this.isbtnsave = saveqty < orderqty ? true : false;
  }

  remline(o: inboulx) {
    this.ngPopups
      .confirm('Do you accept to remove line receipt ?')
      .subscribe((res) => {
        if (res) {
          this.sv.removelx(o).subscribe(
            (res) => {
              this.ngRefresh();
              this.crline.details = res;
              this.crttrcv = this.crline.details.reduce(
                (obl, val) => (obl += val.qtyskurec),
                0
              );
              this.toastr.info(
                "<span class='fn-08e'> Remove line receipt success </span>",
                null,
                { enableHtml: true }
              );
              this.isbtnconf =
                this.crline.details.filter((m) => m.tflow == 'SV').length > 0
                  ? true
                  : false;

              // check pending
              this.enableSaveCheck();
            },

            (err) => {
              this.toastr.error(
                "<span class='fn-08e'>" +
                (err.error == undefined ? err.message : err.error.message) +
                '</span>',
                null,
                { enableHtml: true }
              );
            },
            () => { }
          );
        }
      });
  }
  commitline() {
    if (this.crline.details.filter((e) => e.tflow == 'SV').length == 0) {
      this.toastr.warning(
        "<span class='fn-08e'> Require to recieve atleast 1 line before confirm </span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you accept to confirm line receipt ?')
        .subscribe((res) => {
          if (res) {
            this.sv
              .commitlx(this.crline.details.filter((e) => e.tflow == 'SV')[0])
              .subscribe((res) => {
                this.crline.details = res;
                this.crttrcv = this.crline.details.reduce(
                  (obl, val) => (obl += val.qtyskurec),
                  0
                );
                this.toastr.info(
                  "<span class='fn-08e'> Confirm line receipt success </span>",
                  null,
                  { enableHtml: true }
                );
                this.isbtnconf =
                  this.crline.details.filter((m) => m.tflow == 'SV').length > 0
                    ? true
                    : false;

                this.ngRefresh();

              });
          }
        });
    }
  }
  dscicon(o: string) {
    return '';
  }
  CInt(o: string) {
    return Number.parseInt(o);
  }

  calexpdate() {
    if (this.syspam.allowcalculatemfg == true) {
      this.slcexpdate = new Date();
      // this.slcexpdate = new Date(this.slcexpdate.setDate(this.slcmfgdate.getDate() + this.crline.dlcwarehouse));
      // this.slcexpdate = new Date(this.slcexpdate.setDate(this.slcmfgdate.getDate() + this.crline.dlcall));
      this.slcexpdate = this.addDays(this.slcmfgdate, this.crline.dlcall);
    }
  }
  calmfgdate() {
    if (this.syspam.allowcalculatemfg == true) {
      this.slcmfgdate = new Date();
      // this.slcmfgdate = new Date(this.slcmfgdate.setDate(this.slcexpdate.getDate() + this.crline.dlcfactory));
      this.slcmfgdate = this.addDays(this.slcexpdate, this.crline.dlcall * -1);
    }
  }

  addDays(date, daysToAdd) {
    var _24HoursInMilliseconds = 86400000;
    return new Date(date.getTime() + daysToAdd * _24HoursInMilliseconds);
  }

  dscordtype(o: string) {
    switch (o) {
      case 'ST':
        return 'Stocking';
      case 'XD':
        return 'Crossdocking';
      case 'FW':
        return 'Forwarding';
      default:
        return o;
    }
  }
  setonf() {
    this.isconf = this.isconf == 1 ? 0 : 1;
  }
  setremark() {
    this.ngPopups
      .confirm('Do you confirm remarks on Inbound order ?')
      .subscribe((res) => {
        if (res) {
          this.sv
            .setremarks(this.opsorder.inorder, this.opsorder.remarkrec)
            .subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-08e'>Change remarks success </span>",
                  null,
                  { enableHtml: true }
                );
                this.ngRefresh();
                this.ngRefreshLine();
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-08e'>" +
                  (err.error == undefined ? err.message : err.error.message) +
                  '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => { }
            );
        }
      });
  }

  setinvoice() {
    if (this.valid(this.opsorder.invno)) {
      this.ngPopups
        .confirm('Do you confirm invoice no on Inbound order ?')
        .subscribe((res) => {
          if (res) {
            this.sv
              .setinvoice(this.opsorder.inorder, this.opsorder.invno)
              .subscribe(
                (res) => {
                  this.toastr.success(
                    "<span class='fn-08e'>Set invoice no success </span>",
                    null,
                    { enableHtml: true }
                  );
                  this.ngRefresh();
                  this.ngRefreshLine();
                },
                (err) => {
                  this.toastr.error(
                    "<span class='fn-08e'>" +
                    (err.error == undefined ? err.message : err.error.message) +
                    '</span>',
                    null,
                    { enableHtml: true }
                  );
                },
                () => { }
              );
          }
        });

    }
  }

  setreplan() {
    this.ngPopups
      .confirm('Do you confirm re-plan delivery date on Inbound order ?')
      .subscribe((res) => {
        if (res) {
          this.sv
            .setinvoice(this.opsorder.inorder, this.opsorder.invno)
            .subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-08e'>Change plandelivery date success </span>",
                  null,
                  { enableHtml: true }
                );
                this.ngRefresh();
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-08e'>" +
                  (err.error == undefined ? err.message : err.error.message) +
                  '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => { }
            );
        }
      });
  }

  setstaging() {
    this.ngPopups
      .confirm('Do you confirm setup staging for receipt ?')
      .subscribe((res) => {
        if (res) {
          this.sv
            .setstaging(this.opsorder.inorder, this.slstaging.value)
            .subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-08e'>Setup Staging success </span>",
                  null,
                  { enableHtml: true }
                );
                this.opsorder.dockrec = this.slstaging.value;
                this.opsorder.tflow = 'SA';
                this.ngRefresh();
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-08e'>" +
                  (err.error == undefined ? err.message : err.error.message) +
                  '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => { }
            );
        }
      });
  }

  setstartload() {
    if (this.opsorder.invno.trim() == '') {
      this.toastr.error(
        "<span class='fn-08e'>Require Invoice no before close order</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm start unload product from supplier ?')
        .subscribe((res) => {
          if (res) {
            this.sv.setunloadstart(this.opsorder.inorder).subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-08e'>Set to start unload success </span>",
                  null,
                  { enableHtml: true }
                );
                this.opsorder.tflow = 'SS';
                this.ngRefresh();
                this.ngRefreshLine();
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-08e'>" +
                  (err.error == undefined ? err.message : err.error.message) +
                  '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => { }
            );
          }
        });
    }
  }

  setfinishload() {
    if (this.crline.details.filter((e) => e.tflow == 'SV').length > 0) {
      this.toastr.warning(
        "<span class='fn-08e'>Please confirm receipt that pending before</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm finish unload product from supplier ?')
        .subscribe((res) => {
          if (res) {
            this.sv.setunloadend(this.opsorder.inorder).subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-08e'>Set to finish unload success </span>",
                  null,
                  { enableHtml: true }
                );
                this.opsorder.tflow = 'SE';
                this.ngRefresh();
                this.ngRefreshLine();
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-08e'>" +
                  (err.error == undefined ? err.message : err.error.message) +
                  '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => { }
            );
          }
        });
    }
  }
  setcancel() {
    if (this.opsorder.remarkrec == null) {
      this.opsorder.remarkrec = '';
    }
    if (this.opsorder.lines.filter((x) => x.qtypurec > 0).length > 0)
      this.toastr.error(
        "<span class='fn-08e'>Order is under receipt process can't cancel </span>",
        null,
        { enableHtml: true }
      );
    else if (this.opsorder.remarkrec.trim().length < 10) {
      this.toastr.error(
        "<span class='fn-08e'>Remarks must grater than 10 charactor</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm cancel Inbound order ?')
        .subscribe((res) => {
          if (res) {
            this.sv
              .setcancel(this.opsorder.inorder, this.opsorder.remarkrec)
              .subscribe(
                (res) => {
                  this.toastr.success(
                    "<span class='fn-08e'>Cancel order success </span>",
                    null,
                    { enableHtml: true }
                  );
                  this.opsorder.tflow = 'CL';
                  this.isconf = 0;
                  this.opsorder.datemodify = new Date();
                  this.opsorder.accnmodify = this.av.crProfile.accncode;
                  // refresh current tab
                  this.ngRefresh();
                  // refresh tabl 1
                  this.ngRefreshLine();
                },
                (err) => {
                  this.toastr.error(
                    "<span class='fn-08e'>" +
                    (err.error == undefined
                      ? err.message
                      : err.error.message) +
                    '</span>',
                    null,
                    { enableHtml: true }
                  );
                },
                () => { }
              );
          }
        });
    }
  }
  setclose() {
    if (this.opsorder.invno.trim() == '') {
      this.toastr.error(
        "<span class='fn-08e'>Require Invoice no before close order</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm close Inbound order ?')
        .subscribe((res) => {
          if (res) {
            this.sv.setfinish(this.opsorder.inorder).subscribe(
              (res) => {
                this.toastr.success(
                  "<span class='fn-08e'>Close Inbound order success </span>",
                  null,
                  { enableHtml: true }
                );
                this.opsorder.tflow = 'ED';
                this.ngRefresh();
                this.ngRefreshLine();
              },
              (err) => {
                this.toastr.error(
                  "<span class='fn-08e'>" +
                  (err.error == undefined ? err.message : err.error.message) +
                  '</span>',
                  null,
                  { enableHtml: true }
                );
              },
              () => { }
            );
          }
        });
    }
  }

  setpriority() {
    if (this.syspam.allowchangepriority) {
      this.ngPopups
        .confirm(
          'Do you confirm setup priority of order ' +
          this.opsorder.inorder +
          ' ?'
        )
        .subscribe((res) => {
          if (res) {
            this.opsorder.inpriority = this.opsorder.inpriority == 0 ? 30 : 0;
            this.sv
              .setpriority(this.opsorder.inorder, this.opsorder.inpriority)
              .subscribe(
                (res) => {
                  this.toastr.success(
                    "<span class='fn-08e'>Setup priority success </span>",
                    null,
                    { enableHtml: true }
                  );

                  this.ngRefresh();
                },
                (err) => {
                  this.opsorder.inpriority =
                    this.opsorder.inpriority == 0 ? 30 : 0;
                  this.toastr.error(
                    "<span class='fn-08e'>" +
                    (err.error == undefined
                      ? err.message
                      : err.error.message) +
                    '</span>',
                    null,
                    { enableHtml: true }
                  );
                },
                () => { }
              );
          }
        });
    }
  }

  getdocstatement() {
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
      .getdocstatement(
        this.opsorder.orgcode,
        this.opsorder.site,
        this.opsorder.depot,
        this.opsorder.inorder
      )
      .subscribe((response) => {
        let blob: any = new Blob([response], {
          type: 'text/json; charset=utf-8',
        });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute(
          'download',
          'bgcwms_receipt_statement_' + this.opsorder.inorder + '.pdf'
        );
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId);
      }),
      (error) => {
        this.toastr.clear(this.toastRef.ToastId);
      };
  }

  ngRefreshLine() {
    this.refrln.emit();
  }
  ngDecUnit(o: string) {
    return this.ss.ngDecUnitstock(o);
  }

  public valid(model: any) {
    var isvalid: boolean = false;
    if (model === undefined) {
      isvalid = false;
    } else if (model === null) {
      isvalid = false;
    } else if (model === "") {
      isvalid = false;
    } else if (this.Trim(model) === "") {
      isvalid = false;
    } else {
      isvalid = true;
    }
    return isvalid;
  }
  public Trim(str) {
    return str.toString().replace(/^\s+|\s+$/gm, "");
  }
}
