import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import { route_hu, route_ls, route_md, route_pm, route_thsum } from '../Models/oub.route.model';
import { shareService } from 'src/app/share.service';
@Injectable({ providedIn: 'root' })
export class ourouteService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient, private ss:shareService) { }
    thsum(o:route_pm){
      return  this.http.post<route_thsum[]>(environment.urlapiOubnd + "/ouroute/thsum/"+this.ss.getId(), JSON.stringify(o))
              .pipe( map(res=><route_thsum[]>res));
  }
    find(o:route_pm){
        return  this.http.post<route_ls[]>(environment.urlapiOubnd + "/ouroute/list/"+this.ss.getId(), JSON.stringify(o))
                .pipe( map(res=><route_ls[]>res));
    }
    get(o:route_ls){
        return  this.http.post<route_md>(environment.urlapiOubnd + "/ouroute/get/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><route_md>res));
    }
    upsert(o:route_md){
        return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouroute/upsert/"+this.ss.getId(), JSON.stringify(o))
                .pipe(map(res=><resultRequest>res));
    }
    //allocate
    allocate(o:route_md){
      return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouroute/allocate/"+this.ss.getId(), JSON.stringify(o))
      .pipe(map(res=><resultRequest>res));
    }
    //allocate
    remove(o:route_md){
      return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouroute/remove/"+this.ss.getId(), JSON.stringify(o))
      .pipe(map(res=><resultRequest>res));
    }
    //huload
    huload(o:route_hu){
      return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouroute/huload/"+this.ss.getId(), JSON.stringify(o))
      .pipe(map(res=><resultRequest>res));
    }

    shipment(o:route_md){
      return  this.http.post<resultRequest>(environment.urlapiOubnd + "/ouroute/shipment/"+this.ss.getId(), JSON.stringify(o))
      .pipe(map(res=><resultRequest>res));
    }

    getstaging(){
        return  this.http.post<lov[]>(environment.urlapiOubnd + "/ouroute/getstaging/"+this.ss.getId(), "")
                .pipe( map(res=><lov[]>res));
    }
  getthirdparty() {
    return this.http.post<lov[]>(environment.urlapiOubnd + "/ouroute/getthirdparty/" + this.ss.getId(), "")
      .pipe(map(res => <lov[]>res));
  }
  //gettransporter
  gettransporter() {
    return this.http.post<lov[]>(environment.urlapiOubnd + "/ouroute/gettransporter/" + this.ss.getId(), "")
      .pipe(map(res => <lov[]>res));
  }

  getloaddraft(orgcode:string, site:string, depot:string, routeno:string): any {		
    let search = new URLSearchParams();
    search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('routeno',routeno);
    return this.http.post(environment.urlapiDocument +'/print/getLoadingDraft',search.toString(),{responseType:'blob'});

		// return this.http.get(environment.urlapiDocument +'/get/getLoadingDraft?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&routeno='+routeno+'&id='+this.ss.getId(), { responseType: 'blob' });
  }

  gettransportnote(orgcode:string, site:string, depot:string, routeno:string, trtno:string): any {
    let search = new URLSearchParams();
    search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('routeno',routeno);search.set('outrno',trtno);
    return this.http.post(environment.urlapiDocument +'/print/getTransportnote',search.toString(),{responseType:'blob'});	
		// return this.http.get(environment.urlapiDocument +'/get/getTransportnote?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&routeno='+routeno+'&outrno='+trtno+'&id='+this.ss.getId(), { responseType: 'blob' });
  }

  getpackinglist(orgcode:string, site:string, depot:string, routeno:string, trtno:string): any {	
    let search = new URLSearchParams();
    search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('routeno',routeno);search.set('outrno',trtno);
    return this.http.post(environment.urlapiDocument +'/print/getPackinglist',search.toString(),{responseType:'blob'});		
		// return this.http.get(environment.urlapiDocument +'/get/getPackinglist?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&routeno='+routeno+'&outrno='+trtno+'&id='+this.ss.getId(), { responseType: 'blob' });
  }

}
