import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { adminService } from '../../../admn/services/account.service';
import { lov } from '../../../helpers/lov';
import { product_ls, product_md, product_pm } from '../../models/adm.product.model';
import { admproductService } from '../../services/adm.product.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { ThrowStmt } from '@angular/compiler';
import { shareService } from 'src/app/share.service';
@Component({
  selector: 'appadm-productline',
  templateUrl: 'adm.product.line.html',
  styles: ['.dproduct { height:calc(100vh - 235px) !important;  }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class admproductlineComponent implements OnInit, OnDestroy {
//   @Input() iconstate: string;

    @Output() selln = new EventEmitter<product_ls>();

    ison:boolean=false;
    public lsproduct:product_ls[] = new Array();
    public pm:product_pm = new product_pm();

    public slcbutype:lov = new lov();
    public slcthtype:lov = new lov();
    public slcarticletype:lov=new lov();

    public msdstate:lov[] = new Array();
    public msdbu:lov[] = new Array();
    public msdthtype:lov[] = new Array();
    public msdtype:lov[] = new Array();
    
    
    public crstate:boolean = true;
    public spcareadsc:string;


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

    //LOV
    lsunit:lov[] = new Array();

    //Parameter
    ismeasurement:boolean = false;
    public artrowselect:number;

    constructor(
        private sv: admproductService,
        private av: authService,
        private mv: adminService, 
        private ss: shareService,
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
          this.av.retriveAccess(); 
          this.dateformat = this.av.crProfile.formatdate;
          this.dateformatlong = this.av.crProfile.formatdatelong;        
          this.getmaster(); 
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fndproduct(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcbutype(){ this.msdtype = this.msdthtype.filter(x=>x.valopnfirst == this.slcbutype.value); }
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

    getmaster() { 
      Promise.all([
        this.mv.getlov("ALL","FLOW").toPromise(), 
        this.ss.lovrowlimit().toPromise(),
        // this.mv.getlov("ARTICLE","ROOMTYPE").toPromise(),
        this.mv.getlov("UNIT","KEEP").toPromise(),
        this.mv.getlov("product","BUTYPE").toPromise(),
        // this.mv.getlov("product","TYPE").toPromise()
        this.mv.getlov("ARTICLE","TYPE").toPromise(),
      ]).
      then((res) => {
        this.msdstate = res[0];
        this.msdstate.push({ value : 'NW', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        this.msdstate.push({ value : 'false', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});

        this.lsrowlmt = res[1].sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); 
        this.lsunit = res[2];
        this.msdbu = res[3];
        this.msdthtype = res[4];

      });

      // this.mv.getlov("ALL","FLOW").pipe().subscribe(
      //   (res) => { this.msdstate = res;
      //     this.msdstate.push({ value : 'NW', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
      //     this.msdstate.push({ value : 'false', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
      //   },
      //   (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      // this.mv.getlov("DATAGRID","ROWLIMIT").pipe().subscribe(
      //   (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      // this.mv.getlov("UNIT","KEEP").pipe().subscribe(
      //   (res) => { this.lsunit = res;  },
      //   (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // ); 
      // this.mv.getlov("product","BUTYPE").pipe().subscribe(
      //   (res) => { this.msdbu = res; },
      //   (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      // this.mv.getlov("product","TYPE").pipe().subscribe(
      //   (res) => { this.msdthtype = res; },
      //   (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
    }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }

    fndproduct(){ 
      if (this.ismeasurement == true) { this.pm.ismeasurement = "1"; }else{ this.pm.ismeasurement = undefined;}
      this.pm.articletype = this.slcarticletype.value;
      this.sv.find(this.pm).pipe().subscribe(            
          (res) => { this.lsproduct = res; },
          (err) => { this.toastr.error(err.error.message); this.ison = false; },
          () => { }
      );
    }
    decbool(o:boolean):number {  return (o==true) ? 1 : 0 ; }
    encbool(o:number):boolean { return (o==1)? true : false; }
    selproduct(o:product_ls,ix:number){
      this.artrowselect = ix;
       this.selln.emit(o); 
      }

    ngOnDestroy() { 
      this.ison = null;      delete this.ison;
      this.lsproduct = null; delete this.lsproduct;
      this.pm = null;        delete this.pm;
      this.slcbutype = null; delete this.slcbutype;
      this.slcthtype = null; delete this.slcthtype;
      this.msdstate = null;  delete this.msdstate;
      this.msdbu = null;     delete this.msdbu;
      this.msdthtype = null; delete this.msdthtype;
      this.msdtype = null;   delete this.msdtype;
      this.crstate = null;   delete this.crstate;
      this.spcareadsc = null;delete this.spcareadsc;
      this.dateformat = null;delete this.dateformat;
      this.dateformatlong = null; delete this.dateformatlong;
      this.datereplan = null;delete this.datereplan;
      this.lssort = null;    delete this.lssort;
      this.lsreverse = null; delete this.lsreverse;
      this.page = null;      delete this.page;
      this.pageSize = null;  delete this.pageSize;
      this.slrowlmt = null;  delete this.slrowlmt;
      this.lsunit = null;    delete this.lsunit;
    }
}
