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

declare var $: any;
@Component({
  selector: 'appoub-processdist-summary',
  templateUrl: 'oub.process.dist.summary.html',
  styles: ['.dgsummary {  height:calc(100vh - 235px) !important; '],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class oubprocessdistsummaryComponent implements OnInit {
    //Date format
    public dateformat:string;
    public dateformatlong:string;
    public datereplan: Date | string | null;
    public pm: outbound_pm = new outbound_pm();
    //Sorting 
    public lssort:string = "spcarea";
    public lsreverse: boolean = false; // for sorting
    //PageNavigate
    page = 4;
    pageSize = 200;
    slrowlmt:lov;
    lsrowlmt:lov[] = new Array();
    lsunit:lov[] = new Array(); //unit list

    //flag for revise order line 
    public rqedit:number = 0;
    //flag remarks 
    public chnremark:number = 0;

    //Process order object
    public proc:prepset = new prepset();
    //public proc:outbound_ls[] = new Array();

    //LOV state 
    public lsstate:lov[] = new Array();

    //Process status
    public isonProcess:number = 0;
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
    ngOnInit(): void { this.getmaster(); }
    ngOnDestroy():void {  }
    //Decode
    ngDecIcon(o:string){ return this.lsstate.find(x=>x.value == o).icon; } 
    ngDecStr(o:string,e:string) { return (e!='') ? e : this.lsstate.find(x=>x.value == o).desc;}
    /* get master data */
    getmaster(){ 
      this.mv.lov("ALL","FLOW").pipe().subscribe(
          (res) => { this.lsstate = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
      );
      this.mv.lov("DATAGRID","ROWLIMIT").pipe().subscribe(
        (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.lov("UNIT","KEEP").pipe().subscribe(
        (res) => { this.lsunit = res;  },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );  
    }
    
    /* Row limit changing */
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } 

    startprocess(o:prepset){ 
      this.proc = o;
      this.proc.spcarea = "XD";
      this.proc.procmodify = "prep.distsetup";
      this.proc.opsorder = this.proc.orders.length;
      
      this.pv.distsetup(this.proc).pipe().subscribe(
        (res) => { 
          this.proc = res;
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );  

    }

    launchprocess(){ 
      this.ngPopups.confirm('Do you confirm to generate plan ?')
      .subscribe(res => {
          if (res) {
            this.isonProcess = 1;
            this.pv.procdistb(this.proc.setno).pipe().subscribe(
              (res) => { 
                //this.proc = res;
                this.isonProcess = 0;
                this.toastr.success("<span class='fn-1e15'>Generate plan success </span>",null,{ enableHtml : true }); 
              },
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true }); this.isonProcess = 0; },
              () => { }
            ); 
          } 
      });


    }
    
} 