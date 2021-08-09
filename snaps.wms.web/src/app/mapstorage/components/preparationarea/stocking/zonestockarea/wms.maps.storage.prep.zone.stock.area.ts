import { Component, OnInit,OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { mapsprepzonestockService } from '../../../../services/wms.maps.storage.prep.service';
import { authService } from '../../../../../auth/services/auth.service';
import { zoneprep_md } from '../../../../Models/wms.maps.storage.prep.zone.model';
import { adminService } from 'src/app/admn/services/account.service';
import { NgPopupsService } from 'ng-popups';
import { lov } from 'src/app/helpers/lov';
import { ThumbXDirective } from 'ngx-scrollbar/lib/scrollbar/thumb/thumb.directive';
import { ouhanderlingunitService } from 'src/app/outbound/Services/oub.handerlingunit.service';
import { handerlingunit } from 'src/app/outbound/Models/oub.handlingunit.model';

declare var $: any;
@Component({
  selector: 'app-maps-prep-zone-stock-area',
  templateUrl: 'wms.maps.storage.prep.zone.stock.area.html',
  styles: ['.dgzone { height:calc(100vh - 155px) !important; } '],
})
export class mapsprepzonestockareaComponent implements OnInit {
    @Input() dateformat: string;
    @Input() dateformatlong: string;
    @Output() ngOutvalue = new EventEmitter<zoneprep_md>();

    slczone:string = "";
    slctflow:boolean = false;
    //State List
    lststate:lov[] = new Array();
    //HU Selection
    lsthu:lov[] = new Array();
    slchu:lov = new lov();
    //Zone Selection 
    sobzone:zoneprep_md = new zoneprep_md();
    //Zone parameter 
    pm:zoneprep_md = new zoneprep_md();
    //Zone list
    lstzone:zoneprep_md[] = new Array();
    //handling unit selction
    obchu:handerlingunit = new handerlingunit(); 
    //Handling unit parameter
    prmhu:handerlingunit = new handerlingunit(); 
    constructor(private sv: mapsprepzonestockService,
                private av: authService, 
                private mv: adminService, 
                private hv: ouhanderlingunitService,
                private router: RouterModule,
                private toastr: ToastrService,
                private ngPopups: NgPopupsService) { 
    }
    ngOnInit(): void { this.ngSetmaster();  }
    ngOnDestroy():void {  }
    ngAfterViewInit(){ }
    ngOpsout(){ this.ngOutvalue.emit(this.sobzone); }
    ngOpsslc(o:zoneprep_md){ 
      this.sobzone = o; 
      this.slchu = this.lsthu.find(x=>x.value == o.hutype); 
      this.slctflow = (this.sobzone.tflow == "IO") ? true : false; 
      this.ngOpshu(false);
    }
    ngOpsnew() { 
      this.sobzone = new zoneprep_md(); 
      this.sobzone.spcarea = "ST"; this.sobzone.tflow = "NW"; this.slctflow = true;
      if (this.slchu == null) { 
        this.slchu = this.lsthu[0];         
      }
      this.ngOpshu(true);
      this.toastr.info("<span class='fn-07e'>New zone ready to configuration </span>",null,{ enableHtml : true }); 
    }
    ngOpsvalidate(){ 
      if(this.sobzone.przone == null) { this.toastr.warning("<span class='fn-07e'>Require zone code</span>",null,{ enableHtml : true }); }
      else if (this.sobzone.przonename == null) { this.toastr.warning("<span class='fn-07e'>Require zone name</span>",null,{ enableHtml : true }); }
      else if (this.sobzone.przonedesc == null) { this.toastr.warning("<span class='fn-07e'>Require zone desciption</span>",null,{ enableHtml : true }); }
      else { 
        this.sobzone.hutype = this.slchu.value;
        this.sobzone.tflow = (this.slctflow == true) ? "IO" : "XX";
        this.ngPopups.confirm('Do you confirm zone configuration ?')
        .subscribe(res => {
          if (res) {
            this.sv.prepzoneupsert(this.sobzone).pipe().subscribe(            
              (res) => { this.toastr.success("<span class='fn-07e'>Modify zone success</span>",null,{ enableHtml : true }); this.sobzone.tflow = "IO"; this.ngLst(); },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { }
            );
          } 
        });
      }
    }
    ngOpsremove(){ 
      this.ngPopups.confirm('Do you confirm remove zone ?')
      .subscribe(res => {
        if (res) {
          this.sv.prepzoneremove(this.sobzone).pipe().subscribe(            
            (res) => { this.toastr.success("<span class='fn-07e'>Modify zone success</span>",null,{ enableHtml : true }); this.ngLst(); },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
          );
        } 
      });
    }
    ngOpshu(o:Boolean) {
      this.prmhu.huno = this.slchu.value;
      this.hv.get(this.prmhu).pipe().subscribe((res) => {
        this.obchu = res;
        if (o==true) { 
          this.sobzone.hucapvolume = 100;
          this.sobzone.hucapweight = 100;
          this.sobzone.huvalvolume = this.obchu.mxvolume;
          this.sobzone.huvalweight = this.obchu.mxweight;
        }
      });
    }
    ngCalhu_weightpct(){ 
      this.sobzone.huvalweight = (this.sobzone.hucapweight == null) ? this.obchu.mxweight : (this.obchu.mxweight / 100) * this.sobzone.hucapweight;
    }    
    ngCalhu_weightval(){ 
      this.sobzone.hucapweight = Number(Number((this.sobzone.huvalweight / this.obchu.mxweight) * 100).toFixed(1));
    }

    ngCalhu_volumepct(){ 
      this.sobzone.huvalvolume = (this.sobzone.hucapvolume == null) ? this.obchu.mxvolume : (this.obchu.mxvolume / 100) * this.sobzone.hucapvolume;
    }    
    ngCalhu_volumeval(){ 
      this.sobzone.hucapvolume = Number(Number((this.sobzone.huvalvolume / this.obchu.mxvolume) * 100).toFixed(1));
    }

    ngLst(){ 
      this.pm.spcarea = "ST";
      this.sv.prepzonelist(this.pm).pipe().subscribe(            
        (res) => { this.lstzone = res; },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngSetmaster(){ 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.lststate = res;this.ngLst(); },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.gethus().pipe().subscribe(
        (res) => { this.lsthu = res;  if (this.lsthu.length > 0) { this.slchu = this.lsthu[0]; this.ngOpshu(true); } },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngDecIcon(o:string){ return this.lststate.find(x=>x.value == o).icon; }
    ngDecStr(o:string) { return this.lststate.find(x=>x.value == o).desc; }
}
