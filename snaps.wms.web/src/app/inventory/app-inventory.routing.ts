import { NgModule } from  '@angular/core';
import { Routes, RouterModule } from  '@angular/router';
import { invstockComponent } from './components/stockonhand/inv.stock';
import { invcorrectionComponent } from './components/correction/inv.correction'
import { invcountComponent } from './components/countstock/inv.count.landing';
import { invtransferComponent } from './components/transferstock/inv.transfer';
import { MergehuComponent } from './components/mergehu/mergehu.component';
const  routes:  Routes  = [
        {
            path:  'stockonhand',
            component: invstockComponent
        },
        {
            path:  'correction',
            component: invcorrectionComponent
        },
        {
            path: 'countstock',
            component: invcountComponent
        },
        { 
            path: 'transfer',
            component: invtransferComponent
        }
        ,
        { 
            path: 'mergehu',
            component: MergehuComponent
        }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class inventoryRoutingModule { }