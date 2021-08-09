import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { controllers } from 'chart.js';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { product_ls, product_md } from '../../models/adm.product.model';
import { adminService } from '../../services/account.service';
import { admbarcodeService } from '../../services/adm.barcode.service';
import { admproductService } from '../../services/adm.product.service';
import { admproductmodifyComponent } from './adm.product.modify';
declare var $: any;
@Component({
  selector: 'app-admproduct',
  templateUrl: 'adm.product.html'

})
export class admproductComponent implements OnInit {
  @ViewChild('modify') modify:admproductmodifyComponent;
  
  public lslog:lov[] = new Array();
  public crproduct:product_md = new product_md();
  public slproduct:product_ls;
  crtab:number = 1;
  slcproduct:string = "";
  constructor(
    private sv: admproductService,
    private bv: admbarcodeService,
    private av: authService,
    private mv: adminService, 
    private toastr: ToastrService,) { }
  ngOnInit(): void {  this.ngAllowRevise(); }
  ngOnDestroy():void {  }
  ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/ }
  setupJS() { 
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });   
  }
  getIcon(o:string){ return "";  }
  //toggle(){ $('.snapsmenu').click();  }
  ngAllowRevise() { 
    this.sv.parameter().pipe().subscribe((res)=> { 
      this.modify.allowchangedimension = res.find(e=>e.pmcode == "allowchangedimension").pmvalue;
      this.modify.allowchangehirachy = res.find(e=>e.pmcode == "allowchangehirachy").pmvalue;
      this.modify.allowchangedlc = res.find(e=>e.pmcode == "allowchangedlc").pmvalue;
      this.modify.allowchangeunit = res.find(e=>e.pmcode == "allowchangeunit").pmvalue;
     
    });
    this.bv.parameter().pipe().subscribe((res) => { 
      this.modify.allowchangebarcode = res.find(e=>e.pmcode == "allowchangeofexsource").pmvalue ;
    })
  }
  selproduct(o:product_ls){ 
    this.slcproduct = o.article;
    this.sv.get(o).pipe().subscribe(            
      (res) => { 
        this.crproduct = res; 
        this.modify.ngsetmodify(this.crproduct);
        this.crtab = 2;
      },
      (err) => { this.toastr.error(err.error.message); },
      () => { }
  );
  }

}
