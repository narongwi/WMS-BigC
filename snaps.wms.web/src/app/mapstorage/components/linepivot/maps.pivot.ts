import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { mapstorageService } from '../../services/app-mapstorage.service';
import { locdw_ls, locdw_pivot, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage';
import { NgPopupsService } from 'ng-popups';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'app-mapspivot',
  templateUrl: 'maps.pivot.html',
    styles: ['.dgpivot { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],

})
export class mapspivotComponent implements OnInit,OnDestroy {
  public lslog:lov[] = new Array();

  public lszone:lov[] = new Array();
  public lsaisle:lov[] = new Array();
  public lsbay:lov[] = new Array();
  public lslevel:lov[] = new Array();

  public lslocation:locdw_pivot[] = new Array();

  public pmfnd:locdw_pm = new locdw_pm();
  public pmmsd:locup_pm = new locup_pm();

  public slcmd:locdw_pivot = new locdw_pivot();

  public slczoneto:lov;
  public slczone:lov;
  public slcaisle:lov;
  public slcbay:lov;
  public slclevel:lov;
  public crtab:number = 1;
  constructor(private sv: mapstorageService,
              private av: authService, 
              private ss: shareService,
              private toastr: ToastrService,
              private ngPopups: NgPopupsService) { 
    this.av.retriveAccess(); 
    this.pmmsd.orgcode = this.av.crProfile.orgcode;
    this.pmmsd.site = this.av.crRole.site;
    this.pmmsd.depot = this.av.crRole.depot;
    this.getzone();
    // this.getaisle();
    // this.getbay();
    // this.getlevel();
  }

  ngOnInit(): void { }
  ngAfterViewInit(){ /*setTimeout(this.toggle, 1000);*/ }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  getzone(){ 
        this.ss.storagezone().subscribe((res) => { this.lszone = res; } );
    }
    getaisle(){ 
        this.sv.getaisle().subscribe((res) => { this.lsaisle = res; }
        );
    }
    // getbay() { 
    //     this.sv.getbay().subscribe(            
    //         (res) => { this.lsbay = res; },
    //         (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
    //         () => { } 
    //     );
    // }
    // getlevel(){ 
    //     this.sv.lovleveldist(this.pmmsd).subscribe(            
    //         (res) => { this.lslevel = res; },
    //         (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
    //         () => { } 
    //     );
    // }
  selln(o:locdw_ls){ 
      this.sv.getlocdw(o).pipe().subscribe(            
          (res) => { this.slcmd = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
      );
  }
  selpivot(o:locdw_pivot){
      this.slcmd = o;
  }
  fndlocation(){ 
    this.pmfnd.lszone = (this.slczone) ? this.slczone.desc : "";
    this.pmfnd.lsaisle = (this.slcaisle) ? this.slcaisle.desc : "";
    this.pmfnd.lsbay = (this.slcbay) ? this.slcbay.desc : "";
    this.pmfnd.lslevel = (this.slclevel) ? this.slclevel.desc : "";
    this.pmfnd.fltype = "LC";
    this.sv.getpivot(this.pmfnd).pipe().subscribe(            
        (res) => { this.lslocation = res; },
        (err) => { this.toastr.error(err.error.message); },
        () => { }
    );
  }
  validate() { 
    this.ngPopups.confirm('Do you accept change of pivot address ?')
    .subscribe(res => {
        if (res) {
          this.sv.setpivot(this.slcmd).pipe().subscribe(            
              (res) => { this.toastr.success("<span class='fn-1e15'>Save successful</span>"); this.fndlocation(); },
              (err) => { this.toastr.error(err.error.message); },
              () => { }
          );
           
        } 
    });
  }

  ngOnDestroy():void { 
    this.lslog      = null; delete this.lslog ;    
    this.lszone     = null; delete this.lszone ;   
    this.lsaisle    = null; delete this.lsaisle;   
    this.lsbay      = null; delete this.lsbay  ;   
    this.lslevel    = null; delete this.lslevel ;  
    this.lslocation = null; delete this.lslocation;
    this.pmfnd      = null; delete this.pmfnd ;    
    this.pmmsd      = null; delete this.pmmsd ;    
    this.slcmd      = null; delete this.slcmd ;    
    this.slczoneto  = null; delete this.slczoneto ;
    this.slczone    = null; delete this.slczone ;  
    this.slcaisle   = null; delete this.slcaisle ; 
    this.slcbay     = null; delete this.slcbay  ;  
    this.slclevel   = null; delete this.slclevel;  
    this.crtab      = null; delete this.crtab   ;  
   }
}
