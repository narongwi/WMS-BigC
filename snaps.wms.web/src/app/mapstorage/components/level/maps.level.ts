import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { depot_ls } from '../../../admn/models/adm.depot.model';
import { role_ls, role_md, role_pm } from '../../../admn/models/role.model';
import { warehouse_ls } from '../../../admn/models/adm.warehouse.model';
import { mapstorageService } from '../../services/app-mapstorage.service';

declare var $: any;
@Component({
  selector: 'app-mapslevel',
  templateUrl: 'maps.level.html'

})
export class mapslevelComponent implements OnInit {
  public lslog:lov[] = new Array();
  crtab:number = 1;
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
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });   
  }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }

}
