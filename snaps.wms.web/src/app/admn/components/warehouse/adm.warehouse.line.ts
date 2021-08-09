import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { adminService } from '../../../admn/services/account.service';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { warehouse_ls, warehouse_md, warehouse_pm } from '../../models/adm.warehouse.model';
import { admwarehouseService } from '../../services/adm.warehouse.service';
import { thparty_md, thparty_pm } from '../../models/adm.thparty.model';
import { admthpartyService } from '../../services/adm.thparty.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

@Component({
  selector: 'appadm-warehouseline',
  templateUrl: 'adm.warehouse.line.html',
  styles: ['.dgline { height:calc(100vh - 145px) !important;','.dgvalue { height:200px !important; }'],
})
export class admwarehouselineComponent implements OnInit {
    @Input() item: lov;

    ison:boolean=false;

    public lswarehouse:warehouse_ls[] = new Array();
    public crwarehouse:warehouse_md = new warehouse_md();
    public craddress:thparty_md = new thparty_md();
    public pmwarehouse:warehouse_pm = new warehouse_pm();
    public pmaddress:thparty_pm = new thparty_pm();
    public slcopstype:lov = new lov();

    public msdstate:lov[] = new Array();
    public msdopstype:lov[] = new Array();
    public crstate:Boolean = false;

    constructor(
        private av: authService,
        private sv: admwarehouseService,
        private tv: admthpartyService,
        private mv: adminService,
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.pmwarehouse.orgcode = this.av.crProfile.orgcode;
        this.getstate(); this.gettype();
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fndwarehouse(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    //ngselczone(){ this.crwarehouse.lszone = this.slczone.desc; this.crwarehouse.spcarea = this.slczone.valopnfirst }
    getstate() { 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.msdstate = res; 
          this.msdstate.push({ value : 'NW', desc : 'New Zone', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    gettype() { 
      this.mv.getlov("WAREHOUSE","TYPE").pipe().subscribe(
        (res) => { this.msdopstype = res; 
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }

    newaddress() { 
      this.craddress.orgcode = "";
      this.craddress.site = ""; 
      this.craddress.depot = ""; 
      this.craddress.spcarea = "SI"; 
      this.craddress.thtype = "SI"; 
      this.craddress.thbutype = "SI"; 
      this.craddress.thcode = ""; 
      this.craddress.thname = ""; 
      this.craddress.thgroup = ""; 
      this.craddress.tflow = ""; 
      this.craddress.thtypename = ""; 
      this.craddress.thbutypename = ""; 
      this.craddress.thnamealt = "";
      this.craddress.thcodealt = ""; 
      this.craddress.vatcode = "";     
      this.craddress.thnameint = ""; 
      this.craddress.addressln1 = ""; 
      this.craddress.addressln2 = ""; 
      this.craddress.addressln3 = ""; 
      this.craddress.subdistrict = ""; 
      this.craddress.district = ""; 
      this.craddress.city = ""; 
      this.craddress.country = ""; 
      this.craddress.postcode = ""; 
      this.craddress.region = ""; 
      this.craddress.telephone = ""; 
      this.craddress.email = ""; 
      this.craddress.thcomment = ""; 
      this.craddress.throuteformat = ""; 
      this.craddress.plandelivery = 0;
      this.craddress.naturalloss = 0; 
      this.craddress.mapaddress = ""; 
      this.craddress.datecreate = new Date();
      this.craddress.accncreate = ""; 
      this.craddress.datemodify = new Date();
      this.craddress.accnmodify = ""; 
      this.craddress.procmodify = ""; 
    }
    newwarehouse() { 
        this.crwarehouse.orgcode = this.av.crProfile.orgcode;
        this.crwarehouse.sitecode = "";
        this.crwarehouse.sitename = "";
        this.crwarehouse.sitetype = "";
        this.crwarehouse.datestart = new Date();
        this.crwarehouse.dateend = new Date();
        this.crwarehouse.tflow = "IO";
        this.crwarehouse.sitenamealt = "";
        this.crwarehouse.sitekey = "";
        this.crwarehouse.datecreate = new Date();
        this.crwarehouse.accncreate = "";
        this.crwarehouse.datemodify = new Date();
        this.crwarehouse.accnmodify = "";
        this.crwarehouse.procmodify = "";
        this.crwarehouse.siteid = 0;
        this.crstate = true;
        this.newaddress();
        if(this.lswarehouse.findIndex(x=>x.tflow =="NW") == -1) {this.lswarehouse.push(this.crwarehouse);}        
        this.toastr.info("<span class='fn-1e15'>New warehouse is ready to setup</span>",null,{ enableHtml : true });
    }
    fndwarehouse(){ 
        this.sv.find(this.pmwarehouse).pipe().subscribe(            
            (res) => { this.lswarehouse = res; },
            (err) => { this.toastr.error(err.error.message); this.ison = false; },
            () => { }
        );
    }
    getsite(o:warehouse_ls) { 
      this.sv.get(o).pipe().subscribe(            
        (res) => { 
          this.crwarehouse = res; 
          this.crstate = (this.crwarehouse.tflow == "IO") ? true : false; 
          this.slcopstype = this.msdopstype.find(x=>x.value == this.crwarehouse.sitetype);
        },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }
    getaddress(o:warehouse_ls) {
      this.pmaddress.orgcode = o.orgcode;
      this.pmaddress.site = o.sitecode;
      this.pmaddress.thcode = o.sitecode;
      this.pmaddress.thbutype = "SI"; 
      this.tv.get(this.pmaddress).pipe().subscribe(            
        (res) => { 
          console.log(res);
          if (res.thcode != null) { 
            this.craddress = res; 
          }else {
            this.newaddress();
          }
        },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }
    selwarehouse(o:warehouse_ls){ this.getsite(o); this.getaddress(o); }
    validate() {
      this.crwarehouse.tflow = (this.crstate == true) ? "IO" : "XX";
      this.craddress.tflow = (this.crstate == true)? "IO" : "XX";  
      this.craddress.thname = this.crwarehouse.sitename;
      this.craddress.thcode = this.crwarehouse.sitecode;
      this.craddress.thcodealt = this.crwarehouse.sitenamealt;
      this.craddress.site = this.crwarehouse.sitecode;
      this.crwarehouse.sitetype = this.slcopstype.value;
      this.ngPopups.confirm('Do you accept change of site?')
      .subscribe(res => { 
          if (res) {
            this.ison = true;
            this.sv.upsert(this.crwarehouse).pipe().subscribe(            
                  (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndwarehouse(); },
                  (err) => { this.toastr.error(err.error.message); this.ison = false; },
                  () => { }
            );
            this.tv.upsert(this.craddress).pipe().subscribe(            
              (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndwarehouse(); },
              (err) => { this.toastr.error(err.error.message); this.ison = false; },
              () => { }
            );
            //this.crwarehouse.tflowcnt = this.crwarehouse.tflow;
            // this.sv.upsertlocup(this.crwarehouse).pipe().subscribe(            
            //     (res) => { this.toastr.success('Save successful'); this.ison = false; this.fndwarehouse(); },
            //     (err) => { this.toastr.error(err.error.message); this.ison = false; },
            //     () => { }
            // );
              
          } 
      });
    }

}
