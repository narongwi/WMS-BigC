import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable, VirtualTimeScheduler } from 'rxjs';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { stringify } from 'querystring';
import { zoneprep_md, zoneprln_md } from '../Models/wms.maps.storage.prep.zone.model';



@Injectable({ providedIn: 'root' })
export class mapsprepzonestockService {
    callID:string = "";
    u:string = "";
    private gencode():string { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), 'snaps-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }
    constructor(private http: HttpClient) { this.callID = this.gencode(); }

    prepzonelist(o:zoneprep_md) { 
        return  this.http.post<zoneprep_md[]>(environment.urlapiAdmn + "/zoneprep/list/"+this.callID, JSON.stringify(o))
                .pipe( map(res=><zoneprep_md[]>res));
    }

    prepzoneupsert(o:zoneprep_md){ 
        return  this.http.post(environment.urlapiAdmn + "/zoneprep/ops/upsert/"+this.callID, JSON.stringify(o))
                .pipe( map(res=>res));
    }
    prepzoneremove(o:zoneprep_md){ 
        return  this.http.post<zoneprep_md[]>(environment.urlapiAdmn + "/zoneprep/ops/remove/"+this.callID, JSON.stringify(o))
                .pipe( map(res=>res));
    }

    prepzoneline(o:zoneprep_md) { 
        return  this.http.post<zoneprln_md[]>(environment.urlapiAdmn + "/zoneprep/line/"+this.callID, JSON.stringify(o))
                .pipe( map(res=><zoneprln_md[]>res));
    }

    prlnzoneupsert(o:zoneprln_md){ 
        return  this.http.post(environment.urlapiAdmn + "/zoneprep/ops/upsertline/"+this.callID, JSON.stringify(o))
                .pipe( map(res=>res));
    }
    prlnzoneremove(o:zoneprln_md){ 
        return  this.http.post(environment.urlapiAdmn + "/zoneprep/ops/removeline/"+this.callID, JSON.stringify(o))
                .pipe( map(res=>res));
    }

}