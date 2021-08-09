import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { mapstorageService } from '../../services/app-mapstorage.service';
import { locdw_ls, locdw_picking, locdw_pivot, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage';
import { NgPopupsService } from 'ng-popups';

declare var $: any;
@Component({
  selector: 'app-mapspicking',
  templateUrl: 'maps.picking.html'

})
export class mapspickingComponent implements OnInit {
  public lslog:lov[] = new Array();

  public lszone:lov[] = new Array();
  public lsaisle:lov[] = new Array();
  public lsbay:lov[] = new Array();
  public lslevel:lov[] = new Array();

  public lslocation:locdw_picking[] = new Array();

  public pmfnd:locdw_pm = new locdw_pm();
  public pmmsd:locup_pm = new locup_pm();

  public slcmd:locdw_picking = new locdw_picking();

  public slczoneto:lov;
  public slczone:lov;
  public slcaisle:lov;
  public slcbay:lov;
  public slclevel:lov;

  constructor(private sv: mapstorageService,
              private av: authService, 
              private router: RouterModule,
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
  ngOnDestroy():void {  }
  ngAfterViewInit(){ /*setTimeout(this.toggle, 1000);*/ }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  getzone(){ 
        this.sv.lovzonedist(this.pmmsd).subscribe(            
            (res) => { this.lszone = res; },
            (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { } 
        );
    }
  // getaisle(){ 
  //     this.sv.lovaisledist(this.pmmsd).subscribe(            
  //         (res) => { this.lsaisle = res; },
  //         (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
  //         () => { } 
  //     );
  // }
  // getbay() { 
  //     this.sv.lovbaydist(this.pmmsd).subscribe(            
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

  selpivot(o:locdw_picking){ 
      this.slcmd = o; 
  }
  fndlocation(){ 
    this.pmfnd.lszone = (this.slczone) ? this.slczone.desc : "";
    this.pmfnd.lsaisle = (this.slcaisle) ? this.slcaisle.desc : "";
    this.pmfnd.lsbay = (this.slcbay) ? this.slcbay.desc : "";
    this.pmfnd.lslevel = (this.slclevel) ? this.slclevel.desc : "";
    this.pmfnd.fltype = "LC";
    this.sv.getpicking(this.pmfnd).pipe().subscribe(            
        (res) => { this.lslocation = res; },
        (err) => { this.toastr.error(err.error.message); },
        () => { }
    );
  }
  validate() { 
    this.ngPopups.confirm('Do you accept change of picking address ?')
    .subscribe(res => {
        if (res) {
          this.sv.setpicking(this.slcmd).pipe().subscribe(            
              (res) => { this.toastr.success("<span class='fn-1e15'>Save successful</span>"); this.fndlocation(); },
              (err) => { this.toastr.error(err.error.message); },
              () => { }
          );           
        } 
    });
  }
}
