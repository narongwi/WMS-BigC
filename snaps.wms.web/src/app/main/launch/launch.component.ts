import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { authService } from 'src/app/auth/services/auth.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { accn_profile } from 'src/app/admn/models/account.model';
import { NgProgress,NgProgressRef } from 'ngx-progressbar';
declare var $: any;

@Component({
  selector: 'app-launch',
  templateUrl: './launch.component.html',
  styleUrls: ['./launch.component.scss']
})
export class LaunchComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('backdrop') backdrop: ElementRef;

  public isbackdrop: boolean = false;
  public crprofile: accn_profile = new accn_profile();
  constructor(private sv: authService, private router: Router) {

  }

  onStarted() {
    this.isbackdrop = true;
    console.log("loadding is started");
  }

  onCompleted() {
    console.log("loadding is Completed!");

    setTimeout(() => {
    this.isbackdrop = false;
    }, 200);
  }
  ngOnInit(): void {
    // Progress bar events (optional)
    this.crprofile = this.sv.crProfile;
    // console.log(this.crprofile);
  }
  ngOnDestroy(): void {
    this.isbackdrop = false;
  }
  ngAfterViewInit() {
    this.isbackdrop = false;
    
    //this.setMenu();


    setTimeout(() => {
      this.toggleSidebar();
    }, 800);
  }
  public toggleSidebar() {
    var toggleClassName = "layout-fullwidth";
    var bodyElement = document.querySelectorAll("body")[0];
    if (bodyElement.classList.contains(toggleClassName)) {
      bodyElement.classList.remove(toggleClassName);
    } else {
      bodyElement.classList.add(toggleClassName);
    }
  }

  //   setMenu() { 
  // 

  // 	// sidebar nav scrolling
  // 	$('#left-sidebar .sidebar-scroll').slimScroll({
  // 		height: '95%',
  // 		wheelStep: 5,
  // 		touchScrollStep: 50,
  // 		color: '#cecece'
  // 	});

  // 	// toggle fullwidth layout
  // 	$('.btn-toggle-fullwidth').on('click', function() {
  // 		if(!$('body').hasClass('layout-fullwidth')) {
  // 			$('body').addClass('layout-fullwidth');
  // 			$(this).find(".fa").toggleClass('fa-angle-left fa-angle-right');

  // 			$(this).animate({
  // 				left: "+=28px"
  // 			}, 800);


  // 		} else {
  // 			$('body').removeClass('layout-fullwidth');
  // 			$(this).find(".fa").toggleClass('fa-angle-left fa-angle-right');

  // 			$(this).animate({
  // 				left: "-=28px"
  // 			}, 800);
  // 		}
  // 	});
  //   }

  //   singout() { 
  // 	  this.sv.signout();
  // 	  this.router.navigate(['/signin']);
  //   }

  //   setprofile() { 
  // 	if (!!localStorage.getItem('snapsacnwms')) { 
  // 		this.crprofile = this.sv.getprofile();				
  // 	}else { 
  // 		this.callProfile();		
  // 	}
  //   }
  //   callProfile() {
  //     try { 
  // 		this.sv.getAccess();
  // 		this.sv.retriveProfile("").pipe().subscribe(
  // 			orsl => { this.crprofile =  this.sv.getProfile;  this.setMenu(); } ,
  // 			err => { console.log(err); }
  // 		  );
  // 	  }catch (exx){ 
  // 		console.log(exx);
  // 	  }
  //   }
}
