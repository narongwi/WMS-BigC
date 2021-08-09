import { HttpClientXsrfModule } from '@angular/common/http';
import { Component, ElementRef, HostBinding, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { accn_category, accn_permision, accn_profile, accn_roleacs } from 'src/app/admn/models/account.model';
import { authService } from 'src/app/auth/services/auth.service';
declare var $: any;

@Component({
  selector: 'app-navigate',
  templateUrl: './navigate.component.html',
  styleUrls: ['./navigate.component.scss']
})


export class NavigateComponent implements OnInit {
  public crprofile:accn_profile = new accn_profile();
  public crrole:accn_roleacs = new accn_roleacs();
  public crmodule:accn_category[] = [];
  constructor(private sv: authService,private router: Router) { 
    this.sv.getProfile();
    //this.sv.retriveAccess(); 
    this.crprofile = this.sv.crProfile;
    this.crrole = this.crprofile.roleaccs;
    this.crrole.modules = this.crprofile.roleaccs.modules.filter(p=>p.permission.filter(x=>x.isenable==1).length > 0);

    // permission filter
    for (let index = 0; index < this.crrole.modules.length; index++) {
      const elmodule = this.crrole.modules[index];
      this.crrole.modules[index].permission = elmodule.permission.filter(x=>x.isenable==1);
    }
  }
  ngOnInit(): void {
    
    
  }
  ngOnDestroy():void {  }
  ngAfterViewInit() {
	  this.setMenu();
  }
  setMenu() { 
    $('#main-menu').metisMenu();
  
    // sidebar nav scrolling
    $('#left-sidebar .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });
  
    // toggle fullwidth layout
    // $('.btn-toggle-fullwidth').on('click', function() {
    //   if(!$('body').hasClass('layout-fullwidth')) {
    //     $('body').addClass('layout-fullwidth');
    //     $(this).find(".fa").toggleClass('fa-angle-left fa-angle-right');
  
    //     $(this).animate({
    //       left: "+=28px"
    //     }, 800);
  
  
    //   } else {
    //     $('body').removeClass('layout-fullwidth');
    //     $(this).find(".fa").toggleClass('fa-angle-left fa-angle-right');
  
    //     $(this).animate({
    //       left: "-=28px"
    //     }, 800);
    //   }
    // }, {passive: false});
  }
    

  singout() { 
    this.sv.signout();
    this.router.navigate(['/signin']);
  }
  
}
