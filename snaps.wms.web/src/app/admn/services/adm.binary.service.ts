import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { shareService } from '../../share.service';
import { binary_md  } from '../models/adm.binary.model';
@Injectable({ providedIn: 'root' })
export class binaryService  {
    constructor(private http: HttpClient, private sv:shareService) { }


    desc(o:binary_md){
        return  this.http.post<binary_md[]>(environment.urlapiAdmn + "/binary/desc/"+this.sv.getId(), JSON.stringify(o))
                .pipe( map(res=><binary_md[]>res));
    }
    list(o:binary_md){
        return  this.http.post<binary_md[]>(environment.urlapiAdmn + "/binary/list/"+this.sv.getId(), JSON.stringify(o))
                .pipe(map(res=><binary_md[]>res));
    }
    upsert(o:binary_md){
        return  this.http.post(environment.urlapiAdmn + "/binary/upsert/"+this.sv.getId(), JSON.stringify(o))
                .pipe(map(res=>res));
    }
    remove(o:binary_md){
        return  this.http.post(environment.urlapiAdmn + "/binary/remove/"+this.sv.getId(), JSON.stringify(o))
                .pipe(map(res=>res));
    }

}