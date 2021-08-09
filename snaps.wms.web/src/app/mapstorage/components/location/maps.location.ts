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
import { mapslocationmodify } from './maps.location.modify';

declare var $: any;
@Component({
  selector: 'app-mapslocation',
  templateUrl: 'maps.location.html'

})
export class mapslocationComponent implements OnInit, OnDestroy {
  @ViewChild('modify') modify:mapslocationmodify;
  
  public lslog:lov[] = new Array();
  public slcls:locdw_ls;
  public slcmd:locdw_md = new locdw_md();
  crtab:number = 1;
  crlocation:string;
  constructor(private sv: mapstorageService,
              private av: authService, 
              private router: RouterModule,
              private toastr: ToastrService) { 
    this.av.retriveAccess();  
  }

  ngOnInit(): void { }
  ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
  setupJS() { 
    // sidebar nav scrolling
  }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  
  selln(o:locdw_ls){ 
      this.crlocation = o.lscode;
      this.sv.getlocdw(o).pipe().subscribe(            
          (res) => { 
            this.slcmd = res; 
            this.modify.ngSetup(this.slcmd);
            this.crtab = 2;
           },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
      );
  }
  ngOnDestroy():void { 
    this.lslog = null; delete this.lslog;
    this.slcls = null; delete this.slcls;
    this.slcmd = null; delete this.slcmd;
    this.crtab = null; delete this.crtab;
    this.crlocation = null; delete this.crlocation;
  }

}
