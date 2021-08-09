import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { product_at, product_ls, product_md, product_pm } from '../models/adm.product.model';
import { pam_parameter } from '../models/adm.parameter.model';
@Injectable({ providedIn: 'root' })
export class admproductService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }
    
    find(o:product_pm){
        return  this.http.post<product_ls[]>(environment.urlapiAdmn + "/admproduct/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><product_ls[]>res));
    }
    active(o:product_pm){
        return  this.http.post<product_at>(environment.urlapiAdmn + "/admproduct/active/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><product_at>res));
    }
    get(o:product_ls){
        return  this.http.post<product_md>(environment.urlapiAdmn + "/admproduct/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><product_md>res));
    }
    upsert(o:product_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admproduct/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    parameter(){ 
        return  this.http.post<pam_parameter[]>(environment.urlapiAdmn + "/admparameter/getParameter/master/product/"+this.gencode(), JSON.stringify(""))
                .pipe(map(res=><pam_parameter[]>res));
    }
    

}