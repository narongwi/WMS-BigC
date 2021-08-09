import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from '../../../helpers/ngx-bootstrap.config';
import { shareService } from '../../../share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { prepset } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';
import { oubprocessdistoperateComponent } from './oub.process.dist.operate';
import { oubprocessdistselectionComponent } from './oub.process.dist.selection';
import { oubprocessdistsummaryComponent } from './oub.process.dist.summary';

declare var $: any;
@Component({
  selector: 'appoub-processdist',
  templateUrl: 'oub.process.dist.landing.html',
  styles: ['.dgorder {  height:250px !important;','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class oubprocessdistComponent implements OnInit {

    @ViewChild(oubprocessdistoperateComponent) choperate: oubprocessdistoperateComponent;
    @ViewChild(oubprocessdistselectionComponent) chselection: oubprocessdistselectionComponent;
    @ViewChild(oubprocessdistsummaryComponent) chsummary: oubprocessdistsummaryComponent;

    public lsstate:lov[] = new Array();
    public lstype:lov[] = new Array();
    public instocksum:number = 0;
    public rqedit:number = 0;
    public crstate:boolean = false;

    public lsorder:outbound_ls[] = new Array();
    public slcorder:outbound_md;
    public slclines:outbouln_md[] = new Array();
    public slclineso:number = 0;
    public slcliness:number = 0;
    public pm: outbound_pm = new outbound_pm();

    crtab:number = 1;
    crorder:string = "";

    //Date format
    public dateformat:string;
    public dateformatlong:string;
    public datereplan: Date | string | null;

    //Sorting 
    public lssort:string = "spcarea";
    public lsreverse: boolean = false; // for sorting

    //PageNavigate
    page = 4;
    pageSize = 200;
    slrowlmt:lov;
    lsrowlmt:lov[] = new Array();

    lsunit:lov[] = new Array(); //unit list

  public lshandle: lov[] = new Array();
    constructor(private sv: outboundService,
                private pv: ouprepService,
                private av: authService, 
                private mv: shareService,
                private router: RouterModule,
                private toastr: ToastrService,                
                private ngPopups: NgPopupsService,) { 
      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong;
      
    }

    ngOnInit(): void { }
    ngOnDestroy():void {  }
    ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/this.getmaster();  this.fnd(); }
    setupJS() { 
        // sidebar nav scrolling
        $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' });   
    }
    getIcon(o:string){ return "";  }
    //toggle(){ $('.snapsmenu').click();  }
    decstate(o:string){ return this.lsstate.find(x=>x.value == o).desc; }
    decstateicn(o:string){ return this.lsstate.find(x=>x.value == o).icon; }
    dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }

    selorder(o:outbound_ls) { this.choperate.selorder(o); }
    selorderall(o:outbound_ls[]) { this.choperate.selorderall(o); }

    getmaster(){ 
      this.mv.lov("OUBORDER","FLOW").pipe().subscribe(
          (res) => { this.lsstate = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
        this.mv.lov("DATAGRID","ROWLIMIT").pipe().subscribe(
          (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
          (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
        this.mv.lov("TASK","TYPE").pipe().subscribe(
          (res) => { this.lstype = res;  },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
        this.mv.lov("UNIT","KEEP").pipe().subscribe(
          (res) => { this.lsunit = res;  },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );  
  }
  fnd(){ 
      this.sv.find(this.pm).subscribe(            
        (res) => { 
            this.lsorder = res;
            this.lsorder.forEach(x=>x.selc = false);
            
          },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
  }
  getinfo(o:outbound_ls){ 
    this.sv.get(o).subscribe(            
      (res) => { 
        this.slcorder = res; this.slclines = res.lines; 
        this.slclineso = this.slclines.reduce((obl, val) => obl += val.qtysku, 0); 
        this.slcliness = this.slclines.reduce((obl, val) => obl += val.qtystock, 0);
        o.tflow = (this.slcliness >= this.slclineso) ? "RP" : "NE";
      },
      (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { } 
    );
  }

  selecprocess(o:prepset){ 
  //selecprocess(o:outbound_ls[]){ 
    this.crtab = 3
    this.chsummary.startprocess(o);
  }




}
