import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, from, Observable, VirtualTimeScheduler } from 'rxjs';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { shareService } from 'src/app/share.service';
import { exsFile } from '../models/snaps.wms.externalsource.file';
import { exsBarcode } from '../models/snaps.wms.externalsource.barcode';
import { exsThirdparty } from '../models/snaps.wms.externalsource.thparty';
import { exsInbouln, exsInbound } from '../models/snaps.wms.externalsource.inbound';
import { exsOutbound, exsOutbouln, exsPrep, exsLocup, exsLocdw } from '../models/snaps.wms.externalsource.outbound';
import { exsProduct } from '../models/snaps.wms.externalsource.product';
@Injectable({ providedIn: 'root' })
export class externalSourcelService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient, private ss:shareService) {

    }

    findProduct(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsBarcode/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }

    getProductLines(o:exsFile) { 
        return  this.http.post<exsProduct[]>(environment.urlapiExssource + "/exsProduct/lines/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsProduct[]>res));
    }
    uploadProduct(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsProduct/Upload/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    findBarcode(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsProduct/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }

    getBarcodeLines(o:exsFile) { 
        return  this.http.post<exsBarcode[]>(environment.urlapiExssource + "/exsBarcode/lines/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsBarcode[]>res));
    }
    uploadbarcode(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsBarcode/Upload/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    findThparty(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsTHParty/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }

    getTHPartyLines(o:exsFile) { 
        return  this.http.post<exsThirdparty[]>(environment.urlapiExssource + "/exsTHParty/lines/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsThirdparty[]>res));
    }
    uploadTHParty(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsTHParty/Upload/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    findInbound(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsInbound/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }
    getInboundLines(o:exsFile) { 
        return  this.http.post<exsInbound[]>(environment.urlapiExssource + "/exsInbound/lines/Inbound/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsInbound[]>res));
    }
    uploadInbound(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsInbound/Upload/Inbound/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }
    getInboulnLines(o:exsFile) { 
        return  this.http.post<exsInbouln[]>(environment.urlapiExssource + "/exsInbound/lines/Inbouln/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsInbouln[]>res));
    }
    uploadInbouln(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsInbound/Upload/Inbouln/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    findOutbound(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsOutbound/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }
    getOutboundLines(o:exsFile) { 
        return  this.http.post<exsOutbound[]>(environment.urlapiExssource + "/exsOutbound/lines/Outbound/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsOutbound[]>res));
    }
    uploadOutbound(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsOutbound/Upload/Outbound/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }
    getOutboulnLines(o:exsFile) { 
        return  this.http.post<exsOutbouln[]>(environment.urlapiExssource + "/exsOutbound/lines/Outbouln/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsOutbouln[]>res));
    }
    uploadOutbouln(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsOutbound/Upload/Outbouln/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    //Preparation path

    findPrep(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsPrep/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }
    getPrepLines(o:exsFile) { 
        return  this.http.post<exsPrep[]>(environment.urlapiExssource + "/exsPrep/lines/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsPrep[]>res));
    }
    uploadPrep(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsPrep/Upload/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    //Location Upper
    findLocup(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsLocup/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }
    getLocupLines(o:exsFile) { 
        return  this.http.post<exsLocup[]>(environment.urlapiExssource + "/exsLocup/lines/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsLocup[]>res));
    }
    uploadLocup(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsLocup/Upload/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

    //Location Lower
    findLocdw(o:exsFile) { 
        return  this.http.post<exsFile[]>(environment.urlapiExssource + "/exsLocdw/find/"+this.ss.getId(),o)
                .pipe(map(res=><exsFile[]>res));
    }
    getLocdwLines(o:exsFile) { 
        return  this.http.post<exsLocdw[]>(environment.urlapiExssource + "/exsLocdw/lines/"+ o.fileid + "/" +this.ss.getId(),o)
                .pipe(map(res=><exsLocdw[]>res));
    }
    uploadLocdw(o:FormData,type:string){
        return  this.http.post<any>(environment.urlapiExssource + "/exsLocdw/Upload/"+type+"/"+this.ss.getId(),o)
                .pipe(map(res=>res));
    }

}