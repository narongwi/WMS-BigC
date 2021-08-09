import { NgModule } from  '@angular/core';
import { Routes, RouterModule } from  '@angular/router';
import { ouborderComponent } from './Components/order/oub.order';
import { oubprocessstockComponent } from './Components/processStock/oub.process.stock.landing';

import { oubpreparationComponent } from './Components/preparation/oub.preparation.landing';
import { oubrouteComponent } from './Components/routeplan/oub.route';
import { ouballocateComponent } from './Components/allocate/oub.allocate';
import { oubdeliveryComponent } from './Components/delivery/oub.delivery';
import { oubhandlingunitComponent } from './Components/handlingunit/oub.handlingunit';
import{ oubhistoryComponent } from './Components/history/oub.history';
import { oubprocessdistComponent } from './Components/processDistribute/oub.process.dist.landing';
const  routes:  Routes  = [
        {
            path:  'order',
            component: ouborderComponent
        },
        {
            path:   'stockProcess',
            component: oubprocessstockComponent
        },
        {
            path:   'distributeProcess',
            component: oubprocessdistComponent
        },
        {
          path: 'handlingunit',
          component: oubhandlingunitComponent
        },
        {
            path:   'preparation',
            component: oubpreparationComponent
        },
        {
            path:   'route',
            component: oubrouteComponent
        },
        {
            path:   'allocation',
            component: ouballocateComponent
        },
        {
            path:   'delivery',
            component: oubdeliveryComponent
        },
        {
            path:   'history',
            component: oubhistoryComponent
        }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class outboundRoutingModule { }
