export class warehouse_ls {
    orgcode: string;
    siteid:number;
    sitecode: string; 
    sitename: string; 
    sitetype: string; 
    datestart: Date | string | null; 
    dateend: Date | string | null; 
    tflow: string; 
    sitenamealt: string; 
}
export class warehouse_pm extends warehouse_ls { 
    sitenamealt: string; 
}
export class warehouse_ix extends warehouse_ls { 

}
export class warehouse_md extends warehouse_ls  {

    sitekey: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
}