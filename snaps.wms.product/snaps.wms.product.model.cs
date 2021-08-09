using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.product
{
    public class product_ls
    {
        public String orgcode { get; set; }
        public String site { get; set; }
        public String depot { get; set; }
        public String spcarea { get; set; }
        public String article { get; set; }
        public String articletype { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set; }
        public String descalt { get; set; }
        public String thcode { get; set; }
        public String thname { get; set; }
        public String tflow { get; set; }
    }
    public class product_pm : product_ls
    {
        public String description { get; set; }
        public String hdivison { get; set; }
        public String hdepartment { get; set; }
        public String hsubdepart { get; set; }
        public String hclass { get; set; }
        public String hsubclass { get; set; }
        public String typemanage { get; set; }
        public Int32 ismeasurement { get; set; }
    }
    public class product_ix : product_ls
    {
        public string description { get; set; }
        public Int32 dlcall { get; set; }
        public Int32 dlcfactory { get; set; }
        public Int32 dlcwarehouse { get; set; }
        public Int32 dlcshop { get; set; }
        public Int32 dlconsumer { get; set; }
        public string hdivison { get; set; }
        public string hdepartment { get; set; }
        public string hsubdepart { get; set; }
        public string hclass { get; set; }
        public string hsubclass { get; set; }
        public string typemanage { get; set; }
        public string unitmanage { get; set; }
        public string unitdesc { get; set; }
        public string unitreceipt { get; set; }
        public string unitprep { get; set; }
        public string unitsale { get; set; }
        public string unitstock { get; set; }
        public string unitweight { get; set; }
        public string unitdimension { get; set; }
        public string unitvolume { get; set; }
        public string hucode { get; set; }
        public Int32 rtoskuofpu { get; set; }
        public Int32 rtoskuofipck { get; set; }
        public Int32 rtoskuofpck { get; set; }
        public Int32 rtoskuoflayer { get; set; }
        public Int32 rtoskuofhu { get; set; }
        public Int32 rtopckoflayer { get; set; }
        public Int32 rtolayerofhu { get; set; }
        public Decimal innaturalloss { get; set; }
        public Decimal ounaturalloss { get; set; }
        public Decimal costinbound { get; set; }
        public Decimal costoutbound { get; set; }
        public Decimal costavg { get; set; }
        public Decimal skulength { get; set; }
        public Decimal skuwidth { get; set; }
        public Decimal skuheight { get; set; }
        public Decimal skugrossweight { get; set; }
        public Decimal skuweight { get; set; }
        public Decimal skuvolume { get; set; }
        public Decimal pulength { get; set; }
        public Decimal puwidth { get; set; }
        public Decimal puheight { get; set; }
        public Decimal pugrossweight { get; set; }
        public Decimal puweight { get; set; }
        public Decimal puvolume { get; set; }
        public Decimal ipcklength { get; set; }
        public Decimal ipckwidth { get; set; }
        public Decimal ipckheight { get; set; }
        public Decimal ipckgrossweight { get; set; }
        public Decimal ipckweight { get; set; }
        public Decimal ipckvolume { get; set; }
        public Decimal pcklength { get; set; }
        public Decimal pckwidth { get; set; }
        public Decimal pckheight { get; set; }
        public Decimal pckgrossweight { get; set; }
        public Decimal pckweight { get; set; }
        public Decimal pckvolume { get; set; }
        public Decimal layerlength { get; set; }
        public Decimal layerwidth { get; set; }
        public Decimal layerheight { get; set; }
        public Decimal layergrossweight { get; set; }
        public Decimal layerweight { get; set; }
        public Decimal layervolume { get; set; }
        public Decimal hulength { get; set; }
        public Decimal huwidth { get; set; }
        public Decimal huheight { get; set; }
        public Decimal hugrossweight { get; set; }
        public Decimal huweight { get; set; }
        public Decimal huvolume { get; set; }
        public Int32 isdangerous { get; set; }
        public Int32 ishighvalue { get; set; }
        public Int32 isfastmove { get; set; }
        public Int32 isslowmove { get; set; }
        public Int32 isprescription { get; set; }
        public Int32 isdlc { get; set; }
        public Int32 ismaterial { get; set; }
        public Boolean isunique { get; set; }
        public Int32 isalcohol { get; set; }
        public Int32 istemperature { get; set; }
        public Int32 isdynamicpick { get; set; }
        public Int32 ismixingprep { get; set; }
        public Int32 isfinishgoods { get; set; }
        public Int32 isnaturalloss { get; set; }
        public Int32 isbatchno { get; set; }
        public Int32 ismeasurement { get; set; }
        public string roomtype { get; set; }
        public Decimal tempmin { get; set; }
        public Decimal tempmax { get; set; }
        public string alcmanage { get; set; }
        public string alccategory { get; set; }
        public string alccontent { get; set; }
        public string alccolor { get; set; }
        public string dangercategory { get; set; }
        public string dangerlevel { get; set; }
        public Int32 stockthresholdmin { get; set; }
        public Int32 stockthresholdmax { get; set; }
        public string spcrecvzone { get; set; }
        public string spcrecvaisle { get; set; }
        public string spcrecvbay { get; set; }
        public string spcrecvlevel { get; set; }
        public string spcrecvlocation { get; set; }
        public string spcprepzone { get; set; }
        public string spcdistzone { get; set; }
        public string spcdistshare { get; set; }
        public string spczonedelv { get; set; }
        public string orbitsource { get; set; }
        public string fileid { get; set; }
        public string rowops { get; set; }
        public DateTimeOffset? datecreate { get; set; }
        public DateTimeOffset? dateops { get; set; }
        public string ermsg { get; set; }
    }
    public class product_md : product_ls
    {
        public string description { get; set; }
        public Int32 dlcall { get; set; }
        public Int32 dlcfactory { get; set; }
        public Int32 dlcwarehouse { get; set; }
        public Int32 dlcshop { get; set; }
        public Int32 dlconsumer { get; set; }
        public string hdivison { get; set; }
        public string hdepartment { get; set; }
        public string hsubdepart { get; set; }
        public string hclass { get; set; }
        public string hsubclass { get; set; }
        public string typemanage { get; set; }
        public string unitmanage { get; set; }
        public string unitdesc { get; set; }
        public string unitreceipt { get; set; }
        public string unitprep { get; set; }
        public string unitsale { get; set; }
        public string unitstock { get; set; }
        public string unitweight { get; set; }
        public string unitdimension { get; set; }
        public string unitvolume { get; set; }
        public string hucode { get; set; }
        public Int32 rtoskuofpu { get; set; }
        public Int32 rtoskuofipck { get; set; }
        public Int32 rtoskuofpck { get; set; }
        public Int32 rtoskuoflayer { get; set; }
        public Int32 rtoskuofhu { get; set; }
        public Int32 rtoipckofpck { get; set; }
        public Int32 rtopckoflayer { get; set; }
        public Int32 rtolayerofhu { get; set; }
        public Decimal innaturalloss { get; set; }
        public Decimal ounaturalloss { get; set; }
        public Decimal costinbound { get; set; }
        public Decimal costoutbound { get; set; }
        public Decimal costavg { get; set; }
        public Decimal skulength { get; set; }
        public Decimal skuwidth { get; set; }
        public Decimal skuheight { get; set; }
        public Decimal skugrossweight { get; set; }
        public Decimal skuweight { get; set; }
        public Decimal skuvolume { get; set; }
        public Decimal pulength { get; set; }
        public Decimal puwidth { get; set; }
        public Decimal puheight { get; set; }
        public Decimal pugrossweight { get; set; }
        public Decimal puweight { get; set; }
        public Decimal puvolume { get; set; }
        public Decimal ipcklength { get; set; }
        public Decimal ipckwidth { get; set; }
        public Decimal ipckheight { get; set; }
        public Decimal ipckgrossweight { get; set; }
        public Decimal ipckweight { get; set; }
        public Decimal ipckvolume { get; set; }
        public Decimal pcklength { get; set; }
        public Decimal pckwidth { get; set; }
        public Decimal pckheight { get; set; }
        public Decimal pckgrossweight { get; set; }
        public Decimal pckweight { get; set; }
        public Decimal pckvolume { get; set; }
        public Decimal layerlength { get; set; }
        public Decimal layerwidth { get; set; }
        public Decimal layerheight { get; set; }
        public Decimal layergrossweight { get; set; }
        public Decimal layerweight { get; set; }
        public Decimal layervolume { get; set; }
        public Decimal hulength { get; set; }
        public Decimal huwidth { get; set; }
        public Decimal huheight { get; set; }
        public Decimal hugrossweight { get; set; }
        public Decimal huweight { get; set; }
        public Decimal huvolume { get; set; }
        public Boolean isdangerous { get; set; }
        public Boolean ishighvalue { get; set; }
        public Boolean isfastmove { get; set; }
        public Boolean isslowmove { get; set; }
        public Boolean isprescription { get; set; }
        public Boolean isdlc { get; set; }
        public Boolean ismaterial { get; set; }
        public Boolean isunique { get; set; }
        public Boolean isalcohol { get; set; }
        public Boolean istemperature { get; set; }
        public Boolean isdynamicpick { get; set; }
        public Boolean ismixingprep { get; set; }
        public Boolean isfinishgoods { get; set; }
        public Boolean isnaturalloss { get; set; }
        public Boolean isbatchno { get; set; }
        public Boolean ismeasurement { get; set; }
        public string roomtype { get; set; }
        public Decimal tempmin { get; set; }
        public Decimal tempmax { get; set; }
        public string alcmanage { get; set; }
        public string alccategory { get; set; }
        public string alccontent { get; set; }
        public string alccolor { get; set; }
        public string dangercategory { get; set; }
        public string dangerlevel { get; set; }
        public Int32 stockthresholdmin { get; set; }
        public Int32 stockthresholdmax { get; set; }
        public string spcrecvzone { get; set; }
        public string spcrecvaisle { get; set; }
        public string spcrecvaisleto { get; set; }
        public string spcrecvbay { get; set; }
        public string spcrecvbayto { get; set; }
        public string spcrecvlevel { get; set; }

        public string spcrecvlevelto { get; set; }
        public string spcrecvlocation { get; set; }
        public string spcprepzone { get; set; }
        public string spcdistzone { get; set; }
        public string spcdistshare { get; set; }
        public string spczonedelv { get; set; }
        public string orbitsource { get; set; }
        public DateTimeOffset? datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset? datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set; }
        public DateTimeOffset? lasrecv { get; set; }
        public DateTimeOffset? lasdelivery { get; set; }
        public string lasbatchno { get; set; }
        public string laslotno { get; set; }
        public DateTimeOffset? lasdatemfg { get; set; }
        public DateTimeOffset? lasdateexp { get; set; }
        public string lasserialno { get; set; }

        public string hdivisionname { get; set; }
        public string hdepartmentname { get; set; }
        public string hsubdepartname { get; set; }
        public string hclassname { get; set; }
        public string hsubclassname { get; set; }

    }

    public class product_strict : product_ls
    {
        public Int32 isdlc { get; set; }
        public Int32 isunique { get; set; }
        public Int32 isdynamicpick { get; set; }
        public Int32 isnaturalloss { get; set; }
        public Int32 isbatchno { get; set; }
        public Int32 ismixingprep { get; set; }
        public Int32 ismeasuremnet { get; set; }

        //Specific for zone inbound
        public String spcrecvzone { get; set; }
        public String spcrecvaisle { get; set; }
        public String spcrecvbay { get; set; }
        public String spcrecvlevel { get; set; }
        public String spcrecvlocation { get; set; }

        //Specific for zone outbound
        public String spczoneprep { get; set; }
        public String spczonedist { get; set; }
        public String spcdistshare { get; set; }

        //Unit for operate
        public String unitmanage { get; set; }
        public String unitreceipt { get; set; }
        public String unitprep { get; set; }
        public String unitsale { get; set; }
        public String unitstock { get; set; }
        public String unitweight { get; set; }
        public String unitdimension { get; set; }
        public String unitvolume { get; set; }

        //Product Ratio 
        public Int32 rtoskuofpu { get; set; }
        public Int32 rtoskuofipck { get; set; }
        public Int32 rtoskuofpck { get; set; }
        public Int32 rtoskuoflayer { get; set; }
        public Int32 rtoskuofhu { get; set; }

        //DLC controle
        public Decimal dlcall { get; set; }
        public Decimal dlcfactory { get; set; }
        public Decimal dlcwarehouse { get; set; }
        public Decimal dlcshop { get; set; }
        public Decimal dlconsumer { get; set; }

        //Product dimension for sku
        public Decimal skulength { get; set; }
        public Decimal skuwidth { get; set; }
        public Decimal skuheight { get; set; }
        public Decimal skuvolume { get; set; }

        //Product dimension for PU
        public Decimal pulength { get; set; }
        public Decimal puwidth { get; set; }
        public Decimal puheight { get; set; }
        public Decimal puvolume { get; set; }


        //Handerling unit 
        public Decimal hucode { get; set; }

        //Natural loss
        public Decimal innatualloss { get; set; }
        public Decimal ounatualloss { get; set; }
    }

    public class product_active
    {
        public String orgcode { get; set; }
        public String site { get; set; }
        public String depot { get; set; }
        public String spcarea { get; set; }
        public string barcode { get; set; }
        public string article { get; set; }
        public int pv { get; set; }
        public int lv { get; set; }
        public string descalt { get; set; }
        public double skulength { get; set; }
        public double skuwidth { get; set; }
        public double skuheight { get; set; }
        public double skugrossweight { get; set; }
        public double skuweight { get; set; }
        public double skuvolume { get; set; }
        public int rtoskuofpu { get; set; }
        public int rtopckoflayer { get; set; }
        public int rtolayerofhu { get; set; }
        public int rtopckofpallet { get; set; }
        public int rtoskuofipck { get; set; }
        public int rtoskuofpck { get; set; }
        public int rtoskuoflayer { get; set; }
        public int rtoskuofhu { get; set; }
        public string unitprep { get; set; }
        public string unitreceipt { get; set; }
        public string unitmanage { get; set; }
        public string unitdesc { get; set; }
        public string articletype { get; set; }

    }
}