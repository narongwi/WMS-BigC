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
  selector: 'appmaps-damageline',
  templateUrl: 'maps.damage.line.html',
  styles: ['.dgdamage { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsdamagelineComponent implements OnInit, OnDestroy {

  ison:boolean=false;
  public lsdamage:locup_md[] = new Array();
  public crdamage:locup_md = new locup_md();
  public lndamage:locdw_md = new locdw_md();

  public pmdamage:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcdamage:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fnddamage(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcdamage(){ this.crdamage.lszone = this.slcdamage.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newdamage() { 
        this.crdamage = new locup_md();
        this.crdamage.orgcode = this.av.crProfile.orgcode;
        this.crdamage.site = this.av.crRole.site;
        this.crdamage.depot = this.av.crRole.depot;
        this.crdamage.spcarea = "DM"; 
        this.crdamage.fltype = "BL";
        this.crdamage.lszone = "";
        this.crdamage.lscode = "";
        this.crdamage.lsdesc = "";
        this.crdamage.lsseq = 0;
        this.crdamage.tflow = "NW";
        this.crdamage.tflowcnt = "IO";
        this.crdamage.datemodify = new Date();
        this.crdamage.accnmodify = this.av.crProfile.accncode;
        this.crdamage.lshash = "NW";
        this.crdamage.lsformat = "";       
        this.crdamage.lsbay = "";
        this.crdamage.lslevel = "";
        this.crdamage.lscodealt = "";
        this.crdamage.lscodefull = "";
        this.crdamage.lsaisle = "";
        this.crdamage.spcarea = "";
        this.crdamage.lsformat = "";
        this.crdamage.lsdesc = "";

        this.lndamage = new locdw_md();
        this.lndamage.crfreepct = 100;
        this.lndamage.crvolume = 0;
        this.lndamage.crweight = 0;
        this.lndamage.depot = this.av.crRole.depot;
        this.lndamage.fltype = "BL";
        this.lndamage.lsaisle = "";
        this.lndamage.lsbay = "";
        this.lndamage.lscode = "";
        this.lndamage.lsdesc = "";
        this.lndamage.lsdigit = "";
        this.lndamage.lsgapbuttom = 0;
        this.lndamage.lsgapleft = 0;
        this.lndamage.lsgapright = 0;
        this.lndamage.lsgaptop = 0;
        this.lndamage.lshash = "000";
        this.lndamage.lslevel = "";
        this.lndamage.lsdmlength = 0;
        this.lndamage.lsdmwidth = 0;
        this.lndamage.lsdmheight = 0;
        this.lndamage.lsmixage = 1;
        this.lndamage.lsmixarticle = 1;
        this.lndamage.lsmixlotno = 1;
        this.lndamage.lsmnsafety = 0;
        this.lndamage.lsmxheight = 0;
        this.lndamage.lsmxhuno = 9999999; 
        this.lndamage.lsmxlength = 9999999;
        this.lndamage.lsmxvolume = 9999999;
        this.lndamage.lsmxweight = 9999999;
        this.lndamage.lsmxwidth = 9999999;
        this.lndamage.lsremarks = ""; 
        this.lndamage.lsstack = "1";
        this.lndamage.orgcode = this.av.crProfile.orgcode;
        this.lndamage.procmodify = "locdamage";
        this.lndamage.site = this.av.crRole.site;
        this.lndamage.spcarea = "DM"; 
        this.lndamage.spcarticle = "";
        this.lndamage.spcblock = 0; 
        this.lndamage.spchuno = "";
        this.lndamage.spclasttouch = "";
        this.lndamage.spcpickunit = "";
        this.lndamage.spcpicking = 0;
        this.lndamage.spcpivot = "";
        this.lndamage.spcseqpath = 0;
        this.lndamage.spctaskfnd = "";
        this.lndamage.spcthcode = "";
        this.lndamage.tflow = "IO";  
        this.lndamage.tflowcnt = ""
        this.lndamage.spcrpn = "";
        this.lndamage.lsloc = "";


        if(this.lsdamage.findIndex(x=>x.lshash =="NW") == -1) {this.lsdamage.push(this.crdamage);}        
        this.toastr.info("<span class='fn-07e'>New damage is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crdamage.lsformat,"AA","","","",""));
    }
    fnddamage(){ 
      this.pmdamage.fltype = "BL";
      this.pmdamage.spcarea = "DM";
        this.sv.fndlocup(this.pmdamage).pipe().subscribe(            
            (res) => { this.lsdamage = res; if (this.lsdamage.length > 0 ) { this.seldamage(this.lsdamage[0]); } }
        );
    }

    seldamage(o:locup_md){ 
      this.crdamage = o; this.crstate = (this.crdamage.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lndamage = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lndamage.lsloctype);
      });
    }
    validate() {
    this.crdamage.tflow = (this.crdamage.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crdamage.lscodefull = this.crdamage.orgcode + "-" + this.crdamage.site+ "-" +this.crdamage.depot+ "-" + this.crdamage.lscodealt;
    //this.crdamage.lscodeid = this.crdamage.orgcode + "-" + this.crdamage.site+ "-" +this.crdamage.depot+ "-" + this.crdamage.lscodealt;
    this.crdamage.lszone = this.crdamage.lscode;
    this.crdamage.spcarea = "DM";
    if (this.crdamage.lscode == "") { this.crdamage.spcarea = this.slccategory.value; }        
    this.lndamage.lszone = this.crdamage.lscode;
    this.lndamage.lscode = this.crdamage.lscode;
    this.lndamage.spcarea = this.crdamage.spcarea;
    this.lndamage.lsloctype = this.slccategory.value;
    this.lndamage.lscodealt = this.crdamage.lscodealt;
    this.lndamage.lscodefull = this.crdamage.orgcode + "-" + this.crdamage.site+ "-" +this.crdamage.depot+ "-" + this.crdamage.lscode;
    this.ngPopups.confirm('Do you accept change of damage ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crdamage.tflowcnt = this.crdamage.tflow;
                this.sv.upsertlocup(this.crdamage).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup damage area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crdamage.lshash = "-"; 
                      this.sv.upsertlocdw(this.lndamage).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup damage storage success</span>",null,{ enableHtml : true }); 
                        this.ison = false; this.crdamage.lshash = "-"; this.fnddamage(); },
                      );
                    }
                );                 
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop damage ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crdamage.tflowcnt = this.crdamage.tflow;
              this.sv.droplocup(this.crdamage).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lndamage).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop damage storage success</span>",null,{ enableHtml : true }); 
                        this.fnddamage(); 
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
      this.mv.getlov("DAMAGE","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsdamage    = null; delete this.lsdamage;
      this.crdamage    = null; delete this.crdamage;
      this.lndamage    = null; delete this.lndamage;
      this.pmdamage    = null; delete this.pmdamage;
      this.pmdw           = null; delete this.pmdw;
      this.slcdamage   = null; delete this.slcdamage;
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
