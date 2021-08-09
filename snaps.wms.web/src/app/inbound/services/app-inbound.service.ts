import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { inboulx, inbound_hs, inbound_ls, inbound_md, inbound_pm } from '../models/mdl-inbound';
import { lov } from 'src/app/helpers/lov';
import { pam_inbound, pam_set } from 'src/app/admn/models/adm.parameter.model';
import { Observable } from 'rxjs';
import { shareService } from 'src/app/share.service';
@Injectable({ providedIn: 'root' })
export class inboundService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient, private ss: shareService) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(o:inbound_pm){
        return  this.http.post<inbound_ls[]>(environment.urlapiInbnd + "/inbound/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><inbound_ls[]>res));
    }
    get(o:inbound_ls){
        return  this.http.post<inbound_md>(environment.urlapiInbnd + "/inbound/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><inbound_md>res));
    }
    upsert(o:inbound_md){
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    getproductratio(article:string, pv:string, lv:string){
        return  this.http.post<lov[]>(environment.urlapiInbnd + "/inbound/getproductratio/"+article+"/"+pv+"/"+lv,"tron a live")
                .pipe(map(res=><lov[]>res));
    }
    getstaging(o:number){
        return  this.http.post<lov[]>(environment.urlapiInbnd + "/inbound/getstaging/"+o.toString(),"tron a live")
                .pipe(map(res=><lov[]>res));
    }

    gethistory(o:inbound_pm){
        return  this.http.post<inbound_hs[]>(environment.urlapiInbnd + "/inbound/history/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><inbound_hs[]>res));
    }

    // cgetdocstatement(o:string) { 
    //     return this.http.post(environment.urlapiDocument + "/get/statement?order="+o,JSON.stringify(o),{ responseType : 'blob' });
    // }

    getdocstatement(orgcode:string, site:string, depot:string, inorder:string, ): any {		
        let search = new URLSearchParams();
        search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('inorder', inorder);
        return this.http.post(environment.urlapiDocument +'/print/statement',search.toString(),{responseType:'blob'});
		// return this.http.get(environment.urlapiDocument 
        //     +'/get/statement?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&inorder='+inorder, { responseType: 'blob' });
    }


    setpriority(inbo:string, o:number){
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setpriority/"+inbo.toString()+"/"+ o.toString(),"tron a live")
                .pipe(map(res=><resultRequest>res));
    }
    setstaging(inbo:string, o:String){
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setstaging/"+inbo.toString()+"/"+ o.toString(),"tron a live")
                .pipe(map(res=><resultRequest>res));
    }

    setremarks(inbo:string, remarks:string) { 
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setremarks/" + inbo.toString(),JSON.stringify(remarks))
                .pipe(map(res=><resultRequest>res));
    }

    setinvoice(inbo:string, invoiceno:string) { 
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setinvoice/" + inbo.toString(),JSON.stringify(invoiceno))
                .pipe(map(res=><resultRequest>res));
    }
    setreplan(inbo:string, replandate: Date | string | null) { 
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setreplan/" + inbo.toString(),JSON.stringify(replandate))
                .pipe(map(res=><resultRequest>res));
    }
    setunloadstart(inbo:string){
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setunloadstart/"+inbo.toString(),"tron a live")
                .pipe(map(res=><resultRequest>res));
    }

    setunloadend(inbo:string){
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setunloadend/"+inbo.toString(),"tron a live")
                .pipe(map(res=><resultRequest>res));
    }

    setfinish(inbo:string){
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setfinish/"+inbo.toString(),"tron a live")
                .pipe(map(res=><resultRequest>res));
    }
    setcancel(inbo:string, remarks:string) { 
        return  this.http.post<resultRequest>(environment.urlapiInbnd + "/inbound/setcancel/" + inbo.toString(),JSON.stringify(remarks))
                .pipe(map(res=><resultRequest>res));
    }

    getlx(o:inboulx){ 
        return  this.http.post<inboulx[]>(environment.urlapiInbnd + "/inbound/getlx/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><inboulx[]>res));
    }
    upsertlx(o:inboulx){
        return  this.http.post<inboulx[]>(environment.urlapiInbnd + "/inbound/upsertlx/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><inboulx[]>res));
    }

    removelx(o:inboulx){
        return  this.http.post<inboulx[]>(environment.urlapiInbnd + "/inbound/removelx/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><inboulx[]>res));
    }
    commitlx(o:inboulx){ 
        return  this.http.post<inboulx[]>(environment.urlapiInbnd + "/inbound/commitlx/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><inboulx[]>res));
    }

    getParameter(){         
        return  this.http.post<pam_inbound>(environment.urlapiInbnd + "/parameter/get/"+this.gencode(), "")
                .pipe( map(res=><pam_inbound>res));
    }
}