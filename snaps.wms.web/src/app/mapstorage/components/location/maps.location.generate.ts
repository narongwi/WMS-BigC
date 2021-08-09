import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { locdw_gn, locdw_ls, locdw_md, locdw_pm, locup_pm } from '../../Models/mdl-mapstorage'
import { mapstorageService } from '../../services/app-mapstorage.service';
@Component({
  selector: 'appmaps-locationgenerate',
  templateUrl: 'maps.location.generate.html',
  styles: ['.dgentity { height:290px !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
})
export class mapslocationgenerate implements OnInit {
    ops:locdw_gn = new locdw_gn();
    slczone:lov;
    slcaislefr:lov;
    slcaisleto:lov;
    slcbayfr:lov;
    slcbayto:lov;
    slclevelfr:lov;
    slclevelto:lov;
    slcstack:lov;

    lsstack:lov[] = [];
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
    ngOnInit() { this.getzone(); this.getstack();}
  //   export class lov { 
  //     desc: string;
  //     value: string;
  //     valopnfirst: string;
  //     valopnsecond: string;
  //     valopnthird:string;
  //     valopnfour:string;
  //     icon: string;
  // }

    getstack(){
      this.lsstack = [
        { desc:"A,B,C...",value:"AZ",valopnfirst:"",valopnsecond:"",valopnthird:"",valopnfour:"",icon:""},
        { desc:"1,2,3...",value:"NM",valopnfirst:"",valopnsecond:"",valopnthird:"",valopnfour:"",icon:""},
      ];

      this.slcstack = this.lsstack[0];
    }
    getzone(){ 
      this.pmzone.orgcode = this.av.crProfile.orgcode;
      this.pmzone.site = this.av.crRole.site;
      this.pmzone.depot = this.av.crRole.depot;
      this.sv.lovzone(this.pmzone).subscribe(            
          (res) => { this.lszone = res; if(this.lszone.length > 0) { this.slczone = this.lszone[0]; this.getaisle(); } },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }

    getaisle(){
      this.pmzone.lszone = this.slczone.value;
      this.sv.lovaisle(this.pmzone).subscribe(            
        (res) => { this.lsaisle = res; },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    recal() { 
      if(this.ops.lsdmheight != null && this.ops.lsdmlength != null && this.ops.lsdmwidth != null && this.ops.location != null) { 
         this.ops.lsmxheight = (this.ops.lsdmheight / this.ops.location) - this.ops.lsgaptop;
         this.ops.lsmxlength = (this.ops.lsdmlength / this.ops.location) - this.ops.lsgapleft;
         this.ops.lsmxwidth = (this.ops.lsdmwidth / this.ops.location) - this.ops.lsgapright;
         this.ops.lsmxvolume = this.ops.lsmxheight * this.ops.lsmxlength * this.ops.lsmxwidth;
      }
    }
    generate(){ 
      this.ops.zone = this.slczone.value;
      this.ops.spcarea = this.slczone.valopnfirst;
      this.ops.aislefr = this.slcaislefr.value;
      this.ops.aisleto = this.slcaisleto.value;
      this.ops.tflow = (this.swflow == true) ? "IO" : "XX";
      this.ops.lsmixage = ( this.swmixaging == true) ? 1 : 0;
      this.ops.lsmixarticle = (this.swmixproduct == true) ? 1 : 0;
      this.ops.lsmixlotno = ( this.swmixmfglot == true) ? 1 : 0;
      this.ops.spcpicking = ( this.swpicking == true) ? 1 : 0;
      this.ops.lsstackable = ( this.swstackable == true) ? 1 : 0;
      this.ops.lsstacklabel = this.slcstack.value.toString();

      if(this.ops.location > 0){
        this.ngPopups.confirm('Do you confirm to generate the location storage ?')
        .subscribe(res => {
            if (res) {
              this.isOn = true;
              this.sv.genlocation(this.ops).subscribe(            
                (res) => { this.lsaisle = res;this.toastr.success('generate the location successful'); },
                (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                () => { } 
              );             
            } else {
              console.log('You clicked Cancel. You smart.'); this.isOn = false;
            }
        });
      }else {
        this.toastr.warning('location per level is required!');
      }
    }
}