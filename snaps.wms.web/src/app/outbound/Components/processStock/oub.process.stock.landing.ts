import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { textChangeRangeIsUnchanged } from 'typescript';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { prepset } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';
import { oubprocessstockoperateComponent } from './oub.process.stock.operate';
import { oubprocessstockselectionComponent } from './oub.process.stock.selection';
import { oubprocessstocksummaryComponent } from './oub.process.stock.summary';

declare var $: any;
@Component({
  selector: 'appoub-processstock',
  templateUrl: 'oub.process.stock.landing.html',
  styles: ['.dgorder {  height:250px !important;', '.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }]

})
export class oubprocessstockComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild(oubprocessstockoperateComponent) choperate: oubprocessstockoperateComponent;
  @ViewChild(oubprocessstockselectionComponent) chselection: oubprocessstockselectionComponent;
  @ViewChild(oubprocessstocksummaryComponent) chsummary: oubprocessstocksummaryComponent;
  pm: outbound_pm = new outbound_pm();
  crtab: number = 1;
  //Date format
  dateformat: string;
  dateformatlong: string;
  datereplan: Date | string | null;
  constructor(private av: authService, private mv: shareService) {
    //this.mv.ngSetup();
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;

  }

  ngOnInit(): void { }

  ngAfterViewInit() { 
    this.getmaster(); 
    this.setupJS(); 
    /*setTimeout(this.toggle, 1000);*/ 
  }
  setupJS() { 
    $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' }); 
  }
  toggle() { $('.snapsmenu').click(); }

  // Tab 1 Select Order
  selorder(o: outbound_ls) {
    // tab 2 push item , remove item
    this.choperate.selorder(o);
    this.chsummary.clearSelected();
  }

  refresh(isrefresh) {
    if (isrefresh) {
      this.choperate.clearSelorder();
      //this.chsummary.clearSelected();
    }
  }


  getmaster() {
    // Promise.all([
    //   this.mv.getlov("OUBORDER","FLOW").toPromise(), 
    //   this.mv.getlov("ORDER","SUBTYPE").toPromise(),
    //   this.mv.lovms("OUBORDER","FLOW").toPromise()
    // ]).then(res=> {
    //   console.log(res);
    //   this.lsstate = res[0];
    //   this.lstype = res[1].filter(e=>['OU','INOU'].includes(e.valopnfour));
    //   this.lsstatem = res[2];
    //   this.lsspcarea = this.mv.getArea();
    //   this.lsyesno = this.mv.getYesno();
    // })
    this.mv.getlov("OUBORDER", "FLOW").pipe().subscribe((res) => {
      this.choperate.lsstate = res;
      this.chselection.lsstate = res;
      this.chsummary.lsstate = res;
    });
    this.mv.getlov("ORDER", "SUBTYPE").pipe().subscribe((res) => {
      this.choperate.lssubtype = res.filter(e=>['OU','INOU'].includes(e.valopnfour));
      this.chselection.lssubtype = res.filter(e=>['OU','INOU'].includes(e.valopnfour));
      this.chsummary.lssubtype = res.filter(e=>['OU','INOU'].includes(e.valopnfour));
    });
    this.mv.ngIntRowlimit().pipe().subscribe((res) => {
      res = res.sort((a, b) => parseInt(a.value) - parseInt(b.value));
      this.choperate.lsrowlmt = res; this.choperate.slrowlmt = this.choperate.lsrowlmt.find(e => parseInt(e.value) == this.choperate.pageSize);
      this.chselection.lsrowlmt = res; this.chselection.slrowlmt = this.chselection.lsrowlmt.find(e => parseInt(e.value) == this.chselection.pageSize);
      this.chsummary.lsrowlmt = res; this.chsummary.slrowlmt = this.chsummary.lsrowlmt.find(e => parseInt(e.value) == this.chsummary.pageSize);
    });

    this.mv.ngIntArea().pipe().subscribe((res) => {
      this.chselection.lsspcarea = res.filter(x => x.value == "ST");
    });
    this.chselection.lsspcarea = this.mv.getArea();
  }


  // select order on tab 1
  selecprocess(o: prepset) {
    this.chsummary.startprocess(o);
    // Next focus tab 3
    this.crtab = 3
  }

  processFinish(isfinsih: boolean) {
    if (isfinsih) {
      // tab 1
      this.chselection.clearSelorder();

      // tab 2
      this.choperate.clearSelorder();

      // this.chselection.clearSelorder();
      console.log("Tab 3 Process Finished");
    }
  }
  ngOnDestroy() {
    this.pm = null; delete this.pm;
    this.crtab = null; delete this.crtab;
    this.dateformat = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
  }
}
