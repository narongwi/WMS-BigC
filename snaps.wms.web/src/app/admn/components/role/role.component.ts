import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from 'src/app/auth/services/auth.service';
import { lov } from 'src/app/helpers/lov';
import { depot_ls } from '../../models/adm.depot.model';
import { role_ls, role_md, role_pm } from '../../models/role.model';
import { warehouse_ls } from '../../models/adm.warehouse.model';
import { adminService } from '../../services/account.service';
import { oprrole } from './role-opr';
declare var $: any;
@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss'],
  styles: ['.dgaccn { height:calc(100vh - 240px) !important;  } ', '.dglines { height:calc(100vh - 200px) !important; }'],
})
export class RoleComponent implements OnInit {
  public lswarehouse: lov[] = [];
  public lsdepot: depot_ls[] = new Array();

  public lsrole: role_ls[] = new Array();
  public pmrole: role_pm = new role_pm();
  public mdrole: role_md;

  public lsstate: lov[] = new Array();
  public slcstate: lov;
  public slcwarehouse:lov;

  crtab: number = 1;
  constructor(private sv: adminService,
    private av: authService,
    private router: RouterModule,
    private toastr: ToastrService) {
    this.av.retriveAccess(); this.pmrole.orgcode = "bgc"; this.getstate();
  }

  ngOnInit(): void { }
  ngOnDestroy(): void { }
  ngAfterViewInit() { 
    this.setupJS();
    //  
     this.sv.getWarehouse().subscribe( (res) => { 
       this.lswarehouse = res;
       this.fndrole();
      });
      /*setTimeout(this.toggle, 1000);*/ 
    }

    
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });
  }
  getIcon(o: string) { return this.lsstate.find(x => x.value == o).icon; }
  toggle() { $('.snapsmenu').click(); }

  getstate() {
    this.sv.getlov("ALL", "FLOW").subscribe((res) => { this.lsstate = res; });
  }
  fndrole() {
    if(this.slcwarehouse == null) return;
    this.pmrole.site = this.slcwarehouse.value;
    this.pmrole.depot = this.slcwarehouse.valopnfirst;
    this.sv.rolefind(this.pmrole).pipe().subscribe((res) => { this.lsrole = res; });
  }
  ngSelcompare(item, selected) { return item.value === selected } /* ngSelect compare object */

  refresh(e:any){
    this.fndrole();
  }

  getrole(o: role_ls,ix:number) {
    this.sv.roleget(o).pipe().subscribe((res) => { 
      this.mdrole = res; 
    });
  }
  newrole() {
    this.sv.rolemaster(this.pmrole.site,this.pmrole.depot).pipe().subscribe((res) => {
      this.mdrole = res;
      this.mdrole.tflow = 'NW';
      this.mdrole.rolecode = "";
      this.mdrole.rolename = "";
    });
  }



}
