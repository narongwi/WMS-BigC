import { Component, OnInit,OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { task_ls, task_md, task_pm } from '../../Models/task.movement.model';
import { taskService } from '../../Services/task.movement.service';
import { taskprocessComponent } from './task.process';

declare var $: any;
@Component({
  selector: 'apptask-movement',
  templateUrl: 'task.movement.html',
  styles: ['.dgmovement { height:calc(100vh - 510px) !important; }',
        '.px-50{width:50px;margin-left:5px;margin-right:5px;}',
        '.px-60{width:60px;margin-left:5px;margin-right:5px;}',
        '.px-70{width:70px;margin-left:5px;margin-right:5px;}',
        '.px-80{width:80px;margin-left:5px;margin-right:5px;}',
        '.px-90{width:90px;margin-left:5px;margin-right:5px;}',
        '.px-100{width:100px;margin-left:5px;margin-right:5px;}',
        '.px-110{width:110px;margin-left:5px;margin-right:5px;}',
        '.px-120{width:120px;margin-left:5px;margin-right:5px;}',
        '.px-130{width:130px;margin-left:5px;margin-right:5px;}',
        '.px-140{width:140px;margin-left:5px;margin-right:5px;}',
        '.row-p-0{display:flex;flex-wrap: wrap;padding-right:10px;padding-left:2px;margin:0;line-height: 2.2}'] ,
  providers: [
    {provide: NgbDateAdapter,         useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}]      

})
export class taskmovementComponent implements OnInit, OnDestroy , AfterViewInit {
    @ViewChild('operate') taskproc:taskprocessComponent;
    public lsstate:lov[] = new Array();
    
    public crtask:task_md = new task_md();
    public lstask:task_ls[] = new Array();
    public pm:task_pm = new task_pm();

    //Task type 
    public lstype:lov[] = new Array();

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

    lstate:lov[] = new Array();
    lstatem:lov[] = new Array();    
    lspriority:lov[] = new Array();
    slcpriority:lov;
    slcstate:lov;
    slctasktype:lov;
    public taskrowselect:number;
    constructor(private sv: taskService,
                private av: authService, 
                private mv: shareService,
                private router: RouterModule,
                private toastr: ToastrService) { 
        this.av.retriveAccess(); 
        this.dateformat = this.av.crProfile.formatdate;
        this.dateformatlong = this.av.crProfile.formatdatelong;
        this.getMaster(); 
        // this.pm.taskdatefrom = new Date().toLocaleDateString();
        this.pm.taskdatefrom = new Date();
    }

    ngOnInit(): void { }

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
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
    decstate(o:string){ return this.lsstate.find(x=>x.value == o).desc; }
    decstateicn(o:string){ return this.lsstate.find(x=>x.value == o).icon; }
    dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }
    getMaster(){ 
        this.lspriority.push({ value : '1', desc : 'Yes', icon : '', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        this.lspriority.push({ value : '0', desc : 'No', icon : '', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});


        Promise.all([
            this.mv.getlov("DATAGRID","ROWLIMIT").toPromise(),
            this.mv.getlov("TASK","FLOW").toPromise(),
            this.mv.lovms("TASK","FLOW").toPromise(),
            this.mv.getlov("TASK","TYPE").toPromise()
          ])
          .then((res) => {
            this.lsrowlmt = res[0].sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString());
            this.lsstate = res[1];
            this.lstatem = res[2];
            this.lstype = res[3];
            this.lsrowlmt = this.mv.getRowlimit();
          });

        // this.mv.getlov("DATAGRID","ROWLIMIT").pipe().subscribe(
        //     (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
        //     (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        //     () => { }
        // );
        // this.mv.getlov("TASK","FLOW").pipe().subscribe(
        //     (res) => { this.lsstate = res; },
        //     (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        //     () => { }
        // );
        // this.mv.lovms("TASK","FLOW").pipe().subscribe(
        //     (res) => { this.lstatem = res; },
        //     (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        //     () => { }
        // );
        // this.mv.getlov("TASK","TYPE").pipe().subscribe(
        // (res) => { this.lstype = res;  },
        // (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        // () => { }
        // );
    }
    fndtask(){ 
        this.pm.tflow = (this.slcstate != null) ? this.slcstate.value : null;
        this.pm.tasktype = (this.slctasktype != null) ? this.slctasktype.value : null;
        this.pm.priority = (this.slcpriority != null) ? this.slcpriority.value : null;
        this.sv.find(this.pm).subscribe(            
          (res) => { this.lstask = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }

    seltask(o:task_ls,ix:number){
        this.taskrowselect = ix;
        this.sv.get(o).subscribe((res) => { 
                this.crtask = res;
                this.taskproc.ngsel(this.crtask);
            },
            (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { } 
        );
    }

    confirmtask(o:string) { 
        try {this.lstask.find(e=>e.taskno == o).tflow  = "ED"; }catch(exc){}        
    }
    cancletask(o){ 
        try {this.lstask.find(e=>e.taskno == o).tflow  = "CL"; }catch(exc){}        
    }

    ngOnDestroy():void {  
        this.lsstate        = null; delete this.lsstate;
        this.crtask         = null; delete this.crtask;
        this.lstask         = null; delete this.lstask;
        this.pm             = null; delete this.pm;
        this.lstype         = null; delete this.lstype;
        this.lssort         = null; delete this.lssort;
        this.lsreverse      = null; delete this.lsreverse;
        this.lsrowlmt       = null; delete this.lsrowlmt;
        this.slrowlmt       = null; delete this.slrowlmt;
        this.page           = null; delete this.page;
        this.pageSize       = null; delete this.pageSize;
        this.dateformat     = null; delete this.dateformat;
        this.dateformatlong = null; delete this.dateformatlong;
        this.datereplan     = null; delete this.datereplan;
        this.crtab          = null; delete this.crtab;
        this.lstate         = null; delete this.lstate;
        this.lstatem        = null; delete this.lstatem;
        this.lspriority     = null; delete this.lspriority;
        this.slcpriority    = null; delete this.slcpriority;
        this.slcstate       = null; delete this.slcstate;
        this.slctasktype    = null; delete this.slctasktype;
    }

}
