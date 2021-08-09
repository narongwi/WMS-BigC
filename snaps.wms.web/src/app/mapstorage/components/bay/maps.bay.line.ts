import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { timestamp } from 'rxjs/operators';
import { adminService } from 'src/app/admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
@Component({
  selector: 'appmaps-bayline',
  templateUrl: 'maps.bay.line.html',
  styles: ['.dgbay { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsbayline implements OnInit,OnDestroy {
    ison:boolean=false;
    public lszone:lov[] = new Array();
    public lsaisle:lov[] = new Array();

    public lsbay:locup_md[] = new Array();
    public crbay:locup_md = new locup_md();

    public pmzone:locup_pm = new locup_pm();

    public slczone:lov = new lov();
    public slcaisle:lov = new lov();
    public slcbay:lov = new lov();

    public msdstate:lov[] = new Array();
    public crstate:Boolean = true;

    public formatdate:string;
    public formatdatelong:string;

    //PageNavigate
    public page = 4;
    public pageSize = 200;
    public slrowlmt:lov;
    public lsrowlmt:lov[] = new Array();
    //Sorting 
    public lssort:string = "spcarea";
    public lsreverse: boolean = false; // for sorting

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
    ngAfterViewInit(){  this.fndbay(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselczone(){ this.crbay.lszone = this.slczone.value; this.crbay.spcarea = this.slczone.valopnfirst; this.mv.lovaisle(this.slczone.value).subscribe( (resz) => { this.lsaisle = resz;}); }
    ngselcaisle() { this.crbay.lsaisle = this.slcaisle.value; }
    newbay() { 
        this.crbay = new locup_md();
        this.crbay.orgcode = this.av.crProfile.orgcode;
        this.crbay.site = this.av.crRole.site;
        this.crbay.depot = this.av.crRole.depot;
        this.crbay.fltype = "BA";
        this.crbay.lszone = "";
        this.crbay.lscode = "";
        this.crbay.lsdesc = "";
        this.crbay.lsseq = 0;
        this.crbay.tflow = "NW";
        this.crbay.tflowcnt = "NW";
        this.crbay.datemodify = new Date();
        this.crbay.accnmodify = this.av.crProfile.accncode;
        this.crbay.lshash = "NW";
        this.crbay.lsformat = "";
        this.crbay.lsbay = "";
        this.crbay.lslevel = "";
        this.crbay.lscodealt = "";
        this.crbay.lscodefull = "";
        this.crbay.spcarea = "ST";
        this.crbay.lscodeid = "";
        this.crstate = true;
        this.slcaisle = null;
        this.slczone = null;
        this.crstate = true;
        if(this.lsbay.findIndex(x=>x.lshash =="NW") == -1) {this.lsbay.push(this.crbay);}        
        this.toastr.info("<span class='fn-1e15'>New bay is ready to setup</span>",null,{ enableHtml : true });
    }
    fndbay(){ 
      this.pmzone.fltype = "BA";
      this.pmzone.lszone = "";      
        this.sv.fndlocup(this.pmzone).pipe().subscribe( (res) => { this.lsbay = res; });
    }
    selbay(o:locup_md){
       this.crbay = o; 
       this.crstate = (this.crbay.tflow == "IO") ? true : false;
       this.slczone  = this.lszone.find(e=>e.value == this.crbay.lszone);
       this.slcaisle = this.lsaisle.find(e=>e.value == this.crbay.lsaisle);
    }
    validate() {

      if (this.slczone == null) { 
        this.toastr.warning("<span class='fn-08e'>Zone is require</span>"); 
      } else if (this.slcaisle == null) {
        this.toastr.warning("<span class='fn-08e'>Aisle is require</span>"); 
      } else if (this.crbay.lscode == "") { 
          this.toastr.warning("<span class='fn-08e'>bay code is require</span>"); 
      } else { 

      this.crbay.tflow = (this.crbay.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
      this.crbay.lscodefull = this.crbay.orgcode + "-" + this.crbay.site+ "-" +this.crbay.depot+ "-" + this.crbay.lszone + "-" + this.crbay.lscode;
      this.crbay.lszone = this.slczone.value;
      this.crbay.lsaisle = this.slcaisle.value;
      this.ngPopups.confirm('Do you accept change of bay?')
        .subscribe(res => {
            if (res) {
              this.ison = true;
              this.crbay.tflowcnt = this.crbay.tflow;
              this.sv.upsertlocup(this.crbay).pipe().subscribe(            
                  (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndbay(); },
                  (err) => { this.toastr.error(err.error.message); this.ison = false; },
                  () => { }
              );
               
            } 
        });
      }
    
    }
    drop() { 
      this.ngPopups.confirm('Do you accept to drop bay ?')
      .subscribe(res => { 
          if (res) {
            this.ison = true;
            this.sv.droplocup(this.crbay).pipe().subscribe(            
                (res) => { this.toastr.success('Drop aisle successful'); this.ison = false; this.fndbay(); }
            );              
          } 
      });
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
      this.crbay = null;    delete this.crbay;
      this.pmzone = null;   delete this.pmzone;
      this.slczone = null;  delete this.slczone;
      this.slcaisle = null; delete this.slcaisle;
      this.slcbay = null;   delete this.slcbay;
      this.msdstate = null; delete this.msdstate;
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
