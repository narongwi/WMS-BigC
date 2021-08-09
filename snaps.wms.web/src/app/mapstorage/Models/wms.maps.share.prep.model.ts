export class shareprep_md { 
    orgcode: string;
    site: string;
    depot: string;
    spcarea: string;
    shprep: string;
    shprepname: string;
    shprepdesc: string;
    tflow: string;
    datecreate: Date | string | null;
    accncreate: string;
    datemodify: Date | string | null;
    accnmodify: string;
    procmodify: string;
    isfullfill: number;
    lines: shareprln_md[];
}

export class shareprln_md { 
    orgcode: string;
    site: string;
    depot: string;
    shprep: string;
    thcode: string;
    priority: number;
    tflow: string;
    datecreate: Date | string | null;
    accncreate: string;
    datemodify: Date | string | null;        
    accnmodify: string;
    thname: string;
}