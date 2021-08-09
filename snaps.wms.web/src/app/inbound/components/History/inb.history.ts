import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { adminService } from 'src/app/admn/services/account.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { inbound_hs, inbound_md, inbound_pm } from '../../models/mdl-inbound';
import { inboundService } from '../../services/app-inbound.service';
declare var $: any;
@Component({
  selector: 'appinb-history',
  templateUrl: 'inb.history.html',
  styles: ['.dghistory { height:calc(100vh - 245px) !important;','.dglines { height:335px !important; } .dginfo { height:365px !important; }'] ,
  providers: [
    {provide: NgbDateAdapter,         useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}]      
})
export class inbhistoryComponent implements OnInit {

    public lslog:lov[] = new Array();
    public pm:inbound_pm = new inbound_pm();
    public crhistory:inbound_hs[] = new Array();
    public crtab:number = 1;
    //Sorting 
    public lssort:string = "spcarea";
    public lsreverse: boolean = false; // for sorting
    //PageNavigate
    lsrowlmt:lov[] = new Array();
    slrowlmt:lov;
    page = 4;
    pageSize = 200;
    //Date format
    public dateformat:string;
    public dateformatlong:string;
    public datereplan: Date | string | null;

    constructor(private sv: inboundService,
                private mv: adminService,
                private av: authService, 
                private router: RouterModule,
                private toastr: ToastrService) { 
        this.av.retriveAccess(); 
        this.dateformat = this.av.crProfile.formatdate;
        this.dateformatlong = this.av.crProfile.formatdatelong;
    }

    ngOnInit(): void { }
   
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

    gethistory(){
        this.sv.gethistory(this.pm).subscribe(            
          (res) => { this.crhistory = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );    
        
        this.mv.getlov("DATAGRID","ROWLIMIT").pipe().subscribe(
          (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
          (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
    }

    ngOnDestroy():void { 
      this.lslog          = null; delete this.lslog;         
      this.pm             = null; delete this.pm ;           
      this.crhistory      = null; delete this.crhistory ;    
      this.crtab          = null; delete this.crtab;        
      this.lssort         = null; delete this.lssort;        
      this.lsreverse      = null; delete this.lsreverse ;    
      this.lsrowlmt       = null; delete this.lsrowlmt;      
      this.slrowlmt       = null; delete this.slrowlmt ;     
      this.page           = null; delete this.page ;         
      this.pageSize       = null; delete this.pageSize;      
      this.dateformat     = null; delete this.dateformat ;   
      this.dateformatlong = null; delete this.dateformatlong;
      this.datereplan	   = null; delete this.datereplan	;  
     }
}
