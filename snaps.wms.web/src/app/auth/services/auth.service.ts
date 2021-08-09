import { Injectable, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable, VirtualTimeScheduler } from 'rxjs';
import { environment } from  '../../../environments/environment';
import { accn_acs, accn_signup } from '../models/accn_signup';
import { resultRequest } from '../../helpers/resultRequest';
import { NgProgressComponent } from 'ngx-progressbar';
import { NgControlStatus } from '@angular/forms';
import { accn_md, accn_priv, accn_profile, accn_roleacs } from 'src/app/admn/models/account.model';
import { ConsoleService } from '@ng-select/ng-select/lib/console.service';
import { shareService } from 'src/app/share.service';
import { lov } from 'src/app/helpers/lov';

@Injectable({ providedIn: 'root' })
export class authService {
    public u:string;
    public guid:string;
    public site:string;

    public crAccnOb: Observable<accn_profile>;
    public crAccnbhv: BehaviorSubject<accn_profile>;

    public crAccsOb: Observable<accn_acs>;
    public crAccsbhv: BehaviorSubject<accn_acs>;

    public crRoleOb: Observable<accn_roleacs>;
    public crRolebhv: BehaviorSubject<accn_roleacs>;



    public crProfile: accn_profile = new accn_profile();
    public crAccs: accn_acs;
    public crRole: accn_roleacs = new accn_roleacs();

    constructor(private http: HttpClient) {       
        
        this.crAccsbhv = new BehaviorSubject<accn_acs>(this.crAccs);
        this.crAccsOb = this.crAccsbhv.asObservable();        
    
        this.crAccnbhv = new BehaviorSubject<accn_profile>(this.crProfile);
        this.crAccnOb = this.crAccnbhv.asObservable();
       
    }
    public getProfile() : void {
         
        if (!!localStorage.getItem("snapsacnwms")) { 

            this.decodeProfile();
        }else { 
            if (this.crAccs == null || this.crAccs == undefined) { 
                this.retriveAccess();
            }
            
            this.retriveProfile(this.site).subscribe( 
                (res) => { 
                localStorage.setItem("snapsacnwms", this.enc(JSON.stringify(res))); this.decodeProfile();  }
            );;
        }
    }
    // public getProfile(site:string) : void {
         
    //     if (!!localStorage.getItem("snapsacnwms")) { 

    //         this.decodeProfile();
    //     }else { 
    //         if (this.crAccs == null || this.crAccs == undefined) { 
    //             this.retriveAccess();
    //         }
            
    //         this.retriveProfile(site).subscribe( 
    //             (res) => { 
    //             localStorage.setItem("snapsacnwms", this.enc(JSON.stringify(res))); this.decodeProfile();  }
    //         );;
    //     }
    // }
    getWarehouse() { 
        return  this.http.post<lov[]>(environment.urlapiAdmn + "/LOV/allwarehouse/"+this.gencode(), JSON.stringify(""))
                .pipe( map(res=><lov[]>res));
    } 
    public get getToken() { 
        if (!!this.crAccs){
            return this.crAccs.accscode;
        }else { 
            if (localStorage.getItem("snapsacs")){ 
                this.crAccs = JSON.parse(this.dec(localStorage.getItem('snapsacs')));
                this.crAccsbhv.next(this.crAccs);
                this.crAccsOb = this.crAccsbhv.asObservable();
                return this.crAccs.accscode; 
            }else { 
                return null;
            }            
        }        
    }
    public get getKey() { 
        if (!!this.crAccs){
            return this.crAccs.accnkey;
        }else { 
            return null;
        } 
    }

    public retriveAccess() : void {
        if (!!localStorage.getItem('snapsacs')) {
            if (this.crAccs == undefined|| this.crAccs == null) { 
                this.crAccs = JSON.parse(this.dec(localStorage.getItem('snapsacs')));
                this.crAccsbhv.next(this.crAccs);
                this.crAccsOb = this.crAccsbhv.asObservable();
            }
        }else {
            return null;
        }
    }

    public get isAuthen() : Boolean { 
        return !!localStorage.getItem("snapsacs");
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
        return [this.u.substr(0,8), this.u.substr(8,4), 'snaps-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
    }
    verify(o:accn_signup)  { 
        this.site = o.site;
        return this.http.post<resultRequest>(environment.urlapiAuth + "/Auth/verify/"+this.gencode(), JSON.stringify(o))
                .pipe( map( 
                    res => { 
                            this.crAccs = new accn_acs(res.message,res.reqid);
                            localStorage.setItem("snapsacs", this.enc(JSON.stringify(this.crAccs)));
                            this.crAccsbhv = new BehaviorSubject<accn_acs>(this.crAccs);
                            this.crAccsbhv.next(this.crAccs);
                            this.crAccsOb = this.crAccsbhv.asObservable();
                            }));
    }
    signup(o:accn_signup) { 
        return  this.http.post<accn_acs>(environment.urlapiAuth + "/Auth/signup/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><accn_acs>res));
    }
    signout() { 
        localStorage.removeItem('snapsacnwms'); localStorage.removeItem('snapsacs');
    }


    changeProfile(o: accn_roleacs) { // use for change role permission on the air 
        this.crRole = o;
        this.crRolebhv = new BehaviorSubject<accn_roleacs>(this.crRole);
        this.crRolebhv.next(this.crRole); 
        this.crRoleOb = this.crRolebhv.asObservable();
    }
    decodeProfile(){ 
        this.crProfile = JSON.parse(this.dec(localStorage.getItem('snapsacnwms')));
        this.crAccnbhv = new BehaviorSubject<accn_profile>(this.crProfile);
        this.crAccnbhv.next(this.crProfile); 
        this.crAccnOb = this.crAccnbhv.asObservable();

        this.crRole = this.crProfile.roleaccs;
        this.crRolebhv = new BehaviorSubject<accn_roleacs>(this.crRole);
        this.crRolebhv.next(this.crRole); 
        this.crRoleOb = this.crRolebhv.asObservable();
    }
    retriveProfile(site:string) {
       return this.http.post<accn_profile>(environment.urlapiAdmn + "/Account/getProfile/" + site +"/" + this.gencode(), JSON.stringify("Tron alive"))
            .pipe(map(res=>{ 
                localStorage.setItem("snapsacnwms", this.enc(JSON.stringify(res))); this.decodeProfile();  }
            ));
    }

    myProfile() {         
        return  this.http.post<accn_md>(environment.urlapiAdmn + "/Account/myAccount/"+this.gencode(), JSON.stringify("Tron alive"))
                .pipe(map(res=><accn_md>res));
    }
    changePriv(o:accn_priv) { 
        //changePriv
        return  this.http.post<accn_md>(environment.urlapiAdmn + "/Account/changePriv/"+this.gencode(), JSON.stringify(o))
                .pipe(map(res=><accn_md>res));
    }
}