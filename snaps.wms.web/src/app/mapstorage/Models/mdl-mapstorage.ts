export class locup_ls {
    orgcode : string;
    site: string; 
    depot: string; 
    spcarea: string; 
    fltype: string; 
    fltypedsc: string; 
    lslevel: string; 
    lscode: string; 
    lszone: string; 
    lsaisle: string; 
    lsbay: string; 
    crfreepct: number; 
    tflow: string; 
    datemodify: Date | string | null; 
}
export class locup_pm extends locup_ls { 
    tflowcnt: string; 
}
export class locup_ix extends locup_ls { 
}
export class locup_md extends locup_ls  {
    lsformat: string; 
    lsseq: number; 
    lsdesc: string;
    lscodealt: string; 
    lscodefull: string; 
    lscodeid: string;
    crweight: number; 
    crvolume: number; 
    crlocation: number; 
    tflowcnt: string; 
    lshash: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    accnmodify: string; 
    procmodify: string; 

}
export class locup_prc { 
    opsaccn: string;
    opsobj: locup_md; 
}

export class locdw_ls {
    orgcode : string;
    site: string; 
    depot: string; 
    spcarea: string; 
    fltype: string; 
    lszone: string; 
    lsaisle: string; 
    lsbay: string; 
    lslevel: string; 
    lsstack: string; 
    lscode: string; 
    lscodealt:string;
    crweight: number; 
    crvolume: number; 
    crfreepct: number;     
    spcpicking: number;
    tflow: string; 
    spcareaname:string;
    lszonename:string;
}
export class locdw_pm extends locdw_ls { 
    spchuno: string; 
    spcarticle: string; 
    spcblock: number; 
    spctaskfnd: string; 
    spcseqpath: number; 
    spclasttouch: string; 
    spcpivot: string; 
    spcpickunit: string;
    lsloctype: string; 


    zone:string;
    aislefrom:string;
    aisleto:string;
    bayfrom:string;
    bayto:string;
    levelfrom:string;
    levelto:string;
    locationfrom:string;
    locationto:string;
    mixproduct:string;
    mixaging:string;
    mixlot:string;
    ispicking:string;
    isreserve:string;
    spcthcode:string;
    spcproduct:string;
    lasttourch:string;

}
export class locdw_ix extends locdw_ls { 
}
export class locdw_md extends locdw_ls  {
    lsdesc:string;
    lsseq:number;
    lsloc:string;
    lscodealt: string; 
    lscodefull: string;
    lscodeid: string;
    lsdmlength: number; 
    lsdmwidth: number; 
    lsdmheight: number; 
    lsmxweight: number; 
    lsmxvolume: number; 
    lsmxlength: number; 
    lsmxwidth: number; 
    lsmxheight: number; 
    lsmxhuno: number; 
    lsmnsafety: number; 
    lsmixarticle: number; 
    lsmixage:number;
    lsmixlotno:number;
    lsloctype: string; 
    lsremarks: string; 
    lsgaptop: number; 
    lsgapleft: number; 
    lsgapright: number; 
    lsgapbuttom: number; 
    lsdigit: string; 
    lsstackable:number;
    spcthcode: string; 
    spchuno: string; 
    spcarticle: string; 
    spcblock: number; 
    spctaskfnd: string; 
    spcseqpath: number; 
    spclasttouch: string; 
    spcpivot: string; 
    spcpickunit: string; 
    spcrpn: string;
    spcmnaging: number;
    spcmxaging: number;
    tflowcnt: string; 
    lshash: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    lsstacklimit:number;
}

export class locdw_gn { 
    orgcode:string;
    site:string;
    depot:string;
    spcarea:string;
    zone:string;
    aislefr:string;
    aisleto:string;
    bayfr:string;
    bayto:string;
    levelfr:string;
    levelto:string;
    location:number;
    tflow:string;
    lsseq:number;
    lsdmlength: number; 
    lsdmwidth: number; 
    lsdmheight: number; 
    lsmxweight: number; 
    lsmxvolume: number; 
    lsmxlength: number; 
    lsmxwidth: number; 
    lsmxheight: number; 
    lsmxhuno: number; 
    lsmnsafety: number; 
    lsmixarticle: number; 
    lsmixage:number;
    lsmixlotno:number;
    lsloctype: string; 
    lsremarks: string; 
    lsgaptop: number; 
    lsgapleft: number; 
    lsgapright: number; 
    lsgapbuttom: number; 
    lsdigit: string; 
    lsstackable:number;
    lsstacklimit:number;
    spcthcode: string; 
    spchuno: string; 
    spcarticle: string; 
    spcblock: number; 
    spctaskfnd: string; 
    spcseqpath: number; 
    spclasttouch: string; 
    spcpivot: string;
    spcpicking: number;
    spcpickunit: string; 
    spcrpn: string;
    spcmnaging: number;
    spcmxaging: number;
    lsformat:string;
    lsstacklabel:string;
}

export class locdw_gngrid { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    zone: string;
    lsloc: number;
    lsseq: number; 
    tflow: string;
    lsfornat: string; 
    lslength: string;
    lswidth: string;
    lsmxhuno: number;
    lsmxweight: number; 
    lsmxvolume: number; 
    spcthcode: string;
    lsmixarticle: number;
    location:number;
    lsformat:string;
}

export class locdw_pivot extends locdw_ls { 
    spcpivot: string; 
}
export class locdw_picking extends locdw_ls { 
    spcpickunit: string; 
    spcseqpath: number;
    spcrpn: string;
    spcarticle: string;
    lsmnsafety: number;
    
}
