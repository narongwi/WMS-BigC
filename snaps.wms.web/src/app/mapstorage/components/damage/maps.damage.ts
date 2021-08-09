import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { mapstorageService } from '../../services/app-mapstorage.service';

declare var $: any;
@Component({
  selector: 'app-mapsdamage',
  templateUrl: 'maps.damage.html'

})
export class mapsdamageComponent implements OnInit {
  public lslog:lov[] = new Array();
  public crtab:number = 1;
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
