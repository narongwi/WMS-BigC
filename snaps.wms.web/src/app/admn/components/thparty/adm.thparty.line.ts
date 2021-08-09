import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { adminService } from '../../../admn/services/account.service';
import { lov } from '../../../helpers/lov';
import { thparty_ls, thparty_md, thparty_pm } from '../../models/adm.thparty.model';
import { admthpartyService } from '../../services/adm.thparty.service';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
@Component({
  selector: 'appadm-thpartyline',
  templateUrl: 'adm.thparty.line.html',
  styles: ['.dgthirdparty { height:calc(100vh - 235px) !important;  }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class admthpartylineComponent implements OnInit {
//   @Input() iconstate: string;

  @Output() selln = new EventEmitter<thparty_ls>();

  ison:boolean=false;
  public lsthparty:thparty_ls[] = new Array();


  public pm:thparty_pm = new thparty_pm();




  public msdstate:lov[] = new Array();
  public msdbu:lov[] = new Array();
  public msdthtype:lov[] = new Array();
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

  //Tab
  crtab:number = 1;

  //Select 
  slcthparty:string;
  slcbutype:lov;
  slcthtype:lov;
    constructor(
        private sv: admthpartyService,
        private av: authService,
        private mv: adminService, 
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 

        this.getmaster(); 
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fndthparty(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcbutype(){ this.msdtype = this.msdthtype.filter(x=>x.valopnfirst == this.slcbutype.value); }
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

    getmaster() { 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.msdstate = res;
          this.msdstate.push({ value : 'NW', desc : 'New thparty', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
          this.msdstate.push({ value : 'false', desc : 'New thparty', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.getlov("THPARTY","BUTYPE").pipe().subscribe(
        (res) => { this.msdbu = res; },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );

      this.mv.getlov("THPARTY","TYPE").pipe().subscribe(
        (res) => { this.msdthtype = res; },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.getlov("DATAGRID","ROWLIMIT").pipe().subscribe(
        (res) => { this.lsrowlmt = res.sort((a,b) => parseInt(a.value) - parseInt(b.value));  this.slrowlmt = this.lsrowlmt.find(x=>x.value == this.pageSize.toString()); },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }


    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }

    fndthparty(){ 

      this.pm.thbutype = (this.slcbutype != null) ? this.slcbutype.value : null;
      this.pm.thtype   = (this.slcthtype != null) ? this.slcthtype.value : null;

      this.sv.find(this.pm).pipe().subscribe(            
          (res) => { 
            
            this.lsthparty = res; 
            this.lsthparty.forEach(x=> {
              try { 
                x.thbutypename = this.msdbu.find(e=> e.value.toString().trim() == x.thbutype.toString().trim()).desc;
              }catch(err) {
                x.thbutypename = x.thbutype;
              }

              try { 
                x.thtypename = this.msdthtype.find(e=> e.value == x.thtype).desc;
              }catch(err) {
                x.thtypename = x.thtype;
              }
            });
          },
          (err) => { this.toastr.error(err.error.message); this.ison = false; },
          () => { }
      );
    }
    decbool(o:boolean):number {  return (o==true) ? 1 : 0 ; }
    encbool(o:number):boolean { return (o==1)? true : false; }
    selthparty(o:thparty_ls){
      this.selln.emit(o);
      
    }
    //   this.sv.get(o).pipe().subscribe(            
    //     (res) => { 
    //       this.crthparty = res; 
    //       this.crstate = (this.crthparty.tflow == "IO") ? true : false; 
    //     },
    //     (err) => { this.toastr.error(err.error.message); this.ison = false; },
    //     () => { }
    // );
    // }
}
