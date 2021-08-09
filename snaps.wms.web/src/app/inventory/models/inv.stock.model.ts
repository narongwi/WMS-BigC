export class stock_ls {
    orgcode         :string; 
    site            :string; 
    depot           :string; 
    spcarea         :string; 
    article         :string;
    pv              :number;
    lv              :number;
    barcode         :string;
    description     :string; 
    cronhand        :number;
    cronhandpu      :number;
    tflow           :string; 
    unitmanage      :string;
    unitratio       :number;
    huno            :string; 
    loccode         :string;
    inrefno         :string;
    dateexp         :Date | string | null; 
    serialno        :string;
    isblock         :string;
    dlcall          :number;
    dlcfactory      :number;
    dlcwarehouse    :number;
}
export class stock_pm { 
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    article: string; 
    description: string; 
    cronhand: number;  
    tflow: string; 
    stockid: string; 
    huno: string; 
    thcode: string; 
    inrefno: string; 
    inrefln: number; 
    loccode: string; 
    batchno: string;
    daterec: Date | string | null;
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    stkremarks: string; 

    serialno: string;
    hdivision: string;
    hdepartment: string;
    hsubdepart: string;
    hclass: string;
    isfastmove: number;
    ishighvalue: number;
    isslowmove: number;
    isdangerous: number;
    isdlc: number;
    isunique: number;
    isprescription: number;
    isalcohol: number;

    searchall: string;
    isblock     :string;
    dlcall          :number;
    dlcfactory      :number;
    dlcwarehouse    :number;
}
export class stock_ix extends stock_ls { 
}
export class stock_md extends stock_ls  {
    stockid: number;
    qtysku: number;
    qtypu: number;

    hutype: string; 
    huno: string; 
    hunosource: string; 
    thcode: string; 
    inrefno: string; 
    inrefln: number; 
    loccode: string; 
    pv: number; 
    lv: number; 
    qtyweight: number; 
    qtyvolume: number; 
    daterec: Date | string | null; 
    batchno: string; 
    lotno: string; 
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    serialno: string; 
    stkremarks: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    accnmodify: string;
    procmodify: string;
    
    datemodify: Date | string | null; 
    inagrn : string;
    ingrno : string;
    tflowsign:string;

}



// Movement
export class stock_mvin { 
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
    opscode: string;

}

export class stock_mvou {
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



export class stock_info extends stock_ls{
    thcode: string;
    thname: string; 
    unitmanage: string;
    unitratio: number;

    tihi: string;
    skuweight: number;
    dimension: string;
    lines: stock_md[];

    crincoming: number;
    cravailable: number;
    crbulknrtn: number;
    croverflow: number;
    crplanship: number;
    crprep: number;
    crstaging: number;
    crtask: number; 
    crreturn: number;
    crsinbin: number;
    crdamage: number;
    crblock: number; 
    crexchange: number;
    crrtv: number;

    isunique:number;
    isdlc:number;
    isbatchno:number;
}
