import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { tap,catchError } from 'rxjs/operators';
import { authService } from '../auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterModule } from '@angular/router';
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private sv: authService,
                private toastr: ToastrService, 
                private router: Router) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            tap(evt => {
                if (evt instanceof HttpResponse) {
                    //if(evt.body && evt.body.error)
                        //this.sal.danger(evt.body.error.message); 
                }
            }),
            catchError(err => {
            if ([401, 403].indexOf(err.status) !== -1) {
                // auto logout if 401 Unauthorized or 403 Forbidden response returned from api
                this.sv.signout();
                this.router.navigate(['/signin']);
            }
            const error = err.error.description || err.error.message || err.statusText;
            // this.toastr.error("<span class='fn-07e'>"+((error != undefined) ? error : err.error)+"</span>",null,{ enableHtml : true }); 
            console.log(((error != undefined) ? error : err.error));
            //console.log(err.error);
            return throwError(err.error);
        }))
    }
}