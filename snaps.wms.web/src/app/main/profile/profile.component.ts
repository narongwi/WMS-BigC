import { Component, OnInit } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { accn_ls, accn_md, accn_priv } from 'src/app/admn/models/account.model';
import { adminService } from 'src/app/admn/services/account.service';
import { authService } from 'src/app/auth/services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  crtab:number = 1;
  pm:accn_ls = new accn_ls();
  mdaccn:accn_md = new accn_md();
  rsaccn: accn_priv =  new accn_priv();
  constructor(private mv: adminService, private av: authService,
    private toastr: ToastrService,
    private ngPopups: NgPopupsService ) {
    this.pm.accncode = this.av.crProfile.accncode;
  }

  ngOnInit(): void { this.accnget(); }
  accnget() { 
    this.av.myProfile().pipe().subscribe((res)=> { this.mdaccn = res; });
  } 
  changeprive() { 
    this.ngPopups.confirm('Confirm change password ?')
    .subscribe(res => {
        if (res) {
            this.av.changePriv(this.rsaccn).pipe().subscribe(            
                (res) => { 
                  this.rsaccn =  new accn_priv();
                  this.toastr.success("<span class='fn-07e'>Change password success, redicrecto login</span>",null,{ enableHtml : true }); 
                }
            );
                     
        } 
    });
  }
}
