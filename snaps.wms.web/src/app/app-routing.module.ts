import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { LaunchComponent } from './main/launch/launch.component';
import { InboundModule } from './inbound/app-inbound.module';
import { AuthGuard } from './helpers/auth.guard';
import { PagenotfoundComponent } from './main/pagenotfound/pagenotfound.component';
import { ProfileComponent } from './main/profile/profile.component';

const routes: Routes = [
  // { path: 'mobiletask' , component : taskmobileComponent }, //hardcode
  // { path: 'mobilereceipt', component: inbmobileComponent }, //hardcode
  // { path: 'mobilecount', component: invcountmobileComponent }, //hardcode
  // { path: 'mobilepick', component: oubmobilepickComponent },//hardcode
  // { path: 'mobiledist', component: oubmobiledistComponent },//hardcode
  { path : 'Luanch',  canActivate : [AuthGuard], component: LaunchComponent  },
  { path : 'Inbound', canActivate : [AuthGuard], component: LaunchComponent,
    children: [ {
        path:  '',
        loadChildren: () => import('./inbound/app-inbound.module').then(m => m.InboundModule)
      }
    ]  
  },

  { path :'Admin', canActivate : [AuthGuard], component : LaunchComponent, 
    children : [ {
        path : '',
        loadChildren: () => import('./admn/app-admn.module').then(m=>m.admnModule)
      }
    ]
  },

  { path :'External', canActivate : [AuthGuard], component : LaunchComponent, 
    children : [ {
        path : '',
        loadChildren: () => import('./externalsource/app-external.module').then(m=>m.externalModule)
      }
    ]
  },

  { path :'Mapstorage', canActivate : [AuthGuard], component : LaunchComponent, 
    children : [ {
        path : '',
        loadChildren: () => import('./mapstorage/app-mapstorage.module').then(m=>m.mapstorageModule)
      }
    ]
  },

  { path :'Task', canActivate : [AuthGuard], component : LaunchComponent, 
    children : [ {
        path : '',
        loadChildren: () => import('./task/app-task.module').then(m=>m.TaskModule)
      }
    ]
  },

  { path :'Inventory', canActivate : [AuthGuard], component : LaunchComponent, 
    children : [ {
        path : '',
        loadChildren: () => import('./inventory/app-inventory.module').then(m=>m.inventoryModule)
      }
    ]
  },
  
  { path :'Outbound', canActivate : [AuthGuard], component : LaunchComponent, 
    children : [{
        path : '',
        loadChildren: () => import('./outbound/app-outbound.module').then(m=>m.outboundModule)
      }
    ]
  },
  { path: 'profile', component: LaunchComponent, canActivate : [AuthGuard], 
    children : [{ path: 'my', component: ProfileComponent }]
   },
  { path: 'signin', component: LoginComponent },
  { path: 'signup', component: SignupComponent },

  { path: '' , redirectTo : 'Luanch', pathMatch: 'full' },
  { path: '**', component: PagenotfoundComponent },
];

@NgModule({
  imports: [CommonModule, RouterModule.forRoot(routes,{ enableTracing: false}), InboundModule],
  exports: [RouterModule]
})
export class AppRoutingModule { }
