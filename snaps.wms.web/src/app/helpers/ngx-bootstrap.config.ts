
import { Component, OnInit,OnDestroy, ViewChild, Injectable  } from '@angular/core';
import { DatePipe } from '@angular/common';
import { NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct, NgbInputDatepickerConfig, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { authService } from '../auth/services/auth.service';

@Injectable()
export class CustomDateParserFormatter extends NgbDateParserFormatter {
  readonly DELIMITER = '/';
  constructor(private Datepipe:DatePipe, private av:authService){  super(); }
  parse(value: string): NgbDateStruct | null {
    if (value) {
      let date = value.split(this.DELIMITER);
      return {
        day : parseInt(date[0], 10),
        month : parseInt(date[1], 10),
        year : parseInt(date[2], 10)
      };
    }
    return null;
  }  
  format(indate: NgbDateStruct | null): string {
    return indate ? this.Datepipe.transform(Date.parse(indate.year+"-"+indate.month+"-"+indate.day), this.av.crProfile.formatdate) : '';
  }
}

@Injectable()
export class CustomAdapter extends NgbDateAdapter<Date> {
  constructor(private Datepipe:DatePipe){  super(); }
  fromModel(value: Date | null ): NgbDateStruct | null {
    if (value){ 
      value = new Date(value);
      //console.log("Current value " + value);
      return { 
        day : value.getDate(),
        month : value.getMonth()+1,
        year : value.getFullYear()
      }
    }
  }
  getDate(indate: NgbDateStruct):Date{ 
    let rn = new Date();
    //rn = new Date(indate.year,indate.month-1,indate.day);
    rn = new Date(Date.UTC(indate.year, indate.month-1, indate.day, 0, 0, 0))
    // console.log("Prov date +0 " + new Date(indate.year,indate.month,indate.day));
    // console.log("Prov date -1 " + new Date(indate.year,indate.month-1,indate.day));

    // console.log("Current Indate " + indate.year + " : " + indate.month + " : " + indate.day);
    // console.log("Current Directive Date " + rn);
    return rn;
  }
  toModel(indate: NgbDateStruct | null) : Date | null { 
     return indate ? 
      //new Date().setDate(new Date() { day: indate.day, month: indate.month, year: indate.year, hour: 0, minutes: 0, seconds : 0, milliseconds: 0 })
      this.getDate(indate)
     : null;
    // //this.Datepipe.transform(indate.year+"-"+indate.month+"-"+indate.day, "yyyy-MM-dd")
    // : null;
  }
}

export class NgbdPaginationConfig {
    page = 4;
    constructor(config: NgbPaginationConfig) {
      // customize default values of paginations used by this component tree
      config.size = 'sm';
      config.boundaryLinks = true;    
    }
}
  