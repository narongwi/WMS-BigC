import { Component, OnInit, Input } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { adminService } from '../../../admn/services/account.service';
import { lov } from '../../../helpers/lov';
import { admdevice_ls, admdevice_md, admdevice_pm } from '../../models/adm.device.model';
import { admdeviceService } from '../../services/adm.device.service';
@Component({
  selector: 'appadm-deviceline',
  templateUrl: 'adm.device.line.html'
})
export class admdevicelineComponent implements OnInit {
//   @Input() iconstate: string;
    ison:boolean=false;
    public lsdevice:admdevice_ls[] = new Array();
    public crdevice:admdevice_md = new admdevice_md();

    public pmdevice:admdevice_pm = new admdevice_pm();
    public slcdevice:lov = new lov();
    public slccategory:lov = new lov();

    public msdstate:lov[] = new Array();
    public msdcategory:lov[] = new Array();

    public crisreceipt: boolean = false; 
    public cristaskptw: boolean = false; 
    public cristasktrf: boolean = false; 
    public cristaskload: boolean = false; 
    public cristaskrpn: boolean = false; 
    public cristaskgen: boolean = false; 
    public crispick: boolean = false; 
    public crisdistribute: boolean = false; 
    public crisforward: boolean = false; 
    public criscount: boolean = false; 
    
    public crstate:boolean = true;
    public spcareadsc:string;

    constructor(
        private sv: admdeviceService,
        private av: authService,
        private mv: adminService, 
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.pmdevice.orgcode = this.av.crProfile.orgcode;
        this.pmdevice.site = this.av.crRole.site;
        this.pmdevice.depot = this.av.crRole.depot;
        this.getstate(); this.gettype();
     }

    ngOnInit() { }
    ngAfterViewInit(){ this.fnddevice(); } 

    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcdevice(){ this.crdevice.devtype = this.slcdevice.value; }
    getstate() { 
      this.mv.getlov("ALL","FLOW").pipe().subscribe(
        (res) => { this.msdstate = res;
          this.msdstate.push({ value : 'NW', desc : 'New device', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
          this.msdstate.push({ value : 'false', desc : 'New device', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        },
        (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
        () => { }
      );
    }
    gettype() { 
        this.mv.getlov("DEVICE","TYPE").pipe().subscribe(
          (res) => { this.msdcategory = res; },
          (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
          () => { }
        );
      }

    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newdevice() { 
        this.crdevice.orgcode = this.av.crProfile.orgcode;
        this.crdevice.site = this.av.crRole.site;
        this.crdevice.depot = this.av.crRole.depot;
        this.crdevice.spcarea = "";
        this.crdevice.devtype = "";
        this.crdevice.devcode = "";
        this.crdevice.devid = 0;
        this.crdevice.devmodel = "";
        this.crdevice.devserial = "";
        this.crdevice.datelastactive = new Date();
        this.crdevice.opsmaxheight = 0;
        this.crdevice.opsmaxweight = 0;
        this.crdevice.opsmaxvolume = 0;
        this.crdevice.isreceipt = 0;
        this.crdevice.istaskptw = 0;
        this.crdevice.istasktrf = 0;
        this.crdevice.istaskload = 0;
        this.crdevice.istaskrpn = 0;
        this.crdevice.istaskgen =0;
        this.crdevice.ispick = 0;
        this.crdevice.isdistribute = 0;
        this.crdevice.isforward = 0;
        this.crdevice.iscount = 0;
        this.crdevice.tflow = "IO";
        this.crdevice.devhash = "";

        this.crdevice.devserial = "";
        this.crdevice.opsmaxheight = 9999999;
        this.crdevice.opsmaxweight = 9999999;
        this.crdevice.opsmaxvolume = 9999999;
        this.crdevice.devremarks = "";

        this.crisreceipt = false;
        this.cristaskptw = false;
        this.cristasktrf = false;
        this.cristaskload = false;
        this.cristaskrpn = false;
        this.cristaskgen = false;
        this.crispick = false;
        this.crisdistribute = false;
        this.crisforward = false;
        this.criscount = false;
        
        this.crdevice.datecreate =  new Date();
        this.crdevice.datemodify =  new Date();

        this.crdevice.procmodify = "moddevice"   
        this.toastr.info("<span class='fn-1e15'>New device is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crdevice.lsformat,"AA","","","",""));
    }
    fnddevice(){ 
      this.sv.find(this.pmdevice).pipe().subscribe(            
          (res) => { this.lsdevice = res; },
          (err) => { this.toastr.error(err.error.message); this.ison = false; },
          () => { }
      );
    }
    seldevice(o:admdevice_ls){ 
      this.sv.get(o).pipe().subscribe(            
        (res) => { this.crdevice = res; },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }

    decbool(o:boolean):number {  return (o==true) ? 1 : 0 ; }
    encbool(o:number):boolean { return (o==1)? true : false; }

    validate() {
      this.crdevice.isreceipt = this.decbool(this.crisreceipt);
      this.crdevice.istaskptw = this.decbool(this.cristaskptw);
      this.crdevice.istasktrf = this.decbool(this.cristasktrf);
      this.crdevice.istaskload = this.decbool(this.cristaskload);
      this.crdevice.istaskrpn = this.decbool(this.cristaskrpn);
      this.crdevice.istaskgen = this.decbool(this.cristaskgen);
      this.crdevice.ispick = this.decbool(this.crispick);
      this.crdevice.isdistribute = this.decbool(this.crisdistribute);
      this.crdevice.isforward = this.decbool(this.crisforward);
      this.crdevice.iscount = this.decbool(this.criscount);

      this.crdevice.tflow = (this.crstate == true) ? "IO" : "XX";
      this.crdevice.devtype = this.slccategory.value;
      this.ngPopups.confirm('Do you accept change configuration of device ?')
          .subscribe(res => {
              if (res) {
                  this.ison = true;
                  this.sv.upsert(this.crdevice).pipe().subscribe(            
                      (res) => { 
                        this.toastr.success("<span class='fn-1e15'>Setup device area success</span>",null,{ enableHtml : true }); this.ison = false; this.fnddevice(); 
                      },
                      (err) => { this.toastr.error("<span class='fn-1e15'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                      () => { }
                  );
                            
              } 
          });
      }

}
