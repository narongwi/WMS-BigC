export interface exsBarcode { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    article: string;
    pv: number;
    lv: number;
    barops: string;
    barcode: string;
    bartype: string;
    thcode: string;
    orbitsource: string;
    tflow: string;
    fileid: string;
    rowops: string;
    ermsg: string;
    dateops: Date | string | null;
    rowid: number;
}