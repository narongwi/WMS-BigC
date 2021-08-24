import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { UiSwitchModule } from 'ngx-ui-switch';
import { FormsModule } from '@angular/forms';
import { OrderModule } from 'ngx-order-pipe';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OnlynumberDirective } from './helpers/numericonly.directive';
import { admTableFilter } from './admn/components/tempreport/admTableFilter';

export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http);
  }
    
  @NgModule({
    declarations: [ OnlynumberDirective,admTableFilter ],
    imports: [
      CommonModule,
      TranslateModule.forChild({
          loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        },
        isolate: false
      }),
      UiSwitchModule,
      OrderModule, // sort data on table
      NgbModule,
      
    ],
    exports: [
      TranslateModule,
      UiSwitchModule, 
      OnlynumberDirective,
      NgbModule,
      admTableFilter,
    ],
  })
  export class SharedModule { }