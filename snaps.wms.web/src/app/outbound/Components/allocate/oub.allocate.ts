import { ArrayType, ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { StringLiteralLike } from 'typescript';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { task_pm } from '../../../task/Models/task.movement.model';
import { route_hu, route_ls, route_md, route_pm } from '../../Models/oub.route.model';
import { ourouteService } from '../../Services/oub.route.service';
import { outboundService } from '../../Services/oub.service';

declare var $: any;
@Component({
  selector: 'appoub-allocate',
  templateUrl: 'oub.allocate.html',
  styles: ['.dgroute { height:calc(100vh - 720px) !important; }','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class ouballocateComponent implements OnInit,OnDestroy {
    public lsstate:lov[] = new Array();
    public lstype:lov[] = new Array();
    public instocksum:number = 0;
    public rqedit:number = 0;
    public crstate:boolean = false;

    public lsroutesource:route_ls[] = new Array();
    public lsroutetarget:route_ls[] = new Array();
    public pm:route_pm = new route_pm();
    public tg:route_pm = new route_pm();


    public routesource:route_md = new route_md;
    public routetarget:route_md = new route_md();

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

    public lsroutetype:lov[] = new Array();
    public lsloadtype:lov[] = new Array();
    public lspayment:lov[] = new Array();
    public lstrucktype:lov[] = new Array();
    public lstrtmode:lov[] = new Array();
    public lstransportor:lov[] = new Array();
    public srcrowselect:number;
    public tarrowselect:number; 
    public sumsrc:routesummary = {qty:0,weight:0,volume:0};
    public sumtar:routesummary = {qty:0,weight:0,volume:0};
    public sourceStore:string;
    public targetStore:string;

    constructor(private sv: ourouteService,
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

    ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/this.getMaster(); this.fnd(); }
    setupJS() { 
        // sidebar nav scrolling
        $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' });   
    }
    getIcon(o:string){ return "";  }
    //toggle(){ $('.snapsmenu').click();  }
    decstate(o:string){ return this.lsstate.find(x=>x.value == o).desc; }
    decstateicn(o:string){ return this.lsstate.find(x=>x.value == o).icon; }
    dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }

    fnd(){ 
      this.srcrowselect = -1;
      this.tarrowselect = -1;
      this.pm.tflow = "IO";
      this.pm.thcode = this.sourceStore;
        this.sv.find(this.pm).subscribe(            
          (res) => { 
              this.lsroutesource = res; 
              this.lsroutetarget = [];
              this.routesource = new route_md();
              this.routetarget = new route_md();
            },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }
    fndtarget(){ 
      this.tg.tflow = "IO";
      this.tg.thcode = this.targetStore;
      this.sv.find(this.tg).subscribe(            
        (res) => { 
            this.lsroutetarget = res; 
            this.lsroutetarget = this.lsroutetarget.filter(x=>x.routeno != this.routesource.routeno);
            this.routetarget = new route_md();
            this.tarrowselect = -1;
          },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    // select source route
    getsourceinfo(o:route_ls,ix:number){ 
      this.srcrowselect = ix;
      this.sv.get(o).subscribe(            
        (res) => {             
            this.routesource = res; 
            this.pm.thcode = this.routesource.thcode;
            this.routesource.thname = o.thname;
            this.ngcalSummary();

            // this.targetStore = this.routesource.thcode;
            // this.fndtarget();
          },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    gettargetinfo(o:route_ls,ix:number){ 
      this.tarrowselect = ix;
      // this.tg.thcode = o.thcode;
      this.sv.get(o).subscribe(            
        (res) => { 
            this.routetarget = res; 
            this.ngcalSummary();
            this.routetarget.thname = o.thname;
            this.routetarget.allocate = new Array();
          },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    allocate(o:route_hu){ 
      if(this.routetarget.hus == null){
        this.toastr.error("<span class='fn-1e15'>Please Select the Target Route</span>",null,{ enableHtml : true });
      }else {
        this.routetarget.hus.push(o);
        this.routetarget.allocate.push(o);
        this.routesource.hus = this.routesource.hus.filter(x=>x.huno != o.huno);
        this.ngcalSummary();
      }
    }
    unallocate(o:route_hu){ 
      if(this.routesource.hus == null){
        this.toastr.error("<span class='fn-1e15'>Please Select the Source Route</span>",null,{ enableHtml : true });
      }else {
      this.routesource.hus.push(o);
      this.routesource.allocate = this.routetarget.allocate.filter(x=>x.huno != o.huno);
      this.routetarget.hus = this.routetarget.hus.filter(x=>x.huno != o.huno);
      this.ngcalSummary();
    }

    }

    confirm(){
      let vsource = "";
      this.ngPopups.confirm('Do you confirm HU allocate  ?')
      .subscribe(res => {
          if (res) {
            this.routetarget.routesource = this.routesource.routeno;
            this.sv.allocate(this.routetarget).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-1e15'>Allocate success</span>",null,{ enableHtml : true }); 
                this.pm.thcode = this.routetarget.thcode;
                vsource = this.routesource.routeno;
                this.fnd();
                this.getsourceinfo(this.lsroutesource.find(e=>e.routeno == this.routetarget.routeno),this.srcrowselect);
                this.gettargetinfo(this.lsroutesource.find(e=>e.routeno == vsource),this.tarrowselect);
              },
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                 
          } 
      });

    }

    getMaster() {
      // this.mv.getlov("OUORDER","FLOW").pipe().subscribe((res)=> { this.lsstate = res; });
      // this.mv.getlov("ROUTE","TYPE").pipe().subscribe((res)=> { this.lsroutetype = res; });
      // this.mv.getlov("TRANSPORT","LOADTYPE").pipe().subscribe((res)=> { this.lsloadtype = res; });
      // this.mv.getlov("TRANSPORT","PAYMENT").pipe().subscribe((res)=> { this.lspayment = res; });
      // this.mv.getlov("TRANSPORT","TRUCKTYPE").pipe().subscribe((res)=> { this.lstrucktype = res; });
      // this.mv.getlov("TRANSPORT","TRTMODE").pipe().subscribe((res)=> { this.lstrtmode = res; });
      // this.sv.gettransporter().subscribe( (res) => { this.lstransportor = res; } );

      Promise.all([
        this.mv.getlov("OUORDER","FLOW").toPromise(), 
        this.mv.getlov("ROUTE","TYPE").toPromise(),
        this.mv.getlov("TRANSPORT","LOADTYPE").toPromise(),
        this.mv.getlov("TRANSPORT","PAYMENT").toPromise(),
        this.mv.getlov("TRANSPORT","TRUCKTYPE").toPromise(),
        this.mv.getlov("TRANSPORT","TRTMODE").toPromise(),
        this.sv.gettransporter().toPromise()
      ]).then(res=> {
        this.lsstate = res[0];
        this.lsroutetype = res[1];
        this.lsloadtype = res[2];
        this.lspayment = res[3];
        this.lstrucktype = res[4];
        this.lstrtmode = res[5];
        this.lstransportor = res[6];
        this.lsrowlmt = this.mv.getRowlimit();
      })

    }
    ngDectype(o:string) { try { return this.lsroutetype.find(e=>e.value == o).desc }catch(ex) { return o;} }
    ngDecload(o:string) { try { return this.lsloadtype.find(e=>e.value == o).desc }catch(ex) { return o;} }
    ngDecpayment(o:string) { try { return this.lspayment.find(e=>e.value == o).desc }catch(ex) { return o;} }
    ngDectruck(o:string) { try { return this.lstrucktype.find(e=>e.value == o).desc }catch(ex) { return o;} }
    ngDectrtmode(o:string) { try { return this.lstrtmode.find(e=>e.value == o).desc }catch(ex) { return o;} }
    ngDectransport(o:string) { try {return this.lstransportor.find(e=>e.value == o).desc  }catch(ex) { return o;} }
    ngDecIconRoute(o){ return (o=="P") ? "fas fa-shopping-cart " : "fas fa-truck text-primary"; }
    ngOnDestroy():void {
      this.lsstate        = null; delete this.lsstate;
      this.lstype         = null; delete this.lstype;
      this.instocksum     = null; delete this.instocksum;
      this.rqedit         = null; delete this.rqedit;
      this.crstate        = null; delete this.crstate;
      this.lsroutesource  = null; delete this.lsroutesource;
      this.lsroutetarget  = null; delete this.lsroutetarget;
      this.pm             = null; delete this.pm;
      this.routesource    = null; delete this.routesource;
      this.routetarget    = null; delete this.routetarget;
      this.crtab          = null; delete this.crtab;
      this.crorder        = null; delete this.crorder
      this.dateformat     = null; delete this.dateformat;
      this.dateformatlong = null; delete this.dateformatlong;
      this.datereplan     = null; delete this.datereplan;
      this.lssort         = null; delete this.lssort;
      this.lsreverse      = null; delete this.lsreverse;
      this.page           = null; delete this.page;
      this.pageSize       = null; delete this.pageSize;
      this.slrowlmt       = null; delete this.slrowlmt;
      this.lsrowlmt       = null; delete this.lsrowlmt;
      this.lsroutetype    = null; delete this.lsroutetype;
      this.lsloadtype     = null; delete this.lsloadtype;
      this.lspayment      = null; delete this.lspayment;
      this.lstrucktype    = null; delete this.lstrucktype;
      this.lstrtmode      = null; delete this.lstrtmode;
      this.lstransportor  = null; delete this.lstransportor ;
    }

    ngcalSummary(){
      this.sumsrc = {qty:0,weight:0,volume:0};
      this.sumtar = {qty:0,weight:0,volume:0};
      this.routesource?.hus?.forEach(x=>{
        this.sumsrc.qty = this.sumsrc.qty + x.crsku;
        this.sumsrc.weight = this.sumsrc.weight + x.crweight;
        this.sumsrc.volume = this.sumsrc.volume + x.crvolume;
      });
      this.routetarget?.hus?.forEach(x=>{
          this.sumtar.qty = this.sumtar.qty + x.crsku;
          this.sumtar.weight = this.sumtar.weight + x.crweight;
          this.sumtar.volume = this.sumtar.volume + x.crvolume;
      });
  }
}


export interface routesummary{
  qty?:number,
  weight?:number,
  volume?:number
}