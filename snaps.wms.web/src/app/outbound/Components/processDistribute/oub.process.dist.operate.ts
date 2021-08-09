import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from '../../../helpers/ngx-bootstrap.config';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';
import { prepset, prepsln } from '../../Models/oub.prep.mode';
import { ouprepService } from '../../Services/oub.prep.service';
import { outboundService } from '../../Services/oub.service';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'appoub-processdist-operate',
  templateUrl: 'oub.process.dist.operate.html',
  styles: ['.dgorder {  height:250px !important;','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class oubprocessdistoperateComponent implements OnInit {
  @Output() selecProcess = new EventEmitter();
  
    //List of state
    public lsstate:lov[] = new Array();
    //List of order 
    public lsorder:outbound_ls[] = new Array();
    //Order object
    public slcorder:outbound_md;
    public slclines:outbouln_md[] = new Array();
    public slcline:outbouln_md;

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

    //Change request date 
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
    ngOnInit(): void { this.ngGetmaster(); }
    ngOnDestroy():void {  }
    ngGetmaster():void { 
      this.mv.getlov("DATAGRID","ROWLIMIT").pipe().subscribe(
        (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }

    selorder(o:outbound_ls){ 
      if (this.lsorder.filter(x=>x.dishuno == o.dishuno).length == 0) { 
        this.lsorder.push(o);
        this.getinfo(o,true);
      } else { 
        this.lsorder = this.lsorder.filter(x=>x.dishuno != o.dishuno);
        this.proc.distb = this.proc.distb.filter(e=>e.dishuno != o.dishuno);
      }
    }
    selorderall(o:outbound_ls[]){
      this.lsorder = [];
      this.proc.distb = [];
      if (o.length > 0) { 
        o.forEach(e=> { this.lsorder.push(e); this.proc.distb.push(e); }); 
        this.getinfo(o[o.length],true);
      }
      
    }

    getinfo(o:outbound_ls, e:boolean = false){ 
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
          this.slcorder = res; 
          this.slclines = res.lines;
          if(e){ this.proc.distb.push(o); }
        },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    dsctoprocln(thcode:string, s:outbouln_md) { 
      console.log(s);
      var t = new prepsln();
      t.orgcode = s.orgcode;
      t.site = s.site;
      t.depot = s.depot;
      t.spcarea = "XD";
      t.routeno = "";
      t.thcode = thcode;
      t.ouorder = s.ouorder;
      t.ouln = s.ouln;
      t.barcode = s.barcode;
      t.article = s.article;
      t.pv = s.pv;
      t.lv = s.lv;
      t.unitprep = s.unitops;
      t.qtyskuorder = s.qtysku;
      t.qtypuorder = s.qtyreqpu;
      t.qtyweightorder = s.qtyweight;
      t.qtyvolumeorder = 0;
      t.qtyskuops = 0;
      t.qtypuops = 0;
      t.qtyweightops = 0;
      t.qtyvolumeops = 0;
      t.batchno = s.batchno;
      t.lotno = s.lotno;
      t.datemfg = s.datemfg;
      t.dateexp = s.dateexp;
      t.serialno = s.serialno;
      t.description = s.articledsc;
      return t;
    }
    setedit(o:outbouln_md) { this.rqedit = 1; this.slcline = o; }
    flagremarks() {  this.chnremark = (this.chnremark == 0) ? 1 : 0;}
    /* Row limit changing */
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } 

    changereqdate(){
      this.ngPopups.confirm('Do you confirm change request delivery date')
      .subscribe(res => {
          if (res) {
            // this.crstock.tflow = (this.crstate == true) ? "IO" : "XX";
            this.sv.changeRequest(this.slcorder).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-1e15'>Change request delivery date success</span>",null,{ enableHtml : true }); 
              },
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
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

    setlineorder(){
      this.ngPopups.confirm('Do you confirm to set info on order line ?')
      .subscribe(res => {
          if (res) {
            this.sv.setlineorder(this.slcline).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'>set line info success</span>",null,{ enableHtml : true }); 
                this.rqedit = 0;
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                 
          } 
      });
    }

    ulcorder(){ 
      this.rqedit = 0;
    }

    preprocessorder() { 
      this.selecProcess.emit(this.proc); 
    }
  
    ngDecUnitstock(o:string) { return this.mv.ngDecUnitstock(o); } 
}