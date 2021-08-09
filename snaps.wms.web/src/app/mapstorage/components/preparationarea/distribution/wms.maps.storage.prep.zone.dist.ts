import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { mapsprepzonestockService } from '../../../services/wms.maps.storage.prep.service';
import { authService } from '../../../../auth/services/auth.service';
import { zoneprep_md } from '../../../Models/wms.maps.storage.prep.zone.model';
import { mapsprepzonedistlineComponent } from './linedistarea/wms.maps.storage.prep.zone.dist.line';
declare var $: any;
@Component({
  selector: 'app-maps-prep-zone-dist',
  templateUrl: 'wms.maps.storage.prep.zone.dist.html'
})
export class mapsprepzonedistComponent implements OnInit {
  @ViewChild(mapsprepzonedistlineComponent) sobline: mapsprepzonedistlineComponent;
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
