using System;
namespace Snaps.WMS.warehouse { 
    public class locup_ls {
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String fltype { get; set; } 
        public String fltypedsc { get; set; } 
        public String lslevel { get; set; } 
        public String lscode { get; set; } 
        public String lscodealt { get; set; } 

        public String lszone { get; set; } 
        public String lsaisle { get; set; } 
        public String lsbay { get; set; } 
        public Decimal crfreepct { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String lscodeid { get; set; } 
    }
    public class locup_pm : locup_ls { 
        public String tflowcnt { get; set; } 
    }
    public class locup_ix : locup_ls { 
    }
    public class locup_md : locup_ls  {
        public String lsformat { get; set; } 
        public Int32 lsseq { get; set; } 
        public String lscodefull { get; set; } 

        public Decimal crweight { get; set; } 
        public Decimal crvolume { get; set; } 
        public Decimal crlocation { get; set; } 
        public String tflowcnt { get; set; } 
        public String lshash { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String lsdesc { get; set;} 
    }
    public class locup_prc { 
        public String opsaccn { get; set; }
        public locup_md opsobj { get; set; } 
    }
    
    public class locdw_ls {        
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String fltype { get; set; } 
        public String lszone { get; set; } 
        public String lsaisle { get; set; } 
        public String lsbay { get; set; } 
        public String lslevel { get; set; }
        public String lsloc { get; set; }
        public String lsstack { get; set; } 
        public String lscode { get; set; }
        public String lscodealt { get; set;} 
        public Decimal crweight { get; set; } 
        public Decimal crvolume { get; set; } 
        public Decimal crfreepct { get; set; } 
        public String tflow { get; set; } 
        
        public Int32 spcpicking { get; set;}
    }
    public class locdw_pm : locdw_ls { 
        public String spchuno { get; set; } 
        public String spcarticle { get; set; } 
        public Int32 spcblock { get; set; } 
        public String spctaskfnd { get; set; } 
        public Int32 spcseqpath { get; set; } 
        public String spclasttouch { get; set; } 
        public String spcpivot { get; set; } 
        public String spcpickunit { get; set; }
        public String lsloctype  { get; set;} 
        public String spcthcode { get; set;} 

        public string zone { get; set; }
        public string aislefrom { get; set; }
        public string aisleto { get; set; }
        public string bayfrom { get; set; }
        public string bayto { get; set; }
        public string levelfrom { get; set; }
        public string levelto { get; set; }
        public string locationfrom { get; set; }
        public string locationto { get; set; }
        public string mixproduct { get; set; }
        public string mixaging { get; set; }
        public string mixlot { get; set; }
        public string ispicking { get; set; }
        public string isreserve { get; set; }
        public string spcproduct { get; set; }
        public string lasttourch { get; set; }


    }
    public class locdw_ix : locdw_ls { 
    }
    public class locdw_md : locdw_ls  {
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string fltype    { get; set; }
        public string lszone    { get; set; }
        public string lsaisle    { get; set; }
        public string lsbay    { get; set; }
        public string lslevel    { get; set; }
        public string lsloc    { get; set; }
        public string lsstack    { get; set; }
        public string lscode    { get; set; }
        public string lscodealt    { get; set; }
        public string lscodefull    { get; set; }
        public string lscodeid    { get; set; }
        public Decimal lsdmlength    { get; set; }
        public Decimal lsdmwidth    { get; set; }
        public Decimal lsdmheight    { get; set; }
        public Decimal lsmxweight    { get; set; }
        public Decimal lsmxvolume    { get; set; }
        public Decimal lsmxlength    { get; set; }
        public Decimal lsmxwidth    { get; set; }
        public Decimal lsmxheight    { get; set; }
        public Int32 lsmxhuno    { get; set; }
        public Int32 lsmnsafety    { get; set; }
        public Int32 lsmixarticle    { get; set; }
        public Int32 lsmixage    { get; set; }
        public Int32 lsmixlotno    { get; set; }
        public string lsloctype    { get; set; }
        public string lsremarks    { get; set; }
        public Decimal lsgaptop    { get; set; }
        public Decimal lsgapleft    { get; set; }
        public Decimal lsgapright    { get; set; }
        public Decimal lsgapbuttom    { get; set; }
        public Int32 lsstackable    { get; set; }
        public string lsdigit    { get; set; }
        public string spcthcode    { get; set; }
        public string spchuno    { get; set; }
        public string spcarticle    { get; set; }
        public Int32 spcblock    { get; set; }
        public string spctaskfnd    { get; set; }
        public Int32 spcseqpath    { get; set; }
        public string spclasttouch    { get; set; }
        public string spcpivot    { get; set; }
        public Int32 spcpicking    { get; set; }
        public string spcpickunit    { get; set; }
        public string spcrpn    { get; set; }
        public Int32 spcmnaging    { get; set; }
        public Int32 spcmxaging    { get; set; }
        public Decimal crweight    { get; set; }
        public Decimal crvolume    { get; set; }
        public Decimal crfreepct    { get; set; }
        public string tflow    { get; set; }
        public string tflowcnt    { get; set; }
        public string lshash    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
        public Int32 crhu    { get; set; }
        public string lsdirection    { get; set; }
        public Int32 spcpathrecv    { get; set; }
        public Int32 spcpathpick    { get; set; }
        public Int32 spcpathdist    { get; set; }

        
        public Int32 lsstacklimit { get; set; }
        public string lsdesc { get; set;}
    }
    public class locdw_prc { 
        public String opsaccn { get; set; }
        public locdw_md opsobj { get; set; } 
    }

    public class locdw_gn { 
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string zone { get; set; }
        public string aislefr { get; set; }
        public string aisleto { get; set; }
        public string bayfr { get; set; }
        public string bayto { get; set; }
        public string levelfr { get; set; }
        public string levelto { get; set; }
        public Int32 location { get; set; } 
        public string tflow { get; set; }
        public Decimal lsdmlength { get; set; }
        public Decimal lsdmwidth { get; set; } 
        public Decimal lsdmheight { get; set; } 
        public Decimal lsmxweight { get; set; } 
        public Decimal lsmxvolume { get; set; } 
        public Decimal lsmxlength { get; set; } 
        public Decimal lsmxwidth { get; set; } 
        public Decimal lsmxheight { get; set; } 
        public Int32 lsmxhuno { get; set; } 
        public Int32 lsmnsafety { get; set; } 
        public Int32 lsmixage { get; set; }
        public Int32 lsmixarticle { get; set; } 
        public Int32 lsmixlotno { get; set; } 
        public string lsloctype { get; set; } 
        public string lsremarks { get; set; }
        public Decimal lsgaptop { get; set; }  
        public Decimal lsgapleft { get; set; } 
        public Decimal lsgapright { get; set; } 
        public Decimal lsgapbuttom { get; set; } 
        public string lsdigit { get; set; }
        public string spcthcode { get; set; }
        public string spchuno { get; set; }
        public string spcarticle { get; set; }
        public Int32 spcblock { get; set; } 
        public string spctaskfnd { get; set; }
        public Int32 spcseqpath { get; set; } 
        public string spclasttouch { get; set; }
        public string spcpivot { get; set; }
        public string spcpickunit { get; set; }
        public string spcrpn { get; set; }
        public Decimal spcmnaging { get; set; } 
        public Decimal spcmxaging { get; set; }
        public string accncode { get; set;}
        public string lsformat { get; set;}
        public Int32 lsstackable { get; set; }
        public Int32 lsstacklimit { get; set; }
        public Int32 spcpicking { get; set; }
        public string lsstacklabel { get; set; }
    }

    public class locdw_gngrid { 
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string zone { get; set; }
        public Int32 lsloc { get; set; }
        public Int32 lsseq { get; set; } 
        public string tflow { get; set; }
        public string lsfornat { get; set; } 
        public string lslength { get; set;}
        public string lswidth { get; set; }
        public Int32 lsmxhuno { get; set; }
        public Decimal lsmxweight { get; set; } 
        public Decimal lsmxvolume { get; set; } 
        public String spcthcode { get; set;}
        public Int32 lsmixarticle { get; set; }
        public String accncode { get; set; }
        public Int32 location { get; set; }
    }
    public class locdw_pivot : locdw_ls { 
        public String spcpivot { get; set;} 
    }
    public class locdw_picking : locdw_ls { 
        public String spcpickunit { get; set; } 
        public Int32 spcseqpath { get; set; }
        public String spcrpn { get; set; }
        public String spcarticle { get; set;}
        public Int32 lsmnsafety { get; set;}
        
    }
}