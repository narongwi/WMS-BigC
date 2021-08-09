import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { admwarehouseService } from '../../services/adm.warehouse.service';
import { pam_inbound } from '../../models/adm.parameter.model';
declare var $: any;
@Component({
  selector: 'app-admparameter',
  templateUrl: 'adm.parameter.html'

})
export class admparameterComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public crtab:number = 1;



  constructor(private sv: admwarehouseService,
              private av: authService, 
              private router: RouterModule,
              private toastr: ToastrService) { 
    this.av.retriveAccess();  
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ /*setTimeout(this.toggle, 1000);*/ }

  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  

}
