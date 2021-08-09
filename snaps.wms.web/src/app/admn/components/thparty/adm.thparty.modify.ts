import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { adminService } from '../../../admn/services/account.service';
import { lov } from '../../../helpers/lov';
import { thparty_ls, thparty_md, thparty_pm } from '../../models/adm.thparty.model';
import { admthpartyService } from '../../services/adm.thparty.service';
import { shareService } from 'src/app/share.service';
@Component({
  selector: 'appadm-thpartymodify',
  templateUrl: 'adm.thparty.modify.html',
  styles: ['.dgthparty { height:calc(100vh - 200px) !important; }'],
})
export class admthpartymodifyComponent implements OnInit {
   @Input() crthparty: thparty_md;
    ison:boolean=false;
    public lsthparty:thparty_ls[] = new Array();
    //public crthparty:thparty_md = new thparty_md();

    public pmthparty:thparty_pm = new thparty_pm();

    public slcbutype:lov;
    public slcthtype:lov;


    public msdstate:lov[] = new Array();
    public msdbu:lov[] = new Array();
    public msdthtype:lov[] = new Array();
    public msdtype:lov[] = new Array();
    
    public crstate:boolean = true;
    public spcareadsc:string;

    public lsstaginb:lov[] = new Array();
    public slstaginb:lov;
    public lsstagoub:lov[] = new Array();
    public slstagoub:lov;

    public formatdate:string;

    public allowchangeofexsource:boolean = false;
    public allowchangeplandate:boolean = false;
    public allowchangestate:boolean = false;

    constructor(
        private sv: admthpartyService,
        private av: authService,
        private mv: shareService, 
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.formatdate = this.av.crProfile.formatdatelong;
        this.pmthparty.orgcode = this.av.crProfile.orgcode;
        this.pmthparty.site = this.av.crRole.site;
        this.pmthparty.depot = this.av.crRole.depot;

        this.pmthparty.spcarea = "";
        this.pmthparty.thtype = "";
        this.pmthparty.thbutype = "";
        this.pmthparty.thcode = "";
        this.pmthparty.thname = "";
        this.pmthparty.telephone = "";
        this.pmthparty.email = "";
        this.pmthparty.mapaddress = "";
        this.pmthparty.tflow = "";
        //this.crthparty = new thparty_md();
        this.getstate(); this.getbutype(); this.getthtype();
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fndthparty(); } 
    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcbutype(){ 
      this.msdtype = this.msdthtype.filter(x=>x.valopnfirst == this.slcbutype.value); 
    }
    //ngselcbartype(){ this.crthparty.bartype = this.slcbartype.value; }

    getstate() { 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.msdstate = res;
          this.msdstate.push({ value : 'NW', desc : 'New thparty', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
          this.msdstate.push({ value : 'false', desc : 'New thparty', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
      this.mv.lovstaginginb().subscribe((res)=> { this.lsstaginb = res; });
      this.mv.lovstagingoub().subscribe((res)=> { this.lsstagoub = res; });
    }
    getbutype() { 
        this.mv.getlov("THPARTY","BUTYPE").pipe().subscribe(
          (res) => { this.msdbu = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
    }
    getthtype() { 
        this.mv.getlov("THPARTY","TYPE").pipe().subscribe(
          (res) => { this.msdthtype = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
    }


    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newthparty() { 
        this.crthparty.orgcode = this.av.crProfile.orgcode;
        this.crthparty.site = this.av.crRole.site;
        this.crthparty.depot = this.av.crRole.depot;
        this.crthparty.spcarea = ""; 
        this.crthparty.thtype = ""; 
        this.crthparty.thbutype = ""; 
        this.crthparty.thcode = ""; 
        this.crthparty.thname = ""; 
        this.crthparty.thgroup = ""; 
        this.crthparty.tflow = ""; 
        this.crthparty.thtypename = ""; 
        this.crthparty.thbutypename = "";
        this.crthparty.thcodealt = ""; 
        this.crthparty.vatcode = ""; 
        this.crthparty.thnamealt = ""; 
        this.crthparty.thnameint = ""; 
        this.crthparty.addressln1 = ""; 
        this.crthparty.addressln2 = ""; 
        this.crthparty.addressln3 = ""; 
        this.crthparty.subdistrict = ""; 
        this.crthparty.district = ""; 
        this.crthparty.city = ""; 
        this.crthparty.country = ""; 
        this.crthparty.postcode = ""; 
        this.crthparty.region = ""; 
        this.crthparty.telephone = ""; 
        this.crthparty.email = ""; 
        this.crthparty.thcomment = ""; 
        this.crthparty.throuteformat = ""; 
        this.crthparty.plandelivery = 0; 
        this.crthparty.naturalloss = 0; 
        this.crthparty.mapaddress = ""; 
        this.crthparty.datecreate = new Date();
        this.crthparty.accncreate = ""; 
        this.crthparty.datemodify = new Date();
        this.crthparty.accnmodify = ""; 
        this.crthparty.procmodify = ""; 
        
        
        this.toastr.info("<span class='fn-1e15'>New thparty is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crthparty.lsformat,"AA","","","",""));
    }
    fndthparty(){ 
      this.sv.find(this.pmthparty).pipe().subscribe(            
          (res) => { this.lsthparty = res; },
          (err) => { this.toastr.error(err.error.message); this.ison = false; },
          () => { }
      );
    }
    decbool(o:boolean):number {  return (o==true) ? 1 : 0 ; }
    encbool(o:number):boolean { return (o==1)? true : false; }
    selthparty(o:thparty_ls){ 
      this.sv.get(o).pipe().subscribe(            
        (res) => { 
          this.crthparty = res; this.crstate = (this.crthparty.tflow == "IO") ? true : false; 
          this.slcbutype = this.msdbu.find(c=>c.value == this.crthparty.thbutype);
          this.slcthtype = this.msdtype.find(c=>c.value == this.crthparty.thtype);
        },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }
    validate() {
      if (this.slcbutype == null) { 
        this.toastr.warning("<span class='fn-1e15'>Business unit is require !</span>",null,{ enableHtml : true }); 
      }else if (this.slcthtype == null){ 
        this.toastr.warning("<span class='fn-1e15'>Sub type is require !</span>",null,{ enableHtml : true }); 
      }else if (this.slstaginb == null) {
        this.toastr.warning("<span class='fn-1e15'>Inbound staging is require !</span>",null,{ enableHtml : true }); 
      }else if (this.slstagoub == null) {
        this.toastr.warning("<span class='fn-1e15'>Outbound staging is require !</span>",null,{ enableHtml : true }); 
      }else { 
        this.crthparty.tflow = (this.crstate == true) ? "IO" : "XX";
        this.crthparty.thbutype = this.slcbutype.value;
        this.crthparty.thtype = this.slcthtype.value;
        this.crthparty.indock = this.slstaginb.value;
        this.crthparty.oudock = this.slstagoub.value;
        this.ngPopups.confirm('Do you accept change configuration of Third party ?')
            .subscribe(res => {
                if (res) {
                    this.ison = true;
                    this.sv.upsert(this.crthparty).pipe().subscribe(            
                        (res) => { this.toastr.success("<span class='fn-1e15'>Setup thparty success</span>",null,{ enableHtml : true }); this.ison = false; this.fndthparty(); },
                        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                        () => { }
                    );
                             
                } 
            });
        }
    }

}
