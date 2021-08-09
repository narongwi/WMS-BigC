import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { thparty_ls, thparty_md, thparty_pm } from '../../models/adm.thparty.model';
import { adminService } from '../../services/account.service';
import { admthpartyService } from '../../services/adm.thparty.service';
import { admthpartymodifyComponent } from './adm.thparty.modify';
declare var $: any;
@Component({
  selector: 'app-admthparty',
  templateUrl: 'adm.thparty.html',
  styles: ['.dproduct { height:calc(100vh - 235px) !important;  }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class admthpartyComponent implements OnInit {
  @ViewChild('modify') modify:admthpartymodifyComponent;

  public lslog:lov[] = new Array();
  public crthparty:thparty_md = new thparty_md();
  public slthparty:thparty_ls;
  public pm:thparty_pm = new thparty_pm();

  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;

  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting

  //PageNavigate
  page = 4;
  pageSize = 300;
  slrowlmt:lov;
  lsrowlmt:lov[] = new Array();

  //LOV
  lsunit:lov[] = new Array();

  //Tab
  crtab:number = 1;

  //Select 
  slcthparty:string;
  constructor(
    private sv: admthpartyService,
    private av: authService,
    private mv: adminService, 
    private toastr: ToastrService,) { 

      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong;
    }
  ngOnInit(): void { this.ngAllowRevise(); }
  ngOnDestroy():void {  }
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
  ngAllowRevise() { 
    this.sv.parameter().pipe().subscribe((res)=> { 
      this.modify.allowchangeofexsource = res.find(e=>e.pmcode == "allowchangeofexsource").pmvalue;
      this.modify.allowchangeplandate = res.find(e=>e.pmcode == "allowchangeplandate").pmvalue;
      this.modify.allowchangestate = res.find(e=>e.pmcode == "allowchangestate").pmvalue;
    });
  }
  selthparty(o:thparty_ls){ 
    this.sv.get(o).pipe().subscribe(            
      (res) => { 
        this.slcthparty = o.thcode;
        this.crthparty = res; 
        this.crtab = 2;
      },
      (err) => { this.toastr.error(err.error.message); },
      () => { }
  );
  }

}
