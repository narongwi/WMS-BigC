import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { shareprep_md } from '../../Models/wms.maps.share.prep.model';
import { shareprepService } from '../../services/wms.maps.share.prep.service';
import { authService } from '../../../auth/services/auth.service';
import { sharepreplnComponent } from './lineshareprep/wms.maps.share.prep.line';
import { NgPopupsService } from 'ng-popups';
import { ThrowStmt } from '@angular/compiler';
import { shareService } from 'src/app/share.service';
import { setSourceMapRange } from 'typescript';
declare var $: any;
@Component({
    selector: 'app-maps-share-prep',
    templateUrl: 'wms.maps.share.prep.html',
    styles: ['.dgzone { height:calc(100vh - 185px) !important; } '],
})
export class shareprepComponent implements OnInit, OnDestroy {
  @ViewChild(sharepreplnComponent) sobline: sharepreplnComponent;
  dateformat:string = "";
  dateformatlong:string = "";
  slcshprep:string = "";
  crtab:number = 1;
  lsshareprep:shareprep_md[] = new Array();
  public pm:shareprep_md = new shareprep_md();
  slc: shareprep_md = new shareprep_md();
  slctflow:boolean = false;
  constructor(private sv: shareprepService,
              private av: authService, 
              private toastr: ToastrService,
              private ngPopups: NgPopupsService,
              private ss: shareService) { 
      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void { }
 
  ngAfterViewInit(){ this.ngFind(); }
  ngSelect(o:shareprep_md){ 
      this.slcshprep = o.shprep; this.slc = o; this.slctflow = (this.slc.tflow == "IO") ? true : false;
  }
  ngFind(){ 
    this.sv.list(this.pm).pipe().subscribe(            
      (res) => { this.lsshareprep = res; },
      (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      () => { }
    );
  }
  ngNew(){ 
    this.slc = new shareprep_md();
    this.slc.isfullfill = 0;
    this.slc.shprepname = "";
    this.slc.shprepdesc = "";
    this.slc.tflow = "NW";
    this.slc.lines = new Array();
    this.slctflow = true;
  }
  ngUpsert(){
    this.slc.tflow = (this.slctflow == true) ? "IO" : "XX";
    this.ngPopups.confirm('Do you confirm change for share preparation ?')
    .subscribe(res => {
      if (res) {
        this.sv.upsert(this.slc).pipe().subscribe(            
          (res) => { this.toastr.success("<span class='fn-07e'>Modify share preparation success</span>",null,{ enableHtml : true });  this.ngFind(); },
          (err) => { },
          () => { }
        );
      } 
    });
  }
  ngRemove(){ 
    this.ngPopups.confirm('Do you confirm remove share preparation ?')
    .subscribe(res => {
      if (res) {
        this.sv.remove(this.slc).pipe().subscribe(            
          (res) => { this.toastr.success("<span class='fn-07e'>Renive share preparation success</span>",null,{ enableHtml : true });  },
          (err) => { },
          () => { }
        );
      } 
    });
  }
  ngConfig(){ 
    this.sobline.ngSelect(this.slc); this.crtab = 2;
  }


  ngDecIcon(o:string){ return this.ss.ngDecIcon(o); }
  ngDecStr(o:string){ return this.ss.ngDecStr(o); ; }
  ngOnDestroy():void { 
    this.lsshareprep = null; delete this.lsshareprep;
    this.slcshprep = null; delete this.slcshprep;
    this.pm = null; delete this.pm; 
  }
}
