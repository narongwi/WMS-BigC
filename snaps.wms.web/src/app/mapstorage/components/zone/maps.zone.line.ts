import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locup_md, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';

import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'appmaps-zoneline',
  templateUrl: 'maps.zone.line.html',
  styles: ['.dgzone { height:calc(100vh - 150px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class mapszoneline implements OnInit, OnDestroy {
//   @Input() iconstate: string;
    ison:boolean=false;
    public lszone:locup_md[] = new Array();
    public crzone:locup_md = new locup_md();
    public pmzone:locup_pm = new locup_pm();
    public lshus:lov[] = new Array();
    public slczone:lov = new lov();

    public slccategory:lov = new lov();
    public msdstate:lov[] = new Array();
    public msdcategory:lov[] = new Array();
    public crstate:boolean = true;
    public spcareadsc:string;

    //HU Selection 
    slchu:lov = new lov();

    //Date format
    formatdatelong:string;

    constructor(
        private av: authService,
        private sv: mapstorageService,
        private mv: adminService, 
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.formatdatelong = this.av.crProfile.formatdatelong;

        this.pmzone.orgcode = this.av.crProfile.orgcode;
        this.pmzone.site = this.av.crRole.site;
        this.pmzone.depot = this.av.crRole.depot;
        this.crzone.lsformat = "ZND03R0-AID03R0-BAD03R0-LVD03R0-STD03R0";

        this.getstate();
        this.getspcarea();

        this.lshus.push({
          desc : "Pallet", 
          icon : "",
          valopnfirst : "",
          valopnfour : "",
          valopnsecond : "",
          valopnthird : "",
          value : "PL01"
        });

        this.slchu = this.lshus[0];
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fndzone(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselczone(){ this.crzone.lszone = this.slczone.value; }
    getstate() { 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.msdstate = res;
          this.msdstate.push({ value : 'NW', desc : 'New Zone', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    getspcarea() { 
      this.mv.getlov("LOCATION","SPCAREA").pipe().subscribe(
        (res) => { this.msdcategory = res; },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    dscloc(fmt:string,zn:string,al:string,ba:string,lv:string,st:string){
      if (fmt.indexOf("ZND") >= 0) { 
          fmt = this.dsclocdl(fmt,zn) + fmt.substring(8);
      }
      return fmt;
    }
    dsclocdl(fmt:string,val:string){ 
      fmt = fmt.substring(fmt.indexOf("ZND"), 8);
      if (fmt.slice(5,6)=="R") { 
        console.log("Right");
        return val.padStart(parseInt(fmt.slice(3,5)), fmt.slice(6,7)) + fmt.slice(7,8)  ;
      }else if (fmt.slice(5,6)=="L") { 
        console.log("Left");
        return val.padEnd(parseInt(fmt.slice(3,5)), fmt.slice(6,7)) + fmt.slice(7,8) ;
      }else { 
        console.log("none");
        return val + fmt.slice(7,8); }
    }
    newzone() { 
      this.crzone = new locup_md();
      this.crzone.orgcode = this.av.crProfile.orgcode;
      this.crzone.site = this.av.crRole.site;
      this.crzone.depot = this.av.crRole.depot;
      this.crzone.fltype = "ZN";
      this.crzone.lszone = "";
      this.crzone.lscode = "";
      this.crzone.lsdesc = "";
      this.crzone.lsseq = 0;
      this.crzone.tflow = "NW";
      this.crzone.tflowcnt = "IO";
      this.crzone.datemodify = new Date();
      this.crzone.accnmodify = this.av.crProfile.accncode;
      this.crzone.lshash = "NW";
      this.crzone.lsformat = "";
      this.crzone.lszone = "";
      this.crzone.lsbay = "";
      this.crzone.lslevel = "";
      this.crzone.lscodealt = "";
      this.crzone.lscodefull = "";
      this.crzone.lsaisle = "";
      this.crzone.spcarea = "";
      this.crzone.lsformat = "ZND03N0NAID03R0-BAD03R0-LVD03R0-STD03R0";
      this.crzone.lsdesc = "";
      this.crzone.lscodeid = "";
      if(this.lszone.findIndex(x=>x.lshash =="NW") == -1) {this.lszone.push(this.crzone);}        
      this.toastr.info("<span class='fn-07e'>New zone is ready to setup</span>",null,{ enableHtml : true });
    }
    fndzone(){ 
      this.pmzone.fltype = "ZN";
        this.sv.fndloczone(this.pmzone).pipe().subscribe(            
            (res) => { this.lszone = res; if ( this.lszone.length > 0 ) { this.selzone(this.lszone[0]); } }
        );
    }
    selzone(o:locup_md){ this.crzone = o; this.crstate = (this.crzone.tflow == "IO") ? true : false; 
        this.spcareadsc = this.msdcategory.find(x=>x.value == this.crzone.spcarea).desc; 
        this.slccategory = this.msdcategory.find(e=>e.value == this.crzone.spcarea);
      }
    validate() {
      if (this.crzone.lscode == "") { 
          this.toastr.warning("<span class='fn-08e'>Zone code is require</span>"); 
      } else { 
        this.crzone.tflow = (this.crzone.tflow == "NW") ? "NW" : (this.crstate == true) ? "IO" : "XX";
        this.crzone.lscodefull = this.crzone.orgcode + "-" + this.crzone.site+ "-" +this.crzone.depot+ "-" + this.crzone.lscodealt;
        this.crzone.spcarea = (this.crzone.tflow == "NW") ? this.slccategory.value : this.crzone.spcarea;
        this.ngPopups.confirm('Do you accept change of zone?')
            .subscribe(res => {
                if (res) {
                  this.ison = true;
                  this.crzone.tflowcnt = this.crzone.tflow;
                  this.sv.upsertlocup(this.crzone).pipe().subscribe(            
                      (res) => { this.toastr.success("<span class='fn-1e15'>validate success</span>",null,{ enableHtml : true }); this.ison = false; this.crzone.lshash = "-"; this.fndzone(); }
                  );
                } 
            });
      }

    }
    remove() { 
      this.ngPopups.confirm('Do you confirm to remove zone?')
          .subscribe(res => {
              if (res) {
                this.ison = true;
                this.sv.droplocup(this.crzone).pipe().subscribe(            
                    (res) => { this.toastr.success("<span class='fn-07e'>remove success</span>",null,{ enableHtml : true }); this.ison = false; this.crzone.lshash = "-"; this.fndzone(); }
                );
              } 
          });
    }


    ngOnDestroy():void { 
      this.ison           = null; delete this.ison;
      this.lszone         = null; delete this.lszone;
      this.crzone         = null; delete this.crzone;
      this.pmzone         = null; delete this.pmzone;
      this.lshus          = null; delete this.lshus;
      this.slczone        = null; delete this.slczone;
      this.slccategory    = null; delete this.slccategory;
      this.msdstate       = null; delete this.msdstate;
      this.msdcategory    = null; delete this.msdcategory;
      this.crstate        = null; delete this.crstate;
      this.spcareadsc     = null; delete this.spcareadsc;
      this.slchu          = null; delete this.slchu;
      this.formatdatelong = null; delete this.formatdatelong;
    }


}
