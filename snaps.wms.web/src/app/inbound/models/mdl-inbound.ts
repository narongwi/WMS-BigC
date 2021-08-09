
    export class inbound_ls {
        orgcode: string; 
        site: string; 
        depot: string; 
        spcarea: string; 
        thcode: string; 
        subtype: string; 
        inorder: string; 
        dateorder: Date | string | null; 
        dateplan: Date | string | null; 
        dateexpire: Date | string | null; 
        slotdate: Date | string | null; 
        slotno: string; 
        inpriority: number; 
        inpromo: string; 
        tflow: string; 
        daterec: Date | string | null; 
        thname: string; 
        isreqmeasurement:number; 
        remarkrec : string;
        dateremarks: string;
        opsprogress:number;
    }

    export class inbound_pm { 
        orgcode : string ;
        site : string;
        depot : string;
        dateplanfrom: Date | string | null;
        dateplanto: Date | string | null;
        dateorderfrom: Date | string | null;
        dateorderto: Date | string | null;
        daterecfrom: Date | string | null;
        daterecto: Date | string | null;
        ordertype: string;
        spcarea: string;
        inpriority: string; 
        thcode: string;
        inorder: string;
        orderno: string;
        article: string;
        inpromo: string;
        inflag: string;
        dockno: string;
        tflow: string; 
        ismeasure : string;
    }

    export class inbound_ix extends inbound_ls {
        intype: string; 
        inflag: string; 
        fileid: string; 
        rowops: string; 
        ermsg: string; 
        dateops: Date | string | null; 
    }
    export class inbound_md extends inbound_ls  {
        intype: string; 
        inflag: string; 
        dockrec: string; 
        invno: string; 
        remarkrec: string; 
        datecreate: Date | string | null; 
        accncreate: string; 
        datemodify: Date | string | null; 
        accnmodify: string; 
        procmodify: string;
        lines:inbouln_md[];
        huestimate : number;
        insource : string;
        dateassign : Date | string | null;
        dateunloadstart : Date | string | null;
        dateunloadend : Date | string | null;
        datefinish : Date | string | null;
        pendinginf:number;
        waitconfirm:number;
    }

    export class inbouln_ls {
        orgcode: string;
        site: string; 
        depot: string; 
        spcarea: string; 
        inorder: string; 
        inln: number; 
        barcode: string; 
        article: string; 
        pv: number; 
        lv: number; 
        qtysku: number; 
        qtypu: number; 
        qtyweight: number; 
        tflow: string; 
        description: string; 
        unitopsdesc: string; 
    }
    export class inbouln_pm extends inbouln_ls { 
        inrefno: string; 
        inrefln: number; 
        inagrn: string; 
        unitops: string; 
        lotno: string; 
        expdate: Date | string | null; 
        serialno: string; 
        qtypnd: number; 
        qtyskurec: number; 
        qtypurec: number; 
        qtyweightrec: number; 
        qtynaturalloss: number; 
        datemodify: Date | string | null; 
        isdlc: number; 
        ismaterial: number; 
        isunique: number; 
        istemperature: number; 
        unitweight: number; 
        unitvolume: number;
    }
    export class inbouln_ix extends inbouln_ls { 
        inrefno: string; 
        inrefln: number; 
        inagrn: string; 
        unitops: string; 
        lotno: string; 
        expdate: Date | string | null; 
        serialno: string; 
        fileid: string; 
        rowops: string; 
        ermsg: string; 
        dateops: Date | string | null; 
    }
    export class inbouln_md extends inbouln_ls  {
        inrefno: string; 
        inrefln: number; 
        inagrn: string; 
        unitops: string; 
        lotno: string; 
        expdate: Date | string | null; 
        serialno: string; 
        qtypnd: number; 
        qtyskurec: number; 
        qtypurec: number; 
        qtyweightrec: number; 
        qtynaturalloss: number; 
        datecreate: Date | string | null; 
        accncreate: string; 
        datemodify: Date | string | null; 
        accnmodify: string; 
        procmodify: string;
        isdlc: number; 
        ismaterial:number; 
        isunique: number;
        ismixingprep: number;
        isbatchno:number;
        dlcall:number;
        dlcfactory: number;
        dlcwarehouse: number;
        unitreceipt: String;
        innaturalloss: number;
        skulength:number;
        skuwidth:number;
        skuheight:number;
        skuweight:number;
        tihi: string        

        lasbatchno: string;
        laslotno: string;
        lasdatemfg: Date | string | null;
        lasdateexp: Date | string | null;
        lasserialno: string;

        rtoskuofpu:number;
        rtoskuofipck:number;
        rtoskuofpck:number;
        rtoskuoflayer:number;
        rtoskuofhu:number;

        huestimate:number;
        details:inboulx[];

        inseq:number;
    }

    
    export class inboulx  {
        lnix: number; 
        orgcode: string;
        site: string;
        depot: string;
        spcarea: string;
        inorder: string;
        inln: number;
        inrefno: string; 
        inrefln: number;
        barcode: string;
        article: string;
        pv: number; 
        lv: number;
        
        unitops: string;
        unitopsdsc: string;

        qtyskurec: number; 
        qtypurec: number;
        qtyweightrec: number;
        qtynaturalloss: number;
        daterec: Date | string | null; 
        datemfg: Date | string | null;
        dateexp: Date | string | null;
        batchno: string | null;
        lotno: string | null;
        serialno: string;
        datecreate: Date | string | null;
        accncreate: string;
        datemodify: Date | string | null;
        accnmodify: string;
        procmodify: string;
        tflow:string;

        qtyhurec:number;
        dlcfactory: number;
        dlcwarehouse: number;

        inagrn:string;
        inseq:number;
    }


    export interface inbound_hs extends inboulx { 
        stockid: number; 
        descalt: string;
        thnamealt: string; 
    }