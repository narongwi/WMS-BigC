import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { depot_ls } from '../../../admn/models/adm.depot.model';
import { role_ls, role_md, role_pm } from '../../../admn/models/role.model';
import { warehouse_ls } from '../../../admn/models/adm.warehouse.model';
import { mapstorageService } from '../../services/app-mapstorage.service';
import { locdw_ls, locdw_md } from '../../Models/mdl-mapstorage';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { mapsgridmodify } from './maps.grid.modify';

declare var $: any;
@Component({
  selector: 'app-mapsgrid',
  templateUrl: 'maps.grid.html',
  styles: ['.dglocation { height:calc(100vh - 190px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class mapsgridComponent implements OnInit {
  @ViewChild('modify') modify:mapsgridmodify;
  public lslog:lov[] = new Array();
  public crtab:number = 1;
  public slcls:locdw_ls;
  public slcmd:locdw_md = new locdw_md();
  public crgrid:string;
  constructor(private sv: mapstorageService,
              private av: authService, 
              private router: RouterModule,
              private toastr: ToastrService) { 
    this.av.retriveAccess();  
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
  setupJS() { 
    // sidebar nav scrolling
  }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  
  selln(o:locdw_ls){ 
    this.crgrid = o.lscode;
    this.sv.getlocdw(o).pipe().subscribe(            
        (res) => { this.slcmd = res;  this.crtab = 2; }
    );
  }
}
