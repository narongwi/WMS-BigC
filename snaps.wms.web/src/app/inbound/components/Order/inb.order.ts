import { Component, OnInit,OnDestroy, ViewChild, Injectable, ElementRef  } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { adminService } from 'src/app/admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { inbound_md } from '../../models/mdl-inbound';
import { inborderreceiptComponent } from './inb.order.receipt';
import { NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct, NgbInputDatepickerConfig, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter,CustomDateParserFormatter } from '../../../helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { inborderlineComponent } from './inb.order.line';

declare var $: any;
@Component({
  selector: 'appinb-order',
  templateUrl: 'inb.order.html',
  styles: ['.dgorder { height:calc(100vh - 645px) !important;','.dglines { height:365px !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class inborderComponent implements OnInit, OnDestroy {
  @ViewChild('inboundorderoperate') ops:inborderreceiptComponent;
  @ViewChild('inboundorderline') lne:inborderlineComponent;
  crtab:number = 1;
  crorder:string = "";

  //Toast
  toastRef:any;
  //Date Format



  constructor(private ss: shareService,
              private av: authService,
              private mv: adminService,
              private router: RouterModule,
              private toastr: ToastrService,) { 
    this.av.retriveAccess(); 

    

  }

  ngOnInit(): void {  }

  ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }

  setupJS() { 
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });   
  }
  toggle(){ $('.snapsmenu').click(); }
  
  // seelct 0rder line
  selorder(o:inbound_md){  
    this.ops.selorder(o); this.crtab = 2; this.crorder = o.inorder;
  }
  
  selfnd(){ 
    this.lne.fndorder();
  }


  ngOnDestroy():void {  


  }
}
