export class barcode_ls {
    orgcode: string;
    site: string; 
    depot: string; 
    spcarea: string; 
    article: string; 
    pv: number; 
    lv: number; 
    bartype: string;
    barops: string; 
    barcode: string; 
    thname: string; 
    thcode: string; 
    tflow: string; 
    articledsc: string; 

    bartypedesc : string;
    baropsdesc : string;
    isprimary : number;
}
export class barcode_pm extends barcode_ls { 
    searchall:string;
}
export class barcode_ix extends barcode_ls { 
    fileid: string; 
    rowops: string; 
    ermsg: string; 
    dateops: Date | string | null; 
}
export class barcode_md extends barcode_ls  {
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    barremarks: string;
}