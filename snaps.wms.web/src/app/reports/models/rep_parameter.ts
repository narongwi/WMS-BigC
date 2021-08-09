import { lov } from '../../helpers/lov';

export class reportline {
    repcode: string;
    repname: string;
    repdesc: string;
    reproute: string;
    pams: reportpam[];
    modifyaccn: string;
    tiercode:string;
}

export class reportpam { 
    repcode: string;
    pamcode: string;
    pamname: string;
    pamtype: string;
    pamvalue: string;
    pamopt: lov[] = new Array();
}

