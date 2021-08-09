import { Component, OnInit, Input, OnDestroy, Output, EventEmitter } from '@angular/core';
import {  accn_md } from '../../../models/account.model';
import { lov } from '../../../../helpers/lov';
import { adminService } from '../../../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { NgPopupsService } from 'ng-popups';
import { authService } from 'src/app/auth/services/auth.service';
@Component({
  selector: 'app-option',
  templateUrl: './option.component.html',
  styles: ['.dgaccn { height:calc(100vh - 240px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  
})
export class OptionComponent implements OnInit,OnDestroy {
  @Input() mdaccn:accn_md;
  @Input() lsrole:lov[];
  @Input() lsacnstate:lov[];
  @Input() lstype:lov[]
  @Output() refresh = new EventEmitter();

  formatdatelong:string;
  formatdate:string;
  public crstate:string;
  public slcstate:lov;
  public slcrole:lov;
  public slctype:lov;
  public isOn:Boolean = false; // on process state
  constructor(private sv: adminService, private av: authService, private toastr: ToastrService,private ngPopups: NgPopupsService) { 
    this.av.retriveAccess();
    this.formatdate = this.av.crProfile.formatdate;
    this.formatdatelong = this.av.crProfile.formatdatelong;
   }

  ngOnInit() { }
  ngAfterViewInit(){ }

  

  newaccount() { 
    this.mdaccn = new accn_md();
    this.mdaccn.tflow = "NW";
    this.mdaccn.formatdate = this.av.crProfile.formatdate;
    this.mdaccn.formatdatelong = this.av.crProfile.formatdatelong;
    this.mdaccn.formatdateshort = this.av.crProfile.formatdateshort;
    this.slcstate = this.lsacnstate.find(e=>e.value == "IO");
    this.toastr.info("New account is ready ");
  }
    resetpw() {
    this.ngPopups.confirm(`Do you reset password account ` + this.mdaccn.accncode + `?`)
    .subscribe(res => {
        if (res) {
          this.isOn = true;
          if(this.mdaccn.tflow != `NW`) { 
             this.sv.accnReset(this.mdaccn).pipe().subscribe(            
              (res) => { 
                this.toastr.success( `reset password successful`); 
                this.isOn = false;
              }
          ); 
          }
        }
    });
  }
  validate() {
    this.ngPopups.confirm(`Do you confirm to ` + (this.mdaccn.tflow == `NW`) ? `create` : `modify` + ` account ` + this.mdaccn.accncode + `?`)
    .subscribe(res => {
        if (res) {
          this.isOn = true;


          if(this.mdaccn.tflow != `NW`) { 
            this.mdaccn.tflow = this.slcstate.value; 
          }

          this.mdaccn.accsrole = this.slcrole.value;

          if (this.mdaccn.tflow != "NW") {
            this.mdaccn.tflow  = this.slcstate.value; 
          }

          this.mdaccn.accntype = this.slctype.value;
          
          this.sv.accnUpsert(this.mdaccn).pipe().subscribe(            
              (res) => { this.toastr.success( (this.mdaccn.tflow == `NW`) ? `create` : `modify`+ ` account successful`); 
              this.isOn = false;
              this.refresh.emit(true);
            }
          ); 
        }
    });
  }

  ngOnChanges() { 
    console.log(this.mdaccn);
    this.slctype = this.lstype.find(e=>e.value == this.mdaccn.accntype);
    this.slcrole = this.lsrole.find(e=>e.value == this.mdaccn.accsrole);
    this.slcstate = this.lsacnstate.find(e=>e.value == this.mdaccn.tflow);
  }
  ngSelcompare(item, selected) { return item.value === selected } //compare selected object with ng-select
  ngOnDestroy():void{ 

  }

}