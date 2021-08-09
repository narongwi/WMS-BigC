import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable, VirtualTimeScheduler } from 'rxjs';
import { environment } from  '../../../environments/environment';
import { accn_cfg, accn_ls, accn_md, accn_pm } from '../models/account.model';
import { resultRequest } from '../../helpers/resultRequest';
import { warehouse_ls } from '../models/adm.warehouse.model';
import { depot_ls } from '../models/adm.depot.model';
import { role_ls, role_md, role_pm } from '../models/role.model';
import { lov } from 'src/app/helpers/lov';


@Injectable({ providedIn: 'root' })
export class adminService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) {

    }
    

    private enc(str):string {
        return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function(match, p1) {
            return String.fromCharCode(parseInt(p1, 16))
        }))
    } 
    private dec(str):string {
        if (str!=null) {
            return decodeURIComponent(Array.prototype.map.call(atob(str), function(c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
            }).join(''))
        }else { return ""; }
    }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    getWarehouse() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Warehouse/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    } 
    getDepot() {
        return  this.http.post<depot_ls[]>(environment.urlapiAdmn + "/LOV/Depot/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=><depot_ls[]>res));
    }
    getlovrole() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Role/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    sellovrole(site:string,depot:string) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/SelRole/" + site + "/" + depot + "/" +this.gencode(), JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    getlov(btype:String,bcode:String) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/"+btype+"/"+bcode, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }

    // Account
    accnFind(o:accn_pm)  { //Find account
        return  this.http.post<accn_ls[]>(environment.urlapiAdmn + "/Account/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><accn_ls[]>res));
    }

    accnGet(o:accn_ls) { // get account
        return  this.http.post<accn_md>(environment.urlapiAdmn + "/Account/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><accn_md>res));
    }
    accnMod(o:accn_ls) { // get account
        return  this.http.post<accn_md>(environment.urlapiAdmn + "/Account/mod/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><accn_md>res));
    }
    addCfg(o:accn_cfg) { // upsert
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/Account/addCfg/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    delCfg(o:accn_cfg) { // upsert
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/Account/delCfg/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    accnUpsert(o:accn_md) { // upsert
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/Account/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    accnReset(o:accn_md) { // upsert
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/Account/resetpriv/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    // Role 
    rolefind(o:role_pm){
        return  this.http.post<role_ls[]>(environment.urlapiAdmn + "/Role/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><role_ls[]>res));
    }
    roleget(o:role_ls){
        return  this.http.post<role_md>(environment.urlapiAdmn + "/Role/get/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><role_md>res));
    }
    roleupsert(o:role_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/Role/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    roleremove(o:role_md){
        return  this.http.post<resultRequest>(environment.urlapiAdmn + "/Role/drop/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    rolemaster(site:string,depot:string){ 
        return  this.http.post<role_md>(environment.urlapiAdmn + "/Role/getMaster/" + site + "/" +depot+ "/" +this.gencode(), JSON.stringify(""))
                .pipe(map(res=><role_md>res));
    }
    

    //Handerling unit lov
    gethus() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/HU/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }

    //Zone 
    getzone() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Zone/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
}