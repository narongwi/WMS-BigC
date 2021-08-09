import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { shareService } from 'src/app/share.service';

declare var $: any;
@Component({
  selector: 'app-mapsaisle',
  templateUrl: 'maps.aisle.html'

})
export class mapsaisleComponent implements OnInit {
  crtab:number = 1;
  constructor(private sv: shareService,
              private av: authService, 
              private router: RouterModule,
              private toastr: ToastrService) { 
    this.av.retriveAccess();  
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
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
