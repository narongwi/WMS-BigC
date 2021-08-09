import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild, EventEmitter, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { stock_info, stock_ls, stock_md } from 'src/app/inventory/models/inv.stock.model';
import { inventoryService } from 'src/app/inventory/services/inv.stock.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { taln_md, task_ls, task_md, task_pm } from '../../Models/task.movement.model';
import { taskService } from '../../Services/task.movement.service';
import { taskhistoryComponent } from '../task.history/task.history';
declare var $: any;
@Component({
  selector: 'apptask-process',
  templateUrl: 'task.process.html'

})
export class taskprocessComponent implements OnInit, OnDestroy {
  @Output() confirmtask = new EventEmitter<string>();
  @Output() cancletaskevnt = new EventEmitter<string>();

  public lslog:lov[] = new Array();
  public crtask:task_md = new task_md();
  public crline:taln_md = new taln_md();
  public reqnew:number = 0;

  public crproduct:stock_ls = new stock_ls();
  public instock:stock_info;
  public crstsock:stock_md;
  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;

  //Toast Ref
  public toastRef:any;
    constructor(private sv: taskService,
                private av: authService,
                private tv: inventoryService,
                private router: RouterModule,
                private toastr: ToastrService,
                private ngPopups: NgPopupsService,) { 
        this.av.retriveAccess();
        this.dateformat = this.av.crProfile.formatdate;
        this.dateformatlong = this.av.crProfile.formatdatelong;
    }

    ngOnInit(): void { }

    ngAfterViewInit(){  }
    setpriority(){}
    dsctasktype(o:string) { return o; }
    public ngsel(o:task_md){ 
      this.crtask = o;
      this.crline = this.crtask.lines[0];
      this.reqnew = 0;
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
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }


    getdoclabel(o:string) { 
      window.open("http://localhost/bgcwmsdocument/get/putaway?task=" + o, "_blank");
    }
    public assignTask(){ 
      this.ngPopups.confirm('Confirm start task movement ?')
      .subscribe(res => {
          if (res) {
            this.crline.accnassign = this.av.crProfile.accncode;
            this.sv.start(this.crtask).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'> Confirm line receipt success </span>",null,{ enableHtml : true });
                this.startTask();
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
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
              (res) => { this.toastr.success("<span class='fn-07e'> Confirm line receipt success </span>",null,{ enableHtml : true });
                this.crtask.tflow = "PT";
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
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
                this.toastr.success("<span class='fn-07e'> Confirm line receipt success </span>",null,{ enableHtml : true });},
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
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
                this.toastr.success("<span class='fn-07e'> Cancel task success </span>",null,{ enableHtml : true });
                this.cancletaskevnt.emit(this.crtask.taskno);
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                     
          } 
      });
    }

    
    public confirmTask(){ 
      this.ngPopups.confirm('Confirm task movement ?')
      .subscribe(res => {
          if (res) {
            this.crline.accnassign = this.av.crProfile.accncode;
            this.crline.targetloc = this.crline.targetadv;
            this.crline.targethuno = this.crline.soucehuno;
            this.sv.confirm(this.crtask).subscribe(            
              (res) => { 
                this.crtask.tflow = "ED";
                this.toastr.success("<span class='fn-07e'> Confirm line receipt success </span>",null,{ enableHtml : true });
                this.confirmtask.emit(this.crtask.taskno);
              },                
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                     
          } 
      });
    }
    getputaway(){     
      this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
        disableTimeOut: true,
        tapToDismiss: false,
        //toastClass: "toast border-red",
        closeButton: false,
        positionClass:'toast-bottom-right',enableHtml : true
      });
  
      this.sv.getlabelputaway(this.crtask.orgcode, this.crtask.site, this.crtask.depot,this.crtask.taskno).subscribe(response => {
        let blob:any = new Blob([response], { type: 'application/pdf'});
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_putaway_" + this.crtask.taskno + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId); 
      }, 
      error => { 
        this.toastr.clear(this.toastRef.ToastId);
      }); 
    }
    getfullpallet(){     
      this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
        disableTimeOut: true,
        tapToDismiss: false,
        //toastClass: "toast border-red",
        closeButton: false,
        positionClass:'toast-bottom-right',enableHtml : true
      });
  
      this.sv.getlabelfullpallet(this.crtask.orgcode, this.crtask.site, this.crtask.depot,this.crtask.taskno).subscribe(response => {
        let blob:any = new Blob([response], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_putaway_" + this.crtask.taskno + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId); 
      }, 
      error => { 
        this.toastr.clear(this.toastRef.ToastId);
      }); 
    }
    getlabel(){     
      this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
        disableTimeOut: true,
        tapToDismiss: false,
        //toastClass: "toast border-red",
        closeButton: false,
        positionClass:'toast-bottom-right',enableHtml : true
      });
  
      this.sv.getlabelputaway(this.crtask.orgcode, this.crtask.site, this.crtask.depot,this.crtask.taskno).subscribe(response => {
        let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        let downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.setAttribute('download', "bgcwms_putaway_" + this.crtask.taskno + ".pdf");
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.toastr.clear(this.toastRef.ToastId); 
      }), 
      error => { 
        this.toastr.clear(this.toastRef.ToastId);
      }  
    }

    ngOnDestroy():void {  
      this.lslog          = null; delete this.lslog;
      this.crtask         = null; delete this.crtask;
      this.crline         = null; delete this.crline;
      this.reqnew         = null; delete this.reqnew;
      this.crproduct      = null; delete this.crproduct;
      this.instock        = null; delete this.instock;
      this.crstsock       = null; delete this.crstsock;
      this.dateformat     = null; delete this.dateformat;
      this.dateformatlong = null; delete this.dateformatlong;
      this.datereplan     = null; delete this.datereplan;
      this.toastRef       = null; delete this.toastRef;
    }
}
