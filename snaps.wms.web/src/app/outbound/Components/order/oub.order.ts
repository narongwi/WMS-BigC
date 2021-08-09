import { Component, OnInit,OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { shareService } from 'src/app/share.service';
import { resolveModuleName, resolveProjectReferencePath } from 'typescript';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../../Models/oub.order.model';

import { outboundService } from '../../Services/oub.service';

declare var $: any;
@Component({
  selector: 'appoub-order',
  templateUrl: 'oub.order.html',
  styles: ['.dgorder {  height:250px !important; } ','.dglines { height:calc(100vh - 685px) !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class ouborderComponent implements OnInit, OnDestroy, AfterViewInit {
    public lsstate:lov[] = new Array();
    public lstype:lov[] = new Array();
    public instocksum:number = 0;
    public rqedit:number = 0;
    public crstate:boolean = false;

    public lsorder:outbound_ls[] = new Array();
    public slcorder:outbound_md;
    public slclines:outbouln_md[] = new Array();
    public slclineso:number = 0;
    public slcliness:number = 0;
    public slcline:outbouln_md;
    
    public pm:outbound_pm = new outbound_pm();
    
    crtab:number = 1;
    crorder:string = "";

    //Date format
    public dateformat:string;
    public dateformatlong:string;
    public dateformatshort:string;
    public datereplan: Date | string | null;

    //Sorting 
    public lssort:string = "spcarea";
    public lsreverse: boolean = false; // for sorting

    //PageNavigate
    page = 4;
    pageSize = 200;
    slrowlmt:lov;
    lsrowlmt:lov[] = [];
    lsyesno:lov[] = new Array();
    lsunit:lov[] = new Array(); //unit list
    lsstatem:lov[] = new Array();  //order state master
    lsspcarea:lov[] = new Array(); //Aera

    slcordertype:lov;
    slcarea:lov;
    slcpriority:lov;
    slcstate:lov;

    chnrqdate:number = 0;
    chnremark:number = 0;
    public ordrowselect:number;
    constructor(private sv: outboundService,
                private av: authService, 
                private mv: shareService,
                private router: RouterModule,
                private toastr: ToastrService,                
                private ngPopups: NgPopupsService,) { 
        this.av.retriveAccess(); 
        this.dateformat = this.av.crProfile.formatdate;
        this.dateformatlong = this.av.crProfile.formatdatelong;
        this.dateformatshort = this.av.crProfile.formatdateshort;
        this.pm.datereqdel = new Date();
    }

    ngOnInit(): void { }
    ngOnDestroy():void {  }
    ngAfterViewInit(){  
      this.setupJS(); /*setTimeout(this.toggle, 1000);*/
      this.getmaster(); 
      this.fndorder(); 
      this.lsrowlmt = this.mv.getRowlimit();
      this.lsunit = this.mv.getUnit();
    }
    setupJS() { 
        // sidebar nav scrolling
        $('#accn-list .sidebar-scroll').slimScroll({ height: '95%', wheelStep: 5, touchScrollStep: 50, color: '#cecece' }); 
    $('#main-menu').metisMenu();

    }
    getIcon(o:string){ return "";  }
    //toggle(){ $('.snapsmenu').click();  }
    decstate(o:string){ return this.lsstate.find(x=>x.value == o).desc; }
    decstateicn(o:string){ try { return this.lsstate.find(x=>x.value == o).icon; } catch (exp){ return o; } }
    dectype(o:string) { return this.lstype.find(x=>x.value == o).desc; }
    changerowlmt() { this.pageSize = parseInt(this.slrowlmt.value); } /* Row limit */

    setreqdate() { this.chnrqdate = (this.chnrqdate == 0) ? 1 : 0; }
    flagremarks() {  this.chnremark = (this.chnremark == 0) ? 1 : 0;}


    getmaster(){ 
    
      Promise.all([
        this.mv.getlov("OUBORDER","FLOW").toPromise(), 
        this.mv.getlov("ORDER","SUBTYPE").toPromise(),
        this.mv.lovms("OUBORDER","FLOW").toPromise()
      ]).then(res=> {
        console.log(res);
        this.lsstate = res[0];
        this.lstype = res[1].filter(e=>['OU','INOU'].includes(e.valopnfour));
        this.lsstatem = res[2];
        this.lsspcarea = this.mv.getArea();
        this.lsyesno = this.mv.getYesno();
      })
      // this.mv.getlov("OUBORDER","FLOW").pipe().subscribe(
      //     (res) => { this.lsstate = res; },
      //     (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //     () => { }
      //   );
      //   this.mv.getlov("ORDER","SUBTYPE").pipe().subscribe(
      //     (res) => { this.lstype = res.filter(e=>['OU','INOU'].includes(e.valopnfour));  },
      //     (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //     () => { }
      //   );
      // this.mv.lovms("OUBORDER","FLOW").pipe().subscribe(
      //   (res) => { this.lsstatem = res; },
      //   (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );


        
    }
    fndorder(){ 
        this.pm.ousubtype = (this.slcordertype == null) ? null : this.slcordertype.value;
        this.pm.spcarea =  (this.slcarea == null) ? null :  this.slcarea.value;
        this.pm.oupriority = (this.slcpriority == null) ? 100 : parseInt(this.slcpriority.value);
        this.pm.tflow =  (this.slcstate == null) ? null :   this.slcstate.value;
        this.sv.find(this.pm).subscribe(            
          (res) => { 
              this.lsorder = res;
              // this.lsorder.forEach(x=> {
              //   try{ x.ousubtypedesc = this.lstype.find(e=>e.value == x.ousubtype).desc } catch (exp){ x.ousubtypedesc = x.ousubtype; }
              // });
              if (this.lsrowlmt.length == 0) { 
                this.lsrowlmt = this.mv.getRowlimit(); this.slrowlmt = this.lsrowlmt.find(e=> parseInt(e.value) == this.pageSize);
                this.lsunit = this.mv.getUnit();
                
              }
              
              if (this.lsorder.length == 0) { 
                this.toastr.error("<span class='fn-07e'>Order found with your cryteria</span>",null,{ enableHtml : true });
              }else { 
                this.getinfo(this.lsorder[0],0);
              }
              
            },
          (err) => { this.toastr.info("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { } 
        );
    }
    getinfo(o:outbound_ls,ix:number){ 
      this.ordrowselect = ix;
      this.sv.get(o).subscribe(            
        (res) => { this.slcorder = res; this.slclines = res.lines;
          this.slclineso = this.slclines.reduce((obl, val) => obl += val.qtysku, 0); 
          this.slcliness = this.slclines.reduce((obl, val) => obl += val.qtyskudel, 0);        
        },
        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { } 
      );
    }

    setedit(o:outbouln_md) { this.rqedit = 1; this.slcline = o; }
    changereqdate(){
      this.ngPopups.confirm('Do you confirm change request delivery date')
      .subscribe(res => {
          if (res) {
            // this.crstock.tflow = (this.crstate == true) ? "IO" : "XX";
            this.sv.changeRequest(this.slcorder).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-1e15'>Change request delivery date success</span>",null,{ enableHtml : true }); 
              },
              (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { }
            );                 
          } 
        });
    }
    
    setpriority(){
      this.ngPopups.confirm('Do you confirm to set priority of an order  ?')
      .subscribe(res => {
          if (res) {
            this.sv.setpriority(this.slcorder).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'>modify stock line success</span>",null,{ enableHtml : true }); 
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                 
          } 
      });
    }

    setremarks(){
      this.ngPopups.confirm('Do you confirm to set remarks of an order  ?')
      .subscribe(res => {
          if (res) {
            this.sv.setremarks(this.slcorder).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'>set remarks success</span>",null,{ enableHtml : true }); 
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                 
          } 
      });
    }

    setlineorder(){
      this.ngPopups.confirm('Do you confirm to set info on order line ?')
      .subscribe(res => {
          if (res) {
            this.sv.setlineorder(this.slcline).subscribe(            
              (res) => { 
                this.toastr.success("<span class='fn-07e'>set line info success</span>",null,{ enableHtml : true }); 
                this.rqedit = 0;
              },
              (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
              () => { } 
            );                 
          } 
      });
    }

    ulcorder(){ this.rqedit = 0; }
    blockovqty() { 
      if (this.slcline.qtyreqpu > this.slcline.qtypndpu ) {
         this.slcline.qtyreqpu = this.slcline.qtypndpu; 
         this.toastr.warning("<span class='fn-07e'>Quantity is over order</span>",null,{ enableHtml : true }); 
        }else if (this.slcline.qtyreqpu < 0){
          this.slcline.qtyreqpu = this.slcline.qtypndpu; 
          this.toastr.warning("<span class='fn-07e'>Quantity cannot lass than zero</span>",null,{ enableHtml : true }); 
        }
    }
    ngDecType(o:string){ try { return this.lstype.find(e=>e.value == o).desc } catch (exp) { return o; } }
    ngDecUnitstock(o:string){ return this.mv.ngDecUnitstock(o); }
    ngDeclndesc(o:string){ return (o == "PC") ? "Preparing" : this.mv.ngDecState(o); }
    ngDestroy():void{ 
      this.lsstate = null; delete this.lsstate;
      this.lstype = null; delete this.lstype;
      this.instocksum = null; delete this.instocksum;
      this.rqedit = null; delete this.rqedit;
      this.crstate = null; delete this.crstate;
      this.lsorder = null; delete this.lsorder;
      this.slcorder = null; delete this.slcorder;
      this.slclines = null; delete this.slclines;
      this.slclineso = null; delete this.slclineso;
      this.slcliness = null; delete this.slcliness;
      this.slcline = null; delete this.slcline;
      this.pm = null; delete this.pm;
      this.crtab = null; delete this.crtab;
      this.crorder = null; delete this.crorder;
      this.dateformat = null; delete this.dateformat;
      this.dateformatlong = null; delete this.dateformatlong;
      this.datereplan = null; delete this.datereplan;
      this.lssort = null; delete this.lssort;
      this.lsreverse = null; delete this.lsreverse;
      this.page = null; delete this.page;
      this.pageSize = null; delete this.pageSize;
      this.slrowlmt = null; delete this.slrowlmt;
      this.lsrowlmt = null; delete this.lsrowlmt;
      this.lsunit = null; delete this.lsunit;
      this.chnrqdate = null; delete this.chnrqdate;
      this.chnremark = null; delete this.chnremark;
    }

}
