import { accn_roleacs } from './account.model';

export interface role_ls {
    orgcode: string; 
    apcode: string; 
    site: string;
    depot: string;
    rolecode: string; 
    tflow: string; 
    rolename: string; 
}
export class role_pm { 
    orgcode: string;
    site: string;
    depot: string;
    rolecode: string; 
    tflow: string; 
    rolename: string; 
}
export interface role_ix extends role_ls { 
}
export interface role_md extends role_ls  {
    roledesc: string; 
    hashrol: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    roleaccs: accn_roleacs;
}
export interface role_prc { 
    opsaccn: string;
    opsobj: role_md; 
}

export interface roln_ls {
    orgcode: string; 
    apcode: string; 
    rolcode: string; 
    objmodule: string; 
    objtype: string; 
    objcode: string; 
}
export interface roln_pm extends roln_ls { 

}
export interface roln_ix extends roln_ls { 
}
export interface roln_md extends roln_ls  {
    isenable: number; 
    isexecute: number; 
    hashrln: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
}