import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { adminService } from '../../../admn/services/account.service';
import { lov } from '../../../helpers/lov';
import { barcode_ls, barcode_md, barcode_pm } from '../../models/adm.barcode.model';
import { admbarcodeService } from '../../services/adm.barcode.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
@Component({
  selector: 'appadm-barcodeline',
  templateUrl: 'adm.barcode.line.html',
  styles: ['.dgbarcode { height:calc(100vh - 235px) !important;  }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 
})
export class admbarcodelineComponent implements OnInit {
//   @Input() iconstate: string;
    ison:boolean=false;
    public lsbarcode:barcode_ls[] = new Array();
    public crbarcode:barcode_md = new barcode_md();

    public pm:barcode_pm = new barcode_pm();

    public slcbarops:lov = new lov();
    public slcbartype:lov = new lov();

    public msdstate:lov[] = new Array();
    public msdops:lov[] = new Array();
    public msdtype:lov[] = new Array();
    
    public crstate:boolean = true;
    public spcareadsc:string;

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

  public allowchangeofexsource:boolean;

    constructor(
        private sv: admbarcodeService,
        private av: authService,
        private mv: adminService, 
        private ss: shareService,
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.getmaster(); 
        this.ss.ngSetup();
        this.dateformat = this.av.crProfile.formatdate;
        this.dateformatlong = this.av.crProfile.formatdatelong;
       
     }

    ngOnInit() { this.ngAllowRevise(); }
    ngAfterViewInit(){ 
      this.fndbarcode(); 
    } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcbarcode(){ this.crbarcode.barops = this.slcbarops.value; }
    ngselcbartype(){ this.crbarcode.bartype = this.slcbartype.value; }
    changerowlmt() { 
      this.pageSize = parseInt(this.slrowlmt.value); 
    } /* Row limit */
    ngAllowRevise() { 
      this.sv.parameter().pipe().subscribe((res)=> { 
        this.allowchangeofexsource = res.find(e=>e.pmcode == "allowchangeofexsource").pmvalue;
      });
    }

    getmaster() { 

      Promise.all([
        this.mv.getlov("ALL","FLOW").toPromise(), 
        this.mv.getlov("BARCODE","OPR").toPromise(),
        this.mv.getlov("BARCODE","TYPE").toPromise(),
        this.ss.lovrowlimit().toPromise()
      ]).
      then((res) => {
        this.msdstate = res[0];
        this.msdstate.push({ value : 'NW', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        this.msdstate.push({ value : 'false', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});

        this.msdops = res[1];
        this.msdtype = res[2];
        this.lsrowlmt = res[3].sort((a,b)=> Number(a.value) - Number(b.value));
      
      });

      // this.mv.getlov("ALL","FLOW").pipe().subscribe(
      //   (res) => { this.msdstate = res;
      //     this.msdstate.push({ value : 'NW', desc : 'New barcode', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
      //     this.msdstate.push({ value : 'false', desc : 'New barcode', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
      //   },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      // this.mv.getlov("BARCODE","OPR").pipe().subscribe(
      //   (res) => { this.msdops = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      // this.mv.getlov("BARCODE","TYPE").pipe().subscribe(
      //   (res) => { this.msdtype = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
    }



    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newbarcode() { 
        this.crbarcode.orgcode = this.av.crProfile.orgcode;
        this.crbarcode.site = this.av.crRole.site;
        this.crbarcode.depot = this.av.crRole.depot;
        this.crbarcode.spcarea = "";
        this.crbarcode.article = "";
        this.crbarcode.pv = 0;
        this.crbarcode.lv = 0;
        this.crbarcode.bartype = "";
        this.crbarcode.barops = "";
        this.crbarcode.barcode = "";
        this.crbarcode.thname = "";
        this.crbarcode.thcode = "";
        this.crbarcode.tflow = "NW";
        this.crbarcode.articledsc = "";
        this.crbarcode.accncreate = "";
        this.crbarcode.accnmodify = "";
        this.crbarcode.procmodify = ""; 
        this.crbarcode.barremarks = "";
        this.crbarcode.datecreate =  new Date();
        this.crbarcode.datemodify =  new Date();
        this.crbarcode.procmodify = "modbarcode"   
        this.toastr.info("<span class='fn-07e'>New barcode is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crbarcode.lsformat,"AA","","","",""));
    }
    fndbarcode(){ 
      this.sv.find(this.pm).pipe().subscribe(            
          (res) => { this.lsbarcode = res; },
          (err) => { this.toastr.error(err.error.message); this.ison = false; },
          () => { }
      );
    }
    decbool(o:boolean):number {  return (o==true) ? 1 : 0 ; }
    encbool(o:number):boolean { return (o==1)? true : false; }
    selbarcode(o:barcode_ls){ 
      this.sv.get(o).pipe().subscribe(            
        (res) => { 
          this.crbarcode = res; this.crstate = (this.crbarcode.tflow == "IO") ? true : false; 
          this.slcbarops = this.msdops.find(c=>c.value == this.crbarcode.barops);
          this.slcbartype = this.msdtype.find(c=>c.value == this.crbarcode.bartype);
        },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }
    validate() {
        this.crbarcode.tflow = (this.crstate == true) ? "IO" : "XX";
        this.crbarcode.barops = this.slcbarops.value;
        this.crbarcode.bartype = this.slcbartype.value;
        this.ngPopups.confirm('Do you accept change configuration of barcode ?')
            .subscribe(res => {
                if (res) {
                    this.ison = true;
                    this.sv.upsert(this.crbarcode).pipe().subscribe(            
                        (res) => { this.toastr.success("<span class='fn-07e'>Setup barcode success</span>",null,{ enableHtml : true }); this.ison = false; this.fndbarcode(); },
                        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                        () => { }
                    );
                             
                } 
            });
        }

}
