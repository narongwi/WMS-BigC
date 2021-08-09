import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { admdevice_ls, admdevice_md, admdevice_pm } from '../models/adm.device.model';
@Injectable({ providedIn: 'root' })
export class admdeviceService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(o:admdevice_pm){
        return  this.http.post<admdevice_ls[]>(environment.urlapiAdmn + "/admdevice/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><admdevice_ls[]>res));
    }
    get(o:admdevice_ls){
        return  this.http.post<admdevice_md>(environment.urlapiAdmn + "/admdevice/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><admdevice_md>res));
    }
    upsert(o:admdevice_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admdevice/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

}