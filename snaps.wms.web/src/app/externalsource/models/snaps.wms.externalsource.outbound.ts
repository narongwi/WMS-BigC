    export interface exsOutbound { 
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
        ouflag: string;
        oupromo: string;
        dropship: string;
        orbitsource: string;
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
        tflow: string;
        inorder: string;
        fileid: string;
        rowops: string;
        ermsg: string;
        dateops: Date | string | null;
        rowid: number;
    }

 export interface exsOutbouln { 
        orgcode: string;
        site: string;
        depot: string;
        spcarea: string;
        ouorder: string;
        ouln: string;
        ourefno: string;
        ourefln: string;
        inorder: string;
        article: string;
        pv: number;
        lv: number;
        unitops: string;
        qtysku: number;
        qtypu: number;
        qtyweight: number;
        spcselect: string;
        batchno: string;
        lotno: string;
        datemfg: Date | string | null;
        dateexp: Date | string | null;
        serialno: string;
        orbitsource: string;
        tflow: string;
        disthcode: string;
        fileid: string;
        rowops: string;
        ermsg: string;
        dateops: Date | string | null;
        rowid: number;
        ouseq: number;
 }

 export interface exsLocdw { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    fltype: string;
    lszone: string;
    lsaisle: string;
    lsbay: string;
    lslevel: string;
    lsloc: string;
    lsstack: string;
    lscode: string;
    lscodealt: string;
    lscodefull: string;
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
    lsmixage: number;
    lsmixlotno: number;
    lsloctype: string;
    lsgaptop: number;
    lsgapleft: number;
    lsgapright: number;
    lsgapbuttom: number;
    lsstackable: number;
    lsdigit: string;
    spcthcode: string;
    spchuno: string;
    spcarticle: string;
    spcmnaging: number;
    spcmxaging: number;
    lsdirection: string;
    spcpathrecv: number;
    spcpathpick: number;
    spcpathdist: number;
    fileid: string;
    rowops: string;
    ermsg: string;
    dateops: Date | string | null;
    rowid: number;
 }

 export interface exsLocup { 

    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    fltype: string;
    lscode: string;
    lsseq: number;
    lscodealt: string;
    lscodefull: string;
    lscodeid: string;
    lszone: string;
    lsaisle: string;
    lsbay: string;
    lslevel: string;
    tflow: string;
    lsdesc: string;
    lsloctype: string;
    fileid: string;
    rowops: string;
    ermsg: string;
    dateops: Date | string | null;
    rowid: number;
 }

 export interface exsPrep { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    fltype: string;
    przone: string;
    lszone: string;
    lsaisle: string;
    lsbay: string;
    lslevel: string;
    lsloc: string;
    lsstack: string;
    lscode: string;
    spcproduct: string;
    spcpv: string;
    spclv: string;
    spcunit: string;
    spcthcode: string;
    lsdirection: string;
    lspath: number;
    tflow: string;
    rtoskuofpu: number;
    fileid: string;
    rowops: string;
    ermsg: string;
    dateops: Date | string | null;
    rowid: number;
 }