export interface exsInbound { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    thcode: string;
    intype: string;
    subtype: string;
    inorder: string;
    dateorder: Date | string | null;
    dateplan: Date | string | null;
    dateexpire: Date | string | null;
    inpriority: number;
    inflag: string;
    inpromo: string;
    tflow: string;
    orbitsource: string;
    fileid: string;
    rowops: string;
    ermsg: string;
    dateops: Date | string | null;
    rowid: number;
}


export interface exsInbouln { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    inorder: string;
    inln: string;
    inrefno: string;
    inrefln: number;
    article: string;
    pv: number;
    lv: number;
    unitops: string;
    qtysku: number;
    qtypu: number;
    qtyweight: number;
    batchno: string;
    lotno: string;
    expdate: Date | string | null;
    serialno: string;
    orbitsource: string;
    tflow: string;
    fileid: string;
    rowops: string;
    ermsg: string;
    dateops: Date | string | null;
    rowid: number;
}