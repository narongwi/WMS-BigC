import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { shareService } from 'src/app/share.service';
import { merge_find, mergehu_ln, merge_set, merge_md, mergehu_md } from "../models/inv.merge.model";
@Injectable({
  providedIn: 'root'
})
export class MergehuService {
  public u: string;
  public guid: string;
  constructor(private http: HttpClient, private ss: shareService) { this.guid = this.ss.getId(); }
  find(o: merge_find) {
    return this.http.post<mergehu_ln[]>(environment.urlapiInvt + "/merge/find/" + this.guid, JSON.stringify(o))
      .pipe(map(res => <mergehu_ln[]>res));
  }
  list(o: merge_find) {
    return this.http.post<mergehu_md[]>(environment.urlapiInvt + "/merge/list/" + this.guid, JSON.stringify(o))
      .pipe(map(res => <mergehu_md[]>res));
  }
  line(o: mergehu_md) {
    return this.http.post<mergehu_ln[]>(environment.urlapiInvt + "/merge/line/" + this.guid, JSON.stringify(o))
      .pipe(map(res => <mergehu_ln[]>res));
  }
  generate(o: merge_set) {
    return this.http.post<merge_md>(environment.urlapiInvt + "/merge/generate/" + this.guid, JSON.stringify(o))
      .pipe(map(res => <merge_md>res));
  }
  cancel(o: merge_set) {
    return this.http.post(environment.urlapiInvt + "/merge/cancel/" + this.guid, JSON.stringify(o))
      .pipe(map(res => res));
  }
  merge(o: merge_md) {
    return this.http.post(environment.urlapiInvt + "/merge/mergehu/" + this.guid, JSON.stringify(o))
      .pipe(map(res => res));
  }
  label(orgcode: string, site: string, depot: string, huno: string, hutype: string) {
    let search = new URLSearchParams();
    search.set('orgcode', orgcode);
    search.set('site', site);
    search.set('depot', depot);
    search.set('huno', huno);
    search.set('hutype', hutype);
    return this.http.post(environment.urlapiDocument + '/print/labelhu', search.toString(), { responseType: 'blob' });
  }
}
