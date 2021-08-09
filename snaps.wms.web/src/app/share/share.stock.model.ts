export interface stock_mvin { 
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    stockid: number; 
    inorder: string; 
    inln: number; 
    inrefno: string; 
    inrefln: number; 
    barcode: string; 
    article: string; 
    pv: number; 
    lv: number; 
    unitops: string; 
    qtyskurec: number; 
    qtypurec: number; 
    qtyweightrec: number; 
    qtynaturalloss: number; 
    daterec: Date | string | null; 
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    batchno: string; 
    lotno: string; 
    serialno: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 

    //Additional
    procmodify: string;
    hutype: string;
    loccode: string;
    qtypu: number;
    qtysku: number;
    qtyvolume: number;
    qtyweight: number;
    huno: string;
    stkremarks: string; 
    tflow: string; 
    thcode: string;
}

export interface stock_mvou {
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    stockid: number; 
    article: string; 
    pv: number; 
    lv: number; 
    opssku: number; 
    opspu: number; 
    opsweight: number; 
    opsdate: Date | string | null; 
    opstype: string; 
    opscode: string; 
    opsroute: string; 
    opsthcode: string; 
    stockhash: string;
}
