import { NgModule } from  '@angular/core';
import { Routes, RouterModule } from  '@angular/router';
import { inbhistoryComponent } from './components/History/inb.history';
import {  inborderComponent } from './components/Order/inb.order';
const  routes:  Routes  = [
        {
            path:  'order',
            component: inborderComponent
        },
        {
            path:   'history',
            component: inbhistoryComponent
        }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class InboundRoutingModule { }