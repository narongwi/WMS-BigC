import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SharedModule } from '../share.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';

import { NgScrollbarModule } from 'ngx-scrollbar';
import { outboundRoutingModule } from './app-outbound.routing';
import { ouborderComponent } from './Components/order/oub.order';
import { oubprocessstockComponent } from './Components/processStock/oub.process.stock.landing';
import { oubpreparationComponent } from './Components/preparation/oub.preparation.landing';
import { ouballocateComponent } from './Components/allocate/oub.allocate';
import { oubrouteComponent } from './Components/routeplan/oub.route';
import { oubdeliveryComponent } from './Components/delivery/oub.delivery';
import { oubpreparationstockComponent } from './Components/preparation/oub.preparation.stocking';
import { oubpreparationdistComponent } from './Components/preparation/oub.preparation.distribute';
import { oubpreparationhumonComponent } from './Components/preparation/oub.preparation.humonitor';
import { oubhandlingunitComponent } from './Components/handlingunit/oub.handlingunit';

import { ChartsModule } from 'ng2-charts';
import { OrderModule } from 'ngx-order-pipe';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { oubprocessstockoperateComponent } from './Components/processStock/oub.process.stock.operate';
import { oubprocessstockselectionComponent } from './Components/processStock/oub.process.stock.selection';
import { oubprocessstocksummaryComponent } from './Components/processStock/oub.process.stock.summary';
import { oubhistoryComponent } from './Components/history/oub.history';

import { oubprocessdistComponent } from './Components/processDistribute/oub.process.dist.landing';
import { oubprocessdistoperateComponent } from './Components/processDistribute/oub.process.dist.operate';
import { oubprocessdistselectionComponent } from './Components/processDistribute/oub.process.dist.selection';
import { oubprocessdistsummaryComponent } from './Components/processDistribute/oub.process.dist.summary';

@NgModule({
  declarations: [
    //OnlynumberDirective,
    ouborderComponent,
    oubprocessdistComponent,
    oubprocessstockComponent,
    oubpreparationComponent,
    oubrouteComponent,
    ouballocateComponent,
    oubdeliveryComponent,
    oubpreparationstockComponent,
    oubpreparationdistComponent,
    oubpreparationhumonComponent,
    oubhandlingunitComponent,
    oubhistoryComponent,

    oubprocessstockoperateComponent,
    oubprocessstockselectionComponent,
    oubprocessstocksummaryComponent,

    oubprocessdistoperateComponent,
    oubprocessdistselectionComponent,
    oubprocessdistsummaryComponent
  ],
  imports: [
    NgSelectModule,
    FormsModule,
    CommonModule,
    outboundRoutingModule,
    SharedModule,
    NgScrollbarModule,
    ChartsModule,
    OrderModule, // sort data on table
    NgbModule,    
  ],
  providers : [DatePipe]
})
export class outboundModule { } 
