import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_md, locdw_pm, locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-bulkline',
  templateUrl: 'maps.bulk.line.html',
  styles: ['.dgbulk { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsbulklineComponent implements OnInit,OnDestroy {
//   @Input() iconstate: string;
    ison:boolean=false;
  public lsbulk:locup_md[] = new Array();
  public crbulk:locup_md = new locup_md();
  public lnbulk:locdw_md = new locdw_md();

  public pmbulk:locup_pm = new locup_pm();
  public pmdw:locdw_pm = new locdw_pm();
  public slcbulk:lov = new lov();

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
    ngAfterViewInit(){ this.ngSetup(); this.fndbulk(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcbulk(){ this.crbulk.lszone = this.slcbulk.value; }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newbulk() { 
        this.crbulk = new locup_md();
        this.crbulk.orgcode = this.av.crProfile.orgcode;
        this.crbulk.site = this.av.crRole.site;
        this.crbulk.depot = this.av.crRole.depot;
        this.crbulk.spcarea = "BL"; 
        this.crbulk.fltype = "BL";
        this.crbulk.lszone = "";
        this.crbulk.lscode = "";
        this.crbulk.lsdesc = "";
        this.crbulk.lsseq = 0;
        this.crbulk.tflow = "NW";
        this.crbulk.tflowcnt = "IO";
        this.crbulk.datemodify = new Date();
        this.crbulk.accnmodify = this.av.crProfile.accncode;
        this.crbulk.lshash = "NW";
        this.crbulk.lsformat = "";       
        this.crbulk.lsbay = "";
        this.crbulk.lslevel = "";
        this.crbulk.lscodealt = "";
        this.crbulk.lscodefull = "";
        this.crbulk.lsaisle = "";
        this.crbulk.spcarea = "";
        this.crbulk.lsformat = "";
        this.crbulk.lsdesc = "";

        this.lnbulk = new locdw_md();
        this.lnbulk.crfreepct = 100;
        this.lnbulk.crvolume = 0;
        this.lnbulk.crweight = 0;
        this.lnbulk.depot = this.av.crRole.depot;
        this.lnbulk.fltype = "BL";
        this.lnbulk.lsaisle = "";
        this.lnbulk.lsbay = "";
        this.lnbulk.lscode = "";
        this.lnbulk.lsdesc = "";
        this.lnbulk.lsdigit = "";
        this.lnbulk.lsgapbuttom = 0;
        this.lnbulk.lsgapleft = 0;
        this.lnbulk.lsgapright = 0;
        this.lnbulk.lsgaptop = 0;
        this.lnbulk.lshash = "000";
        this.lnbulk.lslevel = "";
        this.lnbulk.lsdmlength = 0;
        this.lnbulk.lsdmwidth = 0;
        this.lnbulk.lsdmheight = 0;
        this.lnbulk.lsmixage = 0;
        this.lnbulk.lsmixarticle = 0;
        this.lnbulk.lsmixlotno = 0;
        this.lnbulk.lsmnsafety = 0;
        this.lnbulk.lsmxheight = 0;
        this.lnbulk.lsmxhuno = 9999999; 
        this.lnbulk.lsmxlength = 9999999;
        this.lnbulk.lsmxvolume = 9999999;
        this.lnbulk.lsmxweight = 9999999;
        this.lnbulk.lsmxwidth = 9999999;
        this.lnbulk.lsremarks = ""; 
        this.lnbulk.lsstack = "1";
        this.lnbulk.orgcode = this.av.crProfile.orgcode;
        this.lnbulk.procmodify = "locbulk";
        this.lnbulk.site = this.av.crRole.site;
        this.lnbulk.spcarea = "BL"; 
        this.lnbulk.spcarticle = "";
        this.lnbulk.spcblock = 0; 
        this.lnbulk.spchuno = "";
        this.lnbulk.spclasttouch = "";
        this.lnbulk.spcpickunit = "";
        this.lnbulk.spcpicking = 0;
        this.lnbulk.spcpivot = "";
        this.lnbulk.spcseqpath = 0;
        this.lnbulk.spctaskfnd = "";
        this.lnbulk.spcthcode = "";
        this.lnbulk.tflow = "IO";  
        this.lnbulk.tflowcnt = ""
        this.lnbulk.spcrpn = "";
        this.lnbulk.lsloc = "";


        if(this.lsbulk.findIndex(x=>x.lshash =="NW") == -1) {this.lsbulk.push(this.crbulk);}        
        this.toastr.info("<span class='fn-1e15'>New bulk is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crbulk.lsformat,"AA","","","",""));
    }
    fndbulk(){ 
      this.pmbulk.fltype = "BL";
      this.pmbulk.spcarea = "BL";
        this.sv.fndlocup(this.pmbulk).pipe().subscribe(            
            (res) => { this.lsbulk = res; if (this.lsbulk.length > 0) { this.selbulk(this.lsbulk[0]); } console.log(res); }
        );
    }
  
    selbulk(o:locup_md){ 
      this.crbulk = o; this.crstate = (this.crbulk.tflow == "IO") ? true : false; 
      this.pmdw.spcarea = o.spcarea;
      this.pmdw.fltype = o.fltype;
      this.pmdw.lscode = o.lscode;
      this.sv.getlocdw(this.pmdw).pipe().subscribe((res)=>{ 
        this.lnbulk = res; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.lnbulk.lsloctype);
      });
    }
    validate() {
    this.crbulk.tflow = (this.crbulk.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
    this.crbulk.lscodefull = this.crbulk.orgcode + "-" + this.crbulk.site+ "-" +this.crbulk.depot+ "-" + this.crbulk.lscodealt;
    //this.crbulk.lscodeid = this.crbulk.orgcode + "-" + this.crbulk.site+ "-" +this.crbulk.depot+ "-" + this.crbulk.lscodealt;
    this.crbulk.lszone = this.crbulk.lscode;
    this.crbulk.spcarea = "BL";
    if (this.crbulk.lscode == "") { this.crbulk.spcarea = this.slccategory.value; }        
    this.lnbulk.lszone = this.crbulk.lscode;
    this.lnbulk.lscode = this.crbulk.lscode;
    this.lnbulk.lsloctype = this.slccategory.value;
    this.lnbulk.lscodealt = this.crbulk.lscodealt;
		this.lnbulk.lscodefull = this.crbulk.orgcode + "-" + this.crbulk.site+ "-" +this.crbulk.depot+ "-" + this.crbulk.lscode;
    this.ngPopups.confirm('Do you accept change of Bulk ?')
        .subscribe(res => {
            if (res) {
                this.ison = true;
                this.crbulk.tflowcnt = this.crbulk.tflow;
                this.sv.upsertlocup(this.crbulk).pipe().subscribe(            
                    (res) => { 
                      this.toastr.success("<span class='fn-1e15'>Setup Bulk area success</span>",null,{ enableHtml : true }); 
                      this.ison = false; this.crbulk.lshash = "-"; this.fndbulk(); 
                    }
                );                  
                this.sv.upsertlocdw(this.lnbulk).pipe().subscribe(
                  (res) => { this.toastr.success("<span class='fn-1e15'>Setup bulk storage success</span>",null,{ enableHtml : true }); this.ison = false; this.crbulk.lshash = "-"; this.fndbulk(); },
                  (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                  () => { }
                );
                                  
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept drop bulk ?')
      .subscribe(res => {
          if (res) {
              this.ison = true;
              this.crbulk.tflowcnt = this.crbulk.tflow;
              this.sv.droplocup(this.crbulk).pipe().subscribe(            
                  (res) => {                     
                    this.sv.droplocdw(this.lnbulk).pipe().subscribe(
                      (res) => { 
                        this.toastr.success("<span class='fn-08e'>Drop bulk storage success</span>",null,{ enableHtml : true }); 
                        this.fndbulk();  
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
      this.mv.getlov("BULK","TYPE").pipe().subscribe(
        (res) => { this.msdcategory = res; },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lsbulk         = null; delete this.lsbulk;
      this.crbulk         = null; delete this.crbulk;
      this.lnbulk         = null; delete this.lnbulk;
      this.pmbulk         = null; delete this.pmbulk;
      this.pmdw           = null; delete this.pmdw;
      this.slcbulk        = null; delete this.slcbulk;
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
