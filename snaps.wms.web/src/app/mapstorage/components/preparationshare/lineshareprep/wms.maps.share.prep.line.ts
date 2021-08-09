import { Component, OnInit,OnDestroy, ViewChild, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { shareService } from 'src/app/share.service';
import { textChangeRangeIsUnchanged } from 'typescript';
import { shareprep_md, shareprln_md } from '../../../Models/wms.maps.share.prep.model';
import { shareprepService } from '../../../services/wms.maps.share.prep.service';


declare var $: any;
@Component({
  selector: 'app-maps-share-prep-line',
  templateUrl: 'wms.maps.share.prep.line.html',
  styles: ['.dgzone { height:calc(100vh - 185px) !important; } '],
})
export class sharepreplnComponent implements OnInit, OnDestroy {
  @Input() dateformat: string;
  @Input() dateformatlong: string;
  slcshprep:shareprep_md = new shareprep_md();
  slcshprln:shareprln_md = new shareprln_md();
  slctflow:boolean = false;
  pm:shareprln_md = new shareprln_md();
  constructor(private sv: shareprepService,
              private ss:shareService,
              private toastr: ToastrService,
              private ngPopups: NgPopupsService) { 
    this.slcshprep.lines = new Array();
  }

  ngOnInit(): void { }  
  ngAfterViewInit(){ }
  ngSelect(o:shareprep_md){ this.slcshprep = o; this.ngGet(); }
  ngGet(){ 
    this.sv.get(this.slcshprep).pipe().subscribe(            
        (res) => { this.slcshprep = res; },
        (err) => { },
        () => { }
      );
  }
  ngSelln(o:shareprln_md){ 
      this.slcshprln = o; 
      this.slctflow  = (this.slcshprln.tflow == "IO") ? true : true;
   }
  ngUpsert(){ 
    this.slcshprln.tflow = (this.slctflow == true) ? "IO" : "XX";
    this.ngPopups.confirm('Do you confirm change for share preparation line ?')
    .subscribe(res => {
      if (res) {
        this.sv.upline(this.slcshprln).pipe().subscribe(            
          (res) => { this.toastr.success("<span class='fn-07e'>Modify share preparation success</span>",null,{ enableHtml : true }); this.ngGet(); },
          (err) => { },
          () => { }
        );
      } 
    });
  }
  ngNew() { 
      this.slcshprln = new shareprln_md();
      this.slcshprln.orgcode = this.slcshprep.orgcode;
      this.slcshprln.site = this.slcshprep.site;
      this.slcshprln.depot = this.slcshprep.depot;
      this.slcshprln.shprep = this.slcshprep.shprep;
      this.slcshprln.thcode = "";
      this.slcshprln.priority = 0;
      this.slctflow = true;
  }
  ngRemove(){ 
    this.ngPopups.confirm('Do you confirm to remove share preparation line ?')
    .subscribe(res => {
      if (res) {
        this.sv.rmline(this.slcshprln).pipe().subscribe(            
          (res) => { this.toastr.success("<span class='fn-07e'>Remove share preparation success</span>",null,{ enableHtml : true }); this.ngGet(); },
          (err) => { },
          () => { }
        );
      } 
    });
  }

  ngDecIcon(o:string){ try { return this.ss.ngDecIcon(o); } catch (exp) { return o; } }
  ngDecStr(o:string){  try { return this.ss.ngDecStr(o);  } catch (exp) { return o; } }
  ngOnDestroy():void { 
    this.slcshprep = null; delete this.slcshprep;
    this.slcshprln = null; delete this.slcshprln;    
    this.slctflow = null; delete this.slctflow;    
    this.pm = null; delete this.pm;
  }
}
