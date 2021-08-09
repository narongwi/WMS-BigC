import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { shareService } from 'src/app/share.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { binary_md } from '../../models/adm.binary.model';
import { binaryService } from '../../services/adm.binary.service';

declare var $: any;
@Component({
  selector: 'app-admlov',
  templateUrl: 'adm.lov.html',
  styles: ['.dgline { height:calc(100vh - 145px) !important;','.dgvalue { height:200px !important; }'],

})
export class admlovComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public crtab:number = 1;

  lsrowlmt:lov[] = new Array();       //List of limit row
  pm:binary_md = new binary_md();     //Parameter 
  lstdesc:binary_md[] = new Array();  //List of Description
  lststate:lov[] = new Array();       //List of state
  lines:binary_md[] = new Array();    //List of Configuration
  slc:binary_md = new binary_md();    //Selection of binary
  cr:binary_md = new binary_md();     //Object of binary 
  crstate:boolean = false;
  constructor(private sv: shareService,
              private av: authService,
              private bv: binaryService,
              private router: RouterModule,
              private toastr: ToastrService,
              private ngPopups: NgPopupsService,) { 
    this.av.retriveAccess();  
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ this.ngGetmaster(); }
  ngGetmaster(){ 
    this.bv.desc(this.pm).pipe().subscribe(
      (res) => { this.lstdesc = res; },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { }
    );
    this.sv.lov("ALL","FLOW").pipe().subscribe(
      (res) => { this.lststate = res; },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { }
    );
  }
  ngGetline(o:binary_md){
    this.slc = o;
    this.bv.list(o).pipe().subscribe(
      (res) => { this.lines = res; },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { }
    );
  }
  ngSelc(o:binary_md){ this.cr = o; this.crstate = (o.bnstate == "IO") ? true : false; }
  ngNew() { 
    this.cr = new binary_md();
    this.cr.orgcode = this.slc.orgcode;
    this.cr.site = this.slc.site;
    this.cr.depot = this.slc.depot;
    this.cr.apps = this.slc.apps;
    this.cr.bntype = this.slc.bncode;
    this.cr.bncode = this.slc.bnvalue;
    this.cr.bnstate = "NW";
    this.crstate = true;
  }
  ngUpsert(){ 
    this.ngPopups.confirm('Do you change configuration of service ?')
    .subscribe(res => { 
      if (res) {
        this.cr.bnstate = (this.crstate == true) ? "IO" : "XX";
        this.bv.upsert(this.cr).pipe().subscribe(
          (res) => { this.toastr.success("<span class='fn-07e'>Modify binary config success</span>",null,{ enableHtml : true });  this.ngGetline(this.cr);},
          (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
      }
    });
  }
  getIcon(o:string){ return this.lststate.find(x=>x.value == o).icon; }
  getDesc(o:string){ return this.lststate.find(x=>x.value == o).desc; }
  //toggle(){ $('.snapsmenu').click();  }
}
