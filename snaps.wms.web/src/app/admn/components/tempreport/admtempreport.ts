import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { correction_md, correction_pm } from 'src/app/inventory/models/inv.correction.mode';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { AdmReportService } from "../../../admn/services/adm.report.service";
import { NgForm } from '@angular/forms';
import { NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { DatePipe } from "@angular/common";
declare var $: any;
@Component({
  selector: 'admtempreport',
  templateUrl: 'admtempreport.html',
  styles: ['.dgproduct {  height:calc(100vh - 195px) !important; } ', '.dgstockline { height:calc(100vh - 470px) !important; } ', '.dgcorrect { height:calc(100vh - 195px) !important; } '],
  providers: [
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]
})
export class tempreportComponent implements OnInit {
  public lsstate: lov[] = new Array();
  public lscode: lov[] = new Array();
  public crcode: lov = new lov();
  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;

  pm: correction_pm = new correction_pm();
  crcort: correction_md = new correction_md();

  //Sorting 
  public lssort: string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //PageNavigate
  lsrowlmt: lov[] = new Array();
  slrowlmt: lov;
  page = 4;
  pageSize = 200;
  //Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;
  //Tab
  crtab: number = 1;

  //is Correction In
  isnonhu: number = 1;
  //List correction for select 
  lscorsel: lov[] = new Array();

  // reports 

  public reports: any[] = [];
  public forms: any[] = [];
  public rpslId: string;
  public rpslCode: string;
  public rpslName: string;
  public rpsOpt: string;
  public rpsFind: string;
  public reprowselect:number;
  constructor(private av: authService,
    private mv: adminService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,
    private datepipe: DatePipe,
    private rpt: AdmReportService) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void {
    // list all report 
    this.rpt.lists().subscribe((res: any) => {
      if (res.success) {
        this.reports = res.data;
      } else {
        console.log(res.message)
      }
    }, error => { console.log() })

  }

  selection(id, code, name, form,ix:number) {
    this.rpslId = id;
    this.rpslCode = code;
    this.rpslName = name;
    this.forms = form;
    this.reprowselect = ix;
  }

  onSubmit(f: NgForm) {
    this.rpt.Export(f.value).subscribe((res: any) => {
      const typeAccept = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
      var exportElement = document.createElement('a');
      const excel = new Blob([res], { type: typeAccept });
      const url = window.URL.createObjectURL(excel);
      const dateExport = this.datepipe.transform(new Date(), 'dd-MM-yyyy hhmm');
        // xx
      // Exporting
      exportElement.href = url;
      exportElement.download = this.rpslName +"_" + dateExport + ".xlsx";
      exportElement.target = "_blank";
      document.body.appendChild(exportElement);
      exportElement.click();

      // Clear Element in body
      document.body.removeChild(exportElement);
      URL.revokeObjectURL(url);
    }, error => {
      console.log(error)
    });
  }

  onChanaged(d) {
    console.log(d);
  }
  format(date: NgbDateStruct): string {
    if (date === null) {
      return '';
    }
    try {
      return date.day + "/" + date.month + "/" + date.day;
    } catch (e) {
      return '';
    }
  }

  ngOnDestroy(): void { }
  ngAfterViewInit() { this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });
  }
  getIcon(o: string) { return ""; }
  toggle() { $('.snapsmenu').click(); }
  decstate(o: string) {
    return o.replace("cr", "").toString();
  }
  decstateicn(o: string) {
    switch (o) {
      case 'crincoming': return 'fas fa-ship text-danger w25';
      case 'crplanship': return 'fas fa-file-alt text-danger w25';
      case 'crall': return 'fas fa-heart fn-second w25';
      case 'crprep': return 'fas fa-hand-paper text-warning w25';
      case 'cravailable': return 'fas fa-heartbeat fn-second w25';
      case 'crstaging': return 'fas fa-truck-loading text-warning w25';
      case 'crdamage': return 'fas fa-heart-broken text-danger w25';
      case 'crbulknrtn': return 'fas fa-pallet fn-second w25';
      case 'crtask': return 'fas fa-dolly-flatbed text-warning w25';
      case 'crblock': return 'fas fa-stop-circle text-danger w25';
      case 'croverflow': return 'fas fa-life-ring fn-second w25';
      case 'crrtv': return 'fas fa-industry text-warning w25';
      case 'crexchange': return 'fas fa-exchange-alt text-danger w25';
      case 'crpicking': return 'fas fa-hand fn-second w25';
      case 'crreserve': return 'fas fa-pallet fn-second w25';
      default: return '';
    }
  }
}
