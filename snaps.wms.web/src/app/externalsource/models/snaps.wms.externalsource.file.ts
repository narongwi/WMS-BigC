export class exsFile { 
    orgcode: string;
    site: string;
    depot: string;
    fileid: number;
    filename: string;
    filetype: string;
    filelength: number;
    imptype: string;
    datestart: Date | string | null;
    dateend: Date | string | null;
    opsperform: string;
    opsins: number;
    opsupd: number;
    opsrem: number;
    opserr: number;
    opsdecstart: Date | string | null;
    opsimpstart: Date | string | null;
    tflow: string;
    datecreate: Date | string | null;
    accncreate: string;
}