import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild, EventEmitter, Output, HostListener, ElementRef, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { countline_md, countplan_md, counttask_md } from '../../models/inv.count.model';
import { countService } from '../../services/Inv.count.service';
import { shareService } from 'src/app/share.service';
import { DatePipe } from '@angular/common';
import { admproductService } from 'src/app/admn/services/adm.product.service';
import { product_pm } from 'src/app/admn/models/adm.product.model';
import { ouhanderlingunitService } from 'src/app/outbound/Services/oub.handerlingunit.service';
import { handerlingunit } from 'src/app/outbound/Models/oub.handlingunit.model';
import { forkJoin } from 'rxjs/internal/observable/forkJoin';
declare var $: any;
@Component({
  selector: 'appinv-countline',
  templateUrl: 'inv.count.line.html',
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
    '.px-140{width:140px;margin-left:5px;margin-right:5px;}',
    '.row-p-0{display:flex;flex-wrap: wrap;padding-right:10px;padding-left:2px;margin:0;line-height: 2.2}',
    '.btn-xs.active{background-color: #a7c95758;font-weight: bold;color: #386641;border: 1px solid #9ab754a1}',
  ],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]

})
export class invcountlineComponent implements OnInit, OnDestroy, AfterViewInit {
  @HostListener("window:resize")
  onResize() { this.editorWidth = (this.planLine.nativeElement as HTMLElement).offsetWidth; }

  @ViewChild('planLine') planLine: ElementRef;
  public editorWidth: number = 0;
  public editorIsOpen: boolean = false;
  public editorLinemd: countline_md = new countline_md();

  @Output() selout = new EventEmitter<counttask_md>();
  public crrtask: counttask_md = new counttask_md();
  public crstate: boolean = false;
  public lspctval: lov[] = new Array();
  public slcpct: lov;
  public lszone: lov[] = new Array();
  public lsstate: lov[] = new Array();
  public lsplan: countplan_md[] = new Array();
  public crplan: countplan_md = new countplan_md();
  // public lssave:countline_md[] = new Array();
  public lsline: countline_md[] = new Array();

  public slczone: lov = new lov();
  public dateformat: string;
  public dateformatlong: string;
  public toastRef: any;
  public disabledSave: boolean;
  public rowselected: number;
  public newline: boolean;
  public lineprogress: number;
  // public planprogress: number;
  constructor(private sv: countService,
    private av: authService,
    private mv: adminService,
    private pv: admproductService,
    private ss: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,
    private hv: ouhanderlingunitService,
    private datepipe: DatePipe) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;

  }

  ngOnInit(): void { this.ngSetup(); }
  ngSetup() {
    this.mv.getlov("COUNT", "RESULT").pipe().subscribe(
      (res) => { this.lsstate = res; },
      (err) => { },
    );
  }
  ngFind() {
    this.sv.listPlan(this.crrtask).pipe().subscribe(
      (res) => {
        let ioline = res.filter(e => e.tflow == "IO");
        if (ioline.length > 0) {
          this.lsplan = ioline;
          // plan progress 
          // let countting: number = 0;
          // let total: number = 0;
          // this.lsplan.forEach(e => {
          //   console.log(e.tflow);
          //   if (e.tflow == "ED") {
          //     countting++;
          //   }
          //   total++;
          // });

          // this.planprogress = Number(((countting * 100) / total).toFixed(2));
        } else {
          this.lsplan = [];
          this.lsline = [];
        }
      },
      (err) => { this.toastr.success("<span class='fn-07e'>get line count error " + err + " </span>", null, { enableHtml: true }); },
    );
  }
  ngSelc(o: counttask_md) {
    this.lineprogress = 0;
    this.crrtask.countcode = o.countcode;
    this.ngFind();
  }
  ngSave() {
    this.ngPopups.confirm('Do you confirm save plan count ?')
      .subscribe(res => {
        if (res) {
          // check input qty and not input article 
          if (this.lsline.filter(e => e.cnqtypu > 0 && !this.valid(e.cnarticle) && !this.valid(e.cnbarcode)).length > 0) {
            this.toastr.error("<span class='fn-07e'>line count qty is required product code</span>", null, { enableHtml: true });
          } else if (this.lsline.filter(e => !this.valid(e.cnqtypu)).length > 0) {
            this.toastr.error("<span class='fn-07e'>line count qty is required </span>", null, { enableHtml: true });
          } else {
            // convert puto number
            this.lsline.forEach(e => e.cnqtypu = Number(e.cnqtypu));
            if (this.crrtask.counttype == "CT") { this.crplan.pctvld = Number.parseInt(this.slcpct.value); }
            this.crplan.pctvld = 100;
            this.sv.upsertLineAsync(this.lsline).pipe().subscribe(() => {
              this.toastr.success("<span class='fn-07e'> Save line count success </span>", null, { enableHtml: true });

              this.ngSelect(this.crplan, this.rowselected);
              // this.selout.emit(this.crrtask);
            }, () => { this.toastr.error("<span class='fn-07e'> Save line count error , please try again.</span>", null, { enableHtml: true }); },
            );
          }
        }
      });
  }
  ngSelect(o: countplan_md, ix: number) {
    this.rowselected = ix;
    this.crplan = o;
    this.ngLine();
  }
  ngLine() {
    //listLineAsync
    this.sv.countLineAsync(this.crplan).pipe().subscribe(
      (res) => {
        this.lsline = [];
        let counted: number = 0;
        let total: number = 0;

        res.forEach(e => {
          // defult count qty 
          if (!this.valid(e.cnflow)) {
            e.cnqtypu = e.locctype == "R" ? e.stqtypu : null;
          }

          if (e.cnflow == 'IO') {
            counted = counted + e.cnqtypu;
          }

          total = total + (!this.valid(e.stqtypu) ? 0 : e.stqtypu);
        });
        this.lsline = res;
        this.disabledSave = this.lsline.length == 0 ? true : false;

        console.log("counted:" + counted);
        console.log("total:" + total);

        // update progress
        total = total==0?1:total;
        this.lineprogress = Number(((counted  * 100) / total).toFixed(2)) ;

        console.log(this.lineprogress);
        // this.lssave = Object.assign([], res); 
      },
      (err) => { this.toastr.error("<span class='fn-07e'> get line count error , please try again.</span>", null, { enableHtml: true }); },
      () => { }
    );
  }


  ngDformat(strdat: Date) {
    let covdate: any;
    if (strdat != null) {
      covdate = this.datepipe.transform(strdat, 'yyyy-MM-dd');
    }
    return covdate;
  }


  ngDecIcon(o: string) { return this.ss.ngDecIcon(o); }
  ngDecStr(o: string) { return this.ss.ngDecStr(o);; }
  ngDeclnIcon(o: string) { return this.lsstate.find(e => e.value == o).icon; }
  ngDeclnStr(o: string) { return this.lsstate.find(e => e.value == o).desc; }
  ngDecUnitstock(o: string) {
    return this.ss.ngDecUnitstock(o);
  }

  ngAfterViewInit(): void { this.ngFind(); }
  ngOnDestroy(): void {
    this.crrtask = null; delete this.crrtask;
    this.crstate = null; delete this.crstate;
    this.lspctval = null; delete this.lspctval;
    this.slcpct = null; delete this.slcpct;
    this.lszone = null; delete this.lszone;
    this.lsstate = null; delete this.lsstate;
    this.lsplan = null; delete this.lsplan;
    this.crplan = null; delete this.crplan;
    this.lsline = null; delete this.lsline;
    // this.lssave         = null; delete this.lssave;
    this.slczone = null; delete this.slczone;
    this.dateformat = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.selout.unsubscribe; delete this.selout;
  }

  public editDateexpMd: string;
  public editLineIndex: number;
  public editLineMessage: string;
  ngEditLine(ln: countline_md, ix: number) {
    this.newline = false;
    this.editLineIndex = -1;
    this.editorLinemd = new countline_md();
    this.editorLinemd = Object.assign({}, ln);
    this.editLineIndex = ix;
    this.editorWidth = (this.planLine.nativeElement as HTMLElement).offsetWidth;
    this.editDateexpMd = this.ngDformat(ln.cndateexp);
    this.editorIsOpen = true;
  }

  ngSaveLine() {
    let errorCount: number = 0;
    this.editLineMessage = "";
    if (this.editorLinemd) {
      if (!this.valid(this.editorLinemd.cnbarcode) && !this.valid(this.editorLinemd.cnarticle)) {
        errorCount++;
        this.editLineMessage = "Barcode or Article is required !";
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      }
      else if (!this.valid(this.editorLinemd.cnhuno)) {
        errorCount++;
        this.editLineMessage = "HU No is required !";
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      }
      else if (this.crplan.isdatexp == 1 && !this.valid(this.editorLinemd.cndateexp)) {
        errorCount++;
        this.editLineMessage = "Date Expire is required !";
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      }
      else if (this.crplan.isbatchno == 1 && !this.valid(this.editorLinemd.cnlotmfg)) {
        errorCount++;
        this.editLineMessage = "Batch no is required !";
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      }
      else if (!this.valid(this.editorLinemd.cnqtypu)) {
        errorCount++;
        this.editLineMessage = "Count qty is required !";
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      }
      // check duplicate input hu
      else if (this.lsline.filter(x => x.cnhuno == this.editorLinemd.cnhuno && x.loccode != this.editorLinemd.loccode).length > 0) {
        errorCount++;
        this.editLineMessage = "HU " + this.editorLinemd.cnhuno + " is duplicate !";
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      }
      else {
        // check pallet parameter
        let hu = new handerlingunit();
        hu.huno = this.editorLinemd.cnhuno;

        // check product parameter
        let fnproduct = new product_pm();
        fnproduct.article = this.editorLinemd.cnarticle == ""
          ? this.editorLinemd.cnbarcode : this.editorLinemd.cnarticle;

        if (this.valid(this.editorLinemd.cnlv)) {
          fnproduct.lv = this.editorLinemd.cnlv;
        }

        // setp 1 check pallet
        if (this.editorLinemd.cnhuno != this.editorLinemd.sthuno) {
          let hu = new handerlingunit(); hu.huno = this.editorLinemd.cnhuno;
          this.hv.find(hu).subscribe(res => {
            if (res.length == 0) {
              this.editLineMessage = "HU is not exists in system!";
              this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
            } else {
              // step 1.1 check product
              this.validateProduct(fnproduct);
            }
          }, (err: any) => {
            this.editLineMessage = err.message;
            this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
          });
        } else {
          // step 2 check product
          this.validateProduct(fnproduct);
        }
      }
    }
  }
  checkNewLine() {
    if (!this.valid(this.editorLinemd.starticle)) {
      this.newline = false;
    } else if (this.valid(this.editorLinemd.starticle) && this.editorLinemd.cnhuno == this.editorLinemd.sthuno) {
      this.newline = false;
    } else {
      this.newline = true;
    }
  }
  validateProduct(o: product_pm) {
    if ((this.editorLinemd.cnarticle != this.editorLinemd.starticle) ||
      (this.editorLinemd.cnlv != this.editorLinemd.cnlv)
      || (this.editorLinemd.cnbarcode != this.editorLinemd.stbarcode)
      || (this.editorLinemd.cnlv != this.editorLinemd.stlv)
    ) {
      this.pv.active(o).subscribe(res => {
        if (!this.valid(res.article)) {
          this.editLineMessage = "product not found";
          this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
        } else {
          // new product
          this.editorLinemd.cnarticle = res.article;
          this.editorLinemd.cnpv = res.pv;
          this.editorLinemd.cnlv = res.lv;
          this.editorLinemd.cnbarcode = res.barcode;
          if (this.newline) {
            // reset to zero
            this.lsline[this.editLineIndex].cnqtypu = 0;
            // insert line
            this.editorLinemd.cnflow = 'NW';
            this.editorLinemd.unitcount = res.unitmanage;
            this.lsline.push(this.editorLinemd);
          } else {
            // update line
            this.lsline[this.editLineIndex] = this.editorLinemd;
          }

          // close form
          this.ngCancleEdit();
        }
      }, (err: any) => {
        this.editLineMessage = err.message;
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      });
    } else {
      this.lsline[this.editLineIndex] = this.editorLinemd;
      this.ngCancleEdit();
    }
  }


  stringToDate = (dateString) => {
    const [day, month, year] = dateString.split('/');
    return new Date([month, day, year].join('/'));
  };

  ngResetEdit() {
    this.editorLinemd.cnhuno = this.editorLinemd.sthuno;
    this.editorLinemd.cnbarcode = this.editorLinemd.stbarcode;
    this.editorLinemd.cnarticle = this.editorLinemd.starticle;
    this.editorLinemd.cnlv = this.editorLinemd.stlv;
    this.editorLinemd.cndateexp = this.editorLinemd.stdateexp;
    this.editorLinemd.cnlotmfg = this.editorLinemd.stlotmfg;
    this.editorLinemd.cnflow = this.editorLinemd.cnflow;
  }
  ngCancleEdit() {
    this.editorIsOpen = false;
    this.editLineIndex = -1;
    this.editorLinemd = new countline_md();
  }
  ngRemvoeEdit() {
    if (this.editLineIndex != -1 && this.editorLinemd.cnflow == 'NW') {
      this.lsline.splice(this.editLineIndex, 1);
      // close form
      this.ngCancleEdit();
    }
  }

  //   getdocstatement() { 
  //     window.open("http://localhost/bgcwmsdocument/get/getCountsheet?planno=4-001" , "_blank");
  //   }


  getdocstatement(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>", null, {
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass: 'toast-bottom-right', enableHtml: true
    });

    this.sv.getcountlist(this.crplan).subscribe(response => {
      let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_countsheet_" + o + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId);
    }, err => {
      this.toastr.clear(this.toastRef.ToastId);
      this.toastr.error("<span class='fn-07e'> Generate Count Sheet error " + err.message + "</span>", null, { enableHtml: true });
    });

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
