import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from '../../../helpers/ngx-bootstrap.config';
import { shareService } from '../../../share.service';
import { lov } from '../../../helpers/lov';
import { exsFile } from '../../models/snaps.wms.externalsource.file';
import { exsInbouln, exsInbound } from '../../models/snaps.wms.externalsource.inbound';
import { externalSourcelService } from '../../services/app-external.service';
import { exsOutbouln } from '../../models/snaps.wms.externalsource.outbound';
@Component({
  selector: 'appext-ordoulsource',
  templateUrl: 'extordoul.source.html',
  styles: ['.dgexsource { height:200px !important; } ','.dgexline { height:calc(100vh - 480px) !important; } '] ,
  providers: [
    {provide: NgbDateAdapter,         useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}]    
})
export class extordoulsource implements OnInit {
  @Input() item: lov;
  @Input() dateformat:string;
  @Input() dateformatlong:string;
  //@Input() lsrowlmt:lov[];
  @Input() pageSize:number;

  public lsline:lov[] = new Array();
  public lssource:exsFile[] = new Array();
  public lsbarcode:exsOutbouln[] = new Array();
  public pm:exsFile = new exsFile();
  public page:number = 4;
  public slrowlmt:lov;

  //Sorting 
  public lssort:string = "spcarea";
  public lsreverse: boolean = false; // for sorting
  //Rowlomit
  public lsrowlmt:lov[] = new Array();

  constructor(private ss:shareService, private sv:externalSourcelService) { this.ss.ngSetup(); this.ngSetup(); this.pm.imptype = "outbouln";  }

  ngOnInit() { }
  ngAfterViewInit(){  this.meFnd(); }

  ngSetup():void{ this.lsrowlmt = this.ss.getRowlimit(); console.log(this.lsrowlmt); }

  meFnd() { this.sv.findOutbound(this.pm).subscribe((res) => {this.lssource = res; if(this.lssource.length > 0) { this.meLine(this.lssource[0]); }});}
  meLine(o:exsFile) { this.sv.getOutboulnLines(o).subscribe((res) => { this.lsbarcode = res;}); }
  
  ngDecState(o:string){ return this.ss.ngDecState(o); }
  ngDecIcon(o:string) { return this.ss.ngDecIcon(o); }
  ngDecUnitstock(o:string){ return this.ss.ngDecUnitstock(o); }
  ngChangeRowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

  ngOnDestroy():void{
    this.lsline = null;         delete this.lsline;
    this.lssource = null;       delete this.lssource;
    this.page = null;           delete this.page;
    this.slrowlmt = null;       delete this.slrowlmt;
    this.dateformat = null;     delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.lsrowlmt = null;       delete this.lsrowlmt;
    this.lsbarcode = null;      delete this.lsbarcode;
    this.pm = null;             delete this.pm;
  }
}