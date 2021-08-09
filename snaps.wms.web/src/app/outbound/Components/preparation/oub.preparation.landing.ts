import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { ɵangular_packages_platform_browser_platform_browser_d } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
declare var $: any;
@Component({
  selector: 'appoub-preparation',
  templateUrl: 'oub.preparation.landing.html'

})
export class oubpreparationComponent implements OnInit {

    //Tab
    crtab:number = 1;
    constructor(private av: authService, 
                private mv: adminService,
                private router: RouterModule,
                private toastr: ToastrService,                
                private ngPopups: NgPopupsService,) { 

    }

  ngOnInit(): void { }
  ngOnDestroy(): void { } 
  ngAfterViewInit() { this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
  toggle() { $('.snapsmenu').click(); }
  setupJS() { $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' }); }
}
