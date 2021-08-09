import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from  '../../../environments/environment';
import { resultRequest } from '../../helpers/resultRequest';
import { pam_parameter } from '../models/adm.parameter.model';

@Injectable({ providedIn: 'root' })
export class admparameterService {
    public u:string;
    public guid:string;

    constructor(private http: HttpClient) { }
    private gencode() { 
        this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
        return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }

    find(){
        return  this.http.post<pam_parameter[]>(environment.urlapiAdmn + "/admparameter/getParameterList/"+this.gencode(), "")
                .pipe( map(res=><pam_parameter[]>res));
    }
    set(o:pam_parameter){
        return  this.http.post(environment.urlapiAdmn + "/admparameter/updateParameter/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=>res));
    }


}