export class pam_barcode { allowchangeofexsource: boolean; }

export class pam_correction { 
    allowblankremarks: boolean;
    allowblankrefereceno: boolean;
    allowchangeunit: boolean;
    allowprintlabelonreserve: boolean;
    allowgentaskfornewhu: boolean;
    allowincludehubelongingtask: boolean;
}

export class pam_inbound {        
    allowchangeofexsource: boolean;
    allowcalculatemfg: boolean;
    allowpartail: boolean;
    allowoverplan: boolean;
    allowexpired: boolean; 
    allowchangeunit: boolean;
    allowshowqtyorder: boolean;
    allowautostaging: boolean;
    allowcontrolcapacity: boolean;
    allowchangepriority: boolean;
    allowcancel: boolean;
    allowgendistplan: boolean;
    allowgenstckputaway: boolean;
    allowreplandelivery: boolean;
}

export class pam_outbound { 

    allowchangeofexsource: boolean;
    allowchangespcdlc: boolean;
    allowchangereqdate: boolean;
    allowchangespcbatch: boolean;
    allowcancel: boolean;

}

export class pam_route { allocatehuwhenprepdone:boolean; }
export class pam_delivery { allowrevisequantity:boolean; }

export class pam_prepstock { 
        
    allowincludestaging: boolean;
    allowpartialprocess: boolean;
    allowstocklessthenorder: boolean;
    allowconsolidateorder: boolean;
    allowprocessbyselectline:boolean;

}

export class pam_prepstock_mobile { 
    
    mobiledigitIOloc: boolean;
    mobilecheckdigit: boolean;
    mobilescanbarcode: boolean;
    mobilefullypick: boolean;
    mobilerepickforshortage: boolean;

}

export class pam_prepdist {

    allowincludestaging: boolean;
    allowpartialprocess: boolean;
    allowstocklessthanorder: boolean;
    allowprocessbyselectline: boolean;

}

export class pam_prepdist_mobile { 
    
    mobiledigitIOlc: boolean;
    mobilecheckdigit: boolean;
    mobilescanbarcode: boolean;
    mobilefullypick: boolean;

}

export class pam_preparation { 

    allowcancel: boolean;
    allowautoassign: boolean;

}

export class pam_product { 
    allowchangeofexsource: boolean;
    allowchangehirachy: boolean;
    allowchangedimension: boolean;
    allowchangedlc: boolean;
    allowchangeunit: boolean;
}

export class pam_taskputaway {
    allowscanhuongrap: boolean;
    allowautoassign: boolean;    
    allowscansourcelocation: boolean;
    allowscanbarcode:boolean; 
    allowinputqtyongrap: boolean;
    allowpickndrop: boolean;
    allowcheckdigit: boolean;    
    allowfullygrap: boolean;
    allowfullycollect: boolean;
    allowchangetarget: boolean;  

}

export class pam_taskapproach { 
    allowchangeworker: boolean;
    allowscanhuno: boolean;    
    allowautoassign: boolean;
    allowscansourcelocation: boolean;
    allowscanbarcode: boolean;
    allowchangequantity: boolean;
    allowpickndrop: boolean;
    allowcheckdigit: boolean;
    allowfullycollect: boolean;
    allowchangetarget:boolean;
}

export class pam_taskreplenishment { 
    allowmanual: boolean;
    allowchangeworker: boolean;
    allowscanhuno: boolean;
    allowscanbarcode: boolean;
    allowautoassign: boolean;
    allowscansourcelocation: boolean;
    allowchangequantity: boolean;
    allowpickndrop: boolean;    
    allowcheckdigit: boolean;
    allowfullycollect: boolean;
}

export class pam_tasktransfer  {
    allowmanual: boolean;
    allowchangeworker: boolean;
    allowscanhuno: boolean;
    allowautoassign: boolean;
    allowscansourcelocation: boolean;
    allowscanbarcode: boolean;    
    allowchangequantity: boolean;
    allowpickndrop: boolean;    
    allowcheckdigit: boolean;
    allowfullycollect: boolean;
    allowchangetarget:boolean;
}

export class pam_thirdparty {
    allowchangeofexsource: boolean;
    allowchangeplandate: boolean;
}

export class pam_transfer { 

    allowchangeunit: boolean;
    allowgenreservetoreserve: boolean;
    allowgenreservetopicking: boolean;
    allowgenreservetobulk: boolean;

    allowgenbulktoreserve: boolean;
    allowgenbulktopicking: boolean;
    allowgenbulktobulk: boolean;

    allowgenpickingtoreserve: boolean;
    allowgenpickingtopicking: boolean;
    allowgenpickingtobulk: boolean;

}



export class pam_set { 
    pmmodule: string;
    pmtype: string;
    pmcode: string;
    pmvalue: boolean;
    pmstate: string;
}

export class pam_list extends pam_set { 
    pmdesc: string;
    pmdescalt: string; 
    datemodify: Date | string | null;
    accnmodify: string;
    pmoption:string;
}

export class pam_parameter extends pam_list { 
    orgcode: string;
    site: string;
    depot: string;
    apps: string;
    datecreate: Date | string | null; 
    accncreate: string; 
}

export interface distributionSharesl{
    title?: string;
    subtitle?: string;
    name?: string;
    checked?:Boolean;
    lastchecked?:Boolean;
}
