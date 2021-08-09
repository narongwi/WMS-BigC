import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { pam_parameter } from 'src/app/admn/models/adm.parameter.model';
import { admparameterService } from 'src/app/admn/services/adm.parameter.service';
import { authService } from '../../../../auth/services/auth.service';
import { lov } from '../../../../helpers/lov';
import { admwarehouseService } from '../../../services/adm.warehouse.service';

declare var $: any;
@Component({
  selector: 'app-admpaminventory',
  templateUrl: 'adm.parameter.inventory.html'

})
export class admpaminventoryComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public lsunit:lov[] = new Array();
  public pam:pam_parameter[] = new Array();


  //Correction stock 
  csallowblankremarks:pam_parameter = new pam_parameter();
  csallowblankrefereceno:pam_parameter = new pam_parameter();
  csallowchangeunit:pam_parameter = new pam_parameter();
  csallowprintlabelonreserve:pam_parameter = new pam_parameter();
  csallowgentaskfornewhu:pam_parameter = new pam_parameter();
  csallowincludehubelongingtask:pam_parameter = new pam_parameter();

  //Transfer stock
  tfallowchangeunit:pam_parameter = new pam_parameter();
  tfallowgenreservetoreserve:pam_parameter = new pam_parameter();
  tfallowgenreservetopicking:pam_parameter = new pam_parameter();
  tfallowgenreservetobulk:pam_parameter = new pam_parameter();
  tfallowgenbulktoreserve:pam_parameter = new pam_parameter();
  tfallowgenbulktopicking:pam_parameter = new pam_parameter();
  tfallowgenbulktobulk:pam_parameter = new pam_parameter();
  tfallowgenpickingtoreserve:pam_parameter = new pam_parameter();
  tfallowgenpickingtopicking:pam_parameter = new pam_parameter();
  tfallowgenpickingtobulk:pam_parameter = new pam_parameter();

  slallowgenreservetoreserve: lov = new lov();
  slallowgenreservetopicking: lov = new lov();
  slallowgenreservetobulk: lov = new lov();
  slallowgenbulktoreserve: lov = new lov();
  slallowgenbulktopicking: lov = new lov();
  slallowgenbulktobulk: lov = new lov();
  slallowgenpickingtoreserve: lov = new lov();
  slallowgenpickingtopicking: lov = new lov();
  slallowgenpickingtobulk: lov = new lov();

  //Date format
  public dateformat:string;
  public dateformatlong:string;
  public datereplan: Date | string | null;
  
    
  constructor(
    private sv: admparameterService,
    private av: authService, 
    private router: RouterModule,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService
  ) { 
    this.av.retriveAccess(); 
    this.dateformat = this.av.crProfile.formatdate;
    this.dateformatlong = this.av.crProfile.formatdatelong;
    this.lsunit.push({value : "MGU", desc : "Manage Unit", icon : "", valopnfirst : "", valopnsecond : '', valopnthird :'', valopnfour:''});
    this.lsunit.push({value : "SKU", desc : "SKU", icon : "", valopnfirst : "", valopnsecond : '', valopnthird :'', valopnfour:''});
    this.lsunit.push({value : "IPCK", desc : "IPCK", icon : "", valopnfirst : "", valopnsecond : '', valopnthird :'', valopnfour:''});
    this.lsunit.push({value : "PCK", desc : "PCK", icon : "", valopnfirst : "", valopnsecond : '', valopnthird :'', valopnfour:''});
    this.lsunit.push({value : "LAYER", desc : "Layer", icon : "", valopnfirst : "", valopnsecond : '', valopnthird :'', valopnfour:''});
    this.lsunit.push({value : "HU", desc : "Full HU", icon : "", valopnfirst : "", valopnsecond : '', valopnthird :'', valopnfour:''});
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ this.getMaster(); }

  getMaster() { 
    this.sv.find().pipe().subscribe(            
      (res) => { 
        this.pam = res.filter(x=>x.pmmodule == "inventory");

        //Correction stock 
        this.csallowblankremarks = this.pam.find(x=>x.pmtype == "correction" && x.pmcode == "allowblankremarks");
        this.csallowchangeunit = this.pam.find(x=>x.pmtype == "correction" && x.pmcode == "allowchangeunit");
        this.csallowblankrefereceno = this.pam.find(x=>x.pmtype == "correction" && x.pmcode == "allowblankrefereceno");
        this.csallowprintlabelonreserve = this.pam.find(x=>x.pmtype == "correction" && x.pmcode == "allowprintlabelonreserve");
        this.csallowgentaskfornewhu = this.pam.find(x=>x.pmtype == "correction" && x.pmcode == "allowgentaskfornewhu");
        this.csallowincludehubelongingtask = this.pam.find(x=>x.pmtype == "correction" && x.pmcode == "allowincludehubelongingtask");
		
        //Transfer stock
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowchangeunit").forEach(x=>{
          this.tfallowchangeunit = x; 
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenreservetoreserve").forEach(x=>{ 
          this.tfallowgenreservetoreserve = x; 
          this.slallowgenreservetoreserve = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenreservetopicking").forEach(x=> {
          this.tfallowgenreservetopicking = x; 
          this.slallowgenreservetopicking = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenreservetobulk").forEach(x=> { 
          this.tfallowgenreservetobulk = x;
          this.slallowgenreservetobulk = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenbulktoreserve").forEach(x=> {
          this.tfallowgenbulktoreserve = x ;
          this.slallowgenbulktoreserve = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenbulktopicking").forEach(x=> {
          this.tfallowgenbulktopicking = x;
          this.slallowgenbulktopicking = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenbulktobulk").forEach(x=> {
          this.tfallowgenbulktobulk = x;
          this.slallowgenbulktobulk = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenpickingtoreserve").forEach(x=>{ 
          this.tfallowgenpickingtoreserve = x;
          this.slallowgenpickingtoreserve = this.lsunit.find(e=>e.value == x.pmoption);
        })
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenpickingtopicking").forEach(x=> {
          this.tfallowgenpickingtopicking = x;
          this.slallowgenpickingtopicking = this.lsunit.find(e=>e.value == x.pmoption);
        });
        this.pam.filter(x=>x.pmtype == "transfer" && x.pmcode == "allowgenpickingtobulk").forEach(x=> {
          this.tfallowgenpickingtobulk = x;
          this.slallowgenpickingtobulk = this.lsunit.find(e=>e.value == x.pmoption);
        })
      
      },
      (err) => { this.toastr.error(err.error.message);  },
      () => { }
  );
   }
  save(o:pam_parameter) { 
      this.ngPopups.confirm('Please confirm to modify parameter ?')
      .subscribe(res => { 
       if (res) {
        this.sv.set(o).subscribe(
          (res) => {  this.toastr.success("<span class='fn-07e'>Modify success </span>",null,{ enableHtml : true });},
          (err) => {  this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });},
          () => {}
          );
        }else { 
          o.pmvalue = !o.pmvalue;
        }
      });
  }
  changeunit(o:pam_parameter,v:lov){ 
    this.ngPopups.confirm('Please confirm to modify unit parameter ?')
    .subscribe(res => { 
     if (res) {
      o.pmoption = v.value;
      this.sv.set(o).subscribe(
        (res) => {  this.toastr.success("<span class='fn-07e'>Modify success </span>",null,{ enableHtml : true });},
        (err) => {  this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });},
        () => {}
        );
      }else { 
        v = this.lsunit.find(e=>e.value == o.pmoption);
      }
    });
  }

}
