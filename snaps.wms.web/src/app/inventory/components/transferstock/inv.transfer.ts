import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { stock_info, stock_ls, stock_md, stock_pm } from '../../models/inv.stock.model';
import { inventoryService } from '../../services/inv.stock.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { transfer_md } from '../../models/inv.transfer.model';
import { adminService } from 'src/app/admn/services/account.service';
import { task_md } from 'src/app/task/Models/task.movement.model';
import { route_thsum } from 'src/app/outbound/Models/oub.route.model';
import { shareService } from 'src/app/share.service';
import { resolveProjectReferencePath, textSpanIntersectsWithPosition } from 'typescript';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


declare var $: any;
@Component({
  selector: 'appinv-transfer',
  templateUrl: 'inv.transfer.html',
  styles: ['.dgproduct { height:230px !important; } ','.dgstockline { height:calc(100vh - 500px) !important; } ','.dgtransfer { height:calc(100vh - 470px) !important; } '] ,
})
export class invtransferComponent implements OnInit {

  public lsstate:lov[] = new Array();
  public lscode:lov[] = new Array();
  public crcode:lov = new lov();
  public crcort:transfer_md = new transfer_md();
  public crstock:stock_md = new stock_md();
  public lsstock:stock_ls[] = new Array();
  public instock:stock_info =new stock_info();
  public pm:stock_pm = new stock_pm();
  public instocksum:number = 0;
  public rqedit:number = 0;
  public crstate:boolean = false;

  //Selection 
  public slproduct:stock_ls = new stock_ls();
  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //PageNavigate
  lsrowlmt:lov[] = new Array();
  slrowlmt:lov;
  page = 4;
  pageSize = 200;
  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;
  //Tab
  crtab:number = 1;

  //is Correction In
  validated:number = 0;
  //List correction for select 
  lscorsel:lov[] = new Array();

  lsloccode:lov[] = new Array();
  crloccode:lov;

  public artrowselect:number;
  public palrowselect:number;
  constructor(private sv: inventoryService,
      private av: authService, 
      private mv: adminService,
      private ss: shareService,
      private router: RouterModule,
      private toastr: ToastrService,                
      private ngPopups: NgPopupsService,) { 
      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong; 
  }

 
    ngOnInit(): void { }
    ngOnDestroy():void {  }
    ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/this.getstate(); this.gettype(); this.fndproct(); }
    setupJS() { 
        // sidebar nav scrolling
        $('#accn-list .sidebar-scroll').slimScroll({
        height: '95%',
        wheelStep: 5,
        touchScrollStep: 50,
        color: '#cecece'
        });   
    }
    getIcon(o:string){ return "";  }
    //toggle(){ $('.snapsmenu').click();  }
    decstate(o:string){ 
      return o.replace("cr","").toString();
     }
    decstateicn(o:string){ 
      switch(o){
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
        case 'crpicking' : return 'fas fa-hand fn-second w25';
        case 'crreserve' : return 'fas fa-pallet fn-second w25';
        default : return '';
      } 
    }
    //dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }
    getstate(){ 
        this.mv.getlov("TASK","FLOW").pipe().subscribe(
            (res) => { this.lsstate = res; },
            (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
          );
    }
    gettype(){ 
        this.mv.getlov("CORRECTION","CODE").pipe().subscribe(
            (res) => { this.lscode = res; this.lscorsel = this.lscode.filter(x=>x.valopnthird == "+"); },
            (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
          );
    }
    fndproct(){ 
        this.sv.findproduct(this.pm).subscribe(            
          (res) => {
             this.lsstock = res; 
            // refresh model;
            this.crcort = new transfer_md();
            this.instock = new stock_info();
          },
          (err) => { 
            this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  
            this.crcort = new transfer_md();
            this.instock = new stock_info();
          },
          () => { } 
        );
    }

    getinfo(o:stock_ls,ix:number){
      this.artrowselect = ix;
      this.palrowselect = -1;
       // refresh model;
       this.crcort = new transfer_md();

      this.sv.getstockInfo(o).subscribe(            
        (res) => { 
          this.slproduct = o;
          this.instock = res; 
          this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);
          this.validated = 0;
          // this.crcort.article=o.article;
          // this.crcort.lv = o.lv;
          // this.crcort.pv = o.pv;
          // this.crcort.description = o.description;
          // this.crcort.unitmanage = o.unitmanage;
          // this.crcort.unitratio = o.unitratio;
          // this.crcort.tflow = "IO";
          // this.getlocation(o);
          this.getline();
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }
    getlocation(o:stock_ls){ 
      this.sv.getlocation(o).subscribe(
        (res)=> { this.lsloccode = res; },
        (err)=> {});
    }

    getline(s:string = ""){
      if (this.pm.huno != '') {this.slproduct.huno = this.pm.huno; }
      if (this.pm.inrefno != '') {this.slproduct.inrefno = this.pm.inrefno; }
      if (this.pm.loccode != '') { this.slproduct.loccode = this.pm.loccode; }
      this.sv.getstockline('crwomovement', this.slproduct).subscribe(            
        (res) => { 
          this.instock.lines = new Array();
          this.instock.lines = res; 
          this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);
          if (s=="o") { 
            this.crcort.tflow = "IO";
            this.rqedit = 0; 
          }
          this.crloccode = null;
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
      }

    slcstock(o:stock_md,ix:number){ 
      this.palrowselect = ix;
      this.validated = 0;
      this.crcort.inrefno = o.inrefno;
      this.crcort.stockid = o.stockid;
      this.crcort.thcode = o.thcode;
      this.crcort.inrefno = o.inrefno;
      this.crcort.loccode = o.loccode;
      this.crcort.article = o.article;
      this.crcort.description = this.slproduct.description;
      this.crcort.pv = o.pv;
      this.crcort.lv = o.lv;
      this.crcort.stockbeforepu = o.qtypu;
      this.crcort.stockbeforesku = o.qtysku;
      this.crcort.daterec = o.daterec;
      this.crcort.batchno = o.batchno;
      this.crcort.lotno = o.lotno;
      this.crcort.datemfg = o.datemfg;
      this.crcort.dateexp = o.dateexp;
      this.crcort.serialno = o.serialno;
      this.crcort.reason = "";
      this.crcort.variancepu = 0;
      this.crcort.variancesku = 0;
      this.crcort.afterpu = o.qtypu;
      this.crcort.aftersku = o.qtysku;
      this.crcort.unitratio = this.slproduct.unitratio;
      this.crcort.unitmanage = this.slproduct.unitmanage;
      this.crcort.huno = o.huno;
      this.crcort.inrefno = o.inrefno;
      this.lscorsel = this.lscode;
      this.crcort.sourcelocation = o.loccode;
      this.crcort.tflow = "IO";
      this.crcort.stocktflow = o.tflow;
      this.crcort.ingrno = o.ingrno;
      this.crcort.inagrn = o.inagrn;
      this.getlocation( this.slproduct);

      // this.crcort.unitmanage = o.unitmanage;
      // this.crcort.unitratio = o.unitratio;
      // this.crcort.tflow = "IO";
      this.getlocation(o);

      console.log(this.slproduct);
    }
    calstock(o:string) { 

      if (o=='vrsku') {
        this.crcort.variancepu = parseInt(this.crcort.variancesku.toString()) / parseInt(this.crcort.unitratio.toString());
        this.crcort.aftersku = parseInt(this.crcort.stockbeforesku.toString()) - parseInt(this.crcort.variancesku.toString());
        this.crcort.afterpu =  this.cvunit((parseInt(this.crcort.stockbeforesku.toString()) -  parseInt(this.crcort.variancesku.toString())) / this.crcort.unitratio);

        if (this.crcort.variancesku > this.crcort.stockbeforesku ) { 
          this.toastr.warning("<span class='fn-08e'>Stock on HU not enough</span>",null,{ enableHtml : true }); 
          this.resetcal();
        }
      }else if (o=='vrpu'){
        this.crcort.variancesku = parseInt(this.crcort.variancepu.toString()) * parseInt(this.crcort.unitratio.toString());
        this.crcort.aftersku = parseInt(this.crcort.stockbeforesku.toString()) - parseInt(this.crcort.variancesku.toString());
        this.crcort.afterpu =  this.cvunit((parseInt(this.crcort.stockbeforesku.toString()) - parseInt(this.crcort.variancesku.toString())) / this.crcort.unitratio);

        if (this.crcort.afterpu < 0 ) { 
          this.toastr.warning("<span class='fn-08e'>Stock on HU not enough</span>",null,{ enableHtml : true }); 
          this.resetcal();
        }
      }else if (o=='afsku'){
        this.crcort.afterpu = parseInt(this.crcort.aftersku.toString()) / parseInt(this.crcort.unitratio.toString());
        this.crcort.variancesku = parseInt(this.crcort.aftersku.toString()) - parseInt(this.crcort.stockbeforesku.toString());
        this.crcort.variancepu = this.cvunit(this.crcort.variancesku / this.crcort.unitratio);    

        if (this.crcort.aftersku > this.crcort.stockbeforesku || this.crcort.aftersku < 1 ) { 
          this.toastr.warning("<span class='fn-08e'>Stock on HU not enough</span>",null,{ enableHtml : true }); 
          this.resetcal();
        } 
        
      }else if (o=='afpu'){
        this.crcort.aftersku = parseInt(this.crcort.afterpu.toString()) * parseInt(this.crcort.unitratio.toString());
        this.crcort.variancesku = parseInt(this.crcort.aftersku.toString()) - parseInt(this.crcort.stockbeforesku.toString());
        this.crcort.variancepu = this.cvunit(this.crcort.variancesku / this.crcort.unitratio);

        if (this.crcort.aftersku > this.crcort.stockbeforesku  || this.crcort.aftersku < 1  ) { 
          this.toastr.warning("<span class='fn-08e'>Stock on HU not enough</span>",null,{ enableHtml : true }); 
          this.resetcal();
        } 
      }
    }

    resetcal() { 
      this.crcort.variancepu = 0;
      this.crcort.variancesku = 0;
      this.crcort.afterpu = this.crcort.stockbeforepu;
      this.crcort.aftersku = this.crcort.stockbeforesku;
    }

    cvunit(sku:number):number{ 
      return Number((sku).toFixed(2));
    }
   
    validate(){ 
      this.sv.validatelocation(this.crcort).subscribe(            
        (res) => { 
          this.validated = 1;
          this.toastr.warning("<span class='fn-1e15'>validate location done</span>",null,{ enableHtml : true }); 
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );    
    }

    addLocation = (loc) => { 
      return new Promise((resolve, reject) => {
        this.sv.checklocation(loc).toPromise().then(
          (res)=>{  resolve(res); })
     })

      // if (name == "BULK05") { 
      //   console.log(name);
      //   this.lsloccode.push({ value : 'BULK05', desc : "BULK05", icon : "", valopnfirst : "",valopnfour : "",valopnsecond : "",valopnthird :""});
      //   this.crloccode = this.lsloccode.find(e=>e.value == name);
      //   console.log(this.crloccode);
      //   return true; 
      // }else { return false; }
      // console.log(name);
    }

    ngDecUnitstock(o:string) { return this.ss.ngDecUnitstock(o); }
    ngStockicon(o:string, m:string) { return this.ss.ngStockicon(o,m); }
    opstransfer(){

      if (this.crcort.variancesku == 0) { 
        this.toastr.warning("<span class='fn-08e'>Quantity to transfer must over than 0</span>",null,{ enableHtml : true }); 
      }else {
        this.ngPopups.confirm('Do you confirm Transfer stock ?')
        .subscribe(res => {
            if (res) {
              this.crcort.targetlocation = this.crloccode.value;
              this.crcort.unitops = this.crcort.unitmanage;
              this.crcort.variancepu = this.cvunit(parseInt(this.crcort.variancepu.toString()));
              this.crcort.afterpu = this.cvunit(parseInt(this.crcort.afterpu.toString()));
              this.sv.opstrasnfer(this.crcort).subscribe(            
                (res) => { 
                  this.toastr.success("<span class='fn-1e15'>Transfer stock success</span>",null,{ enableHtml : true }); 
                  // this.crcort = res;
                  // this.crcort.tflow = "ED";
                  // this.validated = 0;

                  this.crcort  = new transfer_md();

                  this.getline("o");
                },
                (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                () => { } 
              );                 
            } 
        });
      }


    }


}
