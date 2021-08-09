export class thparty_ls {
    orgcode: string;
    site: string; 
    depot: string; 
    spcarea: string; 
    thtype: string; 
    thbutype: string; 
    thcode: string; 
    thname: string; 
    thgroup: string; 
    tflow: string; 
    thtypename: string; 
    thbutypename: string; 
    thnamealt: string;
    thcodealt: string;
    
    
}
export class thparty_pm extends thparty_ls { 
    telephone: string; 
    email: string; 
    mapaddress: string; 
    searchall:string;
}
export class thparty_ix extends thparty_ls { 
    vatcode: string; 
    thnameint: string; 
    addressln1: string; 
    addressln2: string; 
    addressln3: string; 
    subdistrict: string; 
    district: string; 
    city: string; 
    country: string; 
    postcode: string; 
    region: string; 
    telephone: string; 
    email: string; 
    thcomment: string; 
    throuteformat: string; 
    plandelivery: number; 
    naturalloss: number; 
    mapaddress: string; 
    fileid: string; 
    rowops: string; 
    ermsg: string; 
    dateops: Date | string | null; 
}
export class thparty_md extends thparty_ls  {
    vatcode: string;     
    thnameint: string; 
    addressln1: string; 
    addressln2: string; 
    addressln3: string; 
    subdistrict: string; 
    district: string; 
    city: string; 
    country: string; 
    postcode: string; 
    region: string; 
    telephone: string; 
    email: string; 
    thcomment: string; 
    throuteformat: string; 
    plandelivery: number; 
    naturalloss: number; 
    mapaddress: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    indock:string;
    oudock:string;
    traveltime:number;
}
