import { NgModule } from  '@angular/core';
import { Routes, RouterModule } from  '@angular/router';
import { extBarcodeComponent } from './components/barcode/extbar.component';
import { extlocationComponent } from './components/loclower/extlocation.component';
import { extlocationsource } from './components/loclower/extlocation.source';
import { extstorageComponent } from './components/locupper/extstorage.component';
import { extstoragesource } from './components/locupper/extstorage.source';
import { extordinbComponent } from './components/orderinbound/extordinb.component';
import { extordoubComponent } from './components/orderoutbound/extordoub.component';
import { extprepsComponent } from './components/preps/extpreps.component';
import { extproductComponent } from './components/product/extproduct.component';
import { extthirdpartyComponent } from './components/thirdparty/extthird.component';
const  routes:  Routes  = [
        {
            path:  'barcode',
            component:  extBarcodeComponent
        },
        {
            path:  'product',
            component:  extproductComponent
        },
        {
            path:  'thirdparty',
            component:  extthirdpartyComponent
        },
        {
            path:  'inbound',
            component:  extordinbComponent
        },
        {
            path:  'outbound',
            component:  extordoubComponent
        },
        {
            path:  'Location',
            component:  extlocationComponent
        },
        {
            path:  'Storage',
            component:  extstorageComponent
        },
        {
            path:  'preps',
            component:  extprepsComponent
        }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class externalRoutingModule { }