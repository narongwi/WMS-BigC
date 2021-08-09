import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SharedModule } from '../share.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';

import { InboundRoutingModule } from './app-inbound.routing';
import { inborderComponent } from './components/Order/inb.order';
import { inborderlineComponent } from './components/Order/inb.order.line';
import { inborderreceiptComponent } from './components/Order/inb.order.receipt';
import { inbhistoryComponent } from './components/History/inb.history';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { OrderModule } from 'ngx-order-pipe';
import {  NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OnlynumberDirective } from '../helpers/numericonly.directive';
@NgModule({
  declarations: [
    inborderComponent,inborderlineComponent,inborderreceiptComponent,
    inbhistoryComponent
  ],
  imports: [
    NgSelectModule,
    FormsModule,
    CommonModule,
    InboundRoutingModule,
    SharedModule,
    NgScrollbarModule,
    OrderModule, // sort data on table
    NgbModule,    
  ],
  providers : [DatePipe]
})
export class InboundModule { } 