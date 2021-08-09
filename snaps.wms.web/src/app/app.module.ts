
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule} from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { ForgotComponent } from './auth/forgot/forgot.component';
import { RecoveryComponent } from './auth/recovery/recovery.component';
import { JwtInterceptor } from './helpers/InterceptHttp';
import { LaunchComponent } from './main/launch/launch.component';
import { ProfileComponent } from './main/profile/profile.component';
import { NavigateComponent } from './main/navigate/navigate.component';
import { PagenotfoundComponent } from './main/pagenotfound/pagenotfound.component';
import { NgProgressModule } from 'ngx-progressbar';
import { NgProgressHttpModule } from 'ngx-progressbar/http';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {TranslateHttpLoader} from '@ngx-translate/http-loader';

import { NgSelectModule } from '@ng-select/ng-select';
import { ToastrModule } from 'ngx-toastr';
import { NgPopupsModule } from 'ng-popups';
import { SharedModule } from './share.module';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { ChartsModule } from 'ng2-charts';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ErrorInterceptor } from './helpers/InterceptError';
// import { NavenablePipe } from './main/navigate/navenable.pipe';
// import { ModuleenablePipe } from './main/navigate/moduleenable.pipe';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
    ForgotComponent,
    RecoveryComponent,
    LaunchComponent,
    ProfileComponent,
    NavigateComponent,
    PagenotfoundComponent,
  ],
  imports: [
    BrowserModule,
    NgSelectModule, 
    FormsModule,
    BrowserAnimationsModule,
    AppRoutingModule,        
    HttpClientModule,
    // ngprogress with http
    // progressbar config
    NgProgressModule.withConfig({
      ease: 'linear',
      speed: 200,
      trickleSpeed: 600,
      meteor: true,
      spinner: true,
      spinnerPosition: 'right',
      direction: 'ltr+',
      color: '#BC4749',
      thick: true,
    }),
    NgProgressHttpModule,
    SharedModule,
    TranslateModule.forRoot({
      loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
      }
    }),
    ToastrModule.forRoot({
      timeOut: 1500,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
      closeButton: true,
      progressBar: true,
      progressAnimation:'decreasing',
      enableHtml : true
    }),

    NgPopupsModule.forRoot({
      theme: 'dark', // available themes: 'default' | 'material' | 'dark')
    }),
    
    NgScrollbarModule,
    ChartsModule, //Chart module
    NgbModule    
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],

  bootstrap: [AppComponent]
})
export class AppModule { }

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}
