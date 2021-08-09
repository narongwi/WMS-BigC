using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS { 

    public class orbit_product {

        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string article    { get; set; }
        public string articletype    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string description    { get; set; }
        public string descalt    { get; set; }
        public string thcode    { get; set; }
        public Int32 dlcall    { get; set; }
        public Int32 dlcfactory    { get; set; }
        public Int32 dlcwarehouse    { get; set; }
        public Int32 dlcshop    { get; set; }
        public Int32 dlconsumer    { get; set; }
        public string hdivison    { get; set; }
        public string hdepartment    { get; set; }
        public string hsubdepart    { get; set; }
        public string hclass    { get; set; }
        public string hsubclass    { get; set; }
        public string typemanage    { get; set; }
        public string unitmanage    { get; set; }
        public string unitdesc    { get; set; }
        public string unitreceipt    { get; set; }
        public string unitprep    { get; set; }
        public string unitsale    { get; set; }
        public string unitstock    { get; set; }
        public string unitweight    { get; set; }
        public string unitdimension    { get; set; }
        public string unitvolume    { get; set; }
        public string hucode    { get; set; }
        public Int32 rtoskuofpu    { get; set; }
        public Int32 rtoskuofipck    { get; set; }
        public Int32 rtoskuofpck    { get; set; }
        public Int32 rtoskuoflayer    { get; set; }
        public Int32 rtoskuofhu    { get; set; }
        public Int32 rtoipckofpck { get; set; }
        public Int32 rtopckoflayer    { get; set; }
        public Int32 rtolayerofhu    { get; set; }
        public Int32 innaturalloss    { get; set; }
        public Int32 ounaturalloss    { get; set; }
        public Decimal costinbound    { get; set; }
        public Decimal costoutbound    { get; set; }
        public Decimal costavg    { get; set; }
        public Decimal skulength    { get; set; }
        public Decimal skuwidth    { get; set; }
        public Decimal skuheight    { get; set; }
        public Decimal skugrossweight    { get; set; }
        public Decimal skuweight    { get; set; }
        public Decimal skuvolume    { get; set; }
        public Decimal pulength    { get; set; }
        public Decimal puwidth    { get; set; }
        public Decimal puheight    { get; set; }
        public Decimal pugrossweight    { get; set; }
        public Decimal puweight    { get; set; }
        public Decimal puvolume    { get; set; }
        public Decimal ipcklength    { get; set; }
        public Decimal ipckwidth    { get; set; }
        public Decimal ipckheight    { get; set; }
        public Decimal ipckgrossweight    { get; set; }
        public Decimal ipckweight    { get; set; }
        public Decimal ipckvolume    { get; set; }
        public Decimal pcklength    { get; set; }
        public Decimal pckwidth    { get; set; }
        public Decimal pckheight    { get; set; }
        public Decimal pckgrossweight    { get; set; }
        public Decimal pckweight    { get; set; }
        public Decimal pckvolume    { get; set; }
        public Decimal layerlength    { get; set; }
        public Decimal layerwidth    { get; set; }
        public Decimal layerheight    { get; set; }
        public Decimal layergrossweight    { get; set; }
        public Decimal layerweight    { get; set; }
        public Decimal layervolume    { get; set; }
        public Decimal hulength    { get; set; }
        public Decimal huwidth    { get; set; }
        public Decimal huheight    { get; set; }
        public Decimal hugrossweight    { get; set; }
        public Decimal huweight    { get; set; }
        public Decimal huvolume    { get; set; }
        public Int32 isdangerous    { get; set; }
        public Int32 ishighvalue    { get; set; }
        public Int32 isfastmove    { get; set; }
        public Int32 isslowmove    { get; set; }
        public Int32 isprescription    { get; set; }
        public Int32 isdlc    { get; set; }
        public Int32 ismaterial    { get; set; }
        public Int32 isunique    { get; set; }
        public Int32 isalcohol    { get; set; }
        public Int32 istemperature    { get; set; }
        public Int32 isdynamicpick    { get; set; }
        public Int32 ismixingprep    { get; set; }
        public Int32 isfinishgoods    { get; set; }
        public Int32 isnaturalloss    { get; set; }
        public Int32 isbatchno    { get; set; }
        public Int32 ismeasurement    { get; set; }
        public string roomtype    { get; set; }
        public Int32 tempmin    { get; set; }
        public Int32 tempmax    { get; set; }
        public string alcmanage    { get; set; }
        public string alccategory    { get; set; }
        public string alccontent    { get; set; }
        public string alccolor    { get; set; }
        public string dangercategory    { get; set; }
        public string dangerlevel    { get; set; }
        public Int32 stockthresholdmin    { get; set; }
        public Int32 stockthresholdmax    { get; set; }
        public string spcrecvzone    { get; set; }
        public string spcrecvaisle    { get; set; }
        public string spcrecvbay    { get; set; }
        public string spcrecvlevel    { get; set; }
        public string spcrecvlocation    { get; set; }
        public string spcprepzone    { get; set; }
        public string spcdistzone    { get; set; }
        public string spcdistshare    { get; set; }
        public string spczonedelv    { get; set; }
        public string orbitsource    { get; set; }
        public string tflow    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public string ermsg    { get; set; }
        
    }
}