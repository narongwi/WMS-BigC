export interface outbound_ls {
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    ouorder: string; 
    outype: string; 
    ousubtype: string; 
    thcode: string; 
    dateorder: Date | string | null; 
    dateprep: Date | string | null; 
    dateexpire: Date | string | null; 
    oupriority: number; 
    oupromo: string; 
    orbitsource: string; 
    datereqdel: Date | string | null; 
    ouremarks: string; 
    tflow: string; 
    datedelivery: Date | string | null; 
    thname: string; 
    ousourcedsc: string; 
    selc:Boolean;

    disinorder:string;
    disproduct:string;
    disproductdesc:string;

    dateremarks:string;
    ousubtypedesc:string;

    dishuno:string
    disstockid:number;
    disunitops:string;
    disqtypnd:number;
    disloccode:string;
    dispv:number;
    dislv:number;

    inorder:string;
}
export class outbound_pm  { 
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    ouorder: string; 
    outype: string; 
    ousubtype: string; 
    thcode: string; 
    dateorder: Date | string | null; 
    dateprep: Date | string | null; 
    dateexpire: Date | string | null; 
    oupriority: number; 
    oupromo: string; 
    ousource: string; 
    datereqdel: Date | string | null; 
    ouremarks: string; 
    tflow: string; 
    datedelivery: Date | string | null; 
    thname: string; 
    ousourcedsc     :string; 
    stomobile       :string; 
    stoemail        :string; 
    ouflag          :string;
    dropship        :string;
    stopostcode     :string; 

    dateplanfrom    :Date | string | null;
    dateplanto      :Date | string | null;
    dateorderfrom   :Date | string | null;
    dateorderto     :Date | string | null;
    datereqfrom     :Date | string | null;
    datereqto       :Date | string | null;

    huno            :string;
    inorder         :string;

    ispending       :string;
}
export interface outbound_ix extends outbound_ls { 
    ouflag: string; 
    dropship: string; 
    stocode: string; 
    stoname: string; 
    stoaddressln1: string; 
    stoaddressln2: string; 
    stoaddressln3: string; 
    stosubdistict: string; 
    stodistrict: string; 
    stocity: string; 
    stocountry: string; 
    stopostcode: string; 
    stomobile: string; 
    stoemail: string; 
    fileid: string; 
    rowops: string; 
    ermsg: string; 
    opsdate: Date | string | null; 
}
export interface outbound_md extends outbound_ls  {
    ouflag: string; 
    dropship: string; 
    stocode: string; 
    stoname: string; 
    stoaddressln1: string; 
    stoaddressln2: string; 
    stoaddressln3: string; 
    stosubdistict: string; 
    stodistrict: string; 
    stocity: string; 
    stocountry: string; 
    stopostcode: string; 
    stomobile: string; 
    stoemail: string; 
    datereqdel: Date | string | null; 
    dateprocess: Date | string | null; 
    datedelivery: Date | string | null; 
    qtyorder: number; 
    qtypnd: number; 
    ouremarks: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 

    lines: outbouln_md[];
}



//Line
export class outbouln_ls {
    orgcode: string;
    site: string; 
    depot: string; 
    spcarea: string; 
    ouorder: string; 
    ouln: number; 
    ourefno: string; 
    barcode: string; 
    article: string; 
    pv: number; 
    lv: number; 
    unitops: string; 
    qtysku: number; 
    qtypu: number; 
    tflow: string; 
    articledsc: string; 
}
export class outbouln_pm extends outbouln_ls { 
    ourefln: number; 
    inorder: string; 
    serialno: string; 
}
export class outbouln_ix extends outbouln_ls {
    ourefln: number; 
    inorder: string; 
    qtyweight: number; 
    lotno: string; 
    expdate: Date | string | null; 
    serialno: string; 
    fileid: string; 
    rowops: string; 
    ermsg: string; 
    dateops: Date | string | null; 
}
export class outbouln_md extends outbouln_ls  {
    ourefln: number; 
    inorder: string; 
    qtyweight: number; 
    spcselect: string;
    batchno : string;
    lotno: string; 
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    serialno: string; 
    qtypnd: number; 
    datedelivery: Date | string | null; 
    qtyskudel: number; 
    qtypudel: number; 
    qtyweightdel: number; 
    oudnno: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    qtystock:number;
    tflowdesc:string;
    qtypndpu:number;
    qtyreqpu:number;
}
