import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from 'src/app/admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { shareService } from 'src/app/share.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { route_thsum } from 'src/app/outbound/Models/oub.route.model';

@Component({
  selector: 'appmaps-levelline',
  templateUrl: 'maps.level.line.html',
  styles: ['.dglevel { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapslevelline implements OnInit,OnDestroy {
  ison:boolean=false;
  //Lov 
  public lszone:lov[] = new Array();
  public lsaisle:lov[] = new Array();
  public lsbay:lov[] = new Array();
  //Selection 
  public lslevel:locup_md[] = new Array();
  public crlevel:locup_md = new locup_md();
  //Parameter 
  public pmzone:locup_pm = new locup_pm();
  //Selection 
  public slczone:lov = new lov();
  public slcaisle:lov = new lov();
  public slcbay:lov = new lov();
  public crstate:Boolean = true;
  //PageNavigate
  public page = 4;
  public pageSize = 200;
  public slrowlmt:lov;
  public lsrowlmt:lov[] = new Array();
  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //Date format 
  public formatdate:string;
  public formatdatelong:string;

  constructor(
      private av: authService,
      private sv: mapstorageService,
      private mv: shareService,
      private toastr: ToastrService,
      private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); this.ngSetup(); 
        this.formatdate = this.av.crProfile.formatdate;
        this.formatdatelong = this.av.crProfile.formatdatelong;
   }

  ngOnInit() { }
  ngAfterViewInit(){  this.fndlevel(); } 

  ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
  ngselczone(){ this.crlevel.lszone = this.slczone.value; this.crlevel.spcarea = this.slczone.valopnfirst; this.ngGetAisle(); this.slcaisle = null; this.slcbay = null;}
  ngselcaisle() { this.crlevel.lsaisle = this.slcaisle.value; this.ngGetBay(); this.slcbay = null; }
  newlevel() { 
      this.crlevel = new locup_md();
      this.crlevel.orgcode = this.av.crProfile.orgcode;
      this.crlevel.site = this.av.crRole.site;
      this.crlevel.depot = this.av.crRole.depot;
      this.crlevel.fltype = "LV";
      this.crlevel.lszone = "";
      this.crlevel.lscode = "";
      this.crlevel.lsdesc = "";
      this.crlevel.lsseq = 0;
      this.crlevel.tflow = "NW";
      this.crlevel.tflowcnt = "NW";
      this.crlevel.datemodify = new Date();
      this.crlevel.accnmodify = this.av.crProfile.accncode;
      this.crlevel.lshash = "NW";
      this.crlevel.lsformat = "";
      this.crlevel.lsbay = "";
      this.crlevel.lslevel = "";
      this.crlevel.lscodealt = "";
      this.crlevel.lscodefull = "";
      this.crlevel.spcarea = "ST";
      this.crlevel.lscodeid = "";
      this.crstate = true;
      this.slcaisle = null;
      this.slczone = null;
      this.slcbay = null;
      this.crstate = true;
      if(this.lslevel.findIndex(x=>x.lshash =="NW") == -1) {this.lslevel.push(this.crlevel);}        
      this.toastr.info("<span class='fn-1e15'>New bay is ready to setup</span>",null,{ enableHtml : true });
  }
  fndlevel(){ 
    this.pmzone.fltype = "LV";
    this.pmzone.lszone = "";      
      this.sv.fndlocup(this.pmzone).pipe().subscribe( (res) => { this.lslevel = res; });
  }
  sellevel(o:locup_md){
     this.crlevel = o; 
     this.crstate = (this.crlevel.tflow == "IO") ? true : false;
     this.slczone  = this.lszone.find(e=>e.value == this.crlevel.lszone); 
     this.ngGetAisle(true); //this.slcaisle = this.lsaisle.find(e=>e.value == this.crlevel.lsaisle);
     this.ngGetBay(true); //this.slcbay = this.lsbay.find(e=>e.value == this.crlevel.lsbay);

  }
  validate() {

    if (this.slczone == null) { 
      this.toastr.warning("<span class='fn-08e'>Zone is require</span>"); 
    } else if (this.slcaisle == null) {
      this.toastr.warning("<span class='fn-08e'>Aisle is require</span>"); 
    } else if (this.slcbay == null) {
      this.toastr.warning("<span class='fn-08e'>Bay is require</span>"); 
    } else if (this.crlevel.lscode == "") { 
        this.toastr.warning("<span class='fn-08e'>Level code is require</span>"); 
    } else { 

      this.crlevel.tflow = (this.crlevel.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
      this.crlevel.lscodefull = this.crlevel.orgcode + "-" + this.crlevel.site+ "-" +this.crlevel.depot+ "-" + this.crlevel.lszone + "-" + this.crlevel.lscode;
      this.crlevel.lszone = this.slczone.value; this.ngGetAisle();
      this.crlevel.lsaisle = this.slcaisle.value; this.ngGetBay();
      this.crlevel.lsbay = this.slcbay.value;
      this.ngPopups.confirm('Do you accept change of Level ?')
        .subscribe(res => {
            if (res) {
              this.ison = true;
              this.crlevel.tflowcnt = this.crlevel.tflow;
              this.sv.upsertlocup(this.crlevel).pipe().subscribe(            
                  (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndlevel(); },
                  (err) => { this.toastr.error(err.error.message); this.ison = false; },
                  () => { }
              );
            } 
        });
    }
  }
  drop() { 
    this.ngPopups.confirm('Do you accept to drop Level ?')
    .subscribe(res => { 
        if (res) {
          this.ison = true;
          this.sv.droplocup(this.crlevel).pipe().subscribe(            
              (res) => { this.toastr.success('Drop aisle successful'); this.ison = false; this.fndlevel(); }
          );              
        } 
    });
  }
  ngGetAisle(o:boolean = false){ 
    this.mv.lovaisle(this.slczone.value).subscribe( (resz) => { this.lsaisle = resz; if (o==true) { this.slcaisle = this.lsaisle.find(e=>e.value == this.crlevel.lsaisle); } });
  }
  ngGetBay(o:boolean = false){ 
    this.mv.lovbay(this.slczone.value,this.slcaisle.value).subscribe( (resz) => { this.lsbay = resz; if (o==true) { this.slcbay = this.lsbay.find(e=>e.value == this.crlevel.lsbay); }  });  
  } 
  ngDecZone(o:string) { try { return this.lszone.find(e=>e.value == o).desc; } catch(exp){ return o; }}
  ngDecArea(o:string) { return this.mv.ngDecArea(o); }
  ngDecState(o:string){ return this.mv.ngDecState(o); }
  ngDecIcon(o:string) { return this.mv.ngDecIcon(o); }
  ngChangeRowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
  ngSetup(){ 
    this.mv.lovzone().subscribe( (res) => { 
      this.lszone = res.filter(e=>e.valopnfirst == "ST");
      if (this.lszone.length > 0) { 
        this.lsrowlmt = this.mv.getRowlimit();
        this.mv.lovaisle(this.lszone[0].value).subscribe( (resz) => { this.lsaisle = resz;});
      }
    } );
  }
  ngOnDestroy():void{ 
    this.ison = null;     delete this.ison;
    this.lszone = null;   delete this.lszone;
    this.lsaisle = null;  delete this.lsaisle;
    this.lsbay = null;    delete this.lsbay;
    this.lslevel = null;  delete this.lslevel;
    this.crlevel = null;    delete this.crlevel;
    this.pmzone = null;   delete this.pmzone;
    this.slczone = null;  delete this.slczone;
    this.slcaisle = null; delete this.slcaisle;
    this.slcbay = null;   delete this.slcbay;
    this.crstate = null;  delete this.crstate;
    this.formatdate = null; delete this.formatdate;
    this.formatdatelong = null;  delete this.formatdatelong;
    this.page = null;       delete this.page;
    this.pageSize = null;   delete this.pageSize;
    this.slrowlmt = null;   delete this.slrowlmt;
    this.lsrowlmt = null;   delete this.lsrowlmt;
    this.lssort = null;     delete this.lssort;
    this.lsreverse = null;  delete this.lsreverse;
  }
}