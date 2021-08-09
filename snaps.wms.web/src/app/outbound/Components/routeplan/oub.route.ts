import { ArrayType, ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ThumbXDirective } from 'ngx-scrollbar/lib/scrollbar/thumb/thumb.directive';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { route_ls, route_md, route_pm } from '../../Models/oub.route.model';
import { ourouteService } from '../../Services/oub.route.service';

declare var $: any;
@Component({
  selector: 'appoub-route',
  templateUrl: 'oub.route.html',
  styles: ['.dgroute { height:calc(100vh - 235px) !important; }','.dglines { height:calc(100vh - 155px) !important;}'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter },
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter }] 
})
export class oubrouteComponent implements OnInit,OnDestroy ,AfterViewInit{
  public lsstate:lov[] = new Array();

  public instocksum:number = 0;
  public rqedit:number = 0;
  public crstate:boolean = false;

  public lsroute:route_ls[] = new Array();
  public crroute:route_md = new route_md();
  public pm:route_pm = new route_pm();
    
  //public msdstaging:lov[] = new Array();
  public lsstaging:lov[] = new Array();
  public slcstaging:lov = new lov();

  public lsroutetype:lov[] = new Array();
  public slcroutetype:lov = new lov();


  public lsthparty: lov[] = new Array();
  public slcthparty: lov;

  public lstransportor: lov[] = new Array();
  public slctransportor: lov;

  public lsloadtype: lov[] = new Array();
  public slcloadtype: lov;

  public lspayment: lov[] = new Array();
  public slcpayment: lov;

  public lstrucktype: lov[] = new Array();
  public slctrucktype: lov;

  public lstrtmode: lov[] = new Array();
  public slctrtmode: lov;

  public reqnew: number = 0;
  public lsreqmsm:lov[] = new Array();
  public lstatem:lov[] = new Array();
  public pmroutetype:lov;
  public pmthparty:lov;
  public pmstaging:lov;
  public pmtrtmode:lov;
  public pmtransportor:lov;
  public pmloadtype:lov;
  public pmpayment:lov;
  public pmtrucktype:lov;
  public pmpriority:lov;
  public pmstate:lov;

  //Date format
  public dateformat:string;
  public dateformatshort:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;

  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting

  //PageNavigate
  public page = 4;
  public pageSize = 100;
  public slrowlmt:lov;
  public lsrowlmt:lov[] = new Array();

  /* Tab */
  public crtab:Number = 1;
  public gnroute:route_md = new route_md();
  public routenoselect :number;
  constructor(private sv: ourouteService,
              private av: authService, 
              private mv: shareService,
              private router: RouterModule,
              private toastr: ToastrService,                
              private ngPopups: NgPopupsService,) { 

    this.mv.ngSetup();
    this.av.retriveAccess();
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.dateformatshort = this.av.crProfile.formatdateshort;
    this.lstatem.push({ desc :"Active", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "", valopnthird : "", value : "IO" });
    this.lstatem.push({ desc :"Deliveried", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "", valopnthird : "", value : "ED" });
    this.gnroute.mxhu = 16; 
    this.gnroute.mxweight = 99999; 
    this.gnroute.mxvolume = 999999;
    this.gnroute.datereqdeliverytime = "10:00";
    this.gnroute.paytype = "NONE";
  }

  ngOnInit(): void { }

  ngAfterViewInit() { 
    this.setupJS(); 
    /*setTimeout(this.toggle, 1000);*/  
    this.getMaster();  
    this.fndroute(); 
  }

  setupJS() { 
      // sidebar nav scrolling
      $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' });   
  }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  decstate(o:string){ return this.lsstate.find(x=>x.value == o).desc; }
  decstateicn(o:string){ return this.lsstate.find(x=>x.value == o).icon; }

  getMaster() {
    // this.mv.getlov("OUORDER","FLOW").pipe().subscribe((res)=> { this.lsstate = res; });
    // this.mv.getlov("ROUTE","TYPE").pipe().subscribe((res)=> { this.lsroutetype = res; });
    // this.mv.getlov("TRANSPORT","LOADTYPE").pipe().subscribe((res)=> { this.lsloadtype = res; });
    // this.mv.getlov("TRANSPORT","PAYMENT").pipe().subscribe((res)=> { this.lspayment = res; });
    // this.mv.getlov("TRANSPORT","TRUCKTYPE").pipe().subscribe((res)=> { this.lstrucktype = res; });
    // this.mv.getlov("TRANSPORT","TRTMODE").pipe().subscribe((res)=> { this.lstrtmode = res; });
    // this.sv.getstaging().subscribe((res) => { this.lsstaging = res; } );  
    // this.sv.gettransporter().subscribe( (res) => { this.lstransportor = res; } );
    // ;

        
    Promise.all([
      this.mv.getlov("OUORDER","FLOW").toPromise(), 
      this.mv.getlov("ROUTE","TYPE").toPromise(),
      this.mv.getlov("TRANSPORT","LOADTYPE").toPromise(),
      this.mv.getlov("TRANSPORT","PAYMENT").toPromise(), 
      this.mv.getlov("TRANSPORT","TRUCKTYPE").toPromise(),
      this.mv.getlov("TRANSPORT","TRTMODE").toPromise(),
      this.sv.getstaging().toPromise(),
      this.sv.gettransporter().toPromise()
    ]).then(res=> {
      this.lsstate = res[0];
      this.lsroutetype = res[1];
      this.lsloadtype = res[2];
      this.lspayment = res[3];
      this.lstrucktype = res[4];
      this.lstrtmode = res[5];
      this.lsstaging =res[6];
      this.lstransportor = res[7];
      this.lsrowlmt = this.mv.getRowlimit();
      this.slcpayment = this.lspayment.find(e=>e.value == "NONE");
    })

  }
  pregen() { 

    this.slcstaging = this.lsstaging[0];
    this.slctrtmode = this.lstrtmode[1];
    this.slctrucktype = this.lstrucktype[0];
    this.slctransportor = this.lstransportor.find(e=> e.value == "4019");
    this.slcloadtype = this.lsloadtype[0];
    this.slcpayment = this.lspayment.find(e=>e.value == "NONE");
    this.slcroutetype = this.lsroutetype[1];
  }

  fndroute(){ 
    this.pm.routetype = (this.pmroutetype == null) ? null : this.pmroutetype.value;
    //this.pm.thcode = (this.pmroutetype == null) ? null : this.pmthparty.value;
    this.pm.stating = (this.pmstaging == null) ? null : this.pmstaging.value;
    this.pm.trttype = (this.pmtrtmode == null) ? null : this.pmtrtmode.value;
    this.pm.transportor = (this.pmtransportor == null) ? null : this.pmtransportor.value;
    this.pm.loadtype = (this.pmloadtype == null) ? null : this.pmloadtype.value;
    this.pm.paymenttype = (this.pmpayment == null) ? null : this.pmpayment.value;
    this.pm.trucktype = (this.pmtrucktype == null) ? null : this.pmtrucktype.value;
    this.pm.oupriority = (this.pmpriority == null) ? null : parseInt(this.pmpriority.value);
    this.pm.tflow = (this.pmstate == null) ? null : this.pmstate.value;
    this.sv.find(this.pm).subscribe(            
      (res) => {  this.lsroute = res; /*if(this.lsroute.length > 0) { this.getinfo(this.lsroute[0]); }*/ },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { } 
    );
  }
  getinfo(o:route_ls,ix:number){ 
    this.routenoselect = ix;
    this.sv.get(o).subscribe(
      (res) => {
        this.reqnew = 0;
        this.crroute = res;
        this.crroute.routetypename = this.lsroutetype.find(x => x.value == this.crroute.routetype).desc;
        this.slctransportor = this.lstransportor.find(x => x.value == this.crroute.transportor);
        this.slcstaging = this.lsstaging.find(x => x.value == this.crroute.loccode);
        this.slctrucktype = this.lstrucktype.find(x => x.value == this.crroute.trucktype);
       
        this.slcloadtype = this.lsloadtype.find(x => x.value == this.crroute.loadtype);
        //this.slcpayment = this.lspayment.find(e=>e.value == this.crroute.paytype);
        
        this.slcpayment = this.lspayment.find(e=>e.value == ((this.crroute.paytype == '') ? "NONE" : this.crroute.paytype));

        this.crstate = (this.crroute.tflow == "XO" ) ? true : false;
        this.crtab = 2;
      },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { } 
    );
  }

    newroute() { 
      this.crroute.orgcode = "";
      this.crroute.site = "";
      this.crroute.depot = "";
      this.crroute.spcarea = "";
      this.crroute.routetype = "";
      this.crroute.routeno = "";
      this.crroute.oupromo = "";
      this.crroute.thcode = "";
      this.crroute.plandate = new Date();
      this.crroute.utlzcode = "";
      this.crroute.driver = "";
      this.crroute.thname = "";
      this.crroute.routetypename = "";
      this.crroute.oupriority = 0;
      this.crroute.datereqdelivery = new Date();

      this.crroute.routename = "";
      this.crroute.trttype = "";
      this.crroute.loadtype = "";
      this.crroute.trucktype = "";
      this.crroute.trtmode = "";
      this.crroute.loccode = "";
      this.crroute.paytype = "";
      this.crroute.loaddate = new Date();
      this.crroute.dateshipment = new Date();

      this.crroute.relocationto = "";
      this.crroute.relocationtask = "";
      this.crroute.shipper = "";
      this.crroute.mxhu = 0;
      this.crroute.mxweight = 0;
      this.crroute.mxvolume = 0;
      this.crroute.crhu = 0;
      this.crroute.crweight = 0;
      this.crroute.crvolume = 0;
      this.crroute.plateNo = "";
      this.crroute.contactno = "";
      this.crroute.datecreate = new Date();
      this.crroute.accncreate = "";
      this.crroute.datemodify = new Date();
      this.crroute.accnmodify = "";
      this.crroute.procmodify = "";
      this.reqnew = 1;
      this.crroute.tflow = "NW";
    }

    upsert() { 
      this.ngPopups.confirm('Do you confirm to modify route  ?')
      .subscribe(res => {
          if (res) {
            this.crroute.tflow = ( this.crstate == true) ? "XO" : "IO";
            this.crroute.loccode = (this.slcstaging == null) ? null : this.slcstaging.value;
            this.crroute.trucktype = (this.slctrucktype == null) ? null : this.slctrucktype.value;
            this.crroute.loadtype = (this.slcloadtype == null) ? null : this.slcloadtype.value;            
            this.crroute.transportor = (this.slctransportor == null) ? null : this.slctransportor.value;
            this.crroute.paytype = (this.slcpayment == null) ? null : this.slcpayment.value;
            this.sv.upsert(this.crroute).subscribe( res => { 
                this.toastr.success("<span class='fn-07e'>modify route success</span>",null,{ enableHtml : true }); 
                this.fndroute();
              }
            );                 
          } 
      });
    }

    remove(){ 
      this.ngPopups.confirm('Do you confirm to remove route  ?')
      .subscribe(res => {
          if (res) {
            this.sv.remove(this.crroute).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'>remove route success</span>",null,{ enableHtml : true }); 
                this.fndroute();
                this.crtab = 1;
              }
            );                 
          } 
      });
    }

    generate() { 
      if (this.slcroutetype==null) { this.toastr.warning("<span class='fn-07e'>Route type is require</span>",null,{ enableHtml : true });   }
      else if (this.slcroutetype.value != "C" && this.gnroute.thcode == null) { this.toastr.warning("<span class='fn-07e'>Thirdpary is require</span>",null,{ enableHtml : true }); }
      else if (this.slcstaging==null)  { this.toastr.warning("<span class='fn-07e'>Staging is require</span>",null,{ enableHtml : true });  }
      else if (this.slctrtmode==null)  { this.toastr.warning("<span class='fn-07e'>Transport mode is require</span>",null,{ enableHtml : true });  }
      else if (this.slctrucktype==null)  { this.toastr.warning("<span class='fn-07e'>Transport type is require</span>",null,{ enableHtml : true });   }
      else if (this.slcloadtype==null)  { this.toastr.warning("<span class='fn-07e'>Loading type is require</span>",null,{ enableHtml : true });   }
      else if (this.slcpayment==null) { this.toastr.warning("<span class='fn-07e'>Payment type is require</span>",null,{ enableHtml : true });   }
      else { 
        this.gnroute.tflow = "NW";
        this.ngPopups.confirm('Do you confirm to generate route  ?').subscribe(res => {
          if (res) {            
            let dt = new Date(this.gnroute.datereqdelivery);    
            dt.setHours(parseInt(this.gnroute.datereqdeliverytime.substring(0,2)));
            dt.setMinutes(parseInt(this.gnroute.datereqdeliverytime.substring(3,5)));

            this.gnroute.datereqdelivery = dt;
            this.gnroute.routetype = this.slcroutetype.value;
            this.gnroute.loccode = (this.slcstaging == null) ? null : this.slcstaging.value;
            this.gnroute.trucktype = (this.slctrucktype == null) ? null : this.slctrucktype.value;
            this.gnroute.loadtype = (this.slcloadtype == null) ? null : this.slcloadtype.value;            
            this.gnroute.transportor = (this.slctransportor == null) ? null : this.slctransportor.value;
            this.crroute.paytype = (this.slcpayment == null) ? null : this.slcpayment.value;
            this.sv.upsert(this.gnroute).subscribe( res => { 
                this.toastr.success("<span class='fn-07e'>modify route success</span>",null,{ enableHtml : true }); 
                this.fndroute();
              }
            );                 
          } 
        });
      }
    }


    ngDesRoute(o:string) { return (o=="P")? "Preparation": (o=="C") ? "Combine" : "Delivery";}
    ngDecState(o:string) { if (o=="ED") { return "Deliveried"; }else if (o=="PE") { return "Prep.finished"; } else if (o=="XO") { return "Block"; } else { return this.mv.ngDecState(o); } }
    ngDecColor(o:number) { return this.mv.ngDecColor(o); }

    ngOnDestroy():void {  
      this.lsstate        = null; delete this.lsstate;
      this.instocksum     = null; delete this.instocksum;
      this.rqedit         = null; delete this.rqedit;
      this.crstate        = null; delete this.crstate;
      this.lsroute        = null; delete this.lsroute;
      this.crroute        = null; delete this.crroute;
      this.pm             = null; delete this.pm;
      this.lsstaging      = null; delete this.lsstaging;
      this.slcstaging     = null; delete this.slcstaging;
      this.lsroutetype    = null; delete this.lsroutetype;
      this.slcroutetype   = null; delete this.slcroutetype;
      this.lsthparty      = null; delete this.lsthparty;
      this.slcthparty     = null; delete this.slcthparty;
      this.lstransportor  = null; delete this.lstransportor;
      this.slctransportor = null; delete this.slctransportor;
      this.lsloadtype     = null; delete this.lsloadtype;
      this.slcloadtype    = null; delete this.slcloadtype;
      this.lspayment      = null; delete this.lspayment;
      this.slcpayment     = null; delete this.slcpayment;
      this.lstrucktype    = null; delete this.lstrucktype;
      this.slctrucktype   = null; delete this.slctrucktype;
      this.lstrtmode      = null; delete this.lstrtmode;
      this.slctrtmode     = null; delete this.slctrtmode;
      this.reqnew         = null; delete this.reqnew;
      this.lsreqmsm       = null; delete this.lsreqmsm;
      this.lstatem        = null; delete this.lstatem;
      this.pmroutetype    = null; delete this.pmroutetype;
      this.pmthparty      = null; delete this.pmthparty;
      this.pmstaging      = null; delete this.pmstaging;
      this.pmtrtmode      = null; delete this.pmtrtmode;
      this.pmtransportor  = null; delete this.pmtransportor;
      this.pmloadtype     = null; delete this.pmloadtype;
      this.pmpayment      = null; delete this.pmpayment;
      this.pmtrucktype    = null; delete this.pmtrucktype;
      this.pmpriority     = null; delete this.pmpriority;
      this.pmstate        = null; delete this.pmstate;
      this.dateformat     = null; delete this.dateformat;
      this.dateformatlong = null; delete this.dateformatlong ;
      this.datereplan     = null; delete this.datereplan;
      this.lssort         = null; delete this.lssort;
      this.lsreverse      = null; delete this.lsreverse;
      this.page           = null; delete this.page;
      this.pageSize       = null; delete this.pageSize;
      this.slrowlmt       = null; delete this.slrowlmt;
      this.lsrowlmt       = null; delete this.lsrowlmt;
      this.crtab			    = null; delete this.crtab	;		
    }

}
