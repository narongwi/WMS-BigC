import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_gn, locdw_gngrid, locdw_ls, locdw_md, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-gridgenerate',
  templateUrl: 'maps.grid.generate.html'
})
export class mapsgridgenerate implements OnInit, OnDestroy {
    ops:locdw_gngrid = new locdw_gngrid();
    slczone:lov;
    slcaislefr:lov;
    slcaisleto:lov;
    slcbayfr:lov;
    slcbayto:lov;
    slclevelfr:lov;
    slclevelto:lov;

    lszone:lov[] = new Array();
    lsaisle:lov[] = new Array();

    swflow:Boolean = true;
    swmixproduct:Boolean = false;
    swmixaging:Boolean = false;
    swpicking:Boolean = false;
    swmixmfglot:Boolean = false;
    swstackable:Boolean = false;

    public pmzone:locup_pm = new locup_pm();
    public isOn:boolean = false; //status for let every object know under operation
     
    constructor(
      private av: authService,      
      private sv: mapstorageService,
      private router: RouterModule,
      private toastr: ToastrService,
      private ngPopups: NgPopupsService) { 
      this.av.retriveAccess();  
      this.ops.orgcode = this.av.crProfile.orgcode;
      this.ops.site = this.av.crRole.site;
      this.ops.depot = this.av.crRole.depot;
    }
    ngOnInit() { this.getzone(); }

    getzone(){ 
      this.pmzone.orgcode = this.av.crProfile.orgcode;
      this.pmzone.site = this.av.crRole.site;
      this.pmzone.depot = this.av.crRole.depot;
      this.pmzone.spcarea = "XD";
      this.sv.lovzone(this.pmzone).subscribe(            
          (res) => { this.lszone = res.filter(x=>x.valopnfirst == "XD"); if(this.lszone.length > 0) { this.slczone = this.lszone[0];  } },
          (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }

    generate(){ 
      this.ops.zone = this.slczone.value;
      this.ops.spcarea = "XD";
      this.ops.tflow = (this.swflow == true) ? "IO" : "XX";
      this.ngPopups.confirm('Do you confirm to generate the grid storage ?')
      .subscribe(res => {
          if (res) {
            this.isOn = true;
            console.log("Generate "); console.log(this.ops);
            this.sv.gengrid(this.ops).subscribe(            
              (res) => { this.toastr.success("<span class='fn-07e'>Generate successful</span>",null,{ enableHtml : true }); },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );             
          } else {
            console.log('You clicked Cancel. You smart.'); this.isOn = false;
          }
      });
    }

    ngOnDestroy() : void { 
      this.ops          = null; delete this.ops;
      this.slczone      = null; delete this.slczone;
      this.slcaislefr   = null; delete this.slcaislefr;
      this.slcaisleto   = null; delete this.slcaisleto;
      this.slcbayfr     = null; delete this.slcbayfr;
      this.slcbayto     = null; delete this.slcbayto;
      this.slclevelfr   = null; delete this.slclevelfr;
      this.slclevelto   = null; delete this.slclevelto;
      this.lszone       = null; delete this.lszone;
      this.lsaisle      = null; delete this.lsaisle;
      this.swflow       = null; delete this.swflow;
      this.swmixproduct = null; delete this.swmixproduct;
      this.swmixaging   = null; delete this.swmixaging;
      this.swpicking    = null; delete this.swpicking;
      this.swmixmfglot  = null; delete this.swmixmfglot;
      this.swstackable  = null; delete this.swstackable ;
      this.pmzone       = null; delete this.pmzone;
      this.isOn		      = null; delete this.isOn	;	
    }
}