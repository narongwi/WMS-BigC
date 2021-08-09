import { Component, OnInit, Input, EventEmitter, Output, OnDestroy } from '@angular/core';
import { NgbPaginationConfig, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_ls, locdw_md, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-gridline',
  templateUrl: 'maps.grid.line.html',
  styles: ['.dggrid { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsgridline implements OnInit, OnDestroy {

//   @Input() iconstate: string;
    @Output() selln = new EventEmitter<locdw_ls>();
    public mdfloc:locdw_ls;

    ison:boolean=false;
    public lszone:lov[] = new Array();
    public lsaisle:lov[] = new Array();
    public lsbay:lov[] = new Array();
    public lslevel:lov[] = new Array();

    public lsgrid:locdw_ls[] = new Array();
    public crgrid:locdw_md = new locdw_md();

    public pmfnd:locdw_pm = new locdw_pm();
    public pmlov:locup_pm = new locup_pm();

    public slczone:lov = new lov();
    public slcaisle:lov = new lov();
    public slcbay:lov = new lov();
    public slclevel:lov = new lov();

    public spmzone:lov = null; 
    public spmaisle:lov = null;
    public spmbay:lov = null; 
    public spmlevel:lov = null;

    public msdstate:lov[] = new Array();

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
        this.getstate(); this.getzone(); 
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fndgrid(); } 
    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    dscicon(o:string) { return this.msdstate.find(x=>x.value == o).icon; }
    getstate() { 
        this.mv.getlov("ALL","FLOW").pipe().subscribe(
          (res) => { this.msdstate = res;
            this.msdstate.push({ value : 'NW', desc : 'New Zone', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
          },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
    }
    getzone(){ 
        this.sv.lovzonedist(this.pmlov).subscribe(            
            (res) => { this.lszone = res; },
            (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { } 
        );
    }
    // getaisle(){ 
    //     this.sv.lovaisledist(this.pmlov).subscribe(            
    //         (res) => { this.lsaisle = res; },
    //         (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
    //         () => { } 
    //     );
    // }
    // getbay() { 
    //     this.sv.lovbaydist(this.pmlov).subscribe(            
    //         (res) => { this.lsbay = res; },
    //         (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
    //         () => { } 
    //     );
    // }
    // getlevel(){ 
    //     this.sv.lovleveldist(this.pmlov).subscribe(            
    //         (res) => { this.lslevel = res; },
    //         (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
    //         () => { } 
    //     );
    // }

    fndgrid(){ 
        this.pmfnd.lszone = (this.spmzone) ? this.spmzone.value : "";
        this.pmfnd.lsaisle = (this.spmaisle) ? this.spmaisle.value : "";
        this.pmfnd.lsbay = (this.spmbay) ? this.spmbay.value : "";
        this.pmfnd.lslevel = (this.spmlevel) ? this.spmlevel.value : "";
        this.pmfnd.fltype = "LC";
        this.pmfnd.spcarea = "XD";
        this.sv.fndlocdw(this.pmfnd).pipe().subscribe(            
            (res) => { this.lsgrid = res; },
            (err) => { this.toastr.error(err.error.message); this.ison = false; },
            () => { }
        );
    }
    selgrid(o:locdw_ls){
        this.selln.emit(o);
        // this.sv.getlocdw(o).pipe().subscribe(            
        //     (res) => { this.lsgrid = res; },
        //     (err) => { this.toastr.error(err.error.message); this.ison = false; },
        //     () => { }
        // );
    }

    ngOnDestroy() : void { 
        this.mdfloc   = null; delete this.mdfloc;
        this.ison     = null; delete this.ison;
        this.lszone   = null; delete this.lszone;
        this.lsaisle  = null; delete this.lsaisle;
        this.lsbay    = null; delete this.lsbay;
        this.lslevel  = null; delete this.lslevel;
        this.lsgrid   = null; delete this.lsgrid;
        this.crgrid   = null; delete this.crgrid;
        this.pmfnd    = null; delete this.pmfnd;
        this.pmlov    = null; delete this.pmlov;
        this.slczone  = null; delete this.slczone;
        this.slcaisle = null; delete this.slcaisle;
        this.slcbay   = null; delete this.slcbay;
        this.slclevel = null; delete this.slclevel;
        this.spmzone  = null; delete this.spmzone;
        this.spmaisle = null; delete this.spmaisle;
        this.spmbay   = null; delete this.spmbay;
        this.spmlevel = null; delete this.spmlevel;
        this.msdstate = null; delete this.msdstate;
     
    }

}
