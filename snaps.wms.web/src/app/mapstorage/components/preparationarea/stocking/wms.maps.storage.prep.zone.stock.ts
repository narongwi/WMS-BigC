import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { mapsprepzonestockService } from '../../../services/wms.maps.storage.prep.service';
import { authService } from '../../../../auth/services/auth.service';
import { zoneprep_md } from '../../../Models/wms.maps.storage.prep.zone.model';
import { mapsprepzonestocklineComponent } from './linestockarea/wms.maps.storage.prep.zone.stock.line';
declare var $: any;
@Component({
  selector: 'app-maps-prep-zone-stock',
  templateUrl: 'wms.maps.storage.prep.zone.stock.html'
})
export class mapsprepzonestockComponent implements OnInit {
  @ViewChild(mapsprepzonestocklineComponent) sobline: mapsprepzonestocklineComponent;
  dateformat:string = "";
  dateformatlong:string = "";
  slczone:string = "";
  crtab:number = 1;
  constructor(private sv: mapsprepzonestockService,
              private av: authService, 
              private router: RouterModule,
              private toastr: ToastrService) { 
      this.av.retriveAccess(); 
      this.dateformat = this.av.crProfile.formatdate;
      this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void { }
  ngOnDestroy():void {  }
  ngAfterViewInit(){ }
  ngSelect(o:zoneprep_md){ this.slczone = o.przone; this.sobline.ngOpsIn(o);  this.crtab = 2;}
}
