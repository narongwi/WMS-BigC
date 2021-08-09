import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { adminService } from 'src/app/admn/services/account.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { task_ls, task_pm } from '../../Models/task.movement.model';
import { taskService } from '../../Services/task.movement.service';
declare var $: any;
@Component({
  selector: 'apptask-history',
  templateUrl: 'task.history.html',
  styles: ['.dgmovement { height:calc(100vh - 240px) !important; }'] ,
  providers: [
    {provide: NgbDateAdapter,         useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}]     
})
export class taskhistoryComponent implements OnInit, OnDestroy {

    public pm:task_pm = new task_pm();

    //Task List
    public lstask:task_ls[] = new Array();
    public lsstate:lov[] = new Array();
    //Task type 
    public lstype:lov[] = new Array();
    public slctasktype:lov = new lov();

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
    //Tab
    crtab:number = 1;
    
    constructor(private sv: taskService,
        private av: authService, 
        private mv: adminService,
        private router: RouterModule,
        private toastr: ToastrService) { 
        this.av.retriveAccess();  
        this.av.retriveAccess(); 
        this.dateformat = this.av.crProfile.formatdate;
        this.dateformatlong = this.av.crProfile.formatdatelong;
    }

    ngOnInit(): void { this.getMaster(); }

    ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/ this.fndtask(); }
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
    SortOrder(value: string) { if (this.lssort === value) { this.lsreverse = !this.lsreverse; } this.lssort = value; }
    dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }
    decstateicn(o:string){ return this.lsstate.find(x=>x.value == o).icon; }
    getMaster(){ 
        this.mv.getlov("DATAGRID","ROWLIMIT").pipe().subscribe(
            (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
        );
        this.mv.getlov("TASK","FLOW").pipe().subscribe(
            (res) => { this.lsstate = res; },
            (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
        );
        this.mv.getlov("TASK","TYPE").pipe().subscribe(
        (res) => { this.lstype = res;  },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
        );
    }
    fndtask(){ 
        this.sv.find(this.pm).subscribe(            
          (res) => { this.lstask = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }
    ngOnDestroy():void { 
        this.pm             = null; delete this.pm;
        this.lstask         = null; delete this.lstask;
        this.lsstate        = null; delete this.lsstate;
        this.lstype         = null; delete this.lstype;
        this.slctasktype    = null; delete this.slctasktype;
        this.lssort         = null; delete this.lssort;
        this.lsreverse      = null; delete this.lsreverse;
        this.lsrowlmt       = null; delete this.lsrowlmt;
        this.slrowlmt       = null; delete this.slrowlmt;
        this.page           = null; delete this.page;
        this.pageSize       = null; delete this.pageSize;
        this.dateformat     = null; delete this.dateformat;
        this.dateformatlong = null; delete this.dateformatlong;
        this.datereplan     = null; delete this.datereplan;
        this.crtab          = null; delete this.crtab ;
    }

}
