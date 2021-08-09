import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { barcode_ls, barcode_md, barcode_pm } from '../models/adm.barcode.model';
import { pam_parameter } from '../models/adm.parameter.model';
@Injectable({ providedIn: 'root' })
export class admbarcodeService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(o:barcode_pm){
        return  this.http.post<barcode_ls[]>(environment.urlapiAdmn + "/admbarcode/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><barcode_ls[]>res));
    }
    get(o:barcode_ls){
        return  this.http.post<barcode_md>(environment.urlapiAdmn + "/admbarcode/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><barcode_md>res));
    }
    upsert(o:barcode_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admbarcode/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    setPrimary(o:barcode_ls){ 
        return this.http.post<resultRequest>(environment.urlapiAdmn + "/admbarcode/setprimary/"+this.gencode(), JSON.stringify(o))
        .pipe(map(res=><resultRequest>res));
    }
    parameter(){ 
        return  this.http.post<pam_parameter[]>(environment.urlapiAdmn + "/admparameter/getParameter/master/barcode/"+this.gencode(), JSON.stringify(""))
                .pipe(map(res=><pam_parameter[]>res));
    }

}