import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable, VirtualTimeScheduler } from 'rxjs';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from 'src/app/helpers/lov';
import { locdw_gn, locdw_gngrid, locdw_ls, locdw_md, locdw_picking, locdw_pivot, locdw_pm, locup_ls, locup_md, locup_pm } from '../Models/mdl-mapstorage';


@Injectable({ providedIn: 'root' })
export class mapstorageService {
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

    lovzone(o:locup_pm) { 
        // o.fltype = "ZN";
        // return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locupper/"+this.gencode(), JSON.stringify(o))
        //         .pipe( map(res=><lov[]>res));
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/"+this.guid, JSON.stringify(""))
        .pipe( map(res=><lov[]>res));

    }
    lovaisle(o:locup_pm) { 
        // o.fltype = "AL";
        // return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locupper/"+this.gencode(), JSON.stringify(o))
        //         .pipe( map(res=><lov[]>res));
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" +o.lszone + "/"+this.guid, JSON.stringify(""))
        .pipe( map(res=><lov[]>res));
    }
    lovbay(o:locup_pm) { 
        // o.fltype = "BA";
        // return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locupper/"+this.gencode(), JSON.stringify(o))
        //         .pipe( map(res=><lov[]>res));
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" + o.lszone + "/" + o.lsaisle + "/" +this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lovlevel(o:locup_pm) { 
        // o.fltype = "LV";
        // return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locupper/"+this.gencode(), JSON.stringify(o))
        //         .pipe( map(res=><lov[]>res));
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" + o.lszone + "/" + o.lsaisle + "/" + o.lsbay + "/" +this.guid, JSON.stringify(""))
        .pipe( map(res=><lov[]>res));
    }
    lovbinary(btype:String,bcode:String) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/"+btype+"/"+bcode, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }

    lovzonedist(o:locup_pm){ 
        o.fltype = "ZN";
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Prepzonedist/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><lov[]>res));
    }
    // lovaisledist(o:locup_pm){ 
    //     o.fltype = "AL";
    //     return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locdist/"+this.gencode(), JSON.stringify(o))
    //             .pipe( map(res=><lov[]>res));
    // }
    // lovbaydist(o:locup_pm){ 
    //     o.fltype = "BA";
    //     return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locdist/"+this.gencode(), JSON.stringify(o))
    //             .pipe( map(res=><lov[]>res));
    // }
    // lovleveldist(o:locup_pm){ 
    //     o.fltype = "LV";
    //     return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Locdist/"+this.gencode(), JSON.stringify(o))
    //             .pipe( map(res=><lov[]>res));
    // }

    fndloczone(o:locup_pm){         
        return  this.http.post<locup_md[]>(environment.urlapiAdmn + "/Mapstorage/loczone/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><locup_md[]>res));
    }

    fndlocup(o:locup_pm){ 
        return  this.http.post<locup_md[]>(environment.urlapiAdmn + "/Mapstorage/locup/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><locup_md[]>res));
    }
    upsertlocup(o:locup_md){ 
        return  this.http.post(environment.urlapiAdmn + "/Mapstorage/locup/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }
    droplocup(o:locup_md){ 
        return  this.http.post(environment.urlapiAdmn + "/Mapstorage/locup/drop/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }

    fndlocdw(o:locdw_pm){ 
        return  this.http.post<locdw_md[]>(environment.urlapiAdmn + "/Mapstorage/locdw/list/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><locdw_md[]>res));
    }
    getlocdw(o:locdw_ls){
        return  this.http.post<locdw_md>(environment.urlapiAdmn + "/Mapstorage/locdw/get/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><locdw_md>res));
    }
    upsertlocdw(o:locdw_md){ 
        return  this.http.post(environment.urlapiAdmn + "/Mapstorage/locdw/upsert/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }
    droplocdw(o:locdw_md){ 
        return  this.http.post(environment.urlapiAdmn + "/Mapstorage/locdw/drop/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }


    getaisle() { 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/zone/get"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=>res));
    }
    getbay() { 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/zone/get"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=>res));
    }
    getlevel(){ 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/zone/get"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=>res));
    }

    uploadbarcode(){ 
        return  this.http.post<any>(environment.urlapiAdmn + "/External/upload/barcode/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=>res));
    }

    genlocation(o:locdw_gn){ 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/locdw/generate/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }
    gengrid(o:locdw_gngrid){ 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/locdw/generategrid/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }



    getpivot(o:locdw_pm) { 
        return  this.http.post<locdw_pivot[]>(environment.urlapiAdmn + "/Mapstorage/locdw/getpivot/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><locdw_pivot[]>res));
    }
    setpivot(o:locdw_pivot) { 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/locdw/setpivot/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }
    getpicking(o:locdw_pm) { 
        return  this.http.post<locdw_picking[]>(environment.urlapiAdmn + "/Mapstorage/locdw/getpicking/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=><locdw_picking[]>res));
    }
    setpicking(o:locdw_picking){ 
        return  this.http.post<any>(environment.urlapiAdmn + "/Mapstorage/locdw/setpicking/"+this.gencode(), JSON.stringify(o))
                .pipe( map(res=>res));
    }
    
}