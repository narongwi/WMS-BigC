import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { shareService } from 'src/app/share.service';
import { transpileModule } from 'typescript';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_gn, locdw_ls, locdw_md, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-locationmodify',
  templateUrl: 'maps.location.modify.html',
  styles: ['.dgentity { height: 610px !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
})
export class mapslocationmodify implements OnInit {
  //@Input() locmd:locdw_md;
  crlocdw:locdw_md = new locdw_md();
  swflowin:Boolean = true;
  swflowou:boolean = true;
  swmixproduct:boolean = true;
  swmixaging:boolean = true;
  swmixlot:boolean = true;
  swpicking:boolean = true;
  swstacking:boolean;

  //Date format 
  public formatdate:string;
  public formatdatelong:string;

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
    ngSetup(o:locdw_md) { 
      this.crlocdw = o;
      this.swflowin = (['IO','IX'].includes(o.tflow)) ? true : false;
      this.swflowou = (['IO','XO'].includes(o.tflow)) ? true : false;
      this.swmixproduct = (o.lsmixarticle == 1) ? true : false;
      this.swmixaging = (o.lsmixage == 1) ? true : false;
      this.swmixlot = (o.lsmixlotno == 1) ? true : false;
      this.swpicking = (o.spcpicking == 1) ? true : false;
      this.swstacking = (o.lsstackable == 1) ? true : false;
    }
    validate() {
      this.crlocdw.tflow = ((this.swflowin==true)? "I" : "X") + ((this.swflowou==true)?"O":"X");
      this.crlocdw.lsmixarticle = (this.swmixproduct == true) ? 1 : 0;
      this.crlocdw.lsmixage = (this.swmixaging == true) ? 1 : 0;
      this.crlocdw.lsmixlotno = (this.swmixlot == true) ? 1 : 0;
      this.crlocdw.spcpicking = (this.swpicking == true)? 1 : 0;
      this.crlocdw.lsstackable = (this.swstacking == true) ? 1 : 0;
      this.ngPopups.confirm('Do you accept location property ?')
        .subscribe(res => {
            if (res) {
              this.sv.upsertlocdw(this.crlocdw).pipe().subscribe( (res) => { this.toastr.success('Save successful');  } );
            } 
        });
    }
    drop() { 
      this.ngPopups.confirm('Do you accept to drop location ?')
      .subscribe(res => { 
          if (res) {
            this.sv.droplocdw(this.crlocdw).pipe().subscribe(            
                (res) => { this.toastr.success('Drop aisle successful');  }
            );              
          } 
      });
    }
}