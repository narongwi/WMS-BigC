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

declare var $: any;
@Component({
  selector: 'app-maps-prep-zone-dist-area',
  templateUrl: 'wms.maps.storage.prep.zone.dist.area.html',
  styles: ['.dgzone { height:calc(100vh - 155px) !important; } '],
})
export class mapsprepzonedistareaComponent implements OnInit {
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
    constructor(private sv: mapsprepzonestockService,
                private av: authService, 
                private mv: adminService, 
                private router: RouterModule,
                private toastr: ToastrService,
                private ngPopups: NgPopupsService) { 
    }
    ngOnInit(): void { this.ngSetmaster();  }
    ngOnDestroy():void {  }
    ngAfterViewInit(){ }
    ngOpsout(){ this.ngOutvalue.emit(this.sobzone); }
    ngOpsslc(o:zoneprep_md){ this.sobzone = o; this.slchu = this.lsthu.find(x=>x.value == o.hutype); 
      this.slctflow = (this.sobzone.tflow == "IO") ? true : false;
     }
    ngOpsnew() { 
      this.sobzone = new zoneprep_md(); 
      this.sobzone.spcarea = "XD"; this.sobzone.tflow = "NW"; this.slctflow = true;
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

    ngLst(){ 
      this.pm.spcarea = "XD";
      this.sv.prepzonelist(this.pm).pipe().subscribe(            
        (res) => { this.lstzone = res; },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngSetmaster(){ 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.lststate = res; console.log(this.lststate); this.ngLst(); },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.gethus().pipe().subscribe(
        (res) => { this.lsthu = res;  },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngDecIcon(o:string){ return this.lststate.find(x=>x.value == o).icon; }
    ngDecStr(o:string) { return this.lststate.find(x=>x.value == o).desc; }
}
