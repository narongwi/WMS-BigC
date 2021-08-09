import { Component, OnInit, Input } from '@angular/core';
import { lov } from '../../../helpers/lov';
import { role_ls } from '../../models/role.model';
@Component({
  selector: 'role-list',
  templateUrl: 'role-list.html'
})
export class listrole implements OnInit {
  @Input() item: role_ls;
  @Input() iconstate: string;
  constructor() { }

  ngOnInit() { }
  
}