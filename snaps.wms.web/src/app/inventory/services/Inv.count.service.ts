import { product_vld } from './../models/inv.count.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { lov } from '../../helpers/lov';
import {
  stock_info,
  stock_ls,
  stock_md,
  stock_pm,
} from '../models/inv.stock.model';
import { correction_md } from '../models/inv.correction.mode';
import { transfer_md } from '../models/inv.transfer.model';
import { shareService } from 'src/app/share.service';
import {
  confirmline_md,
  countcorrection_md,
  countline_md,
  countplan_md,
  counttask_md,
} from '../models/inv.count.model';
import { Observable, of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class countService {
  public u: string;
  public guid: string;

  constructor(private http: HttpClient, private ss: shareService) {
    this.guid = this.ss.getId();
  }

  //Task
  listTask(o: counttask_md) {
    return this.http
      .post<counttask_md[]>(
        environment.urlapiInvt + '/count/listTask/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <counttask_md[]>res));
  }
  getTask(o: counttask_md) {
    return this.http
      .post<counttask_md>(
        environment.urlapiInvt + '/count/getTask/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <counttask_md>res));
  }
  upsertTask(o: counttask_md) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/upsertTask/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }
  removeTask(o: counttask_md) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/removeTask/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }

  //Plan
  listPlan(o: counttask_md) {
    return this.http
      .post<countplan_md[]>(
        environment.urlapiInvt + '/count/listPlan/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(
        map((res: countplan_md[]) => res),
        catchError(this.handleError<countplan_md[]>('countplan_md', []))
      );
  }
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
  private log(message: string) {
    console.log(message);
  }

  getPlan(o: countplan_md) {
    return this.http
      .post<countplan_md>(
        environment.urlapiInvt + '/count/getPlan/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <countplan_md>res));
  }
  upsertPlan(o: countplan_md) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/upsertPlan/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }
  removePlan(o: countplan_md) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/removePlan/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }
  validatePlan(o: countplan_md) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/validatePlan/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }

  //Line
  listLineAsync(o: countplan_md) {
    return this.http
      .post<countline_md[]>(
        environment.urlapiInvt + '/count/listLine/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <countline_md[]>res));
  }
  //Line
  countLineAsync(o: countplan_md) {
    return this.http
      .post<countline_md[]>(
        environment.urlapiInvt + '/count/countline/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <countline_md[]>res));
  }
  upsertLineAsync(o: countline_md[]) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/upsertLine/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }
  deleteLineAsync(o: countline_md) {
    return this.http
      .post(
        environment.urlapiInvt + '/count/deleteLine/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => res));
  }
  getcountlist(o: countplan_md): any {
    let search = new URLSearchParams();
    search.set('orgcode', o.orgcode);
    search.set('site', o.site);
    search.set('depot', o.depot);
    search.set('countcode', o.countcode);
    search.set('plancode', o.plancode);
    return this.http.post(
      environment.urlapiDocument + '/print/CountSheet',
      search.toString(),
      { responseType: 'blob' }
    );

    // return this.http.get(environment.urlapiDocument +'/get/CountSheet?orgcode='+o.orgcode+'&site='+o.site+'&depot='+o.depot+'&countcode='+o.countcode+'&plancode='+ o.plancode, { responseType: 'blob' });
  }

  //confirm
  listConfirm(o: counttask_md) {
    return this.http
      .post<countcorrection_md[]>(
        environment.urlapiInvt + '/count/getConfirmLine/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <countcorrection_md[]>res));
  }
  countConfirm(o: counttask_md) {
    return this.http.post(
      environment.urlapiInvt + '/count/countconfirm/' + this.guid,
      JSON.stringify(o)
    );
  }
  generatehu(o: product_vld) {
    return this.http
      .post<countline_md>(
        environment.urlapiInvt + '/count/generatehu/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <countline_md>res));
  }
  findProduct(o: product_vld) {
    return this.http
      .post<product_vld>(
        environment.urlapiInvt + '/count/findproduct/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <product_vld>res));
  }
  validatehu(o: product_vld) {
    return this.http
      .post<countline_md>(
        environment.urlapiInvt + '/count/validatehu/' + this.guid,
        JSON.stringify(o)
      )
      .pipe(map((res) => <countline_md>res));
  }
  label(
    orgcode: string,
    site: string,
    depot: string,
    huno: string,
    hutype: string
  ) {
    let search = new URLSearchParams();
    search.set('orgcode', orgcode);
    search.set('site', site);
    search.set('depot', depot);
    search.set('huno', huno);
    search.set('hutype', hutype);
    return this.http.post(
      environment.urlapiDocument + '/print/labelhu',
      search.toString(),
      { responseType: 'blob' }
    );
  }
}
