import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
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
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';
import { route_thsum } from '../../Models/oub.route.model';

declare var $: any;
@Component({
  selector: 'appoub-processdist-selection',
  templateUrl: 'oub.process.dist.selection.html',
  styles: ['.dgorder {  height:250px !important;','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class oubprocessdistselectionComponent implements OnInit, OnDestroy {
   @Output() selectOrder = new EventEmitter();
   @Output() selectOrderall = new EventEmitter();

    //List of state
    public lsstate:lov[] = new Array();
    //List of order 
    public lsorder:outbound_ls[] = new Array();
    //List of subtype
    public lssubtype:lov[] = new Array();
    //Paramater
    public pm: outbound_pm = new outbound_pm();
    public slcorder: outbound_md; //Object 
    public slclines: outbouln_md[] = new Array();; //lines
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

    /* Requst Edit */
    rqedit:number = 0;

    public chnremark:number = 0;
    public chnrqdate:number = 0;
    public slcinorder:string;
    public slchuno:string;
    public slcunitprep:string;
    public slcproduct:string = "";
    public slcprodesc:string = "";
    public slcthcode:string = "";
    public slcthname:string = "";
    public slcdateplan:Date | string;
    public slcdateexp:Date | string;
    public disqtypnd:number;

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
    ngOnInit(): void { this.getmaster(); this.findorder(); }
    decstate(o:string){ return this.lsstate.find(x=>x.value == o).desc; }

    /* Order selection */
    selorder(o:outbound_ls) { o.selc = (o.selc == true) ? false : true; this.selectorder(o); }  
    public selectorder(o:outbound_ls) { this.selectOrder.emit(o); }
    public selectorderall(o:outbound_ls[]) { this.selectOrderall.emit(o); }
    
    /* Row limit changing */
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } 

    /* Flag for remark */
    flagremarks() {  this.chnremark = (this.chnremark == 0) ? 1 : 0;}

    /* get master data */
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
        this.mv.lov("UNIT","KEEP").pipe().subscribe(
          (res) => { this.lsunit = res;  },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        ); 
        this.mv.lov("ORDER","SUBTYPE").pipe().subscribe(
          (res) => { this.lssubtype = res;  },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
  }
  findorder(){ 
      this.pm.spcarea = "XD";
      this.sv.listdist(this.pm).subscribe(            
        (res) => { 
            this.lsorder = res;
            this.lsorder.forEach(x=>{ 
              x.selc = false;
              try{ x.ousubtypedesc = this.lssubtype.find(e=>e.value == x.ousubtype).desc } catch (exp){ x.ousubtypedesc = x.ousubtype; }            
            } );
            
          },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
  }
  setremarks(){
    this.ngPopups.confirm('Do you confirm to set remarks of an order  ?')
    .subscribe(res => {
        if (res) {
          this.sv.setremarks(this.slcorder).subscribe(            
            (res) => { 
              this.toastr.success("<span class='fn-07e'>set remarks success</span>",null,{ enableHtml : true }); this.chnremark = 0;
            },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true }); this.chnremark = 0; },
            () => { } 
          );                 
        } 
    });
  }
  setpriority(){
    this.ngPopups.confirm('Do you confirm to set priority of an order  ?')
    .subscribe(res => {
        if (res) {
          this.sv.setpriority(this.slcorder).subscribe(            
            (res) => { 
              this.toastr.success("<span class='fn-07e'>modify stock line success</span>",null,{ enableHtml : true }); 
            },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { } 
          );                 
        } 
    });
  }
  
  getinfo(o:outbound_ls){ 
    this.slcinorder = o.ouorder;
    this.slchuno = o.dishuno;
    this.slcunitprep = this.ngDecUnitstock(o.disunitops);
    this.slcprodesc = o.disproductdesc;
    this.slcthcode = o.thcode;
    this.slcthname = o.thname;
    this.slcdateexp = o.dateexpire;
    this.slcdateplan = o.dateprep
    this.slcproduct = o.disproduct;   
    this.disqtypnd = o.disqtypnd; 
    this.sv.getdist(o).subscribe(            
      (res) => { 
        this.slclines = res.lines;
      },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { } 
    );
  }


  ngLnselc(ln:outbound_ls) { this.selorder(ln);  }
  ngLnselcAll() { 
    if (this.lsorder.length > 0) { 
      if(this.lsorder[0].selc == true){ 
        this.lsorder.forEach(e=>e.selc = false);
      }else { 
        this.lsorder.forEach(e=>e.selc = true);
      }
      this.selectorderall(this.lsorder);
    }

    //send selection
    
  }

  ngDecUnitstock(o:string) { return this.mv.ngDecUnitstock(o); } 


  ngOnDestroy() : void { 
    this.lsstate         = null; delete this.lsstate;
    this.lsorder         = null; delete this.lsorder;
    this.lssubtype       = null; delete this.lssubtype;
    this.pm              = null; delete this.pm;
    this.slcorder        = null; delete this.slcorder;
    this.slclines        = null; delete this.slclines;
    this.dateformat      = null; delete this.dateformat;
    this.dateformatlong  = null; delete this.dateformatlong;
    this.datereplan      = null; delete this.datereplan;
    this.lssort          = null; delete this.lssort;
    this.lsreverse       = null; delete this.lsreverse;
    this.page            = null; delete this.page;
    this.pageSize        = null; delete this.pageSize;
    this.slrowlmt        = null; delete this.slrowlmt;
    this.lsrowlmt        = null; delete this.lsrowlmt;
    this.lsunit          = null; delete this.lsunit;
    this.rqedit          = null; delete this.rqedit;
    this.chnremark       = null; delete this.chnremark;
    this.chnrqdate       = null; delete this.chnrqdate;
    this.slcinorder      = null; delete this.slcinorder;
    this.slchuno         = null; delete this.slchuno;
    this.slcunitprep     = null; delete this.slcunitprep;
    this.slcproduct      = null; delete this.slcproduct;
    this.slcprodesc      = null; delete this.slcprodesc;
    this.slcthcode       = null; delete this.slcthcode;
    this.slcthname       = null; delete this.slcthname;
    this.slcdateplan     = null; delete this.slcdateplan;
    this.slcdateexp      = null; delete this.slcdateexp;
    this.disqtypnd       = null; delete this.disqtypnd;
  }
}