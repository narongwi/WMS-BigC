import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccnComponent } from './components/accn/accn.component';
import { RoleComponent } from './components/role/role.component';
import { OrgaComponent } from './components/orga/orga.component';
import { admnRoutingModule } from './app-admn.routing';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { listaccount } from './components/accn/accn-list';
import { opraccount } from './components/accn/accn-ope';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SharedModule } from '../share.module';
import { listrole } from './components/role/role-list';
import { oprrole } from './components/role/role-opr';
import { admdeviceComponent } from './components/device/adm.device';
import { admdevicelineComponent } from './components/device/adm.device.line';
import { admbarcodeComponent } from './components/barcode/adm.barcode';
import { admbarcodelineComponent } from './components/barcode/adm.barcode.line';

import { admthpartyComponent } from './components/thparty/adm.thparty';
import { admthpartylineComponent } from './components/thparty/adm.thparty.line';
import { admthpartymodifyComponent } from './components/thparty/adm.thparty.modify';

import { admproductComponent } from './components/product/adm.product';
import { admproductlineComponent } from './components/product/adm.product.line';
import { admproductmodifyComponent } from './components/product/adm.product.modify';

import { admwarehouseComponent } from './components/warehouse/adm.warehouse';
import { admwarehouselineComponent } from './components/warehouse/adm.warehouse.line';
import { admparameterComponent } from './components/parameter/adm.parameter';
import { admlovComponent } from './components/lov/adm.lov';
import { NgScrollbarModule } from 'ngx-scrollbar';

import { admpaminboundComponent } from './components/parameter/Inbound/adm.parameter.inbound';
import { admpamtaskmoveComponent } from './components/parameter/taskmove/adm.parameter.taskmove';
import { admpammasterComponent } from './components/parameter/Master/adm.parameter.master';
import { admpaminventoryComponent } from './components/parameter/Inventory/adm.parameter.inventory';
import { admpamoutboundComponent } from './components/parameter/Outbound/adm.parameter.outbound';
import { admpampreparationComponent } from './components/parameter/Preparation/adm.parameter.preparation';
import { OrderModule } from 'ngx-order-pipe';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { tempreportComponent } from './components/tempreport/admtempreport';
// import { admTableFilter } from "./components/tempreport/admTableFilter";
import { admdepotComponent } from './components/depot/adm.depot';
import { PrinterComponent } from './components/printer/printer.component';
import { OptionComponent } from "./components/printer/option/option.component";

@NgModule({
  declarations: [
    AccnComponent, RoleComponent, OrgaComponent, listaccount, opraccount, listrole, oprrole,
    admbarcodeComponent, admbarcodelineComponent,
    admproductComponent, admproductlineComponent, admproductmodifyComponent,
    admwarehouseComponent, admwarehouselineComponent, admdepotComponent,
    admdeviceComponent, admdevicelineComponent, admthpartyComponent,
    admthpartylineComponent, admthpartymodifyComponent,
    admparameterComponent,
    admlovComponent,

    admpaminboundComponent, admpamtaskmoveComponent, admpammasterComponent, admpaminventoryComponent,
    admpamoutboundComponent, admpampreparationComponent,

    tempreportComponent,
    // admTableFilter,
    PrinterComponent,
    OptionComponent
  ],
  imports: [
    NgSelectModule,
    FormsModule,
    CommonModule,
    admnRoutingModule,
    SharedModule,
    NgScrollbarModule,
    OrderModule, // sort data on table
    NgbModule,
  ]
})
export class admnModule { }
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}  