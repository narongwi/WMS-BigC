import { ThrowStmt } from '@angular/compiler';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { authService } from 'src/app/auth/services/auth.service';
import { adminService } from '../../services/account.service';
import { warehouse_ls } from '../../models/adm.warehouse.model';
import { depot_ls } from '../../models/adm.depot.model';
import { accn_ls, accn_md, accn_pm } from '../../models/account.model';
import { lov } from '../../../helpers/lov';
import { ToastrService } from 'ngx-toastr';
import { shareService } from 'src/app/share.service';


declare var $: any;
@Component({
  selector: 'app-printer',
  templateUrl: './printer.component.html',
  styles: ['.dgaccn { height:calc(100vh - 240px) !important;  } ','.dglines { height:calc(100vh - 685px) !important; }'],
})
export class PrinterComponent implements OnInit, OnDestroy {

  public lswarehouse: warehouse_ls[] = new Array();
  public lsdepot: depot_ls[] = new Array();

  public lsaccn : accn_ls[] = new Array();
  public pmaccn : accn_pm = new accn_pm();
  public mdaccn : accn_md = new accn_md();

  public lsrole: lov[] = [];
  public lsacnstate : lov[] = new Array();
  public lstype : lov[] = new Array();
  public slcstate:lov;

  crtab:number = 1;

  //PageNavigate
  public page = 4;
  public pageSize = 200;
  public slrowlmt:lov;
  public lsrowlmt:lov[] = new Array();
  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //Date format 
  public formatdate:string;
  public formatdatelong:string;
  public accselect:number;
  constructor(private sv: adminService, private av: authService, private ss:shareService, private router: RouterModule,private toastr: ToastrService) { 
    this.ss.ngSetup(); this.av.retriveAccess(); 
  }


  ngOnInit(): void { }
  ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/ this.ngSetup(); this.fndaccn(); }
  setupJS() { 
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });
  }

  getIcon(o:string){  }
  fndaccn() { 
    this.pmaccn.tflow = (this.slcstate == null) ? "" : this.slcstate.value;
    this.sv.accnFind(this.pmaccn).pipe().subscribe(            
      (res) => { this.lsaccn = res; },
      (err) => {  },
      () => { } 
    );
  }
  accnget(o:accn_ls,ix:number) { 
    this.accselect = ix;
    this.sv.accnGet(o).pipe().subscribe(            
      (res) => { this.mdaccn = res; },
      (err) => { this.toastr.error(err.error.message) },
      () => { } 
    );
  } 
  refresh(e:any){
    this.fndaccn(); 
  }
  //toggle(){ $('.snapsmenu').click();  }
  ngDecIcon(o:string){ try { return this.lsacnstate.find(x=>x.value == o).icon; } catch(exp) { return o; } }
  ngDecState(o:string){ try { return this.lsacnstate.find(e=>e.value == o).desc; } catch(exp) { return o;} }
  ngChangeRowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */
  ngSelcompare(item, selected) { return item.value === selected } /* ngSelect compare object */
  ngSetup(){ 
    this.sv.getlov("ACCOUNT","FLOW").subscribe( (res) => { this.lsacnstate = res; } );
    this.sv.getlov("ACCOUNT","TYPE").subscribe( (res) => { this.lstype = res;});
    this.sv.getlovrole().pipe().subscribe( (res) => { this.lsrole = res;  });
    this.lsrowlmt = this.ss.getRowlimit(); 
  }

  ngOnDestroy(){ 

  }
}
