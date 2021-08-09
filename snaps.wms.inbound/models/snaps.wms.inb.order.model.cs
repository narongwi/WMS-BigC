using System;
using System.Collections.Generic;
namespace Snaps.WMS { 

    public class inbouln_ls {
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String inorder { get; set; } 
        public String inln { get; set; } 
        public String barcode { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public Int32 qtysku { get; set; } 
        public Decimal qtypu { get; set; } 
        public Decimal qtyweight { get; set; } 
        public String tflow { get; set; } 
        public String description { get; set; } 
        public String unitopsdesc { get; set; } 
    }
    public class inbouln_pm : inbouln_ls { 
        public String inrefno { get; set; } 
        public Int32 inrefln { get; set; } 
        public String inagrn { get; set; } 
        public String unitops { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? expdate { get; set; } 
        public String serialno { get; set; } 
        public Decimal qtypnd { get; set; } 
        public Int32 qtyskurec { get; set; } 
        public Int32 qtypurec { get; set; } 
        public Decimal qtyweightrec { get; set; } 
        public Decimal qtynaturalloss { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 

    }
    public class inbouln_ix : inbouln_ls { 
        public String inrefno { get; set; } 
        public Int32 inrefln { get; set; } 
        public String inagrn { get; set; } 
        public String unitops { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? expdate { get; set; } 
        public String serialno { get; set; } 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class inbouln_md : inbouln_ls  {
        public String inrefno { get; set; } 
        public Int32 inrefln { get; set; } 
        public String inagrn { get; set; } 
        public String unitops { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? expdate { get; set; } 
        public String serialno { get; set; } 
        public Decimal qtypnd { get; set; } 
        public Int32 qtyskurec { get; set; } 
        public Int32 qtypurec { get; set; } 
        public Int32 qtyhurec { get; set; }
        public Decimal qtyweightrec { get; set; } 
        public Decimal qtynaturalloss { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; }

        public Int32 isdlc { get; set; }
        public Int32 isunique { get; set; }
        public Int32 ismixingprep { get; set; }
        public Int32 isbatchno { get; set; }
        public Int32 dlcall { get; set; }
        public Int32 dlcfactory { get; set; }
        public Int32 dlcwarehouse { get; set; }
        public String unitreceipt{ get; set; }
        public Decimal innaturalloss { get; set; }
        public Decimal skulength { get; set; }
        public Decimal skuwidth { get; set; }
        public Decimal skuheight { get; set; }
        public Decimal skuweight { get; set; }
        public String tihi { get; set; }     

        public String laslotno { get; set; }
        public String lasbatchno { get; set; }
        public DateTimeOffset? lasdatemfg { get; set; }
        public DateTimeOffset? lasdateexp { get; set; }
        public String lasserialno { get; set; }
        public Int32 rtoskuofipck { get; set; }
        public Int32 rtoskuofpck { get; set; }
        public Int32 rtoskuoflayer { get; set; }
        public Int32 rtoskuofhu { get; set; }
        public Decimal huestimate { get; set; }

        public List<inboulx> details { get; set; }
        public Int32 inseq { get; set; }


    }

    public class inbound_ls {
        public String orgcode { get; set;} 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String thcode { get; set; } 
        public String subtype { get; set; } 
        public String inorder { get; set; } 
        public DateTimeOffset? dateorder { get; set; } 
        public DateTimeOffset? dateplan { get; set; } 
        public DateTimeOffset? dateexpire { get; set; } 
        public DateTimeOffset? slotdate { get; set; } 
        public String slotno { get; set; } 
        public Int32 inpriority { get; set; } 
        public String inpromo { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? daterec { get; set; } 
        public String thname { get; set; } 
        public String remarkrec { get; set; }
        public Int32 isreqmeasurement { get; set; }
        public String dateremarks {get; set; }
        public Int32 opsprogress { get; set;}
    }


    public class inbound_pm { 
        public String orgcode { get; set;} 
        public String site { get; set; } 
        public String depot { get; set; } 
        public DateTimeOffset? dateplanfrom { get; set; }
        public DateTimeOffset? dateplanto { get; set; }
        public DateTimeOffset? dateorderfrom { get; set; }
        public DateTimeOffset? dateorderto { get; set; }
        public DateTimeOffset? daterecfrom { get; set; }
        public DateTimeOffset? daterecto { get; set; }
        public String ordertype { get; set;}
        public String spcarea { get; set;}
        public String inpriority { get; set; } 
        public String thcode  { get; set; }
        public String inorder { get; set; }
        public String orderno { get; set; }
        public String article { get; set; }
        public String inpromo { get; set; }
        public String inflag  { get; set; }
        public String dockno  { get; set; }
        public String tflow   { get; set;}
        public string ismeasure { get; set ;}

    }

    public class inbound_ix : inbound_ls {
        public String intype { get; set; } 
        public String inflag { get; set; } 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class inbound_md : inbound_ls  {
        public String intype { get; set; } 
        public String inflag { get; set; } 
        public String dockrec { get; set; } 
        public String invno { get; set; } 
        
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String orbitsource { get; set; }

        public DateTimeOffset? dateassign { get; set; }
        public DateTimeOffset? dateunloadstart { get; set; }
        public DateTimeOffset? dateunloadend { get; set; }
        public DateTimeOffset? datefinish { get; set; }
        
        public List<inbouln_md> lines { get; set; }

        public int pendinginf { get; set; }
        public int waitconfirm { get; set; }

    }

    public class inboulx  {
        public Decimal inlx { get; set;} 
        public String orgcode { get; set;}
        public String site { get; set; }
        public String depot { get; set; }
        public String spcarea { get; set; }

        public String inorder { get; set; }
        public String inln { get; set; }
        public String inrefno { get; set; } 
        public String inrefln { get; set; }
        public String barcode { get; set; }
        public String article { get; set; }
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; }
        public String unitops { get; set; }
        public Int32 qtyskurec { get; set; } 
        public Int32 qtypurec { get; set; }
        public Int32 qtyhurec { get; set ;}
        public Decimal qtyweightrec { get; set; }
        public Decimal qtynaturalloss { get; set; }
        public DateTimeOffset? daterec { get; set; } 
        public DateTimeOffset? datemfg   { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public String lotno { get; set;}
        public String batchno { get; set; }
        public String serialno { get; set;}
        public DateTimeOffset? datecreate { get; set; }
        public String accncreate { get; set; }
        public DateTimeOffset? datemodify { get; set; }
        public String accnmodify { get; set; }
        public String procmodify { get; set; }
        public String tflow { get; set; }
        public String ingrno { get; set; }

        public Int32 rtohu { get; set; }
        public Int32 rtopu { get; set; }
        public String dockno { get; set; }
        public String thcode { get; set; }
        public String intype { get; set ;}
        public String insubtype { get; set; }
        public String inpromo { get; set ;}
        public String orbitsource { get; set ;}
        public Decimal qtyvolumerec { get; set;} 

        public string inagrn { get; set ;}
        public Int32 inseq { get; set; }

        public decimal skuweight { get; set; }
        public decimal skuvolume { get; set; }
        public decimal skucubic { get; set; }
        public string unitmanage { get; set; }
    
    }

    public class inbound_hs : inboulx { 
        public Decimal stockid { get; set;} 
        public String  descalt { get; set; }
        public String  thnamealt { get; set; } 
    }






}