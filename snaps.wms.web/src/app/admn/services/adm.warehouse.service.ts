import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { warehouse_ls, warehouse_md, warehouse_pm } from '../models/adm.warehouse.model';
import { shareService } from 'src/app/share.service';
@Injectable({ providedIn: 'root' })
export class admwarehouseService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient, private ss:shareService) { }
    find(o:warehouse_pm){
        return  this.http.post<warehouse_md[]>(environment.urlapiAdmn + "/admwarehouse/list/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><warehouse_md[]>res));
    }
    get(o:warehouse_ls){
        return  this.http.post<warehouse_md>(environment.urlapiAdmn + "/admwarehouse/get/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><warehouse_md>res));
    }
    upsert(o:warehouse_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admwarehouse/upsert/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    remove(o:warehouse_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admwarehouse/remove/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

}