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
  selector: 'appmaps-forwardline',
  templateUrl: 'maps.forwarding.line.html',
  styles: ['.dgforward { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsforwardlineComponent implements OnInit, OnDestroy {

  ison:boolean=false;
  public lsforward:locup_md[] = new Array();
  public crforward:locup_md = new locup_md();
  public lnforward:locdw_md = new locdw_md();

  public pmforward:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcforward:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndforward(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcforward(){ this.crforward.lszone = this.slcforward.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newforward() { 
        this.crforward = new locup_md();
        this.crforward.orgcode = this.av.crProfile.orgcode;
        this.crforward.site = this.av.crRole.site;
        this.crforward.depot = this.av.crRole.depot;
        this.crforward.spcarea = "FW"; 
        this.crforward.fltype = "BL";
        this.crforward.lszone = "";
        this.crforward.lscode = "";
        this.crforward.lsdesc = "";
        this.crforward.lsseq = 0;
        this.crforward.tflow = "NW";
        this.crforward.tflowcnt = "IO";
        this.crforward.datemodify = new Date();
        this.crforward.accnmodify = this.av.crProfile.accncode;
        this.crforward.lshash = "NW";
        this.crforward.lsformat = "";       
        this.crforward.lsbay = "";
        this.crforward.lslevel = "";
        this.crforward.lscodealt = "";
        this.crforward.lscodefull = "";
        this.crforward.lsaisle = "";
        this.crforward.spcarea = "";
        this.crforward.lsformat = "";
        this.crforward.lsdesc = "";

        this.lnforward = new locdw_md();
        this.lnforward.crfreepct = 100;
        this.lnforward.crvolume = 0;
        this.lnforward.crweight = 0;
        this.lnforward.depot = this.av.crRole.depot;
        this.lnforward.fltype = "BL";
        this.lnforward.lsaisle = "";
        this.lnforward.lsbay = "";
        this.lnforward.lscode = "";
        this.lnforward.lsdesc = "";
        this.lnforward.lsdigit = "";
        this.lnforward.lsgapbuttom = 0;
        this.lnforward.lsgapleft = 0;
        this.lnforward.lsgapright = 0;
        this.lnforward.lsgaptop = 0;
        this.lnforward.lshash = "000";
        this.lnforward.lslevel = "";
        this.lnforward.lsdmlength = 0;
        this.lnforward.lsdmwidth = 0;
        this.lnforward.lsdmheight = 0;
        this.lnforward.lsmixage = 1;
        this.lnforward.lsmixarticle = 1;
        this.lnforward.lsmixlotno = 1;
        this.lnforward.lsmnsafety = 0;
        this.lnforward.lsmxheight = 0;
        this.lnforward.lsmxhuno = 9999999; 
        this.lnforward.lsmxlength = 9999999;
        this.lnforward.lsmxvolume = 9999999;
        this.lnforward.lsmxweight = 9999999;
        this.lnforward.lsmxwidth = 9999999;
        this.lnforward.lsremarks = ""; 
        this.lnforward.lsstack = "1";
        this.lnforward.orgcode = this.av.crProfile.orgcode;
        this.lnforward.procmodify = "locforward";
        this.lnforward.site = this.av.crRole.site;
        this.lnforward.spcarea = "FW"; 
        this.lnforward.spcarticle = "";
        this.lnforward.spcblock = 0; 
        this.lnforward.spchuno = "";
        this.lnforward.spclasttouch = "";
        this.lnforward.spcpickunit = "";
        this.lnforward.spcpicking = 0;
        this.lnforward.spcpivot = "";
        this.lnforward.spcseqpath = 0;
        this.lnforward.spctaskfnd = "";
        this.lnforward.spcthcode = "";
        this.lnforward.tflow = "NW";  
        this.lnforward.tflowcnt = ""
        this.lnforward.spcrpn = "";
        this.lnforward.lsloc = "";


        if(this.lsforward.findIndex(x=>x.lshash =="NW") == -1) {this.lsforward.push(this.crforward);}        
        this.toastr.info("<span class='fn-07e'>New forward is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crforward.lsformat,"AA","","","",""));
    }
    fndforward(){ 
      this.pmforward.fltype = "BL";
      this.pmforward.spcarea = "FW";
        this.sv.fndlocup(this.pmforward).pipe().subscribe(            
            (res) => { this.lsforward = res; if (this.lsforward.length > 0) { this.selforward(this.lsforward[0]); } }
        );
    }

    selforward(o:locup_md){ 
      this.crforward = o; this.crstate = (this.crforward.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnforward = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnforward.lsloctype);
      });
    }
    validate() {
    this.crforward.tflow =  (this.crforward.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crforward.lscodefull = this.crforward.orgcode + "-" + this.crforward.site+ "-" +this.crforward.depot+ "-" + this.crforward.lscodealt;
   // this.crforward.lscodeid = this.crforward.orgcode + "-" + this.crforward.site+ "-" +this.crforward.depot+ "-" + this.crforward.lscodealt;
    this.crforward.lszone = this.crforward.lscode;
    this.crforward.spcarea = "FW";
    if (this.crforward.lscode == "") { this.crforward.spcarea = this.slccategory.value; } 
    this.lnforward.tflow =  (this.crforward.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";          
    this.lnforward.lszone = this.crforward.lscode;
    this.lnforward.lscode = this.crforward.lscode;
    this.lnforward.spcarea = this.crforward.spcarea;
    this.lnforward.lsloctype = this.slccategory.value;
    this.lnforward.lscodealt = this.crforward.lscodealt;
    this.lnforward.lscodefull = this.crforward.orgcode + "-" + this.crforward.site+ "-" +this.crforward.depot+ "-" + this.crforward.lscode;
    this.ngPopups.confirm('Do you accept change of forward ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crforward.tflowcnt = this.crforward.tflow;
                this.sv.upsertlocup(this.crforward).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup forward area success</span>",null,{ enableHtml : true }); 
                      this.sv.upsertlocdw(this.lnforward).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup forward storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crforward.lshash = "-";  this.fndforward();  }
                      );
                    }
                );                  
                
                                  
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop forward ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crforward.tflowcnt = this.crforward.tflow;
              this.sv.droplocup(this.crforward).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnforward).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop forward storage success</span>",null,{ enableHtml : true }); 
                        this.fndforward();
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
      this.mv.getlov("forward","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsforward    = null; delete this.lsforward;
      this.crforward    = null; delete this.crforward;
      this.lnforward    = null; delete this.lnforward;
      this.pmforward    = null; delete this.pmforward;
      this.pmdw           = null; delete this.pmdw;
      this.slcforward   = null; delete this.slcforward;
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
