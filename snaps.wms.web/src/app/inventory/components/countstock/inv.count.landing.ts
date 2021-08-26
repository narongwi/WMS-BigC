import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { stock_md, stock_ls, stock_pm, stock_info } from '../../models/inv.stock.model';
import { inventoryService } from '../../services/inv.stock.service';
import { correction_md } from '../../models/inv.correction.mode';
import { invcounplanComponent } from './inv.count.plan';
import { invcountlineComponent } from './inv.count.line';
import { countplan_md, counttask_md } from '../../models/inv.count.model';
import { taskhistoryComponent } from 'src/app/task/Components/task.history/task.history';
import { InvcountConfirmComponent } from './inv.count.confirm';
import { invcountaskComponent } from './inv.count.task';
declare var $: any;
@Component({
  selector: 'appinv-count',
  templateUrl: 'inv.count.landing.html',
  styles: ['.dgproduct { height:200px !important; } ', '.dgstockline { height:calc(100vh - 470px) !important; } ', '.dgcorrect { height:calc(100vh - 195px) !important; } '],

})
export class invcountComponent implements OnInit, OnDestroy {
  // task
  @ViewChild('obtask') obtask: invcountaskComponent;
  // plan
  @ViewChild('obplan') obplan: invcounplanComponent;
  // line
  @ViewChild('obline') obline: invcountlineComponent;
  // line
  @ViewChild('obconf') obconf: InvcountConfirmComponent;

  //Date format
  public dateformat: string;
  public dateformatlong: string;
  public datereplan: Date | string | null;

  //Tab
  public crtab: number = 1;

  public crplan: string = "";
  public crrtask: counttask_md;
  constructor(private av: authService,
    private mv: adminService,
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService,) {
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void { }
  ngAfterViewInit() {

  }
  feedCount(evt) {
    console.log("Feed Count Line Tab ");
    //evt.activeId Id of the currently active nav.
    //evt.nextId Id of the newly selected nav.
    //evt.preventDefault Id of the currently active nav.
    //https://ng-bootstrap.github.io/#/components/nav/api#NgbNavChangeEvent
    switch (evt.nextId) {
      case 1:
        this.obtask.ngFind();
        break;
      case 2:
        if (this.crrtask.countcode) {
          this.obplan.ngSelc(this.crrtask);
        }
        break;
      case 3:
        if (this.crrtask.countcode) {
          this.obline.ngSelc(this.crrtask);
        }
        break;
      case 4:
        if (this.crrtask.countcode) {
          this.obconf.ngSelc(this.crrtask);
        }
        break;
      default:
        break;
    }
  }
  // select task
  ngSelc(o: counttask_md) {
    this.crplan = o.countcode
    // current count task
    this.crrtask = o;

    // tab plan count
    this.obplan.ngSelc(o);

    // tab input count
    //this.obline.ngSelc(o);

    //tab confirm stock take
    //this.obconf.ngSelc(o);
  }

  ngLsLine(o: countplan_md) {
    this.obline.ngSelc(this.crrtask);
  }

  ngRefTask(o) {
    // counttask_md
    // tab task count
    this.obtask.ngFind();
  }

  ngOnDestroy(): void {
    this.obplan = null; delete this.obplan;
    this.obline = null; delete this.obline;
    this.dateformat = null; delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.crtab = null; delete this.crtab;
    this.crplan = null; delete this.crplan;
  }

}