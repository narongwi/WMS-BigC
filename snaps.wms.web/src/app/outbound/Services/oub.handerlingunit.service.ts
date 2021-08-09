import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import { handerlingunit, handerlingunit_gen, handerlingunit_item } from '../Models/oub.handlingunit.model';
@Injectable({ providedIn: 'root' })
export class ouhanderlingunitService {
  public u: string;
  public guid: string;

  constructor(private http: HttpClient) { }
  private gencode() {
    this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
    return [this.u.substr(0, 8), this.u.substr(8, 4), '4000-8' + this.u.substr(13, 3), this.u.substr(16, 12)].join('-');
  }

  find(o: handerlingunit) {
    return this.http.post<handerlingunit[]>(environment.urlapiOubnd + "/ouhanderlingunit/list/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <handerlingunit[]>res));
  }
  get(o: handerlingunit) {
    return this.http.post<handerlingunit>(environment.urlapiOubnd + "/ouhanderlingunit/get/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <handerlingunit>res));
  }
  upsert(o: handerlingunit) {
    return this.http.post<resultRequest>(environment.urlapiOubnd + "/ouhanderlingunit/upsert/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <resultRequest>res));
  }
  close(o: handerlingunit) {
    return this.http.post<resultRequest>(environment.urlapiOubnd + "/ouhanderlingunit/close/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <resultRequest>res));
  }
  getmaster(o: handerlingunit) {
    return this.http.post<lov[]>(environment.urlapiOubnd + "/ouhanderlingunit/getmaster/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <lov[]>res));
  }

  generate(o: handerlingunit_gen) {
    return this.http.post<resultRequest>(environment.urlapiOubnd + "/ouhanderlingunit/genereate/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <resultRequest>res));
  }

  lines(o: handerlingunit) {
    return this.http.post<handerlingunit_item[]>(environment.urlapiOubnd + "/ouhanderlingunit/lines/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <handerlingunit_item[]>res));
  }
  
  linesnonsum(o: handerlingunit) {
    return this.http.post<handerlingunit_item[]>(environment.urlapiOubnd + "/ouhanderlingunit/linesnonsum/" + this.gencode(), JSON.stringify(o))
      .pipe(map(res => <handerlingunit_item[]>res));
  }

  gethuempty_label(orgcode:string, site:string, depot:string, huno:string): any {		
    let search = new URLSearchParams();
    search.set('orgcode', orgcode); search.set('site', site); search.set('depot', depot); search.set('huno', huno);	
    return this.http.post(environment.urlapiDocument +'/print/huempty',search.toString(),{responseType:'blob'});
    // return this.http.get(environment.urlapiDocument +'/get/huempty?orgcode='+orgcode+'&site='+site+'&depot='+depot+'&huno='+huno ,  { responseType: 'blob' });
  }
  
}
