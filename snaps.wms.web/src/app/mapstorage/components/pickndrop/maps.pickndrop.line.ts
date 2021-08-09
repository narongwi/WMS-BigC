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
  selector: 'appmaps-pickndropline',
  templateUrl: 'maps.pickndrop.line.html',
  styles: ['.dgpickndrop { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapspickndroplineComponent implements OnInit, OnDestroy {

  ison:boolean=false;
  public lspickndrop:locup_md[] = new Array();
  public crpickndrop:locup_md = new locup_md();
  public lnpickndrop:locdw_md = new locdw_md();

  public pmpickndrop:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcpickndrop:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndpickndrop(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcpickndrop(){ this.crpickndrop.lszone = this.slcpickndrop.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newpickndrop() { 
        this.crpickndrop = new locup_md();
        this.crpickndrop.orgcode = this.av.crProfile.orgcode;
        this.crpickndrop.site = this.av.crRole.site;
        this.crpickndrop.depot = this.av.crRole.depot;
        this.crpickndrop.spcarea = "PD"; 
        this.crpickndrop.fltype = "BL";
        this.crpickndrop.lszone = "";
        this.crpickndrop.lscode = "";
        this.crpickndrop.lsdesc = "";
        this.crpickndrop.lsseq = 0;
        this.crpickndrop.tflow = "NW";
        this.crpickndrop.tflowcnt = "IO";
        this.crpickndrop.datemodify = new Date();
        this.crpickndrop.accnmodify = this.av.crProfile.accncode;
        this.crpickndrop.lshash = "NW";
        this.crpickndrop.lsformat = "";       
        this.crpickndrop.lsbay = "";
        this.crpickndrop.lslevel = "";
        this.crpickndrop.lscodealt = "";
        this.crpickndrop.lscodefull = "";
        this.crpickndrop.lsaisle = "";
        this.crpickndrop.spcarea = "";
        this.crpickndrop.lsformat = "";
        this.crpickndrop.lsdesc = "";

        this.lnpickndrop = new locdw_md();
        this.lnpickndrop.crfreepct = 100;
        this.lnpickndrop.crvolume = 0;
        this.lnpickndrop.crweight = 0;
        this.lnpickndrop.depot = this.av.crRole.depot;
        this.lnpickndrop.fltype = "BL";
        this.lnpickndrop.lsaisle = "";
        this.lnpickndrop.lsbay = "";
        this.lnpickndrop.lscode = "";
        this.lnpickndrop.lsdesc = "";
        this.lnpickndrop.lsdigit = "";
        this.lnpickndrop.lsgapbuttom = 0;
        this.lnpickndrop.lsgapleft = 0;
        this.lnpickndrop.lsgapright = 0;
        this.lnpickndrop.lsgaptop = 0;
        this.lnpickndrop.lshash = "000";
        this.lnpickndrop.lslevel = "";
        this.lnpickndrop.lsdmlength = 0;
        this.lnpickndrop.lsdmwidth = 0;
        this.lnpickndrop.lsdmheight = 0;
        this.lnpickndrop.lsmixage = 0;
        this.lnpickndrop.lsmixarticle = 0;
        this.lnpickndrop.lsmixlotno = 0;
        this.lnpickndrop.lsmnsafety = 0;
        this.lnpickndrop.lsmxheight = 0;
        this.lnpickndrop.lsmxhuno = 9999999; 
        this.lnpickndrop.lsmxlength = 9999999;
        this.lnpickndrop.lsmxvolume = 9999999;
        this.lnpickndrop.lsmxweight = 9999999;
        this.lnpickndrop.lsmxwidth = 9999999;
        this.lnpickndrop.lsremarks = ""; 
        this.lnpickndrop.lsstack = "1";
        this.lnpickndrop.orgcode = this.av.crProfile.orgcode;
        this.lnpickndrop.procmodify = "locpickndrop";
        this.lnpickndrop.site = this.av.crRole.site;
        this.lnpickndrop.spcarea = "PD"; 
        this.lnpickndrop.spcarticle = "";
        this.lnpickndrop.spcblock = 0; 
        this.lnpickndrop.spchuno = "";
        this.lnpickndrop.spclasttouch = "";
        this.lnpickndrop.spcpickunit = "";
        this.lnpickndrop.spcpicking = 0;
        this.lnpickndrop.spcpivot = "";
        this.lnpickndrop.spcseqpath = 0;
        this.lnpickndrop.spctaskfnd = "";
        this.lnpickndrop.spcthcode = "";
        this.lnpickndrop.tflow = "NW";  
        this.lnpickndrop.tflowcnt = ""
        this.lnpickndrop.spcrpn = "";
        this.lnpickndrop.lsloc = "";


        if(this.lspickndrop.findIndex(x=>x.lshash =="NW") == -1) {this.lspickndrop.push(this.crpickndrop);}        
        this.toastr.info("<span class='fn-07e'>New pickndrop is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crpickndrop.lsformat,"AA","","","",""));
    }
    fndpickndrop(){ 
      this.pmpickndrop.fltype = "BL";
      this.pmpickndrop.spcarea = "PD";
        this.sv.fndlocup(this.pmpickndrop).pipe().subscribe(            
            (res) => { this.lspickndrop = res; if (this.lspickndrop.length > 0) { this.selpickndrop(this.lspickndrop[0]); } }
        );
    }

    selpickndrop(o:locup_md){ 
      this.crpickndrop = o; this.crstate = (this.crpickndrop.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnpickndrop = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnpickndrop.lsloctype);
      });
    }
    validate() {
    this.crpickndrop.tflow =  (this.crpickndrop.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crpickndrop.lscodefull = this.crpickndrop.orgcode + "-" + this.crpickndrop.site+ "-" +this.crpickndrop.depot+ "-" + this.crpickndrop.lscodealt;
    //this.crpickndrop.lscodeid = this.crpickndrop.orgcode + "-" + this.crpickndrop.site+ "-" +this.crpickndrop.depot+ "-" + this.crpickndrop.lscodealt;
    this.crpickndrop.lszone = this.crpickndrop.lscode;
    this.crpickndrop.spcarea = "PD";
    if (this.crpickndrop.lscode == "") { this.crpickndrop.spcarea = this.slccategory.value; }  
    this.lnpickndrop.tflow =  (this.crpickndrop.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";   
    this.lnpickndrop.lszone = this.crpickndrop.lscode;
    this.lnpickndrop.lscode = this.crpickndrop.lscode;
    this.lnpickndrop.spcarea = this.crpickndrop.spcarea;
    this.lnpickndrop.lsloctype = this.slccategory.value;
    this.lnpickndrop.lscodealt = this.crpickndrop.lscodealt;
    this.lnpickndrop.lscodefull = this.crpickndrop.orgcode + "-" + this.crpickndrop.site+ "-" +this.crpickndrop.depot+ "-" + this.crpickndrop.lscode;
    this.ngPopups.confirm('Do you accept change of pickndrop ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crpickndrop.tflowcnt = this.crpickndrop.tflow;
                this.sv.upsertlocup(this.crpickndrop).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup pickndrop area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crpickndrop.lshash = "-"; 
                      this.sv.upsertlocdw(this.lnpickndrop).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup pickndrop storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crpickndrop.lshash = "-"; this.fndpickndrop(); }
                      );

                    }
                );                  
                
                                  
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop pickndrop ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crpickndrop.tflowcnt = this.crpickndrop.tflow;
              this.sv.droplocup(this.crpickndrop).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnpickndrop).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop pickndrop storage success</span>",null,{ enableHtml : true }); 
                        this.fndpickndrop(); 
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
      this.mv.getlov("PICKNDROP","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lspickndrop    = null; delete this.lspickndrop;
      this.crpickndrop    = null; delete this.crpickndrop;
      this.lnpickndrop    = null; delete this.lnpickndrop;
      this.pmpickndrop    = null; delete this.pmpickndrop;
      this.pmdw           = null; delete this.pmdw;
      this.slcpickndrop   = null; delete this.slcpickndrop;
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
