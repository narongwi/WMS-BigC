import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbPaginationConfig, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { binary_md } from '../../models/adm.binary.model';
import { warehouse_md, warehouse_pm } from '../../models/adm.warehouse.model';
import { binaryService } from '../../services/adm.binary.service';
import { admwarehouseService } from '../../services/adm.warehouse.service';

declare var $: any;
@Component({
  selector: 'app-admwarehouse',
  templateUrl: 'adm.warehouse.html',
  styles: ['.dgline { height:calc(100vh - 145px) !important;','.dgvalue { height:200px !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class admwarehouseComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public crtab:number = 1;

  lsrowlmt:lov[] = new Array();       //List of limit row
  pm:warehouse_pm = new warehouse_pm();     //Parameter 
  lstdesc:binary_md[] = new Array();  //List of Description
  lststate:lov[] = new Array();       //List of state
  lswarehouse:warehouse_md[] = new Array();
  lines:binary_md[] = new Array();    //List of Configuration
  slc:binary_md = new binary_md();    //Selection of binary
  cr:warehouse_md = new warehouse_md();     //Object of binary 
  crstate:boolean = false;

  //Date format
  public dateformat:string;
  public dateformatlong:string;

  constructor(private sv: shareService,
              private av: authService,
              private bv: binaryService,
              private hv: admwarehouseService,
              private toastr: ToastrService,
              private ngPopups: NgPopupsService,) { 
      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ this.crstate = true; this.ngWarehouse(); }

  ngGetline(o:binary_md){
    this.slc = o;
    this.bv.list(o).pipe().subscribe(
      (res) => { this.lines = res; },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { }
    );
  }
  ngSelc(o:warehouse_md){ this.cr = o; this.crstate = (o.tflow == "IO") ? true : false; }
  ngWarehouse() { this.hv.find(this.pm).pipe().subscribe((res) => { this.lswarehouse = res; }); }
  ngNew() { 
    this.cr = new warehouse_md();
    this.cr.orgcode = this.slc.orgcode;
    this.cr.tflow = "NW";
    this.crstate = true;
    this.toastr.info("<span class='fn-08e'>Ready to config new warehouse</span>",null,{ enableHtml : true });
  }
  ngUpsert(){ 
    this.ngPopups.confirm(`Do you confirm `+(this.cr.tflow == "NW") ? `create` : `change` + ` warehouse ?`)
    .subscribe(res => { 
      if (res) {
        this.cr.tflow = (this.cr.tflow == "NW")  ? "NW" : (this.crstate == true) ? "IO" : "XX";
        this.hv.upsert(this.cr).pipe().subscribe(
          (res) => { this.toastr.success("<span class='fn-07e'>Modify warehouse success</span>",null,{ enableHtml : true }); this.ngNew(); this.ngWarehouse(); }
        );
      }
    });
  }
  ngRemove() { 
    this.ngPopups.confirm(` Do you confirm to remove warehouse ? `)
    .subscribe(res => { 
      if (res) {
        this.cr.tflow = (this.cr.tflow == "NW")  ? "NW" : (this.crstate == true) ? "IO" : "XX";
        this.hv.remove(this.cr).pipe().subscribe(
          (res) => { this.toastr.success("<span class='fn-07e'>Remove warehouse success</span>",null,{ enableHtml : true }); this.ngNew();  this.ngWarehouse(); }
        );
      }
    });
  }
  getIcon(o:string){ return this.sv.ngDecIcon(o); }
  getDesc(o:string){ return this.sv.ngDecState(o); }
  //toggle(){ $('.snapsmenu').click();  }

}
