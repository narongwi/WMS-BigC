import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { role_md, role_pm } from '../../models/role.model';
import { lov } from '../../../helpers/lov';
import { adminService } from '../../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { NgPopupsService } from 'ng-popups';
import { accn_category, accn_permision, accn_roleacs } from '../../models/account.model';
@Component({
  selector: 'role-opr',
  templateUrl: 'role-opr.html',
  styles: ['.dgaccn { height:calc(100vh - 240px) !important;  } ', '.dglines { height:calc(100vh - 200px) !important; }'],
})
export class oprrole implements OnInit, OnChanges {
  @Input() mdrole: role_md;
  @Input() lsstate: lov[];
  @Output() refresh = new EventEmitter();
  public crstate: string;
  public isOn: Boolean = false; // on process state
  constructor(private sv: adminService, private toastr: ToastrService, private ngPopups: NgPopupsService) {
  }

  ngOnInit() {

  }
  ngOnChanges(changes: SimpleChanges) {
  }
  ngAfterViewInit() {
  }

  dataModelChanged() { }

  ngselccmpare(item, selected) { return item.value === selected } //compare selected object with ng-select

  validate() {
    this.ngPopups.confirm('Do you confirm change role permission ?')
      .subscribe(res => {
        if (res) {
          this.isOn = true;
          this.sv.roleupsert(this.mdrole).pipe().subscribe(
            (res) => { this.toastr.success('Save successful'); this.isOn = false; this.refresh.emit(true);},
            (err) => { this.toastr.error(err.error.message); this.isOn = false; },
            () => { }
          );
        }
      });
  }
  remove() {
    this.ngPopups.confirm('Do you confirm Remove role permission ?')
      .subscribe(res => {
        if (res) {
          this.isOn = true;
          this.sv.roleremove(this.mdrole).pipe().subscribe(
            (res) => { 
              this.toastr.success('remove role successful'); 
              this.mdrole.tflow = 'XX';
              this.isOn = false;this.refresh.emit(true); 
            },
            (err) => { this.toastr.error(err.error.message); this.isOn = false; },
            () => { }
          );
        }
      });
  }
  ngLnenable(ln: accn_permision) { 
     ln.isenable = (ln.isenable == 1) ? 0 : 1; 
    }
  ngLnexecute(ln: accn_permision) { ln.isexecute = (ln.isexecute == 1) ? 0: 1; }
}