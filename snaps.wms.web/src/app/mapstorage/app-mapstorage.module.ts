import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedModule } from '../share.module';
import { mapstorageRoutingModule } from './app-mapstorage.routing';
import { mapsaisleComponent } from './components/aisle/maps.aisle';
import { mapsaisleline } from './components/aisle/maps.aisle.line';
import { mapszoneComponent } from './components/zone/maps.zone';
import { mapszoneline } from './components/zone/maps.zone.line';
import { mapsbayComponent } from './components/bay/maps.bay';
import { mapsbayline } from './components/bay/maps.bay.line';
import { mapslevelComponent } from './components/level/maps.level';
import { mapslevelline } from './components/level/maps.level.line';
import { mapslocationComponent } from './components/location/maps.location';
import { mapslocationline } from './components/location/maps.location.line';
import { mapslocationmodify } from './components/location/maps.location.modify';
import { mapslocationgenerate } from './components/location/maps.location.generate';
import { mapspivotComponent } from './components/linepivot/maps.pivot';
import { mapspickingComponent }  from './components/linepick_remove/maps.picking';
import { mapsgridComponent }  from './components/grid/maps.grid';
import { mapsgridline }  from './components/grid/maps.grid.line';
import { mapsgridgenerate }  from './components/grid/maps.grid.generate';
import { mapsgridmodify }  from './components/grid/maps.grid.modify';
import { mapsbulkComponent } from './components/bulk/maps.bulk';
import { mapsbulklineComponent } from './components/bulk/maps.bulk.line';
import { mapsreturnComponent } from './components/return/maps.return';
import { mapsreturnlineComponent } from './components/return/maps.return.line';
import { mapsinbstagingComponent } from './components/inbstaging/maps.inbstaging';
import { mapsinbstaginglineComponent } from './components/inbstaging/maps.inbstaging.line';
import { mapspickndropComponent } from './components/pickndrop/maps.pickndrop';
import { mapspickndroplineComponent } from './components/pickndrop/maps.pickndrop.line';
import { mapsdamageComponent } from './components/damage/maps.damage';
import { mapsdamagelineComponent } from './components/damage/maps.damage.line';
import { mapsoverflowComponent } from './components/overflow/maps.overflow';
import { mapsoverflowlineComponent } from './components/overflow/maps.overflow.line';
import { mapsdspstagingComponent } from './components/dspstaging/maps.dspstaging';
import { mapsdspstaginglineComponent } from './components/dspstaging/maps.dspstaging.line';
import { mapsassemblyComponent } from './components/assembly/maps.assembly';
import { mapsassemblylineComponent } from './components/assembly/maps.assembly.line';
import { mapsforwardComponent } from './components/forwarding/maps.forwarding';
import { mapsforwardlineComponent } from './components/forwarding/maps.forwarding.line';
import { mapssinbinComponent } from './components/sinbin/maps.sinbin';
import { mapssinbinlineComponent } from './components/sinbin/maps.sinbin.line';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { ChartsModule } from 'ng2-charts';
import { OrderModule } from 'ngx-order-pipe';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { mapsprepzonestockComponent } from './components/preparationarea/stocking/wms.maps.storage.prep.zone.stock';
import { mapsprepzonestockareaComponent } from './components/preparationarea/stocking/zonestockarea/wms.maps.storage.prep.zone.stock.area';
import { mapsprepzonestocklineComponent } from './components/preparationarea/stocking/linestockarea/wms.maps.storage.prep.zone.stock.line';
import { mapsprepzonedistComponent } from './components/preparationarea/distribution/wms.maps.storage.prep.zone.dist';
import { mapsprepzonedistareaComponent } from './components/preparationarea/distribution/zonedistarea/wms.maps.storage.prep.zone.dist.area';
import { mapsprepzonedistlineComponent } from './components/preparationarea/distribution/linedistarea/wms.maps.storage.prep.zone.dist.line';
import { shareprepComponent} from './components/preparationshare/wms.maps.share.prep';
import { sharepreplnComponent } from './components/preparationshare/lineshareprep/wms.maps.share.prep.line';

@NgModule({
  declarations: [ 
    mapsaisleComponent, mapsaisleline,
    mapszoneComponent, mapszoneline,
    mapsbayComponent, mapsbayline,
    mapslevelComponent, mapslevelline,
    mapslocationComponent, mapslocationline,mapslocationgenerate,mapslocationmodify,
    mapspivotComponent, mapspickingComponent,
    mapsgridComponent, mapsgridline,mapsgridgenerate,mapsgridmodify,
    mapsbulkComponent,mapsbulklineComponent,
    mapsreturnComponent, mapsreturnlineComponent,
    mapsinbstagingComponent, mapsinbstaginglineComponent,    
    mapspickndropComponent, mapspickndroplineComponent,    
    mapsdamageComponent, mapsdamagelineComponent,   
    mapsoverflowComponent, mapsoverflowlineComponent,
    mapsdspstagingComponent, mapsdspstaginglineComponent, 
    mapsassemblyComponent, mapsassemblylineComponent, 
    mapsforwardComponent, mapsforwardlineComponent, 
    mapssinbinComponent, mapssinbinlineComponent,


    //Zone Preparation 
    mapsprepzonestockComponent,mapsprepzonestockareaComponent,mapsprepzonestocklineComponent,
    mapsprepzonedistComponent,mapsprepzonedistareaComponent,mapsprepzonedistlineComponent,

    //Share preparation
    shareprepComponent, sharepreplnComponent
  ],
  imports: [
    NgSelectModule,
    FormsModule,
    CommonModule,
    mapstorageRoutingModule,
    SharedModule,
    NgScrollbarModule,
    ChartsModule,
    OrderModule, // sort data on table
    NgbModule
  ],
  providers : [DatePipe]
})
export class mapstorageModule { }
//export function HttpLoaderFactory(http: HttpClient) { return new TranslateHttpLoader(http); } 