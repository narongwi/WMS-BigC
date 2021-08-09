export class admdevice_ls {
    orgcode: string;
    site: string; 
    depot: string; 
    spcarea: string; 
    devtype: string;
    devid: number;
    devcode: string; 
    devmodel: string; 
    datelastactive: Date | string | null; 
    isreceipt: number; 
    istaskptw: number; 
    istasktrf: number; 
    istaskload: number; 
    istaskrpn: number; 
    istaskgen: number; 
    ispick: number; 
    isdistribute: number; 
    isforward: number; 
    iscount: number; 
    tflow: string; 
    datemodify: Date | string | null; 
}
export class admdevice_pm extends admdevice_ls { 

}
export class admdevice_ix extends admdevice_ls { 
}
export class admdevice_md extends admdevice_ls  {
    devserial: string; 
    opsmaxheight: number; 
    opsmaxweight: number; 
    opsmaxvolume: number; 
    devhash: string; 
    datecreate: Date | string | null; 
    devremarks: string;
    accncreate: string; 
    accnmodify: string; 
    procmodify: string;
    devremars: string;
}