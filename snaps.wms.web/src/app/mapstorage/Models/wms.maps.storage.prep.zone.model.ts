export class zoneprep_md { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    przone: string;
    przonename: string;
    przonedesc: string;
    tflow: string;
    datecreate: Date | string | null;
    accncreate: string;
    datemodify: Date | string | null;
    accnmodify: string;
    procmodify: string;
    hutype:string;
    description: string;
    huvalweight: number; 
    huvalvolume: number;
    hucapweight: number;
    hucapvolume: number; 
}

export class zoneprln_md { 
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
    lshash: string;
    datecreate: Date | string | null;
    accncreate: string;
    datemodify: Date | string | null;
    accnmodify: string;
    procmodify: string;

    //Additional for display on grid
    barcode: string;

    
}