using System;
using System.Linq;
using System.Data;
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

    public partial class product_ops : IDisposable
    {
        private product_active fillat(ref SqlDataReader r)
        {
            return new product_active()
            {
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                barcode = r["barcode"].ToString(),
                article = r["article"].ToString(),
                articletype = r["articletype"].ToString(),
                pv = r["pv"].ToString().CInt32(),
                lv = r["lv"].ToString().CInt32(),
                descalt = r["descalt"].ToString(),
                skulength = r["skulength"].ToString().CDouble(),
                skuwidth = r["skuwidth"].ToString().CDouble(),
                skuheight = r["skuheight"].ToString().CDouble(),
                skugrossweight = r["skugrossweight"].ToString().CDouble(),
                skuweight = r["skuweight"].ToString().CDouble(),
                skuvolume = r["skuvolume"].ToString().CDouble(),

                rtoskuofpu = r["rtoskuofpu"].ToString().CInt32(),
                rtopckoflayer = r["rtopckoflayer"].ToString().CInt32(),
                rtolayerofhu = r["rtolayerofhu"].ToString().CInt32(),
                rtopckofpallet = r["rtopckofpallet"].ToString().CInt32(),
                rtoskuofipck = r["rtoskuofipck"].ToString().CInt32(),
                rtoskuofpck = r["rtoskuofpck"].ToString().CInt32(),
                rtoskuoflayer = r["rtoskuoflayer"].ToString().CInt32(),
                rtoskuofhu = r["rtoskuofhu"].ToString().CInt32(),

                unitprep = r["unitprep"].ToString(),
                unitreceipt = r["unitreceipt"].ToString(),
                unitmanage = r["unitmanage"].ToString(),
                unitdesc = r["unitdesc"].ToString()
            };
        }
        private product_ls fillls(ref SqlDataReader r)
        {
            return new product_ls()
            {
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                article = r["article"].ToString(),
                articletype = r["articletype"].ToString(),
                pv = r.GetInt32(5),
                lv = r.GetInt32(6),
                descalt = r["descalt"].ToString(),
                thcode = r["thcode"].ToString(),
                tflow = r["tflow"].ToString(),
                thname = r["thname"].ToString()
            };
        }
        private product_ix fillix(ref SqlDataReader r)
        {
            product_ix rn = new product_ix();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.article = r["article"].ToString();
            rn.articletype = r["articletype"].ToString();
            rn.pv = r.GetInt32(7);
            rn.lv = r.GetInt32(8);
            rn.description = r["description"].ToString();
            rn.descalt = r["descalt"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.dlcall = r.GetInt32(12);
            rn.dlcfactory = r.GetInt32(13);
            rn.dlcwarehouse = r.GetInt32(14);
            rn.dlcshop = r.GetInt32(15);
            rn.dlconsumer = r.GetInt32(16);
            rn.hdivison = r["hdivison"].ToString();
            rn.hdepartment = r["hdepartment"].ToString();
            rn.hsubdepart = r["hsubdepart"].ToString();
            rn.hclass = r["hclass"].ToString();
            rn.hsubclass = r["hsubclass"].ToString();
            rn.typemanage = r["typemanage"].ToString();
            rn.unitmanage = r["unitmanage"].ToString();
            rn.unitdesc = r["unitdesc"].ToString();
            rn.unitreceipt = r["unitreceipt"].ToString();
            rn.unitprep = r["unitprep"].ToString();
            rn.unitsale = r["unitsale"].ToString();
            rn.unitstock = r["unitstock"].ToString();
            rn.unitweight = r["unitweight"].ToString();
            rn.unitdimension = r["unitdimension"].ToString();
            rn.unitvolume = r["unitvolume"].ToString();
            rn.hucode = r["hucode"].ToString();
            rn.rtoskuofpu = r.GetInt32(33);
            rn.rtoskuofipck = r.GetInt32(34);
            rn.rtoskuofpck = r.GetInt32(35);
            rn.rtoskuoflayer = r.GetInt32(36);
            rn.rtoskuofhu = r.GetInt32(37);
            rn.rtopckoflayer = r.GetInt32(38);
            rn.rtolayerofhu = r.GetInt32(39);
            rn.innaturalloss = r.GetInt32(40);
            rn.ounaturalloss = r.GetInt32(41);
            rn.costinbound = r.GetDecimal(42);
            rn.costoutbound = r.GetDecimal(43);
            rn.costavg = r.GetDecimal(44);
            rn.skulength = r.GetDecimal(45);
            rn.skuwidth = r.GetDecimal(46);
            rn.skuheight = r.GetDecimal(47);
            rn.skugrossweight = r.GetDecimal(48);
            rn.skuweight = r.GetDecimal(49);
            rn.skuvolume = r.GetDecimal(50);
            rn.pulength = r.GetDecimal(51);
            rn.puwidth = r.GetDecimal(52);
            rn.puheight = r.GetDecimal(53);
            rn.pugrossweight = r.GetDecimal(54);
            rn.puweight = r.GetDecimal(55);
            rn.puvolume = r.GetDecimal(56);
            rn.ipcklength = r.GetDecimal(57);
            rn.ipckwidth = r.GetDecimal(58);
            rn.ipckheight = r.GetDecimal(59);
            rn.ipckgrossweight = r.GetDecimal(60);
            rn.ipckweight = r.GetDecimal(61);
            rn.ipckvolume = r.GetDecimal(62);
            rn.pcklength = r.GetDecimal(63);
            rn.pckwidth = r.GetDecimal(64);
            rn.pckheight = r.GetDecimal(65);
            rn.pckgrossweight = r.GetDecimal(66);
            rn.pckweight = r.GetDecimal(67);
            rn.pckvolume = r.GetDecimal(68);
            rn.layerlength = r.GetDecimal(69);
            rn.layerwidth = r.GetDecimal(70);
            rn.layerheight = r.GetDecimal(71);
            rn.layergrossweight = r.GetDecimal(72);
            rn.layerweight = r.GetDecimal(73);
            rn.layervolume = r.GetDecimal(74);
            rn.hulength = r.GetDecimal(75);
            rn.huwidth = r.GetDecimal(76);
            rn.huheight = r.GetDecimal(77);
            rn.hugrossweight = r.GetDecimal(78);
            rn.huweight = r.GetDecimal(79);
            rn.huvolume = r.GetDecimal(80);
            rn.isdangerous = r.GetInt32(81);
            rn.ishighvalue = r.GetInt32(82);
            rn.isfastmove = r.GetInt32(83);
            rn.isslowmove = r.GetInt32(84);
            rn.isprescription = r.GetInt32(85);
            rn.isdlc = r.GetInt32(86);
            rn.ismaterial = r.GetInt32(87);
            rn.isunique = r.GetBoolean(88);
            rn.isalcohol = r.GetInt32(89);
            rn.istemperature = r.GetInt32(90);
            rn.isdynamicpick = r.GetInt32(91);
            rn.ismixingprep = r.GetInt32(92);
            rn.isfinishgoods = r.GetInt32(93);
            rn.isnaturalloss = r.GetInt32(94);
            rn.isbatchno = r.GetInt32(95);
            rn.ismeasurement = r.GetInt32(96);
            rn.roomtype = r["roomtype"].ToString();
            rn.tempmin = r.GetInt32(98);
            rn.tempmax = r.GetInt32(99);
            rn.alcmanage = r["alcmanage"].ToString();
            rn.alccategory = r["alccategory"].ToString();
            rn.alccontent = r["alccontent"].ToString();
            rn.alccolor = r["alccolor"].ToString();
            rn.dangercategory = r["dangercategory"].ToString();
            rn.dangerlevel = r["dangerlevel"].ToString();
            rn.stockthresholdmin = r.GetInt32(106);
            rn.stockthresholdmax = r.GetInt32(107);
            rn.spcrecvzone = r["spcrecvzone"].ToString();
            rn.spcrecvaisle = r["spcrecvaisle"].ToString();
            rn.spcrecvbay = r["spcrecvbay"].ToString();
            rn.spcrecvlevel = r["spcrecvlevel"].ToString();
            rn.spcrecvlocation = r["spcrecvlocation"].ToString();
            rn.spcprepzone = r["spcprepzone"].ToString();
            rn.spcdistzone = r["spcdistzone"].ToString();
            rn.spcdistshare = r["spcdistshare"].ToString();
            rn.spczonedelv = r["spczonedelv"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.datecreate = (r.IsDBNull(121)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(121);
            rn.dateops = (r.IsDBNull(122)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(122);
            rn.ermsg = r["ermsg"].ToString();

            return rn;
        }
        private product_md fillmdl(ref SqlDataReader r)
        {
            product_md rn = new product_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.article = r["article"].ToString();
            rn.articletype = r["articletype"].ToString();
            rn.pv = (r.IsDBNull(6)) ? 0 : r.GetInt32(6);
            rn.lv = (r.IsDBNull(7)) ? 0 : r.GetInt32(7);
            rn.description = r["description"].ToString();
            rn.descalt = r["descalt"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.dlcall = (r.IsDBNull(11)) ? 0 : r.GetInt32(11);
            rn.dlcfactory = (r.IsDBNull(12)) ? 0 : r.GetInt32(12);
            rn.dlcwarehouse = (r.IsDBNull(13)) ? 0 : r.GetInt32(13);
            rn.dlcshop = (r.IsDBNull(14)) ? 0 : r.GetInt32(14);
            rn.dlconsumer = (r.IsDBNull(15)) ? 0 : r.GetInt32(15);
            rn.hdivison = r["hdivison"].ToString();
            rn.hdepartment = r["hdepartment"].ToString();
            rn.hsubdepart = r["hsubdepart"].ToString();
            rn.hclass = r["hclass"].ToString();
            rn.hsubclass = r["hsubclass"].ToString();
            rn.typemanage = r["typemanage"].ToString();
            rn.unitmanage = r["unitmanage"].ToString();
            rn.unitdesc = r["unitdesc"].ToString();
            rn.unitreceipt = r["unitreceipt"].ToString();
            rn.unitprep = r["unitprep"].ToString();
            rn.unitsale = r["unitsale"].ToString();
            rn.unitstock = r["unitstock"].ToString();
            rn.unitweight = r["unitweight"].ToString();
            rn.unitdimension = r["unitdimension"].ToString();
            rn.unitvolume = r["unitvolume"].ToString();
            rn.hucode = r["hucode"].ToString();
            rn.rtoskuofpu = (r.IsDBNull(32)) ? 0 : r.GetInt32(32);
            rn.rtoskuofipck = (r.IsDBNull(33)) ? 0 : r.GetInt32(33);
            rn.rtoskuofpck = (r.IsDBNull(34)) ? 0 : r.GetInt32(34);
            rn.rtoskuoflayer = (r.IsDBNull(35)) ? 0 : r.GetInt32(35);
            rn.rtoskuofhu = (r.IsDBNull(36)) ? 0 : r.GetInt32(36);
            rn.rtoipckofpck = (r.IsDBNull(37)) ? 0 : r.GetInt32(37);
            rn.rtopckoflayer = (r.IsDBNull(38)) ? 0 : r.GetInt32(38);
            rn.rtolayerofhu = (r.IsDBNull(39)) ? 0 : r.GetInt32(39);
            rn.innaturalloss = (r.IsDBNull(40)) ? 0 : r.GetDecimal(40);
            rn.ounaturalloss = (r.IsDBNull(41)) ? 0 : r.GetDecimal(41);
            rn.costinbound = (r.IsDBNull(42)) ? 0 : r.GetDecimal(42);
            rn.costoutbound = (r.IsDBNull(43)) ? 0 : r.GetDecimal(43);
            rn.costavg = (r.IsDBNull(44)) ? 0 : r.GetDecimal(44);
            rn.skulength = (r.IsDBNull(45)) ? 0 : r.GetDecimal(45);
            rn.skuwidth = (r.IsDBNull(46)) ? 0 : r.GetDecimal(46);
            rn.skuheight = (r.IsDBNull(47)) ? 0 : r.GetDecimal(47);
            rn.skugrossweight = (r.IsDBNull(48)) ? 0 : r.GetDecimal(48);
            rn.skuweight = (r.IsDBNull(49)) ? 0 : r.GetDecimal(49);
            rn.skuvolume = (r.IsDBNull(50)) ? 0 : r.GetDecimal(50);
            rn.pulength = (r.IsDBNull(51)) ? 0 : r.GetDecimal(51);
            rn.puwidth = (r.IsDBNull(52)) ? 0 : r.GetDecimal(52);
            rn.puheight = (r.IsDBNull(53)) ? 0 : r.GetDecimal(53);
            rn.pugrossweight = (r.IsDBNull(54)) ? 0 : r.GetDecimal(54);
            rn.puweight = (r.IsDBNull(55)) ? 0 : r.GetDecimal(55);
            rn.puvolume = (r.IsDBNull(56)) ? 0 : r.GetDecimal(56);
            rn.ipcklength = (r.IsDBNull(57)) ? 0 : r.GetDecimal(57);
            rn.ipckwidth = (r.IsDBNull(58)) ? 0 : r.GetDecimal(58);
            rn.ipckheight = (r.IsDBNull(59)) ? 0 : r.GetDecimal(59);
            rn.ipckgrossweight = (r.IsDBNull(60)) ? 0 : r.GetDecimal(60);
            rn.ipckweight = (r.IsDBNull(61)) ? 0 : r.GetDecimal(61);
            rn.ipckvolume = (r.IsDBNull(62)) ? 0 : r.GetDecimal(62);
            rn.pcklength = (r.IsDBNull(63)) ? 0 : r.GetDecimal(63);
            rn.pckwidth = (r.IsDBNull(64)) ? 0 : r.GetDecimal(64);
            rn.pckheight = (r.IsDBNull(65)) ? 0 : r.GetDecimal(65);
            rn.pckgrossweight = (r.IsDBNull(66)) ? 0 : r.GetDecimal(66);
            rn.pckweight = (r.IsDBNull(67)) ? 0 : r.GetDecimal(67);
            rn.pckvolume = (r.IsDBNull(68)) ? 0 : r.GetDecimal(68);
            rn.layerlength = (r.IsDBNull(69)) ? 0 : r.GetDecimal(69);
            rn.layerwidth = (r.IsDBNull(70)) ? 0 : r.GetDecimal(70);
            rn.layerheight = (r.IsDBNull(71)) ? 0 : r.GetDecimal(71);
            rn.layergrossweight = (r.IsDBNull(72)) ? 0 : r.GetDecimal(72);
            rn.layerweight = (r.IsDBNull(73)) ? 0 : r.GetDecimal(73);
            rn.layervolume = (r.IsDBNull(74)) ? 0 : r.GetDecimal(74);
            rn.hulength = (r.IsDBNull(75)) ? 0 : r.GetDecimal(75);
            rn.huwidth = (r.IsDBNull(76)) ? 0 : r.GetDecimal(76);
            rn.huheight = (r.IsDBNull(77)) ? 0 : r.GetDecimal(77);
            rn.hugrossweight = (r.IsDBNull(78)) ? 0 : r.GetDecimal(78);
            rn.huweight = (r.IsDBNull(79)) ? 0 : r.GetDecimal(79);
            rn.huvolume = (r.IsDBNull(80)) ? 0 : r.GetDecimal(80);
            rn.isdangerous = (r.IsDBNull(81)) ? false : (r.GetInt32(81) == 1) ? true : false;
            rn.ishighvalue = (r.IsDBNull(82)) ? false : (r.GetInt32(82) == 1) ? true : false;
            rn.isfastmove = (r.IsDBNull(83)) ? false : (r.GetInt32(83) == 1) ? true : false;
            rn.isslowmove = (r.IsDBNull(84)) ? false : (r.GetInt32(84) == 1) ? true : false;
            rn.isprescription = (r.IsDBNull(85)) ? false : (r.GetInt32(85) == 1) ? true : false;
            rn.isdlc = (r.IsDBNull(86)) ? false : (r.GetInt32(86) == 1) ? true : false;
            rn.ismaterial = (r.IsDBNull(87)) ? false : (r.GetInt32(87) == 1) ? true : false;
            rn.isunique = (r.IsDBNull(88)) ? false : (r.GetInt32(88) == 1) ? true : false;
            rn.isalcohol = (r.IsDBNull(89)) ? false : (r.GetInt32(89) == 1) ? true : false;
            rn.istemperature = (r.IsDBNull(90)) ? false : (r.GetInt32(90) == 1) ? true : false;
            rn.isdynamicpick = (r.IsDBNull(91)) ? false : (r.GetInt32(91) == 1) ? true : false;
            rn.ismixingprep = (r.IsDBNull(92)) ? false : (r.GetInt32(92) == 1) ? true : false;
            rn.isfinishgoods = (r.IsDBNull(93)) ? false : (r.GetInt32(93) == 1) ? true : false;
            rn.isnaturalloss = (r.IsDBNull(94)) ? false : (r.GetInt32(94) == 1) ? true : false;
            rn.isbatchno = (r.IsDBNull(95)) ? false : (r.GetInt32(95) == 1) ? true : false;
            rn.ismeasurement = (r.IsDBNull(96)) ? false : (r.GetInt32(96) == 1) ? true : false;
            rn.roomtype = r["roomtype"].ToString();
            rn.tempmin = (r.IsDBNull(98)) ? 0 : r.GetDecimal(98);
            rn.tempmax = (r.IsDBNull(99)) ? 0 : r.GetDecimal(99);
            rn.alcmanage = r["alcmanage"].ToString();
            rn.alccategory = r["alccategory"].ToString();
            rn.alccontent = r["alccontent"].ToString();
            rn.alccolor = r["alccolor"].ToString();
            rn.dangercategory = r["dangercategory"].ToString();
            rn.dangerlevel = r["dangerlevel"].ToString();
            rn.stockthresholdmin = (r.IsDBNull(106)) ? 0 : r.GetInt32(106);
            rn.stockthresholdmax = (r.IsDBNull(107)) ? 0 : r.GetInt32(107);
            rn.spcrecvzone = r["spcrecvzone"].ToString();
            rn.spcrecvaisle = r["spcrecvaisle"].ToString();
            rn.spcrecvaisleto = r["spcrecvaisleto"].ToString();
            rn.spcrecvbay = r["spcrecvbay"].ToString();
            rn.spcrecvbayto = r["spcrecvbayto"].ToString();
            rn.spcrecvlevel = r["spcrecvlevel"].ToString();
            rn.spcrecvlevelto = r["spcrecvlevelto"].ToString();
            rn.spcrecvlocation = r["spcrecvlocation"].ToString();
            rn.spcprepzone = r["spcprepzone"].ToString();
            rn.spcdistzone = r["spcdistzone"].ToString();
            rn.spcdistshare = r["spcdistshare"].ToString();
            rn.spczonedelv = r["spczonedelv"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(119)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(119);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(121)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(121);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.lasrecv = (r.IsDBNull(124)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(124);
            rn.lasdelivery = (r.IsDBNull(125)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(125);
            rn.lasbatchno = r["lasbatchno"].ToString();
            rn.laslotno = r["laslotno"].ToString();
            rn.lasdatemfg = (r.IsDBNull(128)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(128);
            rn.lasdateexp = (r.IsDBNull(129)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(129);
            rn.lasserialno = r["lasserialno"].ToString();
            rn.thname = r["thname"].ToString();
            rn.hdivisionname = r["hdivisionname"].ToString();
            rn.hdepartmentname = r["hdepartmentname"].ToString();
            rn.hsubdepartname = r["hsubdepartname"].ToString();
            rn.hclassname = r["hclassname"].ToString();
            rn.hsubclassname = r["hsubclassname"].ToString();
            return rn;
        }
        private SqlCommand ixcommand(product_ix o, String sql)
        {
            SqlCommand cm = new SqlCommand(sql, cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar(o.site, "site");
            cm.snapsPar(o.depot, "depot");
            cm.snapsPar(o.spcarea, "spcarea");
            cm.snapsPar(o.article, "article");
            cm.snapsPar(o.articletype, "articletype");
            cm.snapsPar(o.pv, "pv");
            cm.snapsPar(o.lv, "lv");
            cm.snapsPar(o.description, "description");
            cm.snapsPar(o.descalt, "descalt");
            cm.snapsPar(o.thcode, "thcode");
            cm.snapsPar(o.dlcall, "dlcall");
            cm.snapsPar(o.dlcfactory, "dlcfactory");
            cm.snapsPar(o.dlcwarehouse, "dlcwarehouse");
            cm.snapsPar(o.dlcshop, "dlcshop");
            cm.snapsPar(o.dlconsumer, "dlconsumer");
            cm.snapsPar(o.hdivison, "hdivison");
            cm.snapsPar(o.hdepartment, "hdepartment");
            cm.snapsPar(o.hsubdepart, "hsubdepart");
            cm.snapsPar(o.hclass, "hclass");
            cm.snapsPar(o.hsubclass, "hsubclass");
            cm.snapsPar(o.typemanage, "typemanage");
            cm.snapsPar(o.unitmanage, "unitmanage");
            cm.snapsPar(o.unitdesc, "unitdesc");
            cm.snapsPar(o.unitreceipt, "unitreceipt");
            cm.snapsPar(o.unitprep, "unitprep");
            cm.snapsPar(o.unitsale, "unitsale");
            cm.snapsPar(o.unitstock, "unitstock");
            cm.snapsPar(o.unitweight, "unitweight");
            cm.snapsPar(o.unitdimension, "unitdimension");
            cm.snapsPar(o.unitvolume, "unitvolume");
            cm.snapsPar(o.hucode, "hucode");
            cm.snapsPar(o.rtoskuofpu, "rtoskuofpu");
            cm.snapsPar(o.rtoskuofipck, "rtoskuofipck");
            cm.snapsPar(o.rtoskuofpck, "rtoskuofpck");
            cm.snapsPar(o.rtoskuoflayer, "rtoskuoflayer");
            cm.snapsPar(o.rtoskuofhu, "rtoskuofhu");
            cm.snapsPar(o.rtopckoflayer, "rtopckoflayer");
            cm.snapsPar(o.rtolayerofhu, "rtolayerofhu");
            cm.snapsPar(o.innaturalloss, "innaturalloss");
            cm.snapsPar(o.ounaturalloss, "ounaturalloss");
            cm.snapsPar(o.costinbound, "costinbound");
            cm.snapsPar(o.costoutbound, "costoutbound");
            cm.snapsPar(o.costavg, "costavg");
            cm.snapsPar(o.skulength, "skulength");
            cm.snapsPar(o.skuwidth, "skuwidth");
            cm.snapsPar(o.skuheight, "skuheight");
            cm.snapsPar(o.skugrossweight, "skugrossweight");
            cm.snapsPar(o.skuweight, "skuweight");
            cm.snapsPar(o.skuvolume, "skuvolume");
            cm.snapsPar(o.pulength, "pulength");
            cm.snapsPar(o.puwidth, "puwidth");
            cm.snapsPar(o.puheight, "puheight");
            cm.snapsPar(o.pugrossweight, "pugrossweight");
            cm.snapsPar(o.puweight, "puweight");
            cm.snapsPar(o.puvolume, "puvolume");
            cm.snapsPar(o.ipcklength, "ipcklength");
            cm.snapsPar(o.ipckwidth, "ipckwidth");
            cm.snapsPar(o.ipckheight, "ipckheight");
            cm.snapsPar(o.ipckgrossweight, "ipckgrossweight");
            cm.snapsPar(o.ipckweight, "ipckweight");
            cm.snapsPar(o.ipckvolume, "ipckvolume");
            cm.snapsPar(o.pcklength, "pcklength");
            cm.snapsPar(o.pckwidth, "pckwidth");
            cm.snapsPar(o.pckheight, "pckheight");
            cm.snapsPar(o.pckgrossweight, "pckgrossweight");
            cm.snapsPar(o.pckweight, "pckweight");
            cm.snapsPar(o.pckvolume, "pckvolume");
            cm.snapsPar(o.layerlength, "layerlength");
            cm.snapsPar(o.layerwidth, "layerwidth");
            cm.snapsPar(o.layerheight, "layerheight");
            cm.snapsPar(o.layergrossweight, "layergrossweight");
            cm.snapsPar(o.layerweight, "layerweight");
            cm.snapsPar(o.layervolume, "layervolume");
            cm.snapsPar(o.hulength, "hulength");
            cm.snapsPar(o.huwidth, "huwidth");
            cm.snapsPar(o.huheight, "huheight");
            cm.snapsPar(o.hugrossweight, "hugrossweight");
            cm.snapsPar(o.huweight, "huweight");
            cm.snapsPar(o.huvolume, "huvolume");
            cm.snapsPar(o.isdangerous, "isdangerous");
            cm.snapsPar(o.ishighvalue, "ishighvalue");
            cm.snapsPar(o.isfastmove, "isfastmove");
            cm.snapsPar(o.isslowmove, "isslowmove");
            cm.snapsPar(o.isprescription, "isprescription");
            cm.snapsPar(o.isdlc, "isdlc");
            cm.snapsPar(o.ismaterial, "ismaterial");
            cm.snapsPar(o.isunique, "isunique");
            cm.snapsPar(o.isalcohol, "isalcohol");
            cm.snapsPar(o.istemperature, "istemperature");
            cm.snapsPar(o.isdynamicpick, "isdynamicpick");
            cm.snapsPar(o.ismixingprep, "ismixingprep");
            cm.snapsPar(o.isfinishgoods, "isfinishgoods");
            cm.snapsPar(o.isnaturalloss, "isnaturalloss");
            cm.snapsPar(o.isbatchno, "isbatchno");
            cm.snapsPar(o.ismeasurement, "ismeasurement");
            cm.snapsPar(o.roomtype, "roomtype");
            cm.snapsPar(o.tempmin, "tempmin");
            cm.snapsPar(o.tempmax, "tempmax");
            cm.snapsPar(o.alcmanage, "alcmanage");
            cm.snapsPar(o.alccategory, "alccategory");
            cm.snapsPar(o.alccontent, "alccontent");
            cm.snapsPar(o.alccolor, "alccolor");
            cm.snapsPar(o.dangercategory, "dangercategory");
            cm.snapsPar(o.dangerlevel, "dangerlevel");
            cm.snapsPar(o.stockthresholdmin, "stockthresholdmin");
            cm.snapsPar(o.stockthresholdmax, "stockthresholdmax");
            cm.snapsPar(o.spcrecvzone, "spcrecvzone");
            cm.snapsPar(o.spcrecvaisle, "spcrecvaisle");
            cm.snapsPar(o.spcrecvbay, "spcrecvbay");
            cm.snapsPar(o.spcrecvlevel, "spcrecvlevel");
            cm.snapsPar(o.spcrecvlocation, "spcrecvlocation");
            cm.snapsPar(o.spcprepzone, "spcprepzone");
            cm.snapsPar(o.spcdistzone, "spcdistzone");
            cm.snapsPar(o.spcdistshare, "spcdistshare");
            cm.snapsPar(o.spczonedelv, "spczonedelv");
            cm.snapsPar(o.orbitsource, "orbitsource");
            cm.snapsPar(o.tflow, "tflow");
            cm.snapsPar(o.fileid, "fileid");
            cm.snapsPar(o.rowops, "rowops");
            cm.snapsPar(o.datecreate, "datecreate");
            cm.snapsPar(o.dateops, "dateops");
            cm.snapsPar(o.ermsg, "ermsg");
            return cm;
        }
        private SqlCommand obcommand(product_md o, String sql)
        {
            SqlCommand cm = new SqlCommand(sql, cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar(o.site, "site");
            cm.snapsPar(o.depot, "depot");
            cm.snapsPar(o.spcarea, "spcarea");
            cm.snapsPar(o.article, "article");
            cm.snapsPar(o.articletype, "articletype");
            cm.snapsPar(o.pv, "pv");
            cm.snapsPar(o.lv, "lv");
            cm.snapsPar(o.description, "description");
            cm.snapsPar(o.descalt, "descalt");
            cm.snapsPar(o.thcode, "thcode");
            cm.snapsPar(o.dlcall, "dlcall");
            cm.snapsPar(o.dlcfactory, "dlcfactory");
            cm.snapsPar(o.dlcwarehouse, "dlcwarehouse");
            cm.snapsPar(o.dlcshop, "dlcshop");
            cm.snapsPar(o.dlconsumer, "dlconsumer");
            cm.snapsPar(o.hdivison, "hdivison");
            cm.snapsPar(o.hdepartment, "hdepartment");
            cm.snapsPar(o.hsubdepart, "hsubdepart");
            cm.snapsPar(o.hclass, "hclass");
            cm.snapsPar(o.hsubclass, "hsubclass");
            cm.snapsPar(o.typemanage, "typemanage");
            cm.snapsPar(o.unitmanage, "unitmanage");
            cm.snapsPar(o.unitdesc, "unitdesc");
            cm.snapsPar(o.unitreceipt, "unitreceipt");
            cm.snapsPar(o.unitprep, "unitprep");
            cm.snapsPar(o.unitsale, "unitsale");
            cm.snapsPar(o.unitstock, "unitstock");
            cm.snapsPar(o.unitweight, "unitweight");
            cm.snapsPar(o.unitdimension, "unitdimension");
            cm.snapsPar(o.unitvolume, "unitvolume");
            cm.snapsPar(o.hucode, "hucode");
            cm.snapsPar(o.rtoskuofpu, "rtoskuofpu");
            cm.snapsPar(o.rtoskuofipck, "rtoskuofipck");
            cm.snapsPar(o.rtoskuofpck, "rtoskuofpck");
            cm.snapsPar(o.rtoskuoflayer, "rtoskuoflayer");
            cm.snapsPar(o.rtoskuofhu, "rtoskuofhu");
            cm.snapsPar(o.rtoipckofpck, "rtoipckofpck");
            cm.snapsPar(o.rtopckoflayer, "rtopckoflayer");
            cm.snapsPar(o.rtolayerofhu, "rtolayerofhu");
            cm.snapsPar(o.innaturalloss, "innaturalloss");
            cm.snapsPar(o.ounaturalloss, "ounaturalloss");
            cm.snapsPar(o.costinbound, "costinbound");
            cm.snapsPar(o.costoutbound, "costoutbound");
            cm.snapsPar(o.costavg, "costavg");
            cm.snapsPar(o.skulength, "skulength");
            cm.snapsPar(o.skuwidth, "skuwidth");
            cm.snapsPar(o.skuheight, "skuheight");
            cm.snapsPar(o.skugrossweight, "skugrossweight");
            cm.snapsPar(o.skuweight, "skuweight");
            cm.snapsPar(o.skuvolume, "skuvolume");
            cm.snapsPar(o.pulength, "pulength");
            cm.snapsPar(o.puwidth, "puwidth");
            cm.snapsPar(o.puheight, "puheight");
            cm.snapsPar(o.pugrossweight, "pugrossweight");
            cm.snapsPar(o.puweight, "puweight");
            cm.snapsPar(o.puvolume, "puvolume");
            cm.snapsPar(o.ipcklength, "ipcklength");
            cm.snapsPar(o.ipckwidth, "ipckwidth");
            cm.snapsPar(o.ipckheight, "ipckheight");
            cm.snapsPar(o.ipckgrossweight, "ipckgrossweight");
            cm.snapsPar(o.ipckweight, "ipckweight");
            cm.snapsPar(o.ipckvolume, "ipckvolume");
            cm.snapsPar(o.pcklength, "pcklength");
            cm.snapsPar(o.pckwidth, "pckwidth");
            cm.snapsPar(o.pckheight, "pckheight");
            cm.snapsPar(o.pckgrossweight, "pckgrossweight");
            cm.snapsPar(o.pckweight, "pckweight");
            cm.snapsPar(o.pckvolume, "pckvolume");
            cm.snapsPar(o.layerlength, "layerlength");
            cm.snapsPar(o.layerwidth, "layerwidth");
            cm.snapsPar(o.layerheight, "layerheight");
            cm.snapsPar(o.layergrossweight, "layergrossweight");
            cm.snapsPar(o.layerweight, "layerweight");
            cm.snapsPar(o.layervolume, "layervolume");
            cm.snapsPar(o.hulength, "hulength");
            cm.snapsPar(o.huwidth, "huwidth");
            cm.snapsPar(o.huheight, "huheight");
            cm.snapsPar(o.hugrossweight, "hugrossweight");
            cm.snapsPar(o.huweight, "huweight");
            cm.snapsPar(o.huvolume, "huvolume");
            cm.snapsPar(o.isdangerous, "isdangerous");
            cm.snapsPar(o.ishighvalue, "ishighvalue");
            cm.snapsPar(o.isfastmove, "isfastmove");
            cm.snapsPar(o.isslowmove, "isslowmove");
            cm.snapsPar(o.isprescription, "isprescription");
            cm.snapsPar(o.isdlc, "isdlc");
            cm.snapsPar(o.ismaterial, "ismaterial");
            cm.snapsPar(o.isunique, "isunique");
            cm.snapsPar(o.isalcohol, "isalcohol");
            cm.snapsPar(o.istemperature, "istemperature");
            cm.snapsPar(o.isdynamicpick, "isdynamicpick");
            cm.snapsPar(o.ismixingprep, "ismixingprep");
            cm.snapsPar(o.isfinishgoods, "isfinishgoods");
            cm.snapsPar(o.isnaturalloss, "isnaturalloss");
            cm.snapsPar(o.isbatchno, "isbatchno");
            cm.snapsPar(o.ismeasurement, "ismeasurement");
            cm.snapsPar(o.roomtype, "roomtype");
            cm.snapsPar(o.tempmin, "tempmin");
            cm.snapsPar(o.tempmax, "tempmax");
            cm.snapsPar(o.alcmanage, "alcmanage");
            cm.snapsPar(o.alccategory, "alccategory");
            cm.snapsPar(o.alccontent, "alccontent");
            cm.snapsPar(o.alccolor, "alccolor");
            cm.snapsPar(o.dangercategory, "dangercategory");
            cm.snapsPar(o.dangerlevel, "dangerlevel");
            cm.snapsPar(o.stockthresholdmin, "stockthresholdmin");
            cm.snapsPar(o.stockthresholdmax, "stockthresholdmax");
            cm.snapsPar(o.spcrecvzone, "spcrecvzone");
            cm.snapsPar(o.spcrecvaisle, "spcrecvaisle");
            cm.snapsPar(o.spcrecvaisleto, "spcrecvaisleto");
            cm.snapsPar(o.spcrecvbay, "spcrecvbay");
            cm.snapsPar(o.spcrecvbayto, "spcrecvbayto");
            cm.snapsPar(o.spcrecvlevel, "spcrecvlevel");
            cm.snapsPar(o.spcrecvlevelto, "spcrecvlevelto");
            cm.snapsPar(o.spcrecvlocation, "spcrecvlocation");
            cm.snapsPar(o.spcprepzone, "spcprepzone");
            cm.snapsPar(o.spcdistzone, "spcdistzone");
            cm.snapsPar(o.spcdistshare, "spcdistshare");
            cm.snapsPar(o.spczonedelv, "spczonedelv");
            cm.snapsPar(o.orbitsource, "orbitsource");
            cm.snapsPar(o.tflow, "tflow");
            cm.snapsPar(o.datecreate, "datecreate");
            cm.snapsPar(o.accncreate, "accncreate");
            cm.snapsPar(o.datemodify, "datemodify");
            cm.snapsPar(o.accnmodify, "accnmodify");
            cm.snapsPar(o.procmodify, "procmodify");
            cm.snapsPar(o.lasrecv, "lasrecv");
            cm.snapsPar(o.lasdelivery, "lasdelivery");
            cm.snapsPar(o.lasbatchno, "lasbatchno");
            cm.snapsPar(o.laslotno, "laslotno");
            cm.snapsPar(o.lasdatemfg, "lasdatemfg");
            cm.snapsPar(o.lasdateexp, "lasdateexp");
            cm.snapsPar(o.lasserialno, "lasserialno");
            cm.snapsParsysdateoffset();
            return cm;
        }

        public SqlCommand oucommand(product_ls o, String sql, String accnmodify)
        {
            SqlCommand cm = new SqlCommand(sql, cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar(o.site, "site");
            cm.snapsPar(o.depot, "depot");
            cm.snapsPar(o.spcarea, "spcarea");
            cm.snapsPar(o.article, "article");
            cm.snapsPar(o.pv, "pv");
            cm.snapsPar(o.lv, "lv");
            return cm;
        }
        public SqlCommand oucommand(product_md o, String sql)
        {
            SqlCommand cm = new SqlCommand(sql, cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar(o.site, "site");
            cm.snapsPar(o.depot, "depot");
            cm.snapsPar(o.spcarea, "spcarea");
            cm.snapsPar(o.article, "article");
            cm.snapsPar(o.pv, "pv");
            cm.snapsPar(o.lv, "lv");
            return cm;
        }
    }
}