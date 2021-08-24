using System;
using System.Collections.Generic;
using System.Text;

namespace Snaps.WMS{
    public partial class mergehu_ops {
        public readonly string sqlmergehu_insert = @"INSERT INTO wm_mergehu ( orgcode,site,depot,spcarea,hutype,hutarget,loccode,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,remarks ) 
         VALUES (@orgcode,@site,@depot,@spcarea,@hutype,@hutarget,@loccode,@tflow,@datecreate,@accncreate,@datemodify,@accnmodify,@procmodify,@remarks); select cast(SCOPE_IDENTITY() as int) as mergeno";

        public readonly string sqlmergehu_update = @"UPDATE wm_mergehu  SET tflow = @tflow, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, remarks = @remarks
        WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea  AND mergeno = @mergeno";

        public readonly string sqlmergehu_cancel =
			@"DELETE FROM wm_handerlingunit where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno;
			  UPDATE wm_mergeln SET tflow = @tflow, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea  AND mergeno = @mergeno and tflow='IO';
              UPDATE wm_mergehu SET tflow = @tflow, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea  AND mergeno = @mergeno and tflow='IO'";

        public readonly string sqlmergehu_chkloc = "select count(1) from wm_locdw wl where orgcode = @orgcode and site = @site and depot = @depot and lscode =@loccode and tflow = 'IO'";
		public readonly string sqlmergeln_mergelist =
			@"select  orgcode, site, depot, spcarea, mergeno, hutype, hutarget, loccode, tflow,dbo.get_flowdes(orgcode,site,depot,tflow) tflowdes, datecreate, accncreate, datemodify, accnmodify, procmodify, remarks, opstype FROM wm_mergehu 
			  where orgcode = @orgcode and site = @site and depot = @depot ";// and loccode =@loccode and hutarget like '%' + @huno
		public readonly string sqlmergeln_mergeline =
			@"SELECT mergeln, mergeno, orgcode, site, depot, spcarea, stockid, hutype, loccode, huno, inrefno, inrefln, inagrn, ingrno, article, pv, lv, descalt, qtysku, qtypu, 
			qtyweight, qtyvolume, qtyunit, qtyunitdes, daterec, batchno, lotno, datemfg, serialno, dateexp, tflowops, tflowdes, tflowsign, skuops, puops, weightops, 
			volumeops, unitops, unitopsdes, refops, reflnops, remarks,dbo.get_flowdes(orgcode,site,depot,tflow) tflow, datecreate, accncreate, datemodify, accnmodify, procmodify, msgops 
			FROM wm_mergeln where orgcode = @orgcode and site = @site and depot = @depot and mergeno =@mergeno";

		public readonly string sqlmergeln_find =
			@"select 0 mergeln, 0 mergeno,s.orgcode,s.site,s.depot,s.spcarea,s.stockid,s.hutype,s.loccode,s.huno,s.inrefno,
				s.inrefln,s.inagrn,s.ingrno,s.article,s.pv,s.lv,p.descalt,s.qtysku,s.qtypu,
				s.qtyweight,s.qtyvolume,s.unitops qtyunit,dbo.get_unitdes(s.orgcode,s.site,s.depot,s.unitops) qtyunitdes,
				s.daterec,s.batchno,s.lotno,s.datemfg,s.serialno,s.dateexp, 
				s.tflow tflowops,dbo.get_flowdes(s.orgcode,s.site,s.depot,s.tflow) tflowdes ,
				case when l.spcarea in ('BL') and s.tflow = 'IO' then 'bulknrtn' 
					when l.spcarea in ('OV') and s.tflow = 'IO' then 'overflow' 
					when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'staging' 
					when l.spcarea = 'RN' and s.tflow = 'IO' then 'rtv' 
					when l.spcarea = 'SB' and s.tflow = 'IO' then 'sinbin' 
					when l.spcarea = 'DM' and s.tflow = 'IO' then 'damage' 
					when l.spcarea = 'XC' and s.tflow = 'IO' then 'exchange' 
					when l.spcarea = 'ST' and l.lsloctype = 'LC' and l.spcpicking = 1 then 'picking' 
					when l.spcarea = 'ST' and l.lsloctype = 'LC' and l.spcpicking = 0 then 'reserve' 
					when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'block' 
					else 'notfound' 
				end tflowsign,
				s.qtysku skuops,
				cast(s.qtysku as decimal) /(case when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 1 then p.rtoskuofpu
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 2 then p.rtoskuofipck
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 3 then p.rtoskuofpck
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 4 then p.rtoskuoflayer
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 5 then p.rtoskuofhu
								else 1 end ) puops,
 				s.qtysku /(case when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 1 then p.rtoskuofpu
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 2 then p.rtoskuofipck
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 3 then p.rtoskuofpck
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 4 then p.rtoskuoflayer
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 5 then p.rtoskuofhu
								else 1  end) * p.skugrossweight as weightops,
				s.qtysku /(case when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 1 then p.rtoskuofpu
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 2 then p.rtoskuofipck
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 3 then p.rtoskuofpck
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 4 then p.rtoskuoflayer
								when iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) = 5 then p.rtoskuofhu
							else 1 end) * p.skuvolume  as volumeops,
				iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage) unitops, 
				dbo.get_unitdes(s.orgcode,s.site,s.depot,iif(l.spcpicking = 1,isnull(l.spcpickunit,p.unitmanage),p.unitmanage)) unitopsdes,
				s.inrefno refops,
				s.inrefln reflnops,
				'' remarks,
				'IO' tflow,
				SYSDATETIMEOFFSET() as datecreate ,
				'wms.mergehu' as accncreate,
				SYSDATETIMEOFFSET() as datemodify,
				'wms.mergehu' as accnmodify,
				'wms.mergehu' as procmodify,
				'' msgops
			from wm_stock s 
				join wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode 
				join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
			where s.qtysku > 0 and s.tflow = 'IO' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.loccode = @loccode and s.huno like '%' + @huno
			and not EXISTS(select 1 from wm_stobc b where b.orgcode = s.orgcode and b.site = s.site and b.depot = s.depot and b.stockid = s.stockid and b.huno = s.huno and b.tflow='IO')
			and not EXISTS(select 1 from wm_taln t where t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.stockid = s.stockid and t.sourcehuno = s.huno and t.tflow='IO')
			and not EXISTS(select 1 from wm_mergeln m where s.orgcode = m.orgcode and s.site = m.site and s.depot = m.depot and s.stockid = m.stockid and s.loccode = m.loccode and s.huno = m.huno)";
		public readonly string sqlmergeln_insert =
			@"INSERT INTO wm_mergeln (mergeno,orgcode,site,depot,spcarea,stockid,hutype,loccode,huno,inrefno,inrefln,inagrn,ingrno,article,pv,lv,descalt,qtysku,qtypu,qtyweight,qtyvolume,qtyunit,qtyunitdes,daterec,batchno,lotno,datemfg,
			serialno,dateexp,tflowops,tflowdes,tflowsign,skuops,puops,weightops,volumeops,unitops,unitopsdes,refops,reflnops,remarks,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,msgops ) 
			VALUES (@mergeno,@orgcode,@site,@depot,@spcarea,@stockid,@hutype,@loccode,@huno,@inrefno,@inrefln,@inagrn,@ingrno,@article,@pv,@lv,@descalt,@qtysku,@qtypu,@qtyweight,@qtyvolume,@qtyunit,
			@qtyunitdes,@daterec,@batchno,@lotno,@datemfg,@serialno,@dateexp,@tflowops,@tflowdes,@tflowsign,@skuops,@puops,@weightops,@volumeops,@unitops,@unitopsdes,@refops,@reflnops,@remarks,'IO',SYSDATETIMEOFFSET(),
			@accncreate,SYSDATETIMEOFFSET(),@accnmodify,@procmodify,@msgops)";
	}
}
