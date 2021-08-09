export class correction_ls { 
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    dateops: Date | string | null; 
    article: string; 
    pv: number; 
    lv: number; 
    unitops: string; 
    qtypu: number; 
    tflow: string; 
    description: string; 
    accnops: string; 
    accnname: string;
    codeops: string; 
    typeops: string;
    dlcall          :number;
    dlcfactory      :number;
    dlcwarehouse    :number;
}

export class correction_pm extends correction_ls { 

}
export class correction_ix extends correction_ls { 

}

export class correction_md extends correction_ls { 
    stockbeforesku : number; 
    stockbeforepu : number;
    variancesku : number; 
    variancepu : number;
    aftersku : number;
    afterpu : number;

    unitmanage: string;
    unitratio: number;

    seqops: string; 

    thcode: string; 
    qtysku: number; 
    qtyweight: number; 
    qtyvolume: number; 
    inreftype: string; 
    inrefno: string; 
    ingrno: string; 
    inagrn : string;
    inpromo: string; 
    reason: string; 
    stockid: number; 
    huno: string; 
    loccode: string; 
    daterec: Date | string | null; 
    batchno: string; 
    lotno: string; 
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    serialno: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    procmodify: string; 

}
