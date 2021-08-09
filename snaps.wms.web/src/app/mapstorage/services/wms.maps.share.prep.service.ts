import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { zoneprep_md, zoneprln_md } from '../Models/wms.maps.storage.prep.zone.model';
import { shareService } from '../../share.service';
import { shareprep_md, shareprln_md } from '../Models/wms.maps.share.prep.model';

@Injectable({ providedIn: 'root' })
export class shareprepService {
    callID:string = "";
    u:string = "";
    constructor(private http: HttpClient, private ss:shareService ) {}

    list(o:shareprep_md) { 
        return  this.http.post<shareprep_md[]>(environment.urlapiAdmn + "/shareprep/list/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><shareprep_md[]>res));
    }

    get(o:shareprep_md){ 
        return  this.http.post<shareprep_md>(environment.urlapiAdmn + "/shareprep/get/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><shareprep_md>res));
    }

    upsert(o:shareprep_md){ 
        return  this.http.post(environment.urlapiAdmn + "/shareprep/upsert/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=>res));
    }

    remove(o:shareprep_md) { 
        return  this.http.post<shareprep_md>(environment.urlapiAdmn + "/shareprep/remove/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><shareprep_md>res));
    }

    upline(o:shareprln_md){ 
        return  this.http.post(environment.urlapiAdmn + "/shareprep/upline/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=>res));
    }
    rmline(o:shareprln_md){ 
        return  this.http.post(environment.urlapiAdmn + "/shareprep/rmline/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=>res));
    }

}