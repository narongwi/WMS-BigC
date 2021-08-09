import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { authService } from '../auth/services/auth.service';
import { identifierModuleUrl } from '@angular/compiler';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    
    constructor(private sv: authService) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // if (this.asService.crAccn){ 
        //console.log(this.sv.isAuthen);
        if (this.sv.isAuthen){ 
            //For upload must be change content-type
             if (request.url.toLowerCase().indexOf("upload") >= 0) { 
                request = request.clone({
                    withCredentials: true,
                    setHeaders: {
                        Authorization: `Bearer ${this.sv.getToken}`,
                        'orgcode': `bgc`,
                        'accscode': `${this.sv.getKey}`,
                        'accncode': `${this.sv.crProfile.accncode}`,
                        'site': `${this.sv.crRole.site}`,
                        'depot': `${this.sv.crRole.depot}`,
                        'lang' : 'en'
                        //'Content-Type': 'multipart/form-data'
                    }
                });
            } else if (request.url.toLowerCase().indexOf("print") >= 0) {
                request = request.clone({
                    setHeaders: {
                        'Content-Type':'application/x-www-form-urlencoded; charset=utf-8'
                    }
                });
            }
            else { 
                request = request.clone({
                    withCredentials: true,
                    setHeaders: {
                        Authorization: `Bearer ${this.sv.getToken}`,
                        'orgcode': `bgc`,
                        'accscode': `${this.sv.getKey}`,
                        'accncode': `${this.sv.crProfile.accncode}`,
                        'site': `${this.sv.crRole.site}`,
                        'depot': `${this.sv.crRole.depot}`,
                        'lang' : 'en',
                        'Content-Type': 'application/json'
                    }
                });
            }
            return next.handle(request);
        }else { 
            request = request.clone({ 
                setHeaders: {'Content-Type': 'application/json', 
                responseType: "blob"} 
            });
            return next.handle(request);
        }
        //     const isApiUrl = request.url.startsWith(environment.urlapiAuth) || request.url.startsWith(environment.urlapiAuth);
        //     const isRepUrl = ""; //request.url.startsWith(environment.labUrl);
        //     const isBlob = (request.responseType == 'blob') ? true : false;        
        //     if (isLoggedIn && isApiUrl && !isBlob) {
        //         request = request.clone({
        //             withCredentials: true,
        //             setHeaders: {
        //                 Authorization: `Bearer ${crAccs.accscode}`,
        //                 'Content-Type': 'application/json'
        //             }
        //         });
        //     }else if (isApiUrl && isBlob) { 
        //         request = request.clone({
        //             withCredentials: true,
        //              setHeaders: { Authorization: `Bearer ${crAccs.accscode}` }
        //         });
        //     }else if (isRepUrl) { 
        //         request = request.clone({ setHeaders: { 'Content-Type': 'application/json' } });
        //     }else if (isLoggedIn) { 
        //         request = request.clone({
        //             withCredentials: true,
        //             setHeaders: {
        //                 Authorization: `Bearer ${crAccs.accscode}`,
        //                 'Content-Type': 'application/json'
        //             }
        //         });
        //     }else { 
        //         request = request.clone({
        //             setHeaders: {
        //                 'Content-Type': 'application/json',
        //                 //'Access-Control-Allow-Origin': environment.webUrl
        //             }
        //         });
        //     }
        //     return next.handle(request);
        // }else {

        //}

    }
}