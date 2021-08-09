export class depot_ls {
    orgcode: string; 
    sitecode: string; 
    depottype: string; 
    depotcode: string; 
    depotname: string; 
    tflow: string; 
}
export class depot_pm extends depot_ls { 
}
export class depot_ix extends depot_ls { 
}
export class depot_md {
    orgcode: string;
    sitecode: string;
    depottype: string;
    depotcode: string;
    depotname: string;
    depotnamealt: string;
    datestart: Date | string | null;
    dateend: Date | string | null;
    depotkey: string;
    depotops: string;
    tflow: string;
    datecreate: Date | string | null;
    accncreate: string;
    datemodify: Date | string | null;
    accnmodify: string;
    procmodify: string;
    depothash: string;
    unitweight: string;
    unitvolume: string;
    unitdimension: string;
    formatdate: string;
    formatdateshort: string;
    formatdatelong: string;
    authcode: string;
    authdigit: string;
    authdate: Date | string | null;
}