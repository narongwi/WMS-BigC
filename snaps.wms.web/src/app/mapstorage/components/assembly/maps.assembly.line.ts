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
  selector: 'appmaps-assemblyline',
  templateUrl: 'maps.assembly.line.html',
  styles: ['.dgassembly { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsassemblylineComponent implements OnInit {

  ison:boolean=false;
  public lsassembly:locup_md[] = new Array();
  public crassembly:locup_md = new locup_md();
  public lnassembly:locdw_md = new locdw_md();

  public pmassembly:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcassembly:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndassembly(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcassembly(){ this.crassembly.lszone = this.slcassembly.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newassembly() { 
        this.crassembly = new locup_md();
        this.crassembly.orgcode = this.av.crProfile.orgcode;
        this.crassembly.site = this.av.crRole.site;
        this.crassembly.depot = this.av.crRole.depot;
        this.crassembly.spcarea = "AS"; 
        this.crassembly.fltype = "BL";
        this.crassembly.lszone = "";
        this.crassembly.lscode = "";
        this.crassembly.lsdesc = "";
        this.crassembly.lsseq = 0;
        this.crassembly.tflow = "NW";
        this.crassembly.tflowcnt = "IO";
        this.crassembly.datemodify = new Date();
        this.crassembly.accnmodify = this.av.crProfile.accncode;
        this.crassembly.lshash = "NW";
        this.crassembly.lsformat = "";       
        this.crassembly.lsbay = "";
        this.crassembly.lslevel = "";
        this.crassembly.lscodealt = "";
        this.crassembly.lscodefull = "";
        this.crassembly.lsaisle = "";
        this.crassembly.spcarea = "";
        this.crassembly.lsformat = "";
        this.crassembly.lsdesc = "";

        this.lnassembly = new locdw_md();
        this.lnassembly.crfreepct = 100;
        this.lnassembly.crvolume = 0;
        this.lnassembly.crweight = 0;
        this.lnassembly.depot = this.av.crRole.depot;
        this.lnassembly.fltype = "BL";
        this.lnassembly.lsaisle = "";
        this.lnassembly.lsbay = "";
        this.lnassembly.lscode = "";
        this.lnassembly.lsdesc = "";
        this.lnassembly.lsdigit = "";
        this.lnassembly.lsgapbuttom = 0;
        this.lnassembly.lsgapleft = 0;
        this.lnassembly.lsgapright = 0;
        this.lnassembly.lsgaptop = 0;
        this.lnassembly.lshash = "000";
        this.lnassembly.lslevel = "";
        this.lnassembly.lsdmlength = 0;
        this.lnassembly.lsdmwidth = 0;
        this.lnassembly.lsdmheight = 0;
        this.lnassembly.lsmixage = 0;
        this.lnassembly.lsmixarticle = 0;
        this.lnassembly.lsmixlotno = 0;
        this.lnassembly.lsmnsafety = 0;
        this.lnassembly.lsmxheight = 0;
        this.lnassembly.lsmxhuno = 9999999; 
        this.lnassembly.lsmxlength = 9999999;
        this.lnassembly.lsmxvolume = 9999999;
        this.lnassembly.lsmxweight = 9999999;
        this.lnassembly.lsmxwidth = 9999999;
        this.lnassembly.lsremarks = ""; 
        this.lnassembly.lsstack = "1";
        this.lnassembly.orgcode = this.av.crProfile.orgcode;
        this.lnassembly.procmodify = "locassembly";
        this.lnassembly.site = this.av.crRole.site;
        this.lnassembly.spcarea = "AS"; 
        this.lnassembly.spcarticle = "";
        this.lnassembly.spcblock = 0; 
        this.lnassembly.spchuno = "";
        this.lnassembly.spclasttouch = "";
        this.lnassembly.spcpickunit = "";
        this.lnassembly.spcpicking = 0;
        this.lnassembly.spcpivot = "";
        this.lnassembly.spcseqpath = 0;
        this.lnassembly.spctaskfnd = "";
        this.lnassembly.spcthcode = "";
        this.lnassembly.tflow = "NW";  
        this.lnassembly.tflowcnt = ""
        this.lnassembly.spcrpn = "";
        this.lnassembly.lsloc = "";


        if(this.lsassembly.findIndex(x=>x.lshash =="NW") == -1) {this.lsassembly.push(this.crassembly);}        
        this.toastr.info("<span class='fn-07e'>New assembly is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crassembly.lsformat,"AA","","","",""));
    }
    fndassembly(){ 
      this.pmassembly.fltype = "BL";
      this.pmassembly.spcarea = "AS";
        this.sv.fndlocup(this.pmassembly).pipe().subscribe(            
            (res) => { this.lsassembly = res;  if ( this.lsassembly.length > 0) { this.selassembly(this.lsassembly[0]); } }
        );
    }

    selassembly(o:locup_md){ 
      this.crassembly = o; this.crstate = (this.crassembly.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnassembly = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnassembly.lsloctype);
      });
    }

    validate() {
    this.crassembly.tflow =  (this.crassembly.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";    
    this.crassembly.lscodefull = this.crassembly.orgcode + "-" + this.crassembly.site+ "-" +this.crassembly.depot+ "-" + this.crassembly.lscodealt;
    //this.crassembly.lscodeid = this.crassembly.orgcode + "-" + this.crassembly.site+ "-" +this.crassembly.depot+ "-" + this.crassembly.lscodealt;
    this.crassembly.lszone = this.crassembly.lscode;
    this.crassembly.spcarea = "AS";
    if (this.crassembly.lscode == "") { this.crassembly.spcarea = this.slccategory.value; }    
    this.lnassembly.tflow =  (this.crassembly.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";   
    this.lnassembly.lszone = this.crassembly.lscode;
    this.lnassembly.lscode = this.crassembly.lscode;
    this.lnassembly.spcarea = this.crassembly.spcarea;
    this.lnassembly.lsloctype = this.slccategory.value;
    this.lnassembly.lscodealt = this.crassembly.lscodealt;
    this.lnassembly.lscodefull = this.crassembly.orgcode + "-" + this.crassembly.site+ "-" +this.crassembly.depot+ "-" + this.crassembly.lscode;
    this.ngPopups.confirm('Do you accept change of assembly ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crassembly.tflowcnt = this.crassembly.tflow;
                this.sv.upsertlocup(this.crassembly).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup assembly area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crassembly.lshash = "-"; 
                      this.sv.upsertlocdw(this.lnassembly).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup assembly storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crassembly.lshash = "-"; this.fndassembly(); }
                      );
                    }
                );                
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop assembly ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crassembly.tflowcnt = this.crassembly.tflow;
              this.sv.droplocup(this.crassembly).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnassembly).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop assembly storage success</span>",null,{ enableHtml : true }); 
                        this.fndassembly(); 
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
      this.mv.getlov("assembly","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsassembly    = null; delete this.lsassembly;
      this.crassembly    = null; delete this.crassembly;
      this.lnassembly    = null; delete this.lnassembly;
      this.pmassembly    = null; delete this.pmassembly;
      this.pmdw           = null; delete this.pmdw;
      this.slcassembly   = null; delete this.slcassembly;
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
