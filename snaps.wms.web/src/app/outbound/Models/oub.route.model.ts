export class route_thsum { 
    orgcode: string; 
    site: string; 
    depot: string; 
    thcode: string;
    thname: string;
    crroute: number;
    crhu: number;
    crweight: number;
    crvolume: number;
    crcapacity: number;
    crophu: number;

}

export class route_ls {
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    routetype: string; 
    routeno: string; 
    oupromo: string; 
    thcode: string; 
    plandate: Date | string | null; 
    utlzcode: string; 
    tflow: string; 
    driver: string; 
    thname: string; 
    routetypename: string; 
    oupriority: number; 
    datereqdelivery: Date | string | null; 
    crhu: number; 
    crweight: number; 
    crvolume: number; 
    crcapacity: number; 
    crophu:number;
    remarks: string;
}
export class route_pm { 
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    routetype: string; 
    routeno: string; 
    oupromo: string; 
    thcode: string; 
    plandate: Date | string | null; 
    utlzcode: string; 
    tflow: string; 
    driver: string; 
    thname: string; 
    routetypename: string; 
    oupriority: number; 
    datereqfrom: Date | string | null; 
    datereqto: Date | string | null;
    remarks: string;
    datedeliveryfrom : Date | string | null;
    datedeliveryto : Date | string | null;
    stating:string;
    trttype:string; 
    transportor:string;
    loadtype:string;
    paymenttype:string;
    trucktype:string;
    iscombine : string;
}
export class route_ix extends route_ls { 
}
export class route_md extends route_ls  {
    routename: string; 
    trttype: string; 
    loadtype: string; 
    trucktype: string; 
    trtmode: string; 
    loccode: string; 
    paytype: string; 
    loaddate: Date | string | null; 
    dateshipment: Date | string | null; 
    relocationto: string; 
    relocationtask: string; 
    shipper: string; 
    mxhu: number; 
    mxweight: number; 
    mxvolume: number; 
    crhu: number; 
    crweight: number; 
    crvolume: number; 
    plateNo: string; 
    contactno: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    hus: route_hu[];
    allocate : route_hu[];
    transportor: string;
    remarks: string;
    postpone:boolean;
    sealno:string;
    routesource:string;
    outrno:string;
    datereqdeliverytime: string;
}


export class route_hu {    
    huno: string; 
    loccode: string; 
    worker: string; 
    tflow: string;
    routeno: string;
    crsku: number; 
    priority: number;
    crweight: number; 
    crvolume: number; 
    crcapacity: number; 
    accnmodify: string;
    opstype: string;
    opscode: string;
}
