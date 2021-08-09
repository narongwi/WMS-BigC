import { Component, OnInit,OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { pam_parameter, pam_route } from 'src/app/admn/models/adm.parameter.model';
import { admparameterService } from 'src/app/admn/services/adm.parameter.service';
import { authService } from '../../../../auth/services/auth.service';
import { lov } from '../../../../helpers/lov';
import { admwarehouseService } from '../../../services/adm.warehouse.service';

declare var $: any;
@Component({
  selector: 'app-admpamoutbound',
  templateUrl: 'adm.parameter.outbound.html'

})
export class admpamoutboundComponent implements OnInit {
  public lslog:lov[] = new Array();
  public fixval:boolean = true;
  public pam:pam_parameter[] = new Array();
  
  //Outbound 
  allowchangeofexsource: pam_parameter = new pam_parameter();
  allowchangespcdlc: pam_parameter = new pam_parameter();
  allowchangereqdate: pam_parameter = new pam_parameter();
  allowchangespcbatch: pam_parameter = new pam_parameter();
  allowcancel: pam_parameter = new pam_parameter();
  allowpartialship: pam_parameter = new pam_parameter();


  //Route
  allocatehuwhenprepdone:pam_parameter = new pam_parameter();
  allowrevisequantity:pam_parameter = new pam_parameter();


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
  ngAfterViewInit(){  this.getMaster(); }

  getMaster() { 
    this.sv.find().pipe().subscribe(            
      (res) => { 
        this.pam = res.filter(x=>x.pmmodule == "outbound");

        //Outbound order 
        this.allowchangeofexsource = this.pam.find(x=>x.pmtype == "order" && x.pmcode == "allowchangeofexsource");
        this.allowchangespcdlc = this.pam.find(x=>x.pmtype == "order" && x.pmcode == "allowchangespcdlc");
        this.allowchangereqdate = this.pam.find(x=>x.pmtype == "order" && x.pmcode == "allowchangereqdate");
        this.allowchangespcbatch = this.pam.find(x=>x.pmtype == "order" && x.pmcode == "allowchangespcbatch");
        this.allowcancel = this.pam.find(x=>x.pmtype == "order" && x.pmcode == "allowcancel");
        this.allowpartialship= this.pam.find(x=>x.pmtype == "order" && x.pmcode == "allowpartialship");
        //Allocate to Route
        this.allocatehuwhenprepdone = this.pam.find(x=>x.pmtype == "allocate" && x.pmcode == "allocatehuwhenprepdone");
        //Deliveiry
        this.allowrevisequantity = this.pam.find(x=>x.pmtype == "delivery" && x.pmcode == "allowrevisequantity");
      
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
        (res) => {  
          this.toastr.success("<span class='fn-07e'>Modify success </span>",null,{ enableHtml : true });
          this.getMaster() ;
        },
        (err) => {  this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });},
        () => {}
        );
      }else { 
        o.pmvalue = !o.pmvalue;
      }
    });
  }
}
