import { Component, OnInit, Input } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';

import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_ls, locdw_md, locdw_pm, locup_ls, locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-returnline',
  templateUrl: 'maps.return.line.html',
  styles: ['.dgreturn { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
  export class mapsreturnlineComponent implements OnInit {
  ison:boolean=false;
  public lsreturn:locup_md[] = new Array();
  public crreturn:locup_md = new locup_md();
  public lnreturn:locdw_md = new locdw_md();

  public pmreturn:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcreturn:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndreturn(); } 
    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcreturn(){ this.crreturn.lszone = this.slcreturn.value; }
    dscicon(o:string) { return this.msdstate.find(x=>x.value == o).icon; }

    newreturn() { 
        this.crreturn = new locup_md();
        this.crreturn.orgcode = this.av.crProfile.orgcode;
        this.crreturn.site = this.av.crRole.site;
        this.crreturn.depot = this.av.crRole.depot;
        this.crreturn.spcarea = "RN"; 
        this.crreturn.fltype = "BL";
        this.crreturn.lszone = "";
        this.crreturn.lscode = "";
        this.crreturn.lsdesc = "";
        this.crreturn.lsseq = 0;
        this.crreturn.tflow = "NW";
        this.crreturn.tflowcnt = "IO";
        this.crreturn.datemodify = new Date();
        this.crreturn.accnmodify = this.av.crProfile.accncode;
        this.crreturn.lshash = "NW";
        this.crreturn.lsformat = "";       
        this.crreturn.lsbay = "";
        this.crreturn.lslevel = "";
        this.crreturn.lscodealt = "";
        this.crreturn.lscodefull = "";
        this.crreturn.lsaisle = "";
        this.crreturn.spcarea = "";
        this.crreturn.lsformat = "";
        this.crreturn.lsdesc = "";

        this.lnreturn = new locdw_md();
        this.lnreturn.crfreepct = 100;
        this.lnreturn.crvolume = 0;
        this.lnreturn.crweight = 0;
        this.lnreturn.depot = this.av.crRole.depot;
        this.lnreturn.fltype = "BL";
        this.lnreturn.lsaisle = "";
        this.lnreturn.lsbay = "";
        this.lnreturn.lscode = "";
        this.lnreturn.lsdesc = "";
        this.lnreturn.lsdigit = "";
        this.lnreturn.lsgapbuttom = 0;
        this.lnreturn.lsgapleft = 0;
        this.lnreturn.lsgapright = 0;
        this.lnreturn.lsgaptop = 0;
        this.lnreturn.lshash = "000";
        this.lnreturn.lslevel = "";
        this.lnreturn.lsdmlength = 0;
        this.lnreturn.lsdmwidth = 0;
        this.lnreturn.lsdmheight = 0;
        this.lnreturn.lsmixage = 0;
        this.lnreturn.lsmixarticle = 0;
        this.lnreturn.lsmixlotno = 0;
        this.lnreturn.lsmnsafety = 0;
        this.lnreturn.lsmxheight = 0;
        this.lnreturn.lsmxhuno = 9999999; 
        this.lnreturn.lsmxlength = 9999999;
        this.lnreturn.lsmxvolume = 9999999;
        this.lnreturn.lsmxweight = 9999999;
        this.lnreturn.lsmxwidth = 9999999;
        this.lnreturn.lsremarks = ""; 
        this.lnreturn.lsstack = "1";
        this.lnreturn.orgcode = this.av.crProfile.orgcode;
        this.lnreturn.procmodify = "locreturn";
        this.lnreturn.site = this.av.crRole.site;
        this.lnreturn.spcarea = "RN"; 
        this.lnreturn.spcarticle = "";
        this.lnreturn.spcblock = 0; 
        this.lnreturn.spchuno = "";
        this.lnreturn.spclasttouch = "";
        this.lnreturn.spcpickunit = "";
        this.lnreturn.spcpicking = 0;
        this.lnreturn.spcpivot = "";
        this.lnreturn.spcseqpath = 0;
        this.lnreturn.spctaskfnd = "";
        this.lnreturn.spcthcode = "";
        this.lnreturn.tflow = "IO";  
        this.lnreturn.tflowcnt = ""
        this.lnreturn.spcrpn = "";
        this.lnreturn.lsloc = "";


        if(this.lsreturn.findIndex(x=>x.lshash =="NW") == -1) {this.lsreturn.push(this.crreturn);}        
        this.toastr.info("<span class='fn-07e'>New return is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crreturn.lsformat,"AA","","","",""));
    }

    fndreturn(){ 
      this.pmreturn.fltype = "BL";
      this.pmreturn.spcarea = "RN";
        this.sv.fndlocup(this.pmreturn).pipe().subscribe(            
            (res) => { this.lsreturn = res; if (this.lsreturn.length > 0) { this.selreturn(this.lsreturn[0]); } }
        );
    }

    selreturn(o:locup_md){ 
      this.crreturn = o; this.crstate = (this.crreturn.tflow == "IO") ? true : false;     
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;

      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnreturn = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnreturn.lsloctype);
      });
    }
    validate() {
    this.crreturn.tflow = (this.crreturn.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crreturn.lscodefull = this.crreturn.orgcode + "-" + this.crreturn.site+ "-" +this.crreturn.depot+ "-" + this.crreturn.lscodealt;
    //this.crreturn.lscodeid = this.crreturn.orgcode + "-" + this.crreturn.site+ "-" +this.crreturn.depot+ "-" + this.crreturn.lscodealt;
    this.crreturn.lszone = this.crreturn.lscode;
    this.crreturn.spcarea = "RN";
    if (this.crreturn.lscode == "") { this.crreturn.spcarea = this.slccategory.value; }        
    this.lnreturn.lszone = this.crreturn.lscode;
    this.lnreturn.lscode = this.crreturn.lscode;
    this.lnreturn.spcarea = this.crreturn.spcarea;
    this.lnreturn.lsloctype = this.slccategory.value;
    this.lnreturn.lscodealt = this.crreturn.lscodealt;
    this.lnreturn.lscodefull = this.crreturn.orgcode + "-" + this.crreturn.site+ "-" +this.crreturn.depot+ "-" + this.crreturn.lscode;
    this.ngPopups.confirm('Do you accept change of return ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crreturn.tflowcnt = this.crreturn.tflow;
                this.sv.upsertlocup(this.crreturn).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-07e'>Setup return area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crreturn.lshash = "-"; 
                      this.sv.upsertlocdw(this.lnreturn).pipe().subscribe(
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup return storage success</span>",null,{ enableHtml : true }); this.ison = false; this.crreturn.lshash = "-"; this.fndreturn(); }
                      );
                    }
                );                  
                
                                  
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop return ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crreturn.tflowcnt = this.crreturn.tflow;
              this.sv.droplocup(this.crreturn).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnreturn).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop return storage success</span>",null,{ enableHtml : true }); 
                        this.fndreturn(); 
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
      this.mv.getlov("RETURN","TYPE").pipe().subscribe((res) => { this.msdcategory = res; });
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsreturn       = null; delete this.lsreturn;
      this.crreturn       = null; delete this.crreturn;
      this.lnreturn       = null; delete this.lnreturn;
      this.pmreturn       = null; delete this.pmreturn;
      this.pmdw           = null; delete this.pmdw;
      this.slcreturn      = null; delete this.slcreturn;
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
      this.formatdatelong = null; delete this.formatdatelong;
    }
}