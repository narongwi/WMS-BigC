import { Component, OnInit, Input } from '@angular/core';
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
  selector: 'appmaps-sinbinline',
  templateUrl: 'maps.sinbin.line.html',
  styles: ['.dgsinbin { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapssinbinlineComponent implements OnInit {
  ison:boolean=false;
  public lssinbin:locup_md[] = new Array();
  public crsinbin:locup_md = new locup_md();
  public lnsinbin:locdw_md = new locdw_md();

  public pmsinbin:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcsinbin:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndsinbin(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcsinbin(){ this.crsinbin.lszone = this.slcsinbin.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newsinbin() { 
        this.crsinbin = new locup_md();
        this.crsinbin.orgcode = this.av.crProfile.orgcode;
        this.crsinbin.site = this.av.crRole.site;
        this.crsinbin.depot = this.av.crRole.depot;
        this.crsinbin.spcarea = "SB"; 
        this.crsinbin.fltype = "BL";
        this.crsinbin.lszone = "";
        this.crsinbin.lscode = "";
        this.crsinbin.lsdesc = "";
        this.crsinbin.lsseq = 0;
        this.crsinbin.tflow = "NW";
        this.crsinbin.tflowcnt = "IO";
        this.crsinbin.datemodify = new Date();
        this.crsinbin.accnmodify = this.av.crProfile.accncode;
        this.crsinbin.lshash = "NW";
        this.crsinbin.lsformat = "";       
        this.crsinbin.lsbay = "";
        this.crsinbin.lslevel = "";
        this.crsinbin.lscodealt = "";
        this.crsinbin.lscodefull = "";
        this.crsinbin.lsaisle = "";
        this.crsinbin.spcarea = "";
        this.crsinbin.lsformat = "";
        this.crsinbin.lsdesc = "";

        this.lnsinbin = new locdw_md();
        this.lnsinbin.crfreepct = 100;
        this.lnsinbin.crvolume = 0;
        this.lnsinbin.crweight = 0;
        this.lnsinbin.depot = this.av.crRole.depot;
        this.lnsinbin.fltype = "BL";
        this.lnsinbin.lsaisle = "";
        this.lnsinbin.lsbay = "";
        this.lnsinbin.lscode = "";
        this.lnsinbin.lsdesc = "";
        this.lnsinbin.lsdigit = "";
        this.lnsinbin.lsgapbuttom = 0;
        this.lnsinbin.lsgapleft = 0;
        this.lnsinbin.lsgapright = 0;
        this.lnsinbin.lsgaptop = 0;
        this.lnsinbin.lshash = "000";
        this.lnsinbin.lslevel = "";
        this.lnsinbin.lsdmlength = 0;
        this.lnsinbin.lsdmwidth = 0;
        this.lnsinbin.lsdmheight = 0;
        this.lnsinbin.lsmixage = 0;
        this.lnsinbin.lsmixarticle = 0;
        this.lnsinbin.lsmixlotno = 0;
        this.lnsinbin.lsmnsafety = 0;
        this.lnsinbin.lsmxheight = 0;
        this.lnsinbin.lsmxhuno = 9999999; 
        this.lnsinbin.lsmxlength = 9999999;
        this.lnsinbin.lsmxvolume = 9999999;
        this.lnsinbin.lsmxweight = 9999999;
        this.lnsinbin.lsmxwidth = 9999999;
        this.lnsinbin.lsremarks = ""; 
        this.lnsinbin.lsstack = "1";
        this.lnsinbin.orgcode = this.av.crProfile.orgcode;
        this.lnsinbin.procmodify = "locsinbin";
        this.lnsinbin.site = this.av.crRole.site;
        this.lnsinbin.spcarea = "SB"; 
        this.lnsinbin.spcarticle = "";
        this.lnsinbin.spcblock = 0; 
        this.lnsinbin.spchuno = "";
        this.lnsinbin.spclasttouch = "";
        this.lnsinbin.spcpickunit = "";
        this.lnsinbin.spcpicking = 0;
        this.lnsinbin.spcpivot = "";
        this.lnsinbin.spcseqpath = 0;
        this.lnsinbin.spctaskfnd = "";
        this.lnsinbin.spcthcode = "";
        this.lnsinbin.tflow = "NW";  
        this.lnsinbin.tflowcnt = ""
        this.lnsinbin.spcrpn = "";
        this.lnsinbin.lsloc = "";


        if(this.lssinbin.findIndex(x=>x.lshash =="NW") == -1) {this.lssinbin.push(this.crsinbin);}        
        this.toastr.info("<span class='fn-1e15'>New sinbin is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crsinbin.lsformat,"AA","","","",""));
    }
    fndsinbin(){ 
      this.pmsinbin.fltype = "BL";
      this.pmsinbin.spcarea = "SB";
        this.sv.fndlocup(this.pmsinbin).pipe().subscribe(            
            (res) => { this.lssinbin = res; if (this.lssinbin.length > 0) { this.selsinbin(this.lssinbin[0]); } }
        );
    }

    selsinbin(o:locup_md){ 
      this.crsinbin = o; this.crstate = (this.crsinbin.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnsinbin = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnsinbin.lsloctype);
      });
    }
    validate() {
    this.crsinbin.tflow =  (this.crsinbin.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crsinbin.lscodefull = this.crsinbin.orgcode + "-" + this.crsinbin.site+ "-" +this.crsinbin.depot+ "-" + this.crsinbin.lscodealt;
    //this.crsinbin.lscodeid = this.crsinbin.orgcode + "-" + this.crsinbin.site+ "-" +this.crsinbin.depot+ "-" + this.crsinbin.lscodealt;
    this.crsinbin.lszone = this.crsinbin.lscode;
    this.crsinbin.spcarea = "SB";
    if (this.crsinbin.lscode == "") { this.crsinbin.spcarea = this.slccategory.value; }    
    this.lnsinbin.tflow =  (this.crsinbin.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";    
    this.lnsinbin.lszone = this.crsinbin.lscode;
    this.lnsinbin.lscode = this.crsinbin.lscode;
    this.lnsinbin.spcarea = this.crsinbin.spcarea;
    this.lnsinbin.lsloctype = this.slccategory.value;
    this.lnsinbin.lscodealt = this.crsinbin.lscodealt;
    this.lnsinbin.lscodefull = this.crsinbin.orgcode + "-" + this.crsinbin.site+ "-" +this.crsinbin.depot+ "-" + this.crsinbin.lscode;
    this.ngPopups.confirm('Do you accept change of sinbin ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crsinbin.tflowcnt = this.crsinbin.tflow;
                this.sv.upsertlocup(this.crsinbin).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-1e15'>Setup sinbin area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crsinbin.lshash = "-"; 
                      this.sv.upsertlocdw(this.lnsinbin).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-1e15'>Setup sinbin storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crsinbin.lshash = "-";  }
                      );
                      this.fndsinbin(); 
                    }
                );                  
                
                                  
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop sinbin ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crsinbin.tflowcnt = this.crsinbin.tflow;
              this.sv.droplocup(this.crsinbin).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnsinbin).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop sinbin storage success</span>",null,{ enableHtml : true }); 
                        this.fndsinbin();
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
      this.mv.getlov("sinbin","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lssinbin    = null; delete this.lssinbin;
      this.crsinbin    = null; delete this.crsinbin;
      this.lnsinbin    = null; delete this.lnsinbin;
      this.pmsinbin    = null; delete this.pmsinbin;
      this.pmdw           = null; delete this.pmdw;
      this.slcsinbin   = null; delete this.slcsinbin;
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
