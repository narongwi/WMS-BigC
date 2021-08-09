import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SharedModule } from '../share.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { taskhistoryComponent } from './Components/task.history/task.history';
import { taskmovementComponent } from './Components/task.movement/task.movement';
import { taskprocessComponent } from './Components/task.movement/task.process';
import { TaskRoutingModule } from './app-task.routing';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OrderModule } from 'ngx-order-pipe';



@NgModule({
  declarations: [ 
    taskhistoryComponent,
    taskmovementComponent,
    taskprocessComponent
  ],
  imports: [
    NgSelectModule,
    FormsModule,
    CommonModule,
    TaskRoutingModule,
    SharedModule,
    NgScrollbarModule,
    OrderModule, // sort data on table
    NgbModule,    
  ],
  providers : [DatePipe]
})
export class TaskModule { } 