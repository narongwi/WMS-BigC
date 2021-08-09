import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import { stock_info, stock_ls, stock_md, stock_pm } from '../models/inv.stock.model';
import { correction_md } from '../models/inv.correction.mode';
import { transfer_md } from '../models/inv.transfer.model';
import { shareService } from 'src/app/share.service';

@Injectable({ providedIn: 'root' })
export class inventoryService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient, private ss:shareService) { }

    findproduct(o:stock_pm){
        return  this.http.post<stock_ls[]>(environment.urlapiInvt + "/stock/listproduct/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><stock_ls[]>res));
    }

    get(o:stock_ls){
        return  this.http.post<stock_md>(environment.urlapiInvt + "/stock/get/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><stock_md>res));
    }
    getstockInfo(o:stock_ls){
        return  this.http.post<stock_info>(environment.urlapiInvt + "/stock/getstockInfo/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><stock_info>res));
    }
    getstockline(typesel:string, o:stock_ls){
        return  this.http.post<stock_md[]>(environment.urlapiInvt + "/stock/getstockLine/"+typesel+"/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><stock_md[]>res));
    }
    setstockInfo(o:stock_md){
        return  this.http.post<stock_info>(environment.urlapiInvt + "/stock/setstatus/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><stock_info>res));
    }
    getproductratio(article:string, pv:string, lv:string){
        return  this.http.post<lov[]>(environment.urlapiInvt + "/stock/getproductratio/"+article+"/"+pv+"/"+lv,"")
                .pipe(map(res=><lov[]>res));
    }

    //Correction stock 
    opscorrect(o:correction_md){ 
        return  this.http.post(environment.urlapiInvt + "/correction/process/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=>res));
    }
    getlocation(o:stock_ls) { 
        return  this.http.post<lov[]>(environment.urlapiInvt + "/correction/getLocation/"+this.ss.getId(), JSON.stringify(o))
        .pipe(map(res=><lov[]>res));
    }
    //Transfer stsock 
    validatelocation(o:transfer_md){
        return  this.http.post(environment.urlapiInvt + "/transfer/validatelocation/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=>res));
    }
    checklocation(o:string){
        return  this.http.post<lov>(environment.urlapiInvt + "/transfer/checklocation/"+o, JSON.stringify(o))
                .pipe(map(res=><lov>res));
    }
    opstrasnfer(o:transfer_md){ 
        return  this.http.post<transfer_md>(environment.urlapiInvt + "/transfer/transferstock/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><transfer_md>res));
    }

    getlabelhu(orgcode:string, site:string, depot:string, huno:string): any {	
        let search = new URLSearchParams();
        search.set('orgcode', orgcode); 
        search.set('site', site); 
        search.set('depot', depot); 
        search.set('huno', huno);	
        return this.http.post(environment.urlapiDocument +'/print/hu',search.toString(),{responseType:'blob'});
		// return this.http.get(environment.urlapiDocument +'/print/hu?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&huno='+huno+'&id='+this.ss.getId(), { responseType: 'blob' });
    }

    // upsert(o:stock_md){
    //     return  this.http.post<resultRequest>(environment.urlapiInvt + "/task/upsert/"+this.gencode(), JSON.stringify(o))
    //             .pipe(map(res=><resultRequest>res));
    // }


}