export class task_ls {
    orgcode: string;
    site: string; 
    depot: string; 
    spcarea: string; 
    tasktype: string; 
    taskno: string; 
    iopromo: string; 
    iorefno: string; 
    priority: string; 
    taskdate: Date | string | null; 
    tflow: string; 
    taskname: string;
    barcode:string;
    article :string;
    pv:number;
    lv:number;
    sourceloc:string;
    sourcehuno:string;
    targetadv:string;
    descalt:string;
    datestart: Date | string | null;
    dateend: Date | string | null;
}
export class task_pm extends task_ls { 
    taskdatefrom: Date | string | null; 
    taskdateto: Date | string | null; 
}
export class task_ix extends task_ls { 
}
export class task_md {

    orgcode: string;
    site: string; 
    depot: string; 
    spcarea: string; 
    tasktype: string; 
    taskno: string; 
    iopromo: string; 
    iorefno: string; 
    priority: number; 
    taskdate: Date | string | null; 
    tflow: string; 
    description: string; 
    targetloc: string; 

    datestart: Date | string | null; 
    dateend: Date | string | null; 
    datecreate: Date | string | null; 
    accncreate: string; 
    accnmodify: string; 
    procmodify: string; 
    taskname: string; 

    targetqty: number; 
    collectloc: string; 
    collecthuno: string; 
    collectqty: number; 
    accnassign: string; 
    accnwork: string; 
    accnfill: string; 
    accncollect: string; 
    dateassign: Date | string | null; 
    datework: Date | string | null; 
    datefill: Date | string | null; 
    datecollect: Date | string | null; 
    lotno: string; 
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    serialno: string; 

    datemodify: Date | string | null; 
    lines:taln_md[]

    confirmdigit:string;
}

export class taln_ls {
    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    taskno: string; 
    taskseq: number; 
    article: string; 
    sourceloc: number; 
    soucehuno: string; 
    targetadv: string; 
    targethuno: string; 
    iopromo: string; 
    ioreftype: string; 
    iorefno: string; 
    datemodify: Date | string | null; 
    tflow: string; 
    accnassign: string; 
    accnwork: string; 
}
export class taln_pm extends taln_ls { 
}
export class taln_ix extends taln_ls { 
}
export class taln_md extends taln_ls  {

    orgcode: string; 
    site: string; 
    depot: string; 
    spcarea: string; 
    taskno: string; 
    taskseq: number; 
    article: string; 
    soucehuno: string; 
    targetadv: string; 
    targethuno: string; 
    iopromo: string; 
    ioreftype: string; 
    iorefno: string; 
    datemodify: Date | string | null; 
    tflow: string; 

    pv: number; 
    lv: number; 
    targetloc: string; 
    targetqty: number; 
    collectloc: string; 
    collecthuno: string; 
    collectqty: number;
    accnfill: string; 
    accncollect: string; 
    accnassign: string; 
    accnwork: string;
    dateassign: Date | string | null; 
    datework: Date | string | null; 
    datefill: Date | string | null; 
    datecollect: Date | string | null; 
    lotno: string; 
    datemfg: Date | string | null; 
    dateexp: Date | string | null; 
    serialno: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    accnmodify: string; 
    procmodify: string; 
    descalt:string;
    batchno:string;
}

export interface replen_md {
    orgcode?: string;
    site?: string;
    depot?: string;
    spcarea?: string;
    zone?: string;
    aisle?: string;
    level?: string;
    location?: string;
    article?: string;
    pv?: string;
    lv?: string;
    accncode?: string;
}