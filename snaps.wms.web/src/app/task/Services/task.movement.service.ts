import { replen_md } from './../Models/task.movement.model';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import { task_ls, task_md, task_pm } from '../Models/task.movement.model';
import { shareService } from 'src/app/share.service';
import { Observable } from 'rxjs';
@Injectable({ providedIn: 'root' })
export class taskService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient,private ss:shareService) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(o:task_pm){
        return  this.http.post<task_ls[]>(environment.urlapiTask + "/task/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><task_ls[]>res));
    }
    get(o:task_ls){
        return  this.http.post<task_md>(environment.urlapiTask + "/task/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><task_md>res));
    }
    upsert(o:task_md){
        return  this.http.post<resultRequest>(environment.urlapiTask + "/task/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

    assign(o:task_md){
        return  this.http.post<resultRequest>(environment.urlapiTask + "/task/assign/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    start(o:task_md){
        return  this.http.post<resultRequest>(environment.urlapiTask + "/task/start/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    fill(o:task_md){
        return  this.http.post<resultRequest>(environment.urlapiTask + "/task/fill/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    cancel(o:task_md){
        return  this.http.post<resultRequest>(environment.urlapiTask + "/task/cancel/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    confirm(o:task_md){
        return  this.http.post<resultRequest>(environment.urlapiTask + "/task/confirm/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }

    getlabelputaway(orgcode:string, site:string, depot:string, taskno:string, ) {
        let search = new URLSearchParams();
        search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('taskno', taskno);
		return this.http.post(environment.urlapiDocument +'/print/putaway',search.toString(),{responseType:'blob'});
		// return this.http.post(environment.urlapiDocument +'/get/putaway'+orgcode+'&site='+site+'&depot='+depot+'&taskno='+taskno+'&id='+this.ss.getId(), { headers, responseType: 'blob' });
    }
    getlabelfullpallet(orgcode:string, site:string, depot:string, taskno:string, ) {
        let search = new URLSearchParams();
        search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('taskno', taskno);
        return this.http.post(environment.urlapiDocument +'/print/fullpallet',search.toString(),{responseType:'blob'});
		// return this.http.post(environment.urlapiDocument +'/get/fullpallet?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&taskno='+taskno+'&id='+this.ss.getId(), { responseType: 'blob' });
    }
    gettasklist(o:task_pm) { 
        return this.http.post("http://localhost/bgcwmsdocument/get/statement?order="+o,JSON.stringify(o),{ responseType : 'blob' });
    }
    urgenReplenishment(o:replen_md){
      return  this.http.post<resultRequest>(environment.urlapiTask + "/task/replenishment/"+this.gencode(), JSON.stringify(o))
              .pipe(map(res=><resultRequest>res));
    }
    public ExpHeader(params) {
        let headers = new HttpHeaders()
          .set("Content-Type", "application/json")
        const reqData = { headers, responseType: "blob" as "json" };
        if (params) {
          let reqParams = {};
          Object.keys(params).map((k) => {
            reqParams[k] = params[k];
          });
          reqData["params"] = reqParams;
        }
        return reqData;
      }
}