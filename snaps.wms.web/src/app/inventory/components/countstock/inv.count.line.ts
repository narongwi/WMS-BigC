import { product_vld } from './../../models/inv.count.model';
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

  @ViewChild('editpuqty') editpuqty: ElementRef;

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
  public radioGenhuno: boolean;
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
        total = total == 0 ? 1 : total;
        this.lineprogress = Number(((counted * 100) / total).toFixed(2));

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
    // this.radioGenhuno = this.valid(this.editorLinemd.cnhuno)?false:true;
    this.editorWidth = (this.planLine.nativeElement as HTMLElement).offsetWidth;
    this.editDateexpMd = this.ngDformat(ln.cndateexp);
    this.editorIsOpen = true;
  }

  productChanged(inputtype) {
    if (!this.valid(this.editorLinemd.cnbarcode) && !this.valid(this.editorLinemd.cnarticle)) {
      return;
    } else {
      let productval: product_vld = {};
      productval.orgcode = this.editorLinemd.orgcode;
      productval.site = this.editorLinemd.site;
      productval.depot = this.editorLinemd.depot;
      if (inputtype == "barcode") {
        productval.barcode = this.editorLinemd.cnbarcode;
        productval.article = "";
      } else if (inputtype == "article") {
        productval.barcode = "";
        productval.article = this.editorLinemd.cnarticle;
      } else {
        productval.barcode = this.editorLinemd.cnbarcode;
        productval.article = this.editorLinemd.cnarticle;

      }
      productval.pv = this.editorLinemd.cnpv;
      productval.lv = this.editorLinemd.cnlv;
      productval.loccode = this.editorLinemd.loccode;
      productval.huno = this.editorLinemd.cnhuno;
      productval.unitcount = this.editorLinemd.unitcount;
      productval.countcode = this.editorLinemd.countcode;
      productval.plancode = this.editorLinemd.plancode;
      productval.linecode = this.editorLinemd.locseq;
      // productval.qtycount = this.editorLinemd.cnqtypu;
      // productval.isnewhu = this.radioGenhuno;

      this.sv.findProduct(productval).subscribe((res: product_vld) => {
        this.editorLinemd.cnbarcode = res.barcode;
        this.editorLinemd.cnarticle = res.article;
        this.editorLinemd.cnpv = Number(res.pv);
        this.editorLinemd.cnlv = Number(res.lv);
        this.editorLinemd.unitcount = res.unitcount;
        this.editorLinemd.productdesc = res.descalt;
        this.editpuqty.nativeElement.focus();
      }, (err: any) => {
        this.editLineMessage = err.message;
        this.editorLinemd.cnbarcode = "";
        this.editorLinemd.cnarticle = "";
        this.editorLinemd.cnpv = 0;
        this.editorLinemd.cnlv = 0;
        this.editorLinemd.unitcount = "";
        this.editorLinemd.productdesc = "";
        this.editorLinemd.cnqtypu = 0;
        this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
      });
    }
  }
  public generate() {
    this.ngPopups.confirm('Do you Generate New HU ?').subscribe(res => {
      if (res) {
        let generateval: product_vld = {};
        generateval.orgcode = this.editorLinemd.orgcode;
        generateval.site = this.editorLinemd.site;
        generateval.depot = this.editorLinemd.depot;
        generateval.barcode = this.editorLinemd.cnbarcode;
        generateval.article = this.editorLinemd.cnarticle;
        generateval.pv = this.editorLinemd.cnpv;
        generateval.lv = this.editorLinemd.cnlv;
        generateval.lotmfg = this.editorLinemd.cnlotmfg;
        generateval.dateexp = this.editorLinemd.cndateexp;
        generateval.datemfg = this.editorLinemd.cndatemfg;
        generateval.serialno = this.editorLinemd.cnserialno;
        generateval.loccode = this.editorLinemd.loccode;
        generateval.huno = this.editorLinemd.cnhuno;
        generateval.unitcount = this.editorLinemd.unitcount;
        generateval.countcode = this.editorLinemd.countcode;
        generateval.plancode = this.editorLinemd.plancode;
        generateval.linecode = this.editorLinemd.locseq;
        generateval.qtycount = !this.valid(this.editorLinemd.cnqtypu) ? 0 : this.editorLinemd.cnqtypu;

        this.sv.generatehu(generateval).subscribe((res: countline_md) => {
          this.lsline[this.editLineIndex].cnqtypu = 0;
          this.editorLinemd = res;
          this.lsline.push(this.editorLinemd);
          this.editLineIndex = this.lsline.length - 1;
          this.radioGenhuno = false;

          // reset current count = 0
          this.toastr.success("<span class='fn-07e'>Generated Successful</span>", null, { enableHtml: true });
        }, (err: any) => {
          this.editLineMessage = err.message;
          this.toastr.error("<span class='fn-07e'>" + this.editLineMessage + "</span>", null, { enableHtml: true });
        });
      }
    });
  }
  public label() {
    if (!this.valid(this.editorLinemd.cnhuno)) {
      this.toastr.error(
        "<span class='fn-07e'>Invalid HU No !</span>", null,
        { enableHtml: true });
    } else {
      this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>", null, {
        disableTimeOut: true,
        tapToDismiss: false,
        closeButton: false,
        positionClass: 'toast-bottom-right', enableHtml: true
      });

      let sourclabel = "M";// from wm_merge hu data source
      if (this.editorLinemd.cnhuno == this.editorLinemd.sthuno) {
        sourclabel = "N";// from wm_stock data source
      }

      this.sv.label(this.editorLinemd.orgcode, this.editorLinemd.site, this.editorLinemd.depot, this.editorLinemd.cnhuno, sourclabel).subscribe(response => {
        let blob: any = new Blob([response], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');

        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_labelhu_" + this.editorLinemd.cnhuno + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        document.body.removeChild(downloadLink);
        this.toastr.clear(this.toastRef.ToastId);
      }, () => {
        this.toastr.clear(this.toastRef.ToastId);
        this.toastr.warning(
          "<span class='fn-07e'> Label Data Not Found </span>", null,
          { enableHtml: true });
      });
    }
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
      else if (this.radioGenhuno == false && !this.valid(this.editorLinemd.cnhuno)) {
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
        this.ngPopups.confirm('Do you Save Change ?').subscribe(res => {
          if (res) {
            let hunoval: product_vld = {};
            hunoval.orgcode = this.editorLinemd.orgcode;
            hunoval.site = this.editorLinemd.site;
            hunoval.depot = this.editorLinemd.depot;
            hunoval.barcode = this.editorLinemd.cnbarcode;
            hunoval.article = this.editorLinemd.cnarticle;
            hunoval.pv = this.editorLinemd.cnpv;
            hunoval.lv = this.editorLinemd.cnlv;
            hunoval.lotmfg = this.editorLinemd.cnlotmfg;
            hunoval.dateexp = this.editorLinemd.cndateexp;
            hunoval.datemfg = this.editorLinemd.cndatemfg;
            hunoval.serialno = this.editorLinemd.cnserialno;
            hunoval.loccode = this.editorLinemd.loccode;
            hunoval.huno = this.editorLinemd.cnhuno;
            hunoval.unitcount = this.editorLinemd.unitcount;
            hunoval.countcode = this.editorLinemd.countcode;
            hunoval.plancode = this.editorLinemd.plancode;
            hunoval.linecode = this.editorLinemd.locseq;
            hunoval.qtycount = !this.valid(this.editorLinemd.cnqtypu) ? 0 : this.editorLinemd.cnqtypu;

            if(this.editorLinemd.cnhuno == this.lsline[this.editLineIndex].cnhuno &&
              this.editorLinemd.cnarticle == this.lsline[this.editLineIndex].cnarticle &&
              this.editorLinemd.cnlv == this.lsline[this.editLineIndex].cnlv){
                hunoval.isnewhu = false;
              }
              else {
                hunoval.isnewhu =true;
              }

            this.sv.validatehu(hunoval).subscribe((res: countline_md) => {
              // add line ifany
              if (hunoval.isnewhu) {
                this.lsline[this.editLineIndex].cnqtypu = 0;
                this.editorLinemd = res;
                this.lsline.push(this.editorLinemd);
                this.editLineIndex = this.lsline.length - 1;
                this.ngCancleEdit();
              }else{
                this.lsline[this.editLineIndex] = this.editorLinemd;
                this.ngCancleEdit();
              }

             
            }, err => {
              this.editLineMessage = err.message;
              this.toastr.error(
                "<span class='fn-07e'>" + this.editLineMessage + "</span>", null,
                { enableHtml: true });
            });
          }
        });
      }
    }
  }

  stringToDate = (dateString) => {
    const [day, month, year] = dateString.split('/');
    return new Date([month, day, year].join('/'));
  };

  ngResetEdit() {
    this.editorLinemd.cnhuno = this.lsline[this.editLineIndex].cnhuno;
    this.editorLinemd.cnbarcode = this.lsline[this.editLineIndex].cnbarcode;
    this.editorLinemd.cnarticle = this.lsline[this.editLineIndex].cnarticle;
    this.editorLinemd.cnlv = this.lsline[this.editLineIndex].cnlv;
    this.editorLinemd.cndateexp = this.lsline[this.editLineIndex].cndateexp;
    this.editorLinemd.cnlotmfg = this.lsline[this.editLineIndex].cnlotmfg;
    this.editorLinemd.cnflow = this.lsline[this.editLineIndex].cnflow;
    this.editorLinemd.cnqtypu = this.lsline[this.editLineIndex].cnqtypu;
    this.radioGenhuno = false;
    this.editLineMessage = "";
  }
  ngCancleEdit() {
    this.radioGenhuno = false;
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

  public GenhuChange() {
    console.log("qtypu valid : " + this.valid(this.editorLinemd.cnqtypu));
    if (!this.valid(this.editorLinemd.cnqtypu)) {
      this.editorLinemd.cnqtypu = 0;
    }
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
