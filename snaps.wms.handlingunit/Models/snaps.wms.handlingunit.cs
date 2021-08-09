using System;
using System.Collections.Generic;
using System.Text;

namespace Snaps.WMS
{
    public class handerlingunit {
        public string orgcode { get; set; } 
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }         
        public string hutype { get; set; }
        public string huno { get; set; }
        public string loccode { get; set; }
        public string thcode { get; set; }
        public string routeno { get; set; }
        public Decimal mxsku { get; set; } 
        public Decimal mxweight { get; set; }
        public Decimal mxvolume { get; set ;}
        public Int32 crsku { get; set; }
        public Decimal crweight { get; set; } 
        public Decimal crvolume { get; set; }
        public Decimal crcapacity { get; set; }
        public string tflow { get; set; }
        public DateTimeOffset? datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset? datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodfiy { get; set; } 
        public Int32 priority { get; set; }
        public string promo { get; set; }
        public string thname { get; set; }
        public string przone { get; set; } 
    }
    public class handerlingunit_gen : handerlingunit
    {
        public Int32 quantity { get; set; }
        public string opstype { get; set; }
        public string opscode { get; set; }
    }

    public class handerlingunit_item
    {
        public String prepno { get; set;} 
        public String inorder { get; set;} 
        public String ouorder { get; set; }
        public String loccode { get; set; }
        public String article { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set; }
        public Int32 qtysku { get; set; }
        public Decimal qtypu { get; set; }
        public Decimal qtyweight { get; set; }
        public Decimal qtyvolume { get; set; }
        public String descalt { get; set; }
        public String batchno { get; set; }
        public String lotno { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public DateTimeOffset? dateexp { get; set; } 
        public String serialno { get; set; }
        public String unitprep { get; set; }
    }

}
