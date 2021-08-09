import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { lov } from './helpers/lov';
import { map, observeOn } from 'rxjs/operators';
import { environment } from  '../environments/environment';
import { controllers } from "chart.js";
import { BehaviorSubject, Observable } from "rxjs";
import { ThemeService } from "ng2-charts";
import { async } from "rxjs/internal/scheduler/async";
import { ConsoleService } from "@ng-select/ng-select/lib/console.service";
import { connectableObservableDescriptor } from "rxjs/internal/observable/ConnectableObservable";
@Injectable({ providedIn: 'root' })
export class shareService {
    private u:string;
    private guid:string;
    private lsflow:lov[] = new Array();             // Flow state 
    private lsrowlimit:lov[] = new Array();         //Row limit
    private lsunit:lov[] = new Array();             //stock unit
    private lsspcarea:lov[] = new Array();           //Area
    private lsyesno:lov[] = new Array();            //Yes no;

    public osrowlimit = new Observable();
    constructor(private http: HttpClient) { this.guid = this.gencode(); this.ngSetup();}  
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
        return [this.u.substr(0,8), this.u.substr(8,4), 'snaps-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }
    public getId():string { return this.guid; }

    warehouse() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Warehouse/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    } 
    depot() {
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Depot/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    role() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Role/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    getlov(btype:String,bcode:String) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/"+btype+"/"+bcode+"/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lov(btype:String,bcode:String) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/"+btype+"/"+bcode+"/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lovms(btype:String,bcode:String) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Master/"+btype+"/"+bcode+"/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    prepzonestock() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Prepzonestock/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    prepzonedist() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Prepzonedist/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    storagezone() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Storagezone/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    sharedist() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/Sharedist/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    hu(){ 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/HU/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }

    lovzone() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lovaisle(z:string) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" + z + "/"+this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lovbay(z:string,a:string) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" + z + "/" + a + "/" +this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lovlevel(z:string,a:string,b:string) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" + z + "/" + a + "/" + b + "/" +this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }
    lovlocation(z:string,a:string,b:string,l:string) { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/WMP/" + z + "/" + a + "/" + b + "/" + l + "/" +this.guid, JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    }

    lovstaginginb() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/staginginb/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }
    lovstagingoub() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/stagingoub/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }
    lovbulk() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/bulk/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }
    lovdamage() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/damage/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }
    lovsinbin() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/sinbin/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }
    lovexchange() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/exchange/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }
    lovreturn() {         
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/return/"+this.guid, JSON.stringify("")) .pipe( map(res=><lov[]>res));
    }

    lovrowlimit(){ 
        return this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/DATAGRID/ROWLIMIT/"+this.guid, JSON.stringify(""));            
    }

    /** Validate location */
    valaisle(o){ 
        return this.http.post<boolean>(environment.urlapiAdmn + "/LOV/valaisle/"+o, JSON.stringify("")) .pipe(map(res=><boolean>res));
    }
    valbay(o){ 
        return this.http.post<boolean>(environment.urlapiAdmn + "/LOV/valbay/"+o, JSON.stringify("")) .pipe(map(res=><boolean>res));
    }
    vallevel(o){ 
        return this.http.post<boolean>(environment.urlapiAdmn + "/LOV/vallevel/"+o, JSON.stringify("")) .pipe(map(res=><boolean>res));
    }
    vallocation(o){ 
        return this.http.post<boolean>(environment.urlapiAdmn + "/LOV/vallocation/"+o, JSON.stringify("")) .pipe(map(res=><boolean>res));
    }

    ngSetup(){ 
        if (this.lsyesno.length == 0) { 
            this.lsyesno.push({ value : "1", desc : "Yes", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "", valopnthird : ""});
            this.lsyesno.push({ value : "0", desc : "No", icon : "", valopnfirst : "", valopnfour : "", valopnsecond : "", valopnthird : ""});
        }
        if (this.lsrowlimit.length == 0){ this.ngIntRowlimit().subscribe((res)=> this.lsrowlimit = res.sort((a,b) => parseInt(a.value) - parseInt(b.value)) ); }
        if (this.lsspcarea.length == 0) { this.ngIntArea().subscribe((res)=> this.lsspcarea = res ); }
        if (this.lsflow.length == 0)    { this.ngIntFlow().subscribe((res)=> this.lsflow = res ); }
        if (this.lsunit.length == 0)    { this.ngIntUnit().subscribe((res)=> this.lsunit = res ); }
    }
    ngState(btype:string, bcode:string){ 
        return this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/"+btype+"/"+bcode+"/"+this.guid, JSON.stringify(""))
        .pipe(map(res=> <lov[]>res));
    }    
    ngIntRowlimit() { 
        return this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/DATAGRID/ROWLIMIT/"+this.guid, JSON.stringify(""))
        .pipe(map(res=><lov[]>res));            
    }
    ngIntArea() { 
        return this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/LOCATION/SPCAREA/"+this.guid, JSON.stringify(""))
        .pipe(map(res=><lov[]>res));
    }
    ngIntUnit() { 
        return this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/UNIT/KEEP/"+this.guid, JSON.stringify(""))
        .pipe(map(res=><lov[]>res));
    }
    ngIntFlow() { 
        return this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/State/ALL/FLOW/"+this.guid, JSON.stringify(""))
        .pipe(map(res=><lov[]>res));
    }

    getUnit():lov[] { return this.lsunit; }
    getRowlimit():lov[] { return this.lsrowlimit; } 
    getArea():lov[] { return this.lsspcarea;  }
    getYesno():lov[] { return this.lsyesno; }

    ngDecIcon(o:string){ try { return this.lsflow.find(e=>e.value == o).icon; } catch (exc){ return o; } }
    ngDecStr(o:string){ try { return this.lsflow.find(e=>e.value == o).desc;  } catch (exc){ return o; } }
    ngDecUnitstock(o:string) { try { return this.lsunit.find(e=>e.value == o).desc;  } catch (exc){ return o; } } 
    ngEncUnitstock(o:string){ try { return this.lsunit.find(e=>e.desc == o).value;  } catch (exc){ return o; } }
    ngDecState(o:string) { try { return this.lsflow.find(e=>e.value == o).desc; } catch(exc) {return o; } }
    ngEncState(o:string) { try { return this.lsflow.find(e=>e.desc == o).value; } catch(exc) { return o; } } 
    ngDecArea(o:string) { try { return this.lsspcarea.find(e=>e.value == o).desc; } catch(exc) { return o;} }
    ngDecColor(o:number) {
        try { 
            if (o <= 20) { return 'text-danger'; }
            else if (o > 40 && o <= 70) { return 'text-warning'; }
            else { return 'text-primary';  }            
        }catch(exc) { return 'text-mute'; }
    }

    ngStockicon(o:string,m:string){ 
    if(m == 'XX') { return 'fas fa-stop-circle fa-lg dp text-danger w25';}
    else { 
        switch(o){        
        case 'crincoming' : return 'fas fa-ship fa-lg text-danger'
        case 'crplanship' : return 'fas fa-file-alt fa-lg text-danger w25'
        case 'cronhand' : return 'fas fa-heart fa-lg fn-second w25'
        case 'crprep' : return 'fas fa-hand-paper fa-lg text-warning w25'
        case 'crsinbin' : return 'fas fa-exclamation-triangle fa-lg text-danger w25'
        case 'cravailable' : return 'fas fa-heartbeat fn-second fa-lg w25'
        case 'crstaging' : return 'fas fa-truck-loading fa-lg text-warning w25'
        case 'crdamage' : return 'fas fa-heart-broken fa-lg text-danger w25'
        case 'crbulknrtn' : return 'fas fa-pallet tex-danger fa-lg w25'
        case 'crtask' : return 'fas fa-dolly-flatbed fa-lg text-warning w25'
        case 'crblock' : return 'fas fa-stop-circle fa-lg text-danger w25'
        case 'croverflow' : return 'fas fa-life-ring fn-second fa-lg w25'
        case 'crrtv' : return 'fas fa-industry fa-lg text-warning w25'
        case 'crexchange' : return 'fas fa-exchange-alt fa-lg text-danger w25'
        case 'crpicking' : return 'fas fa-heartbeat fn-second fa-lg w25'
        case 'crreserve' : return 'fas fa-heartbeat fn-second fa-lg w25'
        default : return o;
        }
    }
    }

}