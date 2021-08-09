import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import {  warehouse_ls, warehouse_md, warehouse_pm } from '../../models/adm.warehouse.model';
import { admdepotService } from '../../services/adm.depot.service';
import { shareService } from 'src/app/share.service';
import { binary_md } from '../../models/adm.binary.model';
import { binaryService } from '../../services/adm.binary.service';
import { thparty_md, thparty_pm } from '../../models/adm.thparty.model';
import { adminService } from '../../services/account.service';
import { admthpartyService } from '../../services/adm.thparty.service';
import { depot_md } from '../../models/adm.depot.model';
import { NgbPaginationConfig, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { __assign } from 'tslib';
import { timestamp } from 'rxjs/operators';
declare var $: any;
@Component({
  selector: 'appadm-depot',
  templateUrl: 'adm.depot.html',
  styles: ['.dgline { height:calc(100vh - 410px) !important;','.dgvalue { height:200px !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class admdepotComponent implements OnInit {    @Input() item: lov;

    ison:boolean=false;

    public lswarehouse:lov[] = new Array();
    public slcwarehouse:lov;

    public lsdepot:depot_md[] = new Array();
    public crdepot:depot_md = new depot_md();
    public craddress:thparty_md = new thparty_md();
    public pm:depot_md = new depot_md();
    public pmaddress:thparty_pm = new thparty_pm();
    public slcopstype:lov = new lov();

    public msdstate:lov[] = new Array();
    public msdopstype:lov[] = new Array();
    public crstate:Boolean = false;
    public crtab:number = 1;

    public formatdate:string;
    public formatdatelong:string;

    public lsunitweight:lov[] = new Array();
    public lsunitdimension:lov[] = new Array();
    public lsunitvolume:lov[] = new Array();
    public lsunitcubic:lov[] = new Array();
    public slunitweight:lov;
    public slunitdimension:lov;
    public slunitvolume:lov;
    public slunitcubic:lov;

    constructor(
        private av: authService,
        private sv: admdepotService,
        private tv: admthpartyService,
        private mv: adminService,
        private ss: shareService,
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.formatdate = this.av.crProfile.formatdate;
        this.formatdatelong = this.av.crProfile.formatdatelong;
        this.ngSetup();
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fnddepot(); } 
    ngSetup() { 
      this.ss.warehouse().subscribe((res)=> { this.lswarehouse = res; }); 
      this.mv.getlov("LOCATION","SPCAREA").pipe().subscribe( (res) => { this.msdopstype = res; } ); 
      this.ss.getlov("UNIT","WEIGHT").subscribe((res)=> { this.lsunitweight = res;});
      this.ss.getlov("UNIT","LENGTH").subscribe((res)=> { this.lsunitdimension = res; });
      this.ss.getlov("UNIT","VOLUME").subscribe((res)=> { this.lsunitvolume = res; });
    }
    ngSelc(o:depot_md) { 
      this.crdepot = o;
      this.slcwarehouse = this.lswarehouse.find(e=>e.value == o.sitecode);
      this.slcopstype = this.msdopstype.find(e=>e.value == o.depotops);
      this.slunitweight = this.lsunitweight.find(e=>e.value == o.unitweight);
      this.slunitdimension = this.lsunitdimension.find(e=>e.value == o.unitdimension);
      this.slunitvolume = this.lsunitvolume.find(e=>e.value == o.unitvolume);
    }

    newaddress() { 
      this.craddress.orgcode = "";
      this.craddress.site = ""; 
      this.craddress.depot = ""; 
      this.craddress.spcarea = ""; 
      this.craddress.thtype = "WH"; 
      this.craddress.thbutype = "WH"; 
      this.craddress.thcode = ""; 
      this.craddress.thname = ""; 
      this.craddress.thgroup = ""; 
      this.craddress.tflow = ""; 
      this.craddress.thtypename = ""; 
      this.craddress.thbutypename = ""; 
      this.craddress.thnamealt = "";
      this.craddress.thcodealt = ""; 
      this.craddress.vatcode = "";     
      this.craddress.thnameint = ""; 
      this.craddress.addressln1 = ""; 
      this.craddress.addressln2 = ""; 
      this.craddress.addressln3 = ""; 
      this.craddress.subdistrict = ""; 
      this.craddress.district = ""; 
      this.craddress.city = ""; 
      this.craddress.country = ""; 
      this.craddress.postcode = ""; 
      this.craddress.region = ""; 
      this.craddress.telephone = ""; 
      this.craddress.email = ""; 
      this.craddress.thcomment = ""; 
      this.craddress.throuteformat = ""; 
      this.craddress.plandelivery = 0;
      this.craddress.naturalloss = 0; 
      this.craddress.mapaddress = ""; 
      this.craddress.datecreate = new Date();
      this.craddress.accncreate = ""; 
      this.craddress.datemodify = new Date();
      this.craddress.accnmodify = ""; 
      this.craddress.procmodify = ""; 
    }
    ngNew() { 
      this.crdepot = new depot_md();
        this.crdepot.tflow = "NW";
        this.crstate = true;
        this.newaddress();
        if(this.lsdepot.findIndex(x=>x.tflow =="NW") == -1) {this.lsdepot.push(this.crdepot);}        
        this.toastr.info("<span class='fn-1e15'>New depot is ready to setup</span>",null,{ enableHtml : true });
    }
    fnddepot(){ 
        this.sv.find(this.pm).pipe().subscribe(            
            (res) => { this.lsdepot = res; },
            (err) => { this.toastr.error(err.error.message); this.ison = false; },
            () => { }
        );
    }
    getsite(o:warehouse_ls) { 
    //   this.sv.get(o).pipe().subscribe(            
    //     (res) => { 
    //       this.crdepot = res; 
    //       this.crstate = (this.crdepot.tflow == "IO") ? true : false; 
    //       this.slcopstype = this.msdopstype.find(x=>x.value == this.crdepot.sitetype);
    //     },
    //     (err) => { this.toastr.error(err.error.message); this.ison = false; },
    //     () => { }
    //);
    }
    getaddress(o:warehouse_ls) {
      this.pmaddress.orgcode = o.orgcode;
      this.pmaddress.site = o.sitecode;
      this.pmaddress.thcode = o.sitecode;
      this.pmaddress.thbutype = "SI"; 
      this.tv.get(this.pmaddress).pipe().subscribe(            
        (res) => { 
          console.log(res);
          if (res.thcode != null) { 
            this.craddress = res; 
          }else {
            this.newaddress();
          }
        },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }
    selwarehouse(o:warehouse_ls){ this.getsite(o); this.getaddress(o); }
    validate() {
      this.crdepot.sitecode = this.slcwarehouse.value;
      this.crdepot.depotops = this.slcopstype.value;
      this.crdepot.unitdimension = this.slunitdimension.value;
      this.crdepot.unitvolume = this.slunitvolume.value;
      this.crdepot.unitweight = this.slunitweight.value;
      this.crdepot.tflow = (this.crdepot.tflow != "NW") ? (this.crstate == true) ? "IO" : "XX" : "NW";
      this.craddress.tflow = (this.crstate == true)? "IO" : "XX";  
      this.craddress.thname = this.crdepot.depotname;
      this.craddress.thcode = this.crdepot.sitecode;
      this.craddress.depot = this.crdepot.depotcode;
      this.craddress.thcodealt = this.crdepot.depotnamealt;
      this.craddress.site = this.crdepot.sitecode;
      this.ngPopups.confirm('Do you accept change of site?')
      .subscribe(res => { 
          if (res) {
            this.ison = true;
            this.sv.upsert(this.crdepot).pipe().subscribe(            
                  (res) => { 
                    this.toastr.success('Save successful'); this.ison = false; 
                    this.tv.upsert(this.craddress).pipe().subscribe(            
                      (res) => { this.fnddepot(); }
                    );
                }
            );
          } 
      });
    }


    ngDecIcon(o:string) { return this.ss.ngDecIcon(o); }
    ngDecState(o:string) { return this.ss.ngDecState(o); }
}