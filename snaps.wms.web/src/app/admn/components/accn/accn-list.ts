import { Component, OnInit, Input } from '@angular/core';
import { lov } from 'src/app/helpers/lov';
import { shareService } from 'src/app/share.service';
import { accn_ls } from '../../models/account.model'
@Component({
  selector: 'accn-list',
  template: `<div class="col-md-1 tx-center"> <img src="{{item.accnavartar}}" width="25" class="rounded" > </div>
  <div class="col-md-2 pdt-3"> {{item.accncode}} </div>
  <div class="col-md-6 pdt-3"> {{item.accnname}} {{item.accnsurname}}</div>
  <div class="col-md-3 pdt-3 tx-overflow"> <i class="w15 {{iconstate}} fa-lg" ></i>&nbsp;&nbsp;{{descstate}}</div>`  
})
export class listaccount implements OnInit {
  @Input() item: accn_ls;
  @Input() iconstate: string;
  @Input() descstate: string;
  constructor(private sv:shareService) { }
  ngOnInit() { }
  ngDecIcon(){ 
    
  }
}