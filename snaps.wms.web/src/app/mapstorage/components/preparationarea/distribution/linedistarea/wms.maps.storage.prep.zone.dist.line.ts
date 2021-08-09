import { Component, OnInit,OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { mapsprepzonestockService } from '../../../../services/wms.maps.storage.prep.service';
import { authService } from '../../../../../auth/services/auth.service';
import { zoneprep_md, zoneprln_md } from '../../../../Models/wms.maps.storage.prep.zone.model';
import { adminService } from 'src/app/admn/services/account.service';
import { NgPopupsService } from 'ng-popups';
import { lov } from 'src/app/helpers/lov';

declare var $: any;
@Component({
  selector: 'app-maps-prep-zone-dist-line',
  templateUrl: 'wms.maps.storage.prep.zone.dist.line.html',
  styles: ['.dgzone { height:calc(100vh - 155px) !important; } '],
})
export class mapsprepzonedistlineComponent implements OnInit {
    @Input() dateformat: string;
    @Input() dateformatlong: string;

    slczone:string = "";
    slctflow:boolean = false;
    //State List
    lststate:lov[] = new Array();
    //Zone Selection 
    sobzone:zoneprep_md = new zoneprep_md();
    //Line Selection
    sobline:zoneprln_md = new zoneprln_md();
    //Location list
    lstline:zoneprln_md[] = new Array();
    //Zone parameter 
    pm:zoneprep_md = new zoneprep_md();
    //Zone list
    lstzone:zoneprep_md[] = new Array();
    //Preparation unit list 
    lstunit:lov[] = new Array();
    //Preparation unit selection 
    slcunit:lov = new lov();
    //Direction list
    lstdirection:lov[] = new Array();
    //Direction direction
    slcdirection:lov = new lov();

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
    ngOpsIn(o:zoneprep_md){ this.sobzone = o; this.ngLst(); }
    ngOpsslc(o:zoneprln_md){ 
        this.sobline = o; 
        this.slcdirection = this.lstdirection.find(x=>x.value == this.sobline.lsdirection);
        this.slctflow = (this.sobline.tflow == "IO") ? true : false;
    }
    ngOpsnew() { 
      this.sobline = new zoneprln_md(); 
      this.sobline.orgcode = this.sobzone.orgcode; this.sobline.site = this.sobzone.site;
      this.sobline.spcarea = this.sobzone.spcarea; this.sobline.przone = this.sobzone.przone;
      this.sobline.depot = this.sobzone.depot; this.sobline.tflow = "NW"; this.slctflow = true; 
      this.sobzone.spcarea = "XD";
      this.toastr.info("<span class='fn-07e'>New zone ready to configuration </span>",null,{ enableHtml : true }); 
    }
    ngOpsvalidate(){ 
      if(this.sobzone.przone == null) { this.toastr.warning("<span class='fn-07e'>Require zone code</span>",null,{ enableHtml : true }); }
      else if (this.sobzone.przonename == null) { this.toastr.warning("<span class='fn-07e'>Require zone name</span>",null,{ enableHtml : true }); }
      else if (this.sobzone.przonedesc == null) { this.toastr.warning("<span class='fn-07e'>Require zone desciption</span>",null,{ enableHtml : true }); }
      else { 
        this.sobline.tflow =(this.sobline.tflow != "NW") ?  (this.slctflow == true) ? "IO" : "XX" : this.sobline.tflow;
        this.sobline.lsdirection = this.slcdirection.value;
        this.ngPopups.confirm('Do you confirm line path configuration ?')
            .subscribe(res => {
            if (res) {
                this.sv.prlnzoneupsert(this.sobline).pipe().subscribe(            
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
          this.sv.prlnzoneremove(this.sobline).pipe().subscribe(            
            (res) => { this.toastr.success("<span class='fn-07e'>Modify zone success</span>",null,{ enableHtml : true }); this.ngLst(); },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
          );
        } 
      });
    }
    ngLst(){ 
      this.pm.spcarea = "XD";
      this.sv.prepzoneline(this.sobzone).pipe().subscribe(            
        (res) => { this.lstline = res; },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngSetmaster(){ 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.lststate = res; console.log(this.lststate); this.ngLst(); },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.getlov("LOCATIOIN","DIRECTION").pipe().subscribe(
        (res) => { this.lstdirection = res; },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    ngDecIcon(o:string){ return this.lststate.find(x=>x.value == o).icon; }
    ngDecStr(o:string) { return this.lststate.find(x=>x.value == o).desc; }
}
