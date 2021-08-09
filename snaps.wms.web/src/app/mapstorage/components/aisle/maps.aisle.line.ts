import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from 'src/app/auth/services/auth.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { lov } from '../../../helpers/lov';
import { locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-aisleline',
  templateUrl: 'maps.aisle.line.html',
  styles: ['.dgaisle { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class mapsaisleline implements OnInit, OnDestroy {
  @Input() item: lov;
//   @Input() iconstate: string;
    ison:boolean=false;
    public lszone:lov[] = new Array();
    public lsaisle:locup_md[] = new Array();
    public craisle:locup_md = new locup_md();
    public pmaisle:locup_pm = new locup_pm();
    public slczone:lov;

    public msdstate:lov[] = new Array();
    public crstate:Boolean = true;

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
        this.pmaisle.orgcode = this.av.crProfile.orgcode;
        this.pmaisle.site = this.av.crRole.site;
        this.pmaisle.depot = this.av.crRole.depot;
        this.craisle.tflow = "IO";
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.mv.ngSetup();   this.fndaisle(); } 
    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselczone(){ this.craisle.lszone = this.slczone.value; this.craisle.spcarea = this.slczone.valopnfirst }
    ngSetup(){ this.mv.lovzone().subscribe((res) => { this.lszone = res.filter(e=>e.valopnfirst != 'XD');  } );  }
    newaisle() { 
        this.craisle = new locup_md
        this.craisle.orgcode = this.av.crProfile.orgcode;
        this.craisle.site = this.av.crRole.site;
        this.craisle.depot = this.av.crRole.depot;
        this.craisle.fltype = "AL";
        this.craisle.lszone = "";
        this.craisle.lscode = "";
        this.craisle.lsdesc = "";
        this.craisle.lsseq = 0;
        this.craisle.tflow = "NW";
        this.craisle.tflowcnt = "NW";
        this.craisle.datemodify = new Date();
        this.craisle.accnmodify = this.av.crProfile.accncode;
        this.craisle.lshash = "NW";
        this.craisle.lsformat = "";
        this.craisle.lsaisle = "";
        this.craisle.lsbay = "";
        this.craisle.lslevel = "";
        this.craisle.lscodealt = "";
        this.craisle.lscodefull = "";
        this.craisle.lscodeid = "";
        this.craisle.spcarea = "ST";
        this.craisle.lscodeid = "";
        this.crstate = true;
        this.slczone = null;
        if(this.lsaisle.findIndex(x=>x.lshash =="NW") == -1) {this.lsaisle.push(this.craisle);}        
        this.toastr.info("<span class='fn-08e'>New aisle is ready to setup</span>",null,{ enableHtml : true });
    }
    fndaisle(){ 
      this.pmaisle.fltype = "AL";
        this.sv.fndlocup(this.pmaisle).pipe().subscribe(            
            (res) => { this.lsaisle = res; if( this.lsaisle.length > 0) { this.selaisle(this.lsaisle[0]); } }
        );
    }
    selaisle(o:locup_md){ this.craisle = o; this.crstate = (this.craisle.tflow == "IO") ? true : false; this.slczone = this.lszone.find(e=>e.value == this.craisle.lszone); }
    dsczone(o:string){ 
      try{
        return this.lszone.find(x=>x.value == o).desc;
      } catch (error) { return ""; }
    }
    validate() {
      //this.craisle.lscodefull = this.craisle.orgcode + "-" + this.craisle.site+ "-" +this.craisle.depot+ "-" +this.craisle.lsaisle+ "-" +this.craisle.lscode;
      if (this.slczone == null) { 
        this.toastr.warning("<span class='fn-08e'>Zone is require</span>"); 
      } else if (this.craisle.lscode == "") { 
          this.toastr.warning("<span class='fn-08e'>Aisle code is require</span>"); 
      } else { 

        this.craisle.lszone = this.slczone.value;
        this.craisle.lsaisle = this.craisle.lscode;
        this.craisle.tflow = (this.craisle.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
        this.craisle.lscodefull = this.craisle.orgcode + "-" + this.craisle.site+ "-" +this.craisle.depot+ "-" + this.craisle.lszone + "-" + this.craisle.lscode;       
        this.ngPopups.confirm('Do you accept change of aisle?')
        .subscribe(res => { 
            if (res) {
              this.ison = true;
              this.craisle.tflowcnt = this.craisle.tflow;
              this.sv.upsertlocup(this.craisle).pipe().subscribe(            
                  (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndaisle(); }
              );
                
            } 
        });
      }

    }
    drop() { 
      this.ngPopups.confirm('Do you accept to drop the aisle ?')
      .subscribe(res => { 
          if (res) {
            this.ison = true;
            this.sv.droplocup(this.craisle).pipe().subscribe(            
                (res) => { this.toastr.success('Drop aisle successful'); this.ison = false; this.fndaisle(); }
            );              
          } 
      });
    }

    ngDecZone(o:string) { try { return this.lszone.find(e=>e.value == o).desc; } catch(exp){ return o; }}
    ngDecArea(o:string) { return this.mv.ngDecArea(o); }
    ngDecState(o:string){ return this.mv.ngDecState(o); }
    ngDecIcon(o:string) { return this.mv.ngDecIcon(o); }

    ngOnDestroy():void{ 
      this.ison = null; delete this.ison;
      this.lszone = null; delete this.lszone;
      this.lsaisle = null; delete this.lsaisle;
      this.craisle = null; delete this.craisle;
      this.pmaisle = null; delete this.pmaisle;
      this.slczone = null; delete this.slczone;  
      this.msdstate = null; delete this.msdstate;
      this.crstate = null; delete this.crstate;  
      this.formatdate = null; delete this.formatdate;
      this.formatdatelong = null; delete this.formatdatelong;
    }

}
