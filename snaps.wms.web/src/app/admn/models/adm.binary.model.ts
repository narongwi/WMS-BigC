export class binary_ls {
    orgcode: string; 
    site: string; 
    depot: string; 
    bntype: string; 
    bnvalue: string; 
    bncode: string; 
    bndesc: string; 
    apps: string;
    bnicon: string;
    bndescalt: string; 
    bnflex1: string; 
    bnflex2: string; 
    bnflex3: string; 
    bnflex4: string;

}
export class binary_pm extends binary_ls { 
}
export class binary_ix extends binary_ls { 
}
export class binary_md extends binary_ls {
   
    bnstate: string; 
    datecreate: Date | string | null; 
    accncreate: string; 
    datemodify: Date | string | null; 
    accnmodify: string; 
}
export class binary_prc { 
    opsaccn: string;
    opsobj: binary_md; 
}