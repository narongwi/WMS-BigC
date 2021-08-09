import { getLocaleDateFormat } from '@angular/common';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_ls, locdw_md, locdw_pm, locup_ls, locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-inbstagingline',
  templateUrl: 'maps.inbstaging.line.html',
  styles: ['.dginbstaging { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsinbstaginglineComponent implements OnInit, OnDestroy {

  ison:boolean=false;
  public lsinbstaging:locup_md[] = new Array();
  public crinbstaging:locup_md = new locup_md();
  public lninbstaging:locdw_md = new locdw_md();

  public pminbstaging:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcinbstaging:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndinbstaging(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcinbstaging(){ this.crinbstaging.lszone = this.slcinbstaging.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newinbstaging() { 
        this.crinbstaging = new locup_md();
        this.crinbstaging.orgcode = this.av.crProfile.orgcode;
        this.crinbstaging.site = this.av.crRole.site;
        this.crinbstaging.depot = this.av.crRole.depot;
        this.crinbstaging.spcarea = "RS"; 
        this.crinbstaging.fltype = "BL";
        this.crinbstaging.lszone = "";
        this.crinbstaging.lscode = "";
        this.crinbstaging.lsdesc = "";
        this.crinbstaging.lsseq = 0;
        this.crinbstaging.tflow = "NW";
        this.crinbstaging.tflowcnt = "IO";
        this.crinbstaging.datemodify = new Date();
        this.crinbstaging.accnmodify = this.av.crProfile.accncode;
        this.crinbstaging.lshash = "NW";
        this.crinbstaging.lsformat = "";       
        this.crinbstaging.lsbay = "";
        this.crinbstaging.lslevel = "";
        this.crinbstaging.lscodealt = "";
        this.crinbstaging.lscodefull = "";
        this.crinbstaging.lsaisle = "";
        this.crinbstaging.spcarea = "";
        this.crinbstaging.lsformat = "";
        this.crinbstaging.lsdesc = "";

        this.lninbstaging = new locdw_md();
        this.lninbstaging.crfreepct = 100;
        this.lninbstaging.crvolume = 0;
        this.lninbstaging.crweight = 0;
        this.lninbstaging.depot = this.av.crRole.depot;
        this.lninbstaging.fltype = "BL";
        this.lninbstaging.lsaisle = "";
        this.lninbstaging.lsbay = "";
        this.lninbstaging.lscode = "";
        this.lninbstaging.lsdesc = "";
        this.lninbstaging.lsdigit = "";
        this.lninbstaging.lsgapbuttom = 0;
        this.lninbstaging.lsgapleft = 0;
        this.lninbstaging.lsgapright = 0;
        this.lninbstaging.lsgaptop = 0;
        this.lninbstaging.lshash = "000";
        this.lninbstaging.lslevel = "";
        this.lninbstaging.lsdmlength = 0;
        this.lninbstaging.lsdmwidth = 0;
        this.lninbstaging.lsdmheight = 0;
        this.lninbstaging.lsmixage = 0;
        this.lninbstaging.lsmixarticle = 0;
        this.lninbstaging.lsmixlotno = 0;
        this.lninbstaging.lsmnsafety = 0;
        this.lninbstaging.lsmxheight = 0;
        this.lninbstaging.lsmxhuno = 9999999; 
        this.lninbstaging.lsmxlength = 9999999;
        this.lninbstaging.lsmxvolume = 9999999;
        this.lninbstaging.lsmxweight = 9999999;
        this.lninbstaging.lsmxwidth = 9999999;
        this.lninbstaging.lsremarks = ""; 
        this.lninbstaging.lsstack = "1";
        this.lninbstaging.orgcode = this.av.crProfile.orgcode;
        this.lninbstaging.procmodify = "locinbstaging";
        this.lninbstaging.site = this.av.crRole.site;
        this.lninbstaging.spcarea = "RS"; 
        this.lninbstaging.spcarticle = "";
        this.lninbstaging.spcblock = 0; 
        this.lninbstaging.spchuno = "";
        this.lninbstaging.spclasttouch = "";
        this.lninbstaging.spcpickunit = "";
        this.lninbstaging.spcpicking = 0;
        this.lninbstaging.spcpivot = "";
        this.lninbstaging.spcseqpath = 0;
        this.lninbstaging.spctaskfnd = "";
        this.lninbstaging.spcthcode = "";
        this.lninbstaging.tflow = "NW";  
        this.lninbstaging.tflowcnt = ""
        this.lninbstaging.spcrpn = "";
        this.lninbstaging.lsloc = "";


        if(this.lsinbstaging.findIndex(x=>x.lshash =="NW") == -1) {this.lsinbstaging.push(this.crinbstaging);}        
        this.toastr.info("<span class='fn-07e'>New inbstaging is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crinbstaging.lsformat,"AA","","","",""));
    }
    fndinbstaging(){ 
      this.pminbstaging.fltype = "BL";
      this.pminbstaging.spcarea = "RS";
        this.sv.fndlocup(this.pminbstaging).pipe().subscribe(            
            (res) => { this.lsinbstaging = res; if(this.lsinbstaging.length > 0) { this.selinbstaging(this.lsinbstaging[0]); } }
        );
    }

    selinbstaging(o:locup_md){ 
      this.crinbstaging = o; this.crstate = (this.crinbstaging.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lninbstaging = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lninbstaging.lsloctype);
      });
    }
    validate() {
    this.crinbstaging.tflow =  (this.crinbstaging.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crinbstaging.lscodefull = this.crinbstaging.orgcode + "-" + this.crinbstaging.site+ "-" +this.crinbstaging.depot+ "-" + this.crinbstaging.lscodealt;
    //this.crinbstaging.lscodeid = this.crinbstaging.orgcode + "-" + this.crinbstaging.site+ "-" +this.crinbstaging.depot+ "-" + this.crinbstaging.lscodealt;
    this.crinbstaging.lszone = this.crinbstaging.lscode;
    this.crinbstaging.spcarea = "RS";
    if (this.crinbstaging.lscode == "") { this.crinbstaging.spcarea = this.slccategory.value; }     
    this.lninbstaging.tflow =  (this.crinbstaging.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";   
    this.lninbstaging.lszone = this.crinbstaging.lscode;
    this.lninbstaging.lscode = this.crinbstaging.lscode;
    this.lninbstaging.spcarea = this.crinbstaging.spcarea;
    this.lninbstaging.lsloctype = this.slccategory.value;
    this.lninbstaging.lscodealt = this.crinbstaging.lscodealt;
    this.lninbstaging.lscodefull = this.crinbstaging.orgcode + "-" + this.crinbstaging.site+ "-" +this.crinbstaging.depot+ "-" + this.crinbstaging.lscode;
    this.ngPopups.confirm('Do you accept change of inbstaging ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crinbstaging.tflowcnt = this.crinbstaging.tflow;
                this.sv.upsertlocup(this.crinbstaging).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup inbstaging area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crinbstaging.lshash = "-"; 
                      this.sv.upsertlocdw(this.lninbstaging).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup inbstaging storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crinbstaging.lshash = "-"; this.fndinbstaging(); }
                      );
                    }
                );                
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop inbstaging ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crinbstaging.tflowcnt = this.crinbstaging.tflow;
              this.sv.droplocup(this.crinbstaging).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lninbstaging).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop inbstaging storage success</span>",null,{ enableHtml : true }); 
                        this.fndinbstaging();
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
      this.mv.getlov("INBSTAGING","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsinbstaging   = null; delete this.lsinbstaging;
      this.crinbstaging   = null; delete this.crinbstaging;
      this.lninbstaging   = null; delete this.lninbstaging;
      this.pminbstaging   = null; delete this.pminbstaging;
      this.pmdw           = null; delete this.pmdw;
      this.slcinbstaging  = null; delete this.slcinbstaging;
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
