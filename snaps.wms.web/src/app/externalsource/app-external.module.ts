import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SharedModule } from '../share.module';
import { externalRoutingModule } from './app-external.routing';
import { extbarsource } from './components/barcode/extbar.source';
import { extBarcodeComponent } from './components/barcode/extbar.component';
import { DragDropDirective } from '../helpers/dragdrop.directive';
import { extordinbComponent } from './components/orderinbound/extordinb.component';
import { extordinbsource } from './components/orderinbound/extordinb.source';
import { extordoubComponent } from './components/orderoutbound/extordoub.component';
import { extordoubsource } from './components/orderoutbound/extordoub.source';
import { extproductComponent } from './components/product/extproduct.component';
import { extproductsource } from './components/product/extproduct.source';
import { extthirdpartyComponent } from './components/thirdparty/extthird.component';
import { extthirdpartysource } from './components/thirdparty/extthird.source';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OrderModule } from 'ngx-order-pipe';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { extordinlcomponent } from './components/orderinbound/extordinl.component';
import { extordinlsource } from './components/orderinbound/extordinl.source';
import { extordoulcomponent } from './components/orderoutbound/extordoul.component';
import { extordoulsource } from './components/orderoutbound/extordoul.source';
import { extprepssource } from './components/preps/extpreps.source';
import { extprepsComponent } from './components/preps/extpreps.component';

import { extlocationsource } from './components/loclower/extlocation.source';
import { extlocationComponent } from './components/loclower/extlocation.component';

import { extstoragesource } from './components/locupper/extstorage.source';
import { extstorageComponent } from './components/locupper/extstorage.component';
@NgModule({
  declarations: [ 
    DragDropDirective,
    extBarcodeComponent, extbarsource,
    extthirdpartyComponent,extthirdpartysource,
    extproductComponent, extproductsource,
    extordinbComponent, extordinbsource, extordinlcomponent, extordinlsource,
    extordoubComponent, extordoubsource, extordoulcomponent, extordoulsource,
    
    extprepssource,     extprepsComponent,
    extlocationsource,   extlocationComponent,
    extstoragesource,    extstorageComponent,

  ],
  imports: [
    NgSelectModule,
    FormsModule, ReactiveFormsModule,
    CommonModule, 
    externalRoutingModule,
    SharedModule,
    NgScrollbarModule,
    OrderModule, // sort data on table
    NgbModule,   
  ]
})
export class externalModule { }
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
} 