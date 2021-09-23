import { ThrowStmt } from '@angular/compiler';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { authService } from 'src/app/auth/services/auth.service';
import { adminService } from '../../services/account.service';
import { warehouse_ls } from '../../models/adm.warehouse.model';
import { depot_ls } from '../../models/adm.depot.model';
import {
  accn_cfg,
  accn_ls,
  accn_md,
  accn_pm,
} from '../../models/account.model';
import { lov } from '../../../helpers/lov';
import { ToastrService } from 'ngx-toastr';
import { shareService } from 'src/app/share.service';
import { NgPopupsService } from 'ng-popups';

declare var $: any;
@Component({
  selector: 'app-accn',
  templateUrl: './accn.component.html',
  styles: [
    '.dgaccn { height:calc(100vh - 240px) !important;  } ',
    '.dglines { height:calc(100vh - 685px) !important; }',
  ],
})
export class AccnComponent implements OnInit, OnDestroy {
  public lswarehouse: lov[] = [];
  public lsdepot: depot_ls[] = new Array();

  public lsaccn: accn_ls[] = new Array();
  public pmaccn: accn_pm = new accn_pm();
  public mdaccn: accn_md = new accn_md();

  public slcrole: lov;
  public slcwarehouse: lov;

  public lsrole: lov[] = [];
  public lsacnstate: lov[] = new Array();
  public lstype: lov[] = new Array();
  public slcstate: lov;

  crtab: number = 1;

  //PageNavigate
  public page = 4;
  public pageSize = 200;
  public slrowlmt: lov;
  public lsrowlmt: lov[] = new Array();
  //Sorting
  public lssort: string = 'spcarea';
  public lsreverse: boolean = false; // for sorting
  //Date format
  public formatdate: string;
  public formatdatelong: string;
  public accselect: number;
  public cfgmd: accn_cfg = new accn_cfg();
  public cacls: accn_ls = new accn_ls();
  public roleselect: number;
  constructor(
    private sv: adminService,
    private av: authService,
    private ss: shareService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService
  ) {
    this.ss.ngSetup();
    this.av.retriveAccess();
  }

  ngOnInit(): void {}
  ngAfterViewInit() {
    this.setupJS();
    /*setTimeout(this.toggle, 1000);*/ this.ngSetup();
    this.fndaccn();
  }
  setupJS() {
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece',
    });
  }

  getIcon(o: string) {}
  fndaccn() {
    this.pmaccn.tflow = this.slcstate == null ? '' : this.slcstate.value;
    this.sv
      .accnFind(this.pmaccn)
      .pipe()
      .subscribe(
        (res) => {
          this.lsaccn = res;
        },
        (err) => {},
        () => {}
      );
  }
  accnget(o: accn_ls, ix: number) {
    this.cacls = o;
    this.accselect = ix;
    this.sv
      .accnMod(o)
      .pipe()
      .subscribe(
        (res) => {
          this.mdaccn = res;
        },
        (err) => {
          this.toastr.error(err.error.message);
        },
        () => {}
      );
  }
  refresh(e: any) {
    this.fndaccn();
  }
  //toggle(){ $('.snapsmenu').click();  }
  ngDecIcon(o: string) {
    try {
      return this.lsacnstate.find((x) => x.value == o).icon;
    } catch (exp) {
      return o;
    }
  }
  ngDecState(o: string) {
    try {
      return this.lsacnstate.find((e) => e.value == o).desc;
    } catch (exp) {
      return o;
    }
  }
  ngChangeRowlmt() {
    this.pageSize = parseInt(this.slrowlmt.value);
  } /* Row limit */
  ngSelcompare(item, selected) {
    return item.value === selected;
  } /* ngSelect compare object */
  ngSetup() {
    this.sv.getlov('ACCOUNT', 'FLOW').subscribe((res) => {
      this.lsacnstate = res;
    });
    this.sv.getlov('ACCOUNT', 'TYPE').subscribe((res) => {
      this.lstype = res;
    });
    // this.sv.getlovrole().pipe().subscribe( (res) => { this.lsrole = res;  });
    this.sv.getWarehouse().subscribe((res) => {
      this.lswarehouse = res;
    });
    this.lsrowlmt = this.ss.getRowlimit();
  }

  selWarehouse() {
    this.sv
      .sellovrole(this.slcwarehouse.value, this.slcwarehouse.valopnfirst)
      .pipe()
      .subscribe((res) => {
        // re get role
        this.lsrole = res;
        if (this.lsrole.length > 0) {
          this.slcrole = this.lsrole[0];
        }
      });
  }
  selRole(cfg: accn_cfg, ix: number) {
    this.roleselect = ix;
    this.cfgmd = cfg;
    this.slcwarehouse = this.lswarehouse.find((e) => e.value == cfg.site);

    this.sv
      .sellovrole(this.slcwarehouse.value, this.slcwarehouse.valopnfirst)
      .pipe()
      .subscribe((res) => {
        // re get role
        this.lsrole = res;
        // set dropdown
        this.slcrole = this.lsrole.find((e) => e.value == cfg.rolecode);
      });
  }
  addCfg() {
    this.ngPopups
      .confirm('Do you confirm to config role ' + this.slcrole.desc + ' ?')
      .subscribe((res) => {
        if (res) {
          this.cfgmd.orgcode = this.mdaccn.orgcode;
          this.cfgmd.site = this.slcwarehouse.value;
          this.cfgmd.depot = this.slcwarehouse.valopnfirst;
          this.cfgmd.accncode = this.mdaccn.accncode;
          this.cfgmd.rolecode = this.slcrole.value;
          this.cfgmd.rolename = this.slcrole.desc;
          this.sv
            .addCfg(this.cfgmd)
            .pipe()
            .subscribe((res) => {
              this.toastr.success(
                "<span class='fn-07e'>Confirm Successfully</span>",
                null,
                { enableHtml: true }
              );
              this.accnget(this.cacls, this.roleselect);
            });
        }
      });
  }
  delCfg() {
    if (
      this.mdaccn.accncfg.filter(
        (x) =>
          x.site == this.slcwarehouse.value && x.rolecode == this.slcrole.value
      ).length == 0
    ) {
      this.toastr.error(
        "<span class='fn-07e'>please selected role config</span>",
        null,
        { enableHtml: true }
      );
    } else {
      this.ngPopups
        .confirm('Do you confirm to Delete' + this.slcrole.desc + ' ?')
        .subscribe((res) => {
          if (res) {
            this.sv
              .delCfg(this.cfgmd)
              .pipe()
              .subscribe((res) => {
                this.toastr.success(
                  "<span class='fn-07e'>Deleted Successfully</span>",
                  null,
                  { enableHtml: true }
                );
                this.accnget(this.cacls, this.roleselect);
              });
          }
        });
    }
  }

  ngOnDestroy() {}
}
