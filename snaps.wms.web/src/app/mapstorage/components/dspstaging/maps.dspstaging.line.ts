import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbPaginationConfig, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
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
  selector: 'appmaps-dspstagingline',
  templateUrl: 'maps.dspstaging.line.html',
  styles: ['.dgdspstaging { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsdspstaginglineComponent implements OnInit, OnDestroy {

  ison:boolean=false;
  public lsdspstaging:locup_md[] = new Array();
  public crdspstaging:locup_md = new locup_md();
  public lndspstaging:locdw_md = new locdw_md();

  public pmdspstaging:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcdspstaging:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fnddspstaging(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcdspstaging(){ this.crdspstaging.lszone = this.slcdspstaging.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newdspstaging() { 
        this.crdspstaging = new locup_md();
        this.crdspstaging.orgcode = this.av.crProfile.orgcode;
        this.crdspstaging.site = this.av.crRole.site;
        this.crdspstaging.depot = this.av.crRole.depot;
        this.crdspstaging.spcarea = "DS"; 
        this.crdspstaging.fltype = "BL";
        this.crdspstaging.lszone = "";
        this.crdspstaging.lscode = "";
        this.crdspstaging.lsdesc = "";
        this.crdspstaging.lsseq = 0;
        this.crdspstaging.tflow = "NW";
        this.crdspstaging.tflowcnt = "IO";
        this.crdspstaging.datemodify = new Date();
        this.crdspstaging.accnmodify = this.av.crProfile.accncode;
        this.crdspstaging.lshash = "NW";
        this.crdspstaging.lsformat = "";       
        this.crdspstaging.lsbay = "";
        this.crdspstaging.lslevel = "";
        this.crdspstaging.lscodealt = "";
        this.crdspstaging.lscodefull = "";
        this.crdspstaging.lsaisle = "";
        this.crdspstaging.spcarea = "";
        this.crdspstaging.lsformat = "";
        this.crdspstaging.lsdesc = "";

        this.lndspstaging = new locdw_md();
        this.lndspstaging.crfreepct = 100;
        this.lndspstaging.crvolume = 0;
        this.lndspstaging.crweight = 0;
        this.lndspstaging.depot = this.av.crRole.depot;
        this.lndspstaging.fltype = "BL";
        this.lndspstaging.lsaisle = "";
        this.lndspstaging.lsbay = "";
        this.lndspstaging.lscode = "";
        this.lndspstaging.lsdesc = "";
        this.lndspstaging.lsdigit = "";
        this.lndspstaging.lsgapbuttom = 0;
        this.lndspstaging.lsgapleft = 0;
        this.lndspstaging.lsgapright = 0;
        this.lndspstaging.lsgaptop = 0;
        this.lndspstaging.lshash = "000";
        this.lndspstaging.lslevel = "";
        this.lndspstaging.lsdmlength = 0;
        this.lndspstaging.lsdmwidth = 0;
        this.lndspstaging.lsdmheight = 0;
        this.lndspstaging.lsmixage = 0;
        this.lndspstaging.lsmixarticle = 0;
        this.lndspstaging.lsmixlotno = 0;
        this.lndspstaging.lsmnsafety = 0;
        this.lndspstaging.lsmxheight = 0;
        this.lndspstaging.lsmxhuno = 9999999; 
        this.lndspstaging.lsmxlength = 9999999;
        this.lndspstaging.lsmxvolume = 9999999;
        this.lndspstaging.lsmxweight = 9999999;
        this.lndspstaging.lsmxwidth = 9999999;
        this.lndspstaging.lsremarks = ""; 
        this.lndspstaging.lsstack = "1";
        this.lndspstaging.orgcode = this.av.crProfile.orgcode;
        this.lndspstaging.procmodify = "locdspstaging";
        this.lndspstaging.site = this.av.crRole.site;
        this.lndspstaging.spcarea = "DS"; 
        this.lndspstaging.spcarticle = "";
        this.lndspstaging.spcblock = 0; 
        this.lndspstaging.spchuno = "";
        this.lndspstaging.spclasttouch = "";
        this.lndspstaging.spcpickunit = "";
        this.lndspstaging.spcpicking = 0;
        this.lndspstaging.spcpivot = "";
        this.lndspstaging.spcseqpath = 0;
        this.lndspstaging.spctaskfnd = "";
        this.lndspstaging.spcthcode = "";
        this.lndspstaging.tflow = "IO";  
        this.lndspstaging.tflowcnt = ""
        this.lndspstaging.spcrpn = "";
        this.lndspstaging.lsloc = "";


        if(this.lsdspstaging.findIndex(x=>x.lshash =="NW") == -1) {this.lsdspstaging.push(this.crdspstaging);}        
        this.toastr.info("<span class='fn-07e'>New dspstaging is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crdspstaging.lsformat,"AA","","","",""));
    }
    fnddspstaging(){ 
      this.pmdspstaging.fltype = "BL";
      this.pmdspstaging.spcarea = "DS";
        this.sv.fndlocup(this.pmdspstaging).pipe().subscribe(            
            (res) => { this.lsdspstaging = res; if (this.lsdspstaging.length > 0) { this.seldspstaging(this.lsdspstaging[0]);} }
        );
    }

    seldspstaging(o:locup_md){ 
      this.crdspstaging = o; this.crstate = (this.crdspstaging.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lndspstaging = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lndspstaging.lsloctype);
      });
    }
    validate() {
    this.crdspstaging.tflow = (this.crdspstaging.tflow == "NW") ? "NW" :  (this.crstate == true) ? "IO" : "XX";
    this.crdspstaging.lscodefull = this.crdspstaging.orgcode + "-" + this.crdspstaging.site+ "-" +this.crdspstaging.depot+ "-" + this.crdspstaging.lscodealt;
    //this.crdspstaging.lscodeid = this.crdspstaging.orgcode + "-" + this.crdspstaging.site+ "-" +this.crdspstaging.depot+ "-" + this.crdspstaging.lscodealt;
    this.crdspstaging.lszone = this.crdspstaging.lscode;
    this.crdspstaging.spcarea = "DS";
    if (this.crdspstaging.lscode == "") { this.crdspstaging.spcarea = this.slccategory.value; }        
    this.lndspstaging.lszone = this.crdspstaging.lscode;
    this.lndspstaging.lscode = this.crdspstaging.lscode;
    this.lndspstaging.spcarea = this.crdspstaging.spcarea;
    this.lndspstaging.lsloctype = this.slccategory.value;
    this.lndspstaging.lscodealt = this.crdspstaging.lscodealt;
    this.lndspstaging.lscodefull = this.crdspstaging.orgcode + "-" + this.crdspstaging.site+ "-" +this.crdspstaging.depot+ "-" + this.crdspstaging.lscode;
    this.ngPopups.confirm('Do you accept change of dspstaging ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crdspstaging.tflowcnt = this.crdspstaging.tflow;
                this.sv.upsertlocup(this.crdspstaging).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup dspstaging area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crdspstaging.lshash = "-";
                      this.sv.upsertlocdw(this.lndspstaging).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup dspstaging storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crdspstaging.lshash = "-"; this.fnddspstaging(); }
                      );
                    }
                );                
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop dspstaging ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crdspstaging.tflowcnt = this.crdspstaging.tflow;
              this.sv.droplocup(this.crdspstaging).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lndspstaging).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop dspstaging storage success</span>",null,{ enableHtml : true }); 
                        this.fnddspstaging(); 
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
      this.mv.getlov("dspstaging","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsdspstaging    = null; delete this.lsdspstaging;
      this.crdspstaging    = null; delete this.crdspstaging;
      this.lndspstaging    = null; delete this.lndspstaging;
      this.pmdspstaging    = null; delete this.pmdspstaging;
      this.pmdw           = null; delete this.pmdw;
      this.slcdspstaging   = null; delete this.slcdspstaging;
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
