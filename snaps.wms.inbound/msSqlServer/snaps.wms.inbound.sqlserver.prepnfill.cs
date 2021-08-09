using System;
using System.Data.SqlClient;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.Collections.Generic;
namespace Snaps.WMS {

    public partial class inbound_ops : IDisposable { 

        //Fill 
        private inbound_ls fillls(ref SqlDataReader r) { 
            return new inbound_ls() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                thcode = r["thcode"].ToString(),
                subtype = r["subtype"].ToString(),
                inorder = r["inorder"].ToString(),
                dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8),
                dateplan = (r.IsDBNull(31)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(31),
                dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10),
                slotdate = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11),
                slotno = r["slotno"].ToString(),
                inpriority = (r.IsDBNull(13)) ? 0 : r.GetInt32(13),
                inpromo = r["inpromo"].ToString(),
                tflow = r["tflow"].ToString(),
                daterec = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17),
                thname = r["thname"].ToString(),
                dateremarks = r["dateremarks"].ToString(),
                remarkrec = r["remarkrec"].ToString(),
                isreqmeasurement = r["ismeasurement"].ToString().CInt32(),
                opsprogress = r["opsprogress"].ToString().CInt32()
            };
        }
        private inbound_ix fillix(ref SqlDataReader r) { 
            return new inbound_ix() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                thcode = r["thcode"].ToString(),
                intype = r["intype"].ToString(),
                subtype = r["subtype"].ToString(),
                inorder = r["inorder"].ToString(),
                dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8),
                dateplan = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9),
                dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10),
                slotdate = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11),
                slotno = r["slotno"].ToString(),
                inpriority = r.GetInt32(13),
                inflag = r["inflag"].ToString(),
                inpromo = r["inpromo"].ToString(),
                tflow = r["tflow"].ToString(),
                fileid = r["fileid"].ToString(),
                rowops = r["rowops"].ToString(),
                ermsg = r["ermsg"].ToString(),
                dateops = (r.IsDBNull(20)) ? (DateTime?) null : r.GetDateTime(20),
            };
        }
        private inbound_md fillmdl(ref SqlDataReader r) { 
            return new inbound_md() {
                    orgcode = r["orgcode"].ToString(),
                    site = r["site"].ToString(),
                    depot = r["depot"].ToString(),
                    spcarea = r["spcarea"].ToString(),
                    thcode = r["thcode"].ToString(),
                    intype = r["intype"].ToString(),
                    subtype = r["subtype"].ToString(),
                    inorder = r["inorder"].ToString(),
                    dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8),
                    dateplan = (r.IsDBNull(31)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(31),
                    dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10),
                    slotdate = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11),
                    slotno = r["slotno"].ToString(),
                    inpriority =  (r.IsDBNull(13)) ? 0 :r.GetInt32(13),
                    inflag = r["inflag"].ToString(),
                    inpromo = r["inpromo"].ToString(),
                    tflow = r["tflow"].ToString(),
                    daterec = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17),
                    dockrec = r["dockrec"].ToString(),
                    invno = r["invno"].ToString(),
                    remarkrec = r["remarkrec"].ToString(),
                    datecreate = (r.IsDBNull(21)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(21),
                    accncreate = r["accncreate"].ToString(),
                    datemodify = (r.IsDBNull(23)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(23),
                    accnmodify = r["accnmodify"].ToString(),
                    procmodify = r["procmodify"].ToString(),
                    orbitsource = r["orbitsource"].ToString(),
                    dateassign = (r.IsDBNull(27)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(27),
                    dateunloadstart = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28),
                    dateunloadend = (r.IsDBNull(29)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(29),
                    datefinish = (r.IsDBNull(30)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(30),
                    thname = r["thname"].ToString(),
                    pendinginf = r["pendingInf"].ToString().CInt32(),
                    waitconfirm = r["waitconfirm"].ToString().CInt32()
            };
        }
        private inbouln_md filllnmdl(ref SqlDataReader r) { 
            inbouln_md rn = new inbouln_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.inorder = r["inorder"].ToString();
            rn.inln = r["inln"].ToString();
            rn.inrefno = r["inrefno"].ToString();
            rn.inrefln = r.GetInt32(7);
            rn.inagrn = r["inagrn"].ToString();
            rn.barcode = r["barcode"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(11)) ? 0 :r.GetInt32(11);
            rn.lv = (r.IsDBNull(12)) ? 0 :  r.GetInt32(12);
            rn.unitops = r["unitops"].ToString();
            rn.qtysku = (r.IsDBNull(14)) ? 0 : r.GetInt32(14);
            rn.qtypu = (r.IsDBNull(r.GetOrdinal("qtypu"))) ? 0 : r["qtypu"].ToString().CDecimal();
            rn.qtyweight = (r.IsDBNull(16)) ? 0 : r.GetDecimal(16);
            rn.lotno = r["lotno"].ToString();
            rn.expdate = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18);
            rn.serialno = r["serialno"].ToString();
            rn.qtypnd = (r.IsDBNull(r.GetOrdinal("qtypnd"))) ? 0 : r["qtypnd"].ToString().CDecimal();
            rn.qtyskurec = (r.IsDBNull(21)) ? 0 : r.GetInt32(21);
            rn.qtypurec = (r.IsDBNull(22)) ? 0 : r.GetInt32(22);
            rn.qtyweightrec = (r.IsDBNull(23)) ? 0 : r.GetDecimal(23);
            rn.qtynaturalloss = (r.IsDBNull(24)) ? 0 : r.GetDecimal(24);
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(26)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(26);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.description = r["descalt"].ToString();
            rn.unitopsdesc = r["unitopsdesc"].ToString(); 
            rn.inseq = (r.IsDBNull(34)) ? 0 : r.GetInt32(34);
            rn.isdlc = (r.IsDBNull(37)) ? 0 : r.GetInt32(37);
            rn.isunique = (r.IsDBNull(38)) ? 0 : r.GetInt32(38);
            rn.ismixingprep = (r.IsDBNull(39)) ? 0 : r.GetInt32(39);
            rn.isbatchno = (r.IsDBNull(40)) ? 0 : r.GetInt32(40);
            rn.dlcall = (r.GetInt32(r.GetOrdinal("dlcall"))); //=> add by nai
            rn.dlcfactory = (r.IsDBNull(41)) ? 0 : r.GetInt32(41);
            rn.dlcwarehouse = (r.IsDBNull(42)) ? 0 : r.GetInt32(42);
            rn.unitreceipt = r["unitreceipt"].ToString();
            rn.innaturalloss = (r.IsDBNull(44)) ? 0 : r.GetDecimal(44);
            rn.skulength = (r.IsDBNull(45)) ? 0 : r.GetDecimal(45);
            rn.skuwidth = (r.IsDBNull(46)) ? 0 : r.GetDecimal(46);
            rn.skuheight = (r.IsDBNull(47)) ? 0 : r.GetDecimal(47);
            rn.skuweight = (r.IsDBNull(48)) ? 0 : r.GetDecimal(48);
            rn.tihi = r["tihi"].ToString();
            rn.laslotno = r["laslotno"].ToString();
            rn.lasbatchno =r["lasbatchno"].ToString();
            rn.lasdatemfg = (r.IsDBNull(51)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(51);
            rn.lasdateexp = (r.IsDBNull(52)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(52);
            rn.lasserialno = r["lasserialno"].ToString();
            rn.rtoskuofipck = (r.IsDBNull(54)) ? 0 : r.GetInt32(54);
            rn.rtoskuofpck = (r.IsDBNull(55)) ? 0 : r.GetInt32(55);
            rn.rtoskuoflayer = (r.IsDBNull(56)) ? 0 : r.GetInt32(56);
            rn.rtoskuofhu = (r.IsDBNull(57)) ? 0 : r.GetInt32(57);
            rn.huestimate = (r.IsDBNull(58)) ? 0 : r.GetDecimal(58);

            return rn;
        }
        private inbound_hs fillhs(ref SqlDataReader r) {
            return new inbound_hs(){ 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                inorder = r["inorder"].ToString(),
                inln = r["inln"].ToString(),
                inrefno = r["inrefno"].ToString(),
                inrefln = r["inrefln"].ToString(), 
                barcode = r["barcode"].ToString(),
                article = r["article"].ToString(),
                pv = (r.IsDBNull(11)) ? 0 : r.GetInt32(11),
                lv = (r.IsDBNull(12)) ? 0 : r.GetInt32(12),
                unitops = r["unitops"].ToString(),
                qtyskurec = (r.IsDBNull(14)) ? 0 : r.GetInt32(14),
                qtypurec = (r.IsDBNull(15)) ? 0 : r.GetInt32(15),
                qtyweightrec = (r.IsDBNull(16)) ? 0 : r.GetDecimal(16),
                qtynaturalloss = (r.IsDBNull(17)) ? 0 : r.GetDecimal(17),
                daterec = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18),
                datemfg = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19),
                dateexp = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20),
                batchno = r["batchno"].ToString(),
                lotno = r["lotno"].ToString(),
                serialno = r["serialno"].ToString(),
                datecreate = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(26)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(26),
                accnmodify = r["accnmodify"].ToString(),
                thnamealt = r["thnamealt"].ToString(),
                descalt = r["descalt"].ToString(),
                inagrn = r["inagrn"].ToString(),
                ingrno = r["ingrno"].ToString(),
                qtyhurec = r["qtyhurec"].ToString().CInt32()
            };
        }
        private inboulx lxfill(ref SqlDataReader r) {
            inboulx rn = new inboulx();
            
            rn.inlx = r.GetDecimal(0);
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.inorder = r["inorder"].ToString();
            rn.inln =r["inln"].ToString();
            rn.inrefno = r["inrefno"].ToString();
            rn.inrefln = r["inrefln"].ToString();
            rn.barcode = r["barcode"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(11)) ? 0 : r.GetInt32(11);
            rn.lv = (r.IsDBNull(12)) ? 0 : r.GetInt32(12);
            rn.unitops = r["unitops"].ToString();
            rn.qtyskurec = (r.IsDBNull(14)) ? 0 : r.GetInt32(14);
            rn.qtypurec = (r.IsDBNull(15)) ? 0 : r.GetInt32(15);
            rn.qtyweightrec = (r.IsDBNull(16)) ? 0 : r.GetDecimal(16);
            rn.qtynaturalloss = (r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.daterec = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18);
            rn.datemfg = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
            rn.dateexp = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.serialno = r["serialno"].ToString();
            rn.datecreate = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(26)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(26);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.tflow = r["tflow"].ToString();                    
            rn.rtohu =  (r.IsDBNull(34)) ? 0 : r.GetInt32(34);
            rn.rtopu =  (r.IsDBNull(32)) ? 0 : r.GetInt32(32);
            rn.dockno = r["dockrec"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.intype = r["intype"].ToString();
            rn.insubtype = r["insubtype"].ToString();
            rn.inpromo = r["inpromo"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();      
            rn.inagrn = r["inagrn"].ToString();
            rn.skuweight =  (r.IsDBNull(40)) ? 0 : r["skuweight"].ToString().CDecimal();
            rn.skuvolume =  (r.IsDBNull(42)) ? 0 :  r["skuvolume"].ToString().CDecimal();
            rn.skucubic =  (r.IsDBNull(43)) ? 0 :  r["skucubic"].ToString().CDecimal();
            rn.qtyhurec = r["qtyhurec"].ToString().CInt32();

            return rn;
        }

        private task_md filltask(stock_mvin ln, string ointype) { 
            return new task_md() {     
                accncreate = ln.opsaccn,
                accnmodify = ln.opsaccn,
                datecreate = new DateTimeOffset(),
                dateend = null,
                datemodify = new DateTimeOffset(),
                datestart = null,
                depot = ln.depot,
                iopromo = "",
                iorefno = ln.opsrefno,
                lines = new List<taln_md>(),
                orgcode = ln.orgcode,
                priority = "0",
                procmodify = "Inbound",
                site = ln.site,
                spcarea = ln.spcarea,
                taskdate = new DateTimeOffset(),
                taskno = "",
                tasktype = "P",
                taskname = "Putaway",
                tflow = "IO",
                intype = ointype
            };
        }
        private taln_md filltaskln(stock_mvin ln){ 
            return new taln_md() {
                accnassign = "",
                accncollect = "",
                accncreate = ln.opsaccn,
                accnfill = "",
                accnmodify = ln.opsaccn,
                accnwork = "",
                article = ln.article,
                collecthuno = "",
                collectloc = "",
                collectqty = 0,
                dateassign = new DateTimeOffset(),
                datecollect = new DateTimeOffset(),
                datecreate = new DateTimeOffset(),
                dateexp = ln.dateexp,
                datefill = new DateTimeOffset(),
                datemfg = ln.datemfg,
                datemodify = new DateTimeOffset(),
                datework = new DateTimeOffset(),
                depot = ln.depot,
                iopromo = "",
                iorefno = ln.opsrefno,
                ioreftype = "Inb.order",
                lotno = ln.lotno,
                lv = ln.lv,
                orgcode = ln.orgcode,
                procmodify = ln.procmodify,
                pv = ln.pv,
                serialno = ln.serialno,
                site = ln.site,
                sourcehuno = ln.opshuno,
                sourceloc = ln.opsloccode,
                spcarea = ln.spcarea,
                targetadv = "",
                targethuno = "",
                targetloc = "",
                targetqty = 0,
                taskno = "",
                taskseq = 1,
                tflow = "IO",
                sourceqty = ln.opspu,
                sourcevolume = ln.opsvolume,
                stockid = ln.stockid,
                sourceweight = ln.opsweight,
                thcode = ln.opsthcode
            };
        }

        //Fill stokc
        private stock_mvin getStockIn(inboulx lx) {
            stock_mvin rn = new stock_mvin(); 
            rn.orgcode = lx.orgcode;
            rn.site = lx.site;
            rn.depot = lx.depot;
            rn.spcarea = lx.spcarea;
            rn.stockid = 0;
            rn.opsrefno = lx.inorder;
            rn.article = lx.article;
            rn.pv = lx.pv;
            rn.lv = lx.lv;
            rn.opsunit = lx.unitops;
            rn.opssku = lx.qtyskurec;
            rn.opspu = lx.qtypurec;
            rn.opsweight = lx.qtyweightrec;
            rn.opsvolume = 0;
            rn.opsnaturalloss = lx.qtynaturalloss;
            rn.daterec = lx.daterec;
            rn.datemfg = lx.datemfg;
            rn.dateexp = lx.dateexp;
            rn.batchno = lx.batchno;
            rn.lotno = lx.lotno;
            rn.serialno = lx.serialno;
            rn.procmodify = "inb.receipt";
            rn.opsloccode = lx.dockno;
            rn.lotno = lx.lotno;
            rn.inagrn = lx.inagrn;
            rn.ingrno = lx.ingrno;
            rn.opstype = "+";
            rn.opsrefno = lx.inorder;
            rn.opsthcode = lx.thcode;
            return rn;
        }
        
        //Prep
        private SqlCommand ixcommand(inbound_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.intype,"intype");
            cm.snapsPar(o.subtype,"subtype");
            cm.snapsPar(o.inorder,"inorder");
            cm.snapsPar(o.dateorder,"dateorder");
            cm.snapsPar(o.dateplan,"dateplan");
            cm.snapsPar(o.dateexpire,"dateexpire");
            cm.snapsPar(o.slotdate,"slotdate");
            cm.snapsPar(o.slotno,"slotno");
            cm.snapsPar(o.inpriority,"inpriority");
            cm.snapsPar(o.inflag,"inflag");
            cm.snapsPar(o.inpromo,"inpromo");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.fileid,"fileid");
            cm.snapsPar(o.rowops,"rowops");
            cm.snapsPar(o.ermsg,"ermsg");
            cm.snapsPar(o.dateops,"dateops");
            return cm;
        }
        private SqlCommand obcommand(inbound_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.intype,"intype");
            cm.snapsPar(o.subtype,"subtype");
            cm.snapsPar(o.inorder,"inorder");
            cm.snapsPar(o.dateorder,"dateorder");
            cm.snapsPar(o.dateplan,"dateplan");
            cm.snapsPar(o.dateexpire,"dateexpire");
            cm.snapsPar(o.slotdate,"slotdate");
            cm.snapsPar(o.slotno,"slotno");
            cm.snapsPar(o.inpriority,"inpriority");
            cm.snapsPar(o.inflag,"inflag");
            cm.snapsPar(o.inpromo,"inpromo");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.daterec,"daterec");
            cm.snapsPar(o.dockrec,"dockrec");
            cm.snapsPar(o.invno,"invno");
            //cm.snapsPar(o.remarkrec,"remarkrec");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsParsysdateoffset();
            return cm;
        }
        private SqlCommand lxcomamnd (inboulx o) { 
            SqlCommand cm = new SqlCommand("",cn);
            cm.snapsPar(o.inlx,"inlx");
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.inorder,"inorder");
            cm.snapsPar(o.inln,"inln");
            cm.snapsPar(o.inrefno,"inrefno");
            cm.snapsPar(o.inrefln,"inrefln");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitops,"unitops");
            cm.snapsPar(o.qtyskurec,"qtyskurec");
            cm.snapsPar(o.qtypurec,"qtypurec");
            cm.snapsPar(o.qtyweightrec,"qtyweightrec");
            cm.snapsPar(o.qtynaturalloss,"qtynaturalloss");
            cm.snapsPar(o.daterec,"daterec");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.qtyhurec,"qtyhurec");
            cm.snapsPar(o.ingrno,"ingrno");
            cm.snapsPar(o.inagrn,"inagrn");
            cm.snapsPar(o.inseq,"inseq");
            return cm; 
        }
        
        //Lending to orbit 
        private orbit_receipt orbit_receipt(inboulx s){ 
            orbit_receipt t = new orbit_receipt(); 
            t.orgcode = s.orgcode;
            t.site = s.site;
            t.depot = s.depot;
            t.thcode = s.thcode;
            t.intype = s.intype;
            t.insubtype = s.insubtype;
            t.inorder = s.inorder;
            t.inln = s.inln;
            t.ingrno = s.ingrno;
            t.inrefno = s.inrefno;
            t.inrefln = s.inrefln;
            t.barcode = s.barcode;
            t.article = s.article;
            t.pv = s.pv;
            t.lv = s.lv;
            t.unitops = s.unitops;
            t.qtysku = s.qtyskurec;
            t.qtypu = s.qtypurec;
            t.qtyweight = s.qtyweightrec;
            t.qtyvolume = s.qtyvolumerec;
            t.qtynaturalloss = s.qtynaturalloss;
            t.dateops = DateTimeOffset.Now;
            t.accnops = s.accnmodify;
            t.dateexp = s.dateexp;
            t.datemfg = s.datemfg;
            t.batchno = s.batchno;
            t.lotmfg = s.lotno;
            t.serialno = s.serialno;
            t.huno = "";
            t.inpromo = s.inpromo;
            t.orbitsource = s.orbitsource;
            t.inagrn = s.inagrn;
            return t;
        }
    }
}