import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { __assign } from 'tslib';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { task_pm } from '../../../task/Models/task.movement.model';
import { handerlingunit, handerlingunit_item } from '../../Models/oub.handlingunit.model';
import { route_hu, route_ls, route_md, route_pm, route_thsum } from '../../Models/oub.route.model';
import { ouhanderlingunitService } from '../../Services/oub.handerlingunit.service';
import { ourouteService } from '../../Services/oub.route.service';
import { outboundService } from '../../Services/oub.service';

declare var $: any;
@Component({
  selector: 'appoub-delivery',
  templateUrl: 'oub.delivery.html',
  styles: ['.dgroute { height:calc(100vh - 205px) !important; }',
           '.dglines { height:calc(100vh - 465px) !important; }',
           '.dgoperate { height:calc(100vh - 222px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class oubdeliveryComponent implements OnInit, OnDestroy {
  public lsstate:lov[] = new Array();
  public lstype:lov[] = new Array();
  public instocksum:number = 0;
  public rqedit:number = 0;
  public crstate:boolean = false;

  public lsroute:route_ls[] = new Array();
  public pmroute: route_pm = new route_pm();

  public lsafter: lov[] = new Array();
  public lsroutetype: lov[] = new Array();
  public lsstaging: lov[] = new Array();
  public lsthparty: lov[] = new Array();
  public lstransportor: lov[] = new Array();
  public lsloadtype: lov[] = new Array();
  public lspayment: lov[] = new Array();
  public lstrucktype: lov[] = new Array();
  public lstrtmode: lov[] = new Array();

  public lsthsum:route_thsum[] = new Array();

  public slchu:handerlingunit = new handerlingunit();
  public slchuline:handerlingunit_item[] = new Array();

  public routesource:route_md = new route_md();

  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;

  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting

  //PageNavigate
  page = 4;
  pageSize = 200;
  slrowlmt:lov;
  lsrowlmt:lov[] = new Array();

  lsunit:lov[] = new Array(); //unit list

  //Tab
  crtab:number = 1;

  //Ready to ship 
  readytoship:number = 0;
  toastRef:any;

  //selection route 
  crroute:route_ls = new route_ls();

  storerowselect:number;
  routerowselect:number;
  hunorowselect:number;

  constructor(private sv: ourouteService,
              private hv: ouhanderlingunitService,
              private av: authService, 
              private mv: shareService,
              private router: RouterModule,
              private toastr: ToastrService,                
              private ngPopups: NgPopupsService,) {
    this.mv.ngSetup();
    this.av.retriveAccess(); 
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.pmroute.iscombine = "0";
    this.lsafter.push({ value: 'N', desc: "No need", icon: '', valopnfirst: '', valopnsecond : '', valopnthird :'', valopnfour:''});
    this.lsafter.push({ value: 'M', desc: "Waiting for Delivery", icon: '', valopnfirst: '', valopnsecond : '', valopnthird :'', valopnfour:''});
  }

  ngOnInit(): void { }
  ngAfterViewInit() { this.setupJS(); /*setTimeout(this.toggle, 1000);*/ this.getstate(); this.gettype(); this.getMaster(); this.fndthsum(); }
  setupJS() { 
        // sidebar nav scrolling
        $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' });   
    }
    getIcon(o:string){ return "";  }
    //toggle(){ $('.snapsmenu').click();  }
    decstate(o:string){ 
      //return this.lsstate.find(x=>x.value == o).desc;
      return (o=="IO") ? " Active " : "Deliveried";
    }
    decstateicn(o:string){ return this.lsstate.find(x=>x.value == o).icon; }
    dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }

    decicnstate(o:string) { return (o=="PE") ? " fas fa-pallet text-warning ": "fas fa-truck-loading"; }
    decicnstaterv(o:string) { return (o=="LD") ? " fas fa-pallet text-warning ": "fas fa-truck-loading"; }
    descstate(o:string){ 
      switch(o){
        case 'IO' : return 'Waiting start';
        case 'PA' : return 'Preparing';
        case 'PE' : return 'Pick confirmed';
        case 'LD' : return 'Load to truck';
        case 'ED' : return 'Delivery';
        default : return o;
      }
    }

    getstate(){ 
        this.mv.getlov("OUORDER","FLOW").pipe().subscribe(
            (res) => { this.lsstate = res; },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
          );
    }
    gettype(){ 
        this.mv.getlov("TASK","TYPE").pipe().subscribe(
            (res) => { this.lstype = res;  },
            (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
            () => { }
          );
    }

    fndthsum(){ 
      this.sv.thsum(this.pmroute).subscribe((res) => { this.lsthsum = res; if (this.lsthsum.length > 0) { this.pmroute.thcode = this.lsthsum[0].thcode; this.fnd();} });
    }
    fnd(){ 
        this.sv.find(this.pmroute).subscribe(            
          (res) => { 
              this.lsroute = res; 
              this.routerowselect = -1;
              this.hunorowselect = -1;
            },
          (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }

    getinfo(o:route_ls,ix:number){ 
      this.routerowselect = ix;
      // this.hunorowselect = -1;
      this.slchuline = new Array();
      this.readytoship = 0;  
      this.crroute = o;
      this.sv.get(o).subscribe(            
        (res) => {             
          this.routesource = res;
          this.routesource.thname = o.thname;
          try { 
            this.routesource.routetypename = this.lsroutetype.find(x => x.value == this.routesource.routetype).desc;
            this.routesource.transportor = this.lstransportor.find(x => x.value == this.routesource.transportor).desc;
            this.routesource.loccode = this.lsstaging.find(x => x.value == this.routesource.loccode).desc;
            this.routesource.trucktype = this.lstrucktype.find(x => x.value == this.routesource.trucktype).desc;
            this.routesource.loadtype = this.lsloadtype.find(x => x.value == this.routesource.loadtype).desc;
          }catch (excp) { }
          if (this.routesource.hus.length > 0) {
            if (this.routesource.hus.filter(x=> x.tflow == "LD").length == this.routesource.hus.length) {
              this.readytoship = 1;
            }else { 
              this.readytoship = 0;
            }
            this.gethuinfo(this.routesource.hus[0]); 
          }else { 
            this.readytoship = 0;
          }
          }
      );
      this.crtab = 2;
    }
    gethuinfo(o:route_hu) { 
      this.slchu.huno = o.huno;
      this.slchu.tflow = o.tflow;
      this.hv.lines(this.slchu).subscribe(            
        (res) => {             
          this.slchuline = res;
          },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }
    huselect(ix:number){
      this.hunorowselect =  ix;
    }
    huload(o:route_hu,ix:number){      
      this.hunorowselect = ix;
      this.slchu.huno = o.huno;
      o.routeno = this.routesource.routeno;
      this.sv.huload(o).subscribe(            
        (res) => {             
          this.toastr.info("<span class='fn-07e'>Load process success</span>",null,{ enableHtml : true }); 
          this.getinfo(this.crroute,this.routerowselect);
          },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    savereoute() { 
      this.ngPopups.confirm('Do you confirm to modify route infomation ?')
      .subscribe(res => {
          if (res) {
            this.sv.upsert(this.routesource).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'>modify route success</span>",null,{ enableHtml : true }); 
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                 
          } 
      });
    }

    closeshipment(){
      this.ngPopups.confirm('Do you confirm to Close shipment ?')
      .subscribe(res => {
          if (res) {
            this.sv.shipment(this.routesource).subscribe(            
              (res) => {             
                this.toastr.info("<span class='fn-07e'>Close shipment success</span>",null,{ enableHtml : true }); 
                this.fnd();
                this.getinfo(this.crroute,this.routerowselect);
                },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );              
          } 
      });

  }

  getloadingdraft(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.sv.getloaddraft(this.routesource.orgcode, this.routesource.site, this.routesource.depot,this.routesource.routeno).subscribe(response => {
      let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_loadingdraft_" + this.routesource.routeno + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId); 
      }), 
      error => { 
      this.toastr.clear(this.toastRef.ToastId);
      }
  }

  getpackinglist(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.sv.getpackinglist(this.routesource.orgcode, this.routesource.site, this.routesource.depot,this.routesource.routeno, this.routesource.outrno).subscribe(response => {
      let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_Packinglist_" + this.routesource.routeno + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId); 
      }), 
      error => { 
      this.toastr.clear(this.toastRef.ToastId);
      }
  }

  gettransportnote(o: string) {
    this.toastRef = this.toastr.warning(" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",null,{
      disableTimeOut: true,
      tapToDismiss: false,
      //toastClass: "toast border-red",
      closeButton: false,
      positionClass:'toast-bottom-right',enableHtml : true
    });

    this.sv.gettransportnote(this.routesource.orgcode, this.routesource.site, this.routesource.depot,this.routesource.routeno, this.routesource.outrno).subscribe(response => {
      let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.setAttribute('download', "bgcwms_Transportnote_" + this.routesource.routeno + ".pdf");
      document.body.appendChild(downloadLink);
      downloadLink.click();
      this.toastr.clear(this.toastRef.ToastId); 
      }), 
      error => { 
      this.toastr.clear(this.toastRef.ToastId);
      }
  }


  ngDecOpstype(o:string) {  
    switch(o){ 
      case "A" : return ": Pallet pick"
      case "X" : return "Distribute"
      default :  return ": Loose pick"
    }
  }
  getMaster() {
    // this.mv.getlov("OUORDER","FLOW").pipe().subscribe((res)=> { this.lsstate = res; });
    // this.mv.getlov("ROUTE","TYPE").pipe().subscribe((res)=> { this.lsroutetype = res; });
    // this.mv.getlov("TRANSPORT","LOADTYPE").pipe().subscribe((res)=> { this.lsloadtype = res; });
    // this.mv.getlov("TRANSPORT","PAYMENT").pipe().subscribe((res)=> { this.lspayment = res; });
    // this.mv.getlov("TRANSPORT","TRUCKTYPE").pipe().subscribe((res)=> { this.lstrucktype = res; });
    // this.mv.getlov("TRANSPORT","TRTMODE").pipe().subscribe((res)=> { this.lstrtmode = res; });
    // this.sv.gettransporter().subscribe( (res) => { this.lstransportor = res; } );
    // this.lsrowlmt = this.mv.getRowlimit();

    Promise.all([
      this.mv.getlov("OUORDER","FLOW").toPromise(), 
      this.mv.getlov("ROUTE","TYPE").toPromise(),
      this.mv.getlov("TRANSPORT","LOADTYPE").toPromise(),
      this.mv.getlov("TRANSPORT","PAYMENT").toPromise(), 
      this.mv.getlov("TRANSPORT","TRUCKTYPE").toPromise(),
      this.mv.getlov("TRANSPORT","TRTMODE").toPromise(),
      this.sv.gettransporter().toPromise()
    ]).then(res=> {
      this.lsstate = res[0];
      this.lsroutetype = res[1];
      this.lsloadtype = res[2];
      this.lspayment = res[3];
      this.lstrucktype = res[4];
      this.lstrtmode = res[5];
      this.lstransportor =res[6];
      this.lsrowlmt = this.mv.getRowlimit()
    })


  }
  ngSelTH(o:route_thsum,ix:number){
     this.pmroute.thcode = o.thcode;
     this.storerowselect=ix; 
     this.routerowselect = -1;
     this.hunorowselect = -1;
     this.fnd(); }
  ngDecUnitstock(o:string) { return this.mv.ngDecUnitstock(o); } 
  ngDecColor(o:number) { return this.mv.ngDecColor(o); }
  ngDectype(o:string) { try { return this.lsroutetype.find(e=>e.value == o).desc }catch(ex) { return o;} }
  ngDecload(o:string) { try { return this.lsloadtype.find(e=>e.value == o).desc }catch(ex) { return o;} }
  ngDecpayment(o:string) { try { return this.lspayment.find(e=>e.value == o).desc }catch(ex) { return o;} }
  ngDectruck(o:string) { try { return this.lstrucktype.find(e=>e.value == o).desc }catch(ex) { return o;} }
  ngDectrtmode(o:string) { try { return this.lstrtmode.find(e=>e.value == o).desc }catch(ex) { return o;} }
  ngDectransport(o:string) { try {return this.lstransportor.find(e=>e.value == o).desc  }catch(ex) { return o;} }
  ngOnDestroy():void { }
}
