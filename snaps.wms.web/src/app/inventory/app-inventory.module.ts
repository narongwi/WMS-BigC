import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SharedModule } from '../share.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { invstockComponent } from './components/stockonhand/inv.stock';
import { inventoryRoutingModule } from './app-inventory.routing';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { invcorrectionComponent } from './components/correction/inv.correction';
import { invcountaskComponent } from './components/countstock/inv.count.task';
import { invtransferComponent } from './components/transferstock/inv.transfer';
import { invcounplanComponent } from './components/countstock/inv.count.plan';
import { invcountlineComponent } from './components/countstock/inv.count.line';
import { InvcountConfirmComponent } from './components/countstock/inv.count.confirm';

import { OrderModule } from 'ngx-order-pipe';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { invcountComponent } from './components/countstock/inv.count.landing';
import { NgxMaskModule } from 'ngx-mask';

@NgModule({
  declarations: [ 
    invstockComponent, 
    invcorrectionComponent, 
    invcountaskComponent,
    invtransferComponent,
    invcounplanComponent, 
    invcountlineComponent,
    invcountComponent,
    InvcountConfirmComponent,
  ],
  imports: [
    NgSelectModule,
    FormsModule,
    CommonModule,
    inventoryRoutingModule,
    SharedModule,
    NgScrollbarModule,
    OrderModule, // sort data on table
    NgbModule,    
    NgxMaskModule.forRoot({
      showMaskTyped : true,
      clearIfNotMatch : true
    })
  ],
  providers : [DatePipe]
})
export class inventoryModule { } 