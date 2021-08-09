import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbPaginationConfig, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import {  locdw_md, locdw_pm,  locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-overflowline',
  templateUrl: 'maps.overflow.line.html',
  styles: ['.dgoverflow { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsoverflowlineComponent implements OnInit, OnDestroy {

  ison:boolean=false;
  public lsoverflow:locup_md[] = new Array();
  public croverflow:locup_md = new locup_md();
  public lnoverflow:locdw_md = new locdw_md();

  public pmoverflow:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcoverflow:lov = new lov();

  public slccategory:lov = new lov();
  public msdstate:lov[] = new Array();
  public msdcategory:lov[] = new Array();
  public crstate:boolean = true;
  public spcareadsc:string;

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
        this.av.retriveAccess(); 
        this.formatdate = this.av.crProfile.formatdate;
        this.formatdatelong = this.av.crProfile.formatdatelong;
    }

    ngOnInit() { }
    ngAfterViewInit(){ this.ngSetup(); this.fndoverflow(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcoverflow(){ this.croverflow.lszone = this.slcoverflow.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newoverflow() { 
        this.croverflow = new locup_md();
        this.croverflow.orgcode = this.av.crProfile.orgcode;
        this.croverflow.site = this.av.crRole.site;
        this.croverflow.depot = this.av.crRole.depot;
        this.croverflow.spcarea = "OV"; 
        this.croverflow.fltype = "BL";
        this.croverflow.lszone = "";
        this.croverflow.lscode = "";
        this.croverflow.lsdesc = "";
        this.croverflow.lsseq = 0;
        this.croverflow.tflow = "NW";
        this.croverflow.tflowcnt = "IO";
        this.croverflow.datemodify = new Date();
        this.croverflow.accnmodify = this.av.crProfile.accncode;
        this.croverflow.lshash = "NW";
        this.croverflow.lsformat = "";       
        this.croverflow.lsbay = "";
        this.croverflow.lslevel = "";
        this.croverflow.lscodealt = "";
        this.croverflow.lscodefull = "";
        this.croverflow.lsaisle = "";
        this.croverflow.spcarea = "";
        this.croverflow.lsformat = "";
        this.croverflow.lsdesc = "";

        this.lnoverflow = new locdw_md();
        this.lnoverflow.crfreepct = 100;
        this.lnoverflow.crvolume = 0;
        this.lnoverflow.crweight = 0;
        this.lnoverflow.depot = this.av.crRole.depot;
        this.lnoverflow.fltype = "BL";
        this.lnoverflow.lsaisle = "";
        this.lnoverflow.lsbay = "";
        this.lnoverflow.lscode = "";
        this.lnoverflow.lsdesc = "";
        this.lnoverflow.lsdigit = "";
        this.lnoverflow.lsgapbuttom = 0;
        this.lnoverflow.lsgapleft = 0;
        this.lnoverflow.lsgapright = 0;
        this.lnoverflow.lsgaptop = 0;
        this.lnoverflow.lshash = "000";
        this.lnoverflow.lslevel = "";
        this.lnoverflow.lsdmlength = 0;
        this.lnoverflow.lsdmwidth = 0;
        this.lnoverflow.lsdmheight = 0;
        this.lnoverflow.lsmixage = 0;
        this.lnoverflow.lsmixarticle = 0;
        this.lnoverflow.lsmixlotno = 0;
        this.lnoverflow.lsmnsafety = 0;
        this.lnoverflow.lsmxheight = 0;
        this.lnoverflow.lsmxhuno = 9999999; 
        this.lnoverflow.lsmxlength = 9999999;
        this.lnoverflow.lsmxvolume = 9999999;
        this.lnoverflow.lsmxweight = 9999999;
        this.lnoverflow.lsmxwidth = 9999999;
        this.lnoverflow.lsremarks = ""; 
        this.lnoverflow.lsstack = "1";
        this.lnoverflow.orgcode = this.av.crProfile.orgcode;
        this.lnoverflow.procmodify = "locoverflow";
        this.lnoverflow.site = this.av.crRole.site;
        this.lnoverflow.spcarea = "OV"; 
        this.lnoverflow.spcarticle = "";
        this.lnoverflow.spcblock = 0; 
        this.lnoverflow.spchuno = "";
        this.lnoverflow.spclasttouch = "";
        this.lnoverflow.spcpickunit = "";
        this.lnoverflow.spcpicking = 0;
        this.lnoverflow.spcpivot = "";
        this.lnoverflow.spcseqpath = 0;
        this.lnoverflow.spctaskfnd = "";
        this.lnoverflow.spcthcode = "";
        this.lnoverflow.tflow = "NW";  
        this.lnoverflow.tflowcnt = ""
        this.lnoverflow.spcrpn = "";
        this.lnoverflow.lsloc = "";


        if(this.lsoverflow.findIndex(x=>x.lshash =="NW") == -1) {this.lsoverflow.push(this.croverflow);}        
        this.toastr.info("<span class='fn-07e'>New overflow is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.croverflow.lsformat,"AA","","","",""));
    }
    fndoverflow(){ 
      this.pmoverflow.fltype = "BL";
      this.pmoverflow.spcarea = "OV";
        this.sv.fndlocup(this.pmoverflow).pipe().subscribe(            
            (res) => { this.lsoverflow = res; },
            (err) => { this.toastr.error(err.error.message); this.ison = false; },
            () => { }
        );
    }

    seloverflow(o:locup_md){ 
      this.croverflow = o; this.crstate = (this.croverflow.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnoverflow = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnoverflow.lsloctype);
      });
    }
    validate() {
    this.croverflow.tflow =  (this.croverflow.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.croverflow.lscodefull = this.croverflow.orgcode + "-" + this.croverflow.site+ "-" +this.croverflow.depot+ "-" + this.croverflow.lscodealt;
    //this.croverflow.lscodeid = this.croverflow.orgcode + "-" + this.croverflow.site+ "-" +this.croverflow.depot+ "-" + this.croverflow.lscodealt;
    this.croverflow.lszone = this.croverflow.lscode;
    this.croverflow.spcarea = "OV";
    if (this.croverflow.lscode == "") { this.croverflow.spcarea = this.slccategory.value; }
    this.lnoverflow.tflow =  (this.croverflow.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";     
    this.lnoverflow.lszone = this.croverflow.lscode;
    this.lnoverflow.lscode = this.croverflow.lscode;
    this.lnoverflow.spcarea = this.croverflow.spcarea;
    this.lnoverflow.lsloctype = this.slccategory.value;
    this.lnoverflow.lscodealt = this.croverflow.lscodealt;
    this.lnoverflow.lscodefull = this.croverflow.orgcode + "-" + this.croverflow.site+ "-" +this.croverflow.depot+ "-" + this.croverflow.lscode;
    this.ngPopups.confirm('Do you accept change of overflow ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.croverflow.tflowcnt = this.croverflow.tflow;
                this.sv.upsertlocup(this.croverflow).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup overflow area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.croverflow.lshash = "-"; 
                      this.sv.upsertlocdw(this.lnoverflow).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup overflow storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.croverflow.lshash = "-"; this.fndoverflow(); }
                      );
                    }
                );                  
                
                                  
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop overflow ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.croverflow.tflowcnt = this.croverflow.tflow;
              this.sv.droplocup(this.croverflow).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnoverflow).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop overflow storage success</span>",null,{ enableHtml : true }); 
                        this.fndoverflow(); 
                      }
                    );
                  }
              );                  
              
          } 
      });
    }
    ngDecArea(o:string) { return this.mv.ngDecArea(o); }
    ngDecState(o:string){ return this.mv.ngDecState(o); }
    ngDecIcon(o:string) { return this.mv.ngDecIcon(o); }
    ngChangeRowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
    ngSetup() { 
      this.mv.getlov("OVERFLOW","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsoverflow    = null; delete this.lsoverflow;
      this.croverflow    = null; delete this.croverflow;
      this.lnoverflow    = null; delete this.lnoverflow;
      this.pmoverflow    = null; delete this.pmoverflow;
      this.pmdw           = null; delete this.pmdw;
      this.slcoverflow   = null; delete this.slcoverflow;
      this.slccategory    = null; delete this.slccategory;
      this.msdstate       = null; delete this.msdstate;
      this.msdcategory    = null; delete this.msdcategory;
      this.crstate        = null; delete this.crstate;
      this.spcareadsc     = null; delete this.spcareadsc;
      this.page           = null; delete this.page;
      this.pageSize       = null; delete this.pageSize;
      this.slrowlmt       = null; delete this.slrowlmt;
      this.lsrowlmt       = null; delete this.lsrowlmt;
      this.lssort         = null; delete this.lssort;
      this.lsreverse      = null; delete this.lsreverse;
      this.formatdate     = null; delete this.formatdate;
      this.formatdatelong = null; delete this.formatdatelong ;
    }
}
