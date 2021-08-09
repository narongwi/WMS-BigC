import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { TimeoutError } from 'rxjs';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_gn, locdw_ls, locdw_md, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-gridmodify',
  templateUrl: 'maps.grid.modify.html'
})
export class mapsgridmodify implements OnInit {
    @Input() locmd:locdw_md;
    ops:locdw_gn = new locdw_gn();
    selzone:lov;
    selaislefr:lov;
    selaisleto:lov;
    selbayfr:lov;
    selbayto:lov;
    sellevelfr:lov;
    sellevelto:lov;

    lszone:lov[] = new Array();
    lsaisle:lov[] = new Array();

    swflow:Boolean = true;

    formatdate:string;
    formatdatelong:string;
    swflowin:boolean;
    swflowou:boolean;
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
    ngOnChanges() { 
      this.swflowin = (['IO','IX'].includes(this.locmd.tflow)) ? true : false;
      this.swflowou = (['XO','IO'].includes(this.locmd.tflow)) ? true : false;
      console.log(this.locmd.tflow);
    }
    validate() { 
      this.locmd.tflow = ((this.swflowin==true)? "I" : "X") + ((this.swflowou==true)?"O":"X");
      this.ngPopups.confirm('Do you accept location property ?')
        .subscribe(res => {
            if (res) {
              this.sv.upsertlocdw(this.locmd).pipe().subscribe( (res) => { this.toastr.success('Save successful');  } );
            } 
        });
    }
}