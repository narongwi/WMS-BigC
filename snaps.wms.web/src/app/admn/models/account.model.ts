import { lov } from '../../helpers/lov';

export class accn_ls {
    orgcode: string; 
    site:string;
    depot:string;
    accntype: string; 
    accncode: string; 
    accnname: string; 
    email: string; 
    dateexpire: Date | string | null; 
    sessionexpire: Date | string | null;
    tflow: string; 
    accnavartar: string;
    accnsurname:string;
}
export class accn_pm extends accn_ls { 
    accnapline: string; 
}
export interface accn_ix extends accn_ls { 
}
export class accn_md extends accn_ls  {
    mobileno: string; 
    accnapline: string; 
    tkrqpriv: string; 
    cntfailure: number; 
    datelogin: Date | string | null; 
    datelogout: Date | string | null; 
    datechnpriv: Date | string | null; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
    procmodify: string; 
    accsrole : string;
    formatdate:string;
    formatdateshort:string;
    formatdatelong:string;
    accncfg: accn_cfg[];
}
export class accn_cfg {
    orgcode: string;
    site: string;
    depot: string;
    accncode: string;
    rolecode: string;
    rolename: string;
    accncreate: string;
    datecreate: string;
}
export interface accn_signup { 
    accncode: string; 
    accnname: string;
    accnsurname: string; 
    email: string; 
    password: string;
    lang: string;
    accstoken: string;
}
export class accn_prc { 
    accncode: string;
    lang: string;
    unitdimension: string; 
    unitweight: string;
    unitvolumd: string; 
    orgcode: string; 
    sitecode: string; 
    depot: string; 
    pagestart: number; 
    pagelimit: number;
}

export class accn_ppm extends accn_prc { 
    objops: accn_pm;
}
export class accn_pom extends accn_prc { 
    objops: accn_md; 
}
export class accn_profile { 
  orgcode: string; 
  accntype: string; 
  accncode: string; 
  accnname: string; 
  email: string; 
  cfgcode:string;
  cfgvalue:string;
  depot:string;
  site:string;
  dateexpire: Date | string | null; 
  accnavartar: string; 
  accnapline: string; 
  mobileno: string; 
  datelogin: Date | string | null; 
  datelogout: Date | string | null; 
  datechnpriv: Date | string | null; 
  roleaccs: accn_roleacs;
  lang: string;
  formatdateshort: string;
  formatdatelong: string;
  formatdate: string;
}
export class accn_roleacs { 
    site: string;  
    depot: string;
    rolecode: string;
    rolename: string; 
    modules: accn_category[];
}
export class accn_category { 
    modulename: string; 
    moduleicon: string;
    permission: accn_permision[];
}
export class accn_permision { 
    orgcode: string; 
    apcode: string; 
    rolecode: string; 
    objmodule: string; 
    objtype: string; 
    objcode: string; 
    objname: string; 
    objseq: number; 
    objroute: string; 
    objicon: string; 
    isenable: number;
    isexecute: number;
}



export class accn_priv { 
    orgcode: string;
    accncode: string;
    oldpriv: string;
    newpriv: string;
    cnfpriv: string;
    lifetime: number;
    
}

