import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccnComponent } from './components/accn/accn.component';
import { RoleComponent } from './components/role/role.component';
import { OrgaComponent } from './components/orga/orga.component';
import { admdeviceComponent } from './components/device/adm.device';
import { admbarcodeComponent } from './components/barcode/adm.barcode';
import { admthpartyComponent } from './components/thparty/adm.thparty';
import { admproductComponent } from './components/product/adm.product';
import { admwarehouseComponent } from './components/warehouse/adm.warehouse';
import { admparameterComponent } from './components/parameter/adm.parameter';
import { admlovComponent } from './components/lov/adm.lov';
import { tempreportComponent } from './components/tempreport/admtempreport';
import { admdepotComponent } from './components/depot/adm.depot';
import { PrinterComponent } from './components/printer/printer.component';
const routes: Routes = [
    {
        path: 'account',
        component: AccnComponent
    },
    {
        path: 'role',
        component: RoleComponent
    },
    {
        path: 'organization',
        component: OrgaComponent
    },
    {
        path: 'printer',
        component: PrinterComponent,
        //component: admdeviceComponent,
    },
    {
        path: 'barcode',
        component: admbarcodeComponent,
    },
    {
        path: 'thirdparty',
        component: admthpartyComponent,
    },
    {
        path: 'product',
        component: admproductComponent,
    },
    {
        path: 'warehouse',
        component: admwarehouseComponent,
    },
    {
        path: 'depot',
        component : admdepotComponent
    },
    {
        path: 'parameter',
        component: admparameterComponent
    }, {
        path: 'lov',
        component: admlovComponent
    }, {
        path: 'tempreport',
        component: tempreportComponent
    }
    , {
        path: 'orbitmap',
        component: tempreportComponent
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class admnRoutingModule { }