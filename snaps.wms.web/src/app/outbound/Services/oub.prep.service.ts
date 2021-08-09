import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import { prepset, prep_ls, prep_md, prep_pm, prln_md } from '../Models/oub.prep.mode';
import { outbound_ls } from '../Models/oub.order.model';
import { shareService } from 'src/app/share.service';
import { pam_set } from 'src/app/admn/models/adm.parameter.model';
@Injectable({ providedIn: 'root' })
export class ouprepService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient, private ss:shareService) { }

    find(o:prep_pm){
        return  this.http.post<prep_ls[]>(environment.urlapiOubnd + "/ouprep/list/" + this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><prep_ls[]>res));
    }
    get(o:prep_ls){
        return  this.http.post<prep_md>(environment.urlapiOubnd + "/ouprep/get/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><prep_md>res));
    }
    setpriority(o:prep_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/setpriority/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    setStart(o:prep_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/setStart/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    setEnd(o:prep_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/setEnd/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    opsPick(o:prln_md[]){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/opsPick/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    opsPut(o:prln_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/opsPut/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    opsCancel(o:prep_md){ 
      return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/opsCancel/" + this.ss.getId(), JSON.stringify(o))
      .pipe(map(res=><resultRequest>res));
    }

    procsetup(o:prepset){        
        return  this.http.post<prepset>(environment.urlapiOubnd + "/ouprep/procsetup/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><prepset>res));
    }
    procstock(o:string){
        return  this.http.post<prepset>(environment.urlapiOubnd + "/ouprep/procstock/"+o+"/" + this.ss.getId(), JSON.stringify("Tron a live"))
                .pipe(map(res=><prepset>res));
    }
    procdistb(o:string){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouprep/procdistb/"+o+"/" + this.ss.getId(), JSON.stringify("Tron a live"))
                .pipe(map(res=><resultRequest>res));
    }

    distsetup(o:prepset){
        return  this.http.post<prepset>(environment.urlapiOubnd + "/ouprep/distsetup/" + this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><prepset>res));
    }

    getpicklist(orgcode:string, site:string, depot:string, prepno:string): any {		
      let search = new URLSearchParams();
      search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('prepno',prepno);
      return this.http.post(environment.urlapiDocument +'/print/picklist',search.toString(),{responseType:'blob'});

		// return this.http.get(environment.urlapiDocument +'/get/picklist?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&prepno='+prepno+'&id='+this.ss.getId(), { responseType: 'blob' });
    }
    getdistlist(orgcode:string, site:string, depot:string, prepno:string): any {		
      let search = new URLSearchParams();
      search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('prepno',prepno);
      return this.http.post(environment.urlapiDocument +'/print/distlist',search.toString(),{responseType:'blob'});
		  // return this.http.get(environment.urlapiDocument +'/get/distlist?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&prepno='+prepno+'&id='+this.ss.getId(), { responseType: 'blob' });
    }
    getshiplabel_loose(orgcode:string, site:string, depot:string, huno:string): any {		
      let search = new URLSearchParams();
      search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('huno', huno);	
      return this.http.post(environment.urlapiDocument +'/print/labelshipped',search.toString(),{responseType:'blob'});
		  // return this.http.get(environment.urlapiDocument +'/get/labelshipped?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&huno='+huno+'&id='+this.ss.getId(), { responseType: 'blob' });
    }
    getshiplabel_pallet(orgcode:string, site:string, depot:string, huno:string): any {		
      let search = new URLSearchParams();
      search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('huno', huno);	
      return this.http.post(environment.urlapiDocument +'/print/fullpallet',search.toString(),{responseType:'blob'});
		// return this.http.get(environment.urlapiDocument +'/get/labelshipped_pallet?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&huno='+huno+'&id='+this.ss.getId(), { responseType: 'blob' });
    }

    getshiplabel_distribution(orgcode:string, site:string, depot:string, huno:string): any {		
      let search = new URLSearchParams();
      search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('huno', huno);	
      return this.http.post(environment.urlapiDocument +'/print/labelshipped_pallet',search.toString(),{responseType:'blob'});
    }

}