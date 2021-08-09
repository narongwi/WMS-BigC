
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { stock_info, stock_ls, stock_md } from '../../../inventory/models/inv.stock.model';
import { inventoryService } from '../../../inventory/services/inv.stock.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { taln_md, task_ls, task_md, task_pm } from '../../../task/Models/task.movement.model';
import { taskService } from '../../../task/Services/task.movement.service';

declare var $: any;
@Component({
  selector: 'appinv-countmobile',
  templateUrl: 'inv.count.mobile.html'

})
export class invcountmobileComponent implements OnInit {

    public lslog:lov[] = new Array();
    public crtask:task_md = new task_md();
    public crline:taln_md = new taln_md();
    public reqnew:number = 0;

    public crproduct:stock_ls = new stock_ls();
    public instock:stock_info;
    public crstsock:stock_md;

    public crstate:number = 1;

    constructor(private sv: taskService,
                private av: authService,
                private tv: inventoryService,
                private router: RouterModule,
                private toastr: ToastrService,
                private ngPopups: NgPopupsService,) { 
        this.av.retriveAccess();
        this.crstate = 1;
    }

    ngOnInit(): void { }
    ngOnDestroy():void {  }
    ngAfterViewInit(){  }
    setpriority(){}
    dsctasktype(o:string) { return o; }
    public ngsel(o:task_md){ 
      this.crtask = o;
      this.crline = this.crtask.lines[0];
      this.reqnew = 0;
    }

    setstep(o:number) {
        this.crstate = o;
    }

    reqnewtask() {
      this.reqnew = 1;
    }
    slcstock(o:stock_md){
      this.crstsock = o;
    }
    getinfo(o:stock_ls){

      this.tv.getstockInfo(this.crproduct).subscribe(            
        (res) => { 
          this.instock = res; this.instock.lines
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }


    getdoclabel(o:string) { 
      window.open("http://localhost/bgcwmsdocument/get/putaway?task=" + o, "_blank");
    }


    scanlocation(){ }

    public assignTask(){ 
      this.ngPopups.confirm('Confirm start task movement ?')
      .subscribe(res => {
          if (res) {
            this.crline.accnassign = this.av.crProfile.accncode;
            this.sv.start(this.crtask).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-1e15'> Confirm line receipt success </span>",null,{ enableHtml : true });
                this.startTask();
              },
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                     
          } 
      });
    }

    public startTask(){ 
      // this.ngPopups.confirm('Confirm start task movement ?')
      // .subscribe(res => {
      //     if (res) {
            this.crline.accnwork = this.av.crProfile.accncode;            
            this.sv.start(this.crtask).subscribe(            
              (res) => { this.toastr.success("<span class='fn-1e15'> Confirm line receipt success </span>",null,{ enableHtml : true });
                this.crtask.tflow = "PT";
              },
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                     
      //     } 
      // });
    }
    public fillTask(){ 
      this.ngPopups.confirm('Confirm finish task movement ?')
      .subscribe(res => {
          if (res) {
            this.crline.accnassign = this.av.crProfile.accncode;
            this.crline.targetloc = this.crline.targetadv;
            this.crline.targethuno = this.crline.soucehuno;
            this.crline.accnfill = this.av.crProfile.accncode;   
            this.sv.fill(this.crtask).subscribe(            
              (res) => { 
                this.crtask.tflow = "ED";
                this.toastr.success("<span class='fn-1e15'> Confirm line receipt success </span>",null,{ enableHtml : true });},
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                     
          } 
      });
    }

    public gentask() { 
      this.ngPopups.confirm('Confirm generate task movement ?')
      .subscribe(res => {
          if (res) {
                    
          } 
      });
    }
    public cancelTask(){ 
      this.ngPopups.confirm('Confirm cancel task movement ?')
      .subscribe(res => {
          if (res) {
            this.crline.accnassign = this.av.crProfile.accncode;
            this.crline.targetloc = this.crline.targetadv;
            this.crline.targethuno = this.crline.soucehuno;
            this.sv.cancel(this.crtask).subscribe(            
              (res) => { 
                this.crtask.tflow = "CL";
                this.toastr.success("<span class='fn-1e15'> Confirm line receipt success </span>",null,{ enableHtml : true });},
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                     
          } 
      });
    }

}
