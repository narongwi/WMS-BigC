import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import { outbouln_md, outbound_ls, outbound_md, outbound_pm } from '../Models/oub.order.model';
import { pam_set } from 'src/app/admn/models/adm.parameter.model';
import { ouselect } from '../Models/oub.prep.mode';
@Injectable({ providedIn: 'root' })
export class outboundService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(o:outbound_pm){
        return  this.http.post<outbound_ls[]>(environment.urlapiOubnd + "/ouborder/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><outbound_ls[]>res));
    }
    //listdist
    listdist(o:outbound_pm){
        return  this.http.post<outbound_ls[]>(environment.urlapiOubnd + "/ouborder/listdist/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><outbound_ls[]>res));
    }
    get(o:outbound_ls){
        return  this.http.post<outbound_md>(environment.urlapiOubnd + "/ouborder/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><outbound_md>res));
    }
    getdist(o:outbound_ls){
        return  this.http.post<outbound_md>(environment.urlapiOubnd + "/ouborder/getdist/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><outbound_md>res));
    }
    
    upsert(o:outbound_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouborder/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    setpriority(o:outbound_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouborder/setpriority/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    setremarks(o:outbound_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouborder/setremarks/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    changeRequest(o:outbound_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouborder/changeRequest/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

    setlineorder(o:outbouln_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouborder/setlineorder/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

    getparameter(){
        return  this.http.post<pam_set[]>(environment.urlapiOubnd + "/ouparameter/get/" +this.gencode(), JSON.stringify(""))
                .pipe(map(res=><pam_set[]>res));
    }

    selectorder(o:ouselect){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/opsselect/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    unselectorder(o:ouselect){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/opsunselect/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
}