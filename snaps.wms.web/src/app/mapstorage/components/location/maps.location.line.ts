import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input, EventEmitter, Output, OnDestroy } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from 'src/app/admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_ls, locdw_md, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { shareService } from 'src/app/share.service';


@Component({
  selector: 'appmaps-locationline',
  templateUrl: 'maps.location.line.html',
  styles: ['.dglocation { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapslocationline implements OnInit, OnDestroy {

//   @Input() iconstate: string;
    @Output() selln = new EventEmitter<locdw_ls>();
    public mdfloc:locdw_ls;

    ison:boolean=false;
    public lszone:lov[] = new Array();
    public lsaisle:lov[] = new Array();
    public lsbay:lov[] = new Array();
    public lslevel:lov[] = new Array();

    public lslocation:locdw_ls[] = new Array();
    public crlocation:locdw_md = new locdw_md();
    public lsyesno:lov[] = new Array();

    public pmfnd:locdw_pm = new locdw_pm();
    public pmlov:locup_pm = new locup_pm();

    public slczone:lov;
    public slcaisle:lov;
    public slcbay:lov;
    public slclevel:lov;
    public slcmixproduct:lov;
    public slcmixaging:lov;
    public slcmixlot:lov;
    public slcispicking:lov;
    public slcisreserve:lov;
    public slstate:lov;
    public slcstate:lov;

    public spmzone:lov = null; 
    public spmaisle:lov = null;
    public spmbay:lov = null; 
    public spmlevel:lov = null;

    public msdstate:lov[] = new Array();

    public formatdate:string;
    public formatdatelong:string;



    //PageNavigate
    public page = 4;
    public pageSize = 200;
    public slrowlmt:lov;
    public lsrowlmt:lov[] = new Array();
    //Sorting 
    public lssort:string = "spcarea";
    public lsreverse: boolean = false; // for sorting
    
    constructor(
        private av: authService,
        private sv: mapstorageService,
        private mv: shareService,
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.pmfnd.orgcode = this.av.crProfile.orgcode;
        this.pmfnd.site = this.av.crRole.site;
        this.pmfnd.depot = this.av.crRole.depot;
        this.pmlov.orgcode = this.pmfnd.orgcode;
        this.pmlov.site = this.pmfnd.site;
        this.pmlov.depot = this.pmfnd.depot;
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.ngSetup(); } 
    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    fndlocation(){ 
      
        this.pmfnd.lszone = (this.spmzone == null) ? null : this.spmzone.value;
        this.pmfnd.lsaisle = (this.spmaisle != null) ? this.spmaisle.value : "";
        this.pmfnd.lsbay = (this.spmbay != null) ? this.spmbay.value : "";
        this.pmfnd.lslevel = (this.spmlevel != null) ? this.spmlevel.value : "";
        this.pmfnd.fltype = "LC";        
        this.pmfnd.spcarea = "ST";
        this.pmfnd.mixproduct = (this.slcmixproduct == null) ? null : this.slcmixproduct.value;
        this.pmfnd.mixaging = (this.slcmixaging == null) ? null : this.slcmixaging.value;
        this.pmfnd.mixlot = (this.slcmixlot == null) ? null : this.slcmixlot.value;
        this.pmfnd.ispicking = (this.slcispicking == null) ? null : this.slcispicking.value;
        this.pmfnd.isreserve = (this.slcisreserve == null) ? null : this.slcisreserve.value;
        this.pmfnd.tflow = (this.slcstate == null) ? null : this.slcstate.value;
        this.sv.fndlocdw(this.pmfnd).pipe().subscribe(            
            (res) => { 
                
                this.lslocation = res; 
                // this.lslocation.forEach(x=> {
                //     try{ 
                //         x.lszonename = this.lszone.find(e=>e.value == x.lszone).desc;
                //     }catch(ex){ 
                //         x.lszonename = x.lszone;
                //     }
                    
                //     x.spcareaname = (x.spcarea == "ST") ? "Stocking": "Distribite";
                // })
            }
        );
    }
    sellocation(o:locdw_ls){
        this.selln.emit(o);
        // this.sv.getlocdw(o).pipe().subscribe(            
        //     (res) => { this.lslocation = res; },
        //     (err) => { this.toastr.error(err.error.message); this.ison = false; },
        //     () => { }
        // );
    }

    ngDecZone(o:string) { try { return this.lszone.find(e=>e.value == o).desc; } catch(exp){ return o; }}
    ngDecArea(o:string) { return this.mv.ngDecArea(o); }
    ngDecState(o:string){ return this.mv.ngDecState(o); }
    ngDecIcon(o:string) { return this.mv.ngDecIcon(o); }
    ngChangeRowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
    ngSetup(){ 
      this.mv.lovzone().subscribe( (res) => { 
        this.lszone = res;
        if (this.lszone.length > 0) { 
          this.lsrowlmt = this.mv.getRowlimit();
          //this.mv.lovaisle(this.lszone[0].value).subscribe( (resz) => { this.lsaisle = resz;});
        }
      } );

      this.lsyesno = this.mv.getYesno();
    }
    ngOnDestroy():void{ 
        this.selln.unsubscribe(); this.selln = null; delete this.selln;
        this.mdfloc = null; delete this.mdfloc;
        this.ison = null; delete this.ison;
        this.lszone = null; delete this.lszone;
        this.lsaisle = null; delete this.lsaisle;
        this.lsbay = null; delete this.lsbay;
        this.lslevel = null; delete this.lslevel;
        this.lslocation = null; delete this.lslocation;
        this.crlocation = null; delete this.crlocation;
        this.pmfnd = null; delete this.pmfnd;
        this.pmlov = null; delete this.pmlov;
        this.slczone = null; delete this.slczone;
        this.slcaisle = null; delete this.slcaisle;
        this.slcbay = null; delete this.slcbay;
        this.slclevel = null; delete this.slclevel;
        this.spmzone = null; delete this.spmzone;
        this.spmaisle = null; delete this.spmaisle;
        this.spmbay = null; delete this.spmbay;
        this.spmlevel = null; delete this.spmlevel;
        this.msdstate = null; delete this.msdstate;
        this.formatdate = null; delete this.formatdate;
        this.formatdatelong = null; delete this.formatdatelong;
        this.page = null; delete this.page;
        this.pageSize = null; delete this.pageSize;
        this.slrowlmt = null; delete this.slrowlmt;
        this.lsrowlmt = null; delete this.lsrowlmt;
        this.lssort = null; delete this.lssort;
        this.lsreverse = null; delete this.lsreverse;
        
    }
  

}
