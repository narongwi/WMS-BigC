import { NgModule } from  '@angular/core';
import { Routes, RouterModule } from  '@angular/router';
import { taskhistoryComponent } from './Components/task.history/task.history';
import { taskmovementComponent } from './Components/task.movement/task.movement';

const  routes:  Routes  = [
        {
            path:  'process',
            component: taskmovementComponent
        },
        {
            path:   'history',
            component: taskhistoryComponent
        }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class TaskRoutingModule { }