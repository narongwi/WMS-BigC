import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AdmReportService {
  constructor(private http: HttpClient) { }

  public lists() {
    let _headers = new HttpHeaders().set('Content-Type', 'application/json');// .append('Authorization', `Bearer ${this.token}`);
    return this.http.get(environment.urlapiReport + "/api/report", { headers: _headers });
  }

  public Export(fromData: any): Observable<Blob> {
    let _headers = new HttpHeaders().set('Content-Type', 'application/json');// .append('Authorization', `Bearer ${this.token}`);
    return this.http.post<Blob>(environment.urlapiReport + "/api/report", fromData, { headers: _headers, responseType: "blob" as "json" });
  }
}
