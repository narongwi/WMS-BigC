import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { stock_md, stock_ls, stock_pm, stock_info } from '../../models/inv.stock.model';
import { inventoryService } from '../../services/inv.stock.service';
import { correction_md } from '../../models/inv.correction.mode';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'appinv-correction',
  templateUrl: 'inv.correction.html',
  styles: ['.dgproduct { height:200px !important; } ',
           '.dgstockline { height:calc(100vh - 470px) !important; } ',
           '.dgcorrect { height:calc(100vh - 195px) !important; } ',
           '.selectedstock { background-color : #5e696514 }'] ,
  providers: [
    {provide: NgbDateAdapter,         useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}]    
})
export class invcorrectionComponent implements OnInit, OnDestroy {

    public lsstate:lov[] = new Array();
    public lscode:lov[] = new Array();
    public crcode:lov;
    public crcort:correction_md = new correction_md();

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
    isnonhu:number = 1;
    //List correction for select 
    lscorsel:lov[] = new Array();
    //List of location 
    lsloccode:lov[] = new Array();
    crloccode:lov;

    public artrowselect:number;
    public palrowselect:number;
    constructor(private sv: inventoryService,
                private av: authService, 
                private mv: shareService,
                private router: RouterModule,
                private toastr: ToastrService,                
                private ngPopups: NgPopupsService,) { 
      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong;
      this.instock.lines = new Array();
    }

    ngOnInit(): void { }


    ngAfterViewInit(){  this.ngSetup(); this.setupJS(); /*setTimeout(this.toggle, 1000);*/  }
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

    cv2dg(sku:number):number{ return Number((sku).toFixed(3)); }
    fndproduct(){ 
        this.sv.findproduct(this.pm).subscribe(            
          (res) => { 
            this.lsstock = res; 
            this.instock = new stock_info();
            this.crcort = new correction_md();
            if (this.lsstock.length > 0) { 
              this.getinfo(this.lsstock[0],0); 
              this.lscorsel = [];
              this.crcort.inagrn = "";
              this.crcort.ingrno = ""
            } 
          },
          (err) => {
             this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  
             this.instock = new stock_info();
             this.crcort = new correction_md();
            },
          () => { } 
        );
    }

    getinfo(o:stock_ls,ix:number){
      this.lscorsel = [];
      this.crcort = new correction_md();
      this.crcort.inagrn = "";
      this.crcort.ingrno = "";
      this.artrowselect = ix;
      this.palrowselect = -1;
      this.pm.article = o.article;
      this.pm.dlcall = o.dlcall;
      this.pm.dlcfactory = o.dlcfactory;
      this.pm.dlcwarehouse = o.dlcwarehouse;
      this.sv.getstockInfo(o).subscribe(            
        (res) => { 
          this.slproduct = o;
          this.instock = res; 
          this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);
          this.isnonhu = 1;
          this.getline();
          this.getlocation(o);
          this.newcorrect();
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

    getline(){
      if (this.pm.huno != '') {this.slproduct.huno = this.pm.huno; }
      if (this.pm.inrefno != '') {this.slproduct.inrefno = this.pm.inrefno; }
      if (this.pm.loccode != '') { this.slproduct.loccode = this.pm.loccode; }
      this.sv.getstockline('crwomovement', this.slproduct).subscribe(            
        (res) => { 
          this.instock.lines = new Array();
          this.instock.lines = res; 
          this.instocksum = this.instock.lines.reduce((obl, val) => obl += val.qtysku, 0);          
          this.rqedit = 0;
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    slcstock(o:stock_md,ix:number){ 
      if(o.tflow != "IO"){
        this.newcorrect();
        this.toastr.error("<span class='fn-1e15'>this pallet state is not active !</span>",null,{ enableHtml : true });
        return false;
      }else {
        this.crcort = new correction_md();
        this.palrowselect = ix;
        this.isnonhu = 0;
        this.crcort.inrefno = o.inrefno;
        this.crcort.ingrno = o.ingrno;
        this.crcort.inagrn = o.inagrn;
        this.crcort.stockid = o.stockid;
        this.crcort.description = o.description;
        this.crcort.thcode = o.thcode;
        this.crcort.inrefno = o.inrefno;
        this.crcort.loccode = o.loccode;
        this.crcort.article = o.article;
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
        //this.crcort.unitratio = o.unitratio;
        //this.crcort.unitmanage = o.unitmanage;
        this.crcort.huno = o.huno;
        this.crcort.inrefno = o.inrefno;
        this.lscorsel = this.lscode;
        this.crcort.dlcall = o.dlcall;
        this.crcort.dlcwarehouse = o.dlcwarehouse;
        this.crcort.dlcfactory = o.dlcfactory;
        this.crcort.unitratio = this.slproduct.unitratio;
        this.crcort.unitmanage = this.slproduct.unitmanage;
        if (this.lsloccode.filter(e=>e.value == o.loccode).length == 0){ 
          this.sv.checklocation(this.crcort.loccode).pipe().subscribe((res)=> { this.lsloccode.push(res);  this.crloccode = this.lsloccode.find(e=>e.value == o.loccode); });       
        }else { 
          this.crloccode = this.lsloccode.find(e=>e.value == o.loccode);
        }
      }
    }
    newcorrect(){
      this.isnonhu = 1;
      this.crcort.stockid = 0;
      this.crcort.description = this.slproduct.description;
      this.crcort.thcode = "";
      this.crcort.inrefno = "";
      this.crcort.loccode = "";
      this.crcort.article = this.slproduct.article;
      this.crcort.pv = this.slproduct.pv;
      this.crcort.lv = this.slproduct.lv;
      this.crcort.stockbeforepu = 0;
      this.crcort.stockbeforesku = 0;
      this.crcort.daterec = null;
      this.crcort.batchno = "";
      this.crcort.lotno = "";
      this.crcort.datemfg =  null;
      this.crcort.dateexp =  null;
      this.crcort.serialno = "";
      this.crcort.reason = "";
      this.crcort.huno = "0";
      this.crcort.variancepu = 0;
      this.crcort.variancesku = 0;
      this.crcort.afterpu = 0;
      this.crcort.aftersku = 0;
      this.crcort.unitratio = this.slproduct.unitratio;
      this.crcort.unitmanage = this.slproduct.unitmanage;
      // default correction code plus
      this.lscorsel = this.lscode.filter(x=>x.valopnfirst == "+");
    }
    ulcstock(){
      this.rqedit = 0;
      this.crcort = new correction_md(); 

    }

    calexpdate() { 
      console.log(this.pm);
      this.crcort.dateexp = new Date();
        this.crcort.dateexp = this.addDays(this.crcort.datemfg,this.pm.dlcall);
        console.log("datemfg" + this.crcort.datemfg);
        console.log("dlcall" + this.pm.dlcall)
        console.log("dateexp" + this.crcort.dateexp);


    }
    calmfgdate() { 
      console.log(this.pm);

      this.crcort.datemfg = new Date();
      this.crcort.datemfg = this.addDays(this.crcort.dateexp,this.pm.dlcall *-1);
      console.log("dateexp" + this.crcort.dateexp);
      console.log("dlcall" + this.pm.dlcall)
      console.log("datemfg" + this.crcort.datemfg);
    }

    addDays (date, daysToAdd) {
      var _24HoursInMilliseconds = 86400000;
      return new Date(date.getTime() + daysToAdd * _24HoursInMilliseconds);
    };
    
    calstock2(o:string) { 
      if (o=='vrsku') {
        this.crcort.variancepu = this.crcort.variancesku / this.crcort.unitratio;
        this.crcort.aftersku = parseInt(this.crcort.variancesku.toString()) + (( this.isnonhu == 1 ) ? 0 : parseInt(this.crcort.stockbeforesku.toString())); 
        this.crcort.afterpu =  this.cv2dg(this.crcort.aftersku / this.crcort.unitratio);
        
        if(this.crcort.aftersku < 0 && this.isnonhu == 0 ){
          this.toastr.warning("<span class='fn-08e'>Stock on HU is not enough</span>",null,{ enableHtml : true }); 
          this.resetcal();
        }else if(this.crcort.aftersku < 0 && this.isnonhu == 1 ){
          this.toastr.warning("<span class='fn-08e'>Not allow Negative Stock</span>",null,{ enableHtml : true }); 
          this.resetcal();
        }
      }else if (o=='vrpu'){
        this.crcort.variancesku = this.crcort.variancepu * this.crcort.unitratio;
        this.crcort.aftersku = parseInt(this.crcort.variancesku.toString()) + (( this.isnonhu == 1 ) ? 0 : parseInt(this.crcort.stockbeforesku.toString())); 
        this.crcort.afterpu =  this.cv2dg(this.crcort.aftersku / this.crcort.unitratio);

        if(this.crcort.aftersku < 0 && this.isnonhu == 0 ){
          this.toastr.warning("<span class='fn-08e'>Stock on HU is not enough</span>",null,{ enableHtml : true }); 
          this.resetcal();
        }else if(this.crcort.aftersku < 0 && this.isnonhu == 1 ){
          this.toastr.warning("<span class='fn-08e'>Not allow Negative Stock</span>",null,{ enableHtml : true }); 
          this.resetcal();
        }
      }else if (o=='afsku'){
        this.crcort.afterpu = this.crcort.aftersku / this.crcort.unitratio;
        this.crcort.variancesku = parseInt(this.crcort.aftersku.toString()) - (( this.isnonhu == 1 ) ? 0 : parseInt(this.crcort.stockbeforesku.toString()));
        this.crcort.variancepu = this.cv2dg(this.crcort.variancesku / this.crcort.unitratio);    
      }else if (o=='afpu'){
        this.crcort.aftersku = this.crcort.afterpu * this.crcort.unitratio;
        this.crcort.variancesku = parseInt(this.crcort.aftersku.toString()) - (( this.isnonhu == 1 ) ? 0 : parseInt(this.crcort.stockbeforesku.toString()));
        this.crcort.variancepu = this.cv2dg(this.crcort.variancesku / this.crcort.unitratio);
      }
      // dropdown
      if (parseInt(this.crcort.aftersku.toString()) == this.crcort.stockbeforesku ) { 
        this.lscorsel = [];
      }else if (parseInt(this.crcort.aftersku.toString())  > this.crcort.stockbeforesku ) { 
        this.lscorsel = this.lscode.filter(x=>x.valopnthird == "+");
      }else { 
        this.lscorsel = this.lscode.filter(x=>x.valopnthird == "-");
      }
      this.crcode = null;
    }

    resetcal() { 
      this.crcort.variancepu = 0;
      this.crcort.variancesku = 0;
      this.crcort.afterpu = this.crcort.stockbeforepu;
      this.crcort.aftersku = this.crcort.stockbeforesku;
    }
    addLocation = (loc) => { 
      return new Promise((resolve, reject) => {
        this.sv.checklocation(loc).toPromise().then(
          (res)=>{  resolve(res); })
     })
    }
    opscorrect(){
      if (this.crcort.aftersku < 0) { 
        this.toastr.warning("<span class='fn-07e'>not allow for after quantity is minus</span>",null,{ enableHtml : true });
      } else if (this.crcode == null ) {       
        this.toastr.warning("<span class='fn-07e'>Require correction code</span>",null,{ enableHtml : true });
      } else if (this.instock.isunique == 1 && this.crcort.serialno == null){
        this.toastr.warning("<span class='fn-07e'>Serial no require</span>",null,{ enableHtml : true });
      } else if (this.instock.isdlc == 1 && (this.crcort.dateexp == null || this.crcort.datemfg == null )) {
        this.toastr.warning("<span class='fn-07e'>MFG Date and Expire date rqequire</span>",null,{ enableHtml : true });
      } else if (this.instock.isbatchno == 1 && this.crcort.batchno == null) {
        this.toastr.warning("<span class='fn-07e'>Batch no require</span>",null,{ enableHtml : true });
      } else if (this.crcode == null) {
        this.toastr.warning("<span class='fn-07e'>Correction code require</span>",null,{ enableHtml : true });      
      }else { 
        this.ngPopups.confirm('Do you confirm correction stock ?')
        .subscribe(res => {
            if (res) {
              this.crcort.codeops = this.crcode.value;
              this.crcort.typeops = this.crcode.valopnthird;
              this.crcort.unitops = this.crcort.unitmanage;
              this.crcort.loccode = this.crloccode.value;
              this.pm.article = this.crcort.article;
              this.sv.opscorrect(this.crcort).subscribe(            
                (res) => { 
                  this.toastr.success("<span class='fn-07e'>Correction stock success</span>",null,{ enableHtml : true }); 
                  this.newcorrect();
                  this.fndproduct();
                  this.crcode = null;
                },
                (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                () => { } 
              );                 
            } 
        });
      }
    }
    ngStockicon(o:string, m:string) { return this.mv.ngStockicon(o,m); }
    ngDecUnitstock(o:string){ return this.mv.ngDecUnitstock(o); }
    ngDecStr(o) { return this.mv.ngDecStr(o); }
    ngSetup(){ 
      this.mv.getlov("CORRECTION","CODE").pipe().subscribe((res) => { this.lscode = res; this.lscorsel = this.lscode.filter(x=>x.valopnthird == "+");
       //console.log(this.lscode);  
      });
    }
    ngOnDestroy():void {  
      this.lsstate = null;      delete this.lsstate;
      this.lscode = null;       delete this.lscode;
      this.crcode = null;       delete this.crcode;
      this.crcort = null;       delete this.crcort;  
      this.crstock = null;      delete this.crstock;
      this.lsstock = null;      delete this.lsstock;
      this.instock = null;      delete this.instock;
      this.pm = null;           delete this.pm;
      this.instocksum = null;   delete this.instocksum;
      this.rqedit = null;       delete this.rqedit;
      this.crstate = null;      delete this.crstate;
  
      //Selection 
      this.slproduct = null;    delete this.slproduct;
      //Sorting 
      this.lssort = null;       delete this.lssort;
      this.lsreverse = null;    delete this.lsreverse;
      //PageNavigate
      this.lsrowlmt = null;     delete this.lsrowlmt;
      this.slrowlmt = null;     delete this.slrowlmt;
      this.page = null;         delete this.page;
      this.pageSize = null;     delete this.pageSize;
      //Date format
      this.dateformat = null;   delete this.dateformat;
      this.dateformatlong = null; delete this.dateformatlong;
      this.datereplan = null;   delete this.datereplan;
      //Tab
      this.crtab = null;        delete this.crtab;
  
      //is Correction In
      this.isnonhu = null;      delete this.isnonhu;
      //List correction for select 
      this.lscorsel = null;     delete this.lscorsel;
    }


}
