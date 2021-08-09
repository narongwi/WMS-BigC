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
  selector: 'app-admpamtaskmove',
  templateUrl: 'adm.parameter.taskmove.html'

})
export class admpamtaskmoveComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public pam:pam_parameter[] = new Array();

  //Task putaway
  tpallowscanhuongrap:pam_parameter = new pam_parameter();
  tpallowautoassign:pam_parameter = new pam_parameter();  
  tpallowscansourcelocation:pam_parameter = new pam_parameter();
  tpallowscanbarcode:pam_parameter = new pam_parameter();
  tpallowinputqtyongrap:pam_parameter = new pam_parameter();
  tpallowpickndrop:pam_parameter = new pam_parameter();
  tpallowcheckdigit:pam_parameter = new pam_parameter();
  tpallowfullygrap:pam_parameter = new pam_parameter();
  tpallowfullycollect:pam_parameter = new pam_parameter();
  tpallowchangetarget:pam_parameter = new pam_parameter();  
  tpallowputawaypicking : pam_parameter = new pam_parameter();

  //Task approach / full pallet
  taallowchangeworker:pam_parameter = new pam_parameter();
  taallowscanhuno:pam_parameter = new pam_parameter();
  taallowautoassign:pam_parameter = new pam_parameter();
  taallowscansourcelocation:pam_parameter = new pam_parameter();
  taallowscanbarcode:pam_parameter = new pam_parameter();
  taallowchangequantity:pam_parameter = new pam_parameter();
  taallowpickndrop:pam_parameter = new pam_parameter();
  taallowcheckdigit:pam_parameter = new pam_parameter();
  taallowfullycollect:pam_parameter = new pam_parameter();
  taallowchangetarget:pam_parameter = new pam_parameter();

  //Replenishment 
	trallowmanual: pam_parameter = new pam_parameter();
	trallowchangeworker: pam_parameter = new pam_parameter();
	trallowscanhuno: pam_parameter = new pam_parameter();
	trallowscanbarcode: pam_parameter = new pam_parameter();
	trallowautoassign: pam_parameter = new pam_parameter();
	trallowscansourcelocation: pam_parameter = new pam_parameter();
	trallowchangequantity: pam_parameter = new pam_parameter();
	trallowpickndrop: pam_parameter = new pam_parameter();
	trallowcheckdigit: pam_parameter = new pam_parameter();
	trallowfullycollect: pam_parameter = new pam_parameter();

  //transfer 
  tfallowmanual: pam_parameter = new pam_parameter();
  tfallowchangeworker: pam_parameter = new pam_parameter();
  tfallowscanhuno: pam_parameter = new pam_parameter();
  tfallowautoassign: pam_parameter = new pam_parameter();
  tfallowscansourcelocation: pam_parameter = new pam_parameter();
  tfallowscanbarcode: pam_parameter = new pam_parameter();
  tfallowchangequantity: pam_parameter = new pam_parameter();
  tfallowpickndrop: pam_parameter = new pam_parameter();
  tfallowcheckdigit: pam_parameter = new pam_parameter();
  tfallowfullycollect: pam_parameter = new pam_parameter();
  tfallowchangetarget: pam_parameter = new pam_parameter();

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
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ this.getMaster(); }

    getMaster() { 
    this.sv.find().pipe().subscribe(            
      (res) => { 
        this.pam = res.filter(x=>x.pmmodule == "task");
        //Task putaway
        this.tpallowscanhuongrap = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowscanhuongrap");
        this.tpallowautoassign = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowautoassign");  
        this.tpallowscansourcelocation = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowscansourcelocation");
        this.tpallowscanbarcode = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowscanbarcode");
        this.tpallowinputqtyongrap = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowinputqtyongrap");
        this.tpallowpickndrop = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowpickndrop");
        this.tpallowcheckdigit = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowcheckdigit");
        this.tpallowfullygrap = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowfullygrap");
        this.tpallowfullycollect = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowfullycollect");
        this.tpallowchangetarget = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowchangetarget");  
        this.tpallowputawaypicking = this.pam.find(x=>x.pmtype == "putaway" && x.pmcode == "allowputawaypicking");  
        
        //Task approach / full pallet
        this.taallowchangeworker = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowchangeworker");
        this.taallowscanhuno = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowscanhuno");
        this.taallowautoassign = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowautoassign");
        this.taallowscansourcelocation = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowscansourcelocation");
        this.taallowscanbarcode = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowscanbarcode");
        this.taallowchangequantity = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowchangequantity");
        this.taallowpickndrop = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowpickndrop");
        this.taallowcheckdigit = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowcheckdigit");
        this.taallowfullycollect = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowfullycollect");
        this.taallowchangetarget = this.pam.find(x=>x.pmtype == "approach" && x.pmcode == "allowchangetarget");
        
        //Replenishment 
        this.trallowmanual  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowmanual");
        this.trallowchangeworker  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowchangeworker");
        this.trallowscanhuno  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowscanhuno");
        this.trallowscanbarcode  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowscanbarcode");
        this.trallowautoassign  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowautoassign");
        this.trallowscansourcelocation  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowscansourcelocation");
        this.trallowchangequantity  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowchangequantity");
        this.trallowpickndrop  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowpickndrop");
        this.trallowcheckdigit  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowcheckdigit");
        this.trallowfullycollect  = this.pam.find(x=>x.pmtype == "replenishment" && x.pmcode == "allowfullycollect");
        
        //transfer 
        this.tfallowmanual  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowmanual");
        this.tfallowchangeworker  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowchangeworker");
        this.tfallowscanhuno  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowscanhuno");
        this.tfallowautoassign  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowautoassign");
        this.tfallowscansourcelocation  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowscansourcelocation");
        this.tfallowscanbarcode  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowscanbarcode");
        this.tfallowchangequantity  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowchangequantity");
        this.tfallowpickndrop  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowpickndrop");
        this.tfallowcheckdigit  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowcheckdigit");
        this.tfallowfullycollect  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowfullycollect");
        this.tfallowchangetarget  = this.pam.find(x=>x.pmtype == "transfer" && x.pmcode == "allowchangetarget");

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

}
