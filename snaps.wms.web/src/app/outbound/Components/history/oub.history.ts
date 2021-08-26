import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from '../../../../app/helpers/ngx-bootstrap.config';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { outboundService } from '../../Services/oub.service';
import { route_ls, route_md, route_pm } from '../../Models/oub.route.model';
import { ourouteService } from '../../Services/oub.route.service';
declare var $: any;

@Component({
  selector: 'appoub-history',
  templateUrl: 'oub.history.html',
  styles: ['.dgroute { height:calc(100vh - 235px) !important; ', '.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]
})

export class oubhistoryComponent implements OnInit {
  public lsstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  public instocksum: number = 0;
  public rqedit: number = 0;
  public crstate: boolean = false;
  public slchuline: any[] = [];
  public lsorder: outbound_ls[] = new Array();
  public slcorder: outbound_md;
  public slclines: outbouln_md[] = new Array();
  public slclineso: number = 0;
  public slcliness: number = 0;
  public slcline: outbouln_md;
  public pmroute: route_pm = new route_pm();

  crtab: number = 1;
  crorder: string = "";

  //Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;

  //Sorting 
  public lssort: string = "spcarea";
  public lsreverse: boolean = false; // for sorting

  //PageNavigate
  page = 4;
  pageSize = 200;
  slrowlmt: lov;
  lsrowlmt: lov[] = new Array();

  lsunit: lov[] = new Array(); //unit list

  chnrqdate: number = 0;
  chnremark: number = 0;

  public lsroute: route_ls[] = new Array();
  public routesource: route_md = new route_md();

  constructor(private sv: ourouteService,
    private av: authService,
    private mv: adminService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() { this.setupJS(); /*setTimeout(this.toggle, 1000);*/this.getmaster(); this.fnd(); }
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' });
  }
  getIcon(o: string) { return ""; }
  //toggle(){ $('.snapsmenu').click();  }
  decstate(o: string) { return this.lsstate.find(x => x.value == o).desc; }
  decstateicn(o: string) { return this.lsstate.find(x => x.value == o).icon; }
  dectype(o: string) { return this.lstype.find(x => x.value == o).desc; }
  changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

  setreqdate() { this.chnrqdate = (this.chnrqdate == 0) ? 1 : 0; }
  flagremarks() { this.chnremark = (this.chnremark == 0) ? 1 : 0; }


  getmaster() {
    this.mv.getlov("OUBORDER", "FLOW").pipe().subscribe(
      (res) => { this.lsstate = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
    this.mv.getlov("DATAGRID", "ROWLIMIT").pipe().subscribe(
      (res) => { this.lsrowlmt = res.sort((a, b) => parseInt(a.value) - parseInt(b.value)); this.slrowlmt = this.lsrowlmt.find(x => x.value == this.pageSize.toString()); },
      (err) => { this.toastr.error("<span class='fn-07e'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
    this.mv.getlov("TASK", "TYPE").pipe().subscribe(
      (res) => { this.lstype = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
    this.mv.getlov("UNIT", "KEEP").pipe().subscribe(
      (res) => { this.lsunit = res; },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  fnd() {
    this.sv.find(this.pmroute).subscribe(
      (res) => {
        this.lsroute = res;
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }
  getinfo(o: route_ls) {
    this.sv.get(o).subscribe(
      (res) => {
        this.routesource = res;
        this.routesource.thname = o.thname;
        // this.routesource.routetypename = this.lsroutetype.find(x => x.value == this.routesource.routetype).desc;
        // this.routesource.transportor = this.lstransportor.find(x => x.value == this.routesource.transportor).desc;
        // this.routesource.loccode = this.lsstaging.find(x => x.value == this.routesource.loccode).desc;
        // this.routesource.trucktype = this.lstrucktype.find(x => x.value == this.routesource.trucktype).desc;
        // this.routesource.loadtype = this.lsloadtype.find(x => x.value == this.routesource.loadtype).desc;
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>" + ((err.error == undefined) ? err.message : err.error.message) + "</span>", null, { enableHtml: true }); },
      () => { }
    );
  }



}