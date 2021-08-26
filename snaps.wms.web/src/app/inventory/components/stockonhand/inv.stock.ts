import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { throwIfEmpty } from 'rxjs/operators';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { inboundService } from 'src/app/inbound/services/app-inbound.service';
import { shareService } from 'src/app/share.service';
import { convertCompilerOptionsFromJson } from 'typescript';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { stock_md, stock_ls, stock_pm, stock_info } from '../../models/inv.stock.model';
import { inventoryService } from '../../services/inv.stock.service';


declare var $: any;
@Component({
  selector: 'appinv-stock',
  templateUrl: 'inv.stock.html',
  styles: ['.dgproduct { height:150px !important; } ', '.dgstock { height:calc(100vh - 650px) !important; } ',
    '.ng-select.custom .ng-select-container { background-color: #f8f8fa; color: #333; min-height: 25px !important; height: 25px !important; border-radius: 4px; box-shadow: 0 0 1px #eeeeee; }'],
  providers: [
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]

})
export class invstockComponent implements OnInit, OnDestroy {

  public lsstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  public lsratio: lov[] = new Array();
  public lsblock: lov[] = new Array();
  public crstock: stock_md = new stock_md();
  public lsstock: stock_ls[] = new Array();
  public instock: stock_info;
  public snstock: stock_info;

  public pm: stock_pm = new stock_pm();
  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;

  //Selection 
  public slproduct: stock_ls = new stock_ls();
  //Selection unit 
  public slratio: lov;
  public slcqtyratio: number = 1;
  public slcunit: string = "SKU";
  //Select block 
  public slcblock: lov;


  //Sorting 
  public lssort: string = "dateorder";
  public lsreverse: boolean = false; // for sorting
  //PageNavigate
  lsrowlmt: lov[] = new Array();
  slrowlmt: lov;
  page = 1;
  opageSize = 2000;
  pageSize = 200;
  //Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;
  //Tab
  crtab: number = 1;
  //List of value 
  lsspcare: lov[] = new Array(); slcopstype: lov; // Operate type: Stock, XDock, Forward
  //Toast
  toastRef: any;

  public artrowselect: number;
  public palrowselect: number;

  public enableEditState: boolean = true;
  constructor(private sv: inventoryService,
    private av: authService,
    private mv: adminService,
    private ss: shareService,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.ss.ngSetup(); this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.lsspcare = this.ss.getArea();
    this.lsblock.push({ value: "IO", desc: "Available", icon: "", valopnfirst: "", valopnfour: "", valopnsecond: "", valopnthird: "" });
    this.lsblock.push({ value: "XX", desc: "Block", icon: "", valopnfirst: "", valopnfour: "", valopnsecond: "", valopnthird: "" });

  }

  ngOnInit(): void { this.ss.ngSetup(); }
  SortOrder(value: string) { if (this.lssort === value) { this.lsreverse = !this.lsreverse; } this.lssort = value; }

  ngAfterViewInit() { this.ngSetup(); this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });
  }

  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
  decicon(o: string, m: string) {
    if (m == 'XX') { return 'fas fa-stop-circle fa-lg dp text-danger w25'; }
    else {
      switch (o) {
        case 'crincoming': return 'fas fa-ship fa-lg text-danger'
        case 'crplanship': return 'fas fa-file-alt fa-lg text-danger w25'
        case 'cronhand': return 'fas fa-heart fa-lg fn-second w25'
        case 'crprep': return 'fas fa-hand-paper fa-lg text-warning w25'
        case 'crsinbin': return 'fas fa-exclamation-triangle fa-lg text-danger w25'
        case 'cravailable': return 'fas fa-heartbeat fn-second fa-lg w25'
        case 'crstaging': return 'fas fa-truck-loading fa-lg text-warning w25'
        case 'crdamage': return 'fas fa-heart-broken fa-lg text-danger w25'
        case 'crbulknrtn': return 'fas fa-pallet tex-danger fa-lg w25'
        case 'crtask': return 'fas fa-dolly-flatbed fa-lg text-warning w25'
        case 'crblock': return 'fas fa-stop-circle fa-lg text-danger w25'
        case 'croverflow': return 'fas fa-life-ring fn-second fa-lg w25'
        case 'crrtv': return 'fas fa-industry fa-lg text-warning w25'
        case 'crexchange': return 'fas fa-exchange-alt fa-lg text-danger w25'
        case 'crpicking': return 'fas fa-heartbeat fn-second fa-lg w25'
        case 'crreserve': return 'fas fa-heartbeat fn-second fa-lg w25'
        default: return o;
      }
    }
  }
  desstate(o: string, m: string) {
    if (m == 'XX') { return 'Blocked'; }
    else {
      switch (o) {
        case 'crincoming': return 'Incoming'
        case 'crplanship': return 'Plan deliveiry'
        case 'cronhand': return 'on Hand'
        case 'crprep': return 'Preparation'
        case 'crsinbin': return 'Sinbin'
        case 'cravailable': return 'Available'
        case 'crstaging': return 'Staging'
        case 'crdamage': return 'Damage'
        case 'crbulknrtn': return 'Bulk'
        case 'crtask': return 'Task'
        case 'crblock': return 'Block'
        case 'croverflow': return 'Overflow'
        case 'crrtv': return 'Return'
        case 'crexchange': return 'Exchange'
        case 'crpicking': return 'Picking'
        case 'crreserve': return 'Reserve'
        default: return o;
      }
    }
  }
  enableEdit(typesel: string) {
    switch (typesel) {
      case 'crincoming': return false;//'Incoming'
      case 'crplanship': return false;//'Plan deliveiry'
      case 'cronhand': return false;//'on Hand'
      case 'crprep': return false;//'Preparation'
      case 'crsinbin': return false;//'Sinbin'
      case 'cravailable': return true;//'Available'
      case 'crstaging': return false;//'Staging'
      case 'crdamage': return false;//'Damage'
      case 'crbulknrtn': return false;//'Bulk'
      case 'crtask': return false;//'Task'
      case 'crblock': return true;//'Block'
      case 'croverflow': return true;//'Overflow'
      case 'crrtv': return false;//'Return'
      case 'crexchange': return false;//'Exchange'
      case 'crpicking': return false;//'Picking'
      case 'crreserve': return false;//'Reserve'
      default: false;
    }
  }
  getIcon(o: string) { return ""; }
  //toggle(){ $('.snapsmenu').click();  }
  decstate(o: string) { return this.lsstate.find(x => x.value == o).desc; }
  decstateicn(o: string) { return this.lsstate.find(x => x.value == o).icon; }
  dectype(o: string) { return this.lstype.find(x => x.value == o).desc; }

  fndproduct() {
    this.pm.isblock = (this.slcblock != null) ? this.slcblock.value : null;
    this.sv.findproduct(this.pm).subscribe(
      (res) => {
        this.lsstock = res;
        if (this.lsstock.length > 0) {
          if (this.lsstock.length > 10000) {
            this.pageSize = this.lsstock.length / 15;
          }
          this.getinfo(this.lsstock[0], 0);

        } else {
          this.toastr.warning("<span class='fn-07e'>Product not found with your cryteria </span>", null, { enableHtml: true });
        }
      }
    );
  }

  getinfo(o: stock_ls, ix: number) {
    this.artrowselect = ix;
    if (this.slproduct.article != o.article && o != undefined) { this.getratio(o); }
    this.slproduct = o;
    this.slcunit = "SKU";
    this.slratio = this.lsratio.find(e => e.valopnfirst == "1");
    this.sv.getstockInfo(o).subscribe(
      (res) => {
        this.instock = res;
        // snapshot stock
        this.snstock = Object.assign({}, res);
        this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);
        this.instock.lines.forEach(e => console.log(e.huno + ":" + e.qtysku));
        this.rqedit = 0;
        // default
        this.getline("cravailable");
        this.slratio = this.lsratio.find(e => e.valopnfirst == this.instock.unitmanage);

      },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  getline(typesel: string) {
    this.slproduct.huno = this.pm.huno;
    this.slproduct.loccode = this.pm.loccode;
    this.slproduct.inrefno = this.pm.inrefno;
    this.slproduct.dateexp = this.pm.dateexp;
    this.slproduct.serialno = this.pm.serialno;
    this.slproduct.isblock = this.pm.isblock;
    this.enableEditState = this.enableEdit(typesel);
    this.sv.getstockline(typesel, this.slproduct).subscribe(
      (res) => {
        this.instock.lines = new Array();
        this.instock.lines = res;
        this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);
        this.instock.lines.forEach(e => console.log(e.huno + ":" + e.qtysku));
        this.rqedit = 0;
      },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }

  slcstock(o: stock_md, ix: number) {
    this.palrowselect = ix;
    if (o.tflowsign == 'crtask' || o.tflowsign == 'crprep') {
      this.toastr.warning("<span class='fn-07e'>Stock has belonging of " + ((o.tflowsign == 'crtask') ? " Task movement" : " Preparation ") + "</span>", null, { enableHtml: true });
    } else {
      this.rqedit = 1;
      this.crstock = o;
      this.crstate = (o.tflow == "IO") ? true : false;
    }
  }
  ulcstock() {
    this.palrowselect = -1;
    this.rqedit = 0;
    this.crstock = new stock_md();
    this.crstate = false;
  }

  setstate() {
    this.ngPopups.confirm('Do you confirm to modify HU ' + this.crstock.huno + ' ?')
      .subscribe(res => {
        if (res) {
          this.crstock.tflow = (this.crstate == true) ? "IO" : "XX";
          this.sv.setstockInfo(this.crstock).subscribe((res) => {
            //this.instock = res; 
            //this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);
            this.ulcstock();
            this.toastr.success("<span class='fn-07e'>modify stock line success</span>", null, { enableHtml: true });
            this.getinfo(this.slproduct, this.artrowselect);
          },
          );
        }
      });
  }
  getlabel(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>", null, {
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass: 'toast-bottom-right', enableHtml: true
    });

    this.sv.getlabelhu(this.slproduct.orgcode, this.slproduct.site, this.slproduct.depot, o).subscribe(response => {
      let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_HU_" + o + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId);
    }),
      error => {
        this.toastr.clear(this.toastRef.ToastId);
      }
  }
  getratio(o: stock_ls) {
    this.sv.getproductratio(o.article, o.pv.toString(), o.lv.toString()).subscribe(
      (res) => { this.lsratio = res; },
      (err) => { this.toastr.error("<span class='fn-08e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }



  selectunit() {
    this.slcqtyratio = Number(this.slratio.value);
    let _activeratio = Number(this.slratio.value);
    // let skuqty = Number(this.cvunit(this.slproduct.cronhandpu,1));
    this.instock.crincoming = Number(this.snstock.crincoming) / _activeratio;
    this.instock.crplanship = Number(this.snstock.crplanship) / _activeratio;
    this.instock.cronhand = Number(this.snstock.cronhand) / _activeratio;
    this.instock.crprep = Number(this.snstock.crprep) / _activeratio;
    this.instock.crsinbin = Number(this.snstock.crsinbin) / _activeratio;
    this.instock.cravailable = Number(this.snstock.cravailable) / _activeratio;
    this.instock.crstaging = Number(this.snstock.crstaging) / _activeratio;
    this.instock.crdamage = Number(this.snstock.crdamage) / _activeratio;
    this.instock.crbulknrtn = Number(this.snstock.crbulknrtn) / _activeratio;
    this.instock.crtask = Number(this.snstock.crtask) / _activeratio;
    this.instock.crblock = Number(this.snstock.crblock) / _activeratio;
    this.instock.croverflow = Number(this.snstock.croverflow) / _activeratio;
    this.instock.crrtv = Number(this.snstock.crrtv) / this.slcqtyratio;
    this.instock.crexchange = Number(this.snstock.crexchange) / _activeratio;

    if (['Prep', 'Recv'].includes(this.slratio.desc)) {
      this.slcunit = this.ss.ngDecUnitstock(this.slratio.valopnfirst);
    } else {
      this.slcunit = this.slratio.desc;
    }
  }
  cvunit(sku: number, ratio: number) {
    //return  (sku == 0 || sku == null) ? 0 : Number( Number(sku.toFixed(3)) / this.slcqtyratio); 
    return (sku == 0 || sku == null || ratio == 0 || ratio == null) ? 0 : Number(Number(sku.toFixed(3)) / parseInt(ratio.toString())).toFixed(3)
  }

  // isEditState(stocktype) {
  //   let enableEditState = false;
  //   if(stocktype=="available"||stocktype =="overflow"){
  //     enableEditState = true; 
  //   }
  //   return enableEditState;
  // }


  ngDecunit(o: string) {
    return this.ss.ngDecUnitstock(o);
  }
  ngDecIcon(o) { return this.ss.ngDecIcon(o); }
  ngDecStr(o) { return this.ss.ngDecStr(o); }
  ngSetup() {
    this.lsrowlmt = this.ss.getRowlimit();
    if (this.lsrowlmt.length == 0) { this.ss.ngIntRowlimit().subscribe((res) => { this.lsrowlmt = res.sort((a, b) => parseInt(a.value) - parseInt(b.value)); }); this.slrowlmt = this.lsrowlmt.find(e => e.value == this.pageSize.toString()); }
    else { this.slrowlmt = this.lsrowlmt.find(e => e.value == this.pageSize.toString()); }
  }
  ngOnDestroy(): void {
    this.lsstate = null; delete this.lsstate;
    this.lstype = null; delete this.lstype;
    this.crstock = null; delete this.crstock;
    this.lsstock = null; delete this.lsstock;
    this.instock = null; delete this.instock;
    this.pm = null; delete this.pm;
    this.instocksum = null; delete this.instocksum;
    this.rqedit = null; delete this.rqedit;
    this.crstate = null; delete this.crstate;
    //Selection 
    this.slproduct = null; delete this.slproduct;
    //Sorting 
    this.lssort = null; delete this.lssort;
    this.lsreverse = null; delete this.lsreverse;
    //PageNavigate
    this.lsrowlmt = null; delete this.lsrowlmt;
    this.slrowlmt = null; delete this.slrowlmt;
    this.page = null; delete this.page;
    this.pageSize = null; delete this.pageSize;
    //Date format
    this.dateformat = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.datereplan = null; delete this.datereplan;
    //Tab
    this.crtab = null; delete this.crtab;
    //List of value 
    this.lsspcare = null; delete this.lsspcare;
  }

}
