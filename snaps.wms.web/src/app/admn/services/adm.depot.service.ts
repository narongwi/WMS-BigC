import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { depot_ls, depot_md, depot_pm } from '../models/adm.depot.model';
import { shareService } from 'src/app/share.service';
@Injectable({ providedIn: 'root' })
export class admdepotService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient,private ss:shareService) { }

    find(o:depot_pm){
        return  this.http.post<depot_md[]>(environment.urlapiAdmn + "/admdepot/list/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><depot_md[]>res));
    }
    get(o:depot_ls){
        return  this.http.post<depot_md>(environment.urlapiAdmn + "/admdepot/get/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><depot_md>res));
    }
    upsert(o:depot_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/admdepot/upsert/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

}