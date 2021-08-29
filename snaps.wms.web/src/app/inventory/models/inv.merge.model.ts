export interface merge_set {
    orgcode?: string;
    site?: string;
    depot?: string;
    spcarea?: string;
    mergeno?: number;
    hutype?: string;
    huno?: string;
    loccode?: string;
    accncode?: string;
}

export interface merge_find {
    orgcode?: string;
    site?: string;
    depot?: string;
    spcarea?: string;
    loccode?: string;
    huno?: string;
    article?: string;
    accncode?: string;
    datecreate?: Date | string | null;
}
export interface mergehu_md {
    orgcode?: string;
    site?: string;
    depot?: string;
    spcarea?: string;
    mergeno?: number;
    hutype?: string;
    hutarget?: string;
    loccode?: string;
    tflow?: string;
    tflowdes?: string;
    datecreate?: string;
    accncreate?: string;
    datemodify?: string;
    accnmodify?: string;
    procmodify?: string;
    remarks?: string;
}

export interface mergehu_ln {
    mergeln?: number;
    mergeno?: number;
    orgcode?: string;
    site?: string;
    depot?: string;
    spcarea?: string;
    stockid?: number;
    hutype?: string;
    loccode?: string;
    huno?: string;
    inrefno?: string;
    inrefln?: number | null;
    inagrn?: string;
    ingrno?: string;
    article?: string;
    pv?: number;
    lv?: number;
    descalt?: string;
    qtysku?: number;
    qtypu?: number;
    qtyweight?: number;
    qtyvolume?: number;
    qtyunit?: string;
    qtyunitdes?: string;
    daterec?: string;
    batchno?: string;
    lotno?: string;
    datemfg?: string | null;
    serialno?: string;
    dateexp?: string | null;
    tflowops?: string;
    tflowdes?: string;
    tflowsign?: string;
    skuops?: number;
    puops?: number;
    weightops?: number;
    volumeops?: number;
    unitops?: string;
    unitopsdes?: string;
    refops?: string;
    reflnops?: number | null;
    remarks?: string;
    tflow?: string;
    datecreate?: string;
    accncreate?: string;
    datemodify?: string;
    accnmodify?: string;
    procmodify?: string;
    msgops?: string;
}

export interface merge_md extends mergehu_md {
    lines?: mergehu_ln[];
}