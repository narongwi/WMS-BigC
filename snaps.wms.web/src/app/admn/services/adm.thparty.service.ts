import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { thparty_ls, thparty_md, thparty_pm } from '../models/adm.thparty.model';
import { pam_parameter } from '../models/adm.parameter.model';
@Injectable({ providedIn: 'root' })
export class admthpartyService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(o:thparty_pm){
        return  this.http.post<thparty_ls[]>(environment.urlapiAdmn + "/admthparty/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><thparty_ls[]>res));
    }
    get(o:thparty_ls){
        return  this.http.post<thparty_md>(environment.urlapiAdmn + "/admthparty/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><thparty_md>res));
    }
    upsert(o:thparty_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admthparty/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    parameter(){ 
        return  this.http.post<pam_parameter[]>(environment.urlapiAdmn + "/admparameter/getParameter/master/thirdparty/"+this.gencode(), JSON.stringify(""))
                .pipe(map(res=><pam_parameter[]>res));
    }

}