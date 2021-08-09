import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { lov } from '../../../helpers/lov';
import { exsInbound } from '../../models/snaps.wms.externalsource.inbound';
import { exsFile } from '../../models/snaps.wms.externalsource.file';
import { externalSourcelService } from '../../services/app-external.service';
import { exsLocup } from '../../models/snaps.wms.externalsource.outbound';
@Component({
  selector: 'appext-storagesource',
  templateUrl: 'extstorage.source.html',
  styles: ['.dgexsource { height:200px !important; } ','.dgexline { height:calc(100vh - 480px) !important; } '] ,
  providers: [
    {provide: NgbDateAdapter,         useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}]    
})
export class extstoragesource implements OnInit {
  @Input() item: lov;
  @Input() dateformat:string;
  @Input() dateformatlong:string;
  //@Input() lsrowlmt:lov[];
  @Input() pageSize:number;

  public lsline:exsLocup[] = new Array();
  public lssource:exsFile[] = new Array();
  //public lsstorage:exsstorage[] = new Array();
  public pm:exsFile = new exsFile();
  public page:number = 4;
  public slrowlmt:lov;

  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //Rowlomit
  public lsrowlmt:lov[] = new Array();

  constructor(private ss:shareService, private sv:externalSourcelService) { this.ss.ngSetup(); this.ngSetup(); this.pm.imptype = "locup"; }

  ngOnInit() { }
  ngAfterViewInit(){  this.meFnd(); }

  ngSetup():void{ this.lsrowlmt = this.ss.getRowlimit(); console.log(this.lsrowlmt); }

  meFnd() { this.sv.findLocup(this.pm).subscribe((res) => {this.lssource = res; if(this.lssource.length > 0) { this.meLine(this.lssource[0]); }});}
  meLine(o:exsFile) { this.sv.getLocupLines(o).subscribe((res) => { this.lsline = res;}); }
  
  ngDecState(o:string){ return this.ss.ngDecState(o); }
  ngDecIcon(o:string) { return this.ss.ngDecIcon(o); }
  ngChangeRowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

  ngOnDestroy():void{
    this.lsline = null;         delete this.lsline;
    this.lssource = null;       delete this.lssource;
    this.page = null;           delete this.page;
    this.slrowlmt = null;       delete this.slrowlmt;
    this.dateformat = null;     delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.lsrowlmt = null;       delete this.lsrowlmt;
    this.pm = null;             delete this.pm;
  }
  
}
