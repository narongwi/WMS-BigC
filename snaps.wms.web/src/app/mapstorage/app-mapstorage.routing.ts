import { NgModule } from  '@angular/core';
import { Routes, RouterModule } from  '@angular/router';
import { mapsaisleComponent } from './components/aisle/maps.aisle';
import { mapsbayComponent } from './components/bay/maps.bay';
import { mapslevelComponent } from './components/level/maps.level';
import { mapslocationComponent } from './components/location/maps.location';
import { mapszoneComponent } from './components/zone/maps.zone';
import { mapspivotComponent } from './components/linepivot/maps.pivot';
import { mapspickingComponent } from './components/linepick_remove/maps.picking';
import { mapsgridComponent }  from './components/grid/maps.grid'
import { mapsbulkComponent } from './components/bulk/maps.bulk';
import { mapsreturnComponent } from './components/return/maps.return';
import { mapsinbstagingComponent } from './components/inbstaging/maps.inbstaging';
import { mapspickndropComponent } from './components/pickndrop/maps.pickndrop';
import { mapsdamageComponent } from './components/damage/maps.damage';
import { mapsoverflowComponent } from './components/overflow/maps.overflow';
import { mapsdspstagingComponent } from './components/dspstaging/maps.dspstaging';
import { mapsassemblyComponent } from './components/assembly/maps.assembly';
import { mapsforwardComponent } from './components/forwarding/maps.forwarding';
import { mapssinbinComponent } from './components/sinbin/maps.sinbin';
import { mapsprepzonestockComponent } from './components/preparationarea/stocking/wms.maps.storage.prep.zone.stock';
import { mapsprepzonedistComponent } from './components/preparationarea/distribution/wms.maps.storage.prep.zone.dist';
import { shareprepComponent } from './components/preparationshare/wms.maps.share.prep';
const  routes:  Routes  = [
        {
            path:  'zone',
            component:  mapszoneComponent
        },
        {
            path:  'aisle',
            component:  mapsaisleComponent
        },
        {
            path:  'bay',
            component:  mapsbayComponent
        },
        {
            path:  'level',
            component:  mapslevelComponent
        },
        {
            path:  'location',
            component:  mapslocationComponent
        },
        {
            path:  'pivotline',
            component:  mapspivotComponent
        },
        {
            path:  'zoneprep',
            component:  mapsprepzonestockComponent
        },
        {
            path:  'zonedist',
            component:  mapsprepzonedistComponent
        },
        {
            path:  'gridline',
            component:  mapsgridComponent
        },
        {
            path:  'fairshare',
            component:  shareprepComponent
        },
        {
            path:  'bulk',
            component:  mapsbulkComponent
        },
        {
            path:  'return',
            component:  mapsreturnComponent
        },
        {
            path:  'staging',
            component:  mapsinbstagingComponent
        },
        {
            path:  'pickndrop',
            component:  mapspickndropComponent
        },
        {   
            path:   'damage',
            component: mapsdamageComponent
        },
        {   path:   'overflow',
            component: mapsoverflowComponent
        },
        {   path:   'dispatch',
            component: mapsdspstagingComponent
        },
        {   path:   'assembly',
            component: mapsassemblyComponent
        },
        {   path: 'forwarding',
            component: mapsforwardComponent
        },
        {   path: 'sinbin',
            component: mapssinbinComponent
        }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class mapstorageRoutingModule { }